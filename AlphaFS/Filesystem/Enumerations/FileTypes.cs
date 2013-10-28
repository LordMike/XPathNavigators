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

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Retrieves the file type of the specified file.</summary>
   /// <remarks>Used by GetFileType() function.</remarks>
   [Flags]
   public enum FileTypes
   {
      /// <summary>Either the type of the specified file is unknown, or the function failed.</summary>
      None = 0,

      /// <summary>The specified file is a disk file.</summary>
      DiskFile = 1,

      /// <summary>The specified file is a character file, typically an LPT device or a console.</summary>
      CharacterFile = 2,

      /// <summary>The specified file is a socket, a named pipe, or an anonymous pipe.</summary>
      Pipe = 3,

      /// <summary>The specified file is a remote file.</summary>
      Remote = 32768
   }
}