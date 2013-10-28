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
using System.IO;
using System.Linq;
using System.Security;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Provides access to information on a local or remote drive.</summary>
   /// <remarks>
   /// This class models a drive and provides methods and properties to query for drive information.
   /// Use DriveInfo to determine what drives are available, and what type of drives they are.
   /// You can also query to determine the capacity and available free space on the drive.
   /// </remarks>
   [Serializable]
   [SecurityCritical]
   public sealed class DriveInfo
   {
      #region Class Internal Affairs

      #region .NET

      //// <summary>Determines whether the specified Object is equal to the current Object.</summary>
      /// <param name="obj">Another object to compare to.</param>
      /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
      public override bool Equals(object obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         DriveInfo other = obj as DriveInfo;

         if (other == null)
            return false;

         return other.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) &&
                other.RootDirectory.Name.Equals(RootDirectory.Name, StringComparison.OrdinalIgnoreCase);
      }

      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>A hash code for the current Object.</returns>
      public override int GetHashCode()
      {
         unchecked
         {
            int hash = Primes[_random];

            if (!string.IsNullOrEmpty(Name))
               hash = hash * Primes[1] + Name.GetHashCode();

            if (RootDirectory != null)
               if (!string.IsNullOrEmpty(RootDirectory.Name))
                  hash = hash * Primes[1] + RootDirectory.Name.GetHashCode();

            if (!string.IsNullOrEmpty(DosDeviceName))
               hash = hash * Primes[1] + DosDeviceName.GetHashCode();

            if (!string.IsNullOrEmpty(DriveFormat))
               hash = hash * Primes[1] + DriveFormat.GetHashCode();

            return hash;
            //hash = hash * Primes[1] + ClusterSize.GetHashCode();
            //return hash * Primes[1] + SectorsPerCluster.GetHashCode();
         }
      }

      /// <summary>Implements the operator ==</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(DriveInfo left, DriveInfo right)
      {
         return ReferenceEquals(left, null) && ReferenceEquals(right, null) ||
                !ReferenceEquals(left, null) && !ReferenceEquals(right, null) && left.Equals(right);
      }

      /// <summary>Implements the operator !=</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(DriveInfo left, DriveInfo right)
      {
         return !(left == right);
      }

      #endregion // .NET

      #region AlphaFS

      // A random prime number will be picked and add to the HasCode, each time an instance is created.
      [NonSerialized] private readonly int _random = new Random().Next(0, 20);
      [NonSerialized] private static readonly int[] Primes = new[] {17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919};

      #endregion // AlphaFS

      #endregion // Class Internal Affairs

      #region Constructors

      #region .NET

      /// <summary>Initializes a DriveInfo class.</summary>
      /// <param name="drivePath">A valid drive path or drive letter. This can be either uppercase or lowercase, 'a' to 'z' or a network share in the format: \\server\share</param>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public DriveInfo(string drivePath)
      {
         try
         {
            // This will always fail on SUBST.EXE drives.
            drivePath = Volume.GetVolumePathName(drivePath);
         }
         catch
         {
            
            drivePath = Path.IsUnc(drivePath) ? drivePath : Path.MakeDriveLetter(drivePath);
         }

         if (string.IsNullOrEmpty(drivePath))
            throw new ArgumentNullException("drivePath");

         // If an exception is thrown, the original drivePath is used.
         Name = Path.DirectorySeparatorAdd(drivePath, false);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      [NonSerialized]
      private readonly KernelTransaction _transaction;

      /// <summary>Initializes a DriveInfo class.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="drivePath">A valid drive path or drive letter. This can be either uppercase or lowercase, 'a' to 'z' or a network share in the format: \\server\share</param>
      [SecurityCritical]
      public DriveInfo(KernelTransaction transaction, string drivePath)
         : this(drivePath)
      {
         _transaction = transaction;
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // Constructors
      
      #region Methods

      #region  .NET

      #region GetDrives()

      #region  .NET

      /// <summary>Retrieves the drive names of all logical drives and network shares, on a computer.</summary>
      /// <returns>A <see langref="DriveInfo[]"/> object that represents the logical drives and network shares, on a computer.</returns>
      /// <remarks>
      /// This method retrieves all logical drive names and network shares on a computer. You can use this information to iterate through the array
      /// and obtain information on the drives using other <see cref="DriveInfo"/> methods and properties. Use the <see cref="IsReady"/> property to test
      /// whether a drive is ready because using this method on a drive that is not ready will throw a <see cref="IOException"/>.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static DriveInfo[] GetDrives()
      {
         return Directory.GetLogicalDrives(false, false, false).Select(drive => (new DriveInfo(null, drive))).ToArray();
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Retrieves the drive names of all logical drives and network shares, on a computer.</summary>
      /// <returns>An <see cref="IEnumerable{DriveInfo}"/> object that represents the logical drives and network shares, on a computer.</returns>
      /// <remarks>
      /// This method retrieves all logical drive names and network shares on a computer. You can use this information to iterate through the array
      /// and obtain information on the drives using other <see cref="DriveInfo"/> methods and properties. Use the <see cref="IsReady"/> property to test
      /// whether a drive is ready because using this method on a drive that is not ready will throw a <see cref="IOException"/>.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<DriveInfo> GetDrives(bool fromEnvironment)
      {
         return Directory.GetLogicalDrives(fromEnvironment, false, false).Select(drive => new DriveInfo(null, drive));
      }

      /// <summary>Retrieves the drive names of all logical drives and network shares, on a computer.</summary>
      /// <param name="fromEnvironment">Retrieve logical drives as known by the Environment.</param>
      /// <param name="isReady">Retrieve only when accessible (IsReady) logical drives.</param>
      /// <returns>An <see cref="Enumerable"/> <see cref="DriveInfo"/> object that represents the logical drives and network shares, on a computer.</returns>
      /// <remarks>
      /// This method retrieves all logical drive names and network shares on a computer. You can use this information to iterate through the array
      /// and obtain information on the drives using other <see cref="DriveInfo"/> methods and properties. Use the <see cref="IsReady"/> property to test
      /// whether a drive is ready because using this method on a drive that is not ready will throw a <see cref="IOException"/>.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<DriveInfo> GetDrives(bool fromEnvironment, bool isReady)
      {
         return Directory.GetLogicalDrives(fromEnvironment, isReady, false).Select(drive => new DriveInfo(null, drive));
      }

      /// <summary>Retrieves the drive names of all logical drives and network shares, on a computer.</summary>
      /// <param name="fromEnvironment">Retrieve logical drives as known by the Environment.</param>
      /// <param name="isReady">Retrieve only when accessible (IsReady) logical drives.</param>
      /// <param name="removeDirectorySeparator">Remove the <see cref="Path.DirectorySeparatorChar"/> from the logical drive name.</param>
      /// <returns>An <see cref="Enumerable"/> <see cref="DriveInfo"/> object that represents the logical drives and network shares, on a computer.</returns>
      /// <remarks>
      /// This method retrieves all logical drive names and network shares on a computer. You can use this information to iterate through the array
      /// and obtain information on the drives using other <see cref="DriveInfo"/> methods and properties. Use the <see cref="IsReady"/> property to test
      /// whether a drive is ready because using this method on a drive that is not ready will throw a <see cref="IOException"/>.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static IEnumerable<DriveInfo> GetDrives(bool fromEnvironment, bool isReady, bool removeDirectorySeparator)
      {
         return Directory.GetLogicalDrives(fromEnvironment, isReady, removeDirectorySeparator).Select(drive => new DriveInfo(null, drive));
      }

      #endregion // AlphaFS

      #endregion // GetDrives()

      #region ToString

      /// <summary>Returns a drive name as a string.</summary>
      /// <returns>The name of the drive.</returns>
      /// <remarks>This method returns the Name property.</remarks>
      [SecurityCritical]
      public override string ToString()
      {
         return Name;
      }

      #endregion // ToString

      #endregion // .NET

      #region AlphaFS

      #region Private Methods

      #region GetVolumeInfo

      [NonSerialized] private DiskSpaceInfoExtended _dsie;
      [NonSerialized] private bool _initDsie;
      [NonSerialized] private DriveType _driveType = DriveType.Unknown;
      [NonSerialized] private string _dosDeviceName;
      [NonSerialized] private DirectoryInfo _rootDirectory;
      [NonSerialized] private VolumeInfo _volumeInfo;

      /// <summary>Retrieves information about the file system and volume associated with the specified root directory or filestream.</summary>
      [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      private object GetVolumeInfo(int type, int mode)
      {
         try
         {
            switch (type)
            {
               #region Volume

               // VolumeInfo properties.
               case 0:
                  if (_volumeInfo == null)
                     _volumeInfo = Volume.GetVolumeInformationInternal(Name, null, false);

                  switch (mode)
                  {
                     case 0:
                        return _volumeInfo;

                     case 1:
                        return _volumeInfo != null ? _volumeInfo.FileSystemName : string.Empty;

                     case 2:
                        return (_volumeInfo != null) ? _volumeInfo.Name : string.Empty;
                  }
                  break;

               // Volume related.
               case 1:
                  switch (mode)
                  {
                     case 0:
                        // Do not use ?? expression here.
                        if (_dosDeviceName == null)
                           _dosDeviceName = Volume.QueryDosDevice(Name).FirstOrDefault();
                        return _dosDeviceName;
                  }
                  break;

               #endregion // Volume

               #region Drive

               // Drive related.
               case 2:
                  switch (mode)
                  {
                     case 0:
                        if (_driveType == DriveType.Unknown)
                           _driveType = Volume.GetDriveType(Name);
                        return _driveType;

                     case 1:
                        // Do not use ?? expression here.
                        if (_rootDirectory == null)
                           _rootDirectory = new DirectoryInfo(_transaction, Name);
                        return _rootDirectory;
                  }
                  break;

               // DiskSpaceInfoExtended related.
               case 3:
                  if (mode == 0)
                     if (!_initDsie)
                     {
                        _dsie = new DiskSpaceInfoExtended(Name, true);
                        _dsie.Refresh();
                        _initDsie = true;
                     }
                  break;

               #endregion // Drive
            }
         }
         catch
         {
         }

         return type == 0 && mode > 0 ? string.Empty : null;
      }

      #endregion // GetVolumeInfo

      #endregion // Private Methods

      #endregion //AlphaFS

      #endregion // Methods

      #region Properties

      #region .NET

      #region AvailableFreeSpace

      /// <summary>Indicates the amount of available free space on a drive.</summary>
      /// <returns>The amount of free space available on the drive, in bytes.</returns>
      /// <remarks>
      /// This property indicates the amount of free space available on the drive. Note that this number may
      /// be different from  the TotalFreeSpace number because this property takes into account disk quotas.
      /// </remarks>
      public long AvailableFreeSpace
      {
         get
         {
            GetVolumeInfo(3, 0);
            return (long) _dsie.FreeBytesAvailable;
         }
      }

      #endregion // AvailableFreeSpace

      #region DriveFormat

      /// <summary>Gets the name of the file system, such as NTFS or FAT32.</summary>
      /// <remarks>Use DriveFormat to determine what formatting a drive uses.</remarks>
      public string DriveFormat
      {
         get { return (string) GetVolumeInfo(0, 1); }
      }

      #endregion // DriveFormat

      #region DriveType

      /// <summary>Gets the drive type.</summary>
      /// <returns>One of the <see cref="DriveType"/> values.</returns>
      /// <remarks>
      /// The DriveType property indicates whether a drive is any of: CDRom, Fixed, Unknown, Network, NoRootDirectory,
      /// Ram, Removable, or Unknown. Values are listed in the <see cref="DriveType"/> enumeration.
      /// </remarks>
      public DriveType DriveType
      {
         get { return (DriveType) GetVolumeInfo(2, 0); }
      }

      #endregion // DriveType

      #region IsReady

      /// <summary>Gets a value indicating whether a drive is ready.</summary>
      /// <returns><c>true</c> if the drive is ready; <c>false</c> if the drive is not ready.</returns>
      /// <remarks>
      /// IsReady indicates whether a drive is ready. For example, it indicates whether a CD is in a CD drive or whether
      /// a removable storage device is ready for read/write operations. If you do not test whether a drive is ready, and
      /// it is not ready, querying the drive using DriveInfo will raise an IOException.
      /// 
      /// Do not rely on IsReady() to avoid catching exceptions from other members such as TotalSize, TotalFreeSpace, and DriveFormat.
      /// Between the time that your code checks IsReady and then accesses one of the other properties
      /// (even if the access occurs immediately after the check), a drive may have been disconnected or a disk may have been removed.
      /// </remarks>
      public bool IsReady
      {
         get { return Volume.IsReady(Name); }
      }

      #endregion // IsReady

      #region Name

      /// <summary>Gets the name of a drive.</summary>
      /// <returns>The name of the drive.</returns>
      /// <remarks>This property is the name assigned to the drive, such as C:\ or E:\</remarks>
      public string Name { get; private set; }

      #endregion // Name

      #region RootDirectory

      /// <summary>Gets the root directory of a drive.</summary>
      /// <returns>A DirectoryInfo object that contains the root directory of the drive.</returns>
      public DirectoryInfo RootDirectory
      {
         get { return (DirectoryInfo) GetVolumeInfo(2, 1); }
      }

      #endregion // RootDirectory

      #region TotalFreeSpace

      /// <summary>Gets the total amount of free space available on a drive.</summary>
      /// <returns>The total free space available on a drive, in bytes.</returns>
      /// <remarks>
      /// This property indicates the total amount of free space available on the drive, not just what is available to the current user.
      /// The .NET return value is of type <see langref="long"/>, AlphaFS uses type <see lang="ulong"/> instead.
      /// </remarks>
      public long TotalFreeSpace
      {
         get
         {
            GetVolumeInfo(3, 0);
            return (long) _dsie.TotalNumberOfFreeBytes;
         }
      }

      #endregion // TotalFreeSpace

      #region TotalSize

      /// <summary>Gets the total size of storage space on a drive.</summary>
      /// <returns>The total size of the drive, in bytes.</returns>
      /// <remarks>
      /// This property indicates the total size of the drive in bytes, not just what is available to the current user.
      /// The .NET return value is of type <see langref="long"/>, AlphaFS uses type <see lang="ulong"/> instead.
      /// </remarks>
      public long TotalSize
      {
         get
         {
            GetVolumeInfo(3, 0);
            return (long) _dsie.TotalNumberOfBytes;
         }
      }

      #endregion // TotalSize

      #region VolumeLabel

      /// <summary>Gets or sets the volume label of a drive.</summary>
      /// <returns>The volume label.</returns>
      /// <remarks>
      /// The label length is determined by the operating system. For example, NTFS allows a volume label
      /// to be up to 32 characters long. Note that null is a valid VolumeLabel.
      /// </remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      public string VolumeLabel
      {
         get { return (string) GetVolumeInfo(0, 2); }
         set { Volume.SetVolumeLabel(Name, value); }
      }

      #endregion // VolumeLabel

      #endregion // .NET

      #region AlphaFS

      #region DiskSpaceInfoExtended

      /// <summary>Returns the <see cref="DiskSpaceInfoExtended"/> instance.</summary>
      public DiskSpaceInfoExtended DiskSpaceInfoExtended
      {
         get
         {
            GetVolumeInfo(3, 0);
            return _dsie;
         }
      }

      #endregion // ClusterSize

      #region IsDosDeviceSubstitute

      /// <summary>If true, this drive is a SUBST.EXE / DefineDosDevice drive mapping.</summary>
      public bool IsDosDeviceSubstitute
      {
         get { return !string.IsNullOrEmpty(DosDeviceName) && DosDeviceName.StartsWith(Path.SubstitutePrefix, StringComparison.OrdinalIgnoreCase); }
      }

      #endregion // IsDosDeviceSubstitute

      #region IsUnc

      /// <summary>If true, this drive is a unc path.</summary>
      /// <remarks>Only retrieve this information if we're dealing with a real network share mapping: http://alphafs.codeplex.com/discussions/316583</remarks>
      public bool IsUnc
      {
         get { return !IsDosDeviceSubstitute && DriveType == DriveType.Network; }
      }

      #endregion // IsUnc

      #region IsVolume

      /// <summary>Determines whether the specified volume name is a defined volume on the current computer.</summary>
      public bool IsVolume
      {
         get { return GetVolumeInfo(0, 0) != null; }
      }

      #endregion // IsVolume

      #region DosDeviceName

      /// <summary>The MS-DOS device name.</summary>
      public string DosDeviceName
      {
         get { return (string) GetVolumeInfo(1, 0); }
      }

      #endregion // DosDeviceName

      #region VolumeInfo

      /// <summary>Contains information about a file-system volume.</summary>
      /// <returns>A VolumeInfo object that contains file-system volume information of the drive.</returns>
      public VolumeInfo VolumeInfo
      {
         get { return (VolumeInfo) GetVolumeInfo(0, 0); }
      }

      #endregion // VolumeInfo
      
      #endregion // AlphaFS

      #endregion // Properties
   }
}