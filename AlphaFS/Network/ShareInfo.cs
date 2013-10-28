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
using System.Globalization;
using System.Security;

namespace Alphaleonis.Win32.Network
{
   /// <summary>Contains information about Server Message Block (SMB) shares. This class cannot be inherited.</summary>
   [Serializable]
   [SecurityCritical]
   public class ShareInfo
   {
      #region Constructor
      
      #region ShareInfo

      /// <summary>Create a ShareInfo instance. Properties originate from <paramref name="shareInfoLevel"/> structure.</summary>
      /// <param name="host">A computer host to retrieve shares from.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <param name="shareInfo">A ShareInfoXxx structure instance.</param>
      internal ShareInfo(string host, ShareInfoLevel shareInfoLevel, object shareInfo)
      {
         switch (shareInfoLevel)
         {
            case ShareInfoLevel.ShareInfo503:
               NativeMethods.ShareInfo503 s503 = (NativeMethods.ShareInfo503)shareInfo;
               CurrentUses = s503.CurrentUses;
               MaxUses = s503.MaxUses;
               NetName = s503.NetName;
               Password = s503.Password;
               Path = s503.Path;
               Permissions = s503.Permissions;
               Remark = s503.Remark;
               SecurityDescriptor = s503.SecurityDescriptor;
               ServerName = s503.ServerName == "*" ? host ?? Environment.MachineName : s503.ServerName;
               ShareType = s503.ShareType;
               break;

            case ShareInfoLevel.ShareInfo2:
               NativeMethods.ShareInfo2 s2 = (NativeMethods.ShareInfo2)shareInfo;
               CurrentUses = s2.CurrentUses;
               MaxUses = s2.MaxUses;
               NetName = s2.NetName;
               Password = s2.Password;
               Path = s2.Path;
               Permissions = s2.Permissions;
               Remark = s2.Remark;
               ServerName = host ?? Environment.MachineName;
               ShareType = s2.ShareType;
               break;
         }

         ResourceType = ((NativeMethods.ShareInfo1005)Host.GetShareInfoInternal(ServerName, NetName, ShareInfoLevel.ShareInfo1005, true)).ResourceType;
      }

      #endregion // ShareInfo

      #endregion // Constructor

      #region Methods

      #region ToString

      /// <summary>Returns the full path to the share.</summary>
      public override string ToString()
      {
         return string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}", Filesystem.Path.UncPrefix, ServerName, Filesystem.Path.DirectorySeparatorChar, NetName);
      }

      #endregion // ToString

      #endregion // Methods

      #region Properties

      #region CurrentUses

      /// <summary>The number of current connections to the resource.</summary>
      public uint CurrentUses { get; private set; }

      #endregion // CurrentUses

      #region FullPath

      /// <summary>Returns the full path to the share.</summary>
      public string FullPath
      {
         get { return ToString(); }
      }

      #endregion // FullPath

      #region IsLocal

      /// <summary>Check if the share is defined on the local host.</summary>
      /// <remarks>Use <see cref="NativeMethods.ShareInfo503"/> structure to determine if the share is defined on the local host.</remarks>
      public bool IsLocal
      {
         get { return Host.GetShareInfoInternal(ServerName, NetName, ShareInfoLevel.ShareInfo503, false) != null; }
      }

      #endregion // IsLocal

      #region MaxUses

      /// <summary>The maximum number of concurrent connections that the shared resource can accommodate.</summary>
      /// <remarks>The number of connections is unlimited if the value specified in this member is –1.</remarks>
      public uint MaxUses { get; private set; }

      #endregion // MaxUses

      #region NetName

      /// <summary>The name of a shared resource.</summary>
      public string NetName { get; private set; }

      #endregion // NetName

      #region Password

      /// <summary>The share's password (when the server is running with share-level security).</summary>
      public string Password { get; private set; }

      #endregion // Password

      #region Path

      /// <summary>The local path for the shared resource.</summary>
      /// <remarks>For disks, this member is the path being shared. For print queues, this member is the name of the print queue being shared.</remarks>
      public string Path { get; private set; }

      #endregion // Path

      #region Permissions

      /// <summary>The shared resource's permissions for servers running with share-level security.</summary>
      public uint Permissions { get; private set; }

      #endregion // Permissions

      #region Remark

      /// <summary>An optional comment about the shared resource.</summary>
      public string Remark { get; private set; }

      #endregion // Remark

      #region ResourceType

      private ShareInfoTypes _resourceType;

      /// <summary>SHARE_INFO_1005 - Contains information about the shared resource.</summary>
      public ShareInfoTypes ResourceType
      {
         get { return _resourceType; }
         private set { _resourceType = value; }
      }

      #endregion // ResourceType

      #region SecurityDescriptor

      /// <summary>Specifies the SECURITY_DESCRIPTOR associated with this share.</summary>
      public IntPtr SecurityDescriptor { get; private set; }

      #endregion // SecurityDescriptor

      #region ServerName

      /// <summary>A pointer to a string that specifies the DNS or NetBIOS name of the remote server on which the shared resource resides.</summary>
      /// <remarks>A value of "*" indicates no configured server name.</remarks>
      public string ServerName { get; private set; }

      #endregion // ServerName
      
      #region ShareType

      /// <summary>The type of share.</summary>
      public ShareTypes ShareType { get; private set; }

      #endregion // ShareType

      #endregion // Properties
   }
}