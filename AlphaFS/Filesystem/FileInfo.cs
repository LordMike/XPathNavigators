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
using System.Security;
using System.Security.AccessControl;
using System.Text;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Provides properties and instance methods for the creation, copying, deletion, moving, and opening of files, and aids in the creation of <see cref="FileStream"/> objects. This class cannot be inherited.</summary>
   [Serializable]
   [SecurityCritical]
   public sealed class FileInfo : FileSystemInfo
   {
      #region Constructors

      #region FileInfo

      #region .NET

      /// <summary>Initializes a new instance of the FileInfo class, which acts as a wrapper for a file path.</summary>
      /// <param name="path"><para>The full path of the filesystem object on which to create the <see cref="System.IO.FileInfo"/>.</para></param>
      /// <remarks>You can specify either the fully qualified or the relative file name, but the security check gets the fully qualified name.</remarks>
      public FileInfo(string path)
      {
         Initialize(null, false, path);
      }
      
      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Initializes a new instance of the FileInfo class, which acts as a wrapper for a file path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path"><para>The full path of the filesystem object on which to create the <see cref="System.IO.FileInfo"/>.</para></param>
      /// <remarks>You can specify either the fully qualified or the relative file name, but the security check gets the fully qualified name.</remarks>
      public FileInfo(KernelTransaction transaction, string path)
      {
         Initialize(transaction, false, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // FileInfo

      #endregion // Constructors

      #region Fields

      #region AlphaFS

      private long _length = -1;
      private long _lengthCompressed = -1;
      private long _lengthStreams = -1;

      #endregion // AlphaFS

      #endregion // Fields

      #region Methods

      #region .NET

      #region AppendText

      /// <summary>Creates a <see cref="StreamWriter"/> that appends text to the file represented by this instance of the <see cref="FileInfo"/>.</summary>
      /// <returns>A new <see cref="StreamWriter"/></returns>
      [SecurityCritical]
      public StreamWriter AppendText()
      {
         return File.AppendTextInternal(Transaction, FullPath, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Creates a <see cref="StreamWriter"/> that appends text to the file represented by this instance of the <see cref="FileInfo"/>.</summary>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      /// <returns>A new <see cref="StreamWriter"/></returns>
      [SecurityCritical]
      public StreamWriter AppendText(Encoding encoding)
      {
         return File.AppendTextInternal(Transaction, FullPath, encoding);
      }

      #endregion // AppendText

      #region CopyTo

      /// <summary>Copies an existing file to a new file, disallowing the overwriting of an existing file.</summary>
      /// <param name="destinationPath">The name of the new file to copy to.</param>
      /// <returns>A new <see cref="FileInfo"/> file with a fully qualified path. Returns null on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public FileInfo CopyTo(string destinationPath)
      {
         return CopyToMoveToInternal(destinationPath, NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Copies an existing file to a new file, allowing the overwriting of an existing file.</summary>
      /// <param name="destinationPath">The name of the new file to copy to. </param>
      /// <param name="overwrite"><c>true</c> to allow an existing file to be overwritten; otherwise, <c>false</c>.</param>
      /// <returns><see cref="FileInfo"/>A new file, or an overwrite of an existing file if overwrite is true. If the file exists and overwrite is false, an IOException is thrown. Returns null on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public FileInfo CopyTo(string destinationPath, bool overwrite)
      {
         return CopyToMoveToInternal(destinationPath, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, null, null);
      }

      #region AlphaFS

      /// <summary>Copies an existing file to a new file, allowing the overwriting of an existing file.</summary>
      /// <param name="destinationPath">The name of the new file to copy to. </param>
      /// <param name="overwrite"><c>true</c> to allow an existing file to be overwritten; otherwise, <c>false</c>.</param>
      /// <param name="copyProgress"><para>This parameter can be <see langword="null"/>. A callback function that is called each time another portion of the file has been copied.</para></param>
      /// <param name="userProgressData"><para>This parameter can be <see langword="null"/>. The argument to be passed to the callback function.</para></param>
      /// <returns><see cref="FileInfo"/>A new file, or an overwrite of an existing file if overwrite is true. If the file exists and overwrite is false, an IOException is thrown. Returns null on failure or the operation was aborted.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public FileInfo CopyTo(string destinationPath, bool overwrite, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyToMoveToInternal(destinationPath, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, copyProgress, userProgressData);
      }

      #endregion // AlphaFS

      #endregion // CopyTo

      #region Create

      /// <summary>Creates a file.</summary>
      /// <returns><see cref="FileStream"/>A new file.</returns>
      [SecurityCritical]
      public FileStream Create()
      {
         // File.Create()
         return File.CreateFileInternal(Transaction, FullPath, NativeMethods.DefaultFileBufferSize, EFileAttributes.Normal, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      #endregion // Create

      #region CreateText

      /// <summary>Creates a <see crefe="StreamWriter"/> instance that writes a new text file.</summary>
      /// <returns>A new <see cref="StreamWriter"/></returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "A valid handle needs to be returned.")]
      [SecurityCritical]
      public StreamWriter CreateText()
      {
         // File.CreateText()
         return new StreamWriter(File.CreateFileInternal(Transaction, FullPath, NativeMethods.DefaultFileBufferSize, EFileAttributes.Normal, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None), NativeMethods.DefaultFileEncoding);
      }

      #endregion // CreateText

      #region Decrypt

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Decrypt

      #region Delete

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Delete

      #region Encrypt

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Encrypt

      #region GetAccessControl

      /// <summary>Gets a <see cref="System.Security.AccessControl.FileSecurity"/> object that encapsulates the access control list (ACL) entries for the file described by the current <see cref="FileInfo"/> object.</summary>
      /// <returns><see cref="System.Security.AccessControl.FileSecurity"/>A FileSecurity object that encapsulates the access control rules for the current file. </returns>
      [SecurityCritical]
      public FileSecurity GetAccessControl()
      {
         return (FileSecurity)GetSetAccessControlInternal(false, false, FullPath, null, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner);
      }

      /// <summary>Gets a <see cref="System.Security.AccessControl.FileSecurity"/> object that encapsulates the specified type of access control list (ACL) entries for the file described by the current FileInfo object.</summary>
      /// <param name="includeSections">One of the <see cref="System.Security"/> values that specifies which group of access control entries to retrieve.</param>
      /// <returns><see cref="System.Security.AccessControl.FileSecurity"/> object that encapsulates the specified type of access control list (ACL) entries for the file described by the current FileInfo object.</returns>
      [SecurityCritical]
      public FileSecurity GetAccessControl(AccessControlSections includeSections)
      {
         return (FileSecurity)GetSetAccessControlInternal(false, false, FullPath, null, includeSections);
      }

      #endregion // GetAccessControl

      #region Open

      #region .NET

      /// <summary>Opens a file in the specified mode.</summary>
      /// <param name="mode">A <see cref="FileMode"/> constant specifying the mode (for example, Open or Append) in which to open the file.</param>
      /// <returns>A <see cref="FileStream"/> file opened in the specified mode, with read/write access and unshared.</returns>
      [SecurityCritical]
      public FileStream Open(FileMode mode)
      {
         return File.OpenInternal(Transaction, FullPath, mode, 0, FileAccess.Read, FileShare.None, EFileAttributes.Normal);
      }

      /// <summary>Opens a file in the specified mode with read, write, or read/write access.</summary>
      /// <param name="mode">A <see cref="FileMode"/> constant specifying the mode (for example, Open or Append) in which to open the file.</param>
      /// <param name="access">A <see cref="FileAccess"/> constant specifying whether to open the file with Read, Write, or ReadWrite file access. </param>
      /// <returns>A <see cref="FileStream"/> object opened in the specified mode and access, and unshared.</returns>
      [SecurityCritical]
      public FileStream Open(FileMode mode, FileAccess access)
      {
         return File.OpenInternal(Transaction, FullPath, mode, 0, access, FileShare.None, EFileAttributes.Normal);
      }

      /// <summary>Opens a file in the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="mode">A <see cref="FileMode"/> constant specifying the mode (for example, Open or Append) in which to open the file.</param>
      /// <param name="access">A <see cref="FileAccess"/> constant specifying whether to open the file with Read, Write, or ReadWrite file access. </param>
      /// <param name="share">A <see cref="FileShare"/> constant specifying the type of access other F<see cref="FileStream"/> objects have to this file.</param>
      /// <returns>A <see cref="FileStream"/> object opened with the specified mode, access, and sharing options.</returns>
      [SecurityCritical]
      public FileStream Open(FileMode mode, FileAccess access, FileShare share)
      {
         return File.OpenInternal(Transaction, FullPath, mode, 0, access, share, EFileAttributes.Normal);
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Opens a file in the specified mode with read, write, or read/write access.</summary>
      /// <param name="mode">A <see cref="FileMode"/> constant specifying the mode (for example, Open or Append) in which to open the file.</param>
      /// <param name="rights">A <see cref="FileSystemRights"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten along with additional options.</param>
      /// <returns>A <see cref="FileStream"/> object opened in the specified mode and access, and unshared.</returns>
      [SecurityCritical]
      public FileStream Open(FileMode mode, FileSystemRights rights)
      {
         return File.OpenInternal(Transaction, FullPath, mode, rights, 0, FileShare.None, EFileAttributes.Normal);
      }

      /// <summary>Opens a file in the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="mode">A <see cref="FileMode"/> constant specifying the mode (for example, Open or Append) in which to open the file.</param>
      /// <param name="rights">A <see cref="FileSystemRights"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten along with additional options.</param>
      /// <param name="share">A <see cref="FileShare"/> constant specifying the type of access other <see cref="FileStream"/> objects have to this file.</param>
      /// <returns>A <see cref="FileStream"/> object opened with the specified mode, access, and sharing options.</returns>
      [SecurityCritical]
      public FileStream Open(FileMode mode, FileSystemRights rights, FileShare share)
      {
         return File.OpenInternal(Transaction, FullPath, mode, rights, 0, share, EFileAttributes.Normal);
      }

      #endregion // AlphaFS

      #endregion // Open

      #region OpenRead

      /// <summary>Creates a read-only <see cref="FileStream"/>.</summary>
      /// <returns>A new read-only <see cref="FileStream"/> object.</returns>
      /// <remarks>This method returns a read-only <see cref="FileStream"/> object with the <see cref="FileShare"/> mode set to Read.</remarks>
      [SecurityCritical]
      public FileStream OpenRead()
      {
         // File.OpenRead()
         return File.OpenInternal(Transaction, FullPath, FileMode.Open, 0, FileAccess.Read, FileShare.Read, EFileAttributes.Normal);
      }

      #endregion // OpenRead

      #region OpenText

      /// <summary>Creates a <see cref="StreamReader"/> with <see cref="NativeMethods.DefaultFileEncoding"/> encoding that reads from an existing text file.</summary>
      /// <returns>A new <see cref="StreamReader"/> with <see cref="NativeMethods.DefaultFileEncoding"/> encoding.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public StreamReader OpenText()
      {
         // File.OpenText()
         return new StreamReader(File.OpenInternal(Transaction, FullPath, FileMode.Open, 0, FileAccess.Read, FileShare.None, EFileAttributes.Normal), NativeMethods.DefaultFileEncoding);
      }

      #region AlphaFS

      /// <summary>Creates a <see cref="StreamReader"/> with <see cref="Encoding"/> that reads from an existing text file.</summary>
      /// <returns>A new <see cref="StreamReader"/> with the specified <see cref="Encoding"/>.</returns>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public StreamReader OpenText(Encoding encoding)
      {
         // File.OpenText()
         return new StreamReader((File.OpenInternal(Transaction, FullPath, FileMode.Open, 0, FileAccess.Read, FileShare.None, EFileAttributes.Normal)), encoding);
      }

      #endregion // AlphaFS

      #endregion // OpenText

      #region OpenWrite

      /// <summary>Creates a write-only <see cref="FileStream"/>.</summary>
      /// <returns>A write-only unshared <see cref="FileStream"/> object for a new or existing file.</returns>
      [SecurityCritical]
      public FileStream OpenWrite()
      {
         // File.OpenWrite()
         return File.OpenInternal(Transaction, FullPath, FileMode.Open, 0, FileAccess.Write, FileShare.None, EFileAttributes.Normal);
      }

      #endregion // OpenWrite

      #region Refresh

      /// <summary>Refreshes the state of the object.</summary>
      [SecurityCritical]
      public new void Refresh()
      {
         base.Refresh();
      }

      #endregion // Refresh

      #region Replace

      /// <summary>Replaces the contents of a specified file with the file described by the current <see cref="FileInfo"/> object, deleting the original file, and creating a backup of the replaced file.</summary>
      /// <param name="destinationFileName">The name of a file to replace with the current file.</param>
      /// <param name="destinationBackupFileName">The name of a file with which to create a backup of the file described by the destinationFileName parameter.</param>
      /// <returns>A <see cref="FileInfo"/> object that encapsulates information about the file described by the destinationFileName parameter, or null on failure.</returns>
      /// <remarks>The Replace method replaces the contents of a specified file with the contents of the file described by the current <see cref="FileInfo"/> object. It also creates a backup of the file that was replaced. Finally, it returns a new <see cref="FileInfo"/> object that describes the overwritten file.</remarks>
      [SecurityCritical]
      public FileInfo Replace(string destinationFileName, string destinationBackupFileName)
      {
         return Replace(destinationFileName, destinationBackupFileName, false);
      }

      /// <summary>Replaces the contents of a specified file with the file described by the current <see cref="FileInfo"/> object, deleting the original file, and creating a backup of the replaced file. Also specifies whether to ignore merge errors.</summary>
      /// <param name="destinationFileName">The name of a file to replace with the current file.</param>
      /// <param name="destinationBackupFileName">The name of a file with which to create a backup of the file described by the destinationFileName parameter.</param>
      /// <param name="ignoreMetadataErrors">true to ignore merge errors (such as attributes and ACLs) from the replaced file to the replacement file; otherwise false.</param>
      /// <returns><see cref="FileInfo"/>A FileInfo object that encapsulates information about the file described by the destinationFileName parameter, or null on failure.</returns>
      /// <remarks>The Replace method replaces the contents of a specified file with the contents of the file described by the current <see cref="FileInfo"/> object. It also creates a backup of the file that was replaced. Finally, it returns a new <see cref="FileInfo"/> object that describes the overwritten file.</remarks>
      /// <remarks>The last parameter <paramref name="ignoreMetadataErrors"/> is not supported yet.</remarks>
      [SecurityCritical]
      public FileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
      {
         return File.ReplaceInternal(FullPath, destinationFileName, destinationBackupFileName, ignoreMetadataErrors)
                   ? new FileInfo(Transaction, destinationFileName)
                   : null;
      }

      #endregion // Replace

      #region SetAccessControl

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // SetAccessControl

      #region ToString

      /// <summary>Returns the path as a string.</summary>
      /// <remarks>The string returned by the ToString method represents path that was passed to the constructor. When you create a FileInfo object using the constructors, the ToString method returns the fully qualified path.</remarks>
      public override string ToString()
      {
         //return FullPath;
         return OriginalPath;
      }

      #endregion // ToString

      #endregion // .NET

      #region AlphaFS

      #region Compress

      /// <summary>Compresses a file using NTFS compression.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Compress()
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(false, Transaction, FullPath, true);
      }

      #endregion // Compress

      #region Decompress

      /// <summary>Decompresses an NTFS compressed file.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Decompress()
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(false, Transaction, FullPath, false);
      }

      #endregion // Decompress

      #region Decrypt

      /// <summary>Decrypts a file that was encrypted by the current account using the <see cref="FileInfo.Encrypt"/> method.</summary>
      /// <remarks>The Decrypt method allows you to decrypt a file that was encrypted using the Encrypt method.
      /// The Decrypt method can decrypt only files that were encrypted using the current user account.
      /// Both the Encrypt method and the Decrypt method use the cryptographic service provider (CSP) installed on the computer and the file encryption keys of the process calling the method.
      /// The current file system must be formatted as NTFS and the current operating system must be Microsoft Windows NT or later.
      /// </remarks>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Decrypt()
      {
         return File.Decrypt(FullPath);
      }

      #endregion // Decrypt

      #region Delete

      /// <summary>Permanently deletes a file.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      public override bool Delete()
      {
         bool deleteOk = File.DeleteFileInternal(Transaction, FullPath, false);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      /// <summary>Permanently deletes a file.</summary>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides the read only attribute of the file.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>An exception is not thrown if the specified file does not exist.</remarks>
      public bool Delete(bool ignoreReadOnly)
      {
         bool deleteOk = File.DeleteFileInternal(Transaction, FullPath, ignoreReadOnly);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      #endregion // Delete

      #region Encrypt

      /// <summary>Encrypts a file so that only the account used to encrypt the file can decrypt it.</summary>
      /// <remarks>
      /// The Encrypt method allows you to encrypt a file so that only the account used to call this method can decrypt it. Use the Decrypt method to decrypt a file encrypted by the Encrypt method. 
      /// Both the Encrypt method and the Decrypt method use the cryptographic service provider (CSP) installed on the computer and the file encryption keys of the process calling the method. 
      /// The current file system must be formatted as NTFS and the current operating system must be Microsoft Windows NT or later.
      /// </remarks>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Encrypt()
      {
         return File.Encrypt(FullPath);
      }

      #endregion // Encrypt

      #region EnumerateStreams

      /// <summary>Returns <see cref="BackupStreamInfo"/> instances, associated with the file.</summary>
      /// <returns>An enumerable <see langref="BackupStreamInfo"/> collection of streams for the file or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public IEnumerable<BackupStreamInfo> EnumerateStreams()
      {
         return File.EnumerateStreams(Transaction, FullPath);
      }

      #endregion // EnumerateStreams

      #region MoveTo

      /// <summary>Moves a specified file to a new location, providing the option to specify a new file name.</summary>
      /// <param name="destinationPath">The path to move the file to, which can specify a different file name.</param>
      /// <returns>A new <see cref="FileInfo"/> file with a fully qualified path. Returns null on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public FileInfo MoveTo(string destinationPath)
      {
         return CopyToMoveToInternal(destinationPath, null, NativeMethods.MoveOptsReplace, null, null);
      }

      /// <summary>Moves a FileyInfo instance and its contents to a new path.</summary>
      /// <param name="destinationPath">The path to the new location for sourcePath.</param>
      /// <param name="copyProgress"><para>This parameter can be <see langword="null"/>. A callback function that is called each time another portion of the file has been copied.</para></param>
      /// <param name="userProgressData"><para>This parameter can be <see langword="null"/>. The argument to be passed to the callback function.</para></param>
      /// <returns>A new <see cref="FileInfo"/> file with a fully qualified path. Returns null on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public FileInfo MoveTo(string destinationPath, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyToMoveToInternal(destinationPath, null, NativeMethods.MoveOptsReplace, copyProgress, userProgressData);
      }

      #endregion // MoveTo

      #region SetAccessControl

      /// <summary>Applies access control list (ACL) entries described by a FileSecurity object to the file described by the current FileInfo object.</summary>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> object that describes an access control list (ACL) entry to apply to the current file.</param>
      /// <remarks>The SetAccessControl method applies access control list (ACL) entries to the current file that represents the noninherited ACL list. 
      /// Use the SetAccessControl method whenever you need to add or remove ACL entries from a file.
      /// </remarks>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public bool SetAccessControl(FileSecurity fileSecurity)
      {
         // In this case, equals null, is a good thing.
         return SetAccessControl(fileSecurity, AccessControlSections.All);
      }

      /// <summary>Applies access control list (ACL) entries described by a FileSecurity object to the file described by the current FileInfo object.</summary>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> object that describes an access control list (ACL) entry to apply to the current file.</param>
      /// <param name="includeSections">One or more of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to set.</param>
      /// <remarks>The SetAccessControl method applies access control list (ACL) entries to the current file that represents the noninherited ACL list. 
      /// Use the SetAccessControl method whenever you need to add or remove ACL entries from a file.
      /// </remarks>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public bool SetAccessControl(FileSecurity fileSecurity, AccessControlSections includeSections)
      {
         // In this case, equals null, is a good thing.
         return GetSetAccessControlInternal(false, true, FullPath, fileSecurity, includeSections) == null;
      }

      #endregion // SetAccessControl

      
      #region Unified Internals

      #region CopyToMoveToInternal

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="destinationPath"><para>A full path <see cref="String"/> to the destination directory</para></param>
      /// <param name="copyOptions"><para>This parameter can be <see langword="null"/>. Use <see cref="CopyOptions"/> to specify how the file is to be copied.</para></param>
      /// <param name="moveOptions"><para>This parameter can be <see langword="null"/>. Use <see cref="MoveOptions"/> that specify how the file is to be moved.</para></param>
      /// <param name="copyProgress"><para>This parameter can be <see langword="null"/>. A callback function that is called each time another portion of the file has been copied.</para></param>
      /// <param name="userProgressData"><para>This parameter can be <see langword="null"/>. The argument to be passed to the callback function.</para></param>
      /// <returns>A new <see cref="FileInfo"/> file with a fully qualified path. Returns null on failure.</returns>
      /// <remarks>The attributes of the original file are retained in the copied file.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <remarks>This Move method works across disk volumes, and it does not throw an exception if the source and destination are
      /// the same. Note that if you attempt to replace a file by moving a file of the same name into that directory, you
      /// get an IOException. You cannot use the Move method to overwrite an existing file.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      private FileInfo CopyToMoveToInternal(string destinationPath, CopyOptions? copyOptions, MoveOptions? moveOptions, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return File.CopyMoveInternal(Transaction, FullPath, destinationPath, false, copyOptions, moveOptions, copyProgress, userProgressData)
                   ? new FileInfo(Transaction, destinationPath)
                   : null;
      }

      #endregion // CopyToMoveToInternal

      #endregion // Unified Internals

      #endregion // AlphaFS

      #endregion // Methods

      #region Properties

      #region .NET

      #region Directory

      /// <summary>Gets an instance of the parent directory.</summary>
      /// <value>A <see cref="DirectoryInfo"/> object representing the parent directory of this file.</value>
      /// <remarks>To get the parent directory as a string, use the DirectoryName property.</remarks>
      public DirectoryInfo Directory
      {
         get { return new DirectoryInfo(Transaction, DirectoryName); }
      }

      #endregion // Directory

      #region DirectoryName

      /// <summary>Gets a full path string representing the file's parent directory.</summary>
      public string DirectoryName
      {
         get { return MPathInfo.DirectoryName; }
      }

      #endregion // DirectoryName

      #region Exists

      /// <summary>Gets a value indicating whether the file exists.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      public override bool Exists
      {
         get { return SystemInfo != null; }
      }

      #endregion // Exists

      //#region FullName

      ///// <summary>Gets the full path of the file.</summary>
      //public override string FullName
      //{
      //   get { return FullPath; }
      //}

      //#endregion // FullName

      #region IsReadOnly

      /// <summary>Gets or sets a value that determines if the current file is read only.</summary>
      /// <returns><c>true</c> if the current file is read only, <c>false</c> otherwise.</returns>
      public bool IsReadOnly
      {
         get { return Attributes == (FileAttributes)(-1) || NativeMethods.HasFileAttribute(Attributes, FileAttributes.ReadOnly); }

         [SecurityCritical]
         set
         {
            if (value)
               Attributes |= FileAttributes.ReadOnly;

            else if (IsReadOnly)
               Attributes = Attributes & ~FileAttributes.ReadOnly;
         }
      }

      #endregion // IsReadOnly

      #region Length

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Length

      #region Name

      /// <summary>Gets the name of the file.</summary>
      /// <remarks>
      /// The name of the file includes the file extension.
      /// When first called, FileInfo calls Refresh and caches information about the file. On subsequent calls, you must call Refresh to get the latest copy of the information.
      /// </remarks>
      public override string Name
      {
         get { return MPathInfo.FileName; }
      }

      #endregion // Name

      #endregion // .NET

      #region AlphaFS

      #region Length

      /// <summary>Gets the size, in bytes, of the current file.</summary>
      /// <returns>The size of the current file in bytes, or -1 if file doesn't exist.</returns>
      /// <remarks>AlphaFS will not throw <see cref="FileNotFoundException"/> if file doesn't exist.</remarks>
      public long Length
      {
         get
         {
            if (_length == -1)
               Refresh();

            _length = Exists ? SystemInfo.FileSize : -1;

            return _length;
         }
      }

      #endregion // Length

      #region LengthCompressed

      /// <summary>Retrieves the actual number of bytes of disk storage used to store a specified file.</summary>
      /// <returns>The size of the actual number of bytes.</returns>
      public long LengthCompressed
      {
         get
         {
            if (_lengthCompressed == -1)
               Refresh();

            _lengthCompressed = Exists ? File.GetCompressedSize(Transaction, FullPath) : -1;

            return _lengthCompressed;
         }
      }

      #endregion // LengthCompressed

      #region LengthStreams

      /// <summary>Retrieves the actual number of bytes of disk storage used by alternate data streams (NTFS ADS).</summary>
      /// <remarks>Use <see cref="FileInfo.Length"/> + <see cref="FileInfo.LengthStreams"/> = more accurate file size.</remarks>
      /// <returns>The size of the actual number of bytes used by file streams, other then the default stream.</returns>
      public long LengthStreams
      {
         get
         {
            if (_lengthStreams == -1)
               Refresh();

            _lengthStreams = Exists ? File.GetStreamsSize(Transaction, FullPath) : -1;

            return _lengthStreams;
         }
      }

      #endregion LengthStreams

      #endregion AlphaFS

      #endregion // Properties
   }
}