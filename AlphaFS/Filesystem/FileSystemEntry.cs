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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Class that retrieves file system entries (i.e. files and directories) using Win32 API FindFirst()/FindNext().</summary>
   public sealed class FileSystemEntry
   {
      #region Private Members

      private NativeMethods.FindExAdditionalFlags _findExAdditionalFlags = NativeMethods.FindExAdditionalFlags.None;
      private NativeMethods.FindExInfoLevels _findExInfoLevels = NativeMethods.FindExInfoLevels.Standard;
      private string _inputPath;
      private SearchOption _searchOption;
      private string _searchPattern;

      #region EnumerationFinished

      private bool EnumerationFinished()
      {
         int lastError = Marshal.GetLastWin32Error();
         switch ((uint)lastError)
         {
            case Win32Errors.ERROR_FILE_NOT_FOUND:
            case Win32Errors.ERROR_PATH_NOT_FOUND:
            case Win32Errors.ERROR_NO_MORE_FILES:
               return true;

            default:
               if (!ContinueOnAccessError)
                  NativeError.ThrowException(lastError, InputPath);

               return false;
         }
      }

      #endregion // EnumerationFinished

      #endregion // Private Members

      #region Properties

      #region BasicSearch

      /// <summary>Gets or sets a value indicating which <see cref="NativeMethods.FindExInfoLevels"/> to use.</summary>
      /// <value>If set to <c>true</c>, uses <see cref="NativeMethods.FindExInfoLevels.Basic"/>, otherwise uses <see cref="NativeMethods.FindExInfoLevels.Standard"/></value>
      public bool BasicSearch
      {
         get { return _findExInfoLevels == NativeMethods.FindExInfoLevels.Basic; }

         set
         {
            _findExInfoLevels = value
                                   ? NativeMethods.FindExInfoLevels.Basic
                                   : NativeMethods.FindExInfoLevels.Standard;
         }
      }

      #endregion // BasicSearch

      #region ContinueOnAccessError

      /// <summary>Gets or sets the ability to skip on access errors.</summary>
      /// <value>If set to <c>true</c> skip on access errors resulted from ACLs protected directories or non-accessible reparse points, otherwise an <see cref="System.UnauthorizedAccessException"/> is thrown.</value>
      public bool ContinueOnAccessError { get; set; }

      #endregion // ContinueOnAccessError

      #region GetFSOType

      /// <summary>Gets the file system object type.</summary>
      /// <value>
      /// null = return directories and files.
      /// true = return only directories.
      /// false = return only files.
      /// </value>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fso")]
      public bool? GetFsoType { get; set; }

      #endregion // GetFSOType

      #region InputPath

      /// <summary>Gets or sets the path to the folder.</summary>
      /// <value>The path to the folder.</value>
      public string InputPath
      {
         get { return _inputPath; }

         set
         {
            if (string.IsNullOrEmpty(value))
               throw new ArgumentNullException(InputPath);

            Path.CheckInvalidPathChars(value);

            _inputPath = value;
         }
      }

      #endregion // InputPath

      #region LargeCache

      /// <summary>Gets or sets a value indicating which <see cref="NativeMethods.FindExAdditionalFlags"/> to use.</summary>
      /// <value>If set to <c>true</c>, uses <see cref="NativeMethods.FindExAdditionalFlags.LargeFetch"/>, otherwise uses <see cref="NativeMethods.FindExAdditionalFlags.None"/></value>
      public bool LargeCache
      {
         get { return _findExAdditionalFlags == NativeMethods.FindExAdditionalFlags.LargeFetch; }

         set
         {
            _findExAdditionalFlags = value
                                        ? NativeMethods.FindExAdditionalFlags.LargeFetch
                                        : NativeMethods.FindExAdditionalFlags.None;
         }
      }

      #endregion // Cache

      #region SearchOption

      /// <summary>Specifie whether the search operation should include only the current directory or should include all subdirectories.</summary>
      /// <value>One of the <see cref="SearchOption"/> enumeration values.</value>
      public SearchOption SearchOption
      {
         get
         {
            if (_searchOption != SearchOption.TopDirectoryOnly && _searchOption != SearchOption.AllDirectories)
               _searchOption = SearchOption.TopDirectoryOnly;

            return _searchOption;
         }

         set { _searchOption = value; }
      }

      #endregion // SearchOption

      #region SearchPattern

      /// <summary>Search for file system object-name using a pattern.</summary>
      /// <value>A search string, the path which has wildcard characters, for example, an asterisk (<see cref="Path.WildcardStarMatchAll"/>) or a question mark (<see cref="Path.WildcardQuestion"/>).</value>
      public string SearchPattern
      {
         get
         {
            if (string.IsNullOrEmpty(_searchPattern))
               _searchPattern = Path.WildcardStarMatchAll;

            return _searchPattern;
         }

         set
         {
            _searchPattern = !string.IsNullOrEmpty(value)
                                ? value.Trim().Replace(".", @"\.").Replace("*", @".*").Replace("?", @".?")
                                : Path.WildcardStarMatchAll;
         }
      }

      #endregion // SearchPattern

      #region Transaction

      /// <summary>Get or sets the KernelTransaction instance.</summary>
      /// <value>The transaction.</value>
      public KernelTransaction Transaction { get; set; }

      #endregion // Transaction

      #endregion // Properties

      #region Methods

      #region Enumerate

      /// <summary>Get an enumerator that returns all of the file system objects that match the wildcards that are in any of the directories to be searched.</summary>
      /// <returns>An <see cref="IEnumerable{FileSystemEntryInfo}"/> instance.</returns>
      public IEnumerable<FileSystemEntryInfo> Enumerate()
      {
         NativeMethods.Win32FindData win32FindData = new NativeMethods.Win32FindData();

         Stack<string> dirs = new Stack<string>();

         Regex nameFilter = (SearchPattern == Path.WildcardStarMatchAll)
                               ? null
                               : new Regex("^" + SearchPattern + "$", RegexOptions.IgnoreCase);

         dirs.Push(InputPath);

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
         while (dirs.Count != 0)
         {
            string path = dirs.Pop();
            string pathLp = Path.Combine(path, Path.WildcardStarMatchAll);

            // In the ANSI version of this function, the name is limited to MAX_PATH characters.
            // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
            // 2013-01-13: MSDN confirms LongPath usage.
            pathLp = Path.PrefixLongPath(pathLp);

            // A trailing backslash is not allowed and will be removed.
            pathLp = Path.DirectorySeparatorRemove(pathLp, false);

            using (SafeFindFileHandle handle = Transaction == null
                                               ? NativeMethods.FindFirstFileEx(pathLp, _findExInfoLevels, ref win32FindData, NativeMethods.FindExSearchOps.SearchNameMatch, IntPtr.Zero, _findExAdditionalFlags)
                                               : NativeMethods.FindFirstFileTransacted(pathLp, _findExInfoLevels, ref win32FindData, NativeMethods.FindExSearchOps.SearchNameMatch, IntPtr.Zero, _findExAdditionalFlags, Transaction.SafeHandle))
            {
               if (!NativeMethods.IsValidHandle(handle, false))
               {
                  if (!EnumerationFinished())
                     continue;

                  // Stop and return from the method.
                  yield break;
               }

               do
               {
                  string fileName = win32FindData.FileName;

                  // Skip entries: null, string.Empty, "." and ".."
                  if (!(string.IsNullOrEmpty(fileName) ||
                        fileName.Equals(Path.CurrentDirectoryPrefix, StringComparison.OrdinalIgnoreCase) ||
                        fileName.Equals(Path.ParentDirectoryPrefix, StringComparison.OrdinalIgnoreCase)))
                  {
                     bool yieldItem = nameFilter != null && nameFilter.IsMatch(fileName);

                     // Populate FullPath property with current full path.
                     string fullPath = Path.Combine(path, fileName);
                     FileSystemEntryInfo fsei = new FileSystemEntryInfo(win32FindData) { FullPath = fullPath };

                     // If object is a Directory, add it to the queue for later traversal.
                     if (fsei.IsDirectory && SearchOption == SearchOption.AllDirectories)
                        dirs.Push(fullPath);

                     // Make sure the requested file system object type is returned.
                     // null = return directories and files.
                     // true = return only directories.
                     // false = return only files.
                     if (nameFilter == null || yieldItem)
                        switch (GetFsoType)
                        {
                           case null:
                              yield return fsei;
                              break;

                           case true:
                              if (fsei.IsDirectory)
                                 yield return fsei;
                              break;

                           case false:
                              if (fsei.IsFile)
                                 yield return fsei;
                              break;
                        }
                  }

                  if (NativeMethods.FindNextFile(handle, ref win32FindData))
                     continue; // As long as an entry has been read.

                  // End of directory.
                  if (EnumerationFinished())
                     break;

               } while (true);
            }
         }
      }

      #endregion // Enumerate

      #endregion // Methods
   }
}