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
using Alphaleonis.Win32.Network;
using Alphaleonis.Win32.Security;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Performs operations on String instances that contain file or directory path information. These operations are performed in a cross-platform manner.</summary>
   public static class Path
   {
      #region .NET

      #region Fields

      /// <summary>AltDirectorySeparatorChar = '/' Provides a platform-specific alternate character used to separate directory levels in a path string that reflects a hierarchical file system organization.</summary>
      /// <remarks>Equivalent of <see cref="System.IO.Path.AltDirectorySeparatorChar"/></remarks>
      public static readonly char AltDirectorySeparatorChar = System.IO.Path.AltDirectorySeparatorChar;

      /// <summary>DirectorySeparatorChar = '\' Provides a platform-specific character used to separate directory levels in a path string that reflects a hierarchical file system organization.</summary>
      /// <remarks>Equivalent of <see cref="System.IO.Path.DirectorySeparatorChar"/></remarks>
      public static readonly char DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar;

      /// <summary>PathSeparator = ';' A platform-specific separator character used to separate path strings in environment variables.</summary>
      /// <remarks>Equivalent of <see cref="System.IO.Path.PathSeparator"/></remarks>
      public static readonly char PathSeparator = System.IO.Path.PathSeparator;

      /// <summary>VolumeSeparatorChar = ':' Provides a platform-specific Volume Separator character.</summary>
      /// <remarks>Equivalent of <see cref="System.IO.Path.VolumeSeparatorChar"/></remarks>
      public static readonly char VolumeSeparatorChar = System.IO.Path.VolumeSeparatorChar;

      #region AlphaFS

      /// <summary>CurrentDirectoryPrefix = '.' Provides a current directory string.</summary>
      public const string CurrentDirectoryPrefix = ".";

      /// <summary>CurrentDirectoryPrefix = '.' Provides a current directory character.</summary>
      public const char CurrentDirectoryPrefixChar = '.';
      
      /// <summary>ExtensionSeparatorChar = '.' Provides an Extension Separator character.</summary>
      public const char ExtensionSeparatorChar = '.';

      /// <summary>ParentDirectoryPrefix = ".." Provides a parent directory string.</summary>
      public const string ParentDirectoryPrefix = "..";

      /// <summary>StringTerminatorChar = '\0' String Terminator Suffix.</summary>
      internal const char StringTerminatorChar = '\0';

      /// <summary>WildcardStarMatchAll = "*" Provides a match-all-items string.</summary>
      public const string WildcardStarMatchAll = "*";

      /// <summary>WildcardStarMatchAll = '*' Provides a match-all-items character.</summary>
      public const char WildcardStarMatchAllChar = '*';

      /// <summary>WildcardQuestion = "?" Provides a replace-item string.</summary>
      public const string WildcardQuestion = "?";

      /// <summary>WildcardQuestion = "?" Provides a replace-item string.</summary>
      public const char WildcardQuestionChar = '?';


      /// <summary>UncPrefix = "\\" Provides standard Windows Path UNC prefix.</summary>
      public static readonly string UncPrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}", DirectorySeparatorChar, DirectorySeparatorChar);


      /// <summary>LongPathPrefix = "\\?\" Provides standard Windows Long Path prefix.</summary>
      public static readonly string LongPathPrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", UncPrefix, WildcardQuestion, DirectorySeparatorChar);

      /// <summary>LongPathUncPrefix = "\\?\UNC\" Provides standard Windows Long Path UNC prefix.</summary>
      public static readonly string LongPathUncPrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", LongPathPrefix, "UNC", DirectorySeparatorChar);


      /// <summary>GlobalRootPrefix = "\\?\GLOBALROOT\" Provides standard Windows Volume prefix.</summary>
      public static readonly string GlobalRootPrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", LongPathPrefix, "GLOBALROOT", DirectorySeparatorChar);


      /// <summary>MsDosNamespacePrefix = "\\\\.\\" Provides standard Win32 Namespace prefix.</summary>
      [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ms")]
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ms")]
      public static readonly string MsDosNamespacePrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}.{4}{5}", DirectorySeparatorChar, DirectorySeparatorChar, DirectorySeparatorChar, DirectorySeparatorChar, DirectorySeparatorChar, DirectorySeparatorChar);


      /// <summary>SubstitutePrefix = "\??\" Provides a SUBST.EXE Path prefix to a Logical Drive.</summary>
      public static readonly string SubstitutePrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}", DirectorySeparatorChar, WildcardQuestion, WildcardQuestion, DirectorySeparatorChar);


      /// <summary>VolumePrefix = "\\?\Volume" Provides standard Windows Volume prefix.</summary>
      public static readonly string VolumePrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}", LongPathPrefix, "Volume");

      /// <summary>DevicePrefix = "\Device\" Provides standard Windows Device prefix.</summary>
      public static readonly string DevicePrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", DirectorySeparatorChar, "Device", DirectorySeparatorChar);

      /// <summary>DosDeviceLanmanPrefix = "\Device\LanmanRedirector\" Provides a MS-Dos Lanman Redirector Path UNC prefix to a network share.</summary>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lanman")]
      public static readonly string DosDeviceLanmanPrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", DevicePrefix, "LanmanRedirector", DirectorySeparatorChar);

      /// <summary>DosDeviceMupPrefix = "\Device\Mup\" Provides a MS-Dos Mup Redirector Path UNC prefix to a network share.</summary>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mup")]
      public static readonly string DosDeviceMupPrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", DevicePrefix, "Mup", DirectorySeparatorChar);

      /// <summary>DosDeviceUncPrefix = "\??\UNC\" Provides a SUBST.EXE Path UNC prefix to a network share.</summary>
      public static readonly string DosDeviceUncPrefix = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", SubstitutePrefix, "UNC", DirectorySeparatorChar);

      #endregion // AlphaFS

      #endregion // Fields


      #region ChangeExtension

      /// <summary>Changes the extension of a path string.</summary>
      /// <param name="path">The path information to modify. The path cannot contain any of the characters defined in <see cref="GetInvalidPathChars"/>.</param>
      /// <param name="extension">The new extension (with or without a leading period). Specify <see langword="null"/> to remove an existing extension from path.</param>
      /// <exception cref="ArgumentException">Path contains one or more of the invalid characters defined in GetInvalidPathChars.</exception>
      /// <returns>
      /// The modified path information.
      /// On Windows-based desktop platforms, if <paramref name="path"/> is <see langword="null"/> or an empty string (""), the path information is returned unmodified.
      /// If <paramref name="extension"/> is <see langword="null"/>, the returned string contains the specified path with its extension removed.
      /// If <paramref name="path"/> has no extension, and <paramref name="extension"/> is not <see langword="null"/>, the returned path string contains <paramref name="extension"/> appended to the end of path.
      /// </returns>
      public static string ChangeExtension(string path, string extension)
      {
         return System.IO.Path.ChangeExtension(path, extension);
      }

      #endregion // ChangeExtension

      #region Combine

      /// <summary>Combines an array of strings into a path.</summary>
      /// <param name="paths">An array of parts of the path.</param>
      /// <exception cref="ArgumentException">One of the strings in the array contains one or more of the invalid characters defined in <see cref="GetInvalidPathChars"/>.</exception>
      /// <exception cref="ArgumentNullException">One of the strings in the array is <see langword="null"/>.</exception>
      /// <returns>The combined paths.</returns>
      public static string Combine(params string[] paths)
      {
         if (paths == null)
            throw new ArgumentNullException("paths");

         int finalSize = 0;
         int firstComponent = 0;

         // Calculates how large a buffer to allocate and do some precondition checks on the paths passed in.  
         for (int i = 0, l = paths.Length; i < l; i++)
         {
            if (paths[i] == null)
               throw new ArgumentNullException("paths");

            if (paths[i].Length == 0)
               continue;

            if (IsPathRooted(paths[i]))
            {
               firstComponent = i;
               finalSize = paths[i].Length;
            }
            else
               finalSize += paths[i].Length;

            char ch = paths[i][paths[i].Length - 1];

            if (!IsDVsc(ch, null))
               finalSize++;
         }

         // The actual path combination.
         StringBuilder finalPath = new StringBuilder(finalSize);

         for (int i = firstComponent, l = paths.Length; i < l; i++)
         {
            if (paths[i].Length == 0)
               continue;

            if (finalPath.Length == 0)
               finalPath.Append(paths[i]);

            else
            {
               char ch = finalPath[finalPath.Length - 1];

               if (!IsDVsc(ch, null))
                  finalPath.Append(DirectorySeparatorChar);

               finalPath.Append(paths[i]);
            }
         }

         return finalPath.ToString();
      }

      #endregion // Combine

      #region GetDirectoryName

      /// <summary>Returns the directory information for the specified path string.</summary>
      /// <param name="path">The path of a file or directory. </param>
      /// <returns>Directory information for <paramref name="path"/>, or <see langref="null"/> if <paramref name="path"/> denotes a root directory or is <see langref="null"/>. Returns <see langref="string.Empty"/> if <paramref name="path"/> does not contain directory information.</returns>
      public static string GetDirectoryName(string path)
      {
         return path == null ? null : new PathInfo(path, false).DirectoryName;
      }

      #endregion // GetDirectoryName

      #region GetExtension

      /// <summary>Returns the extension of the specified path string.</summary>
      /// <param name="path">The path string from which to get the extension.</param>
      /// <returns>The extension of the specified <paramref name="path"/>, or an empty string if the path contains no extension. If the path is <see langword="null"/>, this method returns <see langword="null"/>.</returns>
      public static string GetExtension(string path)
      {
         return path == null ? null : new PathInfo(path, false).Extension;
      }

      #endregion // GetExtension

      #region GetFileName

      /// <summary>Returns the file name and extension of the specified path string.</summary>
      /// <param name="path">The path string from which to obtain the file name and extension.</param>
      /// <returns>
      /// A string consisting of the characters after the last directory character in path.
      /// If the last character of <paramref name="path"/> is a <see cref="Path.DirectorySeparatorChar"/> or <see cref="Path.VolumeSeparatorChar"/>, this method returns <see langword="string.Empty"/>.
      /// If <paramref name="path"/> is a <see langword="null"/> reference, this method returns <see langword="null"/>.
      /// </returns>
      /// <remarks>If the last character of <paramref name="path"/> is a directory- or volume-separator character, this method returns <see cref="string.Empty"/>.</remarks>
      public static string GetFileName(string path)
      {
         return path == null ? null : new PathInfo(path, false).FileName;
      }

      #endregion // GetFileName

      #region GetFileNameWithoutExtension

      /// <summary>Returns the file name of the specified path string without the extension.</summary>
      /// <param name="path">The path of the file. </param>
      /// <returns>The string returned by GetFileName, minus the last period (.) and all characters following it.</returns>
      public static string GetFileNameWithoutExtension(string path)
      {
         return new PathInfo(path, false).FileNameWithoutExtension;
      }

      #endregion // GetFileNameWithoutExtension

      #region GetFullPath

      // The AlphaFS implementation replaces the .NET implementation.

      ///// <summary>Returns the absolute path for the current directory.</summary>
      ///// <returns>A string containing the fully qualified location of path, such as "C:\MyFile.txt".</returns>
      //public static string GetFullPath()
      //{
      //   return new PathInfo(string.Empty, false).GetFullPath();
      //}

      ///// <summary>Returns the absolute path for the specified path string.</summary>
      ///// <param name="path">The file or directory for which to obtain absolute path information.</param>
      ///// <returns>A string containing the fully qualified location of path, such as "C:\MyFile.txt".</returns>
      //public static string GetFullPath(string path)
      //{
      //   return new PathInfo(!string.IsNullOrEmpty(path) ? path : string.Empty, false).GetFullPath();
      //}

      #endregion // GetFullPath

      #region GetInvalidFileNameChars

      /// <summary>Gets an array containing the characters that are not allowed in file names.</summary>
      /// <returns>An array containing the characters that are not allowed in file names.</returns>
      public static char[] GetInvalidFileNameChars()
      {
         return System.IO.Path.GetInvalidFileNameChars();
      }

      #endregion // GetInvalidFileNameChars

      #region GetInvalidPathChars

      /// <summary>Gets an array containing the characters that are not allowed in path names.</summary>
      /// <returns>An array containing the characters that are not allowed in path names.</returns>
      /// <remarks>This method calls <see cref="System.IO.Path.GetInvalidPathChars"/></remarks>
      public static char[] GetInvalidPathChars()
      {
         return System.IO.Path.GetInvalidPathChars();
      }

      #endregion // GetInvalidPathChars

      #region GetPathRoot

      /// <summary>Gets the root directory information of the specified path.</summary>
      /// <param name="path">The path from which to obtain root directory information.</param>
      /// <returns>The root directory of path, such as "C:\", or <see langword="null"/> if path is <see langword="null"/>, or an empty string if path does not contain root directory information.</returns>
      public static string GetPathRoot(string path)
      {
         // .NET doesn't handle paths well that start with: \\?\UNC\
         //return System.IO.Path.GetPathRoot(path);

         return path == null ? null : new PathInfo(path, false).Root;
      }

      #endregion // GetPathRoot

      #region GetRandomFileName

      /// <summary>Returns a random folder name or file name.</summary>
      /// <returns>A random folder name or file name.</returns>
      /// <remarks>This method calls <see cref="System.IO.Path.GetRandomFileName"/></remarks>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public static string GetRandomFileName()
      {
         return System.IO.Path.GetRandomFileName();
      }

      #endregion // GetRandomFileName

      #region GetTempFileName

      /// <summary>Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.</summary>
      /// <returns>A <see cref="string"/> containing the full path of the temporary file.</returns>
      /// <remarks>This method calls <see cref="System.IO.Path.GetTempFileName"/></remarks>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public static string GetTempFileName()
      {
         return System.IO.Path.GetTempFileName();
      }

      #endregion // GetTempFileName

      #region GetTempPath

      /// <summary>Returns the path of the current user's temporary folder.</summary>
      /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
      /// <returns>
      /// A <see cref="string"/> containing the path information of a temporary directory.
      /// If <see param="combineFolder"/> is specified,  and <see cref="GetLongPath"/>
      /// are applied, path validity is then checked through the <see cref="PathInfo"/> class.
      /// </returns>
      /// <remarks>This method calls <see cref="System.IO.Path.GetTempPath"/></remarks>
      public static string GetTempPath()
      {
         return System.IO.Path.GetTempPath();
      }

      #region AlphaFS

      /// <summary>Returns the path of the current user's temporary folder.</summary>
      /// <param name="combinePath">A <see cref="string"/> foldername to append to the TempPath.</param>
      /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
      /// <exception cref="ArgumentException">Invalid Path Chars.</exception>
      /// <returns>A <see cref="string"/> containing the path information of a temporary directory.</returns>
      /// <remarks>
      /// The GetTempPath function checks for the existence of environment variables
      /// in the following order and uses the first path found:
      ///     The path specified by the TMP environment variable.
      ///     The path specified by the TEMP environment variable.
      ///     The path specified by the USERPROFILE environment variable.
      ///     The Windows directory.
      ///
      /// Note that the function does not verify that the path exists, nor does it test to see if the current
      /// process has any kind of access rights to the path. The GetTempPath function returns the properly
      /// formatted string that specifies the fully-qualified path based on the environment variable search
      /// order as previously specified. The application should verify the existence of the path and adequate
      /// access rights to the path prior to any use for file I/O operations.
      ///
      /// Symbolic link behavior; if the path points to a symbolic link, the temp path name maintains any symbolic links.
      /// </remarks>
      public static string GetTempPath(string combinePath)
      {
         CheckInvalidPathChars(combinePath);

         string tempPath = GetTempPath();

         return !string.IsNullOrEmpty(combinePath) ? Combine(tempPath, combinePath) : tempPath;
      }

      #endregion // AlphaFS

      #endregion // GetTempPath

      #region HasExtension

      /// <summary>Determines whether a path includes a file name extension.</summary>
      /// <param name="path">The path to search for an extension.</param>
      /// <returns><c>true</c> if the specified path has extension, <c>false</c> otherwise.</returns>
      public static bool HasExtension(string path)
      {
         return path != null && new PathInfo(path, false).HasExtension;
      }

      #endregion // HasExtension

      #region IsPathRooted

      /// <summary>Gets a value indicating whether the specified path string contains absolute or relative path information.</summary>
      /// <param name="path">The path to test.</param>
      /// <returns><c>true</c> if <paramref name="path"/> contains an absolute path, <c>false</c> otherwise.</returns>
      /// <remarks>The IsPathRooted method returns <c>true</c> if the first character is a directory separator character such as <see cref="DirectorySeparatorChar"/>, or if the path starts with a drive letter and colon (<see cref="VolumeSeparatorChar"/>). For example, it returns true for path strings such as "\\MyDir\\MyFile.txt", "C:\\MyDir", or "C:MyDir". It returns false for path strings such as "MyDir".</remarks>
      /// <remarks>This method does not verify that the path or file name exists.</remarks>
      public static bool IsPathRooted(string path)
      {
         // A bit overkill.
         //return path != null && new PathInfo(path, false).IsRooted;

         // Same as: PathInfo.IsRooted;
         return path != null && (path.Length >= 1 && IsDVsc(path[0], false) || (path.Length >= 2 && IsDVsc(path[1], true)));
      }

      #endregion // IsPathRooted

      #endregion // .NET

      #region AlphaFS

      #region DirectorySeparatorAdd

      /// <summary>Adds a <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> character to the string.</summary>
      /// <param name="path">A text string to which the <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> is to be added.</param>
      /// <returns>A text string with the <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> character suffixed. The function returns <see langword="null"/> when <see param="path"/> is <see langword="null"/>.</returns>
      public static string DirectorySeparatorAdd(string path)
      {
         return DirectorySeparatorAdd(path, false);
      }

      /// <summary>Adds a <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> character to the string.</summary>
      /// <param name="path">A text string to which the <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> is to be added.</param>
      /// <param name="addAlternateSeparator">if true the <see cref="AltDirectorySeparatorChar"/> character will be added instead.</param>
      /// <returns>A text string with the <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> character suffixed. The function returns <see langword="null"/> when <see param="path"/> is <see langword="null"/>.</returns>
      public static string DirectorySeparatorAdd(string path, bool addAlternateSeparator)
      {
         if (path == null)
            return null;

         return addAlternateSeparator
                    ? ((!path.EndsWith(AltDirectorySeparatorChar.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase))
                           ? path + AltDirectorySeparatorChar
                           : path)
                    : ((!path.EndsWith(DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase))
                           ? path + DirectorySeparatorChar
                           : path);
      }

      #endregion // DirectorySeparatorAdd

      #region DirectorySeparatorRemove

      /// <summary>Removes the <see cref="DirectorySeparatorChar"/> character from the string.</summary>
      /// <param name="path">A text string from which the <see cref="DirectorySeparatorChar"/> is to be removed.</param>
      /// <returns>A text string where the suffixed <see cref="DirectorySeparatorChar"/> has been removed. The function returns <see langword="null"/> when <see param="path"/> is <see langword="null"/>.</returns>
      public static string DirectorySeparatorRemove(string path)
      {
         return DirectorySeparatorRemove(path, false);
      }

      /// <summary>Removes the <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> character from the string.</summary>
      /// <param name="path">A text string from which the <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> is to be removed.</param>
      /// <param name="removeAlternateSeparator">If true the <see cref="AltDirectorySeparatorChar"/> character will be removed instead.</param>
      /// <returns>A text string where the suffixed <see cref="DirectorySeparatorChar"/> or <see cref="AltDirectorySeparatorChar"/> character has been removed. The function returns <see langword="null"/> when <see param="path"/> is <see langword="null"/>.</returns>
      public static string DirectorySeparatorRemove(string path, bool removeAlternateSeparator)
      {
         return path == null ? null : path.TrimEnd((removeAlternateSeparator) ? AltDirectorySeparatorChar : DirectorySeparatorChar);
      }

      #endregion // DirectorySeparatorRemove
      
      #region GetDirectoryNameWithoutRoot

      /// <summary>Returns the directory information for the specified path string without the root information.</summary>
      /// <param name="path">The path.</param>
      /// <returns>The <paramref name="path"/>without the file name part and without the root information (if any), or <see langref="null"/> if <paramref name="path"/> is <see langref="null"/>.</returns>
      public static string GetDirectoryNameWithoutRoot(string path)
      {
         return path == null ? null : new PathInfo(path, false).DirectoryNameWithoutRoot;
      }

      #endregion // GetDirectoryNameWithoutRoot

      #region GetFinalPathNameByHandle

      /// <summary>Retrieves the final path for the specified file, formatted as <see cref="FinalPathFormats"/>.</summary>
      /// <param name="stream">Then handle to a <see cref="FileStream"/> instance.</param>
      /// <returns>Returns the final path as a <c>string</c>.</returns>
      /// <remarks>
      /// A final path is the path that is returned when a path is fully resolved.
      /// For example, for a symbolic link named "C:\tmp\mydir" that points to "D:\yourdir", the final path would be "D:\yourdir".
      /// The string that is returned by this function uses the <see cref="LongPathPrefix"/> syntax.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
      public static string GetFinalPathNameByHandle(FileStream stream)
      {
         return (stream != null && stream.SafeFileHandle != null)
                   ? GetFinalPathNameByHandleInternal(stream.SafeFileHandle, FinalPathFormats.FileNameNormalized)
                   : string.Empty;
      }

      /// <summary>Retrieves the final path for the specified file, formatted as <see cref="FinalPathFormats"/>.</summary>
      /// <param name="stream">Then handle to a <see cref="FileStream"/> instance.</param>
      /// <param name="finalPath">The final path, formatted as <see cref="FinalPathFormats"/></param>
      /// <returns>Returns the final path as a <c>string</c>.</returns>
      /// <remarks>
      /// A final path is the path that is returned when a path is fully resolved.
      /// For example, for a symbolic link named "C:\tmp\mydir" that points to "D:\yourdir", the final path would be "D:\yourdir".
      /// The string that is returned by this function uses the <see cref="LongPathPrefix"/> syntax.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      public static string GetFinalPathNameByHandle(FileStream stream, FinalPathFormats finalPath)
      {
         return (stream != null && stream.SafeFileHandle != null)
                   ? GetFinalPathNameByHandleInternal(stream.SafeFileHandle, finalPath)
                   : string.Empty;
      }

      /// <summary>Retrieves the final path for the specified file, formatted as <see cref="FinalPathFormats"/>.</summary>
      /// <param name="handle">Then handle to a <see cref="SafeFileHandle"/> instance.</param>
      /// <returns>Returns the final path as a <c>string</c>.</returns>
      /// <remarks>
      /// A final path is the path that is returned when a path is fully resolved.
      /// For example, for a symbolic link named "C:\tmp\mydir" that points to "D:\yourdir", the final path would be "D:\yourdir".
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      public static string GetFinalPathNameByHandle(SafeFileHandle handle)
      {
         return GetFinalPathNameByHandleInternal(handle, FinalPathFormats.FileNameNormalized);
      }

      #region GetFinalPathNameByHandleInternal

      /// <summary>Retrieves the final path for the specified file, formatted as <see cref="FinalPathFormats"/>.</summary>
      /// <param name="handle">Then handle to a <see cref="SafeFileHandle"/> instance.</param>
      /// <param name="finalPath">The final path, formatted as <see cref="FinalPathFormats"/></param>
      /// <returns>Returns the final path as a <c>string</c>.</returns>
      /// <remarks>
      /// A final path is the path that is returned when a path is fully resolved.
      /// For example, for a symbolic link named "C:\tmp\mydir" that points to "D:\yourdir", the final path would be "D:\yourdir".
      /// The string that is returned by this function uses the <see cref="LongPathPrefix"/> syntax.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      internal static string GetFinalPathNameByHandleInternal(SafeFileHandle handle, FinalPathFormats finalPath)
      {
         NativeMethods.IsValidHandle(handle);

         if (OperatingSystemInfo.IsAtLeast(OsVersionName.WindowsVista))
         {
            StringBuilder sb = new StringBuilder(NativeMethods.MaxPathUnicode);

            if (NativeMethods.GetFinalPathNameByHandle(handle, sb, (uint)sb.Capacity, finalPath) == Win32Errors.ERROR_SUCCESS)
               NativeError.ThrowException();

            return sb.ToString();
         }

         // Windows XP / Windows Server 2003
         return OperatingSystemInfo.IsAtLeast(OsVersionName.WindowsXp) ? GetFinalPathNameByHandleX3Internal(handle, finalPath) : string.Empty;
      }

      #endregion // GetFinalPathNameByHandleInternal

      #region GetFinalPathNameByHandleX3

      internal static string GetFinalPathNameByHandleX3(FileStream stream)
      {
         return (stream != null && stream.SafeFileHandle != null)
                   ? GetFinalPathNameByHandleX3Internal(stream.SafeFileHandle, FinalPathFormats.FileNameNormalized)
                   : string.Empty;
      }

      internal static string GetFinalPathNameByHandleX3(FileStream stream, FinalPathFormats finalPath)
      {
         return (stream != null && stream.SafeFileHandle != null)
                   ? GetFinalPathNameByHandleX3Internal(stream.SafeFileHandle, finalPath)
                   : string.Empty;
      }

      internal static string GetFinalPathNameByHandleX3(SafeFileHandle handle)
      {
         return GetFinalPathNameByHandleX3Internal(handle, FinalPathFormats.FileNameNormalized);
      }

      [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Alphaleonis.Win32.Filesystem.NativeMethods.GetMappedFileName(System.IntPtr,Alphaleonis.Win32.Security.SafeLocalMemoryBufferHandle,System.Text.StringBuilder,System.UInt32)")]
      
      [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
      private static string GetFinalPathNameByHandleX3Internal(SafeFileHandle handle, FinalPathFormats finalPath)
      {
         NativeMethods.IsValidHandle(handle);

         // Be careful when using GetFileSizeEx to check the size of hFile handle of an unknown "File" type object.
         // This is more towards returning a filename from a file handle. If the handle is a named pipe handle it seems to hang the thread.
         // Check for: FileTypes.DiskFile

         // Can't map a 0 byte file.
         long fileSizeHi;
         if (!NativeMethods.GetFileSizeEx(handle, out fileSizeHi))
            return string.Empty;

         StringBuilder sb = new StringBuilder(NativeMethods.MaxPathUnicode);

         // PAGE_READONLY
         // Allows views to be mapped for read-only or copy-on-write access. An attempt to write to a specific region results in an access violation.
         // The file handle that the hFile parameter specifies must be created with the GENERIC_READ access right.
         // PageReadOnly = 0x02,
         using (SafeFileHandle hFile = NativeMethods.CreateFileMapping(handle, null, 2, 0, 1, null))
         {
            NativeMethods.IsValidHandle(hFile);

            // FILE_MAP_READ
            // Read = 4
            using (SafeLocalMemoryBufferHandle pMem = NativeMethods.MapViewOfFile(hFile, 4, 0, 0, (UIntPtr) 1))
            {
               if (NativeMethods.IsValidHandle(pMem))
                  NativeMethods.GetMappedFileName(Process.GetCurrentProcess().Handle, pMem, sb, (uint) sb.Capacity);

               NativeMethods.UnmapViewOfFile(pMem);
            }
         }


         // Default output from GetMappedFileName(): "\Device\HarddiskVolumeX\path\filename.ext"

         string dosDevice = sb.Length > 0 ? sb.ToString() : string.Empty;

         // Select output format.
         switch (finalPath)
         {
            // As-is.
            case FinalPathFormats.VolumeNameNT:
               return dosDevice;

            // To: "\path\filename.ext"
            case FinalPathFormats.VolumeNameNone:
               return DosDeviceToDosPath(dosDevice, string.Empty);

            // To: "\\?\Volume{GUID}\path\filename.ext"
            case FinalPathFormats.VolumeNameGuid:
               string dosPath = DosDeviceToDosPath(dosDevice, null);
               if (!string.IsNullOrEmpty(dosPath))
               {
                  PathInfo pathInfo = new PathInfo(dosPath, false);

                  string path = pathInfo.SuffixedDirectoryNameWithoutRoot;
                  string driveLetter = DirectorySeparatorRemove(pathInfo.Root, false);
                  string file = pathInfo.FileName;

                  if (!string.IsNullOrEmpty(file))
                  {
                     // Get Logical Drives from: Win32 Api, .IsReady, remove backslash.
                     foreach (string drive in Directory.GetLogicalDrives(false, true, true).Where(drive => driveLetter.Equals(drive, StringComparison.CurrentCultureIgnoreCase)))
                     {
                        string unique = Volume.GetUniqueVolumeNameForPath(drive);
                        return Combine(unique, path, file);
                     }
                  }
               }

               break;
         }

         // To: "\\?\C:\path\filename.ext"
         return string.IsNullOrEmpty(dosDevice)
                    ? string.Empty
                    : LongPathPrefix + DosDeviceToDosPath(dosDevice, null);
      }

      #endregion // GetFinalPathNameByHandleX3

      #endregion // GetFinalPathNameByHandle

      #region GetFullPath

      /// <summary>Returns the absolute path for the specified path string.</summary>
      /// <param name="path">The file or directory for which to obtain absolute path information.</param>
      /// <returns>A <see langref="String"/> string containing the fully qualified location of path, such as "C:\MyFile.txt".</returns>
      /// <remarks>This parameter can be a short (the 8.3 form) or long file name. This string can also be a share or volume name.</remarks>
      /// <remarks>The GetFullPathName function is not recommended for multithreaded applications or shared library code.</remarks>
      public static string GetFullPath(string path)
      {
         return GetFullPath(null, path);
      }

      /// <summary>Returns the absolute path for the specified path string.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain absolute path information.</param>
      /// <returns>A <see langref="String"/> string containing the fully qualified location of path, such as "C:\MyFile.txt".</returns>
      /// <remarks>This parameter can be a short (the 8.3 form) or long file name. This string can also be a share or volume name.</remarks>
      /// <remarks>The GetFullPathName function is not recommended for multithreaded applications or shared library code.</remarks>
      public static string GetFullPath(KernelTransaction transaction, string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-04-15: MSDN confirms LongPath usage.

         // Doesn't work reliable with relative paths.
         //string pathLp = IsPathRooted(path) ? PrefixLongPath(path) : path;
         string pathLp = path;


         // Start with a large buffer to prevent multiple calls.
         uint bufferSize = NativeMethods.MaxPathUnicode;
         StringBuilder buffer;
         bool isDone = false;

         do
         {
            buffer = new StringBuilder((int)bufferSize);
            uint returnLength = transaction == null
                                   ? NativeMethods.GetFullPathName(pathLp, bufferSize, buffer, IntPtr.Zero)
                                   : NativeMethods.GetFullPathNameTransacted(pathLp, bufferSize, buffer, IntPtr.Zero, transaction.SafeHandle);

            switch (returnLength)
            {
               case Win32Errors.ERROR_SUCCESS:
                  NativeError.ThrowException(path);
                  break;

               default:
                  isDone = bufferSize > returnLength;
                  if (!isDone)
                     bufferSize *= 2;
                  break;
            }
         } while (!isDone);

         return buffer.ToString();
      }

      #endregion // GetFullPath

      #region GetLongPath

      /// <summary>Retrieves the full long (or extended) unicode version of the specified <paramref name="path"/>.</summary>
      /// <remarks><para>This method takes care of different path conversions to be usable in Unicode variants of the Win32 functions (which are internally used throughout AlphaFS).</para>
      /// <para>Regular paths are changed like the following:
      ///		<list type="table">
      ///			<item>
      ///				<term><c>C:\Somewhere\Something.txt</c></term>
      ///				<description><c>\\?\C:\Somewhere\Something.txt</c></description>
      ///			</item>
      ///			<item>
      ///				<term><c>\\Somewhere\Something.txt</c></term>
      ///				<description><c>\\?\UNC\Somewhere\Something.txt</c></description>
      ///			</item>
      ///		</list>
      /// </para>
      /// <para>Already processed paths are preserved untouched so to avoid mistakes of double prefixing.</para>
      /// <para>
      ///		If the <paramref name="path"/> is not an absolute path, or is not rooted, the path of the
      ///		current directory (and drive) is combined with the specified <paramref name="path"/> to form
      ///		an absolute path.
      /// </para>
      /// </remarks>
      /// <param name="path">File or Folder name to sanitize and prefix with proper standard.</param>
      /// <returns>The full long (or extended) unicode version of the specified <paramref name="path"/>.</returns>
      /// <remarks>
      /// Method <see cref="GetLongPath"/> creates a <see cref="PathInfo"/> instance which may call <see cref="Directory.GetCurrentDirectory()"/>, yielding possible unexpected results.
      /// Method <see cref="PrefixLongPath"/> only prefixes a <see cref="LongPathPrefix"/> to the specified <paramref name="path"/>.
      /// </remarks>
      public static string GetLongPath(string path)
      {
         return new PathInfo(path, false).GetLongPath();
      }

      #endregion // GetLongPath

      #region GetLongFrom83Path

      /// <summary>Converts the specified existing path to its regular long form.</summary>
      /// <param name="path">An existing path to a folder or file.</param>
      /// <returns>A string containg the regular full path.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static string GetLongFrom83Path(string path)
      {
         return GetLongShort83PathInternal(path, false);
      }

      #endregion // GetLongFrom83Path

      #region GetMappedConnectionName

      /// <summary>Gets the connection name of the locally mapped drive.</summary>
      /// <param name="path">The local path with drive name.</param>
      /// <returns>A string which has the following format <c>\\servername\sharename</c>.</returns>
      /// <exception cref="PathTooLongException">When <paramref name="path"/> exceeds <see cref="NativeMethods.MaxPath"/></exception>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static string GetMappedConnectionName(string path)
      {
         return Host.GetRemoteNameInfoInternal(path).ConnectionName;
      }

      #endregion // GetMappedConnectionName

      #region GetMappedUncName

      /// <summary>Gets the network share name from the locally mapped path.</summary>
      /// <param name="path">The local path with drive name.</param>
      /// <returns>A string in which drive name being replaced with it's network share connection name.</returns>
      /// <exception cref="PathTooLongException">When <paramref name="path"/> exceeds <see cref="NativeMethods.MaxPath"/></exception>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static string GetMappedUncName(string path)
      {
         return Host.GetRemoteNameInfoInternal(path).UniversalName;
      }

      #endregion // GetMappedUncName

      #region GetRegularPath

      /// <summary>Gets the regular path from long prefixed one. i.e. \\?\C:\Temp\file.txt to C:\Temp\file.txt  \\?\UNC\Server\share\file.txt to \\Server\share\file.txt</summary>
      /// <param name="path">The path.</param>
      /// <returns>Regular form path string.</returns>
      /// <remarks>This method does not handle paths with volume names, eg. \\?\Volume{GUID}\Folder\file.txt </remarks>
      public static string GetRegularPath(string path)
      {
         if (path == null)
            return null;

         if (!path.StartsWith(LongPathPrefix, StringComparison.OrdinalIgnoreCase))
            return path;

         if (path.StartsWith(LongPathUncPrefix, StringComparison.OrdinalIgnoreCase))
            return UncPrefix + path.Substring(LongPathUncPrefix.Length);

         return path.Substring(LongPathPrefix.Length);
      }

      #endregion // GetRegularPath

      #region GetShort83Path

      /// <summary>Retrieves the short path form of the specified path.</summary>
      /// <param name="path">An existing path to a folder or file.</param>
      /// <returns>A string with a path that has the 8.3 path form.</returns>
      /// <remarks>Will fail on NTFS volumes with disabled 8.3 name generation.</remarks>
      /// <remarks>The path must actually exist to be able to get the short path name.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static string GetShort83Path(string path)
      {
         return GetLongShort83PathInternal(path, true);
      }

      #endregion // GetShort83Path

      #region GetSuffixedDirectoryName

      /// <summary>Returns the directory information for the specified <paramref name="path"/> with a trailing directory separator.</summary>
      /// <param name="path">The path.</param>
      /// <returns>The directory information for the specified <paramref name="path"/> with a trailing directory separator, or <see langref="null"/> if <paramref name="path"/> is <see langref="null"/>.</returns>
      public static string GetSuffixedDirectoryName(string path)
      {
         return path == null ? null : new PathInfo(path, false).SuffixedDirectoryName;
      }

      #endregion // GetSuffixedDirectoryName

      #region GetSuffixedDirectoryNameWithoutRoot

      /// <summary>Returns the directory information for the specified <paramref name="path"/> without the root and with a trailing directory separator.</summary>
      /// <param name="path">The path.</param>
      /// <returns>The directory information for the specified <paramref name="path"/> without the root and with a trailing directory separator, or <see langref="null"/> if <paramref name="path"/> is <see langref="null"/>.</returns>
      public static string GetSuffixedDirectoryNameWithoutRoot(string path)
      {
         return path == null ? null : new PathInfo(path, false).SuffixedDirectoryNameWithoutRoot;
      }

      #endregion // GetSuffixedDirectoryNameWithoutRoot

      #region IsLogicalDrive

      /// <summary>Determines whether the specified path starts with a Logical Drive; "C:"</summary>
      /// <param name="drive">A pointer to a string that contains the volume's Drive letter as either:  "C" or "C:\..."</param>
      /// <returns><c>true</c> if drive is a logical drive, <c>false</c> if it is not.</returns>
      public static bool IsLogicalDrive(string drive)
      {
         if (string.IsNullOrEmpty(drive))
            return false;

         // Remove LongPath/UNC prefixes, if any.
         drive = GetRegularPath(drive);

         // Don't use char.IsLetter() here as that can be misleading.
         // The only valid drive letters are: a-z and A-Z.
         char c = drive[0];
         bool isLetter = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');

         return drive.Length == 1 ? isLetter : drive.Length >= 2 && (isLetter && drive[1] == VolumeSeparatorChar);
      }

      #endregion // IsLogicalDrive

      #region IsLongPath

      /// <summary>Check if the given path starts with <see cref="LongPathPrefix"/> or <see cref="LongPathUncPrefix"/>.</summary>
      /// <param name="path">The path to the directory or file.</param>
      /// <returns><c>true</c> if path has a (UNC) long path prefix, otherwise <c>false</c>.</returns>
      public static bool IsLongPath(string path)
      {
         return !string.IsNullOrEmpty(path) && path.StartsWith(LongPathPrefix, StringComparison.OrdinalIgnoreCase);
      }

      #endregion // IsLongPath

      #region IsUnc

      /// <summary>Determines whether the specified path is network share path.</summary>
      /// <param name="path">The path to check.</param>
      /// <returns>true if the specified path is a real network share path, otherwise false.</returns>
      public static bool IsUnc(string path)
      {
         return !string.IsNullOrEmpty(path) && (path.StartsWith(UncPrefix, StringComparison.OrdinalIgnoreCase)
                                                   ? path.StartsWith(LongPathUncPrefix, StringComparison.OrdinalIgnoreCase) ||
                                                     !path.StartsWith(LongPathPrefix, StringComparison.OrdinalIgnoreCase)
                                                   : path.StartsWith(DosDeviceLanmanPrefix + ";", StringComparison.OrdinalIgnoreCase) ||
                                                     path.StartsWith(DosDeviceMupPrefix + ";", StringComparison.OrdinalIgnoreCase));

         // Only a real network share has a DosDeviceLanmanPrefix on the DosDevice level.
      }

      #endregion // IsUnc

      #region IsValidName

      /// <summary>Check if file or folder name has any illegal characters.</summary>
      /// <param name="name">File or folder name.</param>
      /// <returns>True or False</returns>
      public static bool IsValidName(string name)
      {
         return name != null && !(name.IndexOfAny(GetInvalidFileNameChars()) >= 0);
      }

      #endregion // IsValidName

      #region IsValidPath

      /// <summary>Verifies that the specified <paramref name="path"/> is valid and optionally may contain wildcards.</summary>
      /// <param name="path">The string to test if it contains a valid path.</param>
      /// <returns><c>true</c> if <paramref name="path"/> is a valid path, <c>false</c> otherwise.</returns>
      public static bool IsValidPath(string path)
      {
         return IsValidPath(path, false);
      }

      /// <summary>Verifies that the specified <paramref name="path"/> is valid and optionally may contain wildcards.</summary>
      /// <param name="path">The string to test if it contains a valid path.</param>
      /// <param name="allowWildcards">If set to <c>true</c> wildcards are allowed in the filename part of the path, otherwise the presence of wildcards will render the path invalid.</param>
      /// <returns><c>true</c> if <paramref name="path"/> is a valid path, <c>false</c> otherwise.</returns>
      public static bool IsValidPath(string path, bool allowWildcards)
      {
         if (string.IsNullOrEmpty(path))
            return false;

         try
         {
            PathInfo pi = new PathInfo(path, allowWildcards);
            return !string.IsNullOrEmpty(pi.GetFullPath());
         }
         catch (ArgumentException)
         {
            return false;
         }
      }

      #endregion // IsValidPath

      #region LocalToUnc

      /// <summary>Converts a local path to a network share path.
      /// A Local path, e.g.: "C:\Windows" will be returned as: "\\localhostname\C$\Windows"
      /// If a logical drive points to a network share path, the share path will be returned.
      /// </summary>
      /// <param name="localPath">A local path, e.g.: "C:\Windows"</param>
      /// <returns>A UNC path or an empty <see cref="string"/> when <paramref name="localPath"/> is <see langword="null"/>.</returns>
      public static string LocalToUnc(string localPath)
      {
         if (string.IsNullOrEmpty(localPath))
            return string.Empty;

         localPath = GetRegularPath(localPath);

         if (IsUnc(localPath))
            return localPath;
         
         // Will return null when no valid drive letter can be created.
         string drive = MakeDriveLetter(localPath);
         if (string.IsNullOrEmpty(drive))
            return string.Empty;

         Network.NativeMethods.RemoteNameInfo unc = Host.GetRemoteNameInfoInternal(drive);

         if (!string.IsNullOrEmpty(unc.ConnectionName))
            // Only leave trailing backslash if "localPath" also ends with backslash.
            return localPath.EndsWith(DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase) ? DirectorySeparatorAdd(unc.ConnectionName, false) : DirectorySeparatorRemove(unc.ConnectionName, false);

         // Split: localDrive[0] = "C", localDrive[1] = "\Windows"
         string[] localDrive = localPath.Split(VolumeSeparatorChar);

         // Return: "\\MachineName\C$\Windows"
         string pathUnc = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}${3}", Host.GetUncName(), DirectorySeparatorChar, localDrive[0], localDrive[1]);

         // Only leave trailing backslash if "localPath" also ends with backslash.
         return localPath.EndsWith(DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase) ? DirectorySeparatorAdd(pathUnc, false) : DirectorySeparatorRemove(pathUnc, false);
      }

      #endregion // LocalToUnc

      #region MakeDriveLetter

      /// <summary>Extract the drive letter from a string: "C:\Program..." --> "C:"</summary>
      /// <param name="path">The path to extract a drive letter from.</param>
      /// <returns>
      /// A string with a valid drive letter followed by a <see cref="Path.VolumeSeparatorChar"/> like "C:"
      /// If <paramref name="path"/> does not start with a valid drive letter, <see langword="null"/> is returned.
      /// </returns>
      public static string MakeDriveLetter(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // Remove LongPath/UNC prefixes, if any.
         path = GetRegularPath(path);

         return IsLogicalDrive(path)
                   ? string.Format(CultureInfo.CurrentCulture, "{0}{1}", path[0], VolumeSeparatorChar)
                   : null;
      }

      #endregion // MakeDriveLetter

      #region PrefixLongPath

      /// <summary>Makes a Unicode path (LongPath) of the specified <paramref name="path"/> by prefixing <see cref="LongPathPrefix"/>.</summary>
      /// <param name="path">The path to the directory or file, this may also be an UNC path.</param>
      /// <returns>The <paramref name="path"/> prefixed with a <see cref="LongPathPrefix"/>.</returns>
      /// <remarks>No path validity checking is performed.</remarks>
      /// <remarks>
      /// Method <see cref="PrefixLongPath"/> only prefixes a <see cref="LongPathPrefix"/> to the specified <paramref name="path"/>.
      /// Method <see cref="GetLongPath"/> creates a <see cref="PathInfo"/> instance which may call <see cref="Directory.GetCurrentDirectory()"/>, yielding possible unexpected results.
      /// </remarks>
      public static string PrefixLongPath(string path)
      {
         if (path == null)
            return null;

         return path.StartsWith(LongPathPrefix, StringComparison.OrdinalIgnoreCase)
                   ? path
                   : (path.StartsWith(UncPrefix, StringComparison.OrdinalIgnoreCase)
                         ? LongPathUncPrefix + path.Substring(UncPrefix.Length)
                         : LongPathPrefix + path);
      }

      #endregion // PrefixLongPath
      

      #region Internal Utility

      #region CheckInvalidPathChars

      /// <summary>Checks that the path contains only valid path-characters.</summary>
      /// <param name="path">A path to the directory or file.</param>
      internal static void CheckInvalidPathChars(string path)
      {
         if (path == null)
            throw new ArgumentNullException("path");

         for (int i = 0, l = path.Length; i < l; i++)
         {
            int c = path[i];

            if (System.IO.Path.GetInvalidPathChars().Any(invalidChar => c == invalidChar))
               throw new ArgumentException("Invalid Path Chars");
         }
      }

      #endregion // CheckInvalidPathChars

      #region DosDeviceToDosPath

      /// <summary>Tranlates DosDevicePath, Volume GUID.
      /// For example: "\Device\HarddiskVolumeX\path\filename.ext" can translate to: "\path\filename.ext" or: "\\?\Volume{GUID}\path\filename.ext".
      /// </summary>
      /// <param name="dosDevice">A DosDevicePath, for example: \Device\HarddiskVolumeX\path\filename.ext</param>
      /// <param name="deviceReplacement">Alternate path/device text, usually <see cref="string.Empty"/> or <see langword="null"/>.</param>
      /// <returns>A translated dos path.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      internal static string DosDeviceToDosPath(string dosDevice, string deviceReplacement)
      {
         if (string.IsNullOrEmpty(dosDevice))
            return string.Empty;

         foreach (string drive in Directory.GetLogicalDrives(false, false, true))
         {
            try
            {
               foreach (string devNt in Volume.QueryDosDevice(drive).Where(dosDevice.StartsWith))
                  return dosDevice.Replace(devNt, deviceReplacement ?? drive);
            }
            catch
            {
            }
         }
         return string.Empty;
      }

      #endregion // DosDeviceToDosPath

      #region EndsWithDVsc

      /// <summary>Check if <paramref name="path"/> ends with a directory- and/or volume-separator character.</summary>
      /// <param name="path">The patch to check.</param>
      /// <param name="checkVolumeSeparatorChar">
      /// If <c>null</c>, checks for all separator characters: <see cref="DirectorySeparatorChar"/>, <see cref="AltDirectorySeparatorChar"/> and <see cref="VolumeSeparatorChar"/>
      /// If <c>false</c>, only checks for: <see cref="DirectorySeparatorChar"/> and <see cref="AltDirectorySeparatorChar"/>
      /// If <c>true</c>, only checks for: <see cref="VolumeSeparatorChar"/>
      /// </param>
      /// <returns><c>true</c> if <paramref name="path"/> ends with a separator character.</returns>
      internal static bool EndsWithDVsc(string path, bool? checkVolumeSeparatorChar)
      {
         return path != null && path.Length >= 1 && IsDVsc(path[path.Length - 1], checkVolumeSeparatorChar);
      }

      #endregion // EndsWithDVsc

      #region IsDVsc

      /// <summary>Check if <paramref name="c"/> is a directory- and/or volume-separator character.</summary>
      /// <param name="c">The character to check.</param>
      /// <param name="checkVolumeSeparatorChar">
      /// If <c>null</c>, checks for all separator characters: <see cref="DirectorySeparatorChar"/>, <see cref="AltDirectorySeparatorChar"/> and <see cref="VolumeSeparatorChar"/>
      /// If <c>false</c>, only checks for: <see cref="DirectorySeparatorChar"/> and <see cref="AltDirectorySeparatorChar"/>
      /// If <c>true</c>, only checks for: <see cref="VolumeSeparatorChar"/>
      /// </param>
      /// <returns><c>true</c> if <paramref name="c"/> is a separator character.</returns>
      internal static bool IsDVsc(char c, bool? checkVolumeSeparatorChar)
      {
         return checkVolumeSeparatorChar == null

                   // Check for all separator characters.
                   ? c == DirectorySeparatorChar || c == AltDirectorySeparatorChar || c == VolumeSeparatorChar

                   // Check for some separator characters.
                   : ((bool) checkVolumeSeparatorChar
                         ? c == VolumeSeparatorChar
                         : c == DirectorySeparatorChar || c == AltDirectorySeparatorChar);
      }

      #endregion // IsDVsc

      #endregion // Internal Utility

      #region Unified Internals

      #region GetLongShort83PathInternal

      /// <summary>Unified method GetLongShort83PathInternal() to retrieve the short path form, or the regular long form of the specified path.</summary>
      /// <param name="path">An existing path to a folder or file.</param>
      /// <param name="getShort"><c>true</c> to retrieve the short path form, <c>false</c> to retrieve the regular long form from the 8.3 <paramref name="path"/>.</param>
      /// <returns>Depending on <paramref name="getShort"/> a string with a path that has the 8.3 path, or the regular long form</returns>
      /// <remarks>Will fail on NTFS volumes with disabled 8.3 name generation.</remarks>
      /// <remarks>The path must actually exist to be able to get the short- or long path name.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "lastError")]
      [SecurityCritical]
      internal static string GetLongShort83PathInternal(string path, bool getShort)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-28: MSDN confirms LongPath usage.
         string pathLp = PrefixLongPath(path);

         StringBuilder buffer = new StringBuilder();
         uint actualLength = (uint) path.Length;

         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            while (actualLength > buffer.Capacity)
            {
               buffer = new StringBuilder((int) actualLength);
               actualLength = getShort
                                 ? NativeMethods.GetShortPathName(pathLp, buffer, (uint) buffer.Capacity)
                                 : NativeMethods.GetLongPathName(pathLp, buffer, (uint) buffer.Capacity);

               if (actualLength == Win32Errors.ERROR_SUCCESS)
                  NativeError.ThrowException(pathLp);
            }

         return GetRegularPath(buffer.ToString());
      }

      #endregion // GetLongShort83PathInternal

      #endregion // Unified Internals

      #endregion // AlphaFS
   }
}