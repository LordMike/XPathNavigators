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
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Alphaleonis.Win32.Security
{
   internal static partial class NativeMethods
   {
      #region GetAccessControlInternal

      /// <summary>Unified method GetAccessControlInternal() to get/set an <see cref="ObjectSecurity"/> for a particular directory or file.</summary>
      /// <param name="isFolder"><c>true</c> indicates a folder object, <c>false</c> indicates a file object.</param>
      /// <param name="path">The path to a directory containing a <see cref="DirectorySecurity"/> object that describes the directory's or file's access control list (ACL) information.</param>
      /// <param name="includeSections">One (or more) of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to receive.</param>
      /// <returns>An <see cref="ObjectSecurity"/> object that encapsulates the access control rules for the directory or file described by the <paramref name="path"/> parameter. </returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      [SecurityCritical]
      internal static ObjectSecurity GetAccessControlInternal(bool isFolder, string path, AccessControlSections includeSections)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // 2012-10-19: Yomodo; GetFileSecurity() seems to perform better than GetNamedSecurityInfo() and doesn't require Administrator rights.

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);


         SecurityInformation securityInfo = 0;
         PrivilegeEnabler privilegeEnabler = null;
         SafeGlobalMemoryBufferHandle buffer = null;
         ObjectSecurity objectSecurity;

         try
         {
            if ((includeSections & AccessControlSections.Access) != 0)
               securityInfo |= SecurityInformation.Dacl;

            if ((includeSections & AccessControlSections.Audit) != 0)
            {
               // We need the SE_SECURITY_NAME privilege enabled to be able to get the
               // SACL descriptor. So we enable it here for the remainder of this function.
               privilegeEnabler = new PrivilegeEnabler(Privilege.Security);
               securityInfo |= SecurityInformation.Sacl;
            }

            if ((includeSections & AccessControlSections.Group) != 0)
               securityInfo |= SecurityInformation.Group;
            
            if ((includeSections & AccessControlSections.Owner) != 0)
               securityInfo |= SecurityInformation.Owner;


            uint sizeRequired;
            buffer = new SafeGlobalMemoryBufferHandle(512);

            bool gotSecurityOk = GetFileSecurity(pathLp, securityInfo, buffer, (uint) buffer.Capacity, out sizeRequired);
            int lastError = Marshal.GetLastWin32Error();

            if (!gotSecurityOk)
            {
               // A larger buffer is required to store the descriptor; increase size and try again.
               if (sizeRequired > buffer.Capacity)
               {
                  buffer.Dispose();
                  using (buffer = new SafeGlobalMemoryBufferHandle((int)sizeRequired))
                  {
                     gotSecurityOk = GetFileSecurity(pathLp, securityInfo, buffer, (uint)buffer.Capacity, out sizeRequired);
                     lastError = Marshal.GetLastWin32Error();
                  }
               }
            }

            if (!gotSecurityOk)
               NativeError.ThrowException(lastError, pathLp);

            objectSecurity = (isFolder) ? (ObjectSecurity)new DirectorySecurity() : new FileSecurity();
            objectSecurity.SetSecurityDescriptorBinaryForm(buffer.ToByteArray(0, buffer.Capacity));
         }
         finally
         {
            if (buffer != null)
               buffer.Dispose();

            if (privilegeEnabler != null)
               privilegeEnabler.Dispose();
         }

         return objectSecurity;
      }

      #endregion // GetAccessControlInternal

      #region SetAccessControlInternal

      /// <summary>Unified method SetAccessControlInternal() applies access control list (ACL) entries described by a <see cref="FileSecurity"/> FileSecurity object to the specified file.</summary>
      /// <param name="path">A file to add or remove access control list (ACL) entries from. This parameter may be <see langword="null"/>.</param>
      /// <param name="handle">A handle to add or remove access control list (ACL) entries from. This parameter may be <see langword="null"/>.</param>
      /// <param name="objectSecurity">A <see cref="DirectorySecurity"/> or <see cref="FileSecurity"/> object that describes an ACL entry to apply to the file described by the <paramref name="path"/> parameter.</param>
      /// <param name="includeSections">One or more of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to set.</param>
      /// <remarks>Supply either a path or handle, not both.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
      [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      internal static bool SetAccessControlInternal(string path, SafeHandle handle, ObjectSecurity objectSecurity, AccessControlSections includeSections)
      {
         if (string.IsNullOrEmpty(path))
         {
            if (handle == null)
               throw new ArgumentNullException("path");
         }

         if (!Filesystem.NativeMethods.IsValidHandle(handle, false))
         {
            if (string.IsNullOrEmpty(path))
               throw new ArgumentException(Resources.HandleInvalid);
         }

         if (objectSecurity == null)
            throw new ArgumentNullException("objectSecurity");


         byte[] managedDescriptor = objectSecurity.GetSecurityDescriptorBinaryForm();
         using (SafeGlobalMemoryBufferHandle hDescriptor = new SafeGlobalMemoryBufferHandle(managedDescriptor.Length))
         {
            hDescriptor.CopyFrom(managedDescriptor, 0, managedDescriptor.Length);

            SecurityDescriptorControl control;
            uint revision;
            if (!GetSecurityDescriptorControl(hDescriptor, out control, out revision))
               NativeError.ThrowException();
            
            PrivilegeEnabler privilegeEnabler = null;
            try
            {
               SecurityInformation securityInfo = SecurityInformation.None;

               if ((includeSections & AccessControlSections.Access) != 0)
                  securityInfo |= SecurityInformation.Dacl;

               if ((includeSections & AccessControlSections.Audit) != 0)
               {
                  // We need the SE_SECURITY_NAME privilege enabled to be able to get the
                  // SACL descriptor. So we enable it here for the remainder of this function.
                  privilegeEnabler = new PrivilegeEnabler(Privilege.Security);
                  securityInfo |= SecurityInformation.Sacl;
               }

               if ((includeSections & AccessControlSections.Group) != 0)
                  securityInfo |= SecurityInformation.Group;

               if ((includeSections & AccessControlSections.Owner) != 0)
                  securityInfo |= SecurityInformation.Owner;


               IntPtr pDacl = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Access) != 0)
               {
                  securityInfo |= SecurityInformation.Dacl;
                  securityInfo |= (control & SecurityDescriptorControl.DaclProtected) != 0
                                     ? SecurityInformation.ProtectedDacl
                                     : SecurityInformation.UnprotectedDacl;

                  bool daclDefaulted, daclPresent;
                  if (!GetSecurityDescriptorDacl(hDescriptor, out daclPresent, out pDacl, out daclDefaulted))
                     NativeError.ThrowException();
               }

               IntPtr pSacl = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Audit) != 0)
               {
                  securityInfo |= SecurityInformation.Sacl;
                  securityInfo |= (control & SecurityDescriptorControl.SaclProtected) != 0
                                     ? SecurityInformation.ProtectedSacl
                                     : SecurityInformation.UnprotectedSacl;

                  privilegeEnabler = new PrivilegeEnabler(Privilege.Security);

                  bool saclDefaulted, saclPresent;
                  if (!GetSecurityDescriptorSacl(hDescriptor, out saclPresent, out pSacl, out saclDefaulted))
                     NativeError.ThrowException();
               }

               IntPtr pOwner = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Owner) != 0)
               {
                  securityInfo |= SecurityInformation.Owner;
                  bool ownerDefaulted;
                  if (!GetSecurityDescriptorOwner(hDescriptor, out pOwner, out ownerDefaulted))
                     NativeError.ThrowException();
               }

               IntPtr pGroup = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Group) != 0)
               {
                  securityInfo |= SecurityInformation.Group;
                  bool groupDefaulted;
                  if (!GetSecurityDescriptorGroup(hDescriptor, out pGroup, out groupDefaulted))
                     NativeError.ThrowException();
               }


               uint lastError;
               if (!string.IsNullOrEmpty(path))
               {
                  // In the ANSI version of this function, the name is limited to MAX_PATH characters.
                  // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
                  // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
                  string pathLp = Path.PrefixLongPath(path);

                  lastError = SetNamedSecurityInfo(pathLp, ObjectType.FileObject, securityInfo, pOwner, pGroup, pDacl, pSacl);
                  if (lastError != Win32Errors.ERROR_SUCCESS)
                     NativeError.ThrowException(lastError, pathLp);
               }

               if (Filesystem.NativeMethods.IsValidHandle(handle, false))
               {
                  lastError = SetSecurityInfo(handle, ObjectType.FileObject, securityInfo, pOwner, pGroup, pDacl, pSacl);
                  if (lastError != Win32Errors.ERROR_SUCCESS)
                     NativeError.ThrowException(lastError);
               }
            }
            finally
            {
               if (privilegeEnabler != null)
                  privilegeEnabler.Dispose();
            }
         }

         return true;
      }

      #endregion // SetAccessControlInternal

      #region DllImport

      #region Privilege

      /// <summary>The AdjustTokenPrivileges function enables or disables privileges in the specified access token. Enabling or disabling privileges in an access token requires TOKEN_ADJUST_PRIVILEGES access.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// To determine whether the function adjusted all of the specified privileges, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, ref TokenPrivileges newState, uint bufferLength, out TokenPrivileges previousState, out uint returnLength);

      /// <summary>The LookupPrivilegeDisplayName function retrieves the display name that represents a specified privilege.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeDisplayNameW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool LookupPrivilegeDisplayName([MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPWStr)] string lpName, ref StringBuilder lpDisplayName, ref uint cchDisplayName, out uint lpLanguageId);

      /// <summary>The LookupPrivilegeValue function retrieves the locally unique identifier (LUID) used on a specified system to locally represent the specified privilege name.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool LookupPrivilegeValue([MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPWStr)] string lpName, out Luid lpLuid);

      #endregion // Privilege

      #region Get/Set Security

      /// <summary>The GetFileSecurity function obtains specified information about the security of a file or directory.
      /// The information obtained is constrained by the caller's access rights and privileges.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFileSecurityW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetFileSecurity([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, SecurityInformation securityInfo, SafeHandle pSecurityDescriptor, uint nLength, out uint lpnLengthNeeded);

      /// <summary>The GetSecurityInfo function retrieves a copy of the security descriptor for an object specified by a handle.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetSecurityInfo(SafeHandle handle, ObjectType objectType, SecurityInformation securityInfo, out IntPtr pSidOwner, out IntPtr pSidGroup, out IntPtr pDacl, out IntPtr pSacl, out SafeGlobalMemoryBufferHandle pSecurityDescriptor);

      /// <summary>The SetSecurityInfo function sets specified security information in the security descriptor of a specified object. 
      /// The caller identifies the object by a handle.</summary>
      /// <returns>
      /// If the function succeeds, the function returns ERROR_SUCCESS.
      /// If the function fails, it returns a nonzero error code defined in WinError.h.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint SetSecurityInfo(SafeHandle handle, ObjectType objectType, SecurityInformation securityInfo, IntPtr psidOwner, IntPtr psidGroup, IntPtr pDacl, IntPtr pSacl);

      /// <summary>The SetNamedSecurityInfo function sets specified security information in the security descriptor of a specified object. The caller identifies the object by name.</summary>
      /// <returns>
      /// If the function succeeds, the function returns ERROR_SUCCESS.
      /// If the function fails, it returns a nonzero error code defined in WinError.h.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetNamedSecurityInfoW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint SetNamedSecurityInfo([MarshalAs(UnmanagedType.LPWStr)] string pObjectName, ObjectType objectType, SecurityInformation securityInfo, IntPtr pSidOwner, IntPtr pSidGroup, IntPtr pDacl, IntPtr pSacl);

      #endregion // Get/Set Security

      #region GetSecurityDescriptorXxx()

      /// <summary>The GetSecurityDescriptorDacl function retrieves a pointer to the discretionary access control list (DACL) in a specified security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorDacl(SafeHandle pSecurityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool lpbDaclPresent, out IntPtr pDacl, [MarshalAs(UnmanagedType.Bool)] out bool lpbDaclDefaulted);

      /// <summary>The GetSecurityDescriptorSacl function retrieves a pointer to the system access control list (SACL) in a specified security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorSacl(SafeHandle pSecurityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool lpbSaclPresent, out IntPtr pSacl, [MarshalAs(UnmanagedType.Bool)] out bool lpbSaclDefaulted);

      /// <summary>The GetSecurityDescriptorGroup function retrieves the primary group information from a security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorGroup(SafeHandle pSecurityDescriptor, out IntPtr pGroup, [MarshalAs(UnmanagedType.Bool)] out bool lpbGroupDefaulted);

      /// <summary>The GetSecurityDescriptorControl function retrieves a security descriptor control and revision information.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorControl(SafeHandle pSecurityDescriptor, out SecurityDescriptorControl pControl, out uint lpdwRevision);

      /// <summary>The GetSecurityDescriptorOwner function retrieves the owner information from a security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorOwner(SafeHandle pSecurityDescriptor, out IntPtr pOwner, [MarshalAs(UnmanagedType.Bool)] out bool lpbOwnerDefaulted);

      /// <summary>The GetSecurityDescriptorLength function returns the length, in bytes, of a structurally valid security descriptor. The length includes the length of all associated structures.</summary>
      /// <returns>
      /// If the function succeeds, the function returns the length, in bytes, of the SECURITY_DESCRIPTOR structure.
      /// If the SECURITY_DESCRIPTOR structure is not valid, the return value is undefined.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetSecurityDescriptorLength(SafeHandle pSecurityDescriptor);

      #endregion // GetSecurityDescriptorXxx()

      #region LocalFree

      /// <summary>Frees the specified local memory object and invalidates its handle.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NULL.
      /// If the function fails, the return value is equal to a handle to the local memory object. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// Note  The local functions have greater overhead and provide fewer features than other memory management functions.
      /// New applications should use the heap functions unless documentation states that a local function should be used.
      /// For more information, see Global and Local Functions.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      internal static extern IntPtr LocalFree(IntPtr hMem);

      #endregion // LocalFree

      #endregion // DllImport
   }
}