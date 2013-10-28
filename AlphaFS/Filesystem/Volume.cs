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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Alphaleonis.Win32.Filesystem
{
    /// <summary>Static class providing utility methods for working with Microsoft Windows devices and volumes.</summary>
    public static class Volume
    {
        #region DosDevice

        #region DefineDosDevice

        /// <summary>Defines, redefines, or deletes MS-DOS device names.</summary>
        /// <param name="deviceName">A pointer to an MS-DOS device name string specifying the device the function is defining, redefining, or deleting.</param>
        /// <param name="targetPath">A pointer to a path string that will implement this device. The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        [SecurityCritical]
        public static bool DefineDosDevice(string deviceName, string targetPath)
        {
            return DefineDosDevice(deviceName, targetPath, DosDeviceAttributes.None);
        }

        /// <summary>Defines, redefines, or deletes MS-DOS device names.</summary>
        /// <param name="deviceName">A pointer to an MS-DOS device name string specifying the device the function is defining, redefining, or deleting.</param>
        /// <param name="targetPath">A pointer to a path string that will implement this device. The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
        /// <param name="deviceAttributes">The controllable aspects of the DefineDosDevice function <see cref="DosDeviceAttributes"/>flags which will be combined with the default.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static bool DefineDosDevice(string deviceName, string targetPath, DosDeviceAttributes deviceAttributes)
        {
            if (string.IsNullOrEmpty(deviceName))
                throw new ArgumentNullException("deviceName");

            // targetPath is allowed to be null.

            deviceName = Path.GetRegularPath(deviceName);

            // In no case is a trailing backslash ("\") allowed.
            deviceName = Path.DirectorySeparatorRemove(deviceName, false);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
                if (!NativeMethods.DefineDosDevice(deviceAttributes, deviceName, targetPath))
                    NativeError.ThrowException(deviceName, targetPath);

            return true;
        }

        #endregion // DefineDosDevice

        #region DeleteDosDevice

        /// <summary>Deletes an MS-DOS device name.</summary>
        /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        [SecurityCritical]
        public static bool DeleteDosDevice(string deviceName)
        {
            return DeleteDosDevice(deviceName, null, false, DosDeviceAttributes.RemoveDefinition);
        }

        /// <summary>Deletes an MS-DOS device name.</summary>
        /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
        /// <param name="targetPath">A pointer to a path string that will implement this device.
        ///  The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        [SecurityCritical]
        public static bool DeleteDosDevice(string deviceName, string targetPath)
        {
            return DeleteDosDevice(deviceName, targetPath, false, DosDeviceAttributes.RemoveDefinition);
        }

        /// <summary>Deletes an MS-DOS device name.</summary>
        /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
        /// <param name="targetPath">A pointer to a path string that will implement this device.
        /// The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
        /// <param name="exactMatch">Only delete MS-DOS device on an exact name match. If exactMatch is true, targetPath must be the same path used to create the mapping.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        [SecurityCritical]
        public static bool DeleteDosDevice(string deviceName, string targetPath, bool exactMatch)
        {
            return DeleteDosDevice(deviceName, targetPath, exactMatch, DosDeviceAttributes.RemoveDefinition);
        }

        /// <summary>Deletes an MS-DOS device name.</summary>
        /// <param name="deviceName">An MS-DOS device name string specifying the device to delete.</param>
        /// <param name="targetPath">A pointer to a path string that will implement this device.
        /// The string is an MS-DOS path string unless the <see cref="DosDeviceAttributes.RawTargetPath"/> flag is specified, in which case this string is a path string.</param>
        /// <param name="exactMatch">Only delete MS-DOS device on an exact name match. If exactMatch is true, targetPath must be the same path used to create the mapping.</param>
        /// <param name="deviceAttributes">The controllable aspects of the DefineDosDevice function <see cref="DosDeviceAttributes"/> flags which will be combined with the default.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static bool DeleteDosDevice(string deviceName, string targetPath, bool exactMatch, DosDeviceAttributes deviceAttributes)
        {
            // A pointer to a path string that will implement this device.
            // The string is an MS-DOS path string unless the DDD_RAW_TARGET_PATH flag is specified, in which case this string is a path string.

            if (exactMatch && !string.IsNullOrEmpty(targetPath))
                deviceAttributes = deviceAttributes | DosDeviceAttributes.ExactMatchOnRemove | DosDeviceAttributes.RawTargetPath;

            // Remove the MS-DOS device name. First, get the name of the Windows NT device
            // from the symbolic link and then delete the symbolic link from the namespace.

            try
            {
                return DefineDosDevice(deviceName, targetPath, deviceAttributes);
            }
            catch
            {
                return false;
            }
        }

        #endregion // DeleteDosDevice

        #region QueryAllDosDevices

        /// <summary>Retrieves a list of all existing MS-DOS device names.</summary>
        /// <returns>An IEnumerable list of Strings of one or more existing MS-DOS device names.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static IEnumerable<string> QueryAllDosDevices()
        {
            return QueryDosDevice(null, null);
        }

        /// <summary>Retrieves a list of all existing MS-DOS device names.</summary>
        /// <param name="deviceName">
        /// (Optional, default: null) An MS-DOS device name string specifying the target of the query.
        /// This parameter can be "sort". In that case a sorted list of all existing MS-DOS device names is returned.
        /// This parameter can be null. In that case, the <see cref="QueryDosDevice"/> function will store a list of all
        /// existing MS-DOS device names into the buffer.
        /// </param>
        /// <returns>An IEnumerable list of Strings of one or more existing MS-DOS device names.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static IEnumerable<string> QueryAllDosDevices(string deviceName)
        {
            return QueryDosDevice(null, deviceName);
        }

        #endregion // QueryAllDosDevices

        #region QueryDosDevice

        /// <summary>Retrieves information about MS-DOS device names. The function can obtain the current mapping for a
        /// particular MS-DOS device name. The function can also obtain a list of all existing MS-DOS device names.
        /// </summary>
        /// <param name="deviceName">
        /// An MS-DOS device name string specifying the target of the query.
        /// This parameter can be null. In that case, the QueryDosDevice function will store a list of all
        /// existing MS-DOS device names into the buffer.
        /// </param>
        /// <param name="options">(Optional, default: "false") If options[0] = "true", a sorted list will be returned.</param>
        /// <returns>An <see cref="IEnumerable{String}"/> object with one or more existing MS-DOS device names.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static IEnumerable<string> QueryDosDevice(string deviceName, params string[] options)
        {
            // The device name cannot have a trailing backslash.
            deviceName = Path.DirectorySeparatorRemove(deviceName, false);
            bool searchFilter = false;

            // Only process options if a device is supplied.
            if (deviceName != null)
            {
                // Check that at least one "options[]" has something to say. If so, rebuild them.
                options = options != null && options.Any() ? new[] { deviceName, options[0] } : new[] { deviceName, string.Empty };

                searchFilter = !Path.IsLogicalDrive(deviceName);

                if (searchFilter)
                    deviceName = null;
            }

            // Choose sorted output.
            bool doSort = options != null &&
                          options.Any(s => s != null && s.Equals("sort", StringComparison.OrdinalIgnoreCase));

            // Start with a larger buffer when using a searchFilter.
            uint bufferSize = (uint)(searchFilter || doSort || (deviceName == null && options == null) ? 32768 : 256);
            uint bufferResult = 0;

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            {
                while (bufferResult == 0)
                {
                    char[] buffer = new char[bufferSize];

                    bufferResult = NativeMethods.QueryDosDevice(deviceName, buffer, bufferSize);
                    int lastError = Marshal.GetLastWin32Error();

                    if (bufferResult == 0)
                        switch ((uint)lastError)
                        {
                            case Win32Errors.ERROR_MORE_DATA:
                            case Win32Errors.ERROR_INSUFFICIENT_BUFFER:
                                bufferSize *= 2;
                                continue;

                            default:
                                NativeError.ThrowException(lastError, deviceName);
                                break;
                        }

                    List<string> dosDev = new List<string>();
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < bufferResult; i++)
                    {
                        if (buffer[i] != Path.StringTerminatorChar)
                            sb.Append(buffer[i]);
                        else if (sb.Length > 0)
                        {
                            dosDev.Add(sb.ToString());
                            sb.Length = 0;
                        }
                    }

                    // Choose the yield back query; filtered or list.
                    IEnumerable<string> selectQuery = (searchFilter)
                                                         ? dosDev.Where(dev => options != null && dev.StartsWith(options[0], StringComparison.OrdinalIgnoreCase))
                                                         : dosDev;

                    if (doSort)
                    {
                        SortedList sorted = new SortedList(StringComparer.InvariantCultureIgnoreCase);

                        foreach (string item in selectQuery)
                            sorted.Add(item, null);

                        foreach (string item in sorted.Keys)
                            yield return item;
                    }
                    else
                    {
                        foreach (string dev in selectQuery)
                            yield return dev;
                    }
                }
            }
        }

        #endregion // QueryDosDevice

        #endregion // DosDevice

        #region Drive

        #region GetCurrentDriveType

        /// <summary>Determines, based on the root of the current directory, whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.</summary>
        /// <returns>A <see cref="DriveType"/> object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [SecurityCritical]
        public static DriveType GetCurrentDriveType()
        {
            return GetDriveType(null);
        }

        #endregion // GetCurrentDriveType

        #region GetDriveFormat

        /// <summary>Gets the name of the file system, such as NTFS or FAT32.</summary>
        /// <param name="rootPathName">The root directory for the drive.</param>
        /// <returns>The name of the file system on the specified drive or <see cref="string.Empty"/> on failure or if not available.</returns>
        /// <remarks>Use DriveFormat to determine what formatting a drive uses.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static string GetDriveFormat(string rootPathName)
        {
            VolumeInfo volInfo = GetVolumeInformationInternal(rootPathName, null, false);
            return (volInfo == null) ? string.Empty : volInfo.FileSystemName;
        }

        #endregion // GetDriveFormat

        #region GetDriveType

        /// <summary>Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.</summary>
        /// <param name="rootPathName">The root directory for the drive. If this parameter is null, the function uses the root of the current directory.</param>
        /// <returns>A <see cref="DriveType"/> object.</returns>
        [SecurityCritical]
        public static DriveType GetDriveType(string rootPathName)
        {
            // rootPathName == null is allowed.

            rootPathName = Path.DirectorySeparatorAdd(rootPathName, false);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
                return NativeMethods.GetDriveType(rootPathName);
        }

        #endregion // GetDriveType

        #region GetDiskFreeSpace

        /// <summary>Retrieves information about the amount of space that is available on a disk volume, which is the total amount of space, the total amount of free space, and the total amount of free space available to the user that is associated with the calling thread.</summary>
        /// <param name="drivePath">The root directory of the disk for which information is to be returned.</param>
        /// <returns>A <see ref="DiskSpaceInfoExtended"/> structure object containing the requested information or null if any of the depended methods fails.</returns>
        /// <remarks>The calling application must have FILE_LIST_DIRECTORY access rights for this directory.</remarks>
        [SecurityCritical]
        public static DiskSpaceInfoExtended GetDiskFreeSpace(string drivePath)
        {
            return new DiskSpaceInfoExtended(drivePath, false);
        }

        #endregion // GetDiskFreeSpace

        #region GetDiskFreeSpaceClusters

        /// <summary>Retrieves information about the specified disk, including the amount of free space on the disk.</summary>
        /// <param name="drivePath">The root directory of the disk for which information is to be returned.
        /// Furthermore, a drive specification must have a trailing backslash (for example, "C:\").</param>
        /// <returns>A <see ref="DiskSpaceInfoExtended"/> structure object containing the requested information or null if any of the depended methods fails.</returns>
        /// <remarks>The calling application must have FILE_LIST_DIRECTORY access rights for this directory.</remarks>
        [SecurityCritical]
        public static DiskSpaceInfoExtended GetDiskFreeSpaceClusters(string drivePath)
        {
            return new DiskSpaceInfoExtended(drivePath, true);
        }

        #endregion // GetDiskFreeSpaceClusters

        #region IsReady

        /// <summary>Gets a value indicating whether a drive is ready.</summary>
        /// <param name="drivePath">A valid drive path or drive letter. This can be either uppercase or lowercase, 'a' to 'z' or a network share in the format: \\server\share</param>
        /// <returns><c>true</c> if the drive is ready; <c>false</c> otherwise.</returns>
        /// <remarks>This function currently does not support Network share paths, instead a Patch.GetCurrentDirectory() will be performed to check if the Network share path is available. If yes: IsReady == true.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static bool IsReady(string drivePath)
        {
            if (string.IsNullOrEmpty(drivePath))
                throw new ArgumentNullException("drivePath");

            drivePath = Path.DirectorySeparatorRemove(drivePath, false);

            // If path is a UNC path, check if the directory or file can be accessed.
            if (Path.IsUnc(drivePath))
                return FileSystemInfo.ExistsInternal(true, null, drivePath);

            try
            {
                // Use .NET function.
                return new System.IO.DriveInfo(drivePath).IsReady;
            }
            catch
            {
                return false;
            }
        }

        #endregion // IsReady

        #endregion // Drive

        #region Volume

        #region Label

        #region DeleteCurrentVolumeLabel

        /// <summary>Deletes the label of the file system volume that is the root of the current directory.</summary>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <exception cref="NativeError.ThrowException()"></exception>
        [SecurityCritical]
        public static bool DeleteCurrentVolumeLabel()
        {
            return SetVolumeLabel(null, null);
        }

        #endregion // DeleteCurrentVolumeLabel

        #region DeleteVolumeLabel

        /// <summary>Deletes the label of a file system volume.</summary>
        /// <param name="rootPathName">The root directory of a file system volume. This is the volume the function will label.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <exception cref="NativeError.ThrowException()"></exception>
        [SecurityCritical]
        public static bool DeleteVolumeLabel(string rootPathName)
        {
            if (string.IsNullOrEmpty(rootPathName))
                throw new ArgumentNullException(rootPathName);

            return SetVolumeLabel(rootPathName, null);
        }

        #endregion // DeleteVolumeLabel

        #region GetVolumeLabel

        /// <summary>Retrieve the label of a file system volume.</summary>
        /// <param name="rootPathName">
        /// A pointer to a string that contains the volume's Drive letter (for example, X:\)
        /// or the path of a mounted folder that is associated with the volume (for example, Y:\MountX\).
        /// If this parameter is null, the root of the current directory is used.
        /// </param>
        /// <returns>The the label of the file system volume. This function can return an empty string since a volume label is generally not mandatory.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static string GetVolumeLabel(string rootPathName)
        {
            VolumeInfo volInfo = GetVolumeInformationInternal(rootPathName, null, false);
            return (volInfo == null) ? string.Empty : volInfo.Name;
        }

        #endregion // GetVolumeLabel

        #region SetCurrentVolumeLabel

        /// <summary>Sets the label of the file system volume that is the root of the current directory.</summary>
        /// <param name="volumeName">A name for the volume. A pointer to a string that contains
        /// the new label for the volume. If this parameter is null, the function deletes any
        /// existing label from the specified volume and does not assign a new label.
        /// </param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <exception cref="NativeError.ThrowException()"></exception>
        [SecurityCritical]
        public static bool SetCurrentVolumeLabel(string volumeName)
        {
            // volumeName == null is allowed.

            //if (string.IsNullOrEmpty(volumeName))
            //throw new ArgumentNullException("volumeName");

            // NTFS uses a limit of 32 characters for the volume label as of Windows Server 2003.
            return SetVolumeLabel(null, volumeName);
        }

        #endregion // SetCurrentVolumeLabel

        #region SetVolumeLabel

        /// <summary>Sets the label of a file system volume.</summary>
        /// <param name="rootPathName">
        /// A pointer to a string that contains the volume's Drive letter (for example, X:\)
        /// or the path of a mounted folder that is associated with the volume (for example, Y:\MountX\).
        /// If this parameter is null, the root of the current directory is used.
        /// </param>
        /// <param name="volumeName">A name for the volume. A pointer to a string that contains
        /// the new label for the volume. If this parameter is null, the function deletes any
        /// existing label from the specified volume and does not assign a new label.
        /// </param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <exception cref="NativeError.ThrowException()"></exception>
        [SecurityCritical]
        public static bool SetVolumeLabel(string rootPathName, string volumeName)
        {
            // rootPathName == null is allowed.

            // Setting volume label only applies to Logical Drives pointing to local resources.
            //if (!Path.IsLogicalDrive(rootPathName))
            //return false;

            rootPathName = Path.DirectorySeparatorAdd(rootPathName, false);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            // NTFS uses a limit of 32 characters for the volume label as of Windows Server 2003.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
                if (!NativeMethods.SetVolumeLabel(rootPathName, volumeName))
                    NativeError.ThrowException(rootPathName, volumeName);

            return true;
        }

        #endregion // SetVolumeLabel

        #endregion // Label

        #region GetDeviceForVolumeName

        /// <summary>Retrieves the Win32 Device name from the Volume name.</summary>
        /// <param name="volumeName">Name of the Volume</param>
        /// <returns>The Win32 Device name from the Volume name or <see cref="string.Empty"/> on error or if unavailable.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static string GetDeviceForVolumeName(string volumeName)
        {
            if (string.IsNullOrEmpty(volumeName))
                throw new ArgumentNullException("volumeName");

            volumeName = Path.DirectorySeparatorRemove(volumeName, false);

            #region GlobalRoot

            if (volumeName.StartsWith(Path.GlobalRootPrefix, StringComparison.OrdinalIgnoreCase))
                return volumeName.Substring(Path.GlobalRootPrefix.Length);

            #endregion // GlobalRoot

            bool doQueryDos = false;

            #region Volume

            if (volumeName.StartsWith(Path.VolumePrefix, StringComparison.OrdinalIgnoreCase))
            {
                // Isolate the DOS Device from the Volume name, in the format: Volume{GUID}
                volumeName = volumeName.Substring(Path.LongPathPrefix.Length);
                doQueryDos = true;
            }

            #endregion // Volume

            #region Logical Drive

            // Check for Logical Drives: C:, D:, ...
            else if (Path.IsLogicalDrive(volumeName))
                doQueryDos = true;

            #endregion // Logical Drive

            if (doQueryDos)
            {
                try
                {
                    // Get the real Device underneath.
                    string dev = QueryDosDevice(volumeName).FirstOrDefault();
                    return !string.IsNullOrEmpty(dev) ? dev : string.Empty;
                }
                catch
                {
                }
            }

            return string.Empty;
        }

        #endregion // GetDeviceForVolumeName

        #region GetDisplayNameForVolume

        /// <summary>Gets the shortest display name for the specified <paramref name="volumeName"/>.</summary>
        /// <param name="volumeName">A volume <see cref="Guid"/> path: \\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\</param>
        /// <returns>The shortest display name for the specified volume found, or <see cref="string.Empty"/> if no display names were found.</returns>
        /// <remarks>This method basically returns the shortest string returned by <see cref="GetVolumePathNamesForVolume"/></remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static string GetDisplayNameForVolume(string volumeName)
        {
            string[] smallestMountPoint = { new string(Path.WildcardStarMatchAllChar, NativeMethods.MaxPathUnicode) };

            try
            {
                foreach (string m in GetVolumePathNamesForVolume(volumeName).Where(m => !string.IsNullOrEmpty(m) && m.Length < smallestMountPoint[0].Length))
                    smallestMountPoint[0] = m;
            }
            catch
            {
            }

            return smallestMountPoint[0][0] == Path.WildcardStarMatchAllChar ? string.Empty : smallestMountPoint[0];
        }

        #endregion // GetDisplayNameForVolume

        #region GetUniqueVolumeNameForPath

        /// <summary>Get the unique volume name for the given path.</summary>
        /// <param name="volumePathName">A pointer to the input path string. Both absolute and relative file and directory names,
        /// for example <see cref="Filesystem.Path.ParentDirectoryPrefix"/>, are acceptable in this path.
        /// If you specify a relative directory or file name without a volume qualifier, GetUniqueVolumeNameForPath returns the Drive letter of the current volume.
        /// </param>
        /// <param name="options">options[0] = true: Remove the trailing backslash.</param>
        /// <returns>
        /// The unique name of the Volume Mount Point, a volume <see cref="Guid"/> path: \\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\
        /// If not available or if the function fails, the return value is <paramref name="volumePathName"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static string GetUniqueVolumeNameForPath(string volumePathName, params bool[] options)
        {
            if (string.IsNullOrEmpty(volumePathName))
                throw new ArgumentNullException("volumePathName");

            bool removeDirectorySeparator = options != null && (options.Any() && options[0]);

            try
            {
                volumePathName = GetVolumeGuid(GetVolumePathName(volumePathName));
                return removeDirectorySeparator ? Path.DirectorySeparatorRemove(volumePathName, false) : volumePathName;
            }
            catch
            {
            }

            return volumePathName;
        }

        #endregion // GetUniqueVolumeNameForPath

        #region GetVolumeInformation

        /// <summary>Retrieves information about the file system and volume associated with the specified root directory or filestream.</summary>
        /// <param name="volumePath">A path that contains the root directory.</param>
        /// <returns>A <see cref="VolumeInfo"/> instance describing the volume associatied with the specified root directory.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static VolumeInfo GetVolumeInformation(string volumePath)
        {
            return GetVolumeInformationInternal(volumePath, null, true);
        }

        /// <summary>Retrieves information about the file system and volume associated with the specified root directory or filestream.</summary>
        /// <param name="volumeHandle">A pointer to a <see cref="FileStream"/> handle.</param>
        /// <returns>A <see cref="VolumeInfo"/> instance describing the volume associatied with the specified root directory.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static VolumeInfo GetVolumeInformation(FileStream volumeHandle)
        {
            return GetVolumeInformationInternal(null, volumeHandle, true);
        }

        #endregion // GetVolumeInformation

        #region GetVolumes

        /// <summary>Retrieves the name of a volume on a computer. FindFirstVolume is used to begin scanning the volumes of a computer.</summary>
        /// <returns>An IEnumerable string containing the volume names on the computer.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [SecurityCritical]
        public static IEnumerable<string> GetVolumes()
        {
            StringBuilder sb = new StringBuilder(NativeMethods.MaxPathUnicode);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            using (SafeFindVolumeHandle handle = NativeMethods.FindFirstVolume(sb, (uint)sb.Capacity))
            {
                while (NativeMethods.IsValidHandle(handle, false))
                {
                    if (NativeMethods.FindNextVolume(handle, sb, (uint)sb.Capacity))
                        yield return sb.ToString();

                    else
                    {
                        int lastError = Marshal.GetLastWin32Error();
                        if (lastError == Win32Errors.ERROR_NO_MORE_FILES)
                            yield break;

                        NativeError.ThrowException(lastError);
                    }
                }
            }
        }

        #endregion // GetVolumes

        #region GetVolumePathName

        /// <summary>Retrieves the volume mount point where the specified path is mounted.
        /// Returns the nearest volume root path for a given directory.
        /// </summary> 
        /// <param name="path">The path to the volume, for example: C:\Windows</param>
        /// <returns>The volume path name, for example: C:\windows --> C:\, in case of failure <paramref name="path"/> is returned.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static string GetVolumePathName(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            {
                // For the following set of examples, U: is mapped to the remote computer \\YourComputer\C$, and Q is a local drive. 
                // Get the root path of the Volume.
                //    Specified path                      Function returns
                //    \\YourComputer\C$\Windows           \\YourComputer\C$\
                //    \\?\UNC\YourComputer\C$\Windows     \\?\UNC\YourComputer\C$\
                //    Q:\Windows                          Q:\
                //    \\?\Q:\Windows                      \\?\Q:\
                //    \\.\Q:\Windows                      \\.\Q:\
                //    \\?\UNC\W:\Windows                  FALSE with error 123 because a specified remote path was not valid; W$ share does not exist or no user access granted.
                //    C:\COM2 (which exists)              \\.\COM2\
                //    C:\COM3 (non-existent)              FALSE with error 123 because a non-existent COM device was specified.

                // For the following set of examples, the paths contain invalid trailing path elements.
                //    Specified path                                                 Function returns
                //    G:\invalid (invalid path)	                                    G:\
                //    \\.\I:\aaa\invalid (invalid path)	                           \\.\I:\
                //    \\YourComputer\C$\invalid (invalid trailing path element)	   \\YourComputer\C$\

                // If a network share is specified, GetVolumePathName returns the shortest path for which GetDriveType returns DRIVE_REMOTE,
                // which means that the path is validated as a remote drive that exists, which the current user can access.

                StringBuilder volumeRootPath = new StringBuilder(NativeMethods.MaxPathUnicode);


                // In the ANSI version of this function, the name is limited to 248 characters.
                // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
                // 2013-07-18: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
                string pathLp = Path.PrefixLongPath(path);

                bool getOk = NativeMethods.GetVolumePathName(pathLp, volumeRootPath, (uint)volumeRootPath.Capacity);
                int lastError = Marshal.GetLastWin32Error();

                if (getOk)
                    return Path.GetRegularPath(volumeRootPath.ToString());

                switch ((uint)lastError)
                {
                    // Don't throw exception on these errors.
                    case Win32Errors.ERROR_NO_MORE_FILES:
                    case Win32Errors.ERROR_INVALID_PARAMETER:
                    case Win32Errors.ERROR_INVALID_NAME:
                        break;

                    default:
                        NativeError.ThrowException(lastError, pathLp);
                        break;
                }

                return path;
            }
        }

        #endregion // GetVolumePathName

        #region GetVolumePathNamesForVolume

        /// <summary>Retrieves a list of Drive letters and mounted folder paths for the specified volume.</summary>
        /// <param name="volumeGuid">A volume <see cref="Guid"/> path: \\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\</param>
        /// <returns>An IEnumerable string containing the path names for the specified volume.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static IEnumerable<string> GetVolumePathNamesForVolume(string volumeGuid)
        {
            if (string.IsNullOrEmpty(volumeGuid))
                throw new ArgumentNullException("volumeGuid");

            if (!volumeGuid.StartsWith(Path.VolumePrefix + "{", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Argument is not a valid Volume GUID.", volumeGuid);

            string volName = Path.DirectorySeparatorAdd(volumeGuid, false);

            uint requiredLength = 6;
            char[] buffer = new char[requiredLength];

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            {
                while (!NativeMethods.GetVolumePathNamesForVolumeName(volName, buffer, (uint)buffer.Length, out requiredLength))
                {
                    int lastError = Marshal.GetLastWin32Error();

                    switch ((uint)lastError)
                    {
                        case Win32Errors.ERROR_MORE_DATA:
                        case Win32Errors.ERROR_INSUFFICIENT_BUFFER:
                            buffer = new char[requiredLength];
                            break;

                        default:
                            NativeError.ThrowException(lastError, volumeGuid);
                            break;
                    }
                }

                StringBuilder sb = new StringBuilder(buffer.Length);
                foreach (char c in buffer)
                {
                    if (c != Path.StringTerminatorChar)
                        sb.Append(c);
                    else
                    {
                        if (sb.Length > 0)
                        {
                            yield return sb.ToString();
                            sb.Length = 0;
                        }
                    }
                }
            }
        }

        #endregion // GetVolumePathNamesForVolume

        #region IsSame

        /// <summary>Determines whether the volume of two filesystem objects is the same.</summary>
        /// <param name="fsoPath1">The first filesystem ojbect with full path information.</param>
        /// <param name="fsoPath2">The second filesystem object with full path information.</param>
        /// <returns><c>true</c> if both filesytem objects reside on the same volume, <c>false</c> otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "fso")]
        public static bool IsSame(string fsoPath1, string fsoPath2)
        {
            if (string.IsNullOrEmpty(fsoPath1))
                throw new ArgumentNullException(fsoPath1);

            if (string.IsNullOrEmpty(fsoPath2))
                throw new ArgumentNullException(fsoPath2);

            try
            {
                VolumeInfo volInfo1 = GetVolumeInformationInternal(GetVolumePathName(fsoPath1), null, false);
                VolumeInfo volInfo2 = GetVolumeInformationInternal(GetVolumePathName(fsoPath2), null, false);

                return (volInfo1 != null && volInfo2 != null) && volInfo1.SerialNumber.Equals(volInfo2.SerialNumber, StringComparison.OrdinalIgnoreCase);
            }
            catch { }

            return false;
        }

        #endregion // IsSame

        #region IsVolume

        /// <summary>Determines whether the specified volume name is a defined volume on the current computer.</summary>
        /// <param name="volumeMountPoint">A string representing the path to a volume. For example: "C:\", "D:", "P:\Mountpoint\Backup", "\\?\Volume{c0580d5e-2ad6-11dc-9924-806e6f6e6963}\"</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SecurityCritical]
        public static bool IsVolume(string volumeMountPoint)
        {
            try
            {
                return !string.IsNullOrEmpty(GetVolumeGuid(volumeMountPoint));
            }
            catch
            {
                return false;
            }
        }

        #endregion // IsVolume

        #region Volume Mount Point

        #region DeleteVolumeMountPoint

        /// <summary>Deletes a Drive letter or mounted folder.</summary>
        /// <param name="volumeMountPoint">The Drive letter or mounted folder to be deleted. For example, X:\ or Y:\MountX\.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <remarks>Deleting a mounted folder does not cause the underlying directory to be deleted.</remarks>
        /// <remarks>It's not an error to attempt to unmount a volume from a volume mount point when there is no volume actually mounted at that volume mount point.</remarks>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static bool DeleteVolumeMountPoint(string volumeMountPoint)
        {
            if (string.IsNullOrEmpty(volumeMountPoint))
                throw new ArgumentNullException("volumeMountPoint");

            volumeMountPoint = Path.DirectorySeparatorAdd(volumeMountPoint, false);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
                if (!NativeMethods.DeleteVolumeMountPoint(volumeMountPoint))
                    NativeError.ThrowException(volumeMountPoint);

            return true;
        }

        #endregion // DeleteVolumeMountPoint

        #region GetVolumeGuid

        /// <summary>Retrieves a volume <see cref="Guid"/> path for the volume that is associated with the specified volume mount point ( drive letter, volume GUID path, or mounted folder).</summary>
        /// <param name="volumeMountPoint">The path of a mounted folder (for example, "Y:\MountX\") or a drive letter (for example, "X:\").</param>
        /// <returns>The unique volume name of the form: "\\?\Volume{GUID}\" where <see cref="Guid"/> is the GUID that identifies the volume.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static string GetVolumeGuid(string volumeMountPoint)
        {
            if (string.IsNullOrEmpty(volumeMountPoint))
                throw new ArgumentNullException("volumeMountPoint");

            volumeMountPoint = Path.GetRegularPath(volumeMountPoint);

            // The string must end with a trailing backslash ('\').
            volumeMountPoint = Path.DirectorySeparatorAdd(volumeMountPoint, false);

            // In the ANSI version of this function, the name is limited to 248 characters.
            // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
            // 2013-07-18: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
            volumeMountPoint = Path.PrefixLongPath(volumeMountPoint);

            StringBuilder volumeGuid = new StringBuilder(100);
            StringBuilder uniqueName = new StringBuilder(100);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            {
                bool getOk = NativeMethods.GetVolumeNameForVolumeMountPoint(volumeMountPoint, volumeGuid, (uint)volumeGuid.Capacity);
                int lastError = Marshal.GetLastWin32Error();
                bool raiseException = !getOk;

                if (getOk)
                {
                    getOk = NativeMethods.GetVolumeNameForVolumeMountPoint(Path.DirectorySeparatorAdd(volumeGuid.ToString(), false), uniqueName, (uint)uniqueName.Capacity);
                    lastError = Marshal.GetLastWin32Error();
                    raiseException = !getOk;

                    if (getOk)
                        return uniqueName.ToString();
                }

                if (raiseException)
                    NativeError.ThrowException(lastError, volumeMountPoint);

                return null;

                // Bottom line is: if you have disk/volume migrations, then you can expect multiple volume names for the same volume.
                // But whatever it happens, there is always a unique volume name for the current boot session.
                // You can obtain this unique name by calling GetVolumeNameForVolumeMountPoint once on your root, get the volume name,
                // and then call GetVolumeNameForVolumeMountPoint again. This will always return the unique volume name.
                //
                // This trick is very useful for example when you want to check if two volume paths V1 and V2 represent the same volume or not.
                // You get the first volume path (V1), call GetVolumeNameForVolumeMountPoint on it twice in the manner described above,
                // and remember the returned volume name. You do the same thing on V2. In the end, you compare the volume names.
                // If they are equal, then the two volumes are identical. 
                //
                // http://blogs.msdn.com/b/adioltean/archive/2005/04/16/408947.aspx
            }
        }

        #endregion // GetVolumeGuid

        #region GetVolumeMountPoints

        /// <summary>Retrieves the names of all mounted folders (volume mount points) on the specified volume.</summary>
        /// <param name="volumeGuid">A <see langref="String"/> containing the volume <see cref="Guid"/>.</param>
        /// <returns>The names of all volume mount points on the specified volume or <see cref="string.Empty"/> on error or if unavailable.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static IEnumerable<string> GetVolumeMountPoints(string volumeGuid)
        {
            if (string.IsNullOrEmpty(volumeGuid))
                throw new ArgumentNullException("volumeGuid");

            if (!volumeGuid.StartsWith(Path.VolumePrefix + "{", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Argument is not a valid Volume GUID.", volumeGuid);

            // A trailing backslash is required.
            volumeGuid = Path.DirectorySeparatorAdd(volumeGuid, false);

            StringBuilder sb = new StringBuilder(NativeMethods.MaxPathUnicode);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            using (SafeFindVolumeMountPointHandle handle = NativeMethods.FindFirstVolumeMountPoint(volumeGuid, sb, (uint)sb.Capacity))
            {
                int lastError = Marshal.GetLastWin32Error();

                if (!NativeMethods.IsValidHandle(handle, false))
                {
                    switch ((uint)lastError)
                    {
                        case Win32Errors.ERROR_NO_MORE_FILES:
                        case Win32Errors.ERROR_PATH_NOT_FOUND: // Observed with USB stick, FAT32 formatted.
                            yield break;

                        default:
                            NativeError.ThrowException(lastError, volumeGuid);
                            break;
                    }
                }

                yield return sb.ToString();

                while (NativeMethods.FindNextVolumeMountPoint(handle, sb, (uint)sb.Capacity))
                {
                    lastError = Marshal.GetLastWin32Error();
                    if (lastError != Win32Errors.ERROR_NO_MORE_FILES)
                        NativeError.ThrowException(lastError, volumeGuid);

                    yield return sb.ToString();
                }
            }
        }

        #endregion // GetVolumeMountPoints

        #region SetVolumeMountPoint

        /// <summary>Associates a volume with a Drive letter or a directory on another volume.</summary>
        /// <param name="volumeMountPoint">
        /// The user-mode path to be associated with the volume. This may be a Drive letter (for example, "X:\")
        /// or a directory on another volume (for example, "Y:\MountX\").
        /// </param>
        /// <param name="volumeGuid">A <see langref="String"/> containing the volume <see cref="Guid"/>.</param>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        public static bool SetVolumeMountPoint(string volumeMountPoint, string volumeGuid)
        {
            if (string.IsNullOrEmpty(volumeMountPoint))
                throw new ArgumentNullException("volumeMountPoint");

            if (string.IsNullOrEmpty(volumeGuid))
                throw new ArgumentNullException("volumeGuid");

            if (!volumeGuid.StartsWith(Path.VolumePrefix + "{", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Argument is not a valid Volume GUID.", volumeGuid);

            volumeMountPoint = Path.GetRegularPath(volumeMountPoint);

            // In the ANSI version of this function, the name is limited to 248 characters.
            // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
            // 2013-07-18: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
            volumeMountPoint = Path.PrefixLongPath(volumeMountPoint);

            // The string must end with a trailing backslash ('\').
            volumeMountPoint = Path.DirectorySeparatorAdd(volumeMountPoint, false);

            // This string must be of the form "\\?\Volume{GUID}\"
            volumeGuid = Path.DirectorySeparatorAdd(volumeGuid, false);



            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
                if (!NativeMethods.SetVolumeMountPoint(volumeMountPoint, volumeGuid))
                {
                    int lastError = Marshal.GetLastWin32Error();

                    // If the lpszVolumeMountPoint parameter contains a path to a mounted folder,
                    // GetLastError returns ERROR_DIR_NOT_EMPTY, even if the directory is empty.

                    if (lastError != Win32Errors.ERROR_DIR_NOT_EMPTY)
                        NativeError.ThrowException(lastError, volumeMountPoint, volumeGuid);
                }

            return true;
        }

        #endregion // SetVolumeMountPoint

        #endregion // Volume Mount Point

        #endregion // Volume


        #region Unified Internals

        /// <summary>Unified method GetVolumeInformationInternal() to retrieve information about the file system and volume associated with the specified root directory or filestream.</summary>
        /// <param name="volumePath">A path that contains the root directory.</param>
        /// <param name="volumeHandle">A pointer to a <see cref="FileStream"/> handle.</param>
        /// <param name="raiseException">If <c>true</c> raises Exceptions, when <c>false</c> no Exceptions are raised and the method returns <see langref="null"/>.</param>
        /// <returns>A <see cref="VolumeInfo"/> instance describing the volume associatied with the specified root directory. See <paramref name="raiseException"/></returns>
        /// <remarks>Either use <paramref name="volumePath"/> or <paramref name="volumeHandle"/>, not both.</remarks>
        /// <exception cref="NativeError.ThrowException()"/>
        [SecurityCritical]
        internal static VolumeInfo GetVolumeInformationInternal(string volumePath, FileStream volumeHandle, bool raiseException)
        {
            bool isHandle = false;

            if (!string.IsNullOrEmpty(volumePath))
            {
                volumePath = Path.GetRegularPath(Path.DirectorySeparatorAdd(volumePath, false));

                // In the ANSI version of this function, the name is limited to 248 characters.
                // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
                // 2013-07-18: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
                volumePath = Path.PrefixLongPath(volumePath);
            }
            else
            {
                if (!NativeMethods.IsValidHandle(volumeHandle.SafeFileHandle, raiseException))

                    return null;

                isHandle = true;
            }

            // The maximum buffer size is MAX_PATH+1.
            StringBuilder volumeNameBuffer = new StringBuilder(NativeMethods.MaxPath);
            StringBuilder fileSystemNameBuffer = new StringBuilder(NativeMethods.MaxPath);

            // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
            // Minimize method calls from here.
            using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
            {
                uint serialNumber;
                uint maximumComponentLength;
                NativeMethods.VolumeInfoAttributes volumeInfoAttrs;

                bool getOk = isHandle
                         ? NativeMethods.GetVolumeInformationByHandle(volumeHandle.SafeFileHandle, volumeNameBuffer, (uint)volumeNameBuffer.Capacity, out serialNumber, out maximumComponentLength, out volumeInfoAttrs, fileSystemNameBuffer, (uint)fileSystemNameBuffer.Capacity)
                         : NativeMethods.GetVolumeInformation(volumePath, volumeNameBuffer, (uint)volumeNameBuffer.Capacity, out serialNumber, out maximumComponentLength, out volumeInfoAttrs, fileSystemNameBuffer, (uint)fileSystemNameBuffer.Capacity);

                int lastError = Marshal.GetLastWin32Error();

                if (!getOk)
                {
                    if (raiseException)
                        NativeError.ThrowException(lastError, volumePath);
                    else
                        return null;
                }

                return new VolumeInfo
                   {
                       FileSystemName = fileSystemNameBuffer.ToString(),
                       FullPath = Path.GetRegularPath(volumePath),
                       MaximumComponentLength = maximumComponentLength,
                       Name = volumeNameBuffer.ToString(),
                       SerialNumber = serialNumber.ToString(CultureInfo.InvariantCulture),
                       VolumeInfoAttributes = volumeInfoAttrs
                   };
            }
        }

        #endregion // Unified Internals
    }
}