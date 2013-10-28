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
using System.IO;

namespace Alphaleonis.Win32.Filesystem
{
   //[Flags]
   internal enum DeviceIoControlCode : uint
   {
      //// STORAGE
      //StorageBase = DeviceIoControlFileDevice.MassStorage,
      //StorageCheckVerify = (StorageBase << 16) | (0x0200 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageCheckVerify2 = (StorageBase << 16) | (0x0200 << 2) | DeviceIoControlMethod.Buffered | (0 << 14), // FileAccess.Any
      //StorageMediaRemoval = (StorageBase << 16) | (0x0201 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageEjectMedia = (StorageBase << 16) | (0x0202 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageLoadMedia = (StorageBase << 16) | (0x0203 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageLoadMedia2 = (StorageBase << 16) | (0x0203 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //StorageReserve = (StorageBase << 16) | (0x0204 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageRelease = (StorageBase << 16) | (0x0205 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageFindNewDevices = (StorageBase << 16) | (0x0206 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageEjectionControl = (StorageBase << 16) | (0x0250 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //StorageMcnControl = (StorageBase << 16) | (0x0251 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //StorageGetMediaTypes = (StorageBase << 16) | (0x0300 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //StorageGetMediaTypesEx = (StorageBase << 16) | (0x0301 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //StorageResetBus = (StorageBase << 16) | (0x0400 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageResetDevice = (StorageBase << 16) | (0x0401 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageGetDeviceNumber = (StorageBase << 16) | (0x0420 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //StoragePredictFailure = (StorageBase << 16) | (0x0440 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //StorageObsoleteResetBus = (StorageBase << 16) | (0x0400 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //StorageObsoleteResetDevice = (StorageBase << 16) | (0x0401 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      
      //// DISK
      DiskBase = DeviceIoControlFileDevice.Disk,
      //DiskGetDriveGeometry = (DiskBase << 16) | (0x0000 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      DiskGetDriveGeometryEx = (DiskBase << 16) | (0x0028 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskGetPartitionInfo = (DiskBase << 16) | (0x0001 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSetPartitionInfo = (DiskBase << 16) | (0x0002 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskGetDriveLayout = (DiskBase << 16) | (0x0003 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSetDriveLayout = (DiskBase << 16) | (0x0004 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskVerify = (DiskBase << 16) | (0x0005 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskFormatTracks = (DiskBase << 16) | (0x0006 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskReassignBlocks = (DiskBase << 16) | (0x0007 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskPerformance = (DiskBase << 16) | (0x0008 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskIsWritable = (DiskBase << 16) | (0x0009 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskLogging = (DiskBase << 16) | (0x000a << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskFormatTracksEx = (DiskBase << 16) | (0x000b << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskHistogramStructure = (DiskBase << 16) | (0x000c << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskHistogramData = (DiskBase << 16) | (0x000d << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskHistogramReset = (DiskBase << 16) | (0x000e << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskRequestStructure = (DiskBase << 16) | (0x000f << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskRequestData = (DiskBase << 16) | (0x0010 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskControllerNumber = (DiskBase << 16) | (0x0011 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskSmartGetVersion = (DiskBase << 16) | (0x0020 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSmartSendDriveCommand = (DiskBase << 16) | (0x0021 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskSmartRcvDriveData = (DiskBase << 16) | (0x0022 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskUpdateDriveSize = (DiskBase << 16) | (0x0032 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskGrowPartition = (DiskBase << 16) | (0x0034 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskGetCacheInformation = (DiskBase << 16) | (0x0035 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSetCacheInformation = (DiskBase << 16) | (0x0036 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskDeleteDriveLayout = (DiskBase << 16) | (0x0040 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskFormatDrive = (DiskBase << 16) | (0x00f3 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskSenseDevice = (DiskBase << 16) | (0x00f8 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //DiskCheckVerify = (DiskBase << 16) | (0x0200 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskMediaRemoval = (DiskBase << 16) | (0x0201 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskEjectMedia = (DiskBase << 16) | (0x0202 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskLoadMedia = (DiskBase << 16) | (0x0203 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskReserve = (DiskBase << 16) | (0x0204 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskRelease = (DiskBase << 16) | (0x0205 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskFindNewDevices = (DiskBase << 16) | (0x0206 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskGetMediaTypes = (DiskBase << 16) | (0x0300 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      
      //// CHANGER
      //ChangerBase = DeviceIoControlFileDevice.Changer,
      //ChangerGetParameters = (ChangerBase << 16) | (0x0000 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerGetStatus = (ChangerBase << 16) | (0x0001 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerGetProductData = (ChangerBase << 16) | (0x0002 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerSetAccess = (ChangerBase << 16) | (0x0004 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //ChangerGetElementStatus = (ChangerBase << 16) | (0x0005 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //ChangerInitializeElementStatus = (ChangerBase << 16) | (0x0006 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerSetPosition = (ChangerBase << 16) | (0x0007 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerExchangeMedium = (ChangerBase << 16) | (0x0008 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerMoveMedium = (ChangerBase << 16) | (0x0009 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerReinitializeTarget = (ChangerBase << 16) | (0x000A << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerQueryVolumeTags = (ChangerBase << 16) | (0x000B << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      
      //// FILESYSTEM
      //FsctlRequestOplockLevel1 = (DeviceIoControlFileDevice.FileSystem << 16) | (0 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlRequestOplockLevel2 = (DeviceIoControlFileDevice.FileSystem << 16) | (1 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlRequestBatchOplock = (DeviceIoControlFileDevice.FileSystem << 16) | (2 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlOplockBreakAcknowledge = (DeviceIoControlFileDevice.FileSystem << 16) | (3 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlOpBatchAckClosePending = (DeviceIoControlFileDevice.FileSystem << 16) | (4 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlOplockBreakNotify = (DeviceIoControlFileDevice.FileSystem << 16) | (5 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlLockVolume = (DeviceIoControlFileDevice.FileSystem << 16) | (6 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlUnlockVolume = (DeviceIoControlFileDevice.FileSystem << 16) | (7 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlDismountVolume = (DeviceIoControlFileDevice.FileSystem << 16) | (8 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlIsVolumeMounted = (DeviceIoControlFileDevice.FileSystem << 16) | (10 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlIsPathnameValid = (DeviceIoControlFileDevice.FileSystem << 16) | (11 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlMarkVolumeDirty = (DeviceIoControlFileDevice.FileSystem << 16) | (12 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlQueryRetrievalPointers = (DeviceIoControlFileDevice.FileSystem << 16) | (14 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlGetCompression = (DeviceIoControlFileDevice.FileSystem << 16) | (15 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      FsctlSetCompression = (DeviceIoControlFileDevice.FileSystem << 16) | (16 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlMarkAsSystemHive = (DeviceIoControlFileDevice.FileSystem << 16) | (19 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlOplockBreakAckNo2 = (DeviceIoControlFileDevice.FileSystem << 16) | (20 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlInvalidateVolumes = (DeviceIoControlFileDevice.FileSystem << 16) | (21 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlQueryFatBpb = (DeviceIoControlFileDevice.FileSystem << 16) | (22 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlRequestFilterOplock = (DeviceIoControlFileDevice.FileSystem << 16) | (23 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlFileSystemGetStatistics = (DeviceIoControlFileDevice.FileSystem << 16) | (24 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlGetNtfsVolumeData = (DeviceIoControlFileDevice.FileSystem << 16) | (25 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlGetNtfsFileRecord = (DeviceIoControlFileDevice.FileSystem << 16) | (26 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlGetVolumeBitmap = (DeviceIoControlFileDevice.FileSystem << 16) | (27 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlGetRetrievalPointers = (DeviceIoControlFileDevice.FileSystem << 16) | (28 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlMoveFile = (DeviceIoControlFileDevice.FileSystem << 16) | (29 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlIsVolumeDirty = (DeviceIoControlFileDevice.FileSystem << 16) | (30 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlGetHfsInformation = (DeviceIoControlFileDevice.FileSystem << 16) | (31 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlAllowExtendedDasdIo = (DeviceIoControlFileDevice.FileSystem << 16) | (32 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlReadPropertyData = (DeviceIoControlFileDevice.FileSystem << 16) | (33 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlWritePropertyData = (DeviceIoControlFileDevice.FileSystem << 16) | (34 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlFindFilesBySid = (DeviceIoControlFileDevice.FileSystem << 16) | (35 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlDumpPropertyData = (DeviceIoControlFileDevice.FileSystem << 16) | (37 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlSetObjectId = (DeviceIoControlFileDevice.FileSystem << 16) | (38 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlGetObjectId = (DeviceIoControlFileDevice.FileSystem << 16) | (39 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlDeleteObjectId = (DeviceIoControlFileDevice.FileSystem << 16) | (40 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlSetReparsePoint = (DeviceIoControlFileDevice.FileSystem << 16) | (41 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      FsctlGetReparsePoint = (DeviceIoControlFileDevice.FileSystem << 16) | (42 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlDeleteReparsePoint = (DeviceIoControlFileDevice.FileSystem << 16) | (43 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlEnumUsnData = (DeviceIoControlFileDevice.FileSystem << 16) | (44 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlSecurityIdCheck = (DeviceIoControlFileDevice.FileSystem << 16) | (45 << 2) | DeviceIoControlMethod.Neither | (FileAccess.Read << 14),
      //FsctlReadUsnJournal = (DeviceIoControlFileDevice.FileSystem << 16) | (46 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlSetObjectIdExtended = (DeviceIoControlFileDevice.FileSystem << 16) | (47 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlCreateOrGetObjectId = (DeviceIoControlFileDevice.FileSystem << 16) | (48 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlSetSparse = (DeviceIoControlFileDevice.FileSystem << 16) | (49 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlSetZeroData = (DeviceIoControlFileDevice.FileSystem << 16) | (50 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Write << 14),
      //FsctlQueryAllocatedRanges = (DeviceIoControlFileDevice.FileSystem << 16) | (51 << 2) | DeviceIoControlMethod.Neither | (FileAccess.Read << 14),
      //FsctlEnableUpgrade = (DeviceIoControlFileDevice.FileSystem << 16) | (52 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Write << 14),
      //FsctlSetEncryption = (DeviceIoControlFileDevice.FileSystem << 16) | (53 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlEncryptionFsctlIo = (DeviceIoControlFileDevice.FileSystem << 16) | (54 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlWriteRawEncrypted = (DeviceIoControlFileDevice.FileSystem << 16) | (55 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlReadRawEncrypted = (DeviceIoControlFileDevice.FileSystem << 16) | (56 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlCreateUsnJournal = (DeviceIoControlFileDevice.FileSystem << 16) | (57 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlReadFileUsnData = (DeviceIoControlFileDevice.FileSystem << 16) | (58 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlWriteUsnCloseRecord = (DeviceIoControlFileDevice.FileSystem << 16) | (59 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlExtendVolume = (DeviceIoControlFileDevice.FileSystem << 16) | (60 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlQueryUsnJournal = (DeviceIoControlFileDevice.FileSystem << 16) | (61 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlDeleteUsnJournal = (DeviceIoControlFileDevice.FileSystem << 16) | (62 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlMarkHandle = (DeviceIoControlFileDevice.FileSystem << 16) | (63 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlSisCopyFile = (DeviceIoControlFileDevice.FileSystem << 16) | (64 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //FsctlSisLinkFiles = (DeviceIoControlFileDevice.FileSystem << 16) | (65 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlHsmMsg = (DeviceIoControlFileDevice.FileSystem << 16) | (66 << 2) | DeviceIoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlNssControl = (DeviceIoControlFileDevice.FileSystem << 16) | (67 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Write << 14),
      //FsctlHsmData = (DeviceIoControlFileDevice.FileSystem << 16) | (68 << 2) | DeviceIoControlMethod.Neither | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlRecallFile = (DeviceIoControlFileDevice.FileSystem << 16) | (69 << 2) | DeviceIoControlMethod.Neither | (0 << 14),
      //FsctlNssRcontrol = (DeviceIoControlFileDevice.FileSystem << 16) | (70 << 2) | DeviceIoControlMethod.Buffered | (FileAccess.Read << 14),
      
      //// VIDEO
      //VideoQuerySupportedBrightness = (DeviceIoControlFileDevice.Video << 16) | (0x0125 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //VideoQueryDisplayBrightness = (DeviceIoControlFileDevice.Video << 16) | (0x0126 << 2) | DeviceIoControlMethod.Buffered | (0 << 14),
      //VideoSetDisplayBrightness = (DeviceIoControlFileDevice.Video << 16) | (0x0127 << 2) | DeviceIoControlMethod.Buffered | (0 << 14)
   }
}