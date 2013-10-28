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

namespace Alphaleonis.Win32.Network
{
   /// <summary>State of the target.</summary>
   /// <remarks>When this structure is returned as a result of calling the NetDfsGetInfo function, this member can be one of the following values.</remarks>
   [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
   [Flags]
   public enum DfsStorageStates
   {
      /// <summary></summary>
      None = 0,

      /// <summary>DFS_STORAGE_STATE_OFFLINE (0x00000001) - The DFS root or link target is offline.</summary>
      Offline = 1,

      /// <summary>DFS_STORAGE_STATE_ONLINE (0x00000002) - The DFS root or link target is online.</summary>
      Online = 2,

      /// <summary>DFS_STORAGE_STATE_ACTIVE (0x00000004) - The DFS root or link target is the active target.</summary>
      /// <remarks>
      /// When this structure is returned as a result of calling the NetDfsGetClientInfo function, the <see cref="DfsStorageStates.Online"/> state is set by default.
      /// If the target is the active target in the DFS client cache, the following value is logically combined with the default value via the OR operator.
      /// </remarks>
      Active = 4
   }
}