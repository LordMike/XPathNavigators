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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Alphaleonis.Win32.Filesystem
{
   #region AlphaFS

   /// <summary>Provides access to a file system object, using Shell32.</summary>
   public static class Shell32
   {
      #region Fields

      internal const FileInfoAttributes DefaultFileInfoAttributes = FileInfoAttributes.Attributes | FileInfoAttributes.DisplayName | FileInfoAttributes.TypeName;

      #endregion // Fields

      #region Structs

      #region FileInfo

      /// <summary>FileInfo structure, contains information about a file system object.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct FileInfo
      {
         /// <summary>A handle to the icon that represents the file.</summary>
         /// <remarks>You are responsible for destroying this handle with DestroyIcon() when you no longer need it.</remarks>
         internal IntPtr IconHandle;

         /// <summary>The index of the icon image within the system image list.</summary>
         internal int IconIndex;

         /// <summary>An array of values that indicates the attributes of the file object.</summary>
         internal GetAttributeOf Attributes;

         /// <summary>A string that contains the name of the file as it appears in the Windows Shell, or the path and file name of the file that contains the icon representing the file.</summary>
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeMethods.MaxPath)]
         internal string DisplayName;

         /// <summary>A string that describes the type of file.</summary>
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
         internal string TypeName;
      }

      #endregion // FileInfo

      #endregion // Structs

      #region Enums

      #region FileInfoAttributes

      /// <summary>FileInfoAttributes structure, used to retrieve the different types of a file system object.</summary>
      [Flags]
      internal enum FileInfoAttributes
      {
         /// <summary>Get file system object large icon.</summary>
         LargeIcon = 0x0,

         /// <summary>Get file system object small icon.</summary>
         SmallIcon = 0x1,

         /// <summary>Get file system object open icon.</summary>
         /// <remarks>A container object displays an open icon to indicate that the container is open.</remarks>
         OpenIcon = 0x2,

         /// <summary>Get file system object Shell-sized icon.</summary>
         /// <remarks>If this attribute is not specified the function sizes the icon according to the system metric values.</remarks>
         ShellIconSize = 0x4,

         /// <summary>Get file system object by its PIDL.</summary>
         /// <remarks>Indicate that the given file contains the address of an ITEMIDLIST structure rather than a path name.</remarks>
         [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pidl", Justification = "2012-09-29: Yomodo; Stick to the original name for now.")]
         Pidl = 0x8,

         /// <summary>Indicates that the given file should not be accessed. Rather, it should act as if the given file exists and use the supplied attributes.</summary>
         /// <remarks>This flag cannot be combined with the <see cref="Attributes"/>, <see cref="ExeType"/> or <see cref="Pidl"/> attributes.</remarks>
         UseFileAttributes = 0x10,

         /// <summary>Apply the appropriate overlays to the file's icon.</summary>
         AddOverlays = 0x20,

         /// <summary>Returns the index of the overlay icon.</summary>
         /// <remarks>The value of the overlay index is returned in the upper eight bits of the iIcon member of the structure specified by psfi.</remarks>
         OverlayIndex = 0x40,

         /// <summary>Retrieve the handle to the icon that represents the file and the index of the icon within the system image list.</summary>
         Icon = 0x100,

         /// <summary>Retrieve the display name for the file.</summary>
         /// <remarks>The returned display name uses the long file name, if there is one, rather than the 8.3 form of the file name.</remarks>
         DisplayName = 0x200,

         /// <summary>Retrieve the string that describes the file's type.</summary>
         TypeName = 0x400,

         /// <summary>Retrieve the item attributes.</summary>
         /// <remarks>Will touch every file, degrading performance.</remarks>
         Attributes = 0x800,

         /// <summary>Retrieve the name of the file that contains the icon representing the file specified by the given file.</summary>
         IconLocation = 0x1000,

         /// <summary>Retrieve the type of the executable file if pszPath identifies an executable file.</summary>
         /// <remarks>This flag cannot be specified with any other attributes.</remarks>
         ExeType = 0x2000,

         /// <summary>Retrieve the index of a system image list icon.</summary>
         SysIconIndex = 0x4000,

         /// <summary>Add the link overlay to the file's icon.</summary>
         LinkOverlay = 0x8000,

         /// <summary>Blend the file's icon with the system highlight color.</summary>
         Selected = 0x10000,

         /// <summary>Indicates that <see cref="Attributes"/> contains specific attributes that are desired.</summary>
         /// <remarks>Will touch every file, degrading performance. This flag cannot be specified with the <see cref="Icon"/> attribute.</remarks>
         [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Attr")]
         AttrSpecified = 0x20000
      }

      #endregion // FileInfoAttributes

      #region GetAttributeOf

      /// <summary>Attributes that can be retrieved from a file system object.</summary>
      [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Property 'HasSubFolder' has a large value.")]
      [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
      [Flags]
      internal enum GetAttributeOf : uint
      {
         /// <summary>Default.</summary>
         None = 0x0,

         /// <summary>The specified items can be copied.</summary>
         CanCopy = 0x1,

         /// <summary>The specified items can be moved.</summary>
         CanMove = 0x2,

         /// <summary>Shortcuts can be created for the specified items.</summary>
         CanLink = 0x4,

         /// <summary>The specified items can be bound to an IStorage object through IShellFolder::BindToObject. For more information about namespace manipulation capabilities, see IStorage.</summary>
         Storage = 0x8,

         /// <summary>The specified items can be renamed. Note that this value is essentially a suggestion; not all namespace clients allow items to be renamed. However, those that do must have this attribute set.</summary>
         CanRename = 0x10,

         /// <summary>The specified items can be deleted.</summary>
         CanDelete = 0x20,

         /// <summary>The specified items have property sheets.</summary>
         HasPropSheet = 0x40,

         /// <summary>The specified items are drop targets.</summary>
         DropTarget = 0x100,

         /// <summary>Mask for the capability attributes.</summary>
         CapabilityMask = 0x177,

         /// <summary>The specified items are system items.</summary>
         ///  <remarks>Windows 7 and later.</remarks>
         System = 0x1000,

         /// <summary>The specified items are encrypted and might require special presentation.</summary>
         Encrypted = 0x2000,

         /// <summary>Accessing the item (through IStream or other storage interfaces) is expected to be a slow operation.</summary>
         IsSlow = 0x4000,

         /// <summary>The specified items are shown as dimmed and unavailable to the user.</summary>
         Ghosted = 0x8000,

         /// <summary>The specified items are shortcuts.</summary>
         Link = 0x10000,

         /// <summary>The specified objects are shared.</summary>
         Share = 0x20000,

         /// <summary>The specified items are read-only. In the case of folders, this means that new items cannot be created in those folders.</summary>
         ReadOnly = 0x40000,

         /// <summary>The item is hidden and should not be displayed unless the Show hidden files and folders option is enabled in Folder Settings.</summary>
         Hidden = 0x80000,

         /// <summary>Do not use.</summary>
         DisplayAttrMask = 0xfc000,

         /// <summary>The items are nonenumerated items and should be hidden. They are not returned through an enumerator such as that created by the IShellFolder::EnumObjects method.</summary>
         NonEnumerated = 0x100000,

         /// <summary>The items contain new content, as defined by the particular application.</summary>
         NewContent = 0x200000,

         /// <summary>Indicates that the item has a stream associated with it.</summary>
         Stream = 0x400000,

         /// <summary>Children of this item are accessible through IStream or IStorage.</summary>
         StorageAncestor = 0x800000,

         /// <summary>When specified as input, instructs the folder to validate that the items contained in a folder or Shell item array exist.</summary>
         Validate = 0x1000000,

         /// <summary>The specified items are on removable media or are themselves removable devices.</summary>
         Removable = 0x2000000,

         /// <summary>The specified items are compressed.</summary>
         Compressed = 0x4000000,

         /// <summary>The specified items can be hosted inside a web browser or Windows Explorer frame.</summary>
         Browsable = 0x8000000,

         /// <summary>The specified folders are either file system folders or contain at least one descendant (child, grandchild, or later) that is a file system folder.</summary>
         FileSysAncestor = 0x10000000,

         /// <summary>The specified items are folders.</summary>
         /// <remarks>
         /// Some items can be flagged with both <see cref="Stream"/> and <see cref="Folder"/>, such as a compressed file
         /// with a .zip file name extension. Some applications might include this flag when testing for items that are both files and containers.
         /// </remarks>
         Folder = 0x20000000,

         /// <summary>The specified folders or files are part of the file system (that is, they are files, directories, or root directories).</summary>
         FileSystem = 0x40000000,

         /// <summary>The specified folders have subfolders.</summary>
         HasSubFolder = 0x80000000,
      }

      #endregion // GetAttributeOf

      #region AssociationAttributes

      /// <summary>Provides information for the IQueryAssociations interface methods, used by Shell32.</summary>
      [Flags]
      internal enum AssociationAttributes
      {
         /// <summary>None of the following options are set.</summary>
         None = 0,

         /// <summary>Instructs not to map CLSID values to ProgID values.</summary>
         InitNoRemapClsid = 1,

         /// <summary>Identifies the value of the supplied file parameter (3rd parameter of function GetAssociation()) as an executable file name.</summary>
         /// <remarks>If this flag is not set, the root key will be set to the ProgID associated with the .exe key instead of the executable file's ProgID.</remarks>
         InitByExeName = 2,

         /// <summary>Identical to <see cref="InitByExeName"/></summary>
         OpenByExeName = 2,

         /// <summary>Specifies that when an IQueryAssociation method does not find the requested value under the root key, it should attempt to retrieve the comparable value from the * subkey.</summary>
         InitDefaultToStar = 4,

         /// <summary>Specifies that when an IQueryAssociation method does not find the requested value under the root key, it should attempt to retrieve the comparable value from the Folder subkey.</summary>
         InitDefaultToFolder = 8,

         /// <summary>Specifies that only HKEY_CLASSES_ROOT should be searched, and that HKEY_CURRENT_USER should be ignored.</summary>
         NoUserSettings = 16,

         /// <summary>Specifies that the return string should not be truncated. Instead, return an error value and the required size for the complete string.</summary>
         NoTruncate = 32,

         /// <summary>
         /// Instructs IQueryAssociations methods to verify that data is accurate.
         /// This setting allows IQueryAssociations methods to read data from the user's hard disk for verification.
         /// For example, they can check the friendly name in the registry against the one stored in the .exe file.
         /// </summary>
         /// <remarks>Setting this flag typically reduces the efficiency of the method.</remarks>
         Verify = 64,

         /// <summary>
         /// Instructs IQueryAssociations methods to ignore Rundll.exe and return information about its target.
         /// Typically IQueryAssociations methods return information about the first .exe or .dll in a command string.
         /// If a command uses Rundll.exe, setting this flag tells the method to ignore Rundll.exe and return information about its target.
         /// </summary>
         RemapRunDll = 128,

         /// <summary>Instructs IQueryAssociations methods not to fix errors in the registry, such as the friendly name of a function not matching the one found in the .exe file.</summary>
         NoFixUps = 256,
         
         /// <summary>Specifies that the BaseClass value should be ignored.</summary>
         IgnoreBaseClass = 512,

         /// <summary>Specifies that the "Unknown" ProgID should be ignored; instead, fail.</summary>
         /// <remarks>Introduced in Windows 7.</remarks>
         InitIgnoreUnknown = 1024,

         /// <summary>(No description available on MSDN)</summary>
         /// <remarks>Introduced in Windows 8.</remarks>
         InitFixedProgId = 2048,

         /// <summary>(No description available on MSDN)</summary>
         /// <remarks>Introduced in Windows 8.</remarks>
         IsProtocol = 4096 
      }

      #endregion // AssociationAttributes

      #region AssociationString

      /// <summary>Used by the GetAssociation() function (IQueryAssociations) to define the type of string that is to be returned.</summary>
      internal enum AssociationString
      {
         /// <summary>A command string associated with a Shell verb.</summary>
         Command = 1,

         /// <summary>
         /// An executable from a Shell verb command string.
         /// For example, this string is found as the (Default) value for a subkey such as HKEY_CLASSES_ROOT\ApplicationName\shell\Open\command.
         /// If the command uses Rundll.exe, set the <see cref="AssociationAttributes.RemapRunDll"/> flag in the attributes parameter of IQueryAssociations::GetString to retrieve the target executable.
         /// </summary>
         Executable,

         /// <summary>The friendly name of a document type.</summary>
         FriendlyDocName,

         /// <summary>The friendly name of an executable file.</summary>
         FriendlyAppName,

         /// <summary>Ignore the information associated with the open subkey.</summary>
         NoOpen,

         /// <summary>Look under the ShellNew subkey.</summary>
         ShellNewValue,

         /// <summary>A template for DDE commands.</summary>
         DDECommand,

         /// <summary>The DDE command to use to create a process.</summary>
         DDEIfExec,

         /// <summary>The application name in a DDE broadcast.</summary>
         DDEApplication,

         /// <summary>The topic name in a DDE broadcast.</summary>
         DDETopic,

         /// <summary>
         /// Corresponds to the InfoTip registry value.
         /// Returns an info tip for an item, or list of properties in the form of an IPropertyDescriptionList from which to create an info tip, such as when hovering the cursor over a file name.
         /// The list of properties can be parsed with PSGetPropertyDescriptionListFromString.
         /// </summary>
         InfoTip,

         /// <summary>
         /// Corresponds to the QuickTip registry value. This is the same as <see cref="InfoTip"/>, except that it always returns a list of property names in the form of an IPropertyDescriptionList.
         /// The difference between this value and <see cref="InfoTip"/> is that this returns properties that are safe for any scenario that causes slow property retrieval, such as offline or slow networks.
         /// Some of the properties returned from <see cref="InfoTip"/> might not be appropriate for slow property retrieval scenarios.
         /// The list of properties can be parsed with PSGetPropertyDescriptionListFromString.
         /// </summary>
         QuickTip,

         /// <summary>
         /// Corresponds to the TileInfo registry value. Contains a list of properties to be displayed for a particular file type in a Windows Explorer window that is in tile view.
         /// This is the same as <see cref="InfoTip"/>, but, like <see cref="QuickTip"/>, it also returns a list of property names in the form of an IPropertyDescriptionList.
         /// The list of properties can be parsed with PSGetPropertyDescriptionListFromString.
         /// </summary>
         TileInfo,

         /// <summary>
         /// Describes a general type of MIME file association, such as image and bmp,
         /// so that applications can make general assumptions about a specific file type.
         /// </summary>
         ContentType,

         /// <summary>
         /// Returns the path to the icon resources to use by default for this association.
         /// Positive numbers indicate an index into the dll's resource table, while negative numbers indicate a resource ID.
         /// An example of the syntax for the resource is "c:\myfolder\myfile.dll,-1".
         /// </summary>
         DefaultIcon,

         /// <summary>
         /// For an object that has a Shell extension associated with it,
         /// you can use this to retrieve the CLSID of that Shell extension object by passing a string representation
         /// of the IID of the interface you want to retrieve as the pwszExtra parameter of IQueryAssociations::GetString.
         /// For example, if you want to retrieve a handler that implements the IExtractImage interface,
         /// you would specify "{BB2E617C-0920-11d1-9A0B-00C04FC2D6C1}", which is the IID of IExtractImage.
         /// </summary>
         ShellExtension,

         /// <summary>
         /// For a verb invoked through COM and the IDropTarget interface, you can use this flag to retrieve the IDropTarget object's CLSID.
         /// This CLSID is registered in the DropTarget subkey.
         /// The verb is specified in the supplied file parameter in the call to IQueryAssociations::GetString.
         /// </summary>
         DropTarget,

         /// <summary>
         /// For a verb invoked through COM and the IExecuteCommand interface, you can use this flag to retrieve the IExecuteCommand object's CLSID.
         /// This CLSID is registered in the verb's command subkey as the DelegateExecute entry.
         /// The verb is specified in the supplied file parameter in the call to IQueryAssociations::GetString.
         /// </summary>
         DelegateExecute,

         /// <summary>(No description available on MSDN)</summary>
         /// <remarks>Introduced in Windows 8.</remarks>
         SupportedUriProtocols,

         /// <summary>The maximum defined <see cref="AssociationString"/> value, used for validation purposes.</summary>
         Max
      }

      #endregion // AssociationString

      #region UrlTypes

      /// <summary>Used by method UrlIs() to define a URL type.</summary>
      internal enum UrlTypes
      {
         /// <summary>Is the URL valid?</summary>
         IsUrl = 0,

         /// <summary>Is the URL opaque?</summary>
         IsOpaque = 1,

         /// <summary>Is the URL a URL that is not typically tracked in navigation history?</summary>
         IsNoHistory = 2,

         /// <summary>Is the URL a file URL?</summary>
         IsFileUrl = 3,

         /// <summary>Attempt to determine a valid scheme for the URL.</summary>
         IsAppliable = 4,

         /// <summary>Does the URL string end with a directory?</summary>
         IsDirectory = 5,

         /// <summary>Does the URL have an appended query string?</summary>
         IsHasquery = 6
      }

      #endregion // UrlTypes

      #endregion // Enums

      #region Methods

      #region FindExecutable

      /// <summary>Retrieves the name of the executable (.exe) file associated with a specific document file.
      /// This is the application that is launched when the document file is directly double-clicked or when Open is chosen from the file's shortcut menu.
      /// </summary>
      /// <param name="file">The file to search for, this file should be a document.</param>
      /// <returns>A path <see cref="string"/> to the found executable. If nothing is found or in case of errror, <see cref="string.Empty"/> is returned.</returns>
      /// <remarks>Use FindExecutable() for documents.</remarks>
      [SecurityCritical]
      public static string FindExecutable(string file)
      {
         return FindExecutable(file, null);
      }

      /// <summary>Retrieves the name of the executable (.exe) file associated with a specific document file.
      /// This is the application that is launched when the document file is directly double-clicked or when Open is chosen from the file's shortcut menu.
      /// </summary>
      /// <param name="file">The file to search for, this file should be a document.</param>
      /// <param name="path">A path to the default directory, This value can be null.</param>
      /// <returns>A path <see cref="string"/> to the found executable. If nothing is found or in case of errror, <see cref="string.Empty"/> is returned.</returns>
      /// <remarks>Use FindExecutable() for documents.</remarks>
      [SecurityCritical]
      public static string FindExecutable(string file, string path)
      {
         if (string.IsNullOrEmpty(file))
            throw new ArgumentNullException("file");

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.

         // Path is allowed to be null.
         string pathLp = (string.IsNullOrEmpty(path)) ? null : Path.PrefixLongPath(path);

         // According to MSDN, may not exceed MAX_PATH.
         StringBuilder sb = new StringBuilder(NativeMethods.MaxPath);

         using (SafeFindFileHandle handle = NativeMethods.FindExecutable(file, pathLp, sb))
         {
            int lastError = Marshal.GetLastWin32Error();

            if (!NativeMethods.IsValidHandle(handle, false))
               NativeError.ThrowException(lastError, path);

            // Returns a value greater than 32 if successful, or a value less than or equal to 32 representing an error. 
            return lastError > 32 && sb.Length > 0 ? sb.ToString() : string.Empty;
         }
      }

      #endregion // FindExecutable

      #region GetAssociation

      /// <summary>Searches for and retrieves a file or protocol association-related string from the registry.</summary>
      /// <param name="path">A path to the file.</param>
      /// <returns>
      /// Returns the associated file- or protocol-related string from the registry.
      /// If no association can be found, <see cref="string.Empty"/> is returned.
      /// If <paramref name="path"/> is <see langword="null"/>, <see langword="null"/> will also be returned.
      /// </returns>
      /// <remarks>Default used: <see cref="AssociationAttributes.Verify"/></remarks>
      /// <remarks>Default used: <see cref="AssociationString.Executable"/></remarks>
      [SecurityCritical]
      internal static string GetAssociation(string path)
      {
         return GetAssociation(path, AssociationAttributes.Verify, AssociationString.Executable);
      }

      /// <summary>Searches for and retrieves a file or protocol association-related string from the registry.</summary>
      /// <param name="path">A path to the file.</param>
      /// <param name="attributes">A <see cref="AssociationAttributes"/> attribute. Only one "InitXXX" attribute can be used.</param>
      /// <returns>
      /// Returns the associated file- or protocol-related string from the registry.
      /// If no association can be found, <see cref="string.Empty"/> is returned.
      /// If <paramref name="path"/> is <see langword="null"/>, <see langword="null"/> will also be returned.
      /// </returns>
      /// <remarks>Default used: <see cref="AssociationString.Executable"/></remarks>
      [SecurityCritical]
      internal static string GetAssociation(string path, AssociationAttributes attributes)
      {
         return GetAssociation(path, attributes, AssociationString.Executable);
      }

      /// <summary>Searches for and retrieves a file or protocol association-related string from the registry.</summary>
      /// <param name="path">A path to a file.</param>
      /// <param name="attributes">A <see cref="AssociationAttributes"/> attribute. Only one "InitXXX" attribute can be used.</param>
      /// <param name="associationType">A <see cref="AssociationString"/> attribute.</param>
      /// <returns>
      /// Returns the associated file- or protocol-related string from the registry.
      /// If no association can be found, <see cref="string.Empty"/> is returned.
      /// If <paramref name="path"/> is <see langword="null"/>, <see langword="null"/> will also be returned.
      /// </returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      internal static string GetAssociation(string path, AssociationAttributes attributes, AssociationString associationType)
      {
         if (path == null)
            return null;

         uint bufferSize = NativeMethods.MaxPathUnicode;
         StringBuilder buffer = new StringBuilder((int) bufferSize);

         try
         {
            // In Debug mode expect: "PInvokeStackImbalance was detected".
            uint retVal = NativeMethods.AssocQueryString(attributes, associationType, path, null, buffer, out bufferSize);
            int lastError = Marshal.GetLastWin32Error();

            if ((uint) lastError == Win32Errors.E_POINTER || retVal != Win32Errors.S_OK)
            {
               // Buffer is too small.
               buffer = new StringBuilder((int) bufferSize);
               retVal = NativeMethods.AssocQueryString(attributes, associationType, path, null, buffer, out bufferSize);
               lastError = Marshal.GetLastWin32Error();

               if ((uint) lastError == Win32Errors.E_POINTER || retVal == Win32Errors.S_OK)
                  NativeError.ThrowException(lastError);
            }

            return buffer.ToString();
         }
         catch
         {
            return string.Empty;
         }
      }

      #endregion // GetAssociation

      #region GetFileType

      /// <summary>Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.</summary>
      /// <param name="path">The path to the file system object which should not exceed <see cref="NativeMethods.MaxPath"/>. Both absolute and relative paths are valid.</param>
      /// <returns>A string that describes the type of file, or null in case of failure or when type is unknown.</returns>
      [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
      [SecurityCritical]
      public static string GetFileType(string path)
      {
         try
         {
            FileInfo fileType = SHGetFileInfo(path, FileAttributes.Normal, DefaultFileInfoAttributes);
            return (string.IsNullOrEmpty(fileType.TypeName)) ? null : fileType.TypeName;
         }
         catch
         {
            return null;
         }
      }

      #endregion // GetFileType

      #region PathCreateFromUrl

      /// <summary>Converts a file URL to a Microsoft MS-DOS path.</summary>
      /// <param name="urlPath">A string that contains the file URL.</param>
      /// <returns>
      /// A <see cref="string"/> containing a Microsoft MS-DOS path.
      /// If no path can be created, <see cref="string.Empty"/> is returned.
      /// If <paramref name="urlPath"/> is <see langword="null"/>, <see langword="null"/> will also be returned.
      /// </returns>
      [SecurityCritical]
      internal static string PathCreateFromUrl(string urlPath)
      {
         if (urlPath == null)
            return null;

         StringBuilder buffer = new StringBuilder(NativeMethods.MaxPathUnicode);
         uint bufferSize = (uint)buffer.Capacity;

         // In Debug mode expect: "PInvokeStackImbalance was detected".
         uint lastError = NativeMethods.PathCreateFromUrl(urlPath, buffer, ref bufferSize, 0);

         // Don't throw exception, but return string.Empty;
         return lastError == Win32Errors.S_OK ? buffer.ToString() : string.Empty;
      }

      #endregion // PathCreateFromUrl

      #region PathCreateFromUrlAlloc

      /// <summary>Creates a path from a file URL.</summary>
      /// <param name="urlPath">A string that contains the URL.</param>
      /// <returns>
      /// A <see cref="string"/> containing the file path.
      /// If no path can be created, <see cref="string.Empty"/> is returned.
      /// If <paramref name="urlPath"/> is <see langword="null"/>, <see langword="null"/> will also be returned.
      /// </returns>
      [SecurityCritical]
      internal static string PathCreateFromUrlAlloc(string urlPath)
      {
         if (urlPath == null)
            return null;

         StringBuilder buffer = new StringBuilder(NativeMethods.MaxPathUnicode);

         // In Debug mode expect: "PInvokeStackImbalance was detected".
         uint lastError = NativeMethods.PathCreateFromUrlAlloc(urlPath, ref buffer, 0);

         // Don't throw exception, but return string.Empty;
         return lastError == Win32Errors.S_OK ? buffer.ToString() : string.Empty;
      }

      #endregion // PathCreateFromUrlAlloc

      #region PathFileExists

      /// <summary>Determines whether a path to a file system object such as a file or folder is valid.</summary>
      /// <param name="path">A string of maximum length <see cref="NativeMethods.MaxPath"/> that contains the full path of the object to verify.</param>
      /// <returns><c>true</c> if the file exists; otherwise, <c>false</c></returns>
      [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "lastError")]
      [SecurityCritical]
      internal static bool PathFileExists(string path)
      {
         if (string.IsNullOrEmpty(path))
            return false;

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);

         return NativeMethods.PathFileExists(pathLp);
      }

      #endregion // PathFileExists

      #region SHGetFileInfo

      /// <summary>Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.</summary>
      /// <param name="path">The path to the file system object which should not exceed <see cref="NativeMethods.MaxPath"/>. Both absolute and relative paths are valid.</param>
      /// <returns>A <see cref="Shell32.FileInfo"/> struct object.</returns>
      /// <remarks>You should call this function from a background thread. Failure to do so could cause the UI to stop responding.</remarks>
      /// <remarks>LongPaths not supported.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static FileInfo SHGetFileInfo(string path)
      {
         return SHGetFileInfo(path, FileAttributes.Normal, DefaultFileInfoAttributes);
      }

      /// <summary>Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.</summary>
      /// <param name="path">The path to the file system object which should not exceed <see cref="NativeMethods.MaxPath"/>. Both absolute and relative paths are valid.</param>
      /// <param name="attributes">A <see cref="FileAttributes"/> attribute.</param>
      /// <returns>A <see cref="Shell32.FileInfo"/> struct object.</returns>
      /// <remarks>You should call this function from a background thread. Failure to do so could cause the UI to stop responding.</remarks>
      /// <remarks>LongPaths not supported.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static FileInfo SHGetFileInfo(string path, FileAttributes attributes)
      {
         return SHGetFileInfo(path, attributes, DefaultFileInfoAttributes);
      }

      /// <summary>Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.</summary>
      /// <param name="path">The path to the file system object which should not exceed <see cref="NativeMethods.MaxPath"/>. Both absolute and relative paths are valid.</param>
      /// <param name="attributes">A <see cref="FileAttributes"/> attribute.</param>
      /// <param name="fileInfoAttributes">A <see cref="Shell32.FileInfoAttributes"/> attribute.</param>
      /// <returns>A <see cref="Shell32.FileInfo"/> struct object.</returns>
      /// <remarks>You should call this function from a background thread. Failure to do so could cause the UI to stop responding.</remarks>
      /// <remarks>LongPaths not supported.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static FileInfo SHGetFileInfo(string path, FileAttributes attributes, FileInfoAttributes fileInfoAttributes)
      {
         // Prevent possible crash.
         FileInfo fileInfo = new FileInfo { DisplayName = string.Empty, TypeName = string.Empty };

         if (!string.IsNullOrEmpty(path))
         {
            // In the ANSI version of this function, the name is limited to 248 characters.
            // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
            // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
            // However, the function fails when using LongPath notation.
            string pathRp = Path.GetRegularPath(path);

            UIntPtr shGetFileInfo = NativeMethods.SHGetFileInfo(pathRp, attributes, out fileInfo, (uint)Marshal.SizeOf(fileInfo), fileInfoAttributes);

            if (shGetFileInfo == UIntPtr.Zero)
               NativeError.ThrowException();
         }

         return fileInfo;
      }

      #endregion // SHGetFileInfo

      #region UrlIs

      /// <summary>Tests whether a URL is a specified type.</summary>
      /// <param name="url">A string containing the URL.</param>
      /// <param name="urlType"> </param>
      /// <returns>
      /// For all but one of the URL types, UrlIs returns true if the URL is the specified type, or false if not.
      /// If UrlIs is set to <see cref="UrlTypes.IsAppliable"/>, UrlIs will attempt to determine the URL scheme.
      /// If the function is able to determine a scheme, it returns true, or false otherwise.
      /// </returns>
      [SecurityCritical]
      internal static bool UrlIs(string url, UrlTypes urlType)
      {
         return NativeMethods.UrlIs(url, urlType);
      }

      #endregion // UrlIs

      #region UrlCreateFromPath

      /// <summary>Converts a Microsoft MS-DOS path to a canonicalized URL.</summary>
      /// <param name="path"></param>
      /// <returns>A <see cref="string"/> containing the URL or <see langword="null"/> on failure or when <see param="path"/> is also <see langword="null"/>.</returns>
      /// <returns>
      /// A <see cref="string"/> containing the URL.
      /// If no URL can be created, <see cref="string.Empty"/> is returned.
      /// If <paramref name="path"/> is <see langword="null"/>, <see langword="null"/> will also be returned.
      /// </returns>
      [SecurityCritical]
      internal static string UrlCreateFromPath(string path)
      {
         if (path == null)
            return null;

         // UrlCreateFromPath does not support extended paths.
         string pathRp = Path.GetRegularPath(path);

         StringBuilder buffer = new StringBuilder(NativeMethods.MaxPathUnicode);
         uint bufferSize = (uint)buffer.Capacity;

         // In Debug mode expect: "PInvokeStackImbalance was detected".
         uint lastError = NativeMethods.UrlCreateFromPath(pathRp, buffer, ref bufferSize, 0);

         // Don't throw exception, but return string.Empty;
         return lastError == Win32Errors.S_OK ? buffer.ToString() : string.Empty;
      }

      #endregion // UrlCreateFromPath

      #region UrlIsFileUrl

      /// <summary>Tests a URL to determine if it is a file URL.</summary>
      /// <param name="url">A string containing the URL.</param>
      /// <returns>Returns true if the URL is a file URL, or false otherwise.</returns>
      [SecurityCritical]
      internal static bool UrlIsFileUrl(string url)
      {
         return NativeMethods.UrlIs(url, UrlTypes.IsFileUrl);
      }

      #endregion // UrlIsFileUrl

      #region UrlIsNoHistory

      /// <summary>Returns whether a URL is a URL that browsers typically do not include in navigation history.</summary>
      /// <param name="url">A string containing the URL.</param>
      /// <returns>Returns true if the URL is a URL that is not included in navigation history, or false otherwise.</returns>
      [SecurityCritical]
      internal static bool UrlIsNoHistory(string url)
      {
         return NativeMethods.UrlIs(url, UrlTypes.IsNoHistory);
      }

      #endregion // UrlIsNoHistory

      #region UrlIsOpaque

      /// <summary>Returns whether a URL is opaque.</summary>
      /// <param name="url">A string containing the URL.</param>
      /// <returns>Returns true if the URL is opaque, or false otherwise.</returns>
      [SecurityCritical]
      internal static bool UrlIsOpaque(string url)
      {
         return NativeMethods.UrlIs(url, UrlTypes.IsOpaque);
      }

      #endregion // UrlIsOpaque

      #endregion // Methods
   }

   #endregion // AlphaFS
}
