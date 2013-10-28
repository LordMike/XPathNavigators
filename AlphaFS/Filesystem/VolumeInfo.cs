/* Copyright (c) 2008-2009 Peter Palotas
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
using System.Security;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Contains information about a filesystem Volume.</summary>
   [Serializable]
   [SecurityCritical]
   public sealed class VolumeInfo
   {
      #region Constructor

      /// <summary>Create a VolumeInfo instance.</summary>
      internal VolumeInfo()
      {
      }

      #endregion // Constructor

      #region Methods

      #region ToString

      /// <summary>Returns the full path of the volume.</summary>
      public override string ToString()
      {
         return FullPath;
      }

      #endregion // ToString

      #endregion // Methods

      #region Properties

      /// <summary>The full path to the volume.</summary>
      public string FullPath { get; internal set; }

      internal NativeMethods.VolumeInfoAttributes VolumeInfoAttributes { private get; set; }


      /// <summary>Gets the name of the volume.</summary>
      /// <value>The name of the volume.</value>
      public string Name { get; internal set; }

      /// <summary>The specified volume supports preserved case of file names when it places a name on disk.</summary>
      public bool CasePreservedNames
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.CasePreservedNames); }
      }

      /// <summary>The specified volume supports case-sensitive file names.</summary>
      public bool CaseSensitiveSearch
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.CaseSensitiveSearch); }
      }

      /// <summary>The volume GUID.</summary>
      public string Guid
      {
         get
         {
            if (string.IsNullOrEmpty(_guid))
               _guid = !string.IsNullOrEmpty(FullPath) ? Volume.GetUniqueVolumeNameForPath(FullPath) : string.Empty;

            if (!string.IsNullOrEmpty(_guid) && _guid.Equals(FullPath, StringComparison.OrdinalIgnoreCase))
               _guid = string.Empty;

            return _guid;
         }
      }

      /// <summary>The specified volume supports Unicode in file names as they appear on disk.</summary>
      public bool UnicodeOnDisk
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.UnicodeOnDisk); }
      }

      /// <summary>The specified volume preserves and enforces access control lists (ACL).</summary>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acls")]
      public bool PersistentAcls
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.PersistentAcls); }
      }

      /// <summary>The specified volume supports file-based compression.</summary>
      public bool Compression
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.Compression); }
      }
      
      /// <summary>The specified volume supports disk quotas.</summary>
      public bool VolumeQuotas
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.VolumeQuotas); }
      }
      
      /// <summary>The specified volume supports sparse files.</summary>
      public bool SupportsSparseFiles
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsSparseFiles); }
      }

      /// <summary>The specified volume supports re-parse points.</summary>
      public bool SupportsReparsePoints
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsReparsePoints); }
      }

      /// <summary>The specified volume supports remote storage. (This property does not appear on MSDN)</summary>
      public bool SupportsRemoteStorage
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsRemoteStorage); }
      }

      /// <summary>The specified volume is a compressed volume, for example, a DoubleSpace volume.</summary>
      public bool VolumeIsCompressed
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.VolumeIsCompressed); }
      }

      /// <summary>The specified volume supports object identifiers.</summary>
      public bool SupportsObjectIds
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsObjectIds); }
      }

      /// <summary>The specified volume supports the Encrypted File System (EFS).</summary>
      public bool SupportsEncryption
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsEncryption); }
      }

      /// <summary>The specified volume supports named streams.</summary>
      public bool NamedStreams
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.NamedStreams); }
      }

      /// <summary>The specified volume is read-only.</summary>
      public bool ReadOnlyVolume
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.ReadOnlyVolume); }
      }

      /// <summary>The specified volume supports a single sequential write.</summary>
      public bool SequentialWriteOnce
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SequentialWriteOnce); }
      }

      /// <summary>The specified volume supports transactions.</summary>
      public bool SupportsTransactions
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsTransactions); }
      }

      /// <summary>The specified volume supports hard links.</summary>
      public bool SupportsHardLinks
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsHardLinks); }
      }

      /// <summary>The specified volume supports extended attributes.</summary>
      public bool SupportsExtendedAttributes
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsExtendedAttributes); }
      }

      /// <summary>The file system supports open by FileID.</summary>
      public bool SupportsOpenByFileId
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsOpenByFileId); }
      }

      /// <summary>The specified volume supports update sequence number (USN) journals.</summary>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Usn")]
      public bool SupportsUsnJournal
      {
         get { return NativeMethods.HasVolumeInfoAttribute(VolumeInfoAttributes, NativeMethods.VolumeInfoAttributes.SupportsUsnJournal); }
      }

      /// <summary>Gets the volume serial number that the operating system assigns when a hard disk is formatted.</summary>
      /// <value>The volume serial number that the operating system assigns when a hard disk is formatted.</value>
      public string SerialNumber { get; internal set; }

      /// <summary>Gets the maximum length of a file name component that the file system supports.</summary>
      /// <value>The maximum length of a file name component that the file system supports.</value>
      public uint MaximumComponentLength { get; internal set; }

      /// <summary>Gets the name of the file system, for example, the FAT file system or the NTFS file system.</summary>
      /// <value>The name of the file system.</value>
      public string FileSystemName { get; internal set; }

      #endregion // Properties

      #region Fields

      private string _guid;

      #endregion // Fields
   }
}