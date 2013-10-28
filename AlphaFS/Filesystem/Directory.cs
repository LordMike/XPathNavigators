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
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using SearchOption = System.IO.SearchOption;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Exposes static methods for creating, moving, and enumerating through directories and subdirectories. This class cannot be inherited.</summary>
   public static class Directory
   {
      #region .NET

      #region CreateDirectory

      #region .NET

      /// <summary>Creates all the directories in a specified path.
      /// If the underlying file system supports security on files and directories,
      /// the function applies a default security descriptor to the new directory.
      /// </summary>
      /// <param name="path">The directory path to create.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(string path)
      {
         return CreateDirectoryInternal(null, null, path, null, null) ? new DirectoryInfo(null, path) : null;
      }

      /// <summary>Creates all the directories in the specified path, applying the specified Windows security.</summary>
      /// <param name="path">The directory path to create.</param>
      /// <param name="directorySecurity">The <see cref="DirectorySecurity"/> access control to apply to the directory.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
      {
         return CreateDirectoryInternal(null, null, path, directorySecurity, null) ? new DirectoryInfo(null, path) : null;
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Creates a new directory with the attributes of a specified template directory. 
      /// If the underlying file system supports security on files and directories, the function 
      /// applies a default security descriptor to the new directory. The new directory retains 
      /// the other attributes of the specified template directory.
      /// </summary>
      /// <param name="templatePath">The path of the directory to use as a template when creating the new directory.</param>
      /// <param name="path">The directory path to create.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(string templatePath, string path)
      {
         return CreateDirectoryInternal(null, templatePath, path, null, null) ? new DirectoryInfo(null, path) : null;
      }

      /// <summary>Creates a new directory with the attributes of a specified template directory. 
      /// If the underlying file system supports security on files and directories, the function 
      /// applies the specified security descriptor to the new directory. The new directory retains 
      /// the other attributes of the specified template directory.
      /// </summary>
      /// <param name="templatePath">The path of the directory to use as a template when creating the new directory.</param>
      /// <param name="path">The directory path to create.</param>
      /// <param name="directorySecurity">The <see cref="DirectorySecurity"/> access control to apply to the directory.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(string templatePath, string path, ObjectSecurity directorySecurity)
      {
         return CreateDirectoryInternal(null, templatePath, path, directorySecurity, null) ? new DirectoryInfo(null, path) : null;
      }

      #region Transacted

      /// <summary>Creates all the directories in a specified path as a transacted operation. 
      /// If the underlying file system supports security on files and directories,
      /// the function applies a default security descriptor to the new directory. 
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path to create.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(KernelTransaction transaction, string path)
      {
         return CreateDirectoryInternal(transaction, null, path, null, null) ? new DirectoryInfo(transaction, path) : null;
      }

      /// <summary>Creates all the directories in a specified path as a transacted operation, with the attributes of a specified template directory. 
      /// If the underlying file system supports security on files and directories, the function applies a default security descriptor to the new directory. 
      /// The new directory retains the other attributes of the specified template directory.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path to create.</param>
      /// <param name="directorySecurity">The <see cref="DirectorySecurity"/> access control to apply to the directory.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(KernelTransaction transaction, string path, ObjectSecurity directorySecurity)
      {
         return CreateDirectoryInternal(transaction, null, path, directorySecurity, null) ? new DirectoryInfo(transaction, path) : null;
      }

      /// <summary>Creates a new directory as a transacted operation, with the attributes of a specified template directory. 
      /// If the underlying file system supports security on files and directories, the function applies a default security descriptor to the new directory. 
      /// The new directory retains the other attributes of the specified template directory.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="templatePath">
      /// <para>The path of the directory to use as a template when creating the new directory. This parameter can be <see langword="null"/>. </para>
      /// <para>The directory must reside on the local computer; otherwise, the an exception of type <see cref="UnsupportedRemoteTransactionException"/> is thrown.</para>
      /// </param>
      /// <param name="path">The directory path to create.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(KernelTransaction transaction, string templatePath, string path)
      {
         return CreateDirectoryInternal(transaction, templatePath, path, null, null) ? new DirectoryInfo(transaction, path) : null;
      }

      /// <summary>Creates a new directory as a transacted operation, with the attributes of a specified template directory. 
      /// If the underlying file system supports security on files and directories, the function applies a default security descriptor to the new directory. 
      /// The new directory retains the other attributes of the specified template directory.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="templatePath">
      /// <para>The path of the directory to use as a template when creating the new directory. This parameter can be <see langword="null"/>. </para>
      /// <para>The directory must reside on the local computer; otherwise, the an exception of type <see cref="UnsupportedRemoteTransactionException"/> is thrown.</para>
      /// </param>
      /// <param name="path">The directory path to create.</param>
      /// <param name="directorySecurity">The <see cref="DirectorySecurity"/> access control to apply to the directory.</param>
      /// <returns>A <see cref="DirectoryInfo"/> object that represents the directory for the specified path, or <see langword="null"/> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DirectoryInfo CreateDirectory(KernelTransaction transaction, string templatePath, string path, ObjectSecurity directorySecurity)
      {
         return CreateDirectoryInternal(transaction, templatePath, path, directorySecurity, null) ? new DirectoryInfo(transaction, path) : null;
      }

      #endregion // Transacted

      #endregion AlphaFS

      #endregion // CreateDirectory

      #region Delete

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Delete

      #region EnumerateDirectories

      #region .NET

      /// <summary>Returns an enumerable collection of directory names in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path
      /// and that match the specified search pattern.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, true, true, false).Cast<string>();
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, true, true, continueOnAccessError).Cast<string>();
      }

      #region Transacted

      /// <summary>Returns an enumerable collection of directory names in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path
      /// and that match the specified search pattern.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, true, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDirectories(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, true, true, continueOnAccessError).Cast<string>();
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // EnumerateDirectories

      #region EnumerateFiles

      #region .NET

      /// <summary>Returns an enumerable collection of file names in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path
      /// and that match the specified search pattern.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, false, true, false).Cast<string>();
      }

      #endregion .NET

      #region AlphaFS

      /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, false, true, continueOnAccessError).Cast<string>();
      }

      #region Transacted

      /// <summary>Returns an enumerable collection of file names in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path
      /// and that match the specified search pattern.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, false, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>
      /// An enumerable <see langref="string"/> collection, of the full names (including paths) for the files in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFiles(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, false, true, continueOnAccessError).Cast<string>();
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // EnumerateFiles

      #region EnumerateFileSystemEntries

      #region .NET

      /// <summary>Returns an enumerable collection of file-system entries in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable <see langref="string"/> collection of file-system entries in the directory specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file-system entries that match a search pattern in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An enumerable <see langref="string"/> collection of file-system entries in the directory specified by path and that match the specified search pattern.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable <see langref="string"/> collection of file-system entries in the directory specified by path and that match the specified search pattern and option.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, null, true, false).Cast<string>();
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Returns an enumerable collection of file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path and that match the specified search pattern and option.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, null, true, continueOnAccessError).Cast<string>();
      }

      #region Transacted

      /// <summary>Returns an enumerable collection of file-system entries in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file-system entries that match a search pattern in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path and that match the specified search pattern.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path and that match the specified search pattern and option.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, null, true, false).Cast<string>();
      }

      /// <summary>Returns an enumerable collection of file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable <see langref="string"/> collection, of the full names (including paths) for the directories in the directory specified by path and that match the specified search pattern and option.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateFileSystemEntries(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, null, true, continueOnAccessError).Cast<string>();
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // EnumerateFileSystemEntries

      #region Exists

      #region .NET

      /// <summary>Determines whether the given path refers to an existing directory on disk.</summary>
      /// <param name="path">The path to test.</param>
      /// <returns><c>true</c> if path refers to an existing directory, <c>false</c> otherwise.</returns>
      /// <remarks>Possible performance improvement may be achieved by utilizing <see cref="NativeMethods.FindExSearchOps.SearchLimitToDirectories"/>.</remarks>
      [SecurityCritical]
      public static bool Exists(string path)
      {
         return FileSystemInfo.ExistsInternal(true, null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Determines whether the given path refers to an existing directory on disk.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to test.</param>
      /// <returns><c>true</c> if path refers to an existing directory, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Exists(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.ExistsInternal(true, transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // Exists

      #region GetAccessControl

      #region .NET

      /// <summary>Gets a <see cref="DirectorySecurity"/> object that encapsulates the access control list (ACL) entries for the specified directory.</summary>
      /// <param name="path">The path to a directory containing a <see cref="DirectorySecurity"/> object that describes the file's access control list (ACL) information.</param>
      /// <returns>A <see cref="DirectorySecurity"/> object that encapsulates the access control rules for the file described by the <paramref name="path"/> parameter.</returns>
      [SecurityCritical]
      public static DirectorySecurity GetAccessControl(string path)
      {
         return (DirectorySecurity)FileSystemInfo.GetSetAccessControlInternal(true, false, path, null, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner);
      }

      /// <summary>Gets a <see cref="DirectorySecurity"/> object that encapsulates the specified type of access control list (ACL) entries for a particular directory.</summary>
      /// <param name="path">The path to a directory containing a <see cref="DirectorySecurity"/> object that describes the directory's access control list (ACL) information.</param>
      /// <param name="includeSections">One (or more) of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to receive.</param>
      /// <returns>A <see cref="DirectorySecurity"/> object that encapsulates the access control rules for the directory described by the <paramref name="path"/> parameter. </returns>
      [SecurityCritical]
      public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
      {
         return (DirectorySecurity)FileSystemInfo.GetSetAccessControlInternal(true, false, path, null, includeSections);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      ///// <summary>Gets a <see cref="DirectorySecurity"/> object that encapsulates the access control list (ACL) entries for the specified directory.</summary>
      ///// <param name="path">The path to a directory containing a <see cref="DirectorySecurity"/> object that describes the file's access control list (ACL) information.</param>
      ///// <returns>A <see cref="DirectorySecurity"/> object that encapsulates the access control rules for the file described by the <paramref name="path"/> parameter.</returns>
      //[SecurityCritical]
      //public static DirectorySecurity GetAccessControl(string path)
      //{
      //   return (DirectorySecurity)File.GetSetAccessControlInternal(true, false, path, null, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner);
      //}

      ///// <summary>Gets a <see cref="DirectorySecurity"/> object that encapsulates the specified type of access control list (ACL) entries for a particular directory.</summary>
      ///// <param name="path">The path to a directory containing a <see cref="DirectorySecurity"/> object that describes the directory's access control list (ACL) information.</param>
      ///// <param name="includeSections">One (or more) of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to receive.</param>
      ///// <returns>A <see cref="DirectorySecurity"/> object that encapsulates the access control rules for the directory described by the <paramref name="path"/> parameter. </returns>
      //[SecurityCritical]
      //public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
      //{
      //   return (DirectorySecurity)File.GetSetAccessControlInternal(true, false, path, null, includeSections);
      //}

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetAccessControl

      #region GetCreationTime

      #region .NET

      /// <summary>Returns the creation date and time of the specified file or directory.</summary>
      /// <param name="path">The file or directory for which to obtain creation date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified file or directory. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTime(string path)
      {
         return FileSystemInfo.GetCreationTimeInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the creation date and time of the specified file or directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain creation date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified file or directory. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTime(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetCreationTimeInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetCreationTime

      #region GetCreationTimeUtc

      #region .NET

      /// <summary>Gets the creation date and time, in Coordinated Universal Time (UTC) format, of a directory.</summary>
      /// <param name="path">The path of the directory.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified directory. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTimeUtc(string path)
      {
         return FileSystemInfo.GetCreationTimeUtcInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Gets the creation date and time, in Coordinated Universal Time (UTC) format, of a directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path of the directory.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified directory. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTimeUtc(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetCreationTimeUtcInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetCreationTimeUtc

      #region GetCurrentDirectory

      #region .NET

      /// <summary>Gets the current working directory of the application.</summary>
      /// <returns>A string that contains the path of the current working directory, and does not end with a backslash (\).</returns>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      [SecurityCritical]
      public static string GetCurrentDirectory()
      {
         return System.IO.Directory.GetCurrentDirectory();
      }

      #endregion .NET

      #endregion // GetCurrentDirectory

      #region GetDirectories

      #region .NET

      /// <summary>Gets the names of subdirectories (including their paths) in the specified directory.</summary>
      /// <param name="path">The path for which an array of subdirectory names is returned.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of subdirectories in the specified path.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of subdirectories (including their paths) that match the specified search pattern in the current directory.</summary>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of the subdirectories that match the search pattern.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of subdirectories in the specified path.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, true, true, false).Cast<string>().ToArray();
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of subdirectories in the specified path.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, true, true, continueOnAccessError).Cast<string>().ToArray();
      }

      #region Transacted

      /// <summary>Gets the names of subdirectories (including their paths) in the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path for which an array of subdirectory names is returned.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of subdirectories in the specified path.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of subdirectories (including their paths) that match the specified search pattern in the current directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of the subdirectories that match the search pattern.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, true, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of subdirectories in the specified path.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, true, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) of subdirectories in the specified path.</returns>
      /// <remarks>
      /// The EnumerateDirectories and GetDirectories methods differ as follows: When you use EnumerateDirectories, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetDirectories, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateDirectories can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetDirectories(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, true, true, continueOnAccessError).Cast<string>().ToArray();
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetDirectories

      #region GetDirectoryRoot

      #region .NET

      /// <summary>Returns the volume information, root information, or both for the specified path.</summary>
      /// <param name="path">The path of a file or directory.</param>
      /// <returns>A string that contains the volume information, root information, or both for the specified path.</returns>
      [SecurityCritical]
      public static string GetDirectoryRoot(string path)
      {
         return new PathInfo(new PathInfo(path, false).GetFullPath(), false).Root;
      }

      #endregion // .NET

      #endregion

      #region GetFiles

      #region .NET

      /// <summary>Returns the names of files (including their paths) in the specified directory.</summary>
      /// <param name="path">The directory from which to retrieve the files.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>().ToArray();
      }

      /// <summary>Returns the names of files (including their paths) that match the specified search pattern in the specified directory.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory that match the specified search pattern.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory that match the specified search pattern and option.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, false, true, false).Cast<string>().ToArray();
      }

      #endregion // .Net

      #region AlphaFS

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory that match the specified search pattern and option.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, false, true, continueOnAccessError).Cast<string>().ToArray();
      }

      #region Transacted

      /// <summary>Returns the names of files (including their paths) in the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory from which to retrieve the files.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>().ToArray();
      }

      /// <summary>Returns the names of files (including their paths) that match the specified search pattern in the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory that match the specified search pattern.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, false, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory that match the specified search pattern and option.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, false, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets the names of the subdirectories (including their paths) that match the specified search pattern in the current directory, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="string"/>[] array of the full names (including paths) for the files in the specified directory that match the specified search pattern and option.</returns>
      /// <remarks>
      /// The EnumerateFiles and GetFiles methods differ as follows: When you use EnumerateFiles, you can start enumerating the collection of names
      /// before the whole collection is returned; when you use GetFiles, you must wait for the whole array of names to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFiles(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, false, true, continueOnAccessError).Cast<string>().ToArray();
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion

      #region GetFileSystemEntries

      #region .NET

      /// <summary>Returns the names of all files and subdirectories in the specified directory.</summary>
      /// <param name="path">The directory for which file and subdirectory names are returned.</param>
      /// <returns>An <see cref="string"/>[] array of the names of files and subdirectories in the specified directory.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>().ToArray();
      }

      /// <summary>Returns an array of file system entries that match the specified search criteria.</summary>
      /// <param name="path">The path to be searched.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An <see cref="string"/>[] array of file system entries that match the specified search criteria.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets an array of all the file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="string"/>[] array of file system entries that match the specified search criteria.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, null, true, false).Cast<string>().ToArray();
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Gets an array of all the file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="string"/>[] array of file system entries that match the specified search criteria.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, null, true, continueOnAccessError).Cast<string>().ToArray();
      }

      #region Transacted

      /// <summary>Returns the names of all files and subdirectories in the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory for which file and subdirectory names are returned.</param>
      /// <returns>An <see cref="string"/>[] array of the names of files and subdirectories in the specified directory.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>().ToArray();
      }

      /// <summary>Returns an array of file system entries that match the specified search criteria.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to be searched.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An <see cref="string"/>[] array of file system entries that match the specified search criteria.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, null, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets an array of all the file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="string"/>[] array of file system entries that match the specified search criteria.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, null, true, false).Cast<string>().ToArray();
      }

      /// <summary>Gets an array of all the file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="string"/>[] array of file system entries that match the specified search criteria.</returns>
      /// <remarks>
      /// The EnumerateFileSystemEntries and GetFileSystemEntries methods differ as follows: When you use EnumerateFileSystemEntries,
      /// you can start enumerating the collection of entries before the whole collection is returned; when you use GetFileSystemEntries,
      /// you must wait for the whole array of entries to be returned before you can access the array.
      /// Therefore, when you are working with many files and directories, EnumerateFiles can be more efficient.
      /// </remarks>
      [SecurityCritical]
      public static string[] GetFileSystemEntries(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, null, true, continueOnAccessError).Cast<string>().ToArray();
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetFileSystemEntries

      #region GetLastAccessTime

      #region .NET

      /// <summary>Returns the date and time the specified file or directory was last accessed.</summary>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTime(string path)
      {
         return FileSystemInfo.GetLastAccessTimeInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTime(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastAccessTimeInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastAccessTime

      #region GetLastAccessTimeUtc

      #region .NET

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtc(string path)
      {
         return FileSystemInfo.GetLastAccessTimeUtcInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtc(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastAccessTimeUtcInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastAccessTimeUtc

      #region GetLastWriteTime

      #region .NET

      /// <summary>Returns the date and time the specified file or directory was last written to.</summary>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTime(string path)
      {
         return FileSystemInfo.GetLastWriteTimeInternal(null, path);
      }

      #endregion //.NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time the specified file or directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTime(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastWriteTimeInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastWriteTime

      #region GetLastWriteTimeUtc

      #region .NET

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last written to.</summary>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtc(string path)
      {
         return FileSystemInfo.GetLastWriteTimeUtcInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtc(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastWriteTimeUtcInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastWriteTimeUtc

      #region GetLogicalDrives

      #region .NET

      /// <summary>Retrieves the names of the logical drives on this computer in the form "driveLetter:\"</summary>
      /// <returns>The logical drives on this computer as a <see cref="string"/>[] array.</returns>
      [SecurityCritical]
      public static string[] GetLogicalDrives()
      {
         return GetLogicalDrives(false, false, false).ToArray();
      }

      #endregion // .NET

      #endregion // GetLogicalDrives

      #region GetParent

      #region .NET

      /// <summary>Retrieves the parent directory of the specified path, including both absolute and relative paths.</summary>
      /// <param name="path">The path for which to retrieve the parent directory.</param>
      /// <returns>The parent directory of type <see cref="DirectoryInfo"/> or null if path is the root directory, including the root of a UNC server or share name.</returns>
      /// <remarks>Trailing spaces are removed from the end of the path parameter before getting the directory.</remarks>
      [SecurityCritical]
      public static DirectoryInfo GetParent(string path)
      {
         return GetParent(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Retrieves the parent directory of the specified path, including both absolute and relative paths.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path for which to retrieve the parent directory.</param>
      /// <returns>The parent directory of type <see cref="DirectoryInfo"/> or null if path is the root directory, including the root of a UNC server or share name.</returns>
      /// <remarks>Trailing spaces are removed from the end of the path parameter before getting the directory.</remarks>
      [SecurityCritical]
      public static DirectoryInfo GetParent(KernelTransaction transaction, string path)
      {
         if (path != null) path = path.TrimEnd(' ');
         PathInfo p = new PathInfo(path, false).Parent;
         //return string.IsNullOrEmpty(p.Path) ? null : new DirectoryInfo(transaction, p.Path);
         return new DirectoryInfo(transaction, !string.IsNullOrEmpty(p.Path) ? p.Path : path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetParent

      #region Move

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Move

      #region SetAccessControl

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // SetAccessControl

      #region SetCreationTime

      #region .NET

      /// <summary>Sets the creation date and time for the specified file or directory.</summary>
      /// <param name="path">The file or directory for which to set the creation date and time information.</param>
      /// <param name="creationTime">An object that contains the value to set for the creation date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTime(string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, null, path, creationTime.ToFileTime(), null, null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the creation date and time for the specified file or directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to set the creation date and time information.</param>
      /// <param name="creationTime">An object that contains the value to set for the creation date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTime(KernelTransaction transaction, string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, creationTime.ToFileTime(), null, null);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetCreationTime

      #region SetCreationTimeUtc

      #region .NET

      /// <summary>Sets the creation date and time, in Coordinated Universal Time (UTC) format, for the specified file or directory.</summary>
      /// <param name="path">The file or directory for which to set the creation date and time information.</param>
      /// <param name="creationTime">An object that contains the value to set for the creation date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTimeUtc(string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, null, path, creationTime.ToFileTimeUtc(), null, null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the creation date and time, in Coordinated Universal Time (UTC) format, for the specified file or directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to set the creation date and time information.</param>
      /// <param name="creationTime">An object that contains the value to set for the creation date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTimeUtc(KernelTransaction transaction, string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, creationTime.ToFileTimeUtc(), null, null);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetCreationTimeUtc

      #region SetCurrentDirectory

      #region .NET

      /// <summary>Sets the application's current working directory to the specified directory.</summary>
      /// <param name="path">The path to which the current working directory is set.</param>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static void SetCurrentDirectory(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         string regularPath = Path.GetRegularPath(path);

         try
         {
            // MaxComponentLength = 255
            System.IO.Directory.SetCurrentDirectory(path.Length > 255 ? Path.GetShort83Path(regularPath) : regularPath);
         }
         catch
         {
         }
      }

      #endregion // .NET

      #endregion // SetCurrentDirectory

      #region SetLastAccessTime

      #region .NET

      /// <summary>Sets the date and time the specified file or directory was last accessed.</summary>
      /// <param name="path">The file or directory for which to set the access date and time information.</param>
      /// <param name="creationTime">An object that contains the value to set for the access date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTime(string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, null, path, null, creationTime.ToFileTime(), null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to set the access date and time information.</param>
      /// <param name="creationTime">An object that contains the value to set for the access date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTime(KernelTransaction transaction, string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, null, creationTime.ToFileTime(), null);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetLastAccessTime

      #region SetLastAccessTimeUtc

      #region .NET

      /// <summary>Sets the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
      /// <param name="path">The file or directory for which to set the access date and time information.</param>
      /// <param name="lastAccessTime">An object that contains the value to set for the access date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, null, path, null, lastAccessTime.ToFileTimeUtc(), null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to set the access date and time information.</param>
      /// <param name="lastAccessTime">An object that contains the value to set for the access date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTimeUtc(KernelTransaction transaction, string path, DateTime lastAccessTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, null, lastAccessTime.ToFileTimeUtc(), null);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetLastAccessTimeUtc

      #region SetLastWriteTime

      #region .NET

      /// <summary>Sets the date and time a directory was last written to.</summary>
      /// <param name="path">The path of the directory.</param>
      /// <param name="creationTime">The date and time the directory was last written to. This value is expressed in local time.</param>
      [SecurityCritical]
      public static void SetLastWriteTime(string path, DateTime creationTime)
      {
         File.SetLastWriteTime(null, path, creationTime);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time a directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path of the directory.</param>
      /// <param name="creationTime">The date and time the directory was last written to. This value is expressed in local time.</param>
      [SecurityCritical]
      public static void SetLastWriteTime(KernelTransaction transaction, string path, DateTime creationTime)
      {
         File.SetLastWriteTime(transaction, path, creationTime);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetLastWriteTime

      #region SetLastWriteTimeUtc

      #region .NET

      /// <summary>Sets the date and time, in Coordinated Universal Time (UTC) format, that a directory was last written to.</summary>
      /// <param name="path">The path of the directory.</param>
      /// <param name="creationTime">The date and time the directory was last written to. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastWriteTimeUtc(string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, null, path, null, null, creationTime.ToFileTimeUtc());
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time, in Coordinated Universal Time (UTC) format, that a directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path of the directory.</param>
      /// <param name="creationTime">The date and time the directory was last written to. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastWriteTimeUtc(KernelTransaction transaction, string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, null, null, creationTime.ToFileTimeUtc());
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetLastWriteTimeUtc

      #endregion .NET

      #region AlphaFS

      #region Compress

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only compress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Compress(string path)
      {
         return CompressDecompressInternal(true, null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only compress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Compress(string path, string searchPattern)
      {
         return CompressDecompressInternal(true, null, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Compress(string path, string searchPattern, SearchOption searchOption)
      {
         return CompressDecompressInternal(true, null, path, searchPattern, searchOption, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Compress(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return CompressDecompressInternal(true, null, path, searchPattern, searchOption, continueOnAccessError);
      }

      #region Transacted

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only compress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Compress(KernelTransaction transaction, string path)
      {
         return CompressDecompressInternal(true, transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only compress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Compress(KernelTransaction transaction, string path, string searchPattern)
      {
         return CompressDecompressInternal(true, transaction, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Compress(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return CompressDecompressInternal(true, transaction, path, searchPattern, searchOption, false);
      }

      /// <summary>Compresses a directory using NTFS compression.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Compress(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return CompressDecompressInternal(true, transaction, path, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Transacted

      #endregion // Compress

      #region CompressionDisable

      /// <summary>Disables compression of the specified directory and the files in it.</summary>
      /// <param name="path">A path to a folder to decompress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method disables the folder-compression attribute. It will not decompress the current contents of the folder.
      /// However, newly created files and folders will be uncompressed.</remarks>
      [SecurityCritical]
      public static bool CompressionDisable(string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(true, null, path, false);
      }

      #region Transacted

      /// <summary>Disables compression of the specified directory and the files in it.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path to a folder to decompress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method disables the folder-compression attribute. It will not decompress the current contents of the folder.
      /// However, newly created files and folders will be uncompressed.</remarks>
      [SecurityCritical]
      public static bool CompressionDisable(KernelTransaction transaction, string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(true, transaction, path, false);
      }

      #endregion // Transacted

      #endregion // CompressionDisable

      #region CompressionEnable

      /// <summary>Enables compression of the specified directory and the files in it.</summary>
      /// <param name="path">A path to a folder to compress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method enables the folder-compression attribute. It will not compress the current contents of the folder.
      /// However, newly created files and folders will be compressed.</remarks>
      [SecurityCritical]
      public static bool CompressionEnable(string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(true, null, path, true);
      }

      #region Transacted

      /// <summary>Enables compression of the specified directory and the files in it.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path to a folder to compress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method enables the folder-compression attribute. It will not compress the current contents of the folder.
      /// However, newly created files and folders will be compressed.</remarks>
      [SecurityCritical]
      public static bool CompressionEnable(KernelTransaction transaction, string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(true, transaction, path, true);
      }

      #endregion // Transacted

      #endregion //CompressionEnable

      #region Copy

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, false, NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Copy will fail on existing folders or files.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath, bool overwrite)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, false, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Copy will fail on existing folders or files.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath, bool overwrite, bool preserveSecurity)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, preserveSecurity, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, null, null);
      }
      
      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the directory is to be copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath, CopyOptions copyOptions, bool preserveSecurity, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, preserveSecurity, copyOptions, null, copyProgress, userProgressData);
      }

      #region Transacted

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, false, NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Copy will fail on existing folders or files.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath, bool overwrite)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, false, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Copy will fail on existing folders or files.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath, bool overwrite, bool preserveSecurity)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, preserveSecurity, overwrite ? NativeMethods.CopyOptsNone : NativeMethods.CopyOptsFail, null, null, null);
      }

      /// <summary>Recursive copying of folders and files from one root to another.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the directory is to be copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath, CopyOptions copyOptions, bool preserveSecurity, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, preserveSecurity, copyOptions, null, copyProgress, userProgressData);
      }

      #endregion // Transacted

      #endregion // Copy

      #region CountDirectories

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountDirectories(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountDirectories(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountDirectories(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>The counted number of directories.</returns>
      [SecurityCritical]
      public static long CountDirectories(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, true, true, continueOnAccessError).Count();
      }

      #region Transacted

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountDirectories(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountDirectories(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>The counted number of directories.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountDirectories(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, true, true, false).Count();
      }

      /// <summary>Counts directories in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>The counted number of directories.</returns>
      [SecurityCritical]
      public static long CountDirectories(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, true, true, continueOnAccessError).Count();
      }

      #endregion // Transacted

      #endregion // CountDirectories

      #region CountFiles

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountFiles(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountFiles(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountFiles(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>The counted number of files.</returns>
      [SecurityCritical]
      public static long CountFiles(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, true, false, true, continueOnAccessError).Count();
      }

      #region Transacted

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountFiles(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountFiles(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>The counted number of files.</returns>
      /// <exception cref="System.UnauthorizedAccessException">An exception is thrown case of access errors.</exception>
      [SecurityCritical]
      public static long CountFiles(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, false, true, false).Count();
      }

      /// <summary>Counts files in a given directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory path.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>The counted number of files.</returns>
      [SecurityCritical]
      public static long CountFiles(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, false, true, continueOnAccessError).Count();
      }

      #endregion // Transacted

      #endregion // CountFiles

      #region Decompress

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decompress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Decompress(string path)
      {
         return CompressDecompressInternal(false, null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decompress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Decompress(string path, string searchPattern)
      {
         return CompressDecompressInternal(false, null, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Decompress(string path, string searchPattern, SearchOption searchOption)
      {
         return CompressDecompressInternal(false, null, path, searchPattern, searchOption, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Decompress(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return CompressDecompressInternal(false, null, path, searchPattern, searchOption, continueOnAccessError);
      }

      #region Transacted

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decompress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Decompress(KernelTransaction transaction, string path)
      {
         return CompressDecompressInternal(false, transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decompress the root items, non recursive.</remarks>
      [SecurityCritical]
      public static bool Decompress(KernelTransaction transaction, string path, string searchPattern)
      {
         return CompressDecompressInternal(false, transaction, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Decompress(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return CompressDecompressInternal(false, transaction, path, searchPattern, searchOption, false);
      }

      /// <summary>Decompresses an NTFS compressed directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decompress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Decompress(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return CompressDecompressInternal(false, transaction, path, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Transacted

      #endregion // Decompress

      #region Decrypt

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(string path)
      {
         return EncryptDecryptInternal(false, null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(string path, string searchPattern)
      {
         return EncryptDecryptInternal(false, null, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(string path, string searchPattern, SearchOption searchOption)
      {
         return EncryptDecryptInternal(false, null, path, searchPattern, searchOption, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EncryptDecryptInternal(false, null, path, searchPattern, searchOption, continueOnAccessError);
      }

      #region Transacted

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(KernelTransaction transaction, string path)
      {
         return EncryptDecryptInternal(false, transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(KernelTransaction transaction, string path, string searchPattern)
      {
         return EncryptDecryptInternal(false, transaction, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return EncryptDecryptInternal(false, transaction, path, searchPattern, searchOption, false);
      }

      /// <summary>Decrypts a directory that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to decrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only decrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EncryptDecryptInternal(false, transaction, path, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Transacted

      #endregion // Decrypt

      #region Delete

      #region .NET

      /// <summary>Deletes an empty directory from a specified path.</summary>
      /// <param name="path">The name of the empty directory to remove. This directory must be writable or empty.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(string path)
      {
         return DeleteDirectoryInternal(null, path, false, false, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes the specified directory and, if indicated, any subdirectories in the directory.</summary>
      /// <param name="path">The name of the directory to remove.</param>
      /// <param name="recursive"><c>true</c> to remove directories, subdirectories, and files in path; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(string path, bool recursive)
      {
         return DeleteDirectoryInternal(null, path, recursive, false, Path.WildcardStarMatchAll);
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Deletes the specified directory and, if indicated, any subdirectories in the directory.</summary>
      /// <param name="path">The name of the directory to remove.</param>
      /// <param name="recursive"><c>true</c> to remove directories, subdirectories, and files in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of files and directories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(string path, bool recursive, bool ignoreReadOnly)
      {
         return DeleteDirectoryInternal(null, path, recursive, ignoreReadOnly, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes the specified directory and, if indicated, any subdirectories in the directory.</summary>
      /// <param name="path">The name of the directory to remove.</param>
      /// <param name="recursive"><c>true</c> to remove directories, subdirectories, and files in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of files and directories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(string path, bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         return DeleteDirectoryInternal(null, path, recursive, ignoreReadOnly, searchPattern);
      }

      #region Transacted

      /// <summary>Deletes an empty directory from a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the empty directory to remove. This directory must be writable or empty.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(KernelTransaction transaction, string path)
      {
         return DeleteDirectoryInternal(transaction, path, false, false, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes the specified directory and, if indicated, any subdirectories in the directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove.</param>
      /// <param name="recursive"><c>true</c> to remove directories, subdirectories, and files in path; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(KernelTransaction transaction, string path, bool recursive)
      {
         return DeleteDirectoryInternal(transaction, path, recursive, false, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes the specified directory and, if indicated, any subdirectories in the directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove.</param>
      /// <param name="recursive"><c>true</c> to remove directories, subdirectories, and files in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of files and directories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(KernelTransaction transaction, string path, bool recursive, bool ignoreReadOnly)
      {
         return DeleteDirectoryInternal(transaction, path, recursive, ignoreReadOnly, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes the specified directory and, if indicated, any subdirectories in the directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove.</param>
      /// <param name="recursive"><c>true</c> to remove directories, subdirectories, and files in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of files and directories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Delete(KernelTransaction transaction, string path, bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         return DeleteDirectoryInternal(transaction, path, recursive, ignoreReadOnly, searchPattern);
      }

      #endregion // Transacted

      #endregion AlphaFS

      #endregion // Delete

      #region DeleteEmpty

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(string path)
      {
         return DeleteEmptyDirectoryInternal(null, path, false, false, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <param name="recursive"><c>true</c> to remove empty subdirectories in path; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(string path, bool recursive)
      {
         return DeleteEmptyDirectoryInternal(null, path, recursive, false, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <param name="recursive"><c>true</c> to remove empty subdirectories in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of empty subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(string path, bool recursive, bool ignoreReadOnly)
      {
         return DeleteEmptyDirectoryInternal(null, path, recursive, ignoreReadOnly, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <param name="recursive"><c>true</c> to remove empty subdirectories in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of empty subdirectories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(string path, bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         return DeleteEmptyDirectoryInternal(null, path, recursive, ignoreReadOnly, searchPattern);
      }

      #region Transacted

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(KernelTransaction transaction, string path)
      {
         return DeleteEmptyDirectoryInternal(transaction, path, false, false, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <param name="recursive"><c>true</c> to remove empty subdirectories in path; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(KernelTransaction transaction, string path, bool recursive)
      {
         return DeleteEmptyDirectoryInternal(transaction, path, recursive, false, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <param name="recursive"><c>true</c> to remove empty subdirectories in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of empty subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(KernelTransaction transaction, string path, bool recursive, bool ignoreReadOnly)
      {
         return DeleteEmptyDirectoryInternal(transaction, path, recursive, ignoreReadOnly, Path.WildcardStarMatchAll);
      }

      /// <summary>Deletes empty subdirectores from the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <param name="recursive"><c>true</c> to remove empty subdirectories in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of empty subdirectories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool DeleteEmpty(KernelTransaction transaction, string path, bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         return DeleteEmptyDirectoryInternal(transaction, path, recursive, ignoreReadOnly, searchPattern);
      }
      
      #endregion // Transacted

      #endregion // DeleteEmpty

      #region EnumerateFileSystemInfos

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> entries in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> entries in the directory specified by path.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false, null, false, false).Cast<FileSystemInfo>();
      }

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> entries that match a search pattern in a specified path.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> entries in the directory specified by path and that match the specified search pattern.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, SearchOption.TopDirectoryOnly, false, null, false, false).Cast<FileSystemInfo>();
      }

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> entries that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> entries in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, false, null, false, false).Cast<FileSystemInfo>();
      }

      #region AlphaFS

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> information objects that matches searchPattern and searchOption.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(null, path, searchPattern, searchOption, false, null, false, continueOnAccessError).Cast<FileSystemInfo>();
      }

      #region Transacted

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> entries in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> entries in the directory specified by path.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false, null, false, false).Cast<FileSystemInfo>();
      }

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> entries that match a search pattern in a specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> entries in the directory specified by path and that match the specified search pattern.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(KernelTransaction transaction, string path, string searchPattern)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, SearchOption.TopDirectoryOnly, false, null, false, false).Cast<FileSystemInfo>();
      }

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> entries that match a search pattern in a specified path, and optionally searches subdirectories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> entries in the directory specified by path
      /// and that match the specified search pattern and option.
      /// </returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, false, null, false, false).Cast<FileSystemInfo>();
      }

      /// <summary>Returns an enumerable collection of <see cref="FileSystemInfo"/> information that matches a specified search pattern and search subdirectory option.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The directory to search.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An enumerable collection of <see cref="FileSystemInfo"/> information objects that matches searchPattern and searchOption.</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos", Justification = "Microsoft chose this name, so we use it too.")]
      [SecurityCritical]
      public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, false, null, false, continueOnAccessError).Cast<FileSystemInfo>();
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // EnumerateFileSystemInfos

      #region EnumerateStreams

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="path">A path that describes a directory.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the directory specified by path, or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(string path)
      {
         return EnumerateStreamsInternal(null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="path">A path that describes a directory.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the directory specified by path, or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(string path, string searchPattern, SearchOption searchOption)
      {
         return EnumerateStreamsInternal(null, path, searchPattern, searchOption, false);
      }

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="path">A path that describes a directory.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the directory specified by path, or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateStreamsInternal(null, path, searchPattern, searchOption, continueOnAccessError);
      }

      #region Transacted

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the directory specified by path, or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(KernelTransaction transaction, string path)
      {
         return EnumerateStreamsInternal(transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the directory specified by path, or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return EnumerateStreamsInternal(transaction, path, searchPattern, searchOption, false);
      }

      /// <summary>Returns an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the directory specified by path, or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EnumerateStreamsInternal(transaction, path, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Transacted

      #endregion // EnumerateStreams

      #region Encrypt

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only encrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(string path)
      {
         return EncryptDecryptInternal(true, null, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only encrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(string path, string searchPattern)
      {
         return EncryptDecryptInternal(true, null, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(string path, string searchPattern, SearchOption searchOption)
      {
         return EncryptDecryptInternal(true, null, path, searchPattern, searchOption, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EncryptDecryptInternal(true, null, path, searchPattern, searchOption, continueOnAccessError);
      }

      #region Transacted

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only encrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(KernelTransaction transaction, string path)
      {
         return EncryptDecryptInternal(true, transaction, path, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This will only encrypt the root items, non recursive.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(KernelTransaction transaction, string path, string searchPattern)
      {
         return EncryptDecryptInternal(true, transaction, path, searchPattern, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption)
      {
         return EncryptDecryptInternal(true, transaction, path, searchPattern, searchOption, false);
      }

      /// <summary>Encrypts a directory so that only the account used to encrypt the directory can decrypt it.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         return EncryptDecryptInternal(true, transaction, path, searchPattern, searchOption, continueOnAccessError);
      }

      #endregion // Transacted

      #endregion // Encrypt

      #region EncryptionDisable

      /// <summary>Disables encryption of the specified directory and the files in it. 
      /// It does not affect encryption of subdirectories below the indicated directory. 
      /// </summary>
      /// <param name="path">The name of the directory for which to disable encryption.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method will create/change the file "Desktop.ini" and wil set Encryption value: "Disable=0"</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool EncryptionDisable(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);

         if (!NativeMethods.EncryptionDisable(pathLp, true))
            NativeError.ThrowException(pathLp);

         return true;
      }

      #endregion // EncryptionDisable

      #region EncryptionEnable

      /// <summary>Enables encryption of the specified directory and the files in it. 
      /// It does not affect encryption of subdirectories below the indicated directory. 
      /// </summary>
      /// <param name="path">The name of the directory for which to enable encryption.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>This method will create/change the file "Desktop.ini" and wil set Encryption value: "Disable=1"</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool EncryptionEnable(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);

         if (!NativeMethods.EncryptionDisable(pathLp, false))
            NativeError.ThrowException(pathLp);

         return true;
      }

      #endregion // EncryptionEnable

      #region GetAttributes

      /// <summary>Gets the <see cref="FileAttributes"/> of the directory on the path.</summary>
      /// <param name="path">The path to the directory.</param>
      /// <returns>The <see cref="FileAttributes"/> of the directory on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(string path)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(null, path, false, true).Attributes;
      }

      /// <summary>Gets the <see cref="FileAttributes"/> of the directory on the path.</summary>
      /// <param name="path">The path to the directory.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <returns>The <see cref="FileAttributes"/> of the directory on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(string path, bool basicSearch)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(null, path, basicSearch, true).Attributes;
      }

      #region Transacted

      /// <summary>Gets the <see cref="FileAttributes"/> of the directory on the path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the directory.</param>
      /// <returns>The <see cref="FileAttributes"/> of the directory on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(transaction, path, false, true).Attributes;
      }

      /// <summary>Gets the <see cref="FileAttributes"/> of the directory on the path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the directory.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <returns>The <see cref="FileAttributes"/> of the directory on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(KernelTransaction transaction, string path, bool basicSearch)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(transaction, path, basicSearch, true).Attributes;
      }

      #endregion // Transacted

      #endregion // GetAttributes

      #region GetFileIdBothDirectoryInfo

      /// <summary>Retrieves information about files in the directory specified by <paramref name="path"/> in <see cref="FileShare.ReadWrite"/> mode.</summary>
      /// <param name="path">A path to a directory from which to retrieve information.</param>
      /// <returns>An enumeration of <see cref="FileIdBothDirectoryInfo"/> records for each file system entry in the specified diretory.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<FileIdBothDirectoryInfo> GetFileIdBothDirectoryInfo(string path)
      {
         return GetFileIdBothDirectoryInfoInternal(null, path, null, FileShare.ReadWrite, true);
      }

      /// <summary>Retrieves information about files in the directory specified by <paramref name="path"/> in specified <see cref="FileShare"/> mode.</summary>
      /// <param name="path">A path to a directory from which to retrieve information.</param>
      /// <param name="shareMode">The <see cref="FileShare"/> mode with which to open a handle to the directory.</param>
      /// <returns>An enumeration of <see cref="FileIdBothDirectoryInfo"/> records for each file system entry in the specified diretory.</returns>      
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<FileIdBothDirectoryInfo> GetFileIdBothDirectoryInfo(string path, FileShare shareMode)
      {
         return GetFileIdBothDirectoryInfoInternal(null, path, null, shareMode, true);
      }

      /// <summary>Retrieves information about files in the directory handle specified.</summary>
      /// <param name="directoryHandle">An open handle to the directory from which to retrieve information.</param>
      /// <returns>An IEnumerable of <see cref="FileIdBothDirectoryInfo"/> records for each file system entry in the specified diretory.</returns>    
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<FileIdBothDirectoryInfo> GetFileIdBothDirectoryInfo(SafeFileHandle directoryHandle)
      {
         // FileShare has no effect since a handle is already opened.
         return GetFileIdBothDirectoryInfoInternal(null, null, directoryHandle, FileShare.ReadWrite, true);
      }

      #region Transacted

      /// <summary>Retrieves information about files in the directory specified by <paramref name="path"/> in <see cref="FileShare.ReadWrite"/> mode.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path to a directory from which to retrieve information.</param>
      /// <returns>An enumeration of <see cref="FileIdBothDirectoryInfo"/> records for each file system entry in the specified diretory.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<FileIdBothDirectoryInfo> GetFileIdBothDirectoryInfo(KernelTransaction transaction, string path)
      {
         return GetFileIdBothDirectoryInfoInternal(transaction, path, null, FileShare.ReadWrite, true);
      }

      /// <summary>Retrieves information about files in the directory specified by <paramref name="path"/> in specified <see cref="FileShare"/> mode.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path to a directory from which to retrieve information.</param>
      /// <param name="shareMode">The <see cref="FileShare"/> mode with which to open a handle to the directory.</param>
      /// <returns>An enumeration of <see cref="FileIdBothDirectoryInfo"/> records for each file system entry in the specified diretory.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<FileIdBothDirectoryInfo> GetFileIdBothDirectoryInfo(KernelTransaction transaction, string path, FileShare shareMode)
      {
         return GetFileIdBothDirectoryInfoInternal(transaction, path, null, shareMode, true);
      }

      #endregion // Transacted
      
      #endregion // GetFileIdBothDirectoryInfo

      #region GetLogicalDrives

      /// <summary>Retrieves the names of the logical drives on this computer in the form "driveLetter:\"</summary>
      /// <param name="fromEnvironment">Retrieve logical drives as known by the Environment.</param>
      /// <returns>The logical drives on this computer as <see cref="string"/>[] array.</returns>
      [SecurityCritical]
      public static IEnumerable<string> GetLogicalDrives(bool fromEnvironment)
      {
         return GetLogicalDrives(fromEnvironment, false, false);
      }

      /// <summary>Retrieves the names of the logical drives on this computer in the form "driveLetter:\"</summary>
      /// <param name="fromEnvironment">Retrieve logical drives as known by the Environment.</param>
      /// <param name="isReady">Retrieve only when accessible (IsReady) logical drives.</param>
      /// <returns>The logical drives on this computer as <see cref="string"/>[] array.</returns>
      [SecurityCritical]
      public static IEnumerable<string> GetLogicalDrives(bool fromEnvironment, bool isReady)
      {
         return GetLogicalDrives(fromEnvironment, isReady, false);
      }

      /// <summary>Retrieves the names of the logical drives on this computer in the form "driveLetter:\"</summary>
      /// <param name="fromEnvironment">Retrieve logical drives as known by the Environment.</param>
      /// <param name="isReady">Retrieve only when accessible (IsReady) logical drives.</param>
      /// <param name="removeDirectorySeparator">Remove the <see cref="Path.DirectorySeparatorChar"/> from the logical drive name.</param>
      /// <returns>The logical drives on this computer as <see cref="string"/>[] array.</returns>
      [SecurityCritical]
      public static IEnumerable<string> GetLogicalDrives(bool fromEnvironment, bool isReady, bool removeDirectorySeparator)
      {
         #region Get from Environment

         if (fromEnvironment)
         {
            IEnumerable<string> drivesEnv = isReady
                                               ? Environment.GetLogicalDrives().Where(ld => new DriveInfo(null, ld).IsReady)
                                               : Environment.GetLogicalDrives().Select(ld => ld);

            foreach (string drive in drivesEnv.Select(drv => (removeDirectorySeparator) ? Path.DirectorySeparatorRemove(drv, false) : drv))
            {
               // Optionally check Drive .IsReady.
               if (isReady)
               {
                  if (Volume.IsReady(drive))
                     yield return drive;
               }
               else
                  yield return drive;
            }

            yield break;
         }

         #endregion // Get from Environment

         #region Get through NativeMethod

         uint retVal = NativeMethods.GetLogicalDrives();
         uint drives = retVal;
         int count = 0;
         while (drives != 0)
         {
            if ((drives & 1) != 0)
               count++;

            drives >>= 1;
         }

         string[] result = new string[count];

         char[] root = removeDirectorySeparator
                           ? new[] { 'A', Path.VolumeSeparatorChar }
                           : new[] { 'A', Path.VolumeSeparatorChar, Path.DirectorySeparatorChar };

         drives = retVal;
         count = 0;

         while (drives != 0)
         {
            if ((drives & 1) != 0)
            {
               string drive = new string(root);

               if (isReady)
               {
                  // Optionally check Drive .IsReady.
                  if (Volume.IsReady(drive))
                     yield return drive;
               }
               else
               {
                  // Ready or not.
                  yield return drive;
               }

               result[count++] = drive;
            }

            drives >>= 1;
            root[0]++;
         }

         #endregion // Get through NativeMethod
      }

      #endregion // GetLogicalDrives

      #region GetProperties

      /// <summary>Gets the properties of the particular folder without following any symbolic links or mount points.
      /// Properties include aggregated info from <see cref="FileAttributes"/> of each encountered file system object.
      /// Plus additional ones: Total, File, Size, Error
      /// <para><b>Total:</b> is the total number of enumerated objects.</para>
      /// <para><b>File:</b> is the total number of files. File is considered when object is neither <see cref="FileAttributes.Directory"/> nor <see cref="FileAttributes.ReparsePoint"/>.</para>
      /// <para><b>Size:</b> is the total size of enumerated objects.</para>
      /// <para><b>Error:</b> is the total number of errors encountered during request.</para>
      /// </summary>
      /// <remarks><b>Directory:</b> is an object which has <see cref="FileAttributes.Directory"/> attribute without <see cref="FileAttributes.ReparsePoint"/> one.</remarks>
      /// <param name="path">The target directory.</param>
      /// <returns>A dictionary mapping the keys mentioned above to their respective aggregated values.</returns>
      [SecurityCritical]
      public static Dictionary<string, long> GetProperties(string path)
      {
         return GetPropertiesInternal(null, path, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Gets the properties of the particular folder without following any symbolic links or mount points.
      /// Properties include aggregated info from <see cref="FileAttributes"/> of each encountered file system object.
      /// Plus additional ones: Total, File, Size, Error
      /// <para><b>Total:</b> is the total number of enumerated objects.</para>
      /// <para><b>File:</b> is the total number of files. File is considered when object is neither <see cref="FileAttributes.Directory"/> nor <see cref="FileAttributes.ReparsePoint"/>.</para>
      /// <para><b>Size:</b> is the total size of enumerated objects.</para>
      /// <para><b>Error:</b> is the total number of errors encountered during request.</para>
      /// </summary>
      /// <remarks><b>Directory:</b> is an object which has <see cref="FileAttributes.Directory"/> attribute without <see cref="FileAttributes.ReparsePoint"/> one.</remarks>
      /// <param name="path">The target directory.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> continue on <see cref="System.UnauthorizedAccessException"/> errors.</param>
      /// <returns>A dictionary mapping the keys mentioned above to their respective aggregated values.</returns>
      [SecurityCritical]
      public static Dictionary<string, long> GetProperties(string path, bool continueOnAccessError)
      {
         return GetPropertiesInternal(null, path, SearchOption.TopDirectoryOnly, continueOnAccessError);
      }

      /// <summary>Gets the properties of the particular folder without following any symbolic links or mount points.
      /// Properties include aggregated info from <see cref="FileAttributes"/> of each encountered file system object.
      /// Plus additional ones: Total, File, Size, Error
      /// <para><b>Total:</b> is the total number of enumerated objects.</para>
      /// <para><b>File:</b> is the total number of files. File is considered when object is neither <see cref="FileAttributes.Directory"/> nor <see cref="FileAttributes.ReparsePoint"/>.</para>
      /// <para><b>Size:</b> is the total size of enumerated objects.</para>
      /// <para><b>Error:</b> is the total number of errors encountered during request.</para>
      /// </summary>
      /// <remarks><b>Directory:</b> is an object which has <see cref="FileAttributes.Directory"/> attribute without <see cref="FileAttributes.ReparsePoint"/> one.</remarks>
      /// <param name="path">The target directory.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> continue on <see cref="System.UnauthorizedAccessException"/> errors.</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>A dictionary mapping the keys mentioned above to their respective aggregated values.</returns>
      [SecurityCritical]
      public static Dictionary<string, long> GetProperties(string path, bool continueOnAccessError, SearchOption searchOption)
      {
         return GetPropertiesInternal(null, path, searchOption, continueOnAccessError);
      }

      #region Transacted

      /// <summary>Gets the properties of the particular folder without following any symbolic links or mount points.
      /// Properties include aggregated info from <see cref="FileAttributes"/> of each encountered file system object.
      /// Plus additional ones: Total, File, Size, Error
      /// <para><b>Total:</b> is the total number of enumerated objects.</para>
      /// <para><b>File:</b> is the total number of files. File is considered when object is neither <see cref="FileAttributes.Directory"/> nor <see cref="FileAttributes.ReparsePoint"/>.</para>
      /// <para><b>Size:</b> is the total size of enumerated objects.</para>
      /// <para><b>Error:</b> is the total number of errors encountered during request.</para>
      /// </summary>
      /// <remarks><b>Directory:</b> is an object which has <see cref="FileAttributes.Directory"/> attribute without <see cref="FileAttributes.ReparsePoint"/> one.</remarks>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The target directory.</param>
      /// <returns>A dictionary mapping the keys mentioned above to their respective aggregated values.</returns>
      [SecurityCritical]
      public static Dictionary<string, long> GetProperties(KernelTransaction transaction, string path)
      {
         return GetPropertiesInternal(transaction, path, SearchOption.TopDirectoryOnly, false);
      }

      /// <summary>Gets the properties of the particular folder without following any symbolic links or mount points.
      /// Properties include aggregated info from <see cref="FileAttributes"/> of each encountered file system object.
      /// Plus additional ones: Total, File, Size, Error
      /// <para><b>Total:</b> is the total number of enumerated objects.</para>
      /// <para><b>File:</b> is the total number of files. File is considered when object is neither <see cref="FileAttributes.Directory"/> nor <see cref="FileAttributes.ReparsePoint"/>.</para>
      /// <para><b>Size:</b> is the total size of enumerated objects.</para>
      /// <para><b>Error:</b> is the total number of errors encountered during request.</para>
      /// </summary>
      /// <remarks><b>Directory:</b> is an object which has <see cref="FileAttributes.Directory"/> attribute without <see cref="FileAttributes.ReparsePoint"/> one.</remarks>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The target directory.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> continue on <see cref="System.UnauthorizedAccessException"/> errors.</param>
      /// <returns>A dictionary mapping the keys mentioned above to their respective aggregated values.</returns>
      [SecurityCritical]
      public static Dictionary<string, long> GetProperties(KernelTransaction transaction, string path, bool continueOnAccessError)
      {
         return GetPropertiesInternal(transaction, path, SearchOption.TopDirectoryOnly, continueOnAccessError);
      }

      /// <summary>Gets the properties of the particular folder without following any symbolic links or mount points.
      /// Properties include aggregated info from <see cref="FileAttributes"/> of each encountered file system object.
      /// Plus additional ones: Total, File, Size, Error
      /// <para><b>Total:</b> is the total number of enumerated objects.</para>
      /// <para><b>File:</b> is the total number of files. File is considered when object is neither <see cref="FileAttributes.Directory"/> nor <see cref="FileAttributes.ReparsePoint"/>.</para>
      /// <para><b>Size:</b> is the total size of enumerated objects.</para>
      /// <para><b>Error:</b> is the total number of errors encountered during request.</para>
      /// </summary>
      /// <remarks><b>Directory:</b> is an object which has <see cref="FileAttributes.Directory"/> attribute without <see cref="FileAttributes.ReparsePoint"/> one.</remarks>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The target directory.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> continue on <see cref="System.UnauthorizedAccessException"/> errors.</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <returns>A dictionary mapping the keys mentioned above to their respective aggregated values.</returns>
      [SecurityCritical]
      public static Dictionary<string, long> GetProperties(KernelTransaction transaction, string path, bool continueOnAccessError, SearchOption searchOption)
      {
         return GetPropertiesInternal(transaction, path, searchOption, continueOnAccessError);
      }

      #endregion // Transacted

      #endregion // GetProperties

      #region HasInheritedPermissions

      /// <summary>Check if the directory has permission inheritance enabled.</summary>
      /// <param name="directoryPath">The full path to the directory to check.</param>
      /// <returns><c>true</c> if permission inheritance is enabled, <c>false</c> if permission inheritance is disabled.</returns>
      public static bool HasInheritedPermissions(string directoryPath)
      {
         if (string.IsNullOrEmpty(directoryPath))
            throw new ArgumentNullException("directoryPath");

         DirectorySecurity acl = GetAccessControl(directoryPath);
         return acl.GetAccessRules(false, true, typeof (SecurityIdentifier)).Count > 0;
      }

      /// <summary>Check if the directory has permission inheritance enabled.</summary>
      /// <param name="directoryInfo">A <see cref="DirectoryInfo"/> instance check.</param>
      /// <returns><c>true</c> if permission inheritance is enabled, <c>false</c> if permission inheritance is disabled.</returns>
      public static bool HasInheritedPermissions(DirectoryInfo directoryInfo)
      {
         if (directoryInfo == null)
            throw new ArgumentNullException("directoryInfo");

         DirectorySecurity acl = directoryInfo.GetAccessControl();
         return acl.GetAccessRules(false, true, typeof(SecurityIdentifier)).Count > 0;
      }

      #endregion // HasInheritedPermissions

      #region Move

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="sourcePath">The path of the file or directory to move.</param>
      /// <param name="destinationPath">The path to the new location for sourcePath. If sourcePath is a file, then destinationPath must also be a file name.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, false, null, MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="sourcePath">The path of the file or directory to move.</param>
      /// <param name="destinationPath">The path to the new location for sourcePath. If sourcePath is a file, then destinationPath must also be a file name.</param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Move will fail on existing folders or files.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(string sourcePath, string destinationPath, bool overwrite)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, false, null, overwrite ? NativeMethods.MoveOptsReplace : MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Move will fail on existing folders or files.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(string sourcePath, string destinationPath, bool overwrite, bool preserveSecurity)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, preserveSecurity, null, overwrite ? NativeMethods.MoveOptsReplace : MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="moveOptions"><see cref="MoveOptions"/> that specify how the directory is to be moved. This parameter can be <see langword="null"/>.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(string sourcePath, string destinationPath, MoveOptions moveOptions, bool preserveSecurity, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(true, null, sourcePath, destinationPath, preserveSecurity, null, moveOptions, copyProgress, userProgressData);
      }

      #region Transacted

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The path of the file or directory to move.</param>
      /// <param name="destinationPath">The path to the new location for sourcePath. If sourcePath is a file, then destinationPath must also be a file name.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(KernelTransaction transaction, string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, false, null, MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The path of the file or directory to move.</param>
      /// <param name="destinationPath">The path to the new location for sourcePath. If sourcePath is a file, then destinationPath must also be a file name.</param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Move will fail on existing folders or files.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(KernelTransaction transaction, string sourcePath, string destinationPath, bool overwrite)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, false, null, overwrite ? NativeMethods.MoveOptsReplace : MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="overwrite"><c>true</c> Delete destination folder if it exists; <c>false</c> Move will fail on existing folders or files.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(KernelTransaction transaction, string sourcePath, string destinationPath, bool overwrite, bool preserveSecurity)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, preserveSecurity, null, overwrite ? NativeMethods.MoveOptsReplace : MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or a directory and its contents to a new location.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source folder path, of type <see cref="string"/></param>
      /// <param name="destinationPath">The destination folder path, of type <see cref="string"/></param>
      /// <param name="moveOptions"><see cref="MoveOptions"/> that specify how the directory is to be moved. This parameter can be <see langword="null"/>.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(KernelTransaction transaction, string sourcePath, string destinationPath, MoveOptions moveOptions, bool preserveSecurity, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(true, transaction, sourcePath, destinationPath, preserveSecurity, null, moveOptions, copyProgress, userProgressData);
      }

      #endregion // Transacted

      #endregion // Move

      #region SetAccessControl

      /// <summary>Applies access control list (ACL) entries described by a <see cref="DirectorySecurity"/> object to the specified directory.</summary>
      /// <param name="path">A directory to add or remove access control list (ACL) entries from.</param>
      /// <param name="directorySecurity">A <see cref="DirectorySecurity "/> object that describes an ACL entry to apply to the directory described by the path parameter.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public static bool SetAccessControl(string path, DirectorySecurity directorySecurity)
      {
         // In this case, equals null, is a good thing.
         return (FileSystemInfo.GetSetAccessControlInternal(true, true, path, directorySecurity, AccessControlSections.All) == null);
      }

      /// <summary>Applies access control list (ACL) entries described by a <see cref="DirectorySecurity"/> object to the specified directory.</summary>
      /// <param name="path">A directory to add or remove access control list (ACL) entries from.</param>
      /// <param name="directorySecurity">A <see cref="DirectorySecurity "/> object that describes an ACL entry to apply to the directory described by the path parameter.</param>
      /// <param name="includeSections">One or more of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to set.</param>
      /// <remarks>Note that unlike <see cref="System.IO.File.SetAccessControl"/> this method does <b>not</b> automatically
      /// determine what parts of the specified <see cref="DirectorySecurity"/> instance has been modified. Instead, the
      /// parameter <paramref name="includeSections"/> is used to specify what entries from <paramref name="directorySecurity"/> to apply to <paramref name="path"/>.</remarks>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public static bool SetAccessControl(string path, DirectorySecurity directorySecurity, AccessControlSections includeSections)
      {
         // In this case, equals null, is a good thing.
         return (FileSystemInfo.GetSetAccessControlInternal(true, true, path, directorySecurity, includeSections) == null);
      }

      #endregion // SetAccessControl

      #region SetTimestamps

      /// <summary>Sets the time stamps at once.</summary>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestamps(string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // File.SetTimestamps()
         FileSystemInfo.SetFileTimeInternal(true, null, path, creationTime.ToFileTime(), lastAccessTime.ToFileTime(), lastWriteTime.ToFileTime());
      }

      #region Transacted

      /// <summary>Sets the time stamps at once.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestamps(KernelTransaction transaction, string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // File.SetTimestamps()
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, creationTime.ToFileTime(), lastAccessTime.ToFileTime(), lastWriteTime.ToFileTime());
      }

      #endregion // Transacted

      #endregion // SetTimestamps

      #region SetTimestampsUtc

      /// <summary>Sets all the time stamps at once in UTC.</summary>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <remarks>This method is redundant, because NTFS driver converts any dates in UTC format anyways.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestampsUtc(string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // File.SetTimestampsUtc()
         FileSystemInfo.SetFileTimeInternal(true, null, path, creationTime.ToFileTimeUtc(), lastAccessTime.ToFileTimeUtc(), lastWriteTime.ToFileTimeUtc());
      }

      #region Transacted

      /// <summary>Sets all the time stamps at once in UTC.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <remarks>This method is redundant, because NTFS driver converts any dates in UTC format anyways.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestampsUtc(KernelTransaction transaction, string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // File.SetTimestampsUtc()
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, creationTime.ToFileTimeUtc(), lastAccessTime.ToFileTimeUtc(), lastWriteTime.ToFileTimeUtc());
      }

      #endregion // Transacted

      #endregion // SetTimestampsUtc

      #region TransferTimestamps

      /// <summary>Transfers the time stamps for files and directories.</summary>
      /// <param name="source">The source path.</param>
      /// <param name="destination">The destination path.</param>
      /// <remarks>
      /// This method uses BackupSemantics flag to get Timestamp changed for folders.
      /// This method does not change last access time for the source file.
      /// </remarks>
      [SecurityCritical]
      public static bool TransferTimestamps(string source, string destination)
      {
         return FileSystemInfo.TransferTimestampsInternal(true, null, source, destination);
      }

      #region Transacted

      /// <summary>Transfers the time stamps for files and directories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="source">The source path.</param>
      /// <param name="destination">The destination path.</param>
      /// <remarks>
      /// This method uses BackupSemantics flag to get Timestamp changed for folders.
      /// This method does not change last access time for the source file.
      /// </remarks>
      [SecurityCritical]
      public static bool TransferTimestamps(KernelTransaction transaction, string source, string destination)
      {
         return FileSystemInfo.TransferTimestampsInternal(true, transaction, source, destination);
      }

      #endregion // Transacted

      #endregion // TransferTimestamps
      

      #region Unified Internals

      #region CompressDecompressInternal

      /// <summary>Unified method CompressDecompressInternal() to compress/decompress Non-/Transacted directories/files.</summary>
      /// <param name="compress">When <c>true</c> compress, when <c>false</c> decompress.</param>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory to compress.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      internal static bool CompressDecompressInternal(bool compress, KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         // Process folders and files.
         foreach (string fso in FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, null, true, continueOnAccessError).Cast<string>())
            NativeMethods.DeviceIo.CompressionEnableInternal(true, transaction, fso, compress);

         // Compress the root folder, the given path.
         return NativeMethods.DeviceIo.CompressionEnableInternal(true, transaction, path, compress);
      }

      #endregion // CompressDecompressInternal

      #region CopyMoveInternal

      /// <summary>Unified method CopyMoveInternal() to copy/move a Non-/Transacted file or directory including its children.
      /// You can provide a callback function that receives progress notifications.</summary>
      /// <param name="isFolder"><c>true</c> indicates a directory object, <c>false</c> indicates a file object.</param>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source directory path.</param>
      /// <param name="destinationPath">The destination directory path.</param>
      /// <param name="preserveSecurity"><c>true</c> Preserves directory ACLs information.</param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the directory is to be copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="moveOptions"><see cref="MoveOptions"/> that specify how the directory is to be moved. This parameter can be <see langword="null"/>.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <remarks>This Move method works across disk volumes, and it does not throw an exception if the source and destination are
      /// the same. Note that if you attempt to replace a file by moving a file of the same name into that directory, you
      /// get an IOException. You cannot use the Move method to overwrite an existing file.</remarks>
      /// <returns><c>true</c> when successfully copied or moved, <c>false</c> on failure or the operation was aborted.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool CopyMoveInternal(bool isFolder, KernelTransaction transaction, string sourcePath, string destinationPath, bool preserveSecurity, CopyOptions? copyOptions, MoveOptions? moveOptions, CopyProgressRoutine copyProgress, object userProgressData)
      {
         if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))
            return false;

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string sourcePathLp = Path.PrefixLongPath(sourcePath);
         string destinationPathLp = Path.PrefixLongPath(destinationPath);

         bool isCopy = copyOptions != null && moveOptions == null;
         bool isMove = moveOptions != null && copyOptions == null;
         bool overwrite = isCopy
                             ? ((CopyOptions)copyOptions & CopyOptions.FailIfExists) == 0
                             : isMove && ((MoveOptions)moveOptions & MoveOptions.ReplaceExisting) != 0;

         // Check if "old" directory needs to be removed first.
         if (overwrite)
         {
            if (FileSystemInfo.ExistsInternal(isFolder, transaction, destinationPathLp))
               DeleteDirectoryInternal(transaction, destinationPathLp, true, true, Path.WildcardStarMatchAll);
         }

         #region Move, Same Drive

         // Moving of files and folders + children is only possible if it's performed on the same drive.
         if (isMove && destinationPathLp.StartsWith(new PathInfo(sourcePathLp, false).Root, StringComparison.OrdinalIgnoreCase))
            return File.CopyMoveInternal(transaction, sourcePathLp, destinationPathLp, false, null, moveOptions, copyProgress, userProgressData);

         // Seems that the destination is located on another drive.
         // We perform a "copy all/delete all" instead of throwing an exception like: "source and destination must be on the same drive.";

         #endregion // Move, Same Drive

         #region Copy/Move, Different Drive

         // Preserves directory ACL information.
         DirectorySecurity dirSecurity = (DirectorySecurity)(preserveSecurity && isFolder
               ? FileSystemInfo.GetSetAccessControlInternal(true, false, sourcePathLp, null, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner)
               : null);

         CreateDirectoryInternal(transaction, null, destinationPathLp, dirSecurity, null);

         foreach (FileSystemInfo fsei in FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, sourcePathLp, Path.WildcardStarMatchAll, SearchOption.TopDirectoryOnly, true, null, false, false))
         {
            //if (fse.IsReparsePoint) continue;

            string newDst = Path.Combine(destinationPathLp, fsei.SystemInfo.FileName);

            if (fsei.SystemInfo.IsDirectory)
               CopyMoveInternal(isFolder, transaction, fsei.SystemInfo.FullPath, newDst, preserveSecurity, copyOptions, moveOptions, copyProgress, userProgressData);
            else
               File.CopyMoveInternal(transaction, fsei.SystemInfo.FullPath, newDst, false, copyOptions, moveOptions, copyProgress, userProgressData);
         }

         if (isMove)
            DeleteDirectoryInternal(transaction, sourcePathLp, true, true, Path.WildcardStarMatchAll);

         return true;

         #endregion // Copy/Move, Different Drive
      }

      #endregion // CopyMoveInternal

      #region CreateDirectoryInternal

      /// <summary>Unified method CreateDirectoryInternal() to create a new directory with the attributes of a specified template directory (if one is specified). 
      /// If the underlying file system supports security on files and directories, the function 
      /// applies the specified security descriptor to the new directory. The new directory retains 
      /// the other attributes of the specified template directory.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="templatePath">The path of the directory to use as a template when creating the new directory. 
      /// May be <see langword="null"/> to indicate that no template should be used.</param>
      /// <param name="path">The directory path to create.</param>
      /// <param name="directorySecurity">The <see cref="DirectorySecurity"/> access control to apply to the directory, may be <see langword="null"/>.</param>
      /// <param name="securityAttributes">
      /// The security descriptor to apply to the newly created directory.
      /// May be <see langword="null"/> in which case a default security descriptor will be applied.</param>
      /// <returns>Returns <c>true</c> on success, <c>false</c> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool CreateDirectoryInternal(KernelTransaction transaction, string templatePath, string path, ObjectSecurity directorySecurity, Security.NativeMethods.SecurityAttributes securityAttributes)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         Security.NativeMethods.SecurityAttributes securityAttributes2 = securityAttributes;
         if (directorySecurity != null)
         {
            SafeGlobalMemoryBufferHandle securityDescriptorBuffer;
            securityAttributes2 = new Security.NativeMethods.SecurityAttributes();
            Security.NativeMethods.SecurityAttributes.Initialize(out securityDescriptorBuffer, directorySecurity);
         }

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string templatePathLp = Path.PrefixLongPath(templatePath);
         string newPathLp = Path.PrefixLongPath(path);


         if (transaction == null
                ? (templatePath == null
                      ? NativeMethods.CreateDirectory(newPathLp, securityAttributes2)
                      : NativeMethods.CreateDirectoryEx(templatePathLp, newPathLp, securityAttributes2))
                : NativeMethods.CreateDirectoryTransacted(templatePathLp, newPathLp, securityAttributes2, transaction.SafeHandle))
            return true;

         bool raiseException = false;

         int lastError = Marshal.GetLastWin32Error();
         switch ((uint)lastError)
         {
            case Win32Errors.ERROR_PATH_NOT_FOUND:
               PathInfo pathInfo = new PathInfo(newPathLp, false);
               string dirName = pathInfo.DirectoryName;

               // Check that the root exists and that there's no file with the same name.
               if (FileSystemInfo.ExistsInternal(true, transaction, pathInfo.Root) && !FileSystemInfo.ExistsInternal(true, transaction, dirName))
               {
                  CreateDirectoryInternal(transaction, templatePathLp, dirName, directorySecurity, securityAttributes2);
                  CreateDirectoryInternal(transaction, templatePathLp, newPathLp, directorySecurity, securityAttributes2);
               }
               else
                  raiseException = true;
               break;

            case Win32Errors.ERROR_ALREADY_EXISTS:
               // As stated in the MSDN article for Directory.CreateDirectory() method,
               // it should throw exception only for existing files with the same name as requested directory.
               // http://msdn.microsoft.com/en-us/library/54a0at6s.aspx

               // Check file.
               raiseException = FileSystemInfo.ExistsInternal(false, transaction, newPathLp);
               break;

            default:
               // Throw exceptions for everything else.
               raiseException = true;
               break;
         }

         if (raiseException)
            NativeError.ThrowException(lastError, newPathLp);

         return true;
      }

      #endregion // CreateDirectoryInternal

      #region GetFileIdBothDirectoryInfoInternal

      /// <summary>Unified method GetFileIdBothDirectoryInfoInternal() to retrieves information about files in the directory handle specified.</summary>
      /// <param name="transaction"></param>
      /// <param name="directoryPath">A path to the directory.</param>
      /// <param name="directoryHandle">An open handle to the directory from which to retrieve information.</param>
      /// <param name="shareMode">The <see cref="FileShare"/> mode with which to open a handle to the directory.</param>
      /// <param name="raiseException">If <c>true</c> raises Exceptions, when <c>false</c> no Exceptions are raised and the method returns <see langref="null"/>.</param>
      /// <returns>An IEnumerable of <see cref="FileIdBothDirectoryInfo"/> records for each file system entry in the specified diretory.</returns>    
      /// <remarks>Either use <paramref name="directoryPath"/> or <paramref name="directoryHandle"/>, not both.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static IEnumerable<FileIdBothDirectoryInfo> GetFileIdBothDirectoryInfoInternal(KernelTransaction transaction, string directoryPath, SafeFileHandle directoryHandle, FileShare shareMode, bool raiseException)
      {
         if (!string.IsNullOrEmpty(directoryPath))
         {
            directoryPath = Path.GetRegularPath(Path.DirectorySeparatorAdd(directoryPath, false));

            // In the ANSI version of this function, the name is limited to MAX_PATH characters.
            // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
            // 2013-01-13: MSDN confirms LongPath usage.
            directoryPath = Path.PrefixLongPath(directoryPath);

            // To open a directory using CreateFile, specify the FILE_FLAG_BACKUP_SEMANTICS flag as part of dwFlagsAndAttributes.
            directoryHandle = FileSystemInfo.CreateFileInternal(false, transaction, directoryPath, EFileAttributes.BackupSemantics, null, FileMode.Open, FileSystemRights.ListDirectory, shareMode);
         }
         else if (!NativeMethods.IsValidHandle(directoryHandle, raiseException))
            yield return null;
         
         
         using (SafeGlobalMemoryBufferHandle safeBuffer = new SafeGlobalMemoryBufferHandle(NativeMethods.DefaultFileBufferSize))
         {
            NativeMethods.IsValidHandle(safeBuffer);

            long fileNameOffset = Marshal.OffsetOf(typeof(NativeMethods.FileIdBothDirInfo), "FileName").ToInt64();

            while (NativeMethods.GetFileInformationByHandleEx(directoryHandle, NativeMethods.FileInfoByHandleClass.FileIdBothDirectoryInfo, safeBuffer, NativeMethods.DefaultFileBufferSize))
            {
               IntPtr buffer = safeBuffer.DangerousGetHandle();
               while (buffer != IntPtr.Zero)
               {
                  NativeMethods.FileIdBothDirInfo fdi = NativeMethods.GetStructure<NativeMethods.FileIdBothDirInfo>(0, buffer);

                  string fileName = Marshal.PtrToStringUni(new IntPtr(fileNameOffset + buffer.ToInt64()), (int)(fdi.FileNameLength / 2));

                  if (!string.IsNullOrEmpty(fileName) &&
                      !fileName.Equals(Path.CurrentDirectoryPrefix, StringComparison.OrdinalIgnoreCase) &&
                      !fileName.Equals(Path.ParentDirectoryPrefix, StringComparison.OrdinalIgnoreCase))
                     yield return new FileIdBothDirectoryInfo(fdi, fileName);

                  buffer = fdi.NextEntryOffset != 0 ? new IntPtr(buffer.ToInt64() + fdi.NextEntryOffset) : IntPtr.Zero;
               }
            }

            int lastError = Marshal.GetLastWin32Error();
            switch ((uint)lastError)
            {
               case Win32Errors.ERROR_SUCCESS:
               case Win32Errors.ERROR_NO_MORE_FILES:
               case Win32Errors.ERROR_HANDLE_EOF:
                  yield break;

               default:
                  NativeError.ThrowException(lastError);
                  break;
            }
         }
      }

      #endregion // GetFileIdBothDirectoryInfoInternal

      #region DeleteDirectoryInternal

      /// <summary>Unified method DeleteDirectoryInternal() to delete a Non-/Transacted directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove.</param>
      /// <param name="recursive"><c>true</c> to remove all files and subdirectories recursively; otherwise, <c>false</c> only the top level empty directory.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only attribute of files and directories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include
      /// only the current directory or should include all subdirectories. The default value is <see cref="SearchOption.TopDirectoryOnly"/>.
      /// <returns>Returns <c>true</c> on success, <c>false</c> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool DeleteDirectoryInternal(KernelTransaction transaction, string path, bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string pathLp = Path.PrefixLongPath(path);

         if (recursive)
         {
            foreach (FileSystemInfo fsei in FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, pathLp, searchPattern, SearchOption.AllDirectories, true, null, false, false))
            {
               // Do not recursively delete through reparse points.
               if (fsei.SystemInfo.IsReparsePoint)
               {
                  string mountPoint = fsei.SystemInfo.FullPath; // Path.Combine(pathLp, fsei.FileName);

                  // Do not follow mount points nor symbolic links, but do delete the reparse point itself. 
                  if (fsei.SystemInfo.IsMountPoint)
                     Volume.DeleteVolumeMountPoint(mountPoint);

                  // RemoveDirectory on a symbolic link will remove the link itself. 
                  if (!DeleteDirectoryInternal(transaction, mountPoint, false, true, Path.WildcardStarMatchAll))
                     return false;
               }
               else
               {
                  if (!(fsei.SystemInfo.IsDirectory
                           ? DeleteDirectoryInternal(transaction, fsei.SystemInfo.FullPath, true, ignoreReadOnly, searchPattern)
                           : File.DeleteFileInternal(transaction, fsei.SystemInfo.FullPath, ignoreReadOnly)))
                     return false;
               }
            }
         }
         
         #region Remove

         bool raiseException = false;
         bool deleteOk = transaction == null
                            ? NativeMethods.RemoveDirectory(pathLp)
                            : NativeMethods.RemoveDirectoryTransacted(pathLp, transaction.SafeHandle);

         if (!deleteOk)
         {
            int lastError = Marshal.GetLastWin32Error();
            switch ((uint)lastError)
            {
               case Win32Errors.ERROR_ACCESS_DENIED:
                  if (ignoreReadOnly)
                  {
                     // Repeat the deletion again after removing ReadOnly flag.
                     deleteOk = FileSystemInfo.SetAttributesInternal(transaction, pathLp, FileAttributes.Normal) &&
                                 DeleteDirectoryInternal(transaction, pathLp, false, true, searchPattern);

                     if (deleteOk)
                        deleteOk = DeleteDirectoryInternal(transaction, pathLp, true, false, searchPattern);

                     raiseException = !deleteOk;
                  }
                  else
                     raiseException = true;
                  break;

               case Win32Errors.ERROR_DIR_NOT_EMPTY:
                  return DeleteDirectoryInternal(transaction, pathLp, true, true, searchPattern);

               case Win32Errors.ERROR_FILE_NOT_FOUND:
                  // Ensure we throw a DirectoryNotFoundException.
                  lastError = (int) Win32Errors.ERROR_PATH_NOT_FOUND;
                  raiseException = true;
                  break;

                  // Good job.
               case Win32Errors.ERROR_NO_MORE_FILES:
                  deleteOk = true;
                  break;
                     
               default:
                  raiseException = lastError != Win32Errors.NO_ERROR;
                  break;
            }

            if (raiseException)
               NativeError.ThrowException(lastError, pathLp);
         }

         return deleteOk;

         #endregion // Remove
      }

      #endregion // DeleteDirectoryInternal

      #region DeleteEmptyDirectoryInternal

      /// <summary>Unified method DeleteEmptyDirectoryInternal() to delete empty subdirectores from the specified directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the directory to remove empty subdirectories from.</param>
      /// <param name="recursive"><c>true</c> to remove empty subdirectories in path; otherwise, <c>false</c>.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides read only <see cref="FileAttributes"/> of empty subdirectories.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <returns>Returns <c>true</c> on success, <c>false</c> on failure.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      internal static bool DeleteEmptyDirectoryInternal(KernelTransaction transaction, string path, bool recursive, bool ignoreReadOnly, string searchPattern)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

         try
         {
            foreach (string directoryPath in FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, true, true, false))
               DeleteEmptyDirectoryInternal(transaction, directoryPath, recursive, ignoreReadOnly, searchPattern);

            if (!FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, Path.WildcardStarMatchAll, searchOption, true, null, true, false).Any())
               DeleteDirectoryInternal(transaction, path, false, ignoreReadOnly, Path.WildcardStarMatchAll);

            return true;
         }
         catch
         {
            return false;
         }
      }

      #endregion // DeleteEmptyDirectoryInternal

      #region EncryptDecryptInternal

      /// <summary>Unified method EncryptDecryptInternal() to encrypt/decrypt Non-/Transacted directories/files.</summary>
      /// <param name="encrypt">When <c>true</c> encrypts when <c>false</c> decrypt.</param>
      /// <param name="transaction"></param>
      /// <param name="path">A path that describes a directory to encrypt.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool EncryptDecryptInternal(bool encrypt, KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         // Process folders and files.
         foreach (string fso in FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, null, true, continueOnAccessError).Cast<string>())
         {
            if (encrypt)
               File.Encrypt(fso);

            else
               File.Decrypt(fso);
         }

         // Encrypt/Decrypt the root folder, the given path.
         return (encrypt) ? File.Encrypt(path) : File.Decrypt(path);
      }

      #endregion // EncryptDecryptInternal
      
      #region EnumerateStreamsInternal

      /// <summary>Unified method EnumerateStreamsInternal() to return an enumerable collection of <see cref="BackupStreamInfo"/> instances, associated with the directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a directory.</param>
      /// <param name="searchPattern">A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the directory specified by path, or <see langword="null"/> on error.</returns>
      [SecurityCritical]
      internal static IEnumerable<BackupStreamInfo> EnumerateStreamsInternal(KernelTransaction transaction, string path, string searchPattern, SearchOption searchOption, bool continueOnAccessError)
      {
         foreach (string file in FileSystemInfo.EnumerateFileSystemObjectsInternal(transaction, path, searchPattern, searchOption, true, null, true, continueOnAccessError).Cast<string>())
         {
            foreach (BackupStreamInfo bsi in File.EnumerateStreams(transaction, file))
            {
               // Only add "Source" when property is empty.
               if (string.IsNullOrEmpty(bsi.Source))
                  bsi.Source = file;

               yield return bsi;
            }
         }
      }

      #endregion // EnumerateStreamsInternal
      
      #region GetPropertiesInternal

      /// <summary>Unified method GetPropertiesInternal() to gets the properties of the particular folder without following any symbolic links or mount points.
      /// Properties include aggregated info from <see cref="FileAttributes"/> of each encountered file system object.
      /// Plus additional ones: "Total", "File", "Size" and "SizeCompressed".
      /// <para><b>Total:</b> is the total number of enumerated objects.</para>
      /// <para><b>File:</b> is the total number of files. File is considered when object is neither <see cref="FileAttributes.Directory"/> nor <see cref="FileAttributes.ReparsePoint"/>.</para>
      /// <para><b>Size:</b> is the total size of enumerated objects.</para>
      /// /// <para><b>Size:</b> is the total compressed size of enumerated objects.</para>
      /// <para><b>Error:</b> is the total number of errors encountered during request.</para>
      /// </summary>
      /// <remarks><b>Directory:</b> is an object which has <see cref="FileAttributes.Directory"/> attribute without <see cref="FileAttributes.ReparsePoint"/> one.</remarks>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The target directory.</param>
      /// <param name="searchOption">One of the <see cref="SearchOption"/> enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
      /// <param name="continueOnAccessError">If set to <c>true</c> continue on <see cref="System.UnauthorizedAccessException"/> errors.</param>
      /// <returns>A dictionary mapping the keys mentioned above to their respective aggregated values.</returns>
      [SecurityCritical]
      internal static Dictionary<string, long> GetPropertiesInternal(KernelTransaction transaction, string path, SearchOption searchOption, bool continueOnAccessError)
      {
         const string propFile = "File";
         const string propTotal = "Total";
         const string propSize = "Size";
         const string propSizeCompressed = "SizeCompressed";
         long total = 0;
         long size = 0;
         long sizeCompressed = 0;
         Type typeOfAttrs = typeof(FileAttributes);
         Array attributes = Enum.GetValues(typeOfAttrs);
         Dictionary<string, long> props = Enum.GetNames(typeOfAttrs).ToDictionary<string, string, long>(name => name, name => 0);

         foreach (FileSystemEntryInfo fsei in new FileSystemEntry
            {
               BasicSearch = NativeMethods.BasicSearch,
               ContinueOnAccessError = continueOnAccessError,
               InputPath = path,
               LargeCache = NativeMethods.LargeCache,
               SearchOption = searchOption,
               Transaction = transaction

            }.Enumerate())
         {
            total++;

            if (fsei.IsFile)
            {
               size += fsei.FileSize;
               sizeCompressed += File.GetCompressedSize(transaction, fsei.FullPath);
            }

            foreach (FileAttributes attributeMarker in attributes)
            {
               // Skip on reparse points here to cleanly separate regular directories from links.
               if (fsei.IsReparsePoint)
                  continue;

               FileAttributes attribute = attributeMarker;

               // Marker exists in flags.
               if (NativeMethods.HasFileAttribute(fsei.Attributes, attribute))
                  props[(NativeMethods.HasFileAttribute(attribute, FileAttributes.Directory)
                     // Regular directory that will go to stack, adding directory flag ++
                            ? FileAttributes.Directory
                            : attribute).ToString()]++;
            }
         }

         // Adjust regular files count.
         props.Add(propFile, total - props[FileAttributes.Directory.ToString()] - props[FileAttributes.ReparsePoint.ToString()]);
         props.Add(propTotal, total);
         props.Add(propSize, size);
         props.Add(propSizeCompressed, sizeCompressed);

         return props;
      }

      #endregion // GetPropertiesInternal

      #endregion // Unified Internals

      #endregion // AlphaFS
   }
}