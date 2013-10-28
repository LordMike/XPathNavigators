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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Alphaleonis.Win32.Security
{
   internal static partial class NativeMethods
   {
      /// <summary>Class used to represent the SECURITY_ATTRIBUES native win32 structure. It provides initialization function from an <see cref="ObjectSecurity"/> object.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal sealed class SecurityAttributes
      {
         /// <summary>Initializes the SecurityAttributes structure from an instance of <see cref="ObjectSecurity"/>.</summary>
         /// <param name="memoryHandle">A handle that will refer to the memory allocated by this object for storage of the 
         /// security descriptor. As long as this object is used, the memory handle should be kept alive, and afterwards it
         /// should be disposed as soon as possible.</param>
         /// <param name="securityDescriptor">The <see cref="ObjectSecurity"/> security descriptor to assign to this object. This parameter may be <see langword="null"/>.</param>
         [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
         internal static void Initialize(out SafeGlobalMemoryBufferHandle memoryHandle, ObjectSecurity securityDescriptor)
         {
            //nLength = (uint) Marshal.SizeOf(this);

            if (securityDescriptor == null)
               memoryHandle = new SafeGlobalMemoryBufferHandle();

            else
            {
               byte[] src = securityDescriptor.GetSecurityDescriptorBinaryForm();
               memoryHandle = new SafeGlobalMemoryBufferHandle(src.Length);
               memoryHandle.CopyFrom(src, 0, src.Length);
            }
         }

         //private uint nLength;
      }
   }
}