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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC).</summary>
   [Serializable]
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
   internal struct FileTime
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

         FileTime other = obj is FileTime ? (FileTime)obj : new FileTime();

         return (other.HighDateTime.Equals(HighDateTime) &&
                other.LowDateTime.Equals(LowDateTime));
      }

      #endregion // Equals

      #region GetHashCode

      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>A hash code for the current Object.</returns>
      public override int GetHashCode()
      {
         unchecked
         {
            int hash = 17;
            hash = hash * 23 + HighDateTime.GetHashCode();
            hash = hash * 23 + LowDateTime.GetHashCode();
            return hash;
         }
      }

      #endregion // GetHashCode

      #region ==

      /// <summary>Implements the operator ==</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(FileTime left, FileTime right)
      {
         return left.Equals(right);
      }

      #endregion // ==

      #region !=

      /// <summary>Implements the operator !=</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(FileTime left, FileTime right)
      {
         return !(left == right);
      }

      #endregion // !=

      #endregion // .NET

      /// <summary>Converts a value to long.</summary>
      public static implicit operator long(FileTime ft)
      {
         return ft.ToLong();
      }

      /// <summary>Converts a value to long.</summary>
      [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
      public long ToLong()
      {
         return NativeMethods.ToLong(HighDateTime, LowDateTime);
      }

      ///// <summary>Converts a value to long.</summary>
      //public DateTime AsDateTime()
      //{
      //   return DateTime.FromFileTime(ToLong());
      //}

      #endregion // Class Internal Affairs

      #region Fields

      private readonly uint lowDateTime;
      private readonly uint highDateTime;

      #endregion // Fields

      #region Properties

      /// <summary>The high-order part of the file time.</summary>
      private uint HighDateTime
      {
         get { return highDateTime; }
      }

      /// <summary>The low-order part of the file time.</summary>
      private uint LowDateTime
      {
         get { return lowDateTime; }
      }

      #endregion // Properties
   }
}