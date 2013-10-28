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
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;

namespace Alphaleonis.Win32.Network
{
   internal static partial class NativeMethods
   {
      #region Constants

      /// <summary>MAX_PREFERRED_LENGTH = -1: If you specify MAX_PREFERRED_LENGTH, the function allocates the amount of memory required for the data.</summary>
      internal const int MaxPreferredLength = -1;

      #endregion // Constants

      #region Delegates
      
      internal delegate TClass CreateTDelegate<TClass, TNative>(TNative structure, SafeNetApiBuffer ptr);
      internal delegate uint NetEnumDelegate(DfsInfoLevel level, out SafeNetApiBuffer netApiBuffer, int prefMaxLen, out uint entriesRead, ref uint totalEntries, ref uint resumeHandle);

      #endregion Delegates

      #region GetComputerDomain

      internal static readonly string ComputerDomain = GetComputerDomain();

      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      internal static string GetComputerDomain(bool fdqn = false)
      {
         string domain = Environment.UserDomainName;
         string machine = Environment.MachineName.ToUpper(CultureInfo.CurrentCulture);

         try
         {
            if (fdqn)
            {
               domain = Dns.GetHostEntry("LocalHost").HostName.ToUpper(CultureInfo.CurrentCulture).Replace(machine + ".", "");
               domain = domain.Replace(machine, "");
            }
         }
         catch
         {
         }

         return domain;
      }

      #endregion // GetComputerDomain

      #region mpr.dll

      #region WNetGetUniversalName

      /// <summary>The WNetGetUniversalName function takes a drive-based path for a network resource and returns an information structure that contains a more universal form of the name.</summary>
      /// <returns>
      /// If the function succeeds, the return value is <see cref="Win32Errors.NO_ERROR"/>
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("mpr.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "WNetGetUniversalNameW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint WNetGetUniversalName([MarshalAs(UnmanagedType.LPWStr)] string lpLocalPath, [MarshalAs(UnmanagedType.U4)] uint dwInfoLevel, SafeGlobalMemoryBufferHandle lpBuffer, [MarshalAs(UnmanagedType.U4)] out uint lpBufferSize);

      #endregion // WNetGetUniversalName

      #endregion // mpr.dll

      #region netapi32.dll

      #region NetApiBufferFree

      /// <summary>The NetApiBufferFree function frees the memory that the NetApiBufferAllocate function allocates.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetApiBufferFree")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetApiBufferFree(IntPtr buffer);

      #endregion // NetApiBufferFree

      #region NetDfsEnum

      /// <summary>Enumerates the Distributed File System (DFS) namespaces hosted on a server or DFS links of a namespace hosted by a server.</summary>
      /// <remarks>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetDfsEnum")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetDfsEnum([MarshalAs(UnmanagedType.LPWStr)] string dfsName, [MarshalAs(UnmanagedType.U4)] DfsInfoLevel level, [MarshalAs(UnmanagedType.U4)] int prefMaxLen, out SafeNetApiBuffer buffer, [MarshalAs(UnmanagedType.U4)] out uint entriesRead, [MarshalAs(UnmanagedType.U4)] ref uint resumeHandle);

      #endregion // NetDfsEnum

      #region NetDfsGetClientInfo

      /// <summary>Retrieves information about a Distributed File System (DFS) root or link from the cache maintained by the DFS client.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetDfsGetClientInfo")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetDfsGetClientInfo([MarshalAs(UnmanagedType.LPWStr)] string dfsEntryPath, [MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.LPWStr)] string shareName, DfsInfoLevel level, out SafeNetApiBuffer buffer);

      #endregion // NetDfsGetClientInfo

      #region NetDfsGetInfo

      /// <summary>Retrieves information about a specified Distributed File System (DFS) root or link in a DFS namespace.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetDfsGetInfo")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetDfsGetInfo([MarshalAs(UnmanagedType.LPWStr)] string dfsEntryPath, [MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.LPWStr)] string shareName, DfsInfoLevel level, out SafeNetApiBuffer buffer);

      #endregion // NetDfsGetInfo

      #region NetServerDiskEnum

      /// <summary>The NetServerDiskEnum function retrieves a list of disk drives on a server.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>The function returns an array of three-character strings (a drive letter, a colon, and a terminating null character).</remarks>
      /// <remarks>Only members of the Administrators or Server Operators local group can successfully execute the NetServerDiskEnum function on a remote computer.</remarks>
      /// <remarks>Minimum supported client: Windows 2000 Professional [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetServerDiskEnum")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetServerDiskEnum([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.U4)] ShareInfoLevel level, out SafeNetApiBuffer bufPtr, [MarshalAs(UnmanagedType.I4)] int prefMaxLen, [MarshalAs(UnmanagedType.U4)] out uint entriesRead, [MarshalAs(UnmanagedType.U4)] out uint totalEntries, [MarshalAs(UnmanagedType.U4)] ref uint resumeHandle);

      #endregion // NetServerDiskEnum

      #region NetShareEnum

      /// <summary>Retrieves information about each (hidden) Server Message Block (SMB) resource/share on a server.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>
      /// For interactive users (users who are logged on locally to the machine), no special group membership is required to execute the NetShareEnum function.
      /// For non-interactive users, Administrator, Power User, Print Operator, or Server Operator group membership is required to successfully execute
      /// the NetShareEnum function at levels 2, 502, and 503. No special group membership is required for level 0 or level 1 calls.
      /// </remarks>
      /// <remarks>
      /// Windows Server 2003 and Windows XP: For all users, Administrator, Power User, Print Operator,
      /// or Server Operator group membership is required to successfully execute the NetShareEnum function at levels 2 and 502.
      /// </remarks>
      /// <remarks>
      /// You can also use the WNetEnumResource function to retrieve resource information.
      /// However, WNetEnumResource does not enumerate hidden shares or users connected to a share.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetShareEnum")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetShareEnum([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.U4)] ShareInfoLevel level, out SafeNetApiBuffer bufPtr, [MarshalAs(UnmanagedType.I4)] int prefMaxLen, [MarshalAs(UnmanagedType.U4)] out uint entriesRead, [MarshalAs(UnmanagedType.U4)] out uint totalEntries, [MarshalAs(UnmanagedType.U4)] ref uint resumeHandle);

      #endregion // NetShareEnum
      
      #region NetShareGetInfo

      /// <summary>Retrieves information about a particular Server Message Block (SMB) shared resource on a server.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetShareGetInfo")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetShareGetInfo([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.LPWStr)] string netName, [MarshalAs(UnmanagedType.U4)] ShareInfoLevel level, out SafeNetApiBuffer lpBuffer);

      #endregion // NetShareGetInfo

      #endregion // netapi32.dll
   }
}