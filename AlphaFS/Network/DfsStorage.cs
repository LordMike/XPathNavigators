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
using System.Diagnostics.CodeAnalysis;

namespace Alphaleonis.Win32.Network
{
   /// <summary>DFS_STORAGE_INFO - Contains information about a DFS root or link target in a DFS namespace or from the cache maintained by the DFS client.
   /// Information about a DFS root or link target in a DFS namespace is retrieved by calling the NetDfsGetInfo function.
   /// Information about a DFS root or link target from the cache maintained by the DFS client is retrieved by calling the NetDfsGetClientInfo function.
   /// </summary>
   [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
   public sealed class DfsStorage
   {
      #region Constructor

      /// <summary>DFS_STORAGE_INFO - Contains information about a DFS root or link target in a DFS namespace or from the cache maintained by the DFS client.</summary>
      internal DfsStorage(NativeMethods.DfsStorageInfo structure)
      {
         ServerName = structure.ServerName;
         ShareName = structure.ShareName;
         State = structure.State;
      }

      #endregion // Constructor

      #region Properties

      #region ServerName

      /// <summary>A <see cref="string"/> that specifies the DFS root target or link target server name.</summary>
      public string ServerName { get; private set; }

      #endregion // ServerName

      #region ShareName

      /// <summary>A <see cref="string"/> that specifies the DFS root target or link target share name.</summary>
      public string ShareName { get; private set; }

      #endregion // ShareName

      #region State

      /// <summary>State of the target.</summary>
      public DfsStorageStates State { get; private set; }

      #endregion // State

      #endregion // Properties
   }
}