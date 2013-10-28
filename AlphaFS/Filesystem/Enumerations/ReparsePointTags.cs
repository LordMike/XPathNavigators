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

namespace Alphaleonis.Win32.Filesystem
{
   internal static partial class NativeMethods
   {
      /// <summary>Enumeration specifying the different reparse point tags.</summary>
      internal enum ReparsePointTags : uint
      {
         /// <summary>The entry is not a reparse point.</summary>
         None = 0,

         /// <summary>IO_REPARSE_TAG_DFS</summary>
         Dfs = 2147483658,

         /// <summary>IO_REPARSE_TAG_DFSR</summary>
         Dfsr = 2147483666,

         /// <summary>IO_REPARSE_TAG_MOUNT_POINT - Used for mount point support.</summary>
         MountPoint = 2684354563,

         /// <summary>IO_REPARSE_TAG_SYMLINK - Used for symbolic link support.</summary>
         SymLink = 2684354572
      }
   }
}