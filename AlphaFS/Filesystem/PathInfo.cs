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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

// Also see PathInfoParser.cs and PathInfoComponentList.cs

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>A representation of a path, providing convenient access to the individual components of the path.</summary>
   /// <remarks>Note that no methods in this class verifies whether the path actually exists or not.</remarks>
   [Serializable]
   public partial class PathInfo : IEquatable<PathInfo>, IComparable<PathInfo>
   {
      #region Class Internal Affairs

      /// <summary>Combines two paths.</summary>
      /// <param name="path1">The first path. </param>
      /// <param name="path2">The second path.</param>
      /// <returns>A string containing the combined paths. If one of the specified paths is a zero-length string, this method returns the other path. If path2 contains an absolute path, this method returns path2.</returns>
      [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
      public static PathInfo operator +(PathInfo path1, PathInfo path2)
      {
         return Combine(path1, path2);
      }

      /// <summary>Implements the operator ==.</summary>
      /// <param name="path1">The path1.</param>
      /// <param name="path2">The path2.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(PathInfo path1, PathInfo path2)
      {
         return ReferenceEquals(path1, null) ? ReferenceEquals(path2, null) : path1.Equals(path2);
      }

      /// <summary>Implements the operator !=.</summary>
      /// <param name="path1">The path1.</param>
      /// <param name="path2">The path2.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(PathInfo path1, PathInfo path2)
      {
         if (ReferenceEquals(path1, null))
            return !ReferenceEquals(path2, null);

         return !path1.Equals(path2);
      }

      /// <summary>Implements the operator &lt;</summary>
      /// <param name="path1">The path1.</param>
      /// <param name="path2">The path2.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator <(PathInfo path1, PathInfo path2)
      {
         return path1 != null && (path2 != null && path1.CompareTo(path2) < 0);
      }

      /// <summary>Implements the operator &gt;</summary>
      /// <param name="path1">The path1.</param>
      /// <param name="path2">The path2.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator >(PathInfo path1, PathInfo path2)
      {
         return path1 != null && (path2 != null && path1.CompareTo(path2) > 0);
      }

      /// <summary>Performs a lexiographical comparison of the string representations of this and the other path, ignoring case.</summary>
      /// <param name="other">A <see cref="PathInfo"/> to compare with this object.</param>
      /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.</returns>
      public int CompareTo(PathInfo other)
      {
         if (other == null)
            throw new ArgumentNullException("other");

         return string.Compare(Path, other.Path, StringComparison.OrdinalIgnoreCase);
      }

      /// <summary>Performs a lexiographical comparison for equality of the string representations of this and the other path, ignoring case.</summary>
      /// <param name="other">An object to compare with this object.</param>
      /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
      public bool Equals(PathInfo other)
      {
         return other != null && string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
      }

      /// <summary>Performs a lexiographical comparison for equality of the string representations of this and the other path, ignoring case.</summary>
      /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
      /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, false.</returns>
      /// <exception cref="System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
      public override bool Equals(object obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         PathInfo other = obj as PathInfo;

         if (other == null)
            return false;

         return Equals(other);
      }

      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>A hash code for the current Object.</returns>
      public override int GetHashCode()
      {
         return !string.IsNullOrEmpty(Path) ? Path.GetHashCode() : 17;
      }

      /// <summary>Returns a <see cref="System.String"/> that represents the current <see cref="System.Object"/>.</summary>
      /// <returns>A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.</returns>
      public override string ToString()
      {
         return Path;
      }

      #endregion // Class Internal Affairs

      #region Constructors

      /// <summary>Initializes a new instance of the <see cref="PathInfo"/> class specifying whether wildcards should be accepted or not.</summary>
      /// <param name="path">The path.</param>
      /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/></exception>
      /// <remarks>
      ///     <para>Note that under no circumstances will this class accept wildcards in 
      ///           the directory part of the path, only in the file-name, i.e. the component
      ///           after the last backslash or separator. 
      ///     </para>
      ///     <para>
      ///         Extended length unicode paths (also referred to as long paths) (those starting with \\?\) will <b>not</b> be 
      ///         parsed for wildcards etc., regardless of the setting of this parameter.
      ///         In such a path any character is valid and backslashes alone are considered
      ///         to be separators.
      ///     </para>
      /// </remarks>
      public PathInfo(string path)
         : this(path, false)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="PathInfo"/> class specifying whether wildcards should be accepted or not.</summary>
      /// <param name="path">The path.</param>
      /// <param name="allowWildcardsInFileName">If set to <c>true</c> wildcards are allowed in the file 
      /// name part of the path. If set to <c>false</c>, wildcards are not allowed and an
      /// <see cref="ArgumentException" /> will be thrown if they are present.</param>
      /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/></exception>
      /// <remarks>
      ///     <para>Note that under no circumstances will this class accept wildcards in 
      ///           the directory part of the path, only in the file-name, i.e. the component
      ///           after the last backslash or separator. 
      ///     </para>
      ///     <para>
      ///         Extended length unicode paths (also referred to as long paths) (those starting with \\?\) will <b>not</b> be 
      ///         parsed for wildcards etc., regardless of the setting of this parameter.
      ///         In such a path any character is valid and backslashes alone are considered
      ///         to be separators.
      ///     </para>
      /// </remarks>
      public PathInfo(string path, bool allowWildcardsInFileName)
      {
         Parser parser = new Parser(path, allowWildcardsInFileName);
         _mPath = parser.Path;
         _mIndices = parser.ComponentIndices;
         _mExtensionIndex = parser.ExtensionIndex;
      }

      /// <summary>Initializes a new instance of the <see cref="PathInfo"/> class.</summary>
      /// <param name="path">The path.</param>
      /// <param name="indices">The indices.</param>
      /// <param name="extensionIndex">Position of the beginning of the file extension in the path.</param>
      private PathInfo(string path, List<int> indices, int extensionIndex)
      {
         _mPath = path;
         _mIndices = indices;
         _mExtensionIndex = extensionIndex;
      }

      #endregion // Constructors

      #region Public Methods

      #region GetFullPath

      /// <summary>Returns the absolute path for the specified path string.</summary>
      /// <returns>A <see langref="String"/> string containing the fully qualified location of path, such as "C:\MyFile.txt".</returns>
      /// <remarks>This parameter can be a short (the 8.3 form) or long file name. This string can also be a share or volume name.</remarks>
      /// <remarks>The GetFullPathName function is not recommended for multithreaded applications or shared library code.</remarks>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public string GetFullPath()
      {
         return Filesystem.Path.GetFullPath(_mPath);

         //if (IsRooted)
         //{
         //   if (!Root.Equals(Filesystem.Path.DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase))
         //      return _mPath;

         //   // A rooted directory without a drive/volume/share root.
         //   Parser p = new Parser(Directory.GetCurrentDirectory(), true);
         //   return Filesystem.Path.Combine(p.Root, Path.Substring(1));
         //}

         //PathInfo current = new PathInfo(Directory.GetCurrentDirectory(), false);
         //return Combine(current, this).Path;
      }

      #endregion GetFullPath

      #region GetLongPath

      /// <summary>Retrieves the full long (or extended) unicode version of the path represented by this <see cref="PathInfo"/> instance.</summary>
      /// <remarks>
      /// <para>
      ///		This method takes care of different path conversions to be usable in Unicode 
      ///		variants of the Win32 funcitons (which are internally used throughout AlphaFS).
      /// </para>
      /// <para>
      ///		Regular paths are changed like the following:
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
      /// <para>
      ///		Already processed paths are preserved untouched so to avoid mistakes of double prefixing.
      /// </para>
      /// <para>
      ///		If the path represented by this instance is not an absolute path, or is not rooted, the path of the
      ///		current directory (and drive) is combined with this path to form
      ///		an absolute path.
      /// </para>
      /// </remarks>
      /// <returns>The long or extended unicode version of the specified path.</returns>
      /// <seealso cref="GetFullPath"/>
      /// <seealso cref="Filesystem.Path.GetLongPath"/>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public string GetLongPath()
      {
         if (_mPath.StartsWith(Filesystem.Path.LongPathPrefix, StringComparison.OrdinalIgnoreCase))
            return _mPath;

         return _mPath.StartsWith(Filesystem.Path.UncPrefix, StringComparison.OrdinalIgnoreCase)
                    ? Filesystem.Path.LongPathUncPrefix + _mPath.Substring(2)
                    : Filesystem.Path.LongPathPrefix + GetFullPath();
      }

      #endregion // GetLongPath

      #endregion // Public Methods

      #region Public Properties

      #region DirectoryComponents

      /// <summary>Gets a list exposing the individual components of the directory part of this path.</summary>
      /// <value>The directory components of this path.</value>
      public IList<string> DirectoryComponents
      {
         get
         {
            return new ComponentList(this);
         }
      }

      #endregion // DirectoryComponents

      #region DirectoryName

      /// <summary>Returns the directory information for the path string.</summary>
      /// <value>Directory information for path, or null if path denotes a root directory or is null. Returns <see cref="string.Empty"/> if path does not contain directory information.</value>
      /// <seealso cref="SuffixedDirectoryName"/>
      public string DirectoryName
      {
         //get
         //{
         //   if (_mIndices.Count == 1)
         //   {
         //      // We have a root only.
         //      //if (_mIndices[0] != 0)
         //      if (IsRooted)
         //      {
         //         string root = Root;
         //         return Filesystem.Path.IsDVsc(root[root.Length - 1], false) ? root : string.Empty;
         //      }

         //      return string.Empty;
         //   }

         //   return _mPath.Substring(0, _mIndices[_mIndices.Count - 1] - 1);
         //}

         get
         {
            int rootLength = Root.Length;

            if (_mPath.Length > rootLength)
            {
               int length = _mPath.Length;
               if (length == rootLength)
                  return string.Empty;

               while (length > rootLength && !Filesystem.Path.IsDVsc(_mPath[--length], false))
               {
               }

               return _mPath.Substring(0, length);
            }

            // 2013-03-15: Yomodo; Added to "fix" PathInfo().DirectoryName;
            if (_mExtensionIndex == 0 && Root.Length == 0)
               return string.Empty;

            return null;
         }
      }

      #endregion // DirectoryName

      #region DirectoryNameWithoutRoot

      /// <summary>Returns the directory information for the path with the root stripped off.</summary>
      /// <value>The path without the root and file name part (if any).</value>
      public string DirectoryNameWithoutRoot
      {
         get
         {
            int length = _mIndices[_mIndices.Count - 1] - _mIndices[0] - 1;

            return length > 0 ? _mPath.Substring(_mIndices[0], length) : string.Empty;
         }
      }

      #endregion // DirectoryNameWithoutRoot

      #region Extension

      /// <summary>Gets the extension of the file name of this path.</summary>
      /// <value>The extension of the file name of this path, or an empty string if the path does
      /// not contain a file name or the file name does not have an extension.</value>
      public string Extension
      {
         get { return HasExtension ? Path.Substring(_mExtensionIndex) : string.Empty; }
      }

      #endregion // Extension

      #region FileName

      /// <summary>Gets the file name part of the path.</summary>
      /// <value>The file name part of the path, or an empty string if the path does not contain a file name.</value>
      /// <returns>
      /// A string consisting of the characters after the last directory character in path.
      /// If the last character of <see cref="FileName"/> is a <see cref="Filesystem.Path.DirectorySeparatorChar"/> or <see cref="Filesystem.Path.VolumeSeparatorChar"/>, this method returns <see langword="string.Empty"/>.
      /// </returns>
      /// <remarks>If the last character of <see cref="FileName"/> is a directory- or volume-separator character, this method returns <see cref="string.Empty"/>.</remarks>
      public string FileName
      {
         get
         {
            //return _mPath.Substring(_mIndices[_mIndices.Count - 1]);

            int length = _mPath.Length;
            for (int i = length; --i >= 0;)
            {
               if (Filesystem.Path.IsDVsc(_mPath[i], null))
                  return _mPath.Substring(i + 1, length - i - 1);
            }

            return _mPath;
         }
      }

      #endregion // FileName

      #region FileNameWithoutExtension

      /// <summary>Gets the file name without extension.</summary>
      /// <value>The file name without extension or an empty string if the path does not contain a file name.</value>
      public string FileNameWithoutExtension
      {
         get
         {
            //return _mPath.Substring(_mIndices[_mIndices.Count - 1], _mExtensionIndex - _mIndices[_mIndices.Count - 1]);

            int i;
            string file = FileName;
            return (i = file.LastIndexOf(Filesystem.Path.ExtensionSeparatorChar)) == -1 ? file : file.Substring(0, i);
         }
      }

      #endregion // FileNameWithoutExtension

      #region HasExtension

      /// <summary>Gets a value indicating whether the file name in this path has an extension.</summary>
      /// <value><c>true</c> if the file name in this path has an extension; otherwise, <c>false</c>.</value>
      public bool HasExtension
      {
         get { return _mExtensionIndex < _mPath.Length - 1; }
      }

      #endregion // HasExtension

      #region HasFileName

      /// <summary>Gets a value indicating whether this instance has file name.</summary>
      /// <value><c>true</c> if this instance has file name; otherwise, <c>false</c>.</value>
      public bool HasFileName
      {
         get
         {
            //return _mIndices[_mIndices.Count - 1] != _mPath.Length;

            return !string.IsNullOrEmpty(FileName);
         }
      }

      #endregion // HasFileName

      #region IsRooted

      /// <summary>Gets a value indicating whether the path is rooted.</summary>
      /// <value><c>true</c> if this instance is rooted; otherwise, <c>false</c>.</value>
      public bool IsRooted
      {
         get
         {
            //return _mIndices[0] != 0;

            // Same as: Path.IsPathRooted();
            return _mPath.Length >= 1 && Filesystem.Path.IsDVsc(_mPath[0], false) ||
                   (_mPath.Length >= 2 && Filesystem.Path.IsDVsc(_mPath[1], true));
         }
      }

      #endregion // IsRooted

      #region Parent

      /// <summary>Retrieves the parent directory of the specified path, including both absolute and relative paths.</summary>
      /// <returns>The parent directory, or <see langword="null"/> if path is the root directory, including the root of a server or share name.</returns>
      public PathInfo Parent
      {
         get
         {
            Debug.Assert(_mIndices.Count > 0);

            // The parent of just the root, is the root.
            if (_mIndices.Count == 1)
            {
               // No root, result will be empty string.
               if (_mIndices[0] == 0)
                  return new PathInfo(string.Empty, false);

               return _mIndices[_mIndices.Count - 1] == _mPath.Length
                          ? this
                          : new PathInfo(_mPath.Substring(0, _mIndices[_mIndices.Count - 1]), _mIndices, -1);
            }

            return HasFileName
                       ? new PathInfo(_mPath.Substring(0, _mIndices[_mIndices.Count - 1]), _mIndices, -1)
                       : new PathInfo(_mPath.Substring(0, _mIndices[_mIndices.Count - 2]), _mIndices.GetRange(0, _mIndices.Count - 1), -1);
         }
      }

      #endregion // Parent

      #region Path

      /// <summary>Gets the full normalized path.</summary>
      /// <value>The full path.</value>
      /// <seealso cref="SuffixedPath"/>
      public string Path
      {
         // If No filename, we may have to remove a trailing slash.
         get { return HasFileName || _mIndices.Count == 1 ? _mPath : _mPath.Substring(0, _mPath.Length - 1); }
      }

      #endregion // Path

      #region Root

      /// <summary>Gets the root of the path.</summary>
      /// <value>The root of the path, which may be a drive (eg. "C:\"), a remote computer as part of 
      /// an network share (eg. "\\OtherComputer\"), a unique volume name 
      /// (eg. "\\?\Volume{c00fa7c5-63eb-11dd-b6ed-806e6f6e6963}\") or a single directory
      /// separator ("\") if no drive or volume is present in the path.
      /// If path does not contain any root, an empty string is returned.</value>
      public string Root
      {
         get
         {
            //return _mPath.Substring(0, _mIndices[0]);

            string root = _mPath.Substring(0, _mIndices[0]);

            if (root.StartsWith(Filesystem.Path.VolumePrefix, StringComparison.OrdinalIgnoreCase))
               return Filesystem.Path.DirectorySeparatorRemove(root, false);

            if (Filesystem.Path.IsUnc(root))
            {
               if (Filesystem.Path.EndsWithDVsc(root, false))
                  // Allows: \\server\c$\ => \\server\c$
                  return root.TrimEnd(Filesystem.Path.DirectorySeparatorChar, Filesystem.Path.AltDirectorySeparatorChar);
            }
            else if (Filesystem.Path.EndsWithDVsc(root, true))
               // Allows: C: => C:\
               return root + Filesystem.Path.DirectorySeparatorChar;

            return root;
         }
      }

      #endregion // Root

      #region SuffixedDirectoryName

      /// <summary>Returns the directory information for the path with a trailing directory separator.</summary>
      /// <value>The name of the suffixed directory with a trailing directory separator.</value>
      /// <seealso cref="DirectoryName"/>
      public string SuffixedDirectoryName
      {
         get { return _mPath.Substring(0, _mIndices[_mIndices.Count - 1]); }
      }

      #endregion // SuffixedDirectoryName

      #region SuffixedDirectoryNameWithoutRoot

      /// <summary>Returns the directory information for the path without the root information, and with a trailing backslash.</summary>
      /// <value>The path without the root and file name part (if any) and with a trailing backslash.</value>
      /// <seealso cref="DirectoryNameWithoutRoot"/>
      /// <seealso cref="DirectoryName"/>
      /// <seealso cref="SuffixedDirectoryName"/>
      public string SuffixedDirectoryNameWithoutRoot
      {
         get { return _mPath.Substring(_mIndices[0], _mIndices[_mIndices.Count - 1] - _mIndices[0]); }
      }

      #endregion // SuffixedDirectoryNameWithoutRoot

      #region SuffixedPath

      /// <summary>Gets the full normalized path, with a trailing backslash if the path denotes a directory.</summary>
      /// <value>The full normalized path, with a trailing backslash if the path denotes a directory.</value>
      /// <seealso cref="Path"/>
      public string SuffixedPath
      {
         get { return _mPath; }
      }

      #endregion // SuffixedPath
      
      #endregion // Public Properties

      #region Public Static Methods

      /// <summary>Combines two paths.</summary>
      /// <param name="path1">The first path. </param>
      /// <param name="path2">The second path.</param>
      /// <returns>A string containing the combined paths. If one of the specified paths is a zero-length string, this method returns the other path. If path2 contains an absolute path, this method returns path2.</returns>
      public static PathInfo Combine(PathInfo path1, PathInfo path2)
      {
         if (path1 == null)
            throw new ArgumentNullException("path1");

         if (path2 == null)
            throw new ArgumentNullException("path2");

         if (path2.IsRooted || path1.Path.Length == 0)
            return path2;

         // TODO: This method could be made more efficient
         StringBuilder sb = new StringBuilder(path1.Path.Length + path2.Path.Length + 1);
         sb.Append(path1);
         char lastChar = sb[sb.Length - 1];

         if (!Filesystem.Path.IsDVsc(lastChar, false))
            sb.Append(Filesystem.Path.DirectorySeparatorChar);

         sb.Append(path2);
         return new PathInfo(sb.ToString(), true);
      }

      #endregion // Public Static Methods

      #region Private Fields

      readonly string _mPath;
      readonly List<int> _mIndices;
      readonly int _mExtensionIndex;


      #endregion // Private Fields
   }
}