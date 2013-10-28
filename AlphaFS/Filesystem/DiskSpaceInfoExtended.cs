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
using System.Runtime.InteropServices;
using System.Security;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Represents information space clusters.</summary>
   /// GetDiskFreeSpace()  : Retrieves information about the specified disk, including the amount of free space on the disk.
   /// GetDiskFreeSpaceEx(): Retrieves information about the amount of space that is available on a disk volume, which is the total amount of space, the total amount of free space, and the total amount of free space available to the user that is associated with the calling thread.
   [Serializable]
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
   public struct DiskSpaceInfoExtended
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
         
         DiskSpaceInfoExtended other = obj is DiskSpaceInfoExtended ? (DiskSpaceInfoExtended) obj : new DiskSpaceInfoExtended();

         return other.DriveName.Equals(DriveName, StringComparison.OrdinalIgnoreCase) &&
                other.TotalNumberOfBytes == TotalNumberOfBytes &&
                other.TotalNumberOfClusters == TotalNumberOfClusters &&
                other.TotalNumberOfFreeBytes == TotalNumberOfFreeBytes &&
                other.SectorsPerCluster == SectorsPerCluster &&
                other.FreeBytesAvailable == FreeBytesAvailable;
      }

      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>A hash code for the current Object.</returns>
      public override int GetHashCode()
      {
         unchecked
         {
            int hash = Primes[_random];

            if (!string.IsNullOrEmpty(DriveName))
               hash = hash * Primes[1] + DriveName.GetHashCode();

            hash = hash*Primes[1] + TotalNumberOfBytes.GetHashCode();
            hash = hash*Primes[1] + TotalNumberOfClusters.GetHashCode();
            hash = hash * Primes[1] + TotalNumberOfFreeBytes.GetHashCode();
            hash = hash*Primes[1] + SectorsPerCluster.GetHashCode();
            hash = hash*Primes[1] + FreeBytesAvailable.GetHashCode();

            return hash;
         }
      }

      /// <summary>Implements the operator ==</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator ==(DiskSpaceInfoExtended left, DiskSpaceInfoExtended right)
      {
         return left.Equals(right);
      }

      /// <summary>Implements the operator !=</summary>
      /// <param name="left">A.</param>
      /// <param name="right">B.</param>
      /// <returns>The result of the operator.</returns>
      public static bool operator !=(DiskSpaceInfoExtended left, DiskSpaceInfoExtended right)
      {
         return !(left.Equals(right));
      }

      #endregion // .NET

      #region AlphaFS

      // A random prime number will be picked and add to the HasCode, each time an instance is created.
      [NonSerialized] private readonly int _random;
      [NonSerialized] private static readonly int[] Primes = new[] { 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919 };

      #endregion // AlphaFS

      #endregion // Class Internal Affairs

      #region Constructor

      /// <summary>Initializes a DiskSpaceInfoExtended structure.</summary>
      /// <param name="drivePath">A valid drive path or drive letter. This can be either uppercase or lowercase, 'a' to 'z' or a network share in the format: \\server\share</param>
      /// <param name="getClusterInfo"><c>true</c> to also retrieve disk cluster information, <c>false</c> to only retrieve size information.</param>
      /// <Remark>This is a Lazyloading object; call <see cref="Refresh()"/> to populate all properties first before accessing.</Remark>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public DiskSpaceInfoExtended(string drivePath, bool getClusterInfo) : this()
      {
         _random = new Random().Next(0, 20);
         _initGetClusterInfo = getClusterInfo;

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
         DriveName = Path.DirectorySeparatorAdd(drivePath, false);
         
         Reset();
      }

      #endregion // Constructor

      #region Fields

      #region _initGetCluster

      /// <summary>The initial "getClusterInfo" indicator that was passed to the constructor.</summary>
      private readonly bool _initGetClusterInfo;

      #endregion // _initGetCluster

      #endregion // Fields

      #region Methods

      #region Refresh

      /// <summary>Refreshes the state of the object.</summary>
      public void Refresh()
      {
         // ChangeErrorMode is for the Win32 SetErrorMode() method, used to suppress possible pop-ups.
         // Minimize method calls from here.
         using (new NativeMethods.ChangeErrorMode(NativeMethods.NativeErrorMode.FailCriticalErrors))
         {
            #region Get size information.

            ulong freeBytesAvailable, totalNumberOfBytes, totalNumberOfFreeBytes;

            bool spaceInfoOk = NativeMethods.GetDiskFreeSpaceEx(DriveName, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);
            int lastError = Marshal.GetLastWin32Error();

            if (spaceInfoOk)
            {
               FreeBytesAvailable = freeBytesAvailable;
               TotalNumberOfBytes = totalNumberOfBytes;
               TotalNumberOfFreeBytes = totalNumberOfFreeBytes;
            }
            else
            {
               if (lastError != Win32Errors.NO_ERROR && lastError != Win32Errors.ERROR_NOT_READY)
                  NativeError.ThrowException(DriveName);
            }

            #endregion // Get size information.

            #region Get cluster information.

            if (_initGetClusterInfo)
            {
               uint sectorsPerCluster, bytesPerSector, numberOfFreeClusters, totalNumberOfClusters;

               bool clusterInfoOk = NativeMethods.GetDiskFreeSpace(DriveName, out sectorsPerCluster, out bytesPerSector, out numberOfFreeClusters, out totalNumberOfClusters);
               lastError = Marshal.GetLastWin32Error();

               if (clusterInfoOk)
               {
                  SectorsPerCluster = sectorsPerCluster;
                  BytesPerSector = bytesPerSector;
                  NumberOfFreeClusters = numberOfFreeClusters;
                  TotalNumberOfClusters = totalNumberOfClusters;
               }
               else
               {
                  if (lastError != Win32Errors.NO_ERROR && lastError != Win32Errors.ERROR_NOT_READY)
                     NativeError.ThrowException(DriveName);
               }
            }

            #endregion // Get cluster information.
         }
      }

      #endregion // Refresh

      #region Reset

      /// <summary>Initializes all <see cref="DiskSpaceInfoExtended"/> properties to 0.</summary>
      public void Reset()
      {
         FreeBytesAvailable = 0;
         TotalNumberOfBytes = 0;
         TotalNumberOfFreeBytes = 0;

         BytesPerSector = 0;
         NumberOfFreeClusters = 0;
         SectorsPerCluster = 0;
         TotalNumberOfClusters = 0;
      }

      #endregion // Reset

      #region ToString

      /// <summary>Returns the drive name.</summary>
      public override string ToString()
      {
         return DriveName;
      }

      #endregion // ToString

      #endregion // Methods

      #region Properties

      #region AvailableFreeSpacePercent

      /// <summary>Indicates the amount of available free space on a drive, formatted as percentage.</summary>
      public string AvailableFreeSpacePercent
      {
         get
         {
            double freeSizePct = NativeMethods.PercentCalculate(TotalNumberOfBytes - (TotalNumberOfBytes - TotalNumberOfFreeBytes), 0, TotalNumberOfBytes);
            return string.Format(CultureInfo.CurrentCulture, "{0:0.00}%", freeSizePct);
         }
      }

      #endregion // AvailableFreeSpacePercent

      #region AvailableFreeSpaceUnitSize

      /// <summary>Indicates the amount of available free space on a drive, formatted as a unit size.</summary>
      public string AvailableFreeSpaceUnitSize
      {
         get { return NativeMethods.UnitSizeToText(TotalNumberOfFreeBytes, false, UsePercentSuffix); }
      }

      #endregion // AvailableFreeSpaceUnitSize

      #region BytesPerSectorUnitSize

      /// <summary>The number of bytes per sector, formatted as a unit size.</summary>
      public string BytesPerSectorUnitSize
      {
         get { return NativeMethods.UnitSizeToText(BytesPerSector, false, UsePercentSuffix); }
      }

      #endregion // BytesPerSectorUnitSize

      #region ClusterSize

      /// <summary>Returns the Clusters size.</summary>
      public long ClusterSize
      {
         get { return SectorsPerCluster * BytesPerSector; }
      }

      #endregion // ClusterSize

      #region ClusterSizeUnitSize

      /// <summary>Returns the Clusters size, formatted as a unit size.</summary>
      public string ClusterSizeUnitSize
      {
         get { return NativeMethods.UnitSizeToText(ClusterSize, false, UsePercentSuffix); }
      }

      #endregion // ClusterSizeUnitSize

      #region DriveName

      /// <summary>Gets the name of a drive, such as C:\ or E:\</summary>
      public string DriveName { get; private set; }

      #endregion // DriveName

      #region TotalSizeUnitSize

      /// <summary>The total number of bytes on a disk that are available to the user who is associated with the calling thread, formatted as a unit size.</summary>
      public string TotalSizeUnitSize
      {
         get { return NativeMethods.UnitSizeToText(TotalNumberOfBytes, false, UsePercentSuffix); }
      }

      #endregion // TotalSizeUnitSize

      #region UsePercentSuffix

      /// <summary>true = suffix with "%"</summary>
      public bool UsePercentSuffix { get; set; }

      #endregion // UsePercentSuffix

      #region UsedSpacePercent

      /// <summary>Indicates the amount of used space on a drive, formatted as percentage.</summary>
      public string UsedSpacePercent
      {
         get
         {
            double usedSizePct = NativeMethods.PercentCalculate(TotalNumberOfBytes - FreeBytesAvailable, 0, TotalNumberOfBytes);
            return string.Format(CultureInfo.CurrentCulture, "{0:0.00}%", usedSizePct);
         }
      }

      #endregion // UsedSpacePercent

      #region UsedSpaceUnitSize

      /// <summary>Indicates the amount of used space on a drive, formatted as a unit size.</summary>
      public string UsedSpaceUnitSize
      {
         get { return NativeMethods.UnitSizeToText(TotalNumberOfBytes - FreeBytesAvailable, false, UsePercentSuffix); }
      }

      #endregion // UsedSpaceUnitSize


      #region FreeBytesAvailable

      /// <summary>The total number of free bytes on a disk that are available to the user who is associated with the calling thread.</summary>
      /// <remarks>GetDiskFreeSpaceEx()</remarks>
      public ulong FreeBytesAvailable { get; private set; }

      #endregion // FreeBytesAvailable

      #region TotalNumberOfBytes

      /// <summary>The total number of bytes on a disk that are available to the user who is associated with the calling thread.</summary>
      /// <remarks>GetDiskFreeSpaceEx()</remarks>
      public ulong TotalNumberOfBytes { get; private set; }

      #endregion // TotalNumberOfBytes

      #region TotalNumberOfFreeBytes

      /// <summary>The total number of free bytes on a disk.</summary>
      /// <remarks>GetDiskFreeSpaceEx()</remarks>
      public ulong TotalNumberOfFreeBytes { get; private set; }

      #endregion // TotalNumberOfFreeBytes


      #region BytesPerSector

      /// <summary>The number of bytes per sector.</summary>
      /// <remarks>GetDiskFreeSpace()</remarks>
      public uint BytesPerSector { get; private set; }

      #endregion // BytesPerSector

      #region NumberOfFreeClusters

      /// <summary>The total number of free clusters on the disk that are available to the user who is associated with the calling thread.</summary>
      /// <remarks>GetDiskFreeSpace()</remarks>
      public uint NumberOfFreeClusters { get; private set; }

      #endregion // NumberOfFreeClusters

      #region SectorsPerCluster

      /// <summary>The number of sectors per cluster.</summary>
      /// <remarks>GetDiskFreeSpace()</remarks>
      public uint SectorsPerCluster { get; private set; }

      #endregion // SectorsPerCluster

      #region TotalNumberOfClusters

      /// <summary>The total number of clusters on the disk that are available to the user who is associated with the calling thread.
      /// If per-user disk quotas are in use, this value may be less than the total number of clusters on the disk.
      /// </summary>
      /// <remarks>GetDiskFreeSpace()</remarks>
      public uint TotalNumberOfClusters { get; private set; }

      #endregion // TotalNumberOfClusters

      #endregion // Properties
   }
}