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
using System.Diagnostics.CodeAnalysis;
using Alphaleonis.Win32.Filesystem;

namespace Alphaleonis.Win32.Network
{
   /// <summary>DFS_INFO_4 - Contains information about a Distributed File System (DFS) root or link.
   /// This structure contains the name, status, GUID, time-out, number of targets, and information about each target of the root or link.
   /// </summary>
   /// <remarks>A DFS_INFO_4 structure contains one or more <see cref="NativeMethods.DfsStorageInfo"/> structures, one for each DFS target.</remarks>
   /// <remarks>This structure is only for use with the NetDfsEnum, NetDfsGetClientInfo, and NetDfsGetInfo functions.</remarks>
   /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
   /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
   [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
   public sealed class DfsInfo
   {
      #region Constructor

      /// <summary></summary>
      public DfsInfo()
      {
      }

      internal DfsInfo(NativeMethods.DfsInfo4 structure)
         : this(null, structure)
      {
      }

      internal DfsInfo(KernelTransaction transaction, NativeMethods.DfsInfo4 structure)
      {
         Transaction = transaction;
         Comment = structure.Comment;
         EntryPath = structure.EntryPath;
         State = structure.State;
         Timeout = structure.Timeout;
         Guid = structure.Guid;

         _numberOfStorages = new List<DfsStorage>();
         for (int i = 0; i < structure.NumberOfStorages; i++)
            _numberOfStorages.Add(new DfsStorage(Filesystem.NativeMethods.GetStructure<NativeMethods.DfsStorageInfo>(i, structure.Storage)));
      }

      #endregion // Constructor

      #region Comment

      /// <summary>A <see cref="String"/> that contains a comment associated with the DFS root or link.</summary>
      public string Comment { get; internal set; }

      #endregion // Comment

      #region EntryPath

      /// <summary>A <see cref="String"/> that specifies the Universal Naming Convention (UNC) path of a DFS root or link.</summary>
      public string EntryPath { get; internal set; }

      #endregion // EntryPath

      #region NumberOfStorages

      private readonly List<DfsStorage> _numberOfStorages;

      /// <summary>Specifies the number of DFS targets.</summary>
      public IEnumerable<DfsStorage> NumberOfStorages
      {
         get { return _numberOfStorages; }
      }

      #endregion // NumberOfStorages

      #region State

      /// <summary>A <see cref="DfsVolumeStates"/> that specifies a set of bit flags that describe the DFS root or link.</summary>
      public DfsVolumeStates State { get; internal set; }

      //DfsVolumeStates flavorBits = (structure3.State & (DfsVolumeStates) DfsNamespaceFlavors.All);
      //If (flavorBits == DFS_VOLUME_FLAVOR_STANDALONE)     // Namespace is stand-alone DFS.
      //else if (flavorBits == DFS_VOLUME_FLAVOR_AD_BLOB)   // Namespace is AD Blob.
      //else StateBits = (Flavor & DFS_VOLUME_STATES)        // Unknown flavor.
      // StateBits can be one of the following: 
      //  (DFS_VOLUME_STATE_OK, DFS_VOLUME_STATE_INCONSISTENT, 
      //   DFS_VOLUME_STATE_OFFLINE or DFS_VOLUME_STATE_ONLINE)
      //State = flavorBits | structure3.State;

      #endregion // State

      #region Timeout

      /// <summary>Specifies the time-out, in seconds, of the DFS root or link.</summary>
      public ulong Timeout { get; internal set; }

      #endregion // Timeout

      #region Guid

      /// <summary>Specifies the GUID of the DFS root or link.</summary>
      public Guid Guid { get; internal set; }

      #endregion // Guid

      #region AlphaFS

      #region DirectoryInfo

      private DirectoryInfo _directoryInfo;

      /// <summary>The <see cref="DirectoryInfo"/> instance for this DfsInfo instance.</summary>
      public DirectoryInfo DirectoryInfo
      {
         get
         {
            if (_directoryInfo == null)
               _directoryInfo = new DirectoryInfo(Transaction, EntryPath);

            return _directoryInfo;
         }
      }

      #endregion // DirectoryInfo

      #region Transaction

      /// <summary>The transaction.</summary>
      internal KernelTransaction Transaction { get; set; }

      #endregion // Transaction

      #endregion // AlphaFS
   }
}