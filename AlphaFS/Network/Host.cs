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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Alphaleonis.Win32.Network
{
   /// <summary>Provides static methods to retrieve share-resource information from a local- or remote host.</summary>
   public static class Host
   {
      #region DFS
      
      #region EnumerateDfsLinks

      /// <summary>Enumerates the DFS Links from a DFS namespace.</summary>
      /// <param name="dfsName">A <see cref="string"/> that specifies the Universal Naming Convention (UNC) path of a DFS root or link.</param>
      /// <returns>Returns <see cref="IEnumerable{DfsInfo}"/> of DFS namespaces.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dfs")]
      [SecurityCritical]
      public static IEnumerable<DfsInfo> EnumerateDfsLinks(string dfsName)
      {
         if (string.IsNullOrEmpty(dfsName))
            throw new ArgumentNullException("dfsName");

         return EnumerateDfsResourcesInternal(NativeMethods.DfsInfoLevel.DfsInfo4, dfsName);
      }

      #endregion // EnumerateDfsLinks

      #region EnumerateDfsRoot

      /// <summary>Enumerates the DFS namespaces from the local host.</summary>
      /// <returns>Returns <see cref="IEnumerable{String}"/> of DFS Root namespaces from the local host.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDfsRoot()
      {
         return EnumerateDfsResourcesInternal(NativeMethods.DfsInfoLevel.DfsInfo300, Environment.MachineName).Select(dfs => dfs.EntryPath);
      }

      /// <summary>Enumerates the DFS namespaces from a remote host.</summary>
      /// <param name="host">A <see cref="String"/> that specifies the DNS or NetBIOS name of a remote host.</param>
      /// <returns>Returns <see cref="IEnumerable{String}"/> of DFS Root namespaces from a remote host.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDfsRoot(string host)
      {
         if (string.IsNullOrEmpty(host))
            throw new ArgumentNullException("host");

         return EnumerateDfsResourcesInternal(NativeMethods.DfsInfoLevel.DfsInfo300, host).Select(dfs => dfs.EntryPath);
      }

      #endregion // EnumerateDfsRoot

      #region EnumerateDomainDfsRoot

      /// <summary>Enumerates the DFS namespaces from the domain.</summary>
      /// <returns>Returns <see cref="IEnumerable{String}"/> of DFS Root namespaces from the domain.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDomainDfsRoot()
      {
         return EnumerateDfsResourcesInternal(NativeMethods.DfsInfoLevel.DfsInfo200, NativeMethods.ComputerDomain).Select(dfs => dfs.EntryPath);
      }

      /// <summary>Enumerates the DFS namespaces from a domain.</summary>
      /// <param name="domain">A <see cref="string"/> containing a domain name.</param>
      /// <returns>Returns <see cref="IEnumerable{String}"/> of DFS Root namespaces from a domain.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDomainDfsRoot(string domain)
      {
         if (string.IsNullOrEmpty(domain))
            throw new ArgumentNullException("domain");

         return EnumerateDfsResourcesInternal(NativeMethods.DfsInfoLevel.DfsInfo200, domain).Select(dfs => dfs.EntryPath);
      }

      #endregion // EnumerateDomainDfsRoot

      #region GetDfsClientInfo

      /// <summary>Retrieves information about a DFS root or link from the cache maintained by the DFS client.</summary>
      /// <param name="dfsName">A <see cref="string"/> that specifies the Universal Naming Convention (UNC) path of a DFS root or link.</param>
      /// <returns>Returns <see cref="IEnumerable{DfsInfo}"/></returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dfs")]
      [SecurityCritical]
      public static IEnumerable<DfsInfo> GetDfsClientInfo(string dfsName)
      {
         return GetDfsInfoInternal(true, dfsName, null, null);
      }

      /// <summary>Retrieves information about a DFS root or link from the cache maintained by the DFS client.</summary>
      /// <param name="dfsName">A <see cref="string"/> that specifies the Universal Naming Convention (UNC) path of a DFS root or link.</param>
      /// <param name="serverName">A <see cref="string"/> that specifies the name of the DFS root target or link target server.</param>
      /// <param name="shareName">A <see cref="string"/> that specifies the name of the share corresponding to the DFS root target or link target.</param>
      /// <returns>Returns <see cref="IEnumerable{DfsInfo}"/></returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dfs")]
      [SecurityCritical]
      public static IEnumerable<DfsInfo> GetDfsClientInfo(string dfsName, string serverName, string shareName)
      {
         return GetDfsInfoInternal(true, dfsName, serverName, shareName);
      }

      #endregion // GetDfsClientInfo

      #region GetDfsInfo

      /// <summary>Retrieves information about a specified DFS root or link in a DFS namespace.</summary>
      /// <param name="dfsName">A <see cref="string"/> that specifies the Universal Naming Convention (UNC) path of a DFS root or link.</param>
      /// <returns>Returns <see cref="IEnumerable{DfsInfo}"/></returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dfs")]
      [SecurityCritical]
      public static IEnumerable<DfsInfo> GetDfsInfo(string dfsName)
      {
         return GetDfsInfoInternal(false, dfsName, null, null);
      }

      #endregion // GetDfsInfo

      #endregion // DFS

      #region SMB

      #region EnumerateDrives

      /// <summary>Enumerate drives from the local host.</summary>
      /// <returns>Returns <see cref="IEnumerable{String}"/> drives from the local host.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDrives()
      {
         return EnumerateDriveResourcesInternal(null, false, 0, false).Cast<string>();
      }

      /// <summary>Enumerate drives from the specified host.</summary>
      /// <param name="host">A <see cref="String"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <returns>Returns <see cref="IEnumerable{String}"/> drives from the specified host.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDrives(string host)
      {
         return EnumerateDriveResourcesInternal(host, false, 0, false).Cast<string>();
      }

      #endregion // EnumerateDrives

      #region EnumerateShares

      /// <summary>Enumerate Server Message Block (SMB) shares from the local host.</summary>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the local host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares()
      {
         return EnumerateDriveResourcesInternal(null, true, ShareInfoLevel.ShareInfo503, false).Cast<ShareInfo>();
      }

      /// <summary>Enumerate Server Message Block (SMB) shares from the local host.</summary>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the local host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares(ShareInfoLevel shareInfoLevel)
      {
         return EnumerateDriveResourcesInternal(null, true, shareInfoLevel, false).Cast<ShareInfo>();
      }

      /// <summary>Enumerate Server Message Block (SMB) shares from the specified host.</summary>
      /// <param name="host">A <see cref="String"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the specified host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares(string host)
      {
         return EnumerateDriveResourcesInternal(host, true, ShareInfoLevel.ShareInfo503, false).Cast<ShareInfo>();
      }

      /// <summary>Enumerate Server Message Block (SMB) shares from the specified host.</summary>
      /// <param name="host">A <see cref="String"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the specified host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares(string host, ShareInfoLevel shareInfoLevel)
      {
         return EnumerateDriveResourcesInternal(host, true, shareInfoLevel, false).Cast<ShareInfo>();
      }

      #endregion // EnumerateShares

      #region GetHostShareFromPath

      /// <summary>Gets the host and Server Message Block (SMB) share name for a given unc path.</summary>
      /// <param name="uncPath">The share in the format: \\host\share</param>
      /// <returns>string[0] = host, string[1] = share;</returns>
      [SecurityCritical]
      public static string[] GetHostShareFromPath(string uncPath)
      {
         if (string.IsNullOrEmpty(uncPath))
            return null;

         // Get Host and Share.
         uncPath = uncPath.Replace(Path.LongPathUncPrefix, string.Empty);
         uncPath = uncPath.Replace(Path.UncPrefix, string.Empty);
         return uncPath.Split(Path.DirectorySeparatorChar);
      }

      #endregion // GetHostShareFromPath

      #region GetShareLocalPath

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the local host.</summary>
      /// <param name="uncPath">The share in the format: \\host\share</param>
      /// <returns>Returns the filesystem path for <paramref name="uncPath"/> or <see langref="null"/> on failure or when not available.</returns>
      /// <remarks>GetShareLocalPath() only works correctly for shares defined on the local host.</remarks>
      [SecurityCritical]
      public static string GetShareLocalPath(string uncPath)
      {
         return GetShareLocalPath(uncPath, ShareInfoLevel.ShareInfo2);
      }

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the local host.</summary>
      /// <param name="uncPath">The share in the format: \\host\share</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns the filesystem path for <paramref name="uncPath"/> or <see langref="null"/> on failure or when not available.</returns>
      /// <remarks>GetShareLocalPath() only works correctly for shares defined on the local host.</remarks>
      [SecurityCritical]
      public static string GetShareLocalPath(string uncPath, ShareInfoLevel shareInfoLevel)
      {
         string[] hostShare = GetHostShareFromPath(uncPath);
         return hostShare == null || !hostShare.Any() ? null : GetShareLocalPath(hostShare[0], hostShare[1], shareInfoLevel);
      }

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the host.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="share">A <see cref="string"/> that specifies the name of the Server Message Block (SMB) share.</param>
      /// <returns>Returns the filesystem path for <paramref name="host"/>\<paramref name="share"/> or <see langref="null"/> on failure or when not available.</returns>
      [SecurityCritical]
      public static string GetShareLocalPath(string host, string share)
      {
         return GetShareLocalPath(host, share, ShareInfoLevel.ShareInfo2);
      }

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the host.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="share">A <see cref="string"/> that specifies the name of the Server Message Block (SMB) share.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns the filesystem path for <paramref name="host"/>\<paramref name="share"/> or <see langref="null"/> on failure or when not available.</returns>
      [SecurityCritical]
      public static string GetShareLocalPath(string host, string share, ShareInfoLevel shareInfoLevel)
      {
         var shareInfo = GetShareInfoInternal(host, share, shareInfoLevel, false);
         return shareInfo == null
                   ? null
                   : (shareInfoLevel == ShareInfoLevel.ShareInfo2
                         ? ((NativeMethods.ShareInfo2) shareInfo).Path
                         : ((NativeMethods.ShareInfo503) shareInfo).Path);
      }

      #endregion // GetShareLocalPath

      #endregion // SMB

      #region GetUncName

      /// <summary>Return the host name in UNC format, for example: \\hostname</summary>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      [SecurityCritical]
      public static string GetUncName()
      {
         return string.Format(CultureInfo.CurrentCulture, "{0}{1}", Path.UncPrefix, Environment.MachineName);
      }

      #endregion // GetUncName

      #region Unified Internals

      #region DFS

      #region EnumerateDfsResourcesInternal

      /// <summary>Enumerates the DFS namespaces hosted on a server or DFS links of a namespace hosted by a server.</summary>
      /// <param name="level">The <see cref="NativeMethods.DfsInfoLevel"/> structure to use.</param>
      /// <param name="domainHostDfsName">A <see cref="string"/> that specifies a domain name, the DNS or NetBIOS name of a server or DFS namespace.</param>
      /// <returns>Returns <see cref="IEnumerable{DfsInfo}"/> of DFS namespaces.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SecurityCritical]
      internal static IEnumerable<DfsInfo> EnumerateDfsResourcesInternal(NativeMethods.DfsInfoLevel level, string domainHostDfsName)
      {
         if (string.IsNullOrEmpty(domainHostDfsName))
            throw new ArgumentNullException("domainHostDfsName");

         IEnumerable<DfsInfo> dfsInfoData = null;
         
         switch (level)
         {
            case NativeMethods.DfsInfoLevel.DfsInfo4:
               dfsInfoData = EnumerateDfsInternal(level, (NativeMethods.DfsInfo4 structure, SafeNetApiBuffer safeBuffer) =>
                                                         new DfsInfo(structure),
                  (NativeMethods.DfsInfoLevel lvl, out SafeNetApiBuffer safeBuffer, int prefMaxLen, out uint entriesRead, ref uint totalEntries, ref uint resumeHandle) =>
                  NativeMethods.NetDfsEnum(domainHostDfsName, lvl, prefMaxLen, out safeBuffer, out entriesRead, ref resumeHandle), false);
               break;

            // DFS_INFO_200: Parameter is the name of a domain.
            case NativeMethods.DfsInfoLevel.DfsInfo200:
               dfsInfoData = EnumerateDfsInternal(level, (NativeMethods.DfsInfo200 structure, SafeNetApiBuffer safeBuffer) =>
                                                         new DfsInfo { EntryPath = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}", Path.UncPrefix, NativeMethods.ComputerDomain, Path.DirectorySeparatorChar, structure.FtDfsName) },
                  (NativeMethods.DfsInfoLevel lvl, out SafeNetApiBuffer safeBuffer, int prefMaxLen, out uint entriesRead, ref uint totalEntries, ref uint resumeHandle) =>
                  NativeMethods.NetDfsEnum(domainHostDfsName, lvl, prefMaxLen, out safeBuffer, out entriesRead, ref resumeHandle), false);
               break;

            
            // DFS_INFO_300: Parameter is the name of a server.
            case NativeMethods.DfsInfoLevel.DfsInfo300:
               dfsInfoData = EnumerateDfsInternal(level, (NativeMethods.DfsInfo300 structure, SafeNetApiBuffer safeBuffer) => new DfsInfo { EntryPath = structure.DfsName },
                  (NativeMethods.DfsInfoLevel lvl, out SafeNetApiBuffer safeBuffer, int prefMaxLen, out uint entriesRead, ref uint totalEntries, ref uint resumeHandle) =>
                  NativeMethods.NetDfsEnum(domainHostDfsName, lvl, prefMaxLen, out safeBuffer, out entriesRead, ref resumeHandle), false);
               break;
         }

         if (dfsInfoData != null)
            foreach (DfsInfo dfsInfo in dfsInfoData)
               yield return dfsInfo;
      }

      #endregion // EnumerateDfsResourcesInternal

      #region EnumerateDfsInternal

      [SecurityCritical]
      internal static IEnumerable<TClass> EnumerateDfsInternal<TClass, TNative>(NativeMethods.DfsInfoLevel level, NativeMethods.CreateTDelegate<TClass, TNative> createTClass, NativeMethods.NetEnumDelegate netEnum, bool raiseException)
      {
         SafeNetApiBuffer safeBuffer = null;

         try
         {
            bool isDone = false;
            do
            {
               uint totalEntries = 0;
               uint entriesRead;
               uint resumeHandle = 0;

               uint lastError = netEnum(level, out safeBuffer, NativeMethods.MaxPreferredLength, out entriesRead, ref totalEntries, ref resumeHandle);

               switch (lastError)
               {
                  case Win32Errors.NO_ERROR:
                     for (long i = 0, objectSize = Marshal.SizeOf(typeof (TNative)),
                               itemOffset = safeBuffer.DangerousGetHandle().ToInt64(); i < entriesRead; i++, itemOffset += objectSize)
                     {
                        TNative info = (TNative)Marshal.PtrToStructure(new IntPtr(itemOffset), typeof(TNative));
                        
                        yield return createTClass(info, safeBuffer);
                     }
                     isDone = true;
                     break;

                  case Win32Errors.ERROR_MORE_DATA:
                     // Note that you must free the buffer even if the function fails with ERROR_MORE_DATA.
                     safeBuffer.Dispose();
                     break;

                  default:
                     if (raiseException)
                        NativeError.ThrowException(lastError);
                     isDone = true;
                     break;
               }
            } while (!isDone);
         }
         finally
         {
            if (safeBuffer != null)
               safeBuffer.Dispose();
         }
      }

      #endregion // EnumerateDfsInternal

      #region GetDfsInfoInternal

      /// <summary>Retrieves information about a specified DFS root or link in a DFS namespace.</summary>
      /// <param name="mode"><c>true</c> use <see cref="GetDfsClientInfo(string)"/>, <c>false</c> use <see cref="GetDfsInfo"/></param>
      /// <param name="dfsName">A <see cref="string"/> that specifies the Universal Naming Convention (UNC) path of a DFS root or link.</param>
      /// <param name="serverName">A <see cref="string"/> that specifies the name of the DFS root target or link target server. If <paramref name="mode"/> is <c>false</c>, this parameter is always null.</param>
      /// <param name="shareName">A <see cref="string"/> that specifies the name of the share corresponding to the DFS root target or link target. If <paramref name="mode"/> is <c>false</c>, this parameter is always null.</param>
      /// <returns>Returns <see cref="IEnumerable{DfsInfo}"/></returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dfs")]
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dfs")]
      [SecurityCritical]
      internal static IEnumerable<DfsInfo> GetDfsInfoInternal(bool mode, string dfsName, string serverName, string shareName)
      {
         if (string.IsNullOrEmpty(dfsName))
            throw new ArgumentNullException("dfsName");

         if (serverName != null && serverName.Trim() == string.Empty)
            serverName = null;

         if (shareName != null && shareName.Trim() == string.Empty)
            shareName = null;


         SafeNetApiBuffer safeBuffer;
         uint lastError = (mode)
                             ? NativeMethods.NetDfsGetClientInfo(dfsName, serverName, shareName, NativeMethods.DfsInfoLevel.DfsInfo4, out safeBuffer)
                             : NativeMethods.NetDfsGetInfo(      dfsName, null      , null     , NativeMethods.DfsInfoLevel.DfsInfo4, out safeBuffer);
                                                                          // These parameters are currently ignored and should be null.
         if (lastError != Win32Errors.NO_ERROR)
            NativeError.ThrowException(lastError);

         if (Filesystem.NativeMethods.IsValidHandle(safeBuffer))
            using (safeBuffer)
               yield return new DfsInfo(Filesystem.NativeMethods.GetStructure<NativeMethods.DfsInfo4>(0, safeBuffer.DangerousGetHandle()));
      }

      #endregion // GetDfsInfoInternal

      #endregion // DFS

      #region SMB

      #region EnumerateDriveResourcesInternal

      /// <summary>Enumerate Server Message Block (SMB) shares or drives from the specified host.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="getShares"><c>true</c> enumerates shares, <c>false</c> enumerates drives.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <param name="raiseException">If <c>true</c> raises Exceptions, when <c>false</c> no Exceptions are raised and the method returns <see langref="null"/>.</param>
      /// <returns>If <paramref name="getShares"/> is <c>true</c>, returns <see cref="IEnumerable{ShareInfo}"/> shares, when <c>false</c> returns <see cref="IEnumerable{String}"/> drives.</returns>
      /// <remarks>If <see cref="NativeMethods.ShareInfo503"/> fails, fallback with <see cref="NativeMethods.ShareInfo2"/> is executed. If this also fails an exception is raised.</remarks>
      [SecurityCritical]
      internal static IEnumerable<object> EnumerateDriveResourcesInternal(string host, bool getShares, ShareInfoLevel shareInfoLevel, bool raiseException)
      {
         if (host != null && host.Trim() == string.Empty)
            host = null;

         Type objectType = getShares
                              ? shareInfoLevel == ShareInfoLevel.ShareInfo2
                                   ? typeof (NativeMethods.ShareInfo2)
                                   : typeof (NativeMethods.ShareInfo503)
                              : typeof (IntPtr);

         uint lastError;
         uint resume = 0;
         //bool raiseException = false;
         bool isDone = false;

         do
         {
            uint entriesRead;
            uint totalEntries;

            // +2 is necessary for drives.
            int objectSize = Marshal.SizeOf(objectType) + (getShares ? 0 : 2);

            // Note: MAX_PREFERRED_LENGTH parameter is currently ignored for function: NetServerDiskEnum().

            SafeNetApiBuffer safeBuffer;
            lastError = getShares
                           ? NativeMethods.NetShareEnum(host, shareInfoLevel, out safeBuffer, NativeMethods.MaxPreferredLength, out entriesRead, out totalEntries, ref resume)
                           : NativeMethods.NetServerDiskEnum(host, shareInfoLevel, out safeBuffer, NativeMethods.MaxPreferredLength, out entriesRead, out totalEntries, ref resume);
            
            switch (lastError)
            {
               // Retrieved shares/drives.
               case Win32Errors.NO_ERROR:
                  if (entriesRead > 0)
                  {
                     for (long i = 0, itemOffset = safeBuffer.DangerousGetHandle().ToInt64(); i < entriesRead; i++, itemOffset += objectSize)
                        yield return getShares
                                        ? (object)
                                          new ShareInfo(host, shareInfoLevel, Marshal.PtrToStructure(new IntPtr(itemOffset), objectType))
                                        : Marshal.PtrToStringUni(new IntPtr(itemOffset));
                  }
                  isDone = true;
                  continue;

               case Win32Errors.ERROR_MORE_DATA:
                  // Note that you must free the buffer even if the function fails with ERROR_MORE_DATA.
                  safeBuffer.Dispose();
                  break;

               // Non-existent host.
               case Win32Errors.ERROR_BAD_NETPATH:
                  lastError = Win32Errors.ERROR_BAD_NET_NAME; // Throw more appropriate error.
                  raiseException = true;
                  isDone = true;
                  continue;

               // All other errors.
               default:
                  if (getShares)
                  {
                     // Fallback to ShareInfo2 structure. If that also fails: raise exception.
                     switch (shareInfoLevel)
                     {
                        case ShareInfoLevel.ShareInfo503:
                           shareInfoLevel = ShareInfoLevel.ShareInfo2;
                           objectType = typeof(NativeMethods.ShareInfo2);
                           break;

                        default:
                           raiseException = true;
                           isDone = true;
                           continue;
                     }
                  }
                  else
                  {
                     raiseException = true;
                     isDone = true;
                  }
                  break;
            }
         } while (!isDone);

         if (raiseException)
            NativeError.ThrowException(lastError, host);
      }

      #endregion // EnumerateDriveResourcesInternal

      #region GetRemoteNameInfoInternal

      /// <summary>This method uses <see cref="NativeMethods.RemoteNameInfo"/> level to retieve full REMOTE_NAME_INFO structure.</summary>
      /// <param name="path">The local path with drive name.</param>
      /// <returns>A <see cref="NativeMethods.RemoteNameInfo"/> structure.</returns>
      /// <exception cref="System.IO.PathTooLongException">When <paramref name="path"/> exceeds <see cref="Filesystem.NativeMethods.MaxPath"/></exception>
      /// <remarks>AlphaFS regards network drives created using SUBST.EXE as invalid: http://alphafs.codeplex.com/discussions/316583</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
      [SecurityCritical]
      internal static NativeMethods.RemoteNameInfo GetRemoteNameInfoInternal(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // If path already is a network share path, we fill the RemoteNameInfo structure ourselves.
         if (Path.IsUnc(path))
            return new NativeMethods.RemoteNameInfo
            {
               UniversalName = Path.DirectorySeparatorAdd(path, false),
               ConnectionName = Path.DirectorySeparatorRemove(path, false),
               RemainingPath = Path.DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture)
            };

         path = Path.GetRegularPath(path);

         if (path.Length > Filesystem.NativeMethods.MaxPath)
            throw new PathTooLongException();


         // Start with a large buffer to prevent multiple calls.
         uint bufferSize = 1024;
         SafeGlobalMemoryBufferHandle safeBuffer = new SafeGlobalMemoryBufferHandle((int)bufferSize);

         try
         {
            do
            {
               // Structure: UNIVERSAL_NAME_INFO_LEVEL = 1 (not used in AlphaFS).
               // Structure: REMOTE_NAME_INFO_LEVEL    = 2
               uint lastError = NativeMethods.WNetGetUniversalName(path, 2, safeBuffer, out bufferSize);

               switch (lastError)
               {
                  case Win32Errors.NO_ERROR:
                     return Filesystem.NativeMethods.GetStructure<NativeMethods.RemoteNameInfo>(0, safeBuffer.DangerousGetHandle());

                  case Win32Errors.ERROR_MORE_DATA:
                     safeBuffer.Dispose();
                     safeBuffer = new SafeGlobalMemoryBufferHandle((int)bufferSize);
                     break;

                  // The device specified by the lpLocalPath parameter is not redirected.
                  case Win32Errors.ERROR_NOT_CONNECTED:

                  // -None of the network providers recognize the local name as having a connection.
                  //  However, the network is not available for at least one provider to whom the connection may belong.
                  // -A Softgrid "Q:" drive is encountered;
                  case Win32Errors.ERROR_NO_NET_OR_BAD_PATH:

                     // Return an empty structure (all fields set to null).
                     return new NativeMethods.RemoteNameInfo();

                  default:
                     NativeError.ThrowException(lastError, path);
                     break;
               }
            } while (true);
         }
         finally
         {
            safeBuffer.Dispose();
         }
      }

      #endregion // GetRemoteNameInfoInternal

      #region GetShareInfoInternal

      /// <summary>Gets the ShareInfo structure of a Server Message Block (SMB) share.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="share">A <see cref="string"/> that specifies the name of the Server Message Block (SMB) share.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <param name="raiseException">If <c>true</c> raises Exceptions, when <c>false</c> no Exceptions are raised and the method returns <see langref="null"/>.</param>
      /// <returns>A <see cref="ShareInfo"/> structure, or <see langref="null"/> on failure or when not available.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
      [SecurityCritical]
      internal static object GetShareInfoInternal(string host, string share, ShareInfoLevel shareInfoLevel, bool raiseException)
      {
         if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(share))
            return null;

         SafeNetApiBuffer safeBuffer;

         uint lastError = NativeMethods.NetShareGetInfo(host, share, shareInfoLevel, out safeBuffer);
         if (lastError != Win32Errors.NO_ERROR)
            if (raiseException)
               NativeError.ThrowException(lastError, host, share);
            else
               return null;

         switch (shareInfoLevel)
         {
            case ShareInfoLevel.ShareInfo1005:
               return Filesystem.NativeMethods.GetStructure<NativeMethods.ShareInfo1005>(0, safeBuffer.DangerousGetHandle());

            case ShareInfoLevel.ShareInfo503:
               return Filesystem.NativeMethods.GetStructure<NativeMethods.ShareInfo503>(0, safeBuffer.DangerousGetHandle());

            case ShareInfoLevel.ShareInfo2:
               return Filesystem.NativeMethods.GetStructure<NativeMethods.ShareInfo2>(0, safeBuffer.DangerousGetHandle());

            default:
               return null;
         }
      }

      #endregion // GetShareInfoInternal

      #endregion // SMB

      #endregion // Unified Internals
   }
}