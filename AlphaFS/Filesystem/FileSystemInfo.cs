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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using Alphaleonis.Win32.Security;
using Microsoft.Win32.SafeHandles;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Provides the base class for both FileInfo and DirectoryInfo objects.</summary>
   [Serializable]
   [SecurityCritical]
   public abstract class FileSystemInfo : MarshalByRefObject
   {
      #region Class Internal Affairs

      #region .NET

      #region Equals

      /// <summary>Determines whether the specified Object is equal to the current Object.</summary>
      /// <param name="obj">Another object to compare to.</param>
      /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
      public override bool Equals(object obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         FileSystemInfo other = obj as FileSystemInfo;

         if (other == null)
            return false;

         return other.Name != null &&
               (other.FullName.Equals(FullName, StringComparison.OrdinalIgnoreCase) &&
                other.Attributes.Equals(Attributes) &&
                other.CreationTimeUtc.Equals(CreationTimeUtc) &&
                other.LastWriteTimeUtc.Equals(LastWriteTimeUtc));
      }

      #endregion // Equals

      #region GetHashCode

      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>A hash code for the current Object.</returns>
      public override int GetHashCode()
      {
         unchecked
         {
            int hash = !string.IsNullOrEmpty(FullName) ? FullName.GetHashCode() : 17;

            if (!string.IsNullOrEmpty(Name))
               hash = hash * 23 + Name.GetHashCode();

            hash = hash * 23 + Attributes.GetHashCode();
            hash = hash * 23 + CreationTimeUtc.GetHashCode();
            hash = hash * 23 + LastWriteTimeUtc.GetHashCode();

            return hash;
         }
      }

      #endregion // GetHashCode

      #region ==

      /// <summary>Implements the operator ==</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(FileSystemInfo left, FileSystemInfo right)
      {
         return ReferenceEquals(left, null) && ReferenceEquals(right, null) ||
                !ReferenceEquals(left, null) && !ReferenceEquals(right, null) && left.Equals(right);
      }

      #endregion // ==

      #region !=

      /// <summary>Implements the operator !=</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(FileSystemInfo left, FileSystemInfo right)
      {
         return !(left == right);
      }

      #endregion // !=

      #endregion // .NET

      #endregion // Class Internal Affairs

      #region Fields

      #region .NET

      #region FullPath

      /// <summary>Represents the fully qualified path of the directory or file.</summary>
      /// <remarks>Classes derived from <see cref="FileSystemInfo"/> can use the FullPath field to determine the full path of the object being manipulated.</remarks>
      [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      protected string FullPath;

      #endregion // FullPath

      #region OriginalPath

      /// <summary>The path originally specified by the user, whether relative or absolute.</summary>
      [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
      protected string OriginalPath;

      #endregion // OriginalPath

      #endregion // .NET

      #region AlphaFS

      #region _exists

      /// <summary>Indicator of directory or file existence. It refreshes each time Refresh() has been called.</summary>
      private bool _exists;

      #endregion // _exists

      #region _initIsDirectory

      /// <summary>The initial "IsDirectory" indicator that was passed to the constructor.</summary>
      private bool _initIsDirectory;

      #endregion // _initIsDirectory

      #region _systemInfo

      /// <summary>A <see cref="FileSystemEntryInfo"/> instance representing extended file information.</summary>
      private FileSystemEntryInfo _systemInfo;

      #endregion // _systemInfo
      
      #region MPathInfo

      /// <summary>A <see cref="PathInfo"/> instance representing a path.</summary>
      internal PathInfo MPathInfo;

      #endregion // MPathInfo
      
      #endregion // AlphaFS

      #endregion // Fields

      #region Methods

      #region .NET

      #region Delete

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Delete
      
      #region ToString

      /// <summary>Returns a string that represents the current object.</summary>
      /// <remarks>
      /// DirectoryInfo.ToString(): Returns the original path that was passed by the user.
      /// FileInfo.ToString(): The string returned by the ToString method represents path that was passed to the constructor.
      /// </remarks>
      public new virtual string ToString()
      {
         return FullPath;
      }

      #endregion // ToString

      #endregion // .NET

      #region AlphaFS

      #region Delete

      /// <summary>Deletes a file or directory.</summary>
      [SecurityCritical]
      public abstract bool Delete();

      #endregion // Delete

      #region Initialize

      /// <summary>Initializes the specified file name.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="isFolder"><c>true</c> when object is a folder.</param>
      /// <param name="path">The full path and name of the file.</param>
      internal void Initialize(KernelTransaction transaction, bool isFolder, string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         _initIsDirectory = isFolder;
         Transaction = transaction;

         // No backslash allowed for files.
         if (!isFolder)
            path = Path.DirectorySeparatorRemove(path, false);
         
         MPathInfo = new PathInfo(path, false);
         FullPath = MPathInfo.GetFullPath();
         
         //OriginalPath = isFolder ? MPathInfo.Path : MPathInfo.FileName;
         OriginalPath = path;
      }

      #endregion // Initialize

      #region Refresh

      /// <summary>Refreshes the state of the object.</summary>
      /// <remarks>
      /// FileSystemInfo.Refresh() takes a snapshot of the file from the current file system.
      /// Refresh cannot correct the underlying file system even if the file system returns incorrect or outdated information.
      /// This can happen on platforms such as Windows 98.
      /// Calls must be made to Refresh() before attempting to get the attribute information, or the information will be outdated.
      /// </remarks>
      [SecurityCritical]
      protected void Refresh()
      {
         _systemInfo = GetFileSystemEntryInfoInternal(Transaction, FullPath, false, false);

         // For the filesystem object to exist, we also want it to match the type.
         if (!(_exists = _systemInfo != null && (_initIsDirectory ? _systemInfo.IsDirectory : _systemInfo.IsFile)))
            Reset();
      }

      #endregion // Refresh

      #region VerifyObjectExists

      /// <summary>Performs a <see cref="Refresh()"/> and checks that the directory or file exists. If the filesystem object is not found, a <see cref="DirectoryNotFoundException"/> or <see cref="FileNotFoundException"/> is thrown.</summary>
      /// <exception cref="DirectoryNotFoundException"></exception>
      /// /// <exception cref="FileNotFoundException"></exception>
      private void VerifyObjectExists()
      {
         Refresh();

         if (!_exists)
         {
            string notFound = string.Format(CultureInfo.CurrentCulture, "{0} not found: {1}", _initIsDirectory ? "directory" : "file", FullPath);

            throw _initIsDirectory
                     ? (Exception)new DirectoryNotFoundException(notFound)
                     : new FileNotFoundException(notFound);
         }
      }

      #endregion // VerifyObjectExists

      #region Reset

      /// <summary>Resets the state of the filesystem object to uninitialized.</summary>
      internal void Reset()
      {
         _exists = false;
         _systemInfo = null;
      }

      #endregion // Reset

      
      #region Unified Internals

      #region CreateFileInternal

      /// <summary>Unified method CreateFileInternal() to create or open a file or I/O device. 
      /// The most commonly used I/O devices are as follows: file, file stream, directory, physical disk,
      /// volume, console buffer, tape drive, communications resource, mailslot, and pipe.
      /// </summary>
      /// <param name="isFile"></param>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path and name of the file to create.</param>
      /// <param name="share"> </param>
      /// <param name="attributes">One of the <see cref="EFileAttributes"/> values that describes how to create or overwrite the file.</param>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> instance that determines the access control and audit security for the file.</param>
      /// <param name="mode"> </param>
      /// <param name="rights"> </param>
      /// <returns>A <see cref="SafeFileHandle"/> that provides read/write access to the file specified in path or null on failure.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      internal static SafeFileHandle CreateFileInternal(bool? isFile, KernelTransaction transaction, string path, EFileAttributes attributes, FileSecurity fileSecurity, FileMode mode, FileSystemRights rights, FileShare share)
      {
         if (String.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // When isFile == null, we're working witha device.

         // When opening a VOLUME or removable media drive (for example, a floppy disk drive or flash memory thumb drive),
         // the path string should be the following form: "\\.\X:"
         // Do not use a trailing backslash (\), which indicates the root.
         
         string pathLp = (isFile == null) ? Path.DirectorySeparatorRemove(path, false) : path;

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         pathLp = Path.PrefixLongPath(pathLp);


         PrivilegeEnabler privilegeEnabler = null;
         try
         {
            // AccessSystemSecurity = 0x1000000    AccessSystemAcl access type.
            // MaximumAllowed       = 0x2000000    MaximumAllowed access type.
            if ((rights & (FileSystemRights)0x1000000) != 0)
               privilegeEnabler = new PrivilegeEnabler(Privilege.Security);

            Security.NativeMethods.SecurityAttributes securityAttributes = null;
            SafeGlobalMemoryBufferHandle securityDescriptorBuffer = new SafeGlobalMemoryBufferHandle();
            if (fileSecurity != null)
            {
               securityAttributes = new Security.NativeMethods.SecurityAttributes();
               Security.NativeMethods.SecurityAttributes.Initialize(out securityDescriptorBuffer, fileSecurity);
            }

            using (securityDescriptorBuffer)
            {
               SafeFileHandle handle = transaction == null
                                          ? NativeMethods.CreateFile(pathLp, rights, share, securityAttributes, mode, attributes, securityDescriptorBuffer)
                                          : NativeMethods.CreateFileTransacted(pathLp, rights, share, securityAttributes, mode, attributes, securityDescriptorBuffer, transaction.SafeHandle, IntPtr.Zero, IntPtr.Zero);

               NativeMethods.IsValidHandle(handle);
               return handle;
            }
         }
         finally
         {
            if (privilegeEnabler != null)
               privilegeEnabler.Dispose();
         }
      }

      #endregion // CreateFileInternal
      
      #region ExistsInternal

      /// <summary>Unified method ExistsInternal() to determine whether the given path refers to an existing directory or file on disk.</summary>
      /// <param name="isFolder"><c>true</c> indicates a folder object, <c>false</c> indicates a file object.</param>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to test.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>Note that this files may contain wildcards, such as '*'</remarks>
      /// <remarks>Return value is <c>true</c> if the caller has the required permissions and path contains the name of an existing file; otherwise, <c>false</c>.</remarks>
      /// <remarks>This method also returns <c>false</c> if path is NULL reference (Nothing in Visual Basic), an invalid path, or a zero-length string.</remarks>
      /// <remarks>If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns <c>false</c> regardless of the existence of path.</remarks>
      [SecurityCritical]
      internal static bool ExistsInternal(bool isFolder, KernelTransaction transaction, string path)
      {
         FileSystemEntryInfo fsei = GetFileSystemEntryInfoInternal(transaction, path, true, false);

         // For the filesystem object to exist, we also want it to match the type.
         return fsei != null && (isFolder ? fsei.IsDirectory : fsei.IsFile);
      }

      #endregion ExistsInternal

      #region GetCreationTimeInternal

      /// <summary>Returns the creation date and time of the specified file or directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain creation date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified file or directory. This value is expressed in local time.</returns>
      [SecurityCritical]
      internal static DateTime GetCreationTimeInternal(KernelTransaction transaction, string path)
      {
         return DateTime.FromFileTime(GetFileSystemEntryInfoInternal(transaction, path, true, true).Win32FindData.CreationTime.ToLong());
      }

      #endregion // GetCreationTime

      #region GetCreationTimeUtcInternal

      /// <summary>Gets the creation date and time, in Coordinated Universal Time (UTC) format, of a directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path of the directory.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified directory. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      internal static DateTime GetCreationTimeUtcInternal(KernelTransaction transaction, string path)
      {
         return DateTime.FromFileTimeUtc(GetFileSystemEntryInfoInternal(transaction, path, true, true).Win32FindData.CreationTime.ToLong());
      }

      #endregion // GetCreationTimeUtcInternal

      #region GetFileSystemEntryInfoInternal

      /// <summary>Unified method GetFileSystemEntryInfoInternal() to get a FileSystemEntryInfo from a Non-/Transacted folder/file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <param name="raiseException">On failure <c>true</c> will throw an <see cref="NativeError.ThrowException()"/> while <c>false</c> will return null without throwing any exceptions.</param>
      /// <returns>The <see cref="FileSystemEntryInfo"/> instance of the folder or file on the path.</returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SecurityCritical]
      internal static FileSystemEntryInfo GetFileSystemEntryInfoInternal(KernelTransaction transaction, string path, bool basicSearch, bool raiseException)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string orgPathLp = Path.PrefixLongPath(path);
         string pathLp = orgPathLp;

         // A trailing backslash is not allowed and will be removed.
         pathLp = Path.DirectorySeparatorRemove(pathLp, false);

         NativeMethods.Win32FindData win32FindData = new NativeMethods.Win32FindData();

         NativeMethods.FindExInfoLevels findExInfoLevelsFlag = basicSearch
                                                                  ? NativeMethods.FindExInfoLevels.Basic
                                                                  : NativeMethods.FindExInfoLevels.Standard;

         NativeMethods.FindExAdditionalFlags findExAdditionalFlag = NativeMethods.LargeCache
                                                                       ? NativeMethods.FindExAdditionalFlags.LargeFetch
                                                                       : NativeMethods.FindExAdditionalFlags.None;

         bool allOk = true;

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
         using (SafeFindFileHandle handle = transaction == null
                                               ? NativeMethods.FindFirstFileEx(pathLp, findExInfoLevelsFlag, ref win32FindData, NativeMethods.FindExSearchOps.SearchNameMatch, IntPtr.Zero, findExAdditionalFlag)
                                               : NativeMethods.FindFirstFileTransacted(pathLp, findExInfoLevelsFlag, ref win32FindData, NativeMethods.FindExSearchOps.SearchNameMatch, IntPtr.Zero, findExAdditionalFlag, transaction.SafeHandle))
         {
            // There are couple of common scenarios where this can fail on valid paths like "C:\"
            // and network root share names like: \\server\sharename, will use GetExtendedAttributes in this case.
            if (!NativeMethods.IsValidHandle(handle, false))
            {
               NativeMethods.Win32FileAttributeData win32AttrData = new NativeMethods.Win32FileAttributeData();

               // Reset "pathLp" in case of a backslash that is originally present.
               pathLp = orgPathLp;

               // 0 == GetFileExInfoStandard
               if (transaction == null
                      ? NativeMethods.GetFileAttributesEx(pathLp, 0, ref win32AttrData)
                      : NativeMethods.GetFileAttributesTransacted(pathLp, 0, ref win32AttrData, transaction.SafeHandle))
               {
                  win32FindData.FileName = new PathInfo(pathLp, false).FileName;
                  win32FindData.AlternateFileName = string.Empty;
                  win32FindData.FileAttributes = win32AttrData.FileAttributes;
                  win32FindData.CreationTime = win32AttrData.CreationTime;
                  win32FindData.LastAccessTime = win32AttrData.LastAccessTime;
                  win32FindData.LastWriteTime = win32AttrData.LastWriteTime;
                  win32FindData.FileSizeHigh = win32AttrData.FileSizeHigh;
                  win32FindData.FileSizeLow = win32AttrData.FileSizeLow;
               }
               else
               {
                  allOk = false;

                  if (raiseException)
                     NativeError.ThrowException(pathLp);
               }
            }
         }

         if (!allOk)
            return null;

         // At this point, pathLp *assumably* contains the full path so we might as well copy it.
         // http://alphafs.codeplex.com/discussions/252049

         string pathRp = Path.GetRegularPath(pathLp);
         return new FileSystemEntryInfo(win32FindData)
         {
            FullPath = Path.IsPathRooted(pathRp) && !win32FindData.FileName.Equals(pathRp, StringComparison.OrdinalIgnoreCase)
               ? pathRp
               : string.Empty
         };
      }

      #endregion // GetFileSystemEntryInfoInternal

      #region EnumerateFileSystemObjectsInternal

      /// <summary>Unified method EnumerateFileSystemObjectsInternal() to enumerate Non-/Transacted directories/files.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <param name="getFolders">When <c>true</c>, folders will be returned, when <c>false</c>, files will be returned. When <see langword="null"/> both folders and files will be returned.</param>
      /// <param name="getPath">When <c>true</c>, returns the results as an enumerable <see langref="string"/> object, when <c>false</c> the returned enumerable is of a <see cref="FileSystemInfo"/> object.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>
      /// If <paramref name="getPath"/> is <c>true</c>, an enumerable <see langref="string"/> collection of the full pathnames that match searchPattern and searchOption.
      /// If <paramref name="getPath"/> is <c>false</c>, an enumerable <see cref="FileSystemInfo"/> (<see cref="DirectoryInfo"/> / <see cref="FileInfo"/>) collection that match searchPattern and searchOption.
      /// </returns>
      [SecurityCritical]
      internal static IEnumerable<object> EnumerateFileSystemObjectsInternal(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool basicSearch, bool? getFolders, bool getPath, bool continueOnAccessError)
      {
         // Verify that BasicSearch is available on Operating System.
         if (basicSearch)
            basicSearch = NativeMethods.BasicSearch;

         foreach (FileSystemEntryInfo fsei in new FileSystemEntry
            {
         BasicSearch = basicSearch,
         ContinueOnAccessError = continueOnAccessError,
         GetFsoType = getFolders,
         InputPath = path,
         LargeCache = NativeMethods.LargeCache,
         SearchOption = searchOption,
         SearchPattern = searchPattern,
         Transaction = transaction

      }.Enumerate())
         {
            string fullPath = fsei.FullPath;

            // Return full path as a type: string.
            if (getPath)
               yield return fullPath;

               // Return a specific instance of type: FileSystemInfo, DirectoryInfo or FileInfo.
               // Bonus: the returned FileSystemEntryInfo instance is constructed from a Win32FindData data structure
               // with properties already populated by the Win32 FindFirstFileEx() function.
               // This means that the returned DirectoryInfo/FileInfo instance is already .Refresh()-ed.
               // I call it: Cached LazyLoading.
            else switch (getFolders)
               {
                  // null = return instances of type: DirectoryInfo and FileInfo.
                  case null:
                     yield return fsei.IsDirectory
                                       ? (FileSystemInfo)new DirectoryInfo(transaction, fullPath) { _systemInfo = fsei, _exists = true, _initIsDirectory = true }
                                       : new FileInfo(transaction, fullPath) { _systemInfo = fsei, _exists = true, _initIsDirectory = false };
                     break;

                  // true = return instance of type: DirectoryInfo.
                  case true:
                     yield return new DirectoryInfo(transaction, fullPath) { _systemInfo = fsei, _exists = true, _initIsDirectory = true };
                     break;

                  // false = return instance of type: FileInfo.
                  case false:
                     yield return new FileInfo(transaction, fullPath) { _systemInfo = fsei, _exists = true, _initIsDirectory = false };
                     break;
               }
         }
      }

      #endregion // EnumerateFileSystemObjectsInternal

      #region GetLastAccessTimeInternal

      /// <summary>Returns the date and time the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in local time.</returns>
      [SecurityCritical]
      internal static DateTime GetLastAccessTimeInternal(KernelTransaction transaction, string path)
      {
         return DateTime.FromFileTime(GetFileSystemEntryInfoInternal(transaction, path, true, true).Win32FindData.LastAccessTime.ToLong());
      }

      #endregion // GetLastAccessTimeInternal

      #region GetLastAccessTimeUtcInternal

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      internal static DateTime GetLastAccessTimeUtcInternal(KernelTransaction transaction, string path)
      {
         return DateTime.FromFileTimeUtc(GetFileSystemEntryInfoInternal(transaction, path, true, true).Win32FindData.LastAccessTime.ToLong());
      }

      #endregion // GetLastAccessTimeUtcInternal

      #region GetLastWriteTimeInternal

      /// <summary>Returns the date and time the specified file or directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in local time.</returns>
      [SecurityCritical]
      internal static DateTime GetLastWriteTimeInternal(KernelTransaction transaction, string path)
      {
         return DateTime.FromFileTime(GetFileSystemEntryInfoInternal(transaction, path, true, true).Win32FindData.LastWriteTime.ToLong());
      }

      #endregion // GetLastWriteTimeInternal

      #region GetLastWriteTimeUtcInternal

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      internal static DateTime GetLastWriteTimeUtcInternal(KernelTransaction transaction, string path)
      {
         return DateTime.FromFileTimeUtc(GetFileSystemEntryInfoInternal(transaction, path, true, true).Win32FindData.LastWriteTime.ToLong());
      }

      #endregion // GetLastWriteTimeUtcInternal

      #region GetSetAccessControlInternal

      /// <summary>Unified method GetSetAccessControlInternal() to apply access control list (ACL) entries described by a <see cref="DirectorySecurity"/> or <see cref="FileSecurity"/> object to the specified file or directory.</summary>
      /// <param name="isFolder"><c>true</c> indicates a folder object, <c>false</c> indicates a file object.</param>
      /// <param name="isSet"><c>true</c> indicates a get execution, <c>false</c> indicates a set execution.</param>
      /// <param name="path">A file or directory to add or remove access control list (ACL) entries from.</param>
      /// <param name="fileSecurity">A <see cref="ObjectSecurity "/> object that describes an ACL entry to apply to the directory described by the path parameter.</param>
      /// <param name="includeSections">One or more of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to set.</param>
      /// <returns>An <see cref="ObjectSecurity"/> on get execution, <see langword="null"/> on set execution.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static ObjectSecurity GetSetAccessControlInternal(bool isFolder, bool isSet, string path, ObjectSecurity fileSecurity, AccessControlSections includeSections)
      {
         if (isSet)
         {
            if (Security.NativeMethods.SetAccessControlInternal(path, null, fileSecurity, includeSections))
               return null; // In this case, equals null, is a good thing.
         }

         return Security.NativeMethods.GetAccessControlInternal(isFolder, path, includeSections);
      }

      #endregion // GetSetAccessControlInternal

      #region SetAttributesInternal

      /// <summary>Unified method SetAttributesInternal() to set the attributes for a Non-/Transacted file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file whose attributes are to be set.</param>
      /// <param name="fileAttributes">The file attributes to set for the file. Note that all other values override <see cref="FileAttributes.Normal"/>.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool SetAttributesInternal(KernelTransaction transaction, string path, FileAttributes fileAttributes)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string pathLp = Path.PrefixLongPath(path);

         bool setAttr = transaction == null
                           ? NativeMethods.SetFileAttributes(pathLp, fileAttributes)
                           : NativeMethods.SetFileAttributesTransacted(pathLp, fileAttributes, transaction.SafeHandle);

         if (!setAttr)
            NativeError.ThrowException(pathLp);

         return setAttr;
      }

      #endregion // SetAttributesInternal

      #region SetFileTimeInternal

      /// <summary>Unified method SetFileTimeInternal() for setting file times on a file.</summary>
      /// <param name="isFolder"></param>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <returns><c>true</c> on success, <c>false</c>otherwise.</returns>
      /// <remarks>This method uses <see cref="EFileAttributes.BackupSemantics"/> flag to write Timestamps to folders as well.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool SetFileTimeInternal(bool isFolder, KernelTransaction transaction, string path, long? creationTime, long? lastAccessTime, long? lastWriteTime)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string pathLp = Path.PrefixLongPath(path);

         using (SafeGlobalMemoryBufferHandle hCreationTime = SafeGlobalMemoryBufferHandle.CreateFromLong(creationTime))
         using (SafeGlobalMemoryBufferHandle hLastAccessTime = SafeGlobalMemoryBufferHandle.CreateFromLong(lastAccessTime))
         using (SafeGlobalMemoryBufferHandle hLastWriteTime = SafeGlobalMemoryBufferHandle.CreateFromLong(lastWriteTime))

         // To open a directory using CreateFile, specify the FILE_FLAG_BACKUP_SEMANTICS flag as part of dwFlagsAndAttributes.
         using (SafeFileHandle handle = CreateFileInternal(!isFolder, transaction, pathLp, isFolder ? EFileAttributes.BackupSemantics : EFileAttributes.Normal, null, FileMode.Open, FileSystemRights.WriteAttributes, FileShare.Delete | FileShare.Write))
            if (!NativeMethods.SetFileTime(handle, hCreationTime, hLastAccessTime, hLastWriteTime))
               NativeError.ThrowException(pathLp);

         return true;
      }

      #endregion // SetFileTimeInternal

      #region TransferTimestampsInternal

      /// <summary>Unified method TransferTimestampsInternal() to transfer the time stamps for files and directories.</summary>
      /// <param name="isFolder"></param>
      /// <param name="transaction">The transaction.</param>
      /// <param name="source">The source path.</param>
      /// <param name="destination">The destination path.</param>
      /// <returns><c>true</c> on success, <c>false</c>otherwise.</returns>
      /// <remarks>
      /// This method uses BackupSemantics flag to get Timestamp changed for folders.
      /// This method does not change last access time for the source file.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool TransferTimestampsInternal(bool isFolder, KernelTransaction transaction, string source, string destination)
      {
         FileSystemEntryInfo info = GetFileSystemEntryInfoInternal(transaction, source, true, true);
         return SetFileTimeInternal(isFolder, transaction, destination, info.Win32FindData.CreationTime.ToLong(), info.Win32FindData.LastAccessTime.ToLong(), info.Win32FindData.LastWriteTime.ToLong());
      }

      #endregion // TransferTimestampsInternal

      #endregion // Unified Internals

      #endregion // AlphaFS

      #endregion // Methods

      #region Properties

      #region .NET

      #region Attributes

      /// <summary>Gets or sets the attributes for the current file or directory.</summary>
      /// <exception cref="NativeError.ThrowException()"/>
      public FileAttributes Attributes
      {
         get
         {
            if (_systemInfo == null)
               Refresh();

            return _exists ? _systemInfo.Attributes : (FileAttributes)(-1);
         }

         [SecurityCritical]
         protected set
         {
            if (_systemInfo == null)
               VerifyObjectExists();

            if (SetAttributesInternal(Transaction, FullPath, value))
               Reset();
         }
      }

      #endregion // Attributes

      #region CreationTime

      /// <summary>Gets or sets the creation time of the current file or directory.</summary>
      public DateTime CreationTime
      {
         get { return CreationTimeUtc.ToLocalTime(); }

         [SecurityCritical] protected set { CreationTimeUtc = value.ToUniversalTime(); }
      }

      #endregion // CreationTime

      #region CreationTimeUtc

      /// <summary>Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.</summary>
      [ComVisible(false)]
      public DateTime CreationTimeUtc
      {
         get
         {
            if (_systemInfo == null)
               Refresh();

            return DateTime.FromFileTimeUtc(_exists ? _systemInfo.Win32FindData.CreationTime : new NativeMethods.Win32FindData().CreationTime);
         }

         [SecurityCritical]
         protected set
         {
            if (_systemInfo == null)
               VerifyObjectExists();

            if (SetFileTimeInternal(true, Transaction, FullPath, value.ToFileTimeUtc(), null, null))
               Reset();
         }
      }

      #endregion // CreationTimeUtc

      #region Exists

      /// <summary>Gets a value indicating whether the file or directory exists.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>The <see cref="Exists"/> property returns false if any error occurs while trying to determine if the specified directory or file exists. This can occur in situations that raise exceptions such as passing a directory- or file name with invalid characters or too many characters, a failing or missing disk, or if the caller does not have permission to read the directory or file.</remarks>
      public abstract bool Exists { get; }

      #endregion // Exists

      #region Extension

      /// <summary>Gets the string representing the extension part of the file.</summary>
      public string Extension
      {
         get { return MPathInfo.Extension; }
      }

      #endregion // Extension

      #region FullName

      /// <summary>Gets the full path of the directory or file.</summary>
      public virtual string FullName
      {
         get { return FullPath; }
      }

      #endregion // FullName

      #region LastAccessTime

      /// <summary>Gets or sets the time the current file or directory was last accessed.</summary>
      /// <remarks>When first called, <see cref="FileSystemInfo"/> calls Refresh and returns the cached information on APIs to get attributes and so on. On subsequent calls, you must call Refresh to get the latest copy of the information. 
      /// If the file described in the <see cref="FileSystemInfo"/> object does not exist, this property will return 12:00 midnight, January 1, 1601 A.D. (C.E.) Coordinated Universal Time (UTC), adjusted to local time. 
      /// </remarks>
      public DateTime LastAccessTime
      {
         get { return LastAccessTimeUtc.ToLocalTime(); }

         [SecurityCritical]
         protected set { LastAccessTimeUtc = value.ToUniversalTime(); }
      }

      #endregion // LastAccessTime

      #region LastAccessTimeUtc

      /// <summary>Gets or sets the time, in coordinated universal time (UTC), that the current file or directory was last accessed.</summary>
      /// <remarks>When first called, <see cref="FileSystemInfo"/> calls Refresh and returns the cached information on APIs to get attributes and so on. On subsequent calls, you must call Refresh to get the latest copy of the information. 
      /// If the file described in the <see cref="FileSystemInfo"/> object does not exist, this property will return 12:00 midnight, January 1, 1601 A.D. (C.E.) Coordinated Universal Time (UTC), adjusted to local time. 
      /// </remarks>
      [ComVisible(false)]
      public DateTime LastAccessTimeUtc
      {
         get
         {
            if (_systemInfo == null)
               Refresh();

            return DateTime.FromFileTimeUtc(_exists ? _systemInfo.Win32FindData.LastAccessTime : new NativeMethods.Win32FindData().LastAccessTime);
         }

         [SecurityCritical]
         protected set
         {
            if (_systemInfo == null)
               VerifyObjectExists();

            if (SetFileTimeInternal(true, Transaction, FullPath, null, value.ToFileTimeUtc(), null))
               Reset();
         }
      }

      #endregion // LastAccessTimeUtc

      #region LastWriteTime

      /// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
      public DateTime LastWriteTime
      {
         get { return LastWriteTimeUtc.ToLocalTime(); }

         [SecurityCritical]
         protected set { LastWriteTimeUtc = value.ToUniversalTime(); }
      }

      #endregion // LastWriteTime

      #region LastWriteTimeUtc

      /// <summary>Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to.</summary>
      [ComVisible(false)]
      public DateTime LastWriteTimeUtc
      {
         get
         {
            if (_systemInfo == null)
               Refresh();

            return DateTime.FromFileTimeUtc(_exists ? _systemInfo.Win32FindData.LastWriteTime : new NativeMethods.Win32FindData().LastWriteTime);
         }

         [SecurityCritical]
         protected set
         {
            if (_systemInfo == null)
               VerifyObjectExists();

            if (SetFileTimeInternal(true, Transaction, FullPath, null, null, value.ToFileTimeUtc()))
               Reset();
         }
      }

      #endregion // LastWriteTimeUtc

      #region Name

      /// <summary>For files, gets the name of the file. For directories, gets the name of the last directory in the hierarchy if a hierarchy exists. Otherwise, the Name property gets the name of the directory.</summary>
      /// <remarks>
      /// For a directory, Name returns only the name of the parent directory, such as Dir, not c:\Dir. For a subdirectory, Name returns only the name of the subdirectory, such as Sub1, not c:\Dir\Sub1
      /// For a file, Name returns only the file name and file name extension, such as MyFile.txt, not c:\Dir\Myfile.txt
      /// </remarks>
      public abstract string Name { get; }
      
      #endregion // Name

      #endregion // .NET

      #region AlphaFS

      #region SystemInfo

      /// <summary>Gets the instance of the <see cref="FileSystemEntryInfo"/> class.</summary>
      public FileSystemEntryInfo SystemInfo
      {
         get
         {
            if (_systemInfo == null)
               Refresh();

            return _systemInfo;
         }
      }

      #endregion // SystemInfo

      #region Transaction

      /// <summary>Represents the KernelTransaction that was passed to the constructor.</summary>
      internal KernelTransaction Transaction { get; private set; }

      #endregion // Transaction

      #endregion // AlphaFS

      #endregion // Properties
   }
}