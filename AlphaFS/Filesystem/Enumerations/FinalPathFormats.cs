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

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Used by Win32 API GetFinalPathNameByHandle()</summary>
   [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
   [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
   [Flags]
   public enum FinalPathFormats : uint
   {
      /// <summary>Return the normalized drive name. This is the default.</summary>
      FileNameNormalized = 0,

      /// <summary>Return the path with the drive letter. This is the default.</summary>
      VolumeNameDos = FileNameNormalized,

      /// <summary>Return the path with a volume GUID path instead of the drive name.</summary>
      VolumeNameGuid = 1,
      
      /// <summary>Return the path with the volume device path.</summary>
      VolumeNameNT = 2,

      /// <summary>Return the path with no drive information.</summary>
      VolumeNameNone = 4,

      /// <summary>Return the opened file name (not normalized).</summary>
      FileNameOpened = 8
   }
}