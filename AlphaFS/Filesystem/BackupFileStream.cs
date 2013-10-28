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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using Alphaleonis.Win32.Security;
using Microsoft.Win32.SafeHandles;
using SecurityNativeMethods = Alphaleonis.Win32.Security.NativeMethods;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>The <see cref="BackupFileStream"/> provides access to data associated with a specific file or directory, including security information and alternative data streams, for backup and restore operations.</summary>
   /// <remarks>This class uses the <see href="http://msdn.microsoft.com/en-us/library/aa362509(VS.85).aspx">BackupRead</see>, 
   /// <see href="http://msdn.microsoft.com/en-us/library/aa362510(VS.85).aspx">BackupSeek</see> and 
   /// <see href="http://msdn.microsoft.com/en-us/library/aa362511(VS.85).aspx">BackupWrite</see> functions from the Win32 API to provide access to the file or directory.
   /// </remarks>
   public class BackupFileStream : Stream
   {
      #region Construction and Destruction

      #region File

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path and creation mode.</summary>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file. </param>
      /// <remarks>The file will be opened for exclusive access for both reading and writing.</remarks>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(string path, FileMode mode)
         : this(FileSystemInfo.CreateFileInternal(true, null, path, EFileAttributes.Normal, null, mode, FileSystemRights.Read | FileSystemRights.Write, FileShare.None), FileSystemRights.Read | FileSystemRights.Write)
      //: this(path, mode, FileSystemRights.Read | FileSystemRights.Write)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode and access rights.</summary>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file. </param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <remarks>The file will be opened for exclusive access.</remarks>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(string path, FileMode mode, FileSystemRights access)
         : this(FileSystemInfo.CreateFileInternal(true, null, path, EFileAttributes.Normal, null, mode, access, FileShare.None), access)
      //: this(path, mode, access, FileShare.None)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode, access rights and sharing permission.</summary>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file. </param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <param name="share">A <see cref="FileShare"/> constant that determines how the file will be shared by processes. </param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(string path, FileMode mode, FileSystemRights access, FileShare share)
         : this(FileSystemInfo.CreateFileInternal(true, null, path, EFileAttributes.Normal, null, mode, access, share), access)
      //: this(path, mode, access, share, EFileAttributes.None)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode, access rights and sharing permission, and additional file attributes.</summary>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file.</param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <param name="share">A <see cref="FileShare"/> constant that determines how the file will be shared by processes.</param>
      /// <param name="attributes">A <see cref="EFileAttributes"/> constant that specifies additional file attributes.</param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(string path, FileMode mode, FileSystemRights access, FileShare share, EFileAttributes attributes)
         : this(FileSystemInfo.CreateFileInternal(true, null, path, attributes, null, mode, access, share), access)
      //: this(path, mode, access, share, attributes, null)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode, access rights and sharing permission, additional file attributes, access control and audit security.</summary>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file.</param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <param name="share">A <see cref="FileShare"/> constant that determines how the file will be shared by processes.</param>
      /// <param name="attributes">A <see cref="EFileAttributes"/> constant that specifies additional file attributes.</param>
      /// <param name="security">A <see cref="FileSecurity"/> constant that determines the access control and audit security for the file. This parameter may be <see langword="null"/>.</param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(string path, FileMode mode, FileSystemRights access, FileShare share, EFileAttributes attributes, FileSecurity security)
         : this(FileSystemInfo.CreateFileInternal(true, null, path, attributes, security, mode, access, share), access)
      {
      }

      #region Transacted

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path and creation mode.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file.</param>
      /// <remarks>The file will be opened for exclusive access for both reading and writing.</remarks>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(KernelTransaction transaction, string path, FileMode mode)
         : this(FileSystemInfo.CreateFileInternal(true, transaction, path, EFileAttributes.Normal, null, mode, FileSystemRights.Read | FileSystemRights.Write, FileShare.None), FileSystemRights.Read | FileSystemRights.Write)
      //: this(transaction, path, mode, FileSystemRights.Read | FileSystemRights.Write)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode and access rights.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file.</param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <remarks>The file will be opened for exclusive access.</remarks>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(KernelTransaction transaction, string path, FileMode mode, FileSystemRights access)
         : this(FileSystemInfo.CreateFileInternal(true, transaction, path, EFileAttributes.Normal, null, mode, access, FileShare.None), access)
      //: this(transaction, path, mode, access, FileShare.None)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode, access rights and sharing permission.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file.</param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <param name="share">A <see cref="FileShare"/> constant that determines how the file will be shared by processes.</param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(KernelTransaction transaction, string path, FileMode mode, FileSystemRights access, FileShare share)
         : this(FileSystemInfo.CreateFileInternal(true, transaction, path, EFileAttributes.Normal, null, mode, access, share), access)
      //: this(transaction, path, mode, access, share, EFileAttributes.None)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode, access rights and sharing permission, and additional file attributes.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file.</param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <param name="share">A <see cref="FileShare"/> constant that determines how the file will be shared by processes.</param>
      /// <param name="attributes">A <see cref="EFileAttributes"/> constant that specifies additional file attributes.</param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(KernelTransaction transaction, string path, FileMode mode, FileSystemRights access, FileShare share, EFileAttributes attributes)
         : this(FileSystemInfo.CreateFileInternal(true, transaction, path, attributes, null, mode, access, share), access)
      //: this(transaction, path, mode, access, share, attributes, null)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class with the specified path, creation mode, access rights and sharing permission, additional file attributes, access control and audit security.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A relative or absolute path for the file that the current <see cref="BackupFileStream"/> object will encapsulate.</param>
      /// <param name="mode">A <see cref="FileMode"/> constant that determines how to open or create the file.</param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that determines the access rights to use when creating access and audit rules for the file.</param>
      /// <param name="share">A <see cref="FileShare"/> constant that determines how the file will be shared by processes.</param>
      /// <param name="attributes">A <see cref="EFileAttributes"/> constant that specifies additional file attributes.</param>
      /// <param name="security">A <see cref="FileSecurity"/> constant that determines the access control and audit security for the file. This parameter may be <see langword="null"/>.</param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public BackupFileStream(KernelTransaction transaction, string path, FileMode mode, FileSystemRights access, FileShare share, EFileAttributes attributes, FileSecurity security)
         : this(FileSystemInfo.CreateFileInternal(true, transaction, path, attributes, security, mode, access, share), access)
      //: this(File.CreateFileInternal(transaction, path, attributes, security, mode, access, share), access)
      {
      }

      #endregion //Transacted

      #endregion // File

      #region Stream

      /// <summary>Initializes a new instance of the <see cref="BackupFileStream"/> class for the specified file handle, with the specified read/write permission.</summary>
      /// <param name="handle">A file handle for the file that this <see cref="BackupFileStream"/> object will encapsulate. </param>
      /// <param name="access">A <see cref="FileSystemRights"/> constant that gets the <see cref="CanRead"/> and <see cref="CanWrite"/> properties of the <see cref="BackupFileStream"/> object. </param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public BackupFileStream(SafeFileHandle handle, FileSystemRights access)
      {
         _mFileHandle = handle;
         _mCanRead = (access & FileSystemRights.ReadData) != 0;
         _mCanWrite = (access & FileSystemRights.WriteData) != 0;
      }

      #endregion // Stream

      #region Dispose

      /// <summary>Releases unmanaged resources and performs other cleanup operations before the <see cref="BackupFileStream"/> is reclaimed by garbage collection.</summary>
      ~BackupFileStream()
      {
         Dispose(false);
      }

      #endregion // Dispose

      #endregion // Construction and Destruction

      #region Public Methods

      #region Read / Write

      #region Read

      /// <summary>Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
      /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values
      /// between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
      /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
      /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
      /// <returns>
      /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not
      /// currently available, or zero (0) if the end of the stream has been reached.
      /// </returns>
      /// <exception cref="System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length. </exception>
      /// <exception cref="System.ArgumentNullException">
      /// 	<paramref name="buffer"/> is <see langword="null"/>. </exception>
      /// <exception cref="System.ArgumentOutOfRangeException">
      /// 	<paramref name="offset"/> or <paramref name="count"/> is negative. </exception>
      /// <exception cref="NativeError.ThrowException()">An I/O error occurs. </exception>
      /// <exception cref="System.NotSupportedException">The stream does not support reading. </exception>
      /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
      /// <remarks>This method will not backup the access-control list (ACL) data for the file or directory.</remarks>
      [SecurityCritical]
      public override int Read(byte[] buffer, int offset, int count)
      {
         return Read(buffer, offset, count, false);
      }

      /// <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
      /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values
      /// between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
      /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
      /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
      /// <param name="processSecurity">Indicates whether the function will backup the access-control list (ACL) data for the file or directory. </param>
      /// <returns>
      /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not
      /// currently available, or zero (0) if the end of the stream has been reached.
      /// </returns>
      /// <exception cref="System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length. </exception>
      /// <exception cref="System.ArgumentNullException">
      /// 	<paramref name="buffer"/> is <see langword="null"/>. </exception>
      /// <exception cref="System.ArgumentOutOfRangeException">
      /// 	<paramref name="offset"/> or <paramref name="count"/> is negative. </exception>
      /// <exception cref="NativeError.ThrowException()">An I/O error occurs. </exception>
      /// <exception cref="System.NotSupportedException">The stream does not support reading. </exception>
      /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
      [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
      [SecurityCritical]
      public int Read(byte[] buffer, int offset, int count, bool processSecurity)
      {
         if (!CanRead)
            throw new NotSupportedException("Stream does not support reading");

         if (buffer == null)
            throw new ArgumentNullException("buffer");

         if (offset + count > buffer.Length)
            throw new ArgumentException("The sum of offset and count is larger than the size of the buffer.");

         if (offset < 0)
            throw new ArgumentOutOfRangeException("offset", offset, Resources.OffsetMustNotBeNegative);

         if (count < 0)
            throw new ArgumentOutOfRangeException("count", count, Resources.CountMustNotBeNegative);

         using (SafeGlobalMemoryBufferHandle hBuf = new SafeGlobalMemoryBufferHandle(count))
         {
            uint numberOfBytesRead;

            if (!NativeMethods.BackupRead(_mFileHandle, hBuf, (uint)hBuf.Capacity, out numberOfBytesRead, false, processSecurity, out _mContext))
               NativeError.ThrowException();

            hBuf.CopyTo(buffer, offset, count);

            return (int)numberOfBytesRead;
         }
      }

      #endregion // Read

      #region Write

      /// <summary>Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
      /// <overloads>
      /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
      /// </overloads>
      /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
      /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
      /// <param name="count">The number of bytes to be written to the current stream.</param>
      /// <exception cref="System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length. </exception>
      /// <exception cref="System.ArgumentNullException">
      /// 	<paramref name="buffer"/> is null. </exception>
      /// <exception cref="System.ArgumentOutOfRangeException">
      /// 	<paramref name="offset"/> or <paramref name="count"/> is negative. </exception>
      /// <exception cref="NativeError.ThrowException()">An I/O error occurs. </exception>
      /// <exception cref="System.NotSupportedException">The stream does not support writing. </exception>
      /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
      /// <remarks>This method will not process the access-control list (ACL) data for the file or directory.</remarks>
      [SecurityCritical]
      public override void Write(byte[] buffer, int offset, int count)
      {
         Write(buffer, offset, count, false);
      }

      /// <summary>When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
      /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
      /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
      /// <param name="count">The number of bytes to be written to the current stream.</param>
      /// <param name="processSecurity">Specifies whether the function will restore the access-control list (ACL) data for the file or directory. 
      /// If this is <c>true</c>, you need to specify <see cref="FileSystemRights.TakeOwnership"/> and <see cref="FileSystemRights.ChangePermissions"/> access when 
      /// opening the file or directory handle. If the handle does not have those access rights, the operating system denies 
      /// access to the ACL data, and ACL data restoration will not occur.</param>
      /// <exception cref="System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length. </exception>
      /// <exception cref="System.ArgumentNullException">
      /// 	<paramref name="buffer"/> is null. </exception>
      /// <exception cref="System.ArgumentOutOfRangeException">
      /// 	<paramref name="offset"/> or <paramref name="count"/> is negative. </exception>
      /// <exception cref="NativeError.ThrowException()">An I/O error occurs. </exception>
      /// <exception cref="System.NotSupportedException">The stream does not support writing. </exception>
      /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
      [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
      [SecurityCritical]
      public bool Write(byte[] buffer, int offset, int count, bool processSecurity)
      {
         if (buffer == null)
            throw new ArgumentNullException("buffer");

         if (offset < 0)
            throw new ArgumentOutOfRangeException("offset", offset, Resources.OffsetMustNotBeNegative);

         if (count < 0)
            throw new ArgumentOutOfRangeException("count", count, Resources.CountMustNotBeNegative);

         if (offset + count > buffer.Length)
            throw new ArgumentException(Resources.BufferIsNotLargeEnoughForTheRequestedOperation);

         using (SafeGlobalMemoryBufferHandle hBuf = new SafeGlobalMemoryBufferHandle(count))
         {
            hBuf.CopyFrom(buffer, offset, count);

            uint bytesWritten;

            if (!NativeMethods.BackupWrite(_mFileHandle, hBuf, (uint)hBuf.Capacity, out bytesWritten, false, processSecurity, out _mContext))
               NativeError.ThrowException();
         }

         return true;
      }

      #endregion // Write

      #region Flush

      /// <summary>Clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
      /// <exception cref="NativeError.ThrowException()">An I/O error occurs. </exception>
      [SecurityCritical]
      public override void Flush()
      {
         if (!NativeMethods.FlushFileBuffers(_mFileHandle))
            NativeError.ThrowException();
      }

      #endregion // Flush

      #endregion // Read / Write

      #region Navigate

      #region Seek

      /// <summary>When overridden in a derived class, sets the position within the current stream.</summary>
      /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
      /// <param name="origin">A value of type <see cref="System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
      /// <returns>
      /// The new position within the current stream.
      /// </returns>
      /// <remarks>
      ///     <para>
      ///         <note>
      ///             <para>
      ///                 This stream does not support seeking using this method, and calling this method will always throw 
      ///                 <see cref="NotSupportedException"/>. See <see cref="Skip"/> for an alternative way of seeking forward.
      ///             </para>
      ///         </note>
      ///     </para>
      /// </remarks>
      /// <exception cref="System.NotSupportedException">The stream does not support seeking.</exception>
      public override long Seek(long offset, SeekOrigin origin)
      {
         throw new NotSupportedException();
      }

      #endregion // Seek

      #region SetLength

      /// <summary>When overridden in a derived class, sets the length of the current stream.</summary>
      /// <param name="value">The desired length of the current stream in bytes.</param>
      /// <remarks>This method is not supported by the <see cref="BackupFileStream"/> class, and calling it will always
      /// generate a <see cref="NotSupportedException"/>.</remarks>
      /// <exception cref="System.NotSupportedException">Always thrown by this class.</exception>
      public override void SetLength(long value)
      {
         throw new NotSupportedException(Resources.ThisStreamDoesNotSupportSeeking);
      }

      #endregion // SetLength

      #region Skip

      /// <summary>Skips ahead the specified number of bytes from the current stream.</summary>
      /// <remarks>
      /// <para>
      ///     This method represents the Win32 API implementation of <see href="http://msdn.microsoft.com/en-us/library/aa362509(VS.85).aspx">BackupSeek</see>.
      /// </para>
      /// <para>
      /// Applications use the <see cref="Skip"/> method to skip portions of a data stream that cause errors. This function does not 
      /// seek across stream headers. For example, this function cannot be used to skip the stream name. If an application 
      /// attempts to seek past the end of a substream, the function fails, the return value indicates the actual number of bytes 
      /// the function seeks, and the file position is placed at the start of the next stream header.
      /// </para>
      /// </remarks>
      /// <param name="bytes">The number of bytes to skip.</param>
      /// <returns>The number of bytes actually skipped.</returns>
      [SecurityCritical]
      public long Skip(long bytes)
      {
         uint lowSought, highSought;
         if (!NativeMethods.BackupSeek(_mFileHandle, NativeMethods.GetLowOrderDword(bytes), NativeMethods.GetHighOrderDword(bytes), out lowSought, out highSought, out _mContext))
         {
            int lastError = Marshal.GetLastWin32Error();

            // Error Code 25 indicates a seek error, we just skip that here.
            if (lastError != Win32Errors.NO_ERROR && lastError != Win32Errors.ERROR_SEEK)
               NativeError.ThrowException(lastError);
         }

         return NativeMethods.ToLong(highSought, lowSought);
      }

      #endregion // Skip

      #endregion // Navigate

      #region Get/SetAccessControl

      #region GetAccessControl

      /// <summary>Gets a <see cref="FileSecurity"/> object that encapsulates the access control list (ACL) entries for the file described by the current <see cref="BackupFileStream"/> object.</summary>
      /// <returns>A <see cref="FileSecurity"/> object that encapsulates the access control list (ACL) entries for the file described by the current <see cref="BackupFileStream"/> object. </returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      [SecurityCritical]
      public FileSecurity GetAccessControl()
      {
         IntPtr pSidOwner, pSidGroup, pDacl, pSacl;
         SafeGlobalMemoryBufferHandle pSecurityDescriptor;

         uint lastError = SecurityNativeMethods.GetSecurityInfo(_mFileHandle, ObjectType.FileObject,
                                                                SecurityInformation.Group | SecurityInformation.Owner |
                                                                SecurityInformation.Label | SecurityInformation.Dacl | SecurityInformation.Sacl,
                                                                out pSidOwner, out pSidGroup, out pDacl, out pSacl, out pSecurityDescriptor);
         try
         {
            if (lastError != Win32Errors.ERROR_SUCCESS)
               NativeError.ThrowException(lastError);

            if (!NativeMethods.IsValidHandle(pSecurityDescriptor, false))
               throw new IOException(Resources.InvalidSecurityDescriptorReturnedFromSystem);

            uint length = SecurityNativeMethods.GetSecurityDescriptorLength(pSecurityDescriptor);

            byte[] managedBuffer = new byte[length];
            pSecurityDescriptor.CopyTo(managedBuffer, 0, (int) length);
            FileSecurity fs = new FileSecurity();
            fs.SetSecurityDescriptorBinaryForm(managedBuffer);

            return fs;
         }
         finally
         {
            if (pSecurityDescriptor != null)
               pSecurityDescriptor.Dispose();
         }
      }

      #endregion // GetAccessControl

      #region SetAccessControl

      /// <summary>Applies access control list (ACL) entries described by a <see cref="FileSecurity"/> object to the file described by the  current <see cref="BackupFileStream"/> object.</summary>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> object that describes an ACL entry to apply to the current file.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool SetAccessControl(ObjectSecurity fileSecurity)
      {
         return SecurityNativeMethods.SetAccessControlInternal(null, _mFileHandle, fileSecurity, AccessControlSections.All);
      }

      #endregion // SetAccessControl

      #endregion // Get/SetAccessControl

      #region Lock / Unlock

      #region Lock

      /// <summary>Prevents other processes from changing the <see cref="BackupFileStream"/> while permitting read access.</summary>
      /// <param name="position">The beginning of the range to lock. The value of this parameter must be equal to or greater than zero (0).</param>
      /// <param name="length">The range to be locked. </param>
      /// <exception cref="ArgumentOutOfRangeException"><paramref name="position"/> or <paramref name="length"/> is negative.</exception>
      /// <exception cref="ObjectDisposedException">The file is closed.</exception>
      /// <exception cref="NativeError.ThrowException()">The process cannot access the file because another process has locked a portion of the file. </exception>
      [SecurityCritical]
      public virtual void Lock(long position, long length)
      {
         if (position < 0)
            throw new ArgumentOutOfRangeException("position", position, Resources.BackupFileStream_Unlock_Backup_FileStream_Unlock_Position_must_not_be_negative_);

         if (length < 0)
            throw new ArgumentOutOfRangeException("length", length, Resources.BackupFileStream_Unlock_Backup_FileStream_Lock_Length_must_not_be_negative_);

         if (!NativeMethods.LockFile(_mFileHandle,
                     NativeMethods.GetLowOrderDword(position), NativeMethods.GetHighOrderDword(position),
                     NativeMethods.GetLowOrderDword(length)  , NativeMethods.GetHighOrderDword(length)))
            NativeError.ThrowException();
      }

      #endregion // Lock

      #region Unlock

      /// <summary>Allows access by other processes to all or part of a file that was previously locked.</summary>
      /// <param name="position">The beginning of the range to unlock.</param>
      /// <param name="length">The range to be unlocked.</param>
      /// <exception cref="ArgumentOutOfRangeException"></exception>
      /// <exception cref="ArgumentOutOfRangeException"><paramref name="position"/> or <paramref name="length"/> is negative.</exception>
      /// <exception cref="ObjectDisposedException">The file is closed.</exception>
      [SecurityCritical]
      public virtual void Unlock(long position, long length)
      {
         if (position < 0)
            throw new ArgumentOutOfRangeException("position", position, Resources.BackupFileStream_Unlock_Backup_FileStream_Unlock_Position_must_not_be_negative_);

         if (length < 0)
            throw new ArgumentOutOfRangeException("length", length, Resources.BackupFileStream_Unlock_Backup_FileStream_Lock_Length_must_not_be_negative_);

         if (!NativeMethods.UnlockFile(_mFileHandle,
                     NativeMethods.GetLowOrderDword(position), NativeMethods.GetHighOrderDword(position),
                     NativeMethods.GetLowOrderDword(length)  , NativeMethods.GetHighOrderDword(length)))
            NativeError.ThrowException();
      }

      #endregion Unlock

      #endregion // Lock / Unlock

      #region EnumerateStreams

      /// <summary>Returns <see cref="BackupStreamInfo"/> instances, associated with the file.</summary>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the file specified by path.</returns>
      public IEnumerable<BackupStreamInfo> EnumerateStreams()
      {
         return File.EnumerateStreams(_mFileHandle);
      }

      #endregion // EnumerateStreams

      #region ReadStreamInfo

      /// <summary>Reads a stream header from the current <see cref="BackupFileStream"/>.</summary>
      /// <returns>The stream header read from the current <see cref="BackupFileStream"/>, or <see langword="null"/> if the end-of-file 
      /// was reached before the required number of bytes of a header could be read.</returns>
      /// <remarks>The stream must be positioned at where an actual header starts for the returned object to represent valid information.</remarks>
      public BackupStreamInfo ReadStreamInfo()
      {
         // Return the first entry.
         return File.EnumerateStreams(_mFileHandle).FirstOrDefault();
      }

      #endregion // ReadStreamInfo

      #endregion // Public Methods

      #region Public Properties

      /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
      /// <returns><c>true</c> if the stream supports reading, <c>false</c> otherwise.</returns>
      public override bool CanRead
      {
         get { return _mCanRead; }
      }

      /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>        
      /// <returns>This method always returns <c>false</c>.</returns>
      public override bool CanSeek
      {
         get { return false; }
      }

      /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
      /// <returns><c>true</c> if the stream supports writing, <c>false</c> otherwise.</returns>
      public override bool CanWrite
      {
         get { return _mCanWrite; }
      }

      /// <summary>When overridden in a derived class, gets the length in bytes of the stream.</summary>
      /// <value>This method always throws an exception.</value>
      /// <exception cref="System.NotSupportedException">This exception is always thrown if this property is accessed on a <see cref="BackupFileStream"/>.</exception>
      public override long Length
      {
         get { throw new NotSupportedException(Resources.ThisStreamDoesNotSupportSeeking); }
      }

      /// <summary>When overridden in a derived class, gets or sets the position within the current stream.</summary>
      /// <value>This method always throws an exception.</value>
      /// <exception cref="System.NotSupportedException">This exception is always thrown if this property is accessed on a <see cref="BackupFileStream"/>.</exception>
      public override long Position
      {
         get { throw new NotSupportedException(Resources.ThisStreamDoesNotSupportSeeking); }
         set { throw new NotSupportedException(Resources.ThisStreamDoesNotSupportSeeking); }
      }

      /// <summary>Gets a <see cref="SafeFileHandle"/> object that represents the operating system file handle for the file that the current <see cref="BackupFileStream"/> object encapsulates.</summary>
      /// <value>A <see cref="SafeFileHandle"/> object that represents the operating system file handle for the file that 
      /// the current <see cref="BackupFileStream"/> object encapsulates.</value>
      public SafeFileHandle SafeFileHandle
      {
         get { return _mFileHandle; }
      }

      #endregion // Public Properties

      #region Protected Methods

      /// <summary>Releases the unmanaged resources used by the <see cref="System.IO.Stream"/> and optionally releases the managed resources.</summary>
      /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
      [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations"), SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      protected override void Dispose(bool disposing)
      {
         // If one of the constructors previously threw an exception,
         // than the object hasn't been initialized properly and call from finalize will fail.
         
         if (NativeMethods.IsValidHandle(_mFileHandle, false))
         {
            if (_mContext != IntPtr.Zero)
            {
               uint temp;
               if (!NativeMethods.BackupRead(_mFileHandle, new SafeGlobalMemoryBufferHandle(), 0, out temp, true, false, out _mContext))
                  NativeError.ThrowException();

               _mContext = IntPtr.Zero;
            }

            _mFileHandle.Dispose();
         }

         base.Dispose(disposing);
      }

      #endregion // Protected Methods

      #region Private Fields

      [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
      private IntPtr _mContext = IntPtr.Zero;
      private readonly SafeFileHandle _mFileHandle;
      private readonly bool _mCanRead;
      private readonly bool _mCanWrite;

      #endregion // Private Fields
   }
}