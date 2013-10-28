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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Alphaleonis.Win32.Network
{
   internal static partial class NativeMethods
   {
      /// <summary>DFS_INFO_4 - Contains information about a Distributed File System (DFS) root or link.
      /// This structure contains the name, status, number of DFS targets, and information about each target of the root or link. 
      /// </summary>
      /// <remarks>A DFS_INFO_4 structure contains one or more <see cref="DfsStorageInfo"/> structures, one for each DFS target.</remarks>
      /// <remarks>This structure is only for use with the NetDfsEnum, NetDfsGetClientInfo, and NetDfsGetInfo functions.</remarks>
      /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct DfsInfo4
      {
         /// <summary>A <see cref="String"/> that specifies the Universal Naming Convention (UNC) path of a DFS root or link.</summary>
         [MarshalAs(UnmanagedType.LPWStr)] public readonly string EntryPath;

         /// <summary>A <see cref="String"/> that contains a comment associated with the DFS root or link.</summary>
         [MarshalAs(UnmanagedType.LPWStr)] public readonly string Comment;

         /// <summary>A <see cref="DfsVolumeStates"/> that specifies a set of bit flags that describe the DFS root or link.</summary>
         [MarshalAs(UnmanagedType.U4)] public readonly DfsVolumeStates State;

         /// <summary>Specifies the time-out, in seconds, of the DFS root or link.</summary>
         [MarshalAs(UnmanagedType.U4)] public readonly uint Timeout;

         /// <summary>Specifies the GUID of the DFS root or link.</summary>
         public readonly Guid Guid;

         /// <summary>Specifies the number of DFS targets.</summary>
         [MarshalAs(UnmanagedType.U4)] public readonly uint NumberOfStorages;

         /// <summary>An array of <see cref="DfsStorageInfo"/> structures. The NumberOfStorages member specifies the number of structures in the array.</summary>
         public IntPtr Storage;
      }
   }
}