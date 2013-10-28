﻿/* Copyright (c) 2008-2012 Peter Palotas, Alexandr Normuradov
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

namespace Alphaleonis.Win32.Filesystem
{
   internal static partial class NativeMethods
   {
      /// <summary>Defines values that are used with the FindFirstFileEx function to specify the type of filtering to perform.</summary>
      internal enum FindExSearchOps
      {
         /// <summary>The search for a file that matches a specified file name.</summary>
         SearchNameMatch,

         /// <summary>This is an advisory flag. If the file system supports directory filtering, the function searches for a file that matches
         /// the specified name and is also a directory. If the file system does not support directory filtering, this flag is silently ignored. 
         /// </summary>
         /// <remarks>
         /// The lpSearchFilter parameter of the FindFirstFileEx function must be NULL when this search value is used.
         /// If directory filtering is desired, this flag can be used on all file systems, but because it is an advisory flag and only affects file systems that support it,
         /// the application must examine the file attribute data stored in the lpFindFileData parameter of the FindFirstFileEx function to determine whether the function has returned a handle to a directory.
         /// </remarks>
         SearchLimitToDirectories,

         /// <summary>This filtering type is not available.</summary>
         SearchLimitToDevices
      }
   }
}