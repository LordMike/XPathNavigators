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
using System.Security;
using System.Security.AccessControl;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Exposes instance methods for creating, moving, and enumerating through directories and subdirectories. This class cannot be inherited.</summary>
   [Serializable]
   [SecurityCritical]
   public sealed class DirectoryInfo : FileSystemInfo
   {
      #region Constructors

      #region DirectoryInfo

      #region .NET

      /// <summary>Initializes a new instance of the DirectoryInfo class on the specified path.</summary>
      /// <param name="path">A string specifying the path on which to create the <see cref="System.IO.DirectoryInfo"/>.</param>
      /// <remarks>
      /// This constructor does not check if a directory exists. This constructor is a placeholder for a string that is used to access the disk in subsequent operations.
      /// The path parameter can be a file name, including a file on a Universal Naming Convention (UNC) share.
      /// </remarks>
      public DirectoryInfo(string path)
      {
         Initialize(null, true, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Initializes a new instance of the DirectoryInfo class on the specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A string specifying the path on which to create the <see cref="System.IO.DirectoryInfo"/>.</param>
      /// <remarks>
      /// This constructor does not check if a directory exists. This constructor is a placeholder for a string that is used to access the disk in subsequent operations.
      /// The path parameter can be a file name, including a file on a Universal Naming Convention (UNC) share.
      /// </remarks>
      public DirectoryInfo(KernelTransaction transaction, string path)
      {
         Initialize(transaction, true, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // DirectoryInfo

      #endregion // Constructors

      #region Methods

      #region .NET

      #region Create

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Create

      #region CreateSubdirectory

      /// <summary>Creates a subdirectory or subdirectories on the specified path. The specified path can be relative to this instance of the DirectoryInfo class.</summary>
      /// <param name="path">The specified path. This cannot be a different disk volume or Universal Naming Convention (UNC) name.</param>
      /// <returns>The last directory specified in path as an <see cref="DirectoryInfo"/> object.</returns>
      /// <remarks>
      /// Any and all directories specified in path are created, unless some part of path is invalid.
      /// The path parameter specifies a directory path, not a file path.
      /// If the subdirectory already exists, this method does nothing.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public DirectoryInfo CreateSubdirectory(string path)
      {
         return CreateSubdirectory(path, null);
      }

      /// <summary>Creates a subdirectory or subdirectories on the specified path. The specified path can be relative to this instance of the DirectoryInfo class.</summary>
      /// <param name="path">The specified path. This cannot be a different disk volume or Universal Naming Convention (UNC) name.</param>
      /// <param name="directorySecurity">The <see cref="DirectorySecurity"/> security to apply.</param>
      /// <returns>The last directory specified in path as an <see cref="DirectoryInfo"/> object.</returns>
      /// <remarks>
      /// Any and all directories specified in path are created, unless some part of path is invalid.
      /// The path parameter specifies a directory path, not a file path.
      /// If the subdirectory already exists, this method does nothing.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public DirectoryInfo CreateSubdirectory(string path, DirectorySecurity directorySecurity)
      {
         string subDir = Path.Combine(FullPath, path);
         DirectoryInfo newDir = new DirectoryInfo(Transaction, subDir);

         return ExistsInternal(true, Transaction, subDir)
                   ? newDir
                   : Directory.CreateDirectoryInternal(Transaction, null, subDir, directorySecurity, null)
                         ? newDir
                         : null;
      }

      #endregion // CreateSubdirectory

      #region Delete

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Delete

      #region EnumerateDirectories

      /// <summary>Returns an enumerable collection of directory information in the current directory.</summary>
      /// <returns>An enumerable collection of type <see cref="DirectoryInfo"/> directories in the current directory.</returns>
      [SecurityCritical]
      public IEnumerable<DirectoryInfo> EnumerateDirectories()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, false, false).Cast<DirectoryInfo>();
      }

      /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An enumerable collection of type <see cref="DirectoryInfo"/> directories that matches searchPattern.</returns>
      [SecurityCritical]
      public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, true, true, false, false).Cast<DirectoryInfo>();
      }

      /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable collection of type <see cref="DirectoryInfo"/> directories that matches searchPattern and searchOption.</returns>
      [SecurityCritical]
      public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, true, false, false).Cast<DirectoryInfo>();
      }

      #region AlphaFS

      /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable collection of type <see cref="DirectoryInfo"/> directories that matches searchPattern and searchOption.</returns>
      [SecurityCritical]
      public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, true, false, continueOnAccessError).Cast<DirectoryInfo>();
      }

      #endregion // AlphaFS

      #endregion // EnumerateDirectories

      #region EnumerateFiles

      /// <summary>Returns an enumerable collection of file information in the current directory.</summary>
      /// <returns>An enumerable collection of type <see cref="FileInfo"/> files in the current directory.</returns>
      [SecurityCritical]
      public IEnumerable<FileInfo> EnumerateFiles()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, false, false).Cast<FileInfo>();
      }

      /// <summary>Returns an enumerable collection of file information that matches a search pattern.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An enumerable collection of type <see cref="FileInfo"/> files that matches searchPattern.</returns>
      [SecurityCritical]
      public IEnumerable<FileInfo> EnumerateFiles(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, true, false, false, false).Cast<FileInfo>();
      }

      /// <summary>Returns an enumerable collection of file information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable collection of type <see cref="FileInfo"/> files that matches searchPattern and searchOption.</returns>
      [SecurityCritical]
      public IEnumerable<FileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, false, false, false).Cast<FileInfo>();
      }

      #region AlphaFS

      /// <summary>Returns an enumerable collection of file information in the current directory.</summary>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable collection of type <see cref="FileInfo"/> files in the current directory.</returns>
      [SecurityCritical]
      public IEnumerable<FileInfo> EnumerateFiles(bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, false, continueOnAccessError).Cast<FileInfo>();
      }

      /// <summary>Returns an enumerable collection of directory information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable collection of type <see cref="FileInfo"/> files that matches searchPattern and searchOption.</returns>
      [SecurityCritical]
      public IEnumerable<FileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, false, false, continueOnAccessError).Cast<FileInfo>();
      }

      #endregion // AlphaFS

      #endregion // EnumerateFiles

      #region EnumerateFileSystemInfos

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> information in the current directory.</summary>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> information in the current directory.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, null, false, false).Cast<FileSystemInfo>();
      }

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> information objects that matches searchPattern.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, true, null, false, false).Cast<FileSystemInfo>();
      }

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> information objects that matches searchPattern and searchOption.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, null, false, false).Cast<FileSystemInfo>();
      }

      #region AlphaFS

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> information objects that matches searchPattern and searchOption.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, null, false, continueOnAccessError).Cast<FileSystemInfo>();
      }

      #endregion // AlphaFS

      #endregion // EnumerateFileSystemInfos

      #region GetAccessControl

      /// <summary>Gets a <see cref="DirectorySecurity"/> object that encapsulates the access control list (ACL) entries for the directory described by the current DirectoryInfo object.</summary>
      /// <returns>A <see cref="DirectorySecurity"/> object that encapsulates the access control rules for the directory.</returns>
      [SecurityCritical]
      public DirectorySecurity GetAccessControl()
      {
         return (DirectorySecurity)GetSetAccessControlInternal(true, false, FullPath, null, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner);
      }

      /// <summary>Gets a <see cref="DirectorySecurity"/> object that encapsulates the specified type of access control list (ACL) entries for the directory described by the current <see cref="DirectoryInfo"/> object.</summary>
      /// <param name="includeSections">One of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to receive.</param>
      /// <returns>A <see cref="DirectorySecurity"/> object that encapsulates the access control rules for the file described by the path parameter.</returns>
      [SecurityCritical]
      public DirectorySecurity GetAccessControl(AccessControlSections includeSections)
      {
         return (DirectorySecurity)GetSetAccessControlInternal(true, false, FullPath, null, includeSections);
      }

      #endregion // GetAccessControl

      #region GetDirectories

      /// <summary>Returns the subdirectories of the current directory.</summary>
      /// <returns>An array of <see cref="DirectoryInfo"/> objects. If there are no subdirectories, this method returns an empty array. This method is not recursive.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public DirectoryInfo[] GetDirectories()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, false, false).Cast<DirectoryInfo>().ToArray();
      }

      /// <summary>Returns an array of directories in the current <see cref="DirectoryInfo"/> matching the given search criteria.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An array of <see cref="DirectoryInfo"/> objects matching searchPattern. If there are no subdirectories, this method returns an empty array. This method is not recursive.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public DirectoryInfo[] GetDirectories(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, true, true, false, false).Cast<DirectoryInfo>().ToArray();
      }

      /// <summary>Returns an array of directories in the current <see cref="DirectoryInfo"/> matching the given search criteria and using a value to determine whether to search subdirectories.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An array of <see cref="DirectoryInfo"/> objects matching searchPattern. If there are no subdirectories, this method returns an empty array. This method is not recursive.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, true, false, false).Cast<DirectoryInfo>().ToArray();
      }

      #region AlphaFS

      /// <summary>Returns an array of directories in the current <see cref="DirectoryInfo"/> matching the given search criteria and using a value to determine whether to search subdirectories.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An array of <see cref="DirectoryInfo"/> objects matching searchPattern. If there are no subdirectories, this method returns an empty array. This method is not recursive.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, true, false, continueOnAccessError).Cast<DirectoryInfo>().ToArray();
      }

      #endregion // AlphaFS

      #endregion // GetDirectories

      #region GetFiles

      /// <summary>Returns a file list from the current directory.</summary>
      /// <returns>An array of type <see cref="FileInfo"/>.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public FileInfo[] GetFiles()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, false, false).Cast<FileInfo>().ToArray();
      }

      /// <summary>Returns a file list from the current directory matching the given search pattern.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An array of type <see cref="FileInfo"/>.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public FileInfo[] GetFiles(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, true, false, false, false).Cast<FileInfo>().ToArray();
      }

      /// <summary>Returns a file list from the current directory matching the given search pattern and using a value to determine whether to search subdirectories.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An array of type <see cref="FileInfo"/>.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, false, false, false).Cast<FileInfo>().ToArray();
      }

      #region AlphaFS

      /// <summary>Returns a file list from the current directory matching the given search pattern and using a value to determine whether to search subdirectories.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An array of type <see cref="FileInfo"/>.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, false, false, continueOnAccessError).Cast<FileInfo>().ToArray();
      }

      #endregion // AlphaFS

      #endregion // GetFiles

      #region GetFileSystemInfos

      /// <summary>Returns an array of strongly typed <see cref="FileSystemInfo "/> entries representing all the files and subdirectories in a directory.</summary>
      /// <returns>An array of strongly typed <see cref="FileSystemInfo "/> entries.</returns>
      /// <remarks>
      /// If there are no files or directories in the DirectoryInfo, this method returns an empty array. This method is not recursive.
      /// For subdirectories, the FileSystemInfo objects returned by this method can be cast to the derived class DirectoryInfo.
      /// Use the FileAttributes value returned by the Attributes property to determine whether the FileSystemInfo represents a file or a directory.
      /// </remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public FileSystemInfo[] GetFileSystemInfos()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullName, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, null, false, false).Cast<FileSystemInfo>().ToArray();
      }

      /// <summary>Retrieves an array of strongly typed <see cref="FileSystemInfo "/> objects representing the files and subdirectories that match the specified search criteria.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An array of strongly typed <see cref="FileSystemInfo "/> entries.</returns>
      /// <remarks>
      /// If there are no files or directories in the DirectoryInfo, this method returns an empty array. This method is not recursive.
      /// For subdirectories, the FileSystemInfo objects returned by this method can be cast to the derived class DirectoryInfo.
      /// Use the FileAttributes value returned by the Attributes property to determine whether the FileSystemInfo represents a file or a directory.
      /// </remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public FileSystemInfo[] GetFileSystemInfos(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullName, searchPattern, SearchOption.TopDirectoryOnly, true, null, false, false).Cast<FileSystemInfo>().ToArray();
      }

      /// <summary>Retrieves an array of strongly typed <see cref="FileSystemInfo "/> objects representing the files and subdirectories that match the specified search criteria.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An array of strongly typed <see cref="FileSystemInfo "/> entries.</returns>
      /// <remarks>
      /// If there are no files or directories in the DirectoryInfo, this method returns an empty array. This method is not recursive.
      /// For subdirectories, the FileSystemInfo objects returned by this method can be cast to the derived class DirectoryInfo.
      /// Use the FileAttributes value returned by the Attributes property to determine whether the FileSystemInfo represents a file or a directory.
      /// </remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullName, searchPattern, searchOption, true, null, false, false).Cast<FileSystemInfo>().ToArray();
      }

      #region AlphaFS

      /// <summary>Retrieves an array of strongly typed <see cref="FileSystemInfo "/> objects representing the files and subdirectories that match the specified search criteria.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An array of strongly typed <see cref="FileSystemInfo "/> entries.</returns>
      /// <remarks>
      /// If there are no files or directories in the DirectoryInfo, this method returns an empty array. This method is not recursive.
      /// For subdirectories, the FileSystemInfo objects returned by this method can be cast to the derived class DirectoryInfo.
      /// Use the FileAttributes value returned by the Attributes property to determine whether the FileSystemInfo represents a file or a directory.
      /// </remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullName, searchPattern, searchOption, true, null, false, continueOnAccessError).Cast<FileSystemInfo>().ToArray();
      }

      #endregion // AlphaFS

      #endregion // GetFileSystemInfos

      #region MoveTo

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // MoveTo

      #region Refresh

      /// <summary>Refreshes the state of the object.</summary>
      [SecurityCritical]
      public new void Refresh()
      {
         base.Refresh();
      }

      #endregion // Refresh

      #region SetAccessControl

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // SetAccessControl

      #region ToString

      /// <summary>Returns the original path that was passed by the user.</summary>
      public override string ToString()
      {
         return OriginalPath;
      }

      #endregion // ToString

      #endregion // .NET

      #region AlphaFS

      #region CopyTo

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool CopyTo(string destinationPath)
      {
         return CopyToMoveToInternal(destinationPath, false, NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Copy will fail on existing folders or files.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool CopyTo(string destinationPath, bool overwrite)
      {
         return CopyToMoveToInternal(destinationPath, false, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Copy will fail on existing folders or files.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves ACLs information.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool CopyTo(string destinationPath, bool overwrite, bool preserveSecurity)
      {
         return CopyToMoveToInternal(destinationPath, preserveSecurity, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the file is to be copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves ACLs information.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool CopyTo(string destinationPath, CopyOptions copyOptions, bool preserveSecurity, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyToMoveToInternal(destinationPath, preserveSecurity, copyOptions, null, copyProgress, userProgressData);
      }

      #endregion // CopyTo

      #region CountDirectories

      /// <summary>Counts directories in a given directory.</summary>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public long CountDirectories()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public long CountDirectories(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public long CountDirectories(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>The counted number of directories.</returns>
      [SecurityCritical]
      public long CountDirectories(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, true, true, continueOnAccessError).Count();
      }

      #endregion // CountDirectories

      #region CountFiles

      /// <summary>Counts files in a given directory.</summary>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public long CountFiles()
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public long CountFiles(string searchPattern)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public long CountFiles(string searchPattern, SearchOption searchOption)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>The counted number of files.</returns>
      [SecurityCritical]
      public long CountFiles(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateFileSystemObjectsInternal(Transaction, FullPath, searchPattern, searchOption, true, false, true, continueOnAccessError).Count();
      }

      #endregion // CountFiles

      #region Compress

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only compress the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Compress()
      {
         return Directory.CompressDecompressInternal(true, Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only compress the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Compress(string searchPattern)
      {
         return Directory.CompressDecompressInternal(true, Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Compress(string searchPattern, SearchOption searchOption)
      {
         return Directory.CompressDecompressInternal(true, Transaction, FullPath, searchPattern, searchOption, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Compress(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return Directory.CompressDecompressInternal(true, Transaction, FullPath, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Compress

      #region CompressionDisable

      /// <summary>Disables compression of the specified directory and the files in it.</summary>
      /// <remarks>
      /// This method disables the folder-compression attribute. It will not decompress the current contents of the folder.
      /// However, newly created files and folders will be uncompressed.
      /// </remarks>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool CompressionDisable()
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(true, Transaction, FullPath, false);
      }

      #endregion // CompressionDisable

      #region CompressionEnable

      /// <summary>Enables compression of the specified directory and the files in it.</summary>
      /// <remarks>
      /// This method enables the folder-compression attribute. It will not compress the current contents of the folder.
      /// However, newly created files and folders will be compressed.
      /// </remarks>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool CompressionEnable()
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(true, Transaction, FullPath, true);
      }

      #endregion // CompressionEnable

      #region Create

      /// <summary>Creates a directory.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool Create()
      {
         bool createOk = Directory.CreateDirectoryInternal(Transaction, null, FullPath, null, null);
         Refresh();
         return createOk;
      }

      /// <summary>Creates a directory using a <see cref="DirectorySecurity"/> object.</summary>
      /// <param name="directorySecurity">The access control to apply to the directory.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public bool Create(DirectorySecurity directorySecurity)
      {
         bool createOk = Directory.CreateDirectoryInternal(Transaction, null, FullPath, directorySecurity, null);
         Refresh();
         return createOk;
      }

      #endregion // Create

      #region Decompress

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decompress the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Decompress()
      {
         return Directory.CompressDecompressInternal(false, Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decompress the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Decompress(string searchPattern)
      {
         return Directory.CompressDecompressInternal(false, Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Decompress(string searchPattern, SearchOption searchOption)
      {
         return Directory.CompressDecompressInternal(false, Transaction, FullPath, searchPattern, searchOption, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Decompress(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return Directory.CompressDecompressInternal(false, Transaction, FullPath, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Decompress

      #region Decrypt

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decrypt the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Decrypt()
      {
         return Directory.EncryptDecryptInternal(false, Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decrypt the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Decrypt(string searchPattern)
      {
         return Directory.EncryptDecryptInternal(false, Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Decrypt(string searchPattern, SearchOption searchOption)
      {
         return Directory.EncryptDecryptInternal(false, Transaction, FullPath, searchPattern, searchOption, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Decrypt(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return Directory.EncryptDecryptInternal(false, Transaction, FullPath, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Decrypt

      #region Delete

      /// <summary>Deletes this <see cref="DirectoryInfo"/> if it is empty.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public override bool Delete()
      {
         bool deleteOk = Directory.DeleteDirectoryInternal(Transaction, FullPath, false, false, Path.WildcardStarMatchAll);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      /// <summary>Deletes this instance of a <see cref="DirectoryInfo"/>, specifying whether to delete files and subdirectories.</summary>
      /// <param name="recursive"><c>true</c> to delete this directory, its subdirectories, and all files; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>
      /// If the <see cref="DirectoryInfo"/> has no files or subdirectories, this method deletes the <see cref="DirectoryInfo"/> even if recursive is false.
      /// Attempting to delete a <see cref="DirectoryInfo"/> that is not empty when recursive is false throws an <see cref="IOException"/>.
      /// </remarks>
      [SecurityCritical]
      public bool Delete(bool recursive)
      {
         bool deleteOk = Directory.DeleteDirectoryInternal(Transaction, FullPath, recursive, false, Path.WildcardStarMatchAll);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      #region AlphaFS

      /// <summary>Deletes this instance of a <see cref="DirectoryInfo"/>, specifying whether to delete files and subdirectories.</summary>
      /// <param name="recursive"><c>true</c> to delete this directory, its subdirectories, and all files; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> ignores read only attribute of files and directories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>
      /// If the <see cref="DirectoryInfo"/> has no files or subdirectories, this method deletes the <see cref="DirectoryInfo"/> even if recursive is false.
      /// Attempting to delete a <see cref="DirectoryInfo"/> that is not empty when recursive is false throws an <see cref="IOException"/>.
      /// </remarks>
      public bool Delete(bool recursive, bool ignoreReadOnly)
      {
         bool deleteOk = Directory.DeleteDirectoryInternal(Transaction, FullPath, recursive, ignoreReadOnly, Path.WildcardStarMatchAll);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      /// <summary>Deletes this instance of a <see cref="DirectoryInfo"/>, specifying whether to delete files and subdirectories.</summary>
      /// <param name="recursive"><c>true</c> to delete this directory, its subdirectories, and all files; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> ignores read only attribute of files and directories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>
      /// If the <see cref="DirectoryInfo"/> has no files or subdirectories, this method deletes the <see cref="DirectoryInfo"/> even if recursive is false.
      /// Attempting to delete a <see cref="DirectoryInfo"/> that is not empty when recursive is false throws an <see cref="IOException"/>.
      /// </remarks>
      [SecurityCritical]
      public bool Delete(bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         bool deleteOk = Directory.DeleteDirectoryInternal(Transaction, FullPath, recursive, ignoreReadOnly, searchPattern);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      #endregion // AlphaFS

      #endregion // Delete

      #region DeleteEmpty

      /// <summary>Deletes empty subdirectores from the this <see cref="DirectoryInfo"/> instance.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool DeleteEmpty()
      {
         bool deleteOk = Directory.DeleteEmptyDirectoryInternal(Transaction, FullPath, false, false, Path.WildcardStarMatchAll);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      /// <summary>Deletes empty subdirectores from the this <see cref="DirectoryInfo"/> instance.</summary>
      /// <param name="recursive"><c>true</c> to delete this directory, its subdirectories, and all files; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool DeleteEmpty(bool recursive)
      {
         bool deleteOk = Directory.DeleteEmptyDirectoryInternal(Transaction, FullPath, recursive, false, Path.WildcardStarMatchAll);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      //// <summary>Deletes empty subdirectores from the this <see cref="DirectoryInfo"/> instance.</summary>
      /// <param name="recursive"><c>true</c> to delete this directory, its subdirectories, and all files; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> ignores read only attribute of files and directories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      public bool DeleteEmpty(bool recursive, bool ignoreReadOnly)
      {
         bool deleteOk = Directory.DeleteEmptyDirectoryInternal(Transaction, FullPath, recursive, ignoreReadOnly, Path.WildcardStarMatchAll);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      /// <summary>Deletes empty subdirectores from the this <see cref="DirectoryInfo"/> instance.</summary>
      /// <param name="recursive"><c>true</c> to delete this directory, its subdirectories, and all files; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> ignores read only attribute of files and directories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool DeleteEmpty(bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         bool deleteOk = Directory.DeleteEmptyDirectoryInternal(Transaction, FullPath, recursive, ignoreReadOnly, searchPattern);
         if (deleteOk)
            Reset();
         return deleteOk;
      }

      #endregion // DeleteEmpty

      #region Encrypt

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only encrypt the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Encrypt()
      {
         return Directory.EncryptDecryptInternal(true, Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only encrypt the root items, non recursive.</remarks>
      [SecurityCritical]
      public bool Encrypt(string searchPattern)
      {
         return Directory.EncryptDecryptInternal(true, Transaction, FullPath, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Encrypt(string searchPattern, SearchOption searchOption)
      {
         return Directory.EncryptDecryptInternal(true, Transaction, FullPath, searchPattern, searchOption, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public bool Encrypt(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return Directory.EncryptDecryptInternal(true, Transaction, FullPath, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Encrypt

      #region EncryptionDisable

      /// <summary>Disables encryption of the specified directory and the files in it. It does not affect encryption of subdirectories below the indicated directory.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method will create/change the file "Desktop.ini" and wil set Encryption value: "Disable=0"</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool EncryptionDisable()
      {
         return Directory.EncryptionDisable(FullPath);
      }

      #endregion // EncryptionDisable

      #region EncryptionEnable

      /// <summary>Enables encryption of the specified directory and the files in it. It does not affect encryption of subdirectories below the indicated directory.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method will create/change the file "Desktop.ini" and wil set Encryption value: "Disable=1"</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool EncryptionEnable()
      {
         return Directory.EncryptionEnable(FullPath);
      }

      #endregion // EncryptionEnable

      #region EnumerateStreams

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <returns>An enumerable <see langref="BackupStreamInfo"/> collection of streams for the directory or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public IEnumerable<BackupStreamInfo> EnumerateStreams()
      {
         return Directory.EnumerateStreamsInternal(Transaction, FullPath, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable <see langref="BackupStreamInfo"/> collection of streams for the directory or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public IEnumerable<BackupStreamInfo> EnumerateStreams(string searchPattern, SearchOption searchOption)
      {
         return Directory.EnumerateStreamsInternal(Transaction, FullPath, searchPattern, searchOption, false);
      }

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or not accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable <see langref="BackupStreamInfo"/> collection of streams for the directory or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public IEnumerable<BackupStreamInfo> EnumerateStreams(string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return Directory.EnumerateStreamsInternal(Transaction, FullPath, searchPattern, searchOption, continueOnAccessError);
      }
      
      #endregion // EnumerateStreams

      #region MoveTo

      /// <summary>Moves a DirectoryInfo instance and its contents to a new path.</summary>
      /// <param name="destinationPath">The path to the new location for sourcePath.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool MoveTo(string destinationPath)
      {
         return CopyToMoveToInternal(destinationPath, false, null, MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a DirectoryInfo instance and its contents to a new path.</summary>
      /// <param name="destinationPath">The path to the new location for sourcePath.</param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Move will fail on existing folders or files.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool MoveTo(string destinationPath, bool overwrite)
      {
         return CopyToMoveToInternal(destinationPath, false, null, overwrite ? NativeMethods.MoveOptsReplace : MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a DirectoryInfo instance and its contents to a new path.</summary>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Move will fail on existing folders or files.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves ACLs information.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool MoveTo(string destinationPath, bool overwrite, bool preserveSecurity)
      {
         return CopyToMoveToInternal(destinationPath, preserveSecurity, null, overwrite ? NativeMethods.MoveOptsReplace : MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a DirectoryInfo instance and its contents to a new path.</summary>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="moveOptions">Flags that specify how the file is to be move. This parameter can be <see langword="null"/>.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves ACLs information.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public bool MoveTo(string destinationPath, MoveOptions moveOptions, bool preserveSecurity, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyToMoveToInternal(destinationPath, preserveSecurity, null, moveOptions, copyProgress, userProgressData);
      }

      #endregion // MoveTo

      #region SetAccessControl

      /// <summary>Applies access control list (ACL) entries described by a <see cref="DirectorySecurity"/> object to the directory described by the current DirectoryInfo object.</summary>
      /// <param name="directorySecurity">A <see cref="DirectorySecurity"/> object that describes an ACL entry to apply to the directory described by the path parameter.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public bool SetAccessControl(DirectorySecurity directorySecurity)
      {
         // In this case, equals null, is a good thing.
         return SetAccessControl(directorySecurity, AccessControlSections.All);
      }

      /// <summary>Applies access control list (ACL) entries described by a <see cref="DirectorySecurity"/> object to the directory described by the current DirectoryInfo object.</summary>
      /// <param name="directorySecurity">A <see cref="DirectorySecurity"/> object that describes an ACL entry to apply to the directory described by the path parameter.</param>
      /// <param name="includeSections">One or more of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to set.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public bool SetAccessControl(DirectorySecurity directorySecurity, AccessControlSections includeSections)
      {
         // In this case, equals null, is a good thing.
         return GetSetAccessControlInternal(true, true, FullPath, directorySecurity, includeSections) == null;
      }

      #endregion // SetAccessControl

      
      #region Unified Internals

      #region CopyToMoveToInternal

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="preserveSecurity"><c>true</c> Preserves ACLs information.</param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the file is to be copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="moveOptions"><see cref="MoveOptions"/> that specify how the file is to be moved. This parameter can be <see langword="null"/>.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <remarks>This Move method works across disk volumes, and it does not throw an exception if the source and destination are
      /// the same. Note that if you attempt to replace a file by moving a file of the same name into that directory, you
      /// get an IOException. You cannot use the Move method to overwrite an existing file.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      private bool CopyToMoveToInternal(string destinationPath, bool preserveSecurity, CopyOptions? copyOptions, MoveOptions? moveOptions, CopyProgressRoutine copyProgress, object userProgressData)
      {
         bool copyMoveOk = Directory.CopyMoveInternal(true, Transaction, FullPath, destinationPath, preserveSecurity, copyOptions, moveOptions, copyProgress, userProgressData);

         if (copyMoveOk && moveOptions != null && copyOptions == null)
            Initialize(Transaction, true, destinationPath);

         return copyMoveOk;
      }

      #endregion // CopyToMoveToInternal

      #endregion // Unified Internals

      #endregion // AlphaFS

      #endregion // Methods

      #region Properties

      #region .NET

      #region Exists

      /// <summary>Gets a value indicating whether the directory exists.</summary>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      public override bool Exists
      {
         get { return SystemInfo != null; }
      }

      #endregion // Exists

      //#region FullName

      ///// <summary>Gets the full path of the directory.</summary>
      //public override string FullName
      //{
      //   get { return FullPath; }
      //}

      //#endregion // FullName

      #region Name

      /// <summary>Gets the name of this DirectoryInfo instance.</summary>
      /// <remarks>Returns only the name of the directory, such as "Bin". To get the full path, such as "c:\public\Bin", use the FullName property.</remarks>
      public override string Name
      {
         get
         {
            return MPathInfo.Root.Equals(FullPath, StringComparison.OrdinalIgnoreCase)
                      ? FullPath
                      : MPathInfo.FileName;
         }
      }

      #endregion // Name

      #region Parent

      /// <summary>Gets the parent directory of a specified subdirectory.</summary>
      /// <returns>The parent directory, or null if the path is null or if the file path denotes a root (such as "\", "C:", or * "\\server\share").</returns>
      public DirectoryInfo Parent
      {
         get { return Directory.GetParent(Transaction, FullPath); }
      }

      #endregion // Parent

      #region Root

      /// <summary>Gets the root portion of the directory.</summary>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the root of the directory.</returns>
      public DirectoryInfo Root
      {
         get
         {
            //return new DirectoryInfo(Transaction, new PathInfo(new PathInfo(FullPath, false).GetFullPath(), false).Root);
            return new DirectoryInfo(Transaction, new PathInfo(FullPath, false).Root);
         }
      }

      #endregion // Root

      #endregion // .NET

      #endregion // Properties
   }
}