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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Alphaleonis.Win32.Filesystem
{
   public partial class PathInfo
   {
      // This parser is used by PathInfo to create the internal representation of the path.
      // It could have been made simpler by using eg. regular expressions, however this 
      // implementation should be at least twice as efficient, allowing for greater 
      // flexibility.
      private class Parser
      {
         #region Constructors

         #region Parser

         static Parser()
         {
            int max = 0;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
               if (c > max)
                  max = c;
            }

            mInvalidFileNameCharsArray = new bool[max];

            for (int i = 0, l = mInvalidFileNameCharsArray.Length; i < l; i++)
               mInvalidFileNameCharsArray[i] = Array.IndexOf(System.IO.Path.GetInvalidFileNameChars(), (char) i) != -1;
         }

         public Parser(string path, bool allowWildcards)
         {
            if (path == null)
               throw new ArgumentNullException("path");

            mPath = path;
            mAllowFileNameWildcards = allowWildcards;
            MatchRoot();

            if (!IsSeparator(LA(-1)) && !IsTerminatorChar(LA(-1)) && !IsTerminatorChar(LA(0)))
               throw new ArgumentException("Invalid Path");

            mIndices.Add(mBuilder.Length);
            MatchDir();
         }

         #endregion // Parser

         #endregion // Constructors

         #region Private Matching Functions

         #region MatchDir

         private void MatchDir()
         {
            mExtensionPos = -1;
            do
            {
               bool hasWildcard = false;
               bool hasInvalidFileNameChars = false;

               // Read everything up to a separator.
               char c;
               while (!IsSeparator((c = LA(0))) && !IsTerminatorChar(c))
               {
                  if (mIsNotInternalDirectory)
                  {
                     hasWildcard = hasWildcard || IsWildcard(c);
                     hasInvalidFileNameChars = hasInvalidFileNameChars || (!IsWildcard(c) && !IsValidFileNameChar(c));
                  }

                  if (c == Filesystem.Path.ExtensionSeparatorChar)
                     mExtensionPos = mBuilder.Length;

                  mBuilder.Append(c);
                  ++mCurPos;
               }

               bool add = true;

               // An ending separator has not yet been added under any circumstance.
               if (mIsNotInternalDirectory)
               {
                  if (NewComponentMatches(Filesystem.Path.ParentDirectoryPrefix))
                  {
                     // Remove references to the parent directory if possible.
                     add = !ResolveParentReference();
                  }
                  else
                     if (NewComponentMatches(Filesystem.Path.CurrentDirectoryPrefix))
                     {
                        // Skip references to the current directory (".")
                        add = false;

                        // 2013-03-15: Yomodo; Disabled to get PathInfo().FileName working.
                        //mBuilder.Length = mBuilder.Length - 1;
                     }
               }

               if (IsSeparator(LA(0)))
               {
                  if (add)
                  {
                     // Wildcards not allowed in directory components.
                     if (mIsNotInternalDirectory && (hasInvalidFileNameChars || hasWildcard))
                        throw new ArgumentException("Invalid characters in path");

                     mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
                     mIndices.Add(mBuilder.Length);
                  }
                  Next();
                  // Don't store extension for directories.
                  mExtensionPos = -1;
               }
               else
               {
                  if (mIsNotInternalDirectory)
                  {
                     if (hasInvalidFileNameChars)
                        throw new ArgumentException("Invalid characters in file name");

                     if (!mAllowFileNameWildcards && hasWildcard)
                        throw new ArgumentException("Wildcards are not allowed");
                  }

                  break;
               }
            }
            while (true);
            MatchFile();
         }

         #endregion // MatchDir

         #region MatchDrive

         private void MatchDrive()
         {
            Debug.Assert(IsDrive(LA(0), LA(1)));
            mBuilder.Append(LA(0));
            mBuilder.Append(Filesystem.Path.VolumeSeparatorChar);
            Skip(2);
            if (IsSeparator(LA(0)))
            {
               mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
               Next();
            }
         }

         #endregion // MatchDrive

         #region MatchFile

         private void MatchFile()
         {
            // No extension was found.
            if (mExtensionPos == -1)
               mExtensionPos = mBuilder.Length;

            // 2013-03-12: Yomodo; Disabled to emulate .NET behaviour.
            //else if (mExtensionPos == mBuilder.Length - 1)
            //{
            //   // An empty extension was found, so we remove the trailing dot.
            //   mBuilder.Length = mBuilder.Length - 1;
            //   mExtensionPos = mBuilder.Length;
            //}
            // File name should be in 'file'
         }

         #endregion // MatchFile

         #region MatchPrefixedLongPath

         private void MatchPrefixedLongPath()
         {
            Debug.Assert(LaMatchesExact(Filesystem.Path.LongPathPrefix));
            mBuilder.Append(Filesystem.Path.LongPathPrefix);
            Skip(4);

            // Check for \\?\c:\path\file.ext type paths
            if (IsDrive(LA(0), LA(1)))
            {
               mBuilder.Append(LA(0));
               mBuilder.Append(Filesystem.Path.VolumeSeparatorChar);

               if (Filesystem.Path.IsDVsc(LA(2), false))
               {
                  mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
                  Skip(3);
               }
               else
               {
                  Skip(2);
                  if (Filesystem.Path.IsDVsc(LA(0), false))
                  {
                     mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
                     Skip(1);
                  }
               }
            }
            else if (LaMatchesVolume()) // Check for \\?\volume{D0E234D3-87C4-4b27-8B88-DA418BFDD0C7}\... type paths
            {
               mBuilder.Append(mPath.Substring(mCurPos, 44));
               Skip(44);
               if (Filesystem.Path.IsDVsc(LA(0), false))
               {
                  mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
                  Next();
               }
            }
            else if (LaMatchesIgnoreCase("globalroot")) // Check for \\?\GLOBALROOT\... paths
            {
               mBuilder.Append("GLOBALROOT");
               Skip(10);
               if (Filesystem.Path.IsDVsc(LA(0), false))
               {
                  mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
                  Next();
               }
            }
            else if (LaMatchesIgnoreCase("unc")) // Check for \\?\UNC\Server\Share\... type paths.
            {
               mBuilder.Append("UNC");
               Skip(3);
               if (Filesystem.Path.IsDVsc(LA(0), false))
               {
                  mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
                  Next();
                  MatchServerNameAndShare();
               }
            }
         }

         #endregion // MatchPrefixedLongPath

         #region MatchRoot

         private void MatchRoot()
         {
            // Match "\" (or "/")
            if (IsSeparator(LA(0)))
            {
               // Match "\\?"
               if (IsSeparator(LA(1)) && LA(2) != Filesystem.Path.WildcardQuestionChar)
               {
                  // We have "\\X" where X is not a "?", so must be UNC path.
                  MatchUnc();
               }
               else if (LaMatchesExact(Filesystem.Path.LongPathPrefix))
               {
                  // We have "\\?\"
                  MatchPrefixedLongPath();
                  mIsNotInternalDirectory = false;
               }
               else
               {
                  // We have "\XXXX", i.e. an absolute path without a drive specification.
                  MatchUnqualifiedRoot();
               }
            }
            else if (IsDrive(LA(0), LA(1)))
            {
               // We have "X:" for some valid drive letter X.
               MatchDrive();
            }
         }

         #endregion // MatchRoot

         #region MatchServerNameAndShare

         private void MatchServerNameAndShare()
         {
            Debug.Assert(IsDirNameChar(LA(0)));

            char c;
            while (!IsSeparator(c = LA(0)) && !IsTerminatorChar(c))
            {
               mBuilder.Append(c);
               Next();
            }

            if (IsTerminatorChar(c))
               throw new ArgumentException(Resources.UNCPathShouldMatchTheFormatServerShare, Path);

            Skip(1);
            mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);

            while (!IsSeparator(c = LA(0)) && !IsTerminatorChar(c))
            {
               mBuilder.Append(c);
               Next();
            }
            
            if (!IsTerminatorChar(c))
               Skip(1);

            // No directory or file specified, only server+share (server+hidden-drive).
            // Only append trailing backslash when last checked character is a separator, this will allow: \\server\c$
            if (IsSeparator(c))
               mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
         }

         #endregion // MatchServerNameAndShare

         #region MatchShortPathUncPrefix

         private void MatchShortPathUncPrefix()
         {
            Debug.Assert(IsSeparator(LA(0)) && IsSeparator(LA(1)));
            Skip(2);
            mBuilder.Append(Filesystem.Path.UncPrefix);
         }

         #endregion // MatchShortPathUncPrefix

         #region MatchUnc

         private void MatchUnc()
         {
            MatchShortPathUncPrefix();
            MatchServerNameAndShare();
         }

         #endregion // MatchUnc

         #region MatchUnqualifiedRoot

         private void MatchUnqualifiedRoot()
         {
            Debug.Assert(IsSeparator(LA(0)) && !IsSeparator(LA(1)));
            mBuilder.Append(Filesystem.Path.DirectorySeparatorChar);
            Next();
         }

         #endregion // MatchUnqualifiedRoot

         #endregion // Private Matching Functions

         #region Private Utility Methods

         #region IsDirNameChar

         bool IsDirNameChar(char c)
         {
            return !IsSeparator(c) && !IsWildcard(c) && !IsTerminatorChar(c);
         }

         #endregion // IsDirNameChar
         
         #region IsDrive

         static bool IsDrive(char driveLetter, char separator)
         {
            return IsDriveLetter(driveLetter) && IsDriveSeparator(separator);
         }

         #endregion // IsDrive

         #region IsDriveLetter

         static bool IsDriveLetter(char c)
         {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
         }

         #endregion // IsDriveLetter

         #region IsDriveSeparator

         static bool IsDriveSeparator(char c)
         {
            return c == Filesystem.Path.VolumeSeparatorChar;
         }

         #endregion // IsDriveSeparator

         #region IsSeparator

         /// <summary>Check if a character equals <see cref="Filesystem.Path.DirectorySeparatorChar"/> or <see cref="Filesystem.Path.DirectorySeparatorChar"/></summary>
         /// <param name="c">The character to check.</param>
         /// <returns><c>true</c> if the character is one of the defined separator characters.</returns>
         bool IsSeparator(char c)
         {
            return (c == Filesystem.Path.DirectorySeparatorChar) || (mIsNotInternalDirectory && c == Filesystem.Path.AltDirectorySeparatorChar);
         }

         #endregion // IsSeparator

         #region IsTerminatorChar

         /// <summary>Check if a character equals the <see cref="Filesystem.Path.StringTerminatorChar"/> character</summary>
         /// <param name="c">The character to check.</param>
         /// <returns><c>true</c> if the character is the string terminator character.</returns>
         static bool IsTerminatorChar(char c)
         {
            return c == Filesystem.Path.StringTerminatorChar;
         }

         #endregion // IsTerminatorChar

         #region IsValidFileNameChar

         static bool IsValidFileNameChar(char c)
         {
            int i = c;
            return i >= mInvalidFileNameCharsArray.Length || !mInvalidFileNameCharsArray[i];
         }

         #endregion // IsValidFileNameChar

         #region IsWildcard

         /// <summary>Check if a character equals <see cref="Filesystem.Path.WildcardStarMatchAllChar"/> or <see cref="Filesystem.Path.WildcardQuestionChar"/></summary>
         /// <param name="c">The character to check.</param>
         /// <returns><c>true</c> if the character is one of the defined wildcard characters.</returns>
         static bool IsWildcard(char c)
         {
            return c == Filesystem.Path.WildcardStarMatchAllChar || c == Filesystem.Path.WildcardQuestionChar;
         }

         #endregion // IsWildcard

         #region LA

         char LA(int index)
         {
            int realPos = mCurPos + index;
            return (realPos < 0 || realPos >= mPath.Length) ? Filesystem.Path.StringTerminatorChar : mPath[realPos];
         }

         #endregion // LA

         #region LaMatchesHexDigitSequence

         private bool LaMatchesHexDigitSequence(int length, int index)
         {
            for (int i = index; i < length + index; i++)
            {
               if ((LA(i) < '0' || LA(i) > '9') && (LA(i) < 'a' || LA(i) > 'f') && (LA(i) < 'A' || LA(i) > 'F'))
                  return false;
            }
            return true;
         }

         #endregion // LaMatchesHexDigitSequence

         #region LaMatchesIgnoreCase

         private bool LaMatchesIgnoreCase(string s)
         {
            return !s.Where((t, i) => char.ToLower(LA(i), CultureInfo.InvariantCulture) != char.ToLower(t, CultureInfo.InvariantCulture)).Any();
         }

         #endregion // LaMatchesIgnoreCase

         #region LaMatchesExact

         private bool LaMatchesExact(string s)
         {
            //for (int i = 0; i < s.Length; i++)
            //   if (LA(i) != s[i])
            //      return false;
            //return true;

            return !s.Where((c, i) => LA(i) != c).Any();
         }

         #endregion // LaMatchesExact

         #region LaMatchesVolume

         private bool LaMatchesVolume()
         {
            // Matches eg. volume{D0E234D3-87C4-4b27-8B88-DA418BFDD0C7}
            if (!LaMatchesIgnoreCase("volume{"))
               return false;

            int i = "volume{".Length;

            if (!LaMatchesHexDigitSequence(8, i))
               return false;
            i += 8;

            if (LA(i) != '-')
               return false;

            if (!LaMatchesHexDigitSequence(4, ++i))
               return false;
            i += 4;

            if (LA(i) != '-')
               return false;

            if (!LaMatchesHexDigitSequence(4, ++i))
               return false;
            i += 4;

            if (LA(i) != '-')
               return false;

            if (!LaMatchesHexDigitSequence(4, ++i))
               return false;
            i += 4;

            if (LA(i) != '-')
               return false;

            if (!LaMatchesHexDigitSequence(12, ++i))
               return false;
            i += 12;

            if (LA(i++) != '}')
               return false;

            if (!Filesystem.Path.IsDVsc(LA(i), false) && !IsTerminatorChar(LA(i)))
               return false;

            return true;
         }

         #endregion // LaMatchesVolume

         #region NewComponentMatches

         private bool NewComponentMatches(string s)
         {
            // If there is no last component, we cannot compare it.
            if (mIndices.Count < 1)
               return false;

            if (s.Length != mBuilder.Length - mIndices[mIndices.Count - 1])
               return false;

            int c = 0;
            for (int i = mIndices[mIndices.Count - 1]; i < mBuilder.Length; i++)
               if (mBuilder[i] != s[c++])
                  return false;

            return true;
         }

         #endregion // NewComponentMatches

         #region Next

         void Next()
         {
            ++mCurPos;
         }

         #endregion // Next

         #region ResolveParentReference

         /// <summary>Removes a reference to the parent directory (<see cref="Filesystem.Path.ParentDirectoryPrefix"/>) if possible.</summary>
         /// <returns><c>true</c> if the reference was removed, and <c>false</c> if it was kept.</returns>
         /// <remarks>This must be called *before* the reference to the parent directory has been added.</remarks>
         private bool ResolveParentReference()
         {
            Debug.Assert(NewComponentMatches(Filesystem.Path.ParentDirectoryPrefix));

            // Is the new reference the first directory component?
            if (mIndices.Count < 2)
            {
               // Do we have a root?
               if (HasRoot)
               {
                  // If so, we simply remove it, since the parent of the root
                  // is the root itself.
                  mBuilder.Length = mIndices[mIndices.Count - 1];
                  return true;
               }
               // Otherwise we cannot remove the parent reference,
               // so we keep it.
               return false;
            }

            // The new reference is not the first directory component.
            // We need to check the previous directory component.
            int start = mIndices[mIndices.Count - 2];
            int end = mIndices[mIndices.Count - 1] - 1;

            // Is the previous component also a parent reference?
            if (end - start == 2 && mBuilder[start] == '.' && mBuilder[start + 1] == '.')
            {
               // If so, we need to keep it.
               return false;
            }

            // otherwise, we remove the new reference, *and* the parent 
            // reference.
            mBuilder.Length = start;
            mIndices.RemoveAt(mIndices.Count - 1);
            return true;
         }

         #endregion // ResolveParentReference

         #region Skip

         void Skip(int num)
         {
            mCurPos += num;
         }

         #endregion // Skip
         
         #endregion // Private Utility Methods

         #region Public Properties

         #region ComponentIndices

         public List<int> ComponentIndices
         {
            get { return mIndices; }
         }

         #endregion // ComponentIndices

         #region HasRoot

         public bool HasRoot
         {
            get
            {
               // 2013-03-15: Yomodo; Raises OutOfRange Exception.
               //return mIndices[0] != 0;

               return mIndices.Count > 0 && mIndices[0] != 0;
            }
         }

         #endregion // HasRoot

         #region ExtensionIndex

         public int ExtensionIndex
         {
            get { return mExtensionPos; }
         }

         #endregion // ExtensionIndex

         #region Path

         public string Path
         {
            get { return mBuilder.ToString(); }
         }

         #endregion // Path

         #region Root

         public string Root
         {
            get
            {
               // 2013-03-15: Yomodo; Raises OutOfRange Exception.
               //return mBuilder.ToString().Substring(0, mIndices[0]);

               return (mIndices.Count == 0) ? null : mBuilder.ToString().Substring(0, mIndices[0]);
            }
         }

         #endregion // Root

         #endregion // Public Properties

         #region Private Fields

         bool mIsNotInternalDirectory = true;
         private readonly List<int> mIndices = new List<int>();
         private readonly StringBuilder mBuilder = new StringBuilder();
         private int mExtensionPos;
         private int mCurPos;
         private readonly string mPath;
         private readonly bool mAllowFileNameWildcards = true;
         private static readonly bool[] mInvalidFileNameCharsArray;

         #endregion // Private Fields
      }
   }
}
