/* Copyright (c) 2008-2012 Peter Palotas, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Alphaleonis.Win32.Filesystem
{
   internal static partial class NativeMethods
   {
      internal static partial class DeviceIo
      {
         #region GetLinkTargetInfo

         [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
         [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
         [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
         [SecurityCritical]
         public static LinkTargetInfo GetLinkTargetInfo(SafeFileHandle handle)
         {
            // Start with a large buffer to prevent a 2nd call.
            uint bytesReturned = MaxPathUnicode;
            SafeGlobalMemoryBufferHandle safeBuffer = null;

            try
            {
               safeBuffer = new SafeGlobalMemoryBufferHandle((int)bytesReturned);

               do
               {
                  // Possible PInvoke signature bug: safeBuffer.Capacity and bytesReturned are always the same.
                  // Since we use a large buffer, we're not affected.

                  if (!DeviceIoControl(handle, DeviceIoControlCode.FsctlGetReparsePoint, IntPtr.Zero, 0, safeBuffer, (uint)safeBuffer.Capacity, out bytesReturned, IntPtr.Zero))
                  {
                     int lastError = Marshal.GetLastWin32Error();
                     switch ((uint)lastError)
                     {
                        case Win32Errors.ERROR_MORE_DATA:
                        case Win32Errors.ERROR_INSUFFICIENT_BUFFER:
                           if (safeBuffer.Capacity < bytesReturned)
                           {
                              safeBuffer.Dispose();
                              break;
                           }
                           NativeError.ThrowException(lastError);
                           break;
                     }
                  }
                  else
                     break;
               } while (true);


               IntPtr bufPtr = safeBuffer.DangerousGetHandle();
               Type toMountPointReparseBuffer = typeof(MountPointReparseBuffer);
               Type toReparseDataBufferHeader = typeof(ReparseDataBufferHeader);
               Type toSymbolicLinkReparseBuffer = typeof(SymbolicLinkReparseBuffer);
               IntPtr marshalReparseBuffer = Marshal.OffsetOf(toReparseDataBufferHeader, "data");

               ReparseDataBufferHeader header = GetStructure<ReparseDataBufferHeader>(0, bufPtr);
               
               IntPtr dataPos;
               byte[] dataBuffer;

               switch (header.ReparseTag)
               {
                  case ReparsePointTags.MountPoint:
                     MountPointReparseBuffer mprb = GetStructure<MountPointReparseBuffer>(0, new IntPtr(bufPtr.ToInt64() + marshalReparseBuffer.ToInt64()));

                     dataPos = new IntPtr(marshalReparseBuffer.ToInt64() + Marshal.OffsetOf(toMountPointReparseBuffer, "data").ToInt64());
                     dataBuffer = new byte[bytesReturned - dataPos.ToInt64()];

                     Marshal.Copy(new IntPtr(bufPtr.ToInt64() + dataPos.ToInt64()), dataBuffer, 0, dataBuffer.Length);

                     return new LinkTargetInfo(
                        Encoding.Unicode.GetString(dataBuffer, mprb.SubstituteNameOffset, mprb.SubstituteNameLength),
                        Encoding.Unicode.GetString(dataBuffer, mprb.PrintNameOffset, mprb.PrintNameLength));

                  case ReparsePointTags.SymLink:
                     SymbolicLinkReparseBuffer slrb = GetStructure<SymbolicLinkReparseBuffer>(0, new IntPtr(bufPtr.ToInt64() + marshalReparseBuffer.ToInt64()));

                     dataPos = new IntPtr(marshalReparseBuffer.ToInt64() + Marshal.OffsetOf(toSymbolicLinkReparseBuffer, "data").ToInt64());
                     dataBuffer = new byte[bytesReturned - dataPos.ToInt64()];

                     Marshal.Copy(new IntPtr(bufPtr.ToInt64() + dataPos.ToInt64()), dataBuffer, 0, dataBuffer.Length);

                     return new SymbolicLinkTargetInfo(
                        Encoding.Unicode.GetString(dataBuffer, slrb.SubstituteNameOffset, slrb.SubstituteNameLength),
                        Encoding.Unicode.GetString(dataBuffer, slrb.PrintNameOffset, slrb.PrintNameLength),
                        slrb.Flags);

                  default:
                     throw new UnrecognizedReparsePointException();
               }
            }
            finally
            {
               if (safeBuffer != null)
                  safeBuffer.Dispose();
            }
         }

         #endregion // GetLinkTargetInfo


         #region Unified Internals

         #region CompressionEnable

         /// <summary>Unified method CompressionEnableInternal() to set the compression state of a file or directory on a volume whose file system supports per-file and per-directory compression.</summary>
         /// <param name="isFolder"><c>true</c> indicates a directory object, <c>false</c> indicates a file object.</param>
         /// <param name="transaction">The transaction.</param>
         /// <param name="path">A path that describes a folder or file to compress or decompress.</param>
         /// <param name="compress"><c>true</c> = compress, <c>false</c> = decompress</param>
         /// <returns>If the function succeeds, <c>true</c>, otherwise <c>false</c>.</returns>
         [SecurityCritical]
         internal static bool CompressionEnableInternal(bool isFolder, KernelTransaction transaction, string path, bool compress)
         {
            // To open a directory using CreateFile, specify the FILE_FLAG_BACKUP_SEMANTICS flag as part of dwFlagsAndAttributes.
            using (SafeFileHandle handle = FileSystemInfo.CreateFileInternal(!isFolder, transaction, path, isFolder ? EFileAttributes.BackupSemantics : EFileAttributes.Normal, null, FileMode.Open, FileSystemRights.Modify, FileShare.None))
            {
               // 0 = Decompress, 1 = Compress.
               byte[] data = InvokeIoControlUnknownSize(handle, DeviceIoControlCode.FsctlSetCompression, (compress) ? 1 : 0);

               // Unless an exception is thrown, this will always be: true;
               return data != null;
            }
         }

         #endregion // CompressionEnable

         #region InvokeIoControlUnknownSize

         /// <summary>Repeatedly invokes InvokeIoControl with the specified input until enough memory has been allocated.</summary>
         private static byte[] InvokeIoControlUnknownSize<TV>(SafeFileHandle handle, DeviceIoControlCode controlCode, TV input, uint increment = 128)
         {
            byte[] output;
            uint bytesReturned;

            uint inputSize = (uint)Marshal.SizeOf(input);
            uint outputLength = increment;

            do
            {
               output = new byte[outputLength];
               if (!DeviceIoControl(handle, controlCode, input, inputSize, output, outputLength, out bytesReturned, IntPtr.Zero))
               {
                  int lastError = Marshal.GetLastWin32Error();
                  switch ((uint)lastError)
                  {
                     case Win32Errors.ERROR_MORE_DATA:
                     case Win32Errors.ERROR_INSUFFICIENT_BUFFER:
                        outputLength += increment;
                        break;
                     default:
                        NativeError.ThrowException(lastError);
                        break;
                  }
               }
               else
                  break;
            } while (true);

            // Return the result
            if (output.Length == bytesReturned)
               return output;

            byte[] res = new byte[bytesReturned];
            Array.Copy(output, res, bytesReturned);

            return res;
         }

         #endregion // InvokeIoControlUnknownSize
         
         #endregion // Unified Internals
      }
   }
}