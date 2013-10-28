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
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using Alphaleonis.Win32.Security;
using Microsoft.Win32.SafeHandles;
using FileStream = System.IO.FileStream;
using StreamReader = System.IO.StreamReader;
using StreamWriter = System.IO.StreamWriter;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>Provides static methods for the creation, copying, deletion, moving, and opening of files, and aids in the creation of <see cref="FileStream"/> objects.</summary>
   public static class File
   {
      #region .NET

      #region AppendAllLines

      #region .NET

      /// <summary>Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.</summary>
      /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
      /// <param name="contents">The lines to append to the file.</param>
      /// <remarks>The method creates the file if it doesn’t exist, but it doesn't create new directories. Therefore, the value of the path parameter must contain existing directories.</remarks>
      [SecurityCritical]
      public static void AppendAllLines(string path, IEnumerable<string> contents)
      {
         WriteAppendAllLinesInternal(true, false, null, path, contents, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.</summary>
      /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
      /// <param name="contents">The lines to append to the file.</param>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      /// <remarks>The method creates the file if it doesn’t exist, but it doesn't create new directories. Therefore, the value of the path parameter must contain existing directories.</remarks>
      [SecurityCritical]
      public static void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(true, false, null, path, contents, encoding);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
      /// <param name="contents">The lines to append to the file.</param>
      /// <remarks>The method creates the file if it doesn’t exist, but it doesn't create new directories. Therefore, the value of the path parameter must contain existing directories.</remarks>
      [SecurityCritical]
      public static void AppendAllLines(KernelTransaction transaction, string path, IEnumerable<string> contents)
      {
         WriteAppendAllLinesInternal(true, false, transaction, path, contents, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Appends lines to a file, and then closes the file. If the specified file does not exist, this method creates a file, writes the specified lines to the file, and then closes the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to append the lines to. The file is created if it doesn't already exist.</param>
      /// <param name="contents">The lines to append to the file.</param>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      /// <remarks>The method creates the file if it doesn’t exist, but it doesn't create new directories. Therefore, the value of the path parameter must contain existing directories.</remarks>
      [SecurityCritical]
      public static void AppendAllLines(KernelTransaction transaction, string path, IEnumerable<string> contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(true, false, transaction, path, contents, encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // AppendAllLines

      #region AppendAllText

      #region .NET

      /// <summary>Appends the specified stringto the file, creating the file if it does not already exist.</summary>
      /// <param name="path">The file to append the specified string to.</param>
      /// <param name="contents">The string to append to the file.</param>
      [SecurityCritical]
      public static void AppendAllText(string path, string contents)
      {
         WriteAppendAllLinesInternal(true, false, null, path, new[] { contents }, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Appends the specified string to the file, creating the file if it does not already exist.</summary>
      /// <param name="path">The file to append the specified string to.</param>
      /// <param name="contents">The string to append to the file.</param>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SecurityCritical]
      public static void AppendAllText(string path, string contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(true, false, null, path, new[] { contents }, encoding);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Appends the specified stringto the file, creating the file if it does not already exist.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to append the specified string to.</param>
      /// <param name="contents">The string to append to the file.</param>
      [SecurityCritical]
      public static void AppendAllText(KernelTransaction transaction, string path, string contents)
      {
         WriteAppendAllLinesInternal(true, false, transaction, path, new[] { contents }, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Appends the specified string to the file, creating the file if it does not already exist.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to append the specified string to.</param>
      /// <param name="contents">The string to append to the file.</param>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SecurityCritical]
      public static void AppendAllText(KernelTransaction transaction, string path, string contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(true, false, transaction, path, new[] { contents }, encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // AppendAllText

      #region AppendText

      #region .NET

      /// <summary>Creates a <see cref="StreamWriter"/> that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to an existing file,
      /// or to a new file if the specified file does not exist.</summary>
      /// <param name="path">The path to the file to append to.</param>
      /// <returns>A stream writer that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to the specified file or to a new file.</returns>
      [SecurityCritical]
      public static StreamWriter AppendText(string path)
      {
         return AppendTextInternal(null, path, NativeMethods.DefaultFileEncoding);
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Creates a <see cref="StreamWriter"/> that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to an existing file,
      /// or to a new file if the specified file does not exist.</summary>
      /// <param name="path">The path to the file to append to.</param>
      /// <returns>A stream writer that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to the specified file or to a new file.</returns>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SecurityCritical]
      public static StreamWriter AppendText(string path, Encoding encoding)
      {
         return AppendTextInternal(null, path, encoding);
      }

      #region Transacted

      /// <summary>Creates a <see cref="StreamWriter"/> that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to an existing file,
      /// or to a new file if the specified file does not exist.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file to append to.</param>
      /// <returns>A stream writer that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to the specified file or to a new file.</returns>
      [SecurityCritical]
      public static StreamWriter AppendText(KernelTransaction transaction, string path)
      {
         return AppendTextInternal(transaction, path, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Creates a <see cref="StreamWriter"/> that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to an existing file,
      /// or to a new file if the specified file does not exist.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file to append to.</param>
      /// <returns>A stream writer that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to the specified file or to a new file.</returns>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SecurityCritical]
      public static StreamWriter AppendText(KernelTransaction transaction, string path, Encoding encoding)
      {
         return AppendTextInternal(transaction, path, encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // AppendText

      #region Copy

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Copy

      #region Create

      #region .NET

      /// <summary>Creates or overwrites a file in the specified path.</summary>
      /// <param name="path">The path and name of the file to create.</param>
      /// <returns>A <see cref="FileStream"/> that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(string path)
      {
         return CreateFileInternal(null, path, NativeMethods.DefaultFileBufferSize, EFileAttributes.Normal, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      /// <summary>Creates or overwrites a file in the specified path, specifying a buffer size.</summary>
      /// <param name="path">The path and name of the file to create.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <returns>A <see cref="FileStream"/> with the specified buffer size that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(string path, int bufferSize)
      {
         return CreateFileInternal(null, path, bufferSize, EFileAttributes.Normal, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Creates or overwrites a file in the specified path, specifying a buffer size and a <see cref="EFileAttributes"/> value.</summary>
      /// <param name="path">The path and name of the file to create.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <param name="attributes">One of the <see cref="EFileAttributes"/> values that describes how to create or overwrite the file.</param>
      /// <returns>A <see cref="FileStream"/> that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(string path, int bufferSize, EFileAttributes attributes)
      {
         return CreateFileInternal(null, path, bufferSize, attributes, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      /// <summary>Creates or overwrites a file in the specified path, specifying a buffer size and a <see cref="EFileAttributes"/> value and a <see cref="FileSecurity"/> value.</summary>
      /// <param name="path">The path and name of the file to create.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <param name="attributes">One of the <see cref="EFileAttributes"/> values that describes how to create or overwrite the file.</param>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> instance that determines the access control and audit security for the file.</param>
      /// <returns>A <see cref="FileStream"/> with the specified buffer size that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(string path, int bufferSize, EFileAttributes attributes, FileSecurity fileSecurity)
      {
         return CreateFileInternal(null, path, bufferSize, attributes, fileSecurity, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      /// <summary>Creates or overwrites a file in the specified path, specifying a buffer size and advanced options:
      /// <see cref="EFileAttributes"/>, <see cref="FileSecurity"/>, <see cref="FileMode"/>, <see cref="FileAccess"/>, <see cref="FileShare"/>.
      /// </summary>
      /// <param name="path">The name of the file.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <param name="attributes">The <see cref="EFileAttributes"/> additional advanced options to create a file.</param>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> instance that determines the access control and audit security for the file.</param>
      /// <param name="mode">The <see cref="FileMode"/> option gives you more precise control over how you want to create a file.</param>
      /// <param name="access">The <see cref="FileAccess"/> allow you additionaly specify to default redwrite capability - just write, bypassing any cache.</param>
      /// <param name="share">The <see cref="FileShare"/> option controls how you would like to share created file with other requesters.</param>
      /// <returns>A <see cref="FileStream"/> with the specified buffer size that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(string path, int bufferSize, EFileAttributes attributes, FileSecurity fileSecurity, FileMode mode, FileAccess access, FileShare share)
      {
         return CreateFileInternal(null, path, bufferSize, attributes, fileSecurity, mode, access, share);
      }

      #region Transacted

      /// <summary>Creates or overwrites a file in the specified path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path and name of the file to create.</param>
      /// <returns>A <see cref="FileStream"/> that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(KernelTransaction transaction, string path)
      {
         return CreateFileInternal(transaction, path, NativeMethods.DefaultFileBufferSize, EFileAttributes.Normal, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      /// <summary>Creates or overwrites a file in the specified path, specifying a buffer size.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path and name of the file to create.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <returns>A <see cref="FileStream"/> with the specified buffer size that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(KernelTransaction transaction, string path, int bufferSize)
      {
         return CreateFileInternal(transaction, path, bufferSize, EFileAttributes.Normal, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      /// <summary>Creates or overwrites a file in the specified path, specifying a buffer size and a <see cref="EFileAttributes"/> value.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path and name of the file to create.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <param name="attributes">One of the <see cref="EFileAttributes"/> values that describes how to create or overwrite the file.</param>
      /// <returns>A <see cref="FileStream"/> with the specified buffer size that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(KernelTransaction transaction, string path, int bufferSize, EFileAttributes attributes)
      {
         return CreateFileInternal(transaction, path, bufferSize, attributes, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
      }

      /// <summary>Creates or overwrites a file in the specified path, specifying a buffer size and advanced options:
      /// <see cref="EFileAttributes"/>, <see cref="FileSecurity"/>, <see cref="FileMode"/>, <see cref="FileAccess"/>, <see cref="FileShare"/>.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <param name="attributes">The <see cref="EFileAttributes"/> additional advanced options to create a file.</param>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> instance that determines the access control and audit security for the file.</param>
      /// <param name="mode">The <see cref="FileMode"/> option gives you more precise control over how you want to create a file.</param>
      /// <param name="access">The <see cref="FileAccess"/> allow you additionaly specify to default redwrite capability - just write, bypassing any cache.</param>
      /// <param name="share">The <see cref="FileShare"/> option controls how you would like to share created file with other requesters.</param>
      /// <returns>A <see cref="FileStream"/> with the specified buffer size that provides read/write access to the file specified in path.</returns>
      [SecurityCritical]
      public static FileStream Create(KernelTransaction transaction, string path, int bufferSize, EFileAttributes attributes, FileSecurity fileSecurity, FileMode mode, FileAccess access, FileShare share)
      {
         return CreateFileInternal(transaction, path, bufferSize, attributes, fileSecurity, mode, access, share);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // Create

      #region CreateText

      #region .NET

      /// <summary>Creates or opens a file for writing <see cref="NativeMethods.DefaultFileEncoding"/> encoded text.</summary>
      /// <param name="path">The file to be opened for writing.</param>
      /// <returns>A <see cref="StreamWriter"/> that writes to the specified file using <see cref="NativeMethods.DefaultFileBufferSize"/> encoding.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public static StreamWriter CreateText(string path)
      {
         return CreateTextInternal(null, path, NativeMethods.DefaultFileEncoding);
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Creates or opens a file for writing <see cref="NativeMethods.DefaultFileEncoding"/> encoded text.</summary>
      /// <param name="path">The file to be opened for writing.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A <see cref="StreamWriter"/> that writes to the specified file using <see cref="NativeMethods.DefaultFileBufferSize"/> encoding.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public static StreamWriter CreateText(string path, Encoding encoding)
      {
         return CreateTextInternal(null, path, encoding);
      }

      #region Transacted

      /// <summary>Creates or opens a file for writing <see cref="NativeMethods.DefaultFileEncoding"/> encoded text.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to be opened for writing.</param>
      /// <returns>A <see cref="StreamWriter"/> that writes to the specified file using <see cref="NativeMethods.DefaultFileEncoding"/> encoding.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public static StreamWriter CreateText(KernelTransaction transaction, string path)
      {
         return CreateTextInternal(transaction, path, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Creates or opens a file for writing <see cref="NativeMethods.DefaultFileEncoding"/> encoded text.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to be opened for writing.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A <see cref="StreamWriter"/> that writes to the specified file using <see cref="NativeMethods.DefaultFileBufferSize"/> encoding.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public static StreamWriter CreateText(KernelTransaction transaction, string path, Encoding encoding)
      {
         return CreateTextInternal(transaction, path, encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // CreateText

      #region Decrypt

      #region .NET

      /// <summary>Decrypts a file that was encrypted by the current account using the Encrypt method.</summary>
      /// <param name="path">A path that describes a file to decrypt.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Decrypt(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);

         if (!NativeMethods.DecryptFile(pathLp, 0))
            NativeError.ThrowException(pathLp);

         return true;
      }

      #endregion // .NET

      #endregion // Decrypt

      #region Delete

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Delete
      
      #region Encrypt

      #region .NET

      /// <summary>Encrypts a file so that only the account used to encrypt the file can decrypt it.</summary>
      /// <param name="path">A path that describes a file to encrypt.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Encrypt(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);
         
         if (!NativeMethods.EncryptFile(pathLp))
            NativeError.ThrowException(pathLp);

         return true;
      }

      #endregion // .NET

      #endregion // Encrypt

      #region Exists

      #region .NET

      /// <summary>Determines whether the specified file exists.</summary>
      /// <param name="path">The file to check. Note that this files may contain wildcards, such as '*'.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>Note that this files may contain wildcards, such as '*'</remarks>
      /// <remarks>Return value is <c>true</c> if the caller has the required permissions and path contains the name of an existing file; otherwise, <c>false</c>.</remarks>
      /// <remarks>This method also returns <c>false</c> if path is NULL reference (Nothing in Visual Basic), an invalid path, or a zero-length string.</remarks>
      /// <remarks>If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns <c>false</c> regardless of the existence of path.</remarks>
      [SecurityCritical]
      public static bool Exists(string path)
      {
         return FileSystemInfo.ExistsInternal(false, null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Determines whether the specified file exists.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to check. Note that this files may contain wildcards, such as '*'.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>Note that this files may contain wildcards, such as '*'</remarks>
      /// <remarks>Return value is <c>true</c> if the caller has the required permissions and path contains the name of an existing file; otherwise, <c>false</c>.</remarks>
      /// <remarks>This method also returns <c>false</c> if path is NULL reference (Nothing in Visual Basic), an invalid path, or a zero-length string.</remarks>
      /// <remarks>If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns <c>false</c> regardless of the existence of path.</remarks>
      /// <remarks>A trailing backslash is not allowed and will be removed.</remarks>
      [SecurityCritical]
      public static bool Exists(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.ExistsInternal(false, transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // Exists

      #region GetAccessControl

      #region .NET

      /// <summary>Gets a <see cref="FileSecurity"/> object that encapsulates the access control list (ACL) entries for a specified file.</summary>
      /// <param name="path">The path to a file containing a <see cref="FileSecurity"/> object that describes the file's access control list (ACL) information.</param>
      /// <returns>A <see cref="FileSecurity"/> object that encapsulates the access control rules for the file described by the <paramref name="path"/> parameter.</returns>
      [SecurityCritical]
      public static FileSecurity GetAccessControl(string path)
      {
         return (FileSecurity)FileSystemInfo.GetSetAccessControlInternal(false, false, path, null, AccessControlSections.Access | AccessControlSections.Group | AccessControlSections.Owner);
      }

      /// <summary>Gets a <see cref="FileSecurity"/> object that encapsulates the access control list (ACL) entries for a specified file.</summary>
      /// <param name="path">The path to a file containing a <see cref="FileSecurity"/> object that describes the file's access control list (ACL) information.</param>
      /// <param name="includeSections">One (or more) of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to receive.</param>
      /// <returns>A <see cref="FileSecurity"/> object that encapsulates the access control rules for the file described by the <paramref name="path"/> parameter.</returns>
      [SecurityCritical]
      public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
      {
         return (FileSecurity)FileSystemInfo.GetSetAccessControlInternal(false, false, path, null, includeSections);
      }
      
      #endregion // .NET

      #endregion // GetAccessControl

      #region GetAttributes

      #region .NET

      /// <summary>Gets the <see cref="FileAttributes"/> of the file on the path.</summary>
      /// <param name="path">The path to the file.</param>
      /// <returns>The <see cref="FileAttributes"/> of the file on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(string path)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(null, path, false, true).Attributes;
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Gets the <see cref="FileAttributes"/> of the file on the path.</summary>
      /// <param name="path">The path to the file.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <returns>The <see cref="FileAttributes"/> of the file on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(string path, bool basicSearch)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(null, path, basicSearch, true).Attributes;
      }

      #region Transacted

      /// <summary>Gets the <see cref="FileAttributes"/> of the file on the path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file.</param>
      /// <returns>The <see cref="FileAttributes"/> of the file on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(transaction, path, false, true).Attributes;
      }

      /// <summary>Gets the <see cref="FileAttributes"/> of the file on the path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <returns>The <see cref="FileAttributes"/> of the file on the path.</returns>
      [SecurityCritical]
      public static FileAttributes GetAttributes(KernelTransaction transaction, string path, bool basicSearch)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(transaction, path, basicSearch, true).Attributes;
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion GetAttributes

      #region GetCreationTime

      #region .NET

      /// <summary>Returns the creation date and time of the specified file or directory.</summary>
      /// <param name="path">The file or directory for which to obtain creation date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified file or directory. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTime(string path)
      {
         return FileSystemInfo.GetCreationTimeInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the creation date and time of the specified file or directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain creation date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified file or directory. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTime(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetCreationTimeInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetCreationTime

      #region GetCreationTimeUtc

      #region .NET

      /// <summary>Gets the creation date and time, in Coordinated Universal Time (UTC) format, of a directory.</summary>
      /// <param name="path">The path of the directory.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified directory. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTimeUtc(string path)
      {
         return FileSystemInfo.GetCreationTimeUtcInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Gets the creation date and time, in Coordinated Universal Time (UTC) format, of a directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path of the directory.</param>
      /// <returns>A <see cref="DateTime"/> structure set to the creation date and time for the specified directory. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetCreationTimeUtc(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetCreationTimeUtcInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetCreationTimeUtc

      #region GetLastAccessTime

      #region .NET

      /// <summary>Returns the date and time the specified file or directory was last accessed.</summary>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTime(string path)
      {
         return FileSystemInfo.GetLastAccessTimeInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTime(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastAccessTimeInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastAccessTime

      #region GetLastAccessTimeUtc

      #region .NET

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtc(string path)
      {
         return FileSystemInfo.GetLastAccessTimeUtcInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain access date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last accessed. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastAccessTimeUtc(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastAccessTimeUtcInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastAccessTimeUtc

      #region GetLastWriteTime

      #region .NET

      /// <summary>Returns the date and time the specified file or directory was last written to.</summary>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTime(string path)
      {
         return FileSystemInfo.GetLastWriteTimeInternal(null, path);
      }

      #endregion //.NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time the specified file or directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in local time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTime(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastWriteTimeInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastWriteTime

      #region GetLastWriteTimeUtc

      #region .NET

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last written to.</summary>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtc(string path)
      {
         return FileSystemInfo.GetLastWriteTimeUtcInternal(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Returns the date and time, in Coordinated Universal Time (UTC) format, that the specified file or directory was last written to.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file or directory for which to obtain modification date and time information.</param>
      /// <returns>A <see cref="DateTime"/> structure that is set to the date and time the specified file or directory was last written to. This value is expressed in UTC time.</returns>
      [SecurityCritical]
      public static DateTime GetLastWriteTimeUtc(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetLastWriteTimeUtcInternal(transaction, path);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetLastWriteTimeUtc

      #region Move

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Move

      #region Open

      #region .NET

      /// <summary>Opens a <see cref="FileStream"/> on the specified path with read/write access.</summary>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <returns>A <see cref="FileStream"/> opened in the specified mode and path, with read/write access and not shared.</returns>
      [SecurityCritical]
      public static FileStream Open(string path, FileMode mode)
      {
         return OpenInternal(null, path, mode, 0, FileAccess.ReadWrite, FileShare.None, EFileAttributes.Normal);
      }

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, with the specified mode and access.</summary>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
      /// <returns>An unshared <see cref="FileStream"/> that provides access to the specified file, with the specified mode and access.</returns>
      [SecurityCritical]
      public static FileStream Open(string path, FileMode mode, FileAccess access)
      {
         return OpenInternal(null, path, mode, 0, access, FileShare.None, EFileAttributes.Normal);
      }

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
      /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
      /// <returns>A <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
      [SecurityCritical]
      public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
      {
         return OpenInternal(null, path, mode, 0, access, share, EFileAttributes.Normal);
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
      /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
      /// <param name="attributes">a <see cref="EFileAttributes"/> advanced options for this file.</param>
      /// <returns>A <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
      [SecurityCritical]
      public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share, EFileAttributes attributes)
      {
         return OpenInternal(null, path, mode, 0, access, share, attributes);
      }

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="rights">A <see cref="FileSystemRights"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten along with additional options.</param>
      /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
      /// <param name="attributes">a <see cref="EFileAttributes"/> advanced options for this file.</param>
      /// <returns>A <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
      [SecurityCritical]
      public static FileStream Open(string path, FileMode mode, FileSystemRights rights, FileShare share, EFileAttributes attributes)
      {
         return OpenInternal(null, path, mode, rights, 0, share, attributes);
      }

      #region Transacted

      /// <summary>)(Transacted) Opens a <see cref="FileStream"/> on the specified path with read/write access.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <returns>A <see cref="FileStream"/> opened in the specified mode and path, with read/write access and not shared.</returns>
      [SecurityCritical]
      public static FileStream Open(KernelTransaction transaction, string path, FileMode mode)
      {
         return OpenInternal(transaction, path, mode, 0, FileAccess.ReadWrite, FileShare.None, EFileAttributes.Normal);
      }

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, with the specified mode and access.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
      /// <returns>An unshared <see cref="FileStream"/> that provides access to the specified file, with the specified mode and access.</returns>
      [SecurityCritical]
      public static FileStream Open(KernelTransaction transaction, string path, FileMode mode, FileAccess access)
      {
         return OpenInternal(transaction, path, mode, 0, access, FileShare.None, EFileAttributes.Normal);
      }

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
      /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
      /// <returns>A <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
      [SecurityCritical]
      public static FileStream Open(KernelTransaction transaction, string path, FileMode mode, FileAccess access, FileShare share)
      {
         return OpenInternal(transaction, path, mode, 0, access, share, EFileAttributes.Normal);
      }

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
      /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
      /// <param name="attributes">Advanced <see cref="EFileAttributes"/> options for this file.</param>
      /// <returns>A <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
      [SecurityCritical]
      public static FileStream Open(KernelTransaction transaction, string path, FileMode mode, FileAccess access, FileShare share, EFileAttributes attributes)
      {
         return OpenInternal(transaction, path, mode, 0, access, share, attributes);
      }

      /// <summary>Opens a <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="rights">A <see cref="FileSystemRights"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten along with additional options.</param>
      /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
      /// <param name="attributes">Advanced <see cref="EFileAttributes"/> options for this file.</param>
      /// <returns>A <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
      [SecurityCritical]
      public static FileStream Open(KernelTransaction transaction, string path, FileMode mode, FileSystemRights rights, FileShare share, EFileAttributes attributes)
      {
         return OpenInternal(transaction, path, mode, rights, 0, share, attributes);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // Open

      #region OpenRead

      #region .NET

      /// <summary>Opens an existing file for reading.</summary>
      /// <param name="path">The file to be opened for reading.</param>
      /// <returns>A read-only <see cref="FileStream"/> on the specified path.</returns>
      /// <remarks>This method is equivalent to the FileStream(string, FileMode, FileAccess, FileShare) constructor overload with a <see cref="FileMode"/> value of Open, a <see cref="FileAccess"/> value of Read and a <see cref="FileShare"/> value of Read.</remarks>
      [SecurityCritical]
      public static FileStream OpenRead(string path)
      {
         return OpenInternal(null, path, FileMode.Open, 0, FileAccess.Read, FileShare.Read, EFileAttributes.Normal);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Opens an existing file for reading.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to be opened for reading.</param>
      /// <returns>A read-only <see cref="FileStream"/> on the specified path.</returns>
      /// <remarks>This method is equivalent to the FileStream(string, FileMode, FileAccess, FileShare) constructor overload with a <see cref="FileMode"/> value of Open, a <see cref="FileAccess"/> value of Read and a <see cref="FileShare"/> value of Read.</remarks>
      [SecurityCritical]
      public static FileStream OpenRead(KernelTransaction transaction, string path)
      {
         return OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Read, FileShare.Read, EFileAttributes.Normal);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // OpenRead

      #region OpenText

      #region .NET

      /// <summary>Opens an existing <see cref="NativeMethods.DefaultFileEncoding"/> encoded text file for reading.</summary>
      /// <param name="path">The file to be opened for reading.</param>
      /// <returns>A <see cref="StreamReader"/> on the specified path.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public static StreamReader OpenText(string path)
      {
         // File.OpenRead()
         return new StreamReader(OpenInternal(null, path, FileMode.Open, 0, FileAccess.Read, FileShare.None, EFileAttributes.Normal), NativeMethods.DefaultFileEncoding);
      }

      #endregion // .NET

      #region AlphaFS

      /// <summary>Opens an existing <see cref="Encoding"/> encoded text file for reading.</summary>
      /// <param name="path">The file to be opened for reading.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A <see cref="StreamReader"/> on the specified path.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      public static StreamReader OpenText(string path, Encoding encoding)
      {
         // File.OpenRead()
         return new StreamReader(OpenInternal(null, path, FileMode.Open, 0, FileAccess.Read, FileShare.None, EFileAttributes.Normal), encoding);
      }

      #region Transacted

      /// <summary>Opens an existing <see cref="NativeMethods.DefaultFileEncoding"/> encoded text file for reading.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to be opened for reading.</param>
      /// <returns>A <see cref="StreamReader"/> on the specified path.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      public static StreamReader OpenText(KernelTransaction transaction, string path)
      {
         // File.OpenRead()
         return new StreamReader(OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Read, FileShare.None, EFileAttributes.Normal), NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Opens an existing <see cref="Encoding"/> encoded text file for reading.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to be opened for reading.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A <see cref="StreamReader"/> on the specified path.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      public static StreamReader OpenText(KernelTransaction transaction, string path, Encoding encoding)
      {
         // File.OpenRead()
         return new StreamReader(OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Read, FileShare.None, EFileAttributes.Normal), encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // OpenText

      #region OpenWrite

      #region .NET

      /// <summary>Opens an existing file or creates a new file for writing.</summary>
      /// <param name="path">The file to be opened for writing.</param>
      /// <returns>An unshared <see cref="FileStream"/> object on the specified path with Write access.</returns>
      [SecurityCritical]
      public static FileStream OpenWrite(string path)
      {
         return OpenInternal(null, path, FileMode.Open, 0, FileAccess.Write, FileShare.None, EFileAttributes.Normal);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Opens an existing file or creates a new file for writing.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to be opened for writing.</param>
      /// <returns>An unshared <see cref="FileStream"/> object on the specified path with Write access.</returns>
      [SecurityCritical]
      public static FileStream OpenWrite(KernelTransaction transaction, string path)
      {
         return OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Write, FileShare.None, EFileAttributes.Normal);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // OpenWrite

      #region ReadAllBytes

      #region .NET
      
      /// <summary>Opens a binary file, reads the contents of the file into a byte array, and then closes the file.</summary>
      /// <param name="path">The file to open for reading. </param>
      /// <returns>A Byte array containing the contents of the file.</returns>
      [SecurityCritical]
      public static Byte[] ReadAllBytes(string path)
      {
         return ReadAllBytes(null, path);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Opens a binary file, reads the contents of the file into a byte array, and then closes the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open for reading.</param>
      /// <returns>A Byte array containing the contents of the file.</returns>
      [SecurityCritical]
      public static Byte[] ReadAllBytes(KernelTransaction transaction, string path)
      {
         Byte[] bytes;

         // File.Open()
         using (FileStream fs = OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Read, FileShare.Read, EFileAttributes.Normal))
         {
            int index = 0;
            long fileLength = fs.Length;

            if (fileLength > Int32.MaxValue)
               throw new IOException(string.Format(CultureInfo.CurrentCulture, "File too large: [{0}]", path));
            
            int count = (int)fileLength;
            bytes = new Byte[count];
            while (count > 0)
            {
               int n = fs.Read(bytes, index, count);
               if (n == 0)
                  throw new IOException("Unexpected end of file found");
               index += n;
               count -= n;
            }
         }
         return bytes;
      }
      
      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // ReadAllBytes

      #region ReadAllLines

      #region .NET

      /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
      /// <param name="path">The file to open for reading. </param>
      /// <returns>A string array containing all lines of the file.</returns>
      [SecurityCritical]
      public static string[] ReadAllLines(string path)
      {
         return ReadAllLinesInternal(null, path, NativeMethods.DefaultFileEncoding).ToArray();
      }

      /// <summary>Opens a file, reads all lines of the file with the specified encoding, and then closes the file.</summary>
      /// <param name="path">The file to open for reading.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A string array containing all lines of the file.</returns>
      [SecurityCritical]
      public static string[] ReadAllLines(string path, Encoding encoding)
      {
         return ReadAllLinesInternal(null, path, encoding).ToArray();
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open for reading. </param>
      /// <returns>A string array containing all lines of the file.</returns>
      [SecurityCritical]
      public static string[] ReadAllLines(KernelTransaction transaction, string path)
      {
         return ReadAllLinesInternal(transaction, path, NativeMethods.DefaultFileEncoding).ToArray();
      }

      /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open for reading. </param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A string array containing all lines of the file.</returns>
      [SecurityCritical]
      public static string[] ReadAllLines(KernelTransaction transaction, string path, Encoding encoding)
      {
         return ReadAllLinesInternal(transaction, path, encoding).ToArray();
      }
      
      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // ReadAllLines

      #region ReadAllText

      #region .NET

      /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
      /// <param name="path">The file to open for reading.</param>
      /// <returns>A string containing all lines of the file.</returns>
      [SecurityCritical]
      public static string ReadAllText(string path)
      {
         return ReadAllTextInternal(null, path, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Opens a file, reads all lines of the file with the specified encoding, and then closes the file.</summary>
      /// <param name="path">The file to open for reading.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A string containing all lines of the file.</returns>
      [SecurityCritical]
      public static string ReadAllText(string path, Encoding encoding)
      {
         return ReadAllTextInternal(null, path, encoding);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
      /// /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open for reading.</param>
      /// <returns>A string containing all lines of the file.</returns>
      [SecurityCritical]
      public static string ReadAllText(KernelTransaction transaction, string path)
      {
         return ReadAllTextInternal(transaction, path, NativeMethods.DefaultFileEncoding);
      }

      /// <summary>Opens a text file, reads all lines of the file, and then closes the file.</summary>
      /// /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open for reading.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A string containing all lines of the file.</returns>
      [SecurityCritical]
      public static string ReadAllText(KernelTransaction transaction, string path, Encoding encoding)
      {
         return ReadAllTextInternal(transaction, path, encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // ReadAllText

      #region Replace

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // Replace

      #region SetAccessControl

      // The AlphaFS implementation replaces the .NET implementation.

      #endregion // SetAccessControl

      #region SetAttributes

      #region .NET

      /// <summary>Sets the attributes for a file or directory.</summary>
      /// <param name="path">The name of the file whose attributes are to be set.</param>
      /// <param name="fileAttributes">The file attributes to set for the file. Note that all other values override <see cref="FileAttributes.Normal"/>.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool SetAttributes(string path, FileAttributes fileAttributes)
      {
         return FileSystemInfo.SetAttributesInternal(null, path, fileAttributes);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the attributes for a file or directory.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file whose attributes are to be set.</param>
      /// <param name="fileAttributes">The file attributes to set for the file. Note that all other values override <see cref="FileAttributes.Normal"/>.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool SetAttributes(KernelTransaction transaction, string path, FileAttributes fileAttributes)
      {
         return FileSystemInfo.SetAttributesInternal(transaction, path, fileAttributes);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetAttributes

      #region SetCreationTime

      #region .NET

      /// <summary>Sets the date and time the file was created.</summary>
      /// <param name="path">The file for which to set the creation date and time information.</param>
      /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTime(string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, null, path, creationTime.ToFileTime(), null, null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time the file was created.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file for which to set the creation date and time information.</param>
      /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTime(KernelTransaction transaction, string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, transaction, path, creationTime.ToFileTime(), null, null);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetCreationTime

      #region SetCreationTimeUtc

      #region .NET

      /// <summary>Sets the date and time, in coordinated universal time (UTC), that the file was created.</summary>
      /// <param name="path">The file for which to set the creation date and time information.</param>
      /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTimeUtc(string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, null, path, creationTime.ToFileTimeUtc(), null, null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time, in coordinated universal time (UTC), that the file was created.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file for which to set the creation date and time information.</param>
      /// <param name="creationTime">A <see cref="DateTime"/> containing the value to set for the creation date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetCreationTimeUtc(KernelTransaction transaction, string path, DateTime creationTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, transaction, path, creationTime.ToFileTimeUtc(), null, null);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetCreationTimeUtc

      #region SetLastAccessTime

      #region .NET

      /// <summary>Sets the date and time, in local time, that the file was last accessed.</summary>
      /// <param name="path">The file for which to set the last access date and time information.</param>
      /// <param name="lastAccessTime">A <see cref="DateTime"/> containing the value to set for the last access date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTime(string path, DateTime lastAccessTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, null, path, null, lastAccessTime.ToFileTime(), null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transaction

      /// <summary>Sets the date and time as part of a transaction, in local time, that the file was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file for which to set the last access date and time information.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTime(KernelTransaction transaction, string path, DateTime lastAccessTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, transaction, path, null, lastAccessTime.ToFileTime(), null);
      }

      #endregion // Transaction

      #endregion // AlphaFS

      #endregion // SetLastAccessTime

      #region SetLastAccessTimeUtc

      #region .NET

      /// <summary>Sets the date and time, in coordinated universal time (UTC), that the file was last accessed.</summary>
      /// <param name="path">The file for which to set the last access date and time information.</param>
      /// <param name="lastAccessTime">A <see cref="DateTime"/> containing the value to set for the last access date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, null, path, null, lastAccessTime.ToFileTimeUtc(), null);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time, in coordinated universal time (UTC), that the file was last accessed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file for which to set the last access date and time information.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastAccessTimeUtc(KernelTransaction transaction, string path, DateTime lastAccessTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, transaction, path, null, lastAccessTime.ToFileTimeUtc(), null);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetLastAccessTimeUtc

      #region SetLastWriteTime

      #region .NET

      /// <summary>Sets the date and time, in local time, that the file was last modified.</summary>
      /// <param name="path">The file for which to set the last modification date and time information.</param>
      /// <param name="lastWriteTime">A <see cref="DateTime"/> containing the value to set for the last modification date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastWriteTime(string path, DateTime lastWriteTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, null, path, null, null, lastWriteTime.ToFileTime());
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time, in local time, that the file was last modified.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file for which to set the last modification date and time information.</param>
      /// <param name="lastWriteTime">A <see cref="DateTime"/> containing the value to set for the last modification date and time of path. This value is expressed in local time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastWriteTime(KernelTransaction transaction, string path, DateTime lastWriteTime)
      {
         FileSystemInfo.SetFileTimeInternal(true, transaction, path, null, null, lastWriteTime.ToFileTime());
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetLastWriteTime

      #region SetLastWriteTimeUtc

      #region .NET

      /// <summary>Sets the date and time, in coordinated universal time (UTC), that the file was last modified.</summary>
      /// <param name="path">The file for which to set the last modification date and time information.</param>
      /// <param name="lastWriteTime">A <see cref="DateTime"/> containing the value to set for the last modification date and time of path. This value is expressed in UTC time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, null, path, null, null, lastWriteTime.ToFileTimeUtc());
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Sets the date and time, in coordinated universal time (UTC), that the file was last modified.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file for which to set the last modification date and time information.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetLastWriteTimeUtc(KernelTransaction transaction, string path, DateTime lastWriteTime)
      {
         FileSystemInfo.SetFileTimeInternal(false, transaction, path, null, null, lastWriteTime.ToFileTimeUtc());
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // SetLastWriteTimeUtc

      #region WriteAllBytes

      /// <overloads>
      /// Creates a new file, writes the specified Byte array to the file, and then closes the file. If the target file already exists, it is overwritten.
      /// </overloads>
      /// <summary>
      /// Creates a new file, writes the specified Byte array to the file, and then closes the file. If the target file already exists, it is overwritten.
      /// </summary>
      /// <param name="path">The file to write to.</param>
      /// <param name="bytes">The bytes to write to the file.</param>
      [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes")]
      [SecurityCritical]
      public static void WriteAllBytes(string path, Byte[] bytes)
      {
         WriteAllBytes(null, path, bytes);
      }

      /// <summary>
      /// Creates a new file as part of a transaction, writes the specified Byte array to the file, and then closes the file. If the target file already exists, it is overwritten.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to write to.</param>
      /// <param name="bytes">The bytes to write to the file.</param>
      [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes")]
      [SecurityCritical]
      public static void WriteAllBytes(KernelTransaction transaction, string path, Byte[] bytes)
      {
         if (bytes == null)
            throw new ArgumentNullException("bytes");

         using (FileStream fs = OpenInternal(transaction, path, FileMode.Create, 0, FileAccess.Write, FileShare.Read, EFileAttributes.Normal))
            fs.Write(bytes, 0, bytes.Length);
      }

      #endregion

      #region WriteAllLines

      #region .NET

      /// <summary>Creates a new file, writes a collection of strings to the file, and then closes the file.</summary>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The lines to write to the file.</param>
      /// <remarks>The default behavior of the method is to write out data by using UTF-8 encoding without a byte order mark (BOM).</remarks>
      [SecurityCritical]
      public static void WriteAllLines(string path, IEnumerable<string> contents)
      {
         WriteAppendAllLinesInternal(false, true, null, path, contents, new UTF8Encoding(false, true));
      }

      /// <summary>Creates a new file by using the specified encoding, writes a collection of strings to the file, and then closes the file.</summary>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The lines to write to the file.</param>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SecurityCritical]
      public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(false, true, null, path, contents, encoding);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Creates a new file, writes a collection of strings to the file, and then closes the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The lines to write to the file.</param>
      /// <remarks>The default behavior of the method is to write out data by using UTF-8 encoding without a byte order mark (BOM).</remarks>
      [SecurityCritical]
      public static void WriteAllLines(KernelTransaction transaction, string path, IEnumerable<string> contents)
      {
         WriteAppendAllLinesInternal(false, true, transaction, path, contents, new UTF8Encoding(false, true));
      }

      /// <summary>Creates a new file by using the specified encoding, writes a collection of strings to the file, and then closes the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The lines to write to the file.</param>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SecurityCritical]
      public static void WriteAllLines(KernelTransaction transaction, string path, IEnumerable<string> contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(false, true, transaction, path, contents, encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // WriteAllLines

      #region WriteAllText

      #region .NET

      /// <summary>Creates a new file, writes the specified string to the file, and then closes the file.
      /// If the target file already exists, it is overwritten.
      /// </summary>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The string to write to the file.</param>
      /// <remarks>This method uses UTF-8 encoding without a Byte-Order Mark (BOM)</remarks>
      [SecurityCritical]
      public static void WriteAllText(string path, string contents)
      {
         WriteAppendAllLinesInternal(false, false, null, path, new[] { contents }, new UTF8Encoding(false, true));
      }

      /// <summary>Creates a new file, writes the specified string to the file using the specified encoding, and then closes the file.
      /// If the target file already exists, it is overwritten.</summary>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The string to write to the file.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      [SecurityCritical]
      public static void WriteAllText(string path, string contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(false, false, null, path, new[] { contents }, encoding);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Creates a new file as part of a transaction, write the contents to the file, and then closes the file.
      /// If the target file already exists, it is overwritten.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The string to write to the file.</param>
      /// <remarks>This method uses UTF-8 encoding without a Byte-Order Mark (BOM)</remarks>
      [SecurityCritical]
      public static void WriteAllText(KernelTransaction transaction, string path, string contents)
      {
         WriteAppendAllLinesInternal(false, false, transaction, path, new[] { contents }, new UTF8Encoding(false, true));
      }

      /// <summary>Creates a new file as part of a transaction, writes the specified string to the file using the specified encoding, and then closes the file.
      /// If the target file already exists, it is overwritten.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The string to write to the file.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      [SecurityCritical]
      public static void WriteAllText(KernelTransaction transaction, string path, string contents, Encoding encoding)
      {
         WriteAppendAllLinesInternal(false, false, transaction, path, new[] { contents }, encoding);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // WriteAllText

      #endregion .NET

      #region AlphaFS

      #region Compress

      /// <summary>Compresses a file using NTFS compression.</summary>
      /// <param name="path">A path that describes a file to compress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Compress(string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(false, null, path, true);
      }

      #region Transacted

      /// <summary>Compresses a file using NTFS compression.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a file to compress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Compress(KernelTransaction transaction, string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(false, transaction, path, true);
      }

      #endregion // Transacted

      #endregion // Compress

      #region Copy

      /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is not allowed.</summary>
      /// <param name="sourcePath">The file to copy.</param>
      /// <param name="destinationPath">The name of the destination file. This cannot be a directory or an existing file.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>The attributes of the original file are retained in the copied file.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(null, sourcePath, destinationPath, false, CopyOptions.FailIfExists, null, null, null);
      }

      /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
      /// <param name="sourcePath">The file to copy.</param>
      /// <param name="destinationPath">The name of the destination file. This cannot be a directory.</param>
      /// <param name="overwrite"><c>true</c> if the destination file should be overwritten; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>The attributes of the original file are retained in the copied file.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath, bool overwrite)
      {
         return CopyMoveInternal(null, sourcePath, destinationPath, false, overwrite ? CopyOptions.None : CopyOptions.FailIfExists, null, null, null);
      }

      /// <summary>Copies an existing file to a new file.</summary>
      /// <param name="sourcePath">The name of an existing file.</param>
      /// <param name="destinationPath">The name of the new file.</param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the file is to be copied.</param>
      /// <param name="preserveDates"><c>true</c> if original Timestamps must be preserved, otherwise <c>false</c></param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath, CopyOptions copyOptions, bool preserveDates)
      {
         return CopyMoveInternal(null, sourcePath, destinationPath, preserveDates, copyOptions, null, null, null);
      }

      /// <summary>Copies an existing file to a new file, notifying the application of its progress through a callback function.</summary>
      /// <param name="sourcePath">The name of an existing file.</param>
      /// <param name="destinationPath">The name of the new file.</param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the file is to be copied.</param>
      /// <param name="preserveDates"><c>true</c> if original Timestamps must be preserved, otherwise <c>false</c></param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(string sourcePath, string destinationPath, CopyOptions copyOptions, bool preserveDates, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(null, sourcePath, destinationPath, preserveDates, copyOptions, null, copyProgress, userProgressData);
      }

      #region Transacted

      /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is not allowed.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The file to copy.</param>
      /// <param name="destinationPath">The name of the destination file. This cannot be a directory or an existing file.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>The attributes of the original file are retained in the copied file.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(transaction, sourcePath, destinationPath, false, CopyOptions.FailIfExists, null, null, null);
      }

      /// <summary>Copies an existing file to a new file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The file to copy.</param>
      /// <param name="destinationPath">The name of the destination file. This cannot be a directory.</param>
      /// <param name="overwrite"><c>true</c> if the destination file should be overwritten; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>The attributes of the original file are retained in the copied file.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath, bool overwrite)
      {
         return CopyMoveInternal(transaction, sourcePath, destinationPath, false, overwrite ? CopyOptions.None : CopyOptions.FailIfExists, null, null, null);
      }

      /// <summary>Copies an existing file to a new file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The name of an existing file.</param>
      /// <param name="destinationPath">The name of the new file.</param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the file is to be copied.</param>
      /// <param name="preserveDates"><c>true</c> if original Timestamps must be preserved, otherwise <c>false</c></param>
      /// <returns><c>true</c> when successfully copied, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath, CopyOptions copyOptions, bool preserveDates)
      {
         return CopyMoveInternal(transaction, sourcePath, destinationPath, preserveDates, copyOptions, null, null, null);
      }

      /// <summary>Copies an existing file to a new file, notifying the application of its progress through a callback function.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The name of an existing file.</param>
      /// <param name="destinationPath">The name of the new file.</param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the file is to be copied.</param>
      /// <param name="preserveDates"><c>true</c> if original Timestamps must be preserved, otherwise <c>false</c></param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> if the file was completely copied, or <c>false</c> if the copy operation was aborted/failed.</returns>
      [SecurityCritical]
      public static bool Copy(KernelTransaction transaction, string sourcePath, string destinationPath, CopyOptions copyOptions, bool preserveDates, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(transaction, sourcePath, destinationPath, preserveDates, copyOptions, null, copyProgress, userProgressData);
      }

      #endregion // Transacted

      #endregion // Copy

      #region CreateHardlink

      /// <summary>Establishes a hard link between an existing file and a new file. This function is only supported on the NTFS file system, and only for files, not directories.</summary>
      /// <param name="sourcePath">The source file.</param>
      /// <param name="destinationPath">The destination file.</param>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hardlink")]
      [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
      [SecurityCritical]
      public static void CreateHardlink(string sourcePath, string destinationPath)
      {
         CreateHardlink(null, sourcePath, destinationPath);
      }

      #region Transacted

      /// <summary>Establishes a hard link between an existing file and a new file. This function is only supported on the NTFS file system, and only for files, not directories.</summary>
      /// <param name="sourcePath">The source file.</param>
      /// <param name="destinationPath">The destination file.</param>
      /// <param name="transaction">The transaction.</param>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hardlink")]
      [SecurityCritical]
      public static void CreateHardlink(KernelTransaction transaction, string sourcePath, string destinationPath)
      {
         if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))
            throw new ArgumentNullException("destinationPath");


         if (!(transaction == null
                            ? NativeMethods.CreateHardLink(destinationPath, sourcePath, IntPtr.Zero)
                            : NativeMethods.CreateHardLinkTransacted(destinationPath, sourcePath, IntPtr.Zero, transaction.SafeHandle)))
         {
            int lastError = Marshal.GetLastWin32Error();
            if (lastError == Win32Errors.ERROR_INVALID_FUNCTION)
               throw new NotSupportedException(Resources.HardLinksOnNonNTFSPartitionsIsNotSupported);

            NativeError.ThrowException(lastError, sourcePath, destinationPath);
         }
      }

      #endregion // Transacted

      #endregion // CreateHardlink

      #region CreateSymbolicLink

      /// <summary>Creates a symbolic link.</summary>
      /// <param name="sourcePath">The name of the target for the symbolic link to be created.</param>
      /// <param name="destinationPath">The symbolic link to be created.</param>
      /// <param name="targetType">Indicates whether the link target, <paramref name="destinationPath"/>, is a file or directory.</param>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hardlink")]
      [SecurityCritical]
      public static void CreateSymbolicLink(string sourcePath, string destinationPath, SymbolicLinkTarget targetType)
      {
         CreateSymbolicLink(null, sourcePath, destinationPath, targetType);
      }

      #region Transacted

      /// <summary>Creates a symbolic link.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The name of the target for the symbolic link to be created.</param>
      /// <param name="destinationPath">The symbolic link to be created.</param>
      /// <param name="targetType">Indicates whether the link target, <paramref name="destinationPath"/>, is a file or directory.</param>
      [SecurityCritical]
      public static void CreateSymbolicLink(KernelTransaction transaction, string sourcePath, string destinationPath, SymbolicLinkTarget targetType)
      {
         if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))


         if (!(transaction == null
                            ? NativeMethods.CreateSymbolicLink(destinationPath, sourcePath, targetType)
               : NativeMethods.CreateSymbolicLinkTransacted(destinationPath, sourcePath, targetType, transaction.SafeHandle)))
            NativeError.ThrowException(sourcePath, destinationPath);
      }

      #endregion // Transacted

      #endregion // CreateSymbolicLink

      #region Decompress

      /// <summary>Decompresses an NTFS compressed file.</summary>
      /// <param name="path">A path that describes a file to decompress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Decompress(string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(false, null, path, false);
      }

      #region Transacted

      /// <summary>Decompresses an NTFS compressed file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a file to decompress.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Decompress(KernelTransaction transaction, string path)
      {
         return NativeMethods.DeviceIo.CompressionEnableInternal(false, transaction, path, false);
      }

      #endregion // Transacted

      #endregion // Decompress

      #region Delete

      #region .NET

      /// <summary>Deletes the specified file.</summary>
      /// <param name="path">The name of the file to be deleted.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>An exception is not thrown if the specified file does not exist.</remarks>
      [SecurityCritical]
      public static bool Delete(string path)
      {
         return DeleteFileInternal(null, path, false);
      }

      /// <summary>Deletes the specified file.</summary>
      /// <param name="path">The name of the file to be deleted.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides the read only <see cref="FileAttributes"/> of the file.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>An exception is not thrown if the specified file does not exist.</remarks>
      [SecurityCritical]
      public static bool Delete(string path, bool ignoreReadOnly)
      {
         return DeleteFileInternal(null, path, ignoreReadOnly);
      }

      #endregion // .NET

      #region AlphaFS

      #region Transacted

      /// <summary>Deletes the specified file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file to be deleted.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>An exception is not thrown if the specified file does not exist.</remarks>
      [SecurityCritical]
      public static bool Delete(KernelTransaction transaction, string path)
      {
         return DeleteFileInternal(transaction, path, false);
      }

      /// <summary>Deletes the specified file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file to be deleted.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides the read only <see cref="FileAttributes"/> of the file.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>An exception is not thrown if the specified file does not exist.</remarks>
      [SecurityCritical]
      public static bool Delete(KernelTransaction transaction, string path, bool ignoreReadOnly)
      {
         return DeleteFileInternal(transaction, path, ignoreReadOnly);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // Delete

      #region EnumerateStreams

      /// <summary>Returns <see cref="BackupStreamInfo"/> instances, associated with the file.</summary>
      /// <param name="path">A path that describes a file.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the file specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(string path)
      {
         return EnumerateStreams(null, path);
      }
      
      #region Transacted

      /// <summary>Returns <see cref="BackupStreamInfo"/> instances, associated with the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a file.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the file specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(KernelTransaction transaction, string path)
      {
         using (SafeFileHandle handle = FileSystemInfo.CreateFileInternal(true, transaction, path, EFileAttributes.None, null, FileMode.Open, FileSystemRights.Read, FileShare.Read))
            foreach (BackupStreamInfo bsi in EnumerateStreams(handle))
               yield return bsi;
      }

      #endregion // Transacted

      /// <summary>Returns <see cref="BackupStreamInfo"/> instances, associated with the file.</summary>
      /// <param name="handle">A <see cref="SafeFileHandle"/> connected to the open file from which to retrieve the information.</param>
      /// <returns>An <see cref="IEnumerable{BackupStreamInfo}"/> collection of streams for the file specified by path.</returns>
      [SecurityCritical]
      public static IEnumerable<BackupStreamInfo> EnumerateStreams(SafeFileHandle handle)
      {
         NativeMethods.IsValidHandle(handle);

         Type typeWin32Stream = typeof(NativeMethods.Win32StreamId);
         uint sizeOfType = (uint)Marshal.SizeOf(typeWin32Stream);

         bool doLoop = true;

         using (new PrivilegeEnabler(Privilege.Backup))
         using (SafeGlobalMemoryBufferHandle safeBuffer = new SafeGlobalMemoryBufferHandle(NativeMethods.DefaultFileBufferSize))
         {
            uint numberOfBytesRead;
            IntPtr context;

            while (doLoop)
            {
               if (!NativeMethods.BackupRead(handle, safeBuffer, sizeOfType, out numberOfBytesRead, false, true, out context))
                  NativeError.ThrowException();

               if (numberOfBytesRead == sizeOfType)
               {
                  string name = null;
                  NativeMethods.Win32StreamId stream = NativeMethods.GetStructure<NativeMethods.Win32StreamId>(0, safeBuffer.DangerousGetHandle());

                  if (stream.StreamNameSize > 0)
                  {
                     if (!NativeMethods.BackupRead(handle, safeBuffer, stream.StreamNameSize, out numberOfBytesRead, false, true, out context))
                        NativeError.ThrowException();

                     name = Marshal.PtrToStringUni(safeBuffer.DangerousGetHandle(), (int)numberOfBytesRead / 2);
                  }

                  yield return new BackupStreamInfo(stream, name);

                  if (stream.Size > 0)
                  {
                     uint lo, hi;
                     doLoop = !NativeMethods.BackupSeek(handle, UInt32.MinValue, UInt32.MaxValue, out lo, out hi, out context);
                  }
               }
               else
                  doLoop = false;
            }

            if (!NativeMethods.BackupRead(handle, safeBuffer, 0, out numberOfBytesRead, true, false, out context))
               NativeError.ThrowException();
         }
      }

      #endregion // EnumerateStreams
      
      #region GetCompressedSize

      /// <summary>Retrieves the actual number of bytes of disk storage used to store a specified file.</summary>
      /// <remarks>
      /// If the file is located on a volume that
      /// supports compression and the file is compressed, the value obtained is the compressed size of the specified file.
      /// If the file is located on a volume that supports sparse files and the file is a sparse file, the value obtained is the sparse
      /// size of the specified file.
      /// </remarks>
      /// <param name="path"><para>The name of the file.</para>
      /// 	<para>Do not specify the name of a file on a nonseeking device, such as a pipe or a communications device, as its file size has no meaning.</para></param>
      /// <returns>The actual number of bytes of disk storage used to store the specified file.</returns>
      [SecurityCritical]
      public static long GetCompressedSize(string path)
      {
         return GetCompressedSize(null, path);
      }

      #region Transacted

      /// <summary>Retrieves the actual number of bytes of disk storage used to store a specified file as part of a transaction.
      /// If the file is located on a volume that supports compression and the file is compressed, the value obtained is the compressed size of the specified file.
      /// If the file is located on a volume that supports sparse files and the file is a sparse file, the value obtained is the sparse
      /// size of the specified file.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path"><para>The name of the file.</para>
      /// 	<para>Do not specify the name of a file on a nonseeking device, such as a pipe or a communications device, as its file size has no meaning.</para></param>
      /// <returns>The actual number of bytes of disk storage used to store the specified file.</returns>
      [SecurityCritical]
      public static long GetCompressedSize(KernelTransaction transaction, string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);

         uint fileSizeHigh;
         uint fileSizeLow = transaction == null
                               ? NativeMethods.GetCompressedFileSize(pathLp, out fileSizeHigh)
                               : NativeMethods.GetCompressedFileSizeTransacted(pathLp, out fileSizeHigh, transaction.SafeHandle);

         if (fileSizeLow == Win32Errors.ERROR_INVALID_FILE_SIZE && fileSizeHigh == 0)
         {
            int lastError = Marshal.GetLastWin32Error();
            if (lastError != Win32Errors.NO_ERROR)
               NativeError.ThrowException(lastError, pathLp);
         }

         return NativeMethods.ToLong(fileSizeHigh, fileSizeLow);
      }

      #endregion // Transacted

      #endregion // GetCompressedSize

      #region GetEncryptionStatus

      /// <summary>Retrieves the encryption status of the specified file.</summary>
      /// <param name="path">The name of the file.</param>
      /// <returns>The <see cref="FileEncryptionStatus"/> of the specified <paramref name="path"/>.</returns>
      [SecurityCritical]
      public static FileEncryptionStatus GetEncryptionStatus(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to 248 characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);

         FileEncryptionStatus status;

         if (!NativeMethods.FileEncryptionStatus(pathLp, out status))
            NativeError.ThrowException(pathLp);

         return status;
      }

      #endregion // GetEncryptionStatus

      #region GetFileInformationByHandle

      /// <summary>Retrieves file information for the specified <see cref="FileStream"/>.</summary>
      /// <param name="stream">A <see cref="FileStream"/> connected to the open file from which to retrieve the information.</param>
      /// <returns>A <see cref="ByHandleFileInformation"/> object containing the requested information.</returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
      [SecurityCritical]
      public static ByHandleFileInformation GetFileInformationByHandle(FileStream stream)
      {
         NativeMethods.IsValidStream(stream);

         return GetFileInformationByHandle(stream.SafeFileHandle);
      }

      /// <summary>Retrieves file information for the specified <see cref="SafeFileHandle"/>.</summary>
      /// <param name="safeFile">A <see cref="SafeFileHandle"/> connected to the open file from which to retrieve the information.</param>
      /// <returns>A <see cref="ByHandleFileInformation"/> object containing the requested information.</returns>
      /// <exception cref="NativeError.ThrowException()"></exception>
      [SecurityCritical]
      public static ByHandleFileInformation GetFileInformationByHandle(SafeFileHandle safeFile)
      {
         NativeMethods.IsValidHandle(safeFile);

         NativeMethods.ByHandleFileInformation info;

         if (!NativeMethods.GetFileInformationByHandle(safeFile, out info))
            NativeError.ThrowException();

         return new ByHandleFileInformation(info);
      }

      #endregion // GetFileInformationByHandle
      
      #region GetFileSystemEntryInfo

      /// <summary>Gets the <see cref="FileSystemEntryInfo"/> of the file on the path.</summary>
      /// <param name="path">The path to the file.</param>
      /// <returns>The <see cref="FileSystemEntryInfo"/> instance of the file on the path.</returns>
      [SecurityCritical]
      public static FileSystemEntryInfo GetFileSystemEntryInfo(string path)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(null, path, false, true);
      }

      /// <summary>Gets the <see cref="FileSystemEntryInfo"/> of the file on the path.</summary>
      /// <param name="path">The path to the file.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <returns>The <see cref="FileSystemEntryInfo"/> instance of the file on the path.</returns>
      [SecurityCritical]
      public static FileSystemEntryInfo GetFileSystemEntryInfo(string path, bool basicSearch)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(null, path, basicSearch, true);
      }

      #region AlphaFS

      #region Transacted

      /// <summary>Gets the <see cref="FileSystemEntryInfo"/> of the file on the path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file.</param>
      /// <returns>The <see cref="FileSystemEntryInfo"/> instance of the file on the path.</returns>
      [SecurityCritical]
      public static FileSystemEntryInfo GetFileSystemEntryInfo(KernelTransaction transaction, string path)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(transaction, path, false, true);
      }

      /// <summary>Gets the <see cref="FileSystemEntryInfo"/> of the file on the path.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file.</param>
      /// <param name="basicSearch">When <c>true</c>, does not query the short file name, improving overall enumeration speed.</param>
      /// <returns>The <see cref="FileSystemEntryInfo"/> instance of the file on the path.</returns>
      [SecurityCritical]
      public static FileSystemEntryInfo GetFileSystemEntryInfo(KernelTransaction transaction, string path, bool basicSearch)
      {
         return FileSystemInfo.GetFileSystemEntryInfoInternal(transaction, path, basicSearch, true);
      }

      #endregion // Transacted

      #endregion // AlphaFS

      #endregion // GetFileSystemEntryInfo

      #region GetFileType

      /// <summary>Retrieves the file type of the specified stream.</summary>
      /// <param name="stream">A <see cref="FileStream"/> connected to the open file from which to retrieve the information.</param>
      /// <returns>A <see cref="FileTypes"/> enum object.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
      [SecurityCritical]
      public static FileTypes GetFileType(FileStream stream)
      {
         if (!NativeMethods.IsValidStream(stream))
            return FileTypes.None;

         FileTypes ft = NativeMethods.GetFileType(stream.SafeFileHandle);

         if (ft == FileTypes.None)
         {
            int lastError = Marshal.GetLastWin32Error();
            if (lastError != Win32Errors.NO_ERROR)
               NativeError.ThrowException(lastError);
         }

         return ft;
      }

      /// <summary>Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.</summary>
      /// <param name="path">The path to the file system object which should not exceed <see cref="NativeMethods.MaxPath"/>. Both absolute and relative paths are valid.</param>
      /// <returns>A string that describes the type of file, or null in case of failure or when type is unknown.</returns>
      /// <remarks>This method calls <see cref="Shell32.GetFileType"/></remarks>
      [SecurityCritical]
      public static string GetFileType(string path)
      {
         return Shell32.GetFileType(path);
      }

      #endregion // GetFileType

      #region GetHardlinks

      /// <summary>Creates an enumeration of all the hard links to the specified <paramref name="path"/>.</summary>
      /// <param name="path">The name of the file.</param>
      /// <returns>An enumeration of all the hard links to the specified <paramref name="path"/></returns>
      /// <remarks><b>Required Windows Vista or later.</b></remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hardlinks")]
      [SecurityCritical]
      public static IEnumerable<string> GetHardlinks(string path)
      {
         return GetHardlinks(null, path);
      }

      #region Transacted

      /// <summary>Creates an enumeration of all the hard links to the specified <paramref name="path"/>.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file.</param>
      /// <returns>An enumeration of all the hard links to the specified <paramref name="path"/></returns>
      /// <remarks><b>Required Windows Vista or later.</b></remarks>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hardlinks")]
      [SecurityCritical]
      public static IEnumerable<string> GetHardlinks(KernelTransaction transaction, string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);

         return GetHardlinksInternal((ref uint length, StringBuilder linkName) => transaction == null
               ? NativeMethods.FindFirstFileName(pathLp, 0, ref length, linkName)
               : NativeMethods.FindFirstFileNameTransacted(pathLp, 0, ref length, linkName, transaction.SafeHandle));
      }

      #endregion // Transacted

      #endregion // GetHardLinks

      #region GetLinkTargetInfo

      /// <summary>Gets information about the target of a mount point or symbolic link on an NTFS file system.</summary>
      /// <param name="sourcePath">The path to the reparse point.</param>
      /// <returns>An instance of <see cref="LinkTargetInfo"/> or <see cref="SymbolicLinkTargetInfo"/> containing
      /// information about the symbolic link or mount point pointed to by <paramref name="sourcePath"/>.</returns>
      [SecurityCritical]
      public static LinkTargetInfo GetLinkTargetInfo(string sourcePath)
      {
         return GetLinkTargetInfo(null, sourcePath);
      }

      #region Transacted

      /// <summary>Gets information about the target of a mount point or symbolic link on an NTFS file system.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The path to the reparse point.</param>
      /// <returns>An instance of <see cref="LinkTargetInfo"/> or <see cref="SymbolicLinkTargetInfo"/> containing
      /// information about the symbolic link or mount point pointed to by <paramref name="sourcePath"/>.
      /// </returns>
      [SecurityCritical]
      public static LinkTargetInfo GetLinkTargetInfo(KernelTransaction transaction, string sourcePath)
      {
         using (SafeFileHandle handle = FileSystemInfo.CreateFileInternal(true, transaction, sourcePath, EFileAttributes.OpenReparsePoint | EFileAttributes.BackupSemantics, null, FileMode.Open, 0, FileShare.None))
            return NativeMethods.DeviceIo.GetLinkTargetInfo(handle);
      }

      #endregion // Transacted

      #endregion // GetLinkTargetInfo

      #region GetSize

      /// <summary>Retrieves the file size, in bytes to store a specified file.</summary>
      /// <param name="path">The path to the file.</param>
      /// <returns>The file size, in bytes.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static long GetSize(string path)
      {
         return GetSize(null, path);
      }

      /// <summary>Retrieves the file size, in bytes to store a specified file.</summary>
      /// <param name="handle">The <see cref="SafeFileHandle"/> to the file.</param>
      /// <returns>The file size, in bytes.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static long GetSize(SafeFileHandle handle)
      {
         long fileSize;

         NativeMethods.IsValidHandle(handle);

         if (!NativeMethods.GetFileSizeEx(handle, out fileSize))
            NativeError.ThrowException();

         return fileSize;
      }

      #region Transacted

      /// <summary>Retrieves the file size, in bytes to store a specified file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file.</param>
      /// <returns>The number of bytes of disk storage used to store the specified file.</returns>
      [SecurityCritical]
      public static long GetSize(KernelTransaction transaction, string path)
      {
         using (FileStream stream = OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Read, FileShare.ReadWrite, EFileAttributes.Normal))
            return GetSize(stream.SafeFileHandle);
      }

      #endregion // Transacted

      #endregion // GetSize

      #region GetStreamsSize

      /// <summary>Retrieves the actual number of bytes of disk storage used by alternate data streams (NTFS ADS).</summary>
      /// <param name="path">A path that describes a file.</param>
      /// <remarks>Use <see cref="FileInfo.Length"/> + <see cref="FileInfo.LengthStreams"/> = more accurate file size.</remarks>
      /// <returns>The size of the actual number of bytes used by file streams, other then the default stream.</returns>
      [SecurityCritical]
      public static long GetStreamsSize(string path)
      {
          return EnumerateStreams(null, path).Where(fs => fs.StreamType != BackupStreamTypes.Data).Sum(fs => fs.Size);
      }

      #region Transacted

      /// <summary>Retrieves the actual number of bytes of disk storage used by alternate data streams (NTFS ADS).</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">A path that describes a file.</param>
      /// <remarks>Use <see cref="FileInfo.Length"/> + <see cref="FileInfo.LengthStreams"/> = more accurate file size.</remarks>
      /// <returns>The size of the actual number of bytes used by file streams, other then the default stream.</returns>
      [SecurityCritical]
      public static long GetStreamsSize(KernelTransaction transaction, string path)
      {
         return EnumerateStreams(transaction, path).Where(fs => fs.StreamType != BackupStreamTypes.Data).Sum(fs => fs.Size);
      }

      #endregion // Transacted

      #endregion // GetStreamsSize

      #region Move

      /// <summary>Moves a specified file to a new location, providing the option to specify a new file name.</summary>
      /// <param name="sourcePath">The name of the file to move.</param>
      /// <param name="destinationPath">The new path for the file.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(null, sourcePath, destinationPath, false, null, MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or directory, including its children.</summary>
      /// <param name="sourcePath"><para>The name of the existing file or directory on the local computer.</para>
      /// <para>If <paramref name="options"/> specifies <see cref="MoveOptions.DelayUntilReboot"/>, the file cannot exist on
      /// a remote share because delayed operations are performed before the network is available.</para></param>
      /// <param name="destinationPath">
      /// <para>The new name of the file or directory on the local computer.</para>
      /// <para>When moving a file, <paramref name="destinationPath"/> can be on a different file system or volume.
      /// If <paramref name="destinationPath"/> is on another drive, you must set the
      /// <see cref="MoveOptions.CopyAllowed"/> flag in <paramref name="options"/>.
      /// </para>
      /// <para>When moving a directory, <paramref name="sourcePath"/> and <paramref name="destinationPath"/> must be on the same drive. </para>
      /// </param>
      /// <param name="options">The move options.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      [SecurityCritical]
      public static bool Move(string sourcePath, string destinationPath, MoveOptions options)
      {
         return CopyMoveInternal(null, sourcePath, destinationPath, false, null, options, null, null);
      }

      /// <summary>Moves a file or directory, including its children. You can provide a callback function that receives progress notifications.</summary>
      /// <param name="sourcePath"><para>The name of the existing file or directory on the local computer.</para>
      /// <para>If <paramref name="options"/> specifies <see cref="MoveOptions.DelayUntilReboot"/>, the file cannot exist on
      /// a remote share because delayed operations are performed before the network is available.</para></param>
      /// <param name="destinationPath">
      /// <para>The new name of the file or directory on the local computer.</para>
      /// <para>When moving a file, <paramref name="destinationPath"/> can be on a different file system or volume.
      /// If <paramref name="destinationPath"/> is on another drive, you must set the
      /// <see cref="MoveOptions.CopyAllowed"/> flag in <paramref name="options"/>.
      /// </para>
      /// <para>When moving a directory, <paramref name="sourcePath"/> and <paramref name="destinationPath"/> must be on the same drive. </para>
      /// </param>
      /// <param name="options">The move options.</param>
      /// <param name="copyProgress">A <see cref="CopyProgressRoutine"/> callback function that is called each time another
      /// portion of the file has been moved. The callback function can be useful if you provide a user interface that displays
      /// the progress of the operation. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">An argument to be passed to the <see cref="CopyProgressRoutine"/> callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(string sourcePath, string destinationPath, MoveOptions options, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(null, sourcePath, destinationPath, false, null, options, copyProgress, userProgressData);
      }

      #region Transacted

      /// <summary>Moves a specified file to a new location as part of a transaction, providing the option to specify a new file name.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The name of the file to move.</param>
      /// <param name="destinationPath">The new path for the file.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(KernelTransaction transaction, string sourcePath, string destinationPath)
      {
         return CopyMoveInternal(transaction, sourcePath, destinationPath, false, null, MoveOptions.CopyAllowed, null, null);
      }

      /// <summary>Moves a file or directory  as part of a transaction, including its children.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath"><para>The name of the existing file or directory on the local computer.</para>
      /// 	<para>If <paramref name="options"/> specifies <see cref="MoveOptions.DelayUntilReboot"/>, the file cannot exist on
      /// a remote share because delayed operations are performed before the network is available.</para></param>
      /// <param name="destinationPath"><para>The new name of the file or directory on the local computer.</para>
      /// 	<para>When moving a file, <paramref name="destinationPath"/> can be on a different file system or volume.
      /// If <paramref name="destinationPath"/> is on another drive, you must set the
      /// <see cref="MoveOptions.CopyAllowed"/> flag in <paramref name="options"/>.
      /// </para>
      /// 	<para>When moving a directory, <paramref name="sourcePath"/> and <paramref name="destinationPath"/> must be on the same drive. </para></param>
      /// <param name="options">The move options.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(KernelTransaction transaction, string sourcePath, string destinationPath, MoveOptions options)
      {
         return CopyMoveInternal(transaction, sourcePath, destinationPath, false, null, options, null, null);
      }

      /// <summary>Moves a file or directory as part of a transaction, including its children. You can provide a callback function that receives progress notifications.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath"><para>The name of the existing file or directory on the local computer.</para>
      /// 	<para>If <paramref name="options"/> specifies <see cref="MoveOptions.DelayUntilReboot"/>, the file cannot exist on
      /// a remote share because delayed operations are performed before the network is available.</para></param>
      /// <param name="destinationPath"><para>The new name of the file or directory on the local computer.</para>
      /// 	<para>When moving a file, <paramref name="destinationPath"/> can be on a different file system or volume.
      /// If <paramref name="destinationPath"/> is on another drive, you must set the
      /// <see cref="MoveOptions.CopyAllowed"/> flag in <paramref name="options"/>.
      /// </para>
      /// 	<para>When moving a directory, <paramref name="sourcePath"/> and <paramref name="destinationPath"/> must be on the same drive. </para></param>
      /// <param name="options">The move options.</param>
      /// <param name="copyProgress">A <see cref="CopyProgressRoutine"/> callback function that is called each time another
      /// portion of the file has been moved. The callback function can be useful if you provide a user interface that displays
      /// the progress of the operation. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">An argument to be passed to the <see cref="CopyProgressRoutine"/> callback function. This parameter can be <see langword="null"/>.</param>
      /// <returns><c>true</c> when successfully moved, <c>false</c> on failure or the operation was aborted.</returns>
      /// <remarks>This method works across disk volumes.</remarks>
      /// <remarks>Whenever possible, avoid using short file names (such as XXXXXX~1.XXX) with this method. If two files have equivalent short file names then this method may fail and raise an exception and/or result in undesirable behavior</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static bool Move(KernelTransaction transaction, string sourcePath, string destinationPath, MoveOptions options, CopyProgressRoutine copyProgress, object userProgressData)
      {
         return CopyMoveInternal(transaction, sourcePath, destinationPath, false, null, options, copyProgress, userProgressData);
      }

      #endregion // Transacted

      #endregion // Move

      #region Replace

      /// <summary>Replaces one file with another file, with the option of creating a backup copy of the original file. The replacement file assumes the name of the replaced file and its identity.</summary>
      /// <param name="sourcePath">The name of a file that replaces the file specified by <paramref name="destinationPath"/>.</param>
      /// <param name="destinationPath">The name of the file being replaced.</param>
      /// <param name="destinationBackupPath">The name of the backup file.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Replace(string sourcePath, string destinationPath, string destinationBackupPath)
      {
         return ReplaceInternal(sourcePath, destinationPath, destinationBackupPath, false);
      }

      /// <summary>Replaces one file with another file, with the option of creating a backup copy of the original file. The replacement file assumes the name of the replaced file and its identity.</summary>
      /// <param name="sourcePath">The name of a file that replaces the file specified by <paramref name="destinationPath"/>.</param>
      /// <param name="destinationPath">The name of the file being replaced.</param>
      /// <param name="destinationBackupPath">The name of the backup file.</param>
      /// <param name="ignoreMetadataErrors">set to <c>true</c> to ignore merge errors (such as attributes and access control lists (ACLs)) from the replaced file to the replacement file; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      public static bool Replace(string sourcePath, string destinationPath, string destinationBackupPath, bool ignoreMetadataErrors)
      {
         return ReplaceInternal(sourcePath, destinationPath, destinationBackupPath, ignoreMetadataErrors);
      }

      #endregion // Replace

      #region SetAccessControl

      /// <summary>Applies access control list (ACL) entries described by a <see cref="FileSecurity"/> FileSecurity object to the specified file.</summary>
      /// <param name="path">A file to add or remove access control list (ACL) entries from.</param>
      /// <param name="fileSecurity">A  <see cref="FileSecurity"/> object that describes an ACL entry to apply to the file described by the <paramref name="path"/> parameter.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public static bool SetAccessControl(string path, FileSecurity fileSecurity)
      {
         // In this case, equals null, is a good thing.
         return (FileSystemInfo.GetSetAccessControlInternal(false, true, path, fileSecurity, AccessControlSections.All) == null);
      }

      /// <summary>Applies access control list (ACL) entries described by a <see cref="DirectorySecurity"/> object to the specified directory.</summary>
      /// <param name="path">A directory to add or remove access control list (ACL) entries from.</param>
      /// <param name="fileSecurity">A <see cref="FileSecurity "/> object that describes an ACL entry to apply to the directory described by the path parameter.</param>
      /// <param name="includeSections">One or more of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to set.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = ".NET parameter type.")]
      [SecurityCritical]
      public static bool SetAccessControl(string path, FileSecurity fileSecurity, AccessControlSections includeSections)
      {
         // In this case, equals null, is a good thing.
         return (FileSystemInfo.GetSetAccessControlInternal(false, true, path, fileSecurity, includeSections) == null);
      }

      #endregion // SetAccessControl

      #region SetTimestamps

      /// <summary>Sets the time stamps at once.</summary>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestamps(string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // Directory.SetTimestamps()
         FileSystemInfo.SetFileTimeInternal(false, null, path, creationTime.ToFileTime(), lastAccessTime.ToFileTime(), lastWriteTime.ToFileTime());
      }

      #region Transacted

      /// <summary>Sets the time stamps at once.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestamps(KernelTransaction transaction, string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // Directory.SetTimestamps()
         FileSystemInfo.SetFileTimeInternal(false, transaction, path, creationTime.ToFileTime(), lastAccessTime.ToFileTime(), lastWriteTime.ToFileTime());
      }

      #endregion // Transacted

      #endregion // SetTimestamps

      #region SetTimestampsUtc

      /// <summary>Sets all the time stamps at once in UTC.</summary>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <remarks>This method is redundant, because NTFS driver converts any dates in UTC format anyways.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestampsUtc(string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // Directory.SetTimestampsUtc()
         FileSystemInfo.SetFileTimeInternal(false, null, path, creationTime.ToFileTimeUtc(), lastAccessTime.ToFileTimeUtc(), lastWriteTime.ToFileTimeUtc());
      }

      #region Transacted

      /// <summary>Sets all the time stamps at once in UTC.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path.</param>
      /// <param name="creationTime">The creation time.</param>
      /// <param name="lastAccessTime">The last access time.</param>
      /// <param name="lastWriteTime">The last write time.</param>
      /// <remarks>This method is redundant, because NTFS driver converts any dates in UTC format anyways.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      public static void SetTimestampsUtc(KernelTransaction transaction, string path, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
      {
         // Directory.SetTimestampsUtc()
         FileSystemInfo.SetFileTimeInternal(false, transaction, path, creationTime.ToFileTimeUtc(), lastAccessTime.ToFileTimeUtc(), lastWriteTime.ToFileTimeUtc());
      }

      #endregion // Transacted

      #endregion // SetTimestampsUtc

      #region TransferTimestamps

      /// <summary>Transfers the time stamps for files and directories.</summary>
      /// <param name="source">The source path.</param>
      /// <param name="destination">The destination path.</param>
      /// <remarks>
      /// This method uses BackupSemantics flag to get Timestamp changed for folders.
      /// This method does not change last access time for the source file.
      /// </remarks>
      [SecurityCritical]
      public static bool TransferTimestamps(string source, string destination)
      {
         return FileSystemInfo.TransferTimestampsInternal(false, null, source, destination);
      }

      #region Transacted

      /// <summary>Transfers the time stamps for files and directories.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="source">The source path.</param>
      /// <param name="destination">The destination path.</param>
      /// <remarks>
      /// This method uses BackupSemantics flag to get Timestamp changed for folders.
      /// This method does not change last access time for the source file.
      /// </remarks>
      [SecurityCritical]
      public static bool TransferTimestamps(KernelTransaction transaction, string source, string destination)
      {
         return FileSystemInfo.TransferTimestampsInternal(false, transaction, source, destination);
      }

      #endregion // Transacted

      #endregion // TransferTimestamps


      #region Unified Internals

      #region AppendTextInternal

      /// <summary>Unified method AppendTextInternal() to create a <see cref="StreamWriter"/> that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to an existing file, or to a new file if the specified file does not exist.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the file to append to.</param>
      /// <returns>A stream writer that appends <see cref="NativeMethods.DefaultFileEncoding"/> encoded text to the specified file or to a new file.</returns>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SecurityCritical]
      internal static StreamWriter AppendTextInternal(KernelTransaction transaction, string path, Encoding encoding)
      {
         FileStream fs = OpenInternal(transaction, path, FileMode.OpenOrCreate, 0, FileAccess.Write, FileShare.None, EFileAttributes.Normal);

         try
         {
            fs.Seek(0, SeekOrigin.End);
            return new StreamWriter(fs, encoding);
         }
         catch (IOException)
         {
            fs.Dispose();
            throw;
         }
      }

      #endregion // AppendTextInternal

      #region CopyMoveInternal

      /// <summary>Unified method CopyMoveInternal() to copy/move a Non-/Transacted file or directory including its children.
      /// You can provide a callback function that receives progress notifications.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="sourcePath">The source directory path.</param>
      /// <param name="destinationPath">The destination directory path.</param>
      /// <param name="preserveDates"><c>true</c> if original Timestamps must be preserved, otherwise <c>false</c>. This parameter is ignored for move operations.</param>
      /// <param name="copyOptions"><see cref="CopyOptions"/> that specify how the file is to be copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="moveOptions">Flags that specify how the file is to be move. This parameter can be <see langword="null"/>.</param>
      /// <param name="copyProgress">A callback function that is called each time another portion of the file has been copied. This parameter can be <see langword="null"/>.</param>
      /// <param name="userProgressData">The argument to be passed to the callback function. This parameter can be <see langword="null"/>.</param>
      /// <remarks>This Move method works across disk volumes, and it does not throw an exception if the source and destination are
      /// the same. Note that if you attempt to replace a file by moving a file of the same name into that directory, you
      /// get an IOException. You cannot use the Move method to overwrite an existing file.</remarks>
      /// <returns><c>true</c> when successfully copied or moved, <c>false</c> on failure or the operation was aborted.</returns>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool CopyMoveInternal(KernelTransaction transaction, string sourcePath, string destinationPath, bool preserveDates, CopyOptions? copyOptions, MoveOptions? moveOptions, CopyProgressRoutine copyProgress, object userProgressData)
      {
         if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))
            return false;

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string sourcePathLp = Path.PrefixLongPath(sourcePath);
         string destinationPathLp = Path.PrefixLongPath(destinationPath);
         
         // Setup callback function for progress notifications.
         NativeMethods.NativeCopyProgressRoutine routine = (copyProgress != null)
               ? (totalFileSize, totalBytesTransferred, streamSize, streamBytesTransferred, dwStreamNumber, dwCallbackReason, hSourceFile, hDestinationFile, lpData) =>
                     copyProgress(totalFileSize, totalBytesTransferred, streamSize, streamBytesTransferred, dwStreamNumber, dwCallbackReason, userProgressData)
               : (NativeMethods.NativeCopyProgressRoutine)null;

         // MoveFileWithProgress()/MoveFileTransacted
         // MoveOptions.CopyAllowed: If the file is to be moved to a different volume,
         //                          the function simulates the move by using the CopyFile and DeleteFile functions.

         bool isCopy = copyOptions != null && moveOptions == null;
         bool isMove = moveOptions != null && copyOptions == null;
         bool overwrite = isCopy
                             ? ((CopyOptions) copyOptions & CopyOptions.FailIfExists) == 0
                             : isMove && ((MoveOptions) moveOptions & MoveOptions.ReplaceExisting) != 0;
         int cancel;

         if (!(transaction == null
                  ? isMove
                       ? NativeMethods.MoveFileWithProgress(sourcePathLp, destinationPathLp, routine, IntPtr.Zero, (MoveOptions)moveOptions)
                       : NativeMethods.CopyFileEx(sourcePath, destinationPath, routine, IntPtr.Zero, out cancel, copyOptions ?? CopyOptions.FailIfExists)
                  : isMove
                       ? NativeMethods.MoveFileTransacted(sourcePathLp, destinationPathLp, routine, IntPtr.Zero, (MoveOptions)moveOptions, transaction.SafeHandle)
                       : NativeMethods.CopyFileTransacted(sourcePath, destinationPath, routine, IntPtr.Zero, out cancel, copyOptions ?? CopyOptions.FailIfExists, transaction.SafeHandle)))
         {
            int lastError = Marshal.GetLastWin32Error();
            bool raiseException;

            switch ((uint)lastError)
            {
               // The existing file is left intact.
               case Win32Errors.ERROR_REQUEST_ABORTED:
                  return false;

               case Win32Errors.ERROR_FILE_EXISTS:
                  raiseException = !overwrite || !DeleteFileInternal(transaction, sourcePath, true);
                  break;

               // This function fails with ERROR_ACCESS_DENIED if the destination file already exists
               // and has the FILE_ATTRIBUTE_HIDDEN or FILE_ATTRIBUTE_READONLY attribute set.
               case Win32Errors.ERROR_ACCESS_DENIED:
                  FileSystemInfo.SetAttributesInternal(transaction, destinationPath, FileAttributes.Normal);
                  raiseException = !overwrite || !CopyMoveInternal(transaction, sourcePath, destinationPath, preserveDates, copyOptions, null, copyProgress, userProgressData);
                  break;

               default:
                  raiseException = true;
                  break;
            }

            if (raiseException)
               NativeError.ThrowException(lastError, sourcePath, destinationPath);
         }

         // Apply original Timestamps if requested and action is Copy().
         if (preserveDates && isCopy)
            {
               FileSystemEntryInfo originalAttributes = FileSystemInfo.GetFileSystemEntryInfoInternal(transaction, sourcePath, false, true);
               FileSystemInfo.SetFileTimeInternal(false, transaction, destinationPath, originalAttributes.Win32FindData.CreationTime.ToLong(), originalAttributes.Win32FindData.LastAccessTime.ToLong(), originalAttributes.Win32FindData.LastWriteTime.ToLong());
            }

         return true;
      }

      #endregion // CopyMoveInternal

      #region CreateFileInternal

      /// <summary>Unified method CreateFileInternal() to create or overwrite a file in the specified path, specifying a buffer size and advanced options:
      /// <see cref="EFileAttributes"/>, <see cref="FileSecurity"/>, <see cref="FileMode"/>, <see cref="FileAccess"/>, <see cref="FileShare"/>.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file.</param>
      /// <param name="bufferSize">The number of bytes buffered for reads and writes to the file.</param>
      /// <param name="attributes">The <see cref="EFileAttributes"/> additional advanced options to create a file.</param>
      /// <param name="fileSecurity">A <see cref="FileSecurity"/> instance that determines the access control and audit security for the file.</param>
      /// <param name="mode">The <see cref="FileMode"/> option gives you more precise control over how you want to create a file.</param>
      /// <param name="access">The <see cref="FileAccess"/> allow you additionaly specify to default redwrite capability - just write, bypassing any cache.</param>
      /// <param name="share">The <see cref="FileShare"/> option controls how you would like to share created file with other requesters.</param>
      /// <returns>A <see cref="FileStream"/> that provides read/write access to the file specified in path.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      internal static FileStream CreateFileInternal(KernelTransaction transaction, string path, int bufferSize, EFileAttributes attributes, FileSecurity fileSecurity, FileMode mode, FileAccess access, FileShare share)
      {
         // Caller is responsible for disposing.
         SafeFileHandle handle = FileSystemInfo.CreateFileInternal(true, transaction, path, attributes, fileSecurity, mode, (FileSystemRights)access, share);
         return new FileStream(handle, access, bufferSize, (attributes & EFileAttributes.Overlapped) != 0);
      }

      #endregion // CreateFileInternal

      #region CreateTextInternal

      /// <summary>Unified method CreateTextInternal() to create or open a file for writing <see cref="Encoding"/> encoded text.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to be opened for writing.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A <see cref="StreamWriter"/> that writes to the specified file using <see cref="NativeMethods.DefaultFileBufferSize"/> encoding.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      internal static StreamWriter CreateTextInternal(KernelTransaction transaction, string path, Encoding encoding)
      {
         return new StreamWriter(CreateFileInternal(transaction, path, NativeMethods.DefaultFileBufferSize, EFileAttributes.Normal, null, FileMode.Create, FileAccess.ReadWrite, FileShare.None), encoding);
      }

      #endregion // CreateTextInternal

      #region DeleteFileInternal

      /// <summary>Unified method DeleteFileInternal() to delete a Non-/Transacted file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The name of the file to be deleted.</param>
      /// <param name="ignoreReadOnly">If set to <c>true</c> overrides the read only <see cref="FileAttributes"/> of the file.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      /// <remarks>An exception is not thrown if the specified file does not exist.</remarks>
      /// <exception cref="NativeError.ThrowException()"/>
      [SecurityCritical]
      internal static bool DeleteFileInternal(KernelTransaction transaction, string path, bool ignoreReadOnly)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN confirms LongPath usage.
         string pathLp = Path.PrefixLongPath(path);

         bool deleteOk = true;

         if (!(transaction == null
                            ? NativeMethods.DeleteFile(pathLp)
               : NativeMethods.DeleteFileTransacted(pathLp, transaction.SafeHandle)))
         {
            int lastError = Marshal.GetLastWin32Error();
            switch ((uint)lastError)
            {
               // If the file is a read-only file, the function fails with ERROR_ACCESS_DENIED.
               case Win32Errors.ERROR_ACCESS_DENIED:
                  deleteOk = ignoreReadOnly &&
                             (FileSystemInfo.SetAttributesInternal(transaction, pathLp, FileAttributes.Normal) &&
                              DeleteFileInternal(transaction, pathLp, true));
                  break;

               // .NET: If the file to be deleted does not exist, no exception is thrown.
               case Win32Errors.ERROR_FILE_NOT_FOUND:
                  break;

               default:
                  NativeError.ThrowException(lastError, pathLp);
                  break;
            }
         }

         return deleteOk;
      }

      #endregion // DeleteFileInternal

      #region GetHardlinksInternal

      private delegate SafeFindFileHandle FindFirstFileNameFunction(ref uint length, StringBuilder linkName);
      private static IEnumerable<string> GetHardlinksInternal(FindFirstFileNameFunction findFirstFileName)
      {
         // Default buffer length, will be extended if needed.
         uint length = 256;
         StringBuilder builder = new StringBuilder((int) length);
         bool tryAgain = false;

         do
         {
            using (SafeFindFileHandle handle = findFirstFileName(ref length, builder))
            {
               if (!NativeMethods.IsValidHandle(handle, false))
               {
                  int lastError = Marshal.GetLastWin32Error();

                  // We only want to try again once!
                  if (lastError == Win32Errors.ERROR_MORE_DATA && tryAgain == false)
                  {
                     builder.EnsureCapacity((int)length);
                     tryAgain = true;
                     continue;
                  }

                  NativeError.ThrowException(lastError);
               }

               yield return builder.ToString();

               // We should not try the outer loop again if it succeeded once.
               tryAgain = false;
               bool innerTryAgain = false;
               bool hasMore;

               do
               {
                  builder.Length = 0;
                  hasMore = NativeMethods.FindNextFileName(handle, ref length, builder);
                  if (!hasMore)
                  {
                     int lastError = Marshal.GetLastWin32Error();
                     if (lastError == Win32Errors.ERROR_MORE_DATA && !innerTryAgain)
                     {
                        // Buffer needs more space.
                        builder.EnsureCapacity((int)length);
                        innerTryAgain = true;
                        continue;
                     }

                     if (lastError == Win32Errors.ERROR_HANDLE_EOF)
                        // We've reached the end of the enumeration.
                        yield break;

                     // An unexpected error occurred
                     NativeError.ThrowException(lastError);
                  }
                  else
                     innerTryAgain = false;

                  yield return builder.ToString();
               }
               while (hasMore || innerTryAgain);
            }
         }
         while (tryAgain);
      }

      #endregion // GetHardlinksInternal

      #region OpenInternal

      /// <summary>Unified method OpenInternal() to opens a <see cref="FileStream"/> on the specified path, having the specified mode with read, write,
      /// or read/write access, the specified sharing option and additional options specified.
      /// </summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open.</param>
      /// <param name="mode">A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist, and determines whether the contents of existing files are retained or overwritten.</param>
      /// <param name="rights">A <see cref="FileSystemRights"/> value that specifies whether a file is created if one does not exist,
      /// and determines whether the contents of existing files are retained or overwritten along with additional options.</param>
      /// <param name="access">A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
      /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
      /// <param name="attributes">Advanced <see cref="EFileAttributes"/> options for this file.</param>
      /// <returns>A <see cref="FileStream"/> on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      internal static FileStream OpenInternal(KernelTransaction transaction, string path, FileMode mode, FileSystemRights rights, FileAccess access, FileShare share, EFileAttributes attributes)
      {
         SafeFileHandle handle = (rights != 0)
                                    ? FileSystemInfo.CreateFileInternal(true, transaction, path, attributes, null, mode, rights, share)
                                    : FileSystemInfo.CreateFileInternal(true, transaction, path, attributes, null, mode, (FileSystemRights)access, share);

         return (rights != 0)
                   ? new FileStream(handle, FileAccess.Write)
                   : new FileStream(handle, access);
      }

      #endregion // OpenInternal

      #region ReadAllLinesInternal

      /// <summary>Unified method ReadAllLinesInternal() to open a file, read all lines of the file, and then close the file.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open for reading. </param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>An IEnumerable string containing all lines of the file.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SecurityCritical]
      internal static IEnumerable<string> ReadAllLinesInternal(KernelTransaction transaction, string path, Encoding encoding)
      {
         using (StreamReader sr = new StreamReader(OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Read, FileShare.Read, EFileAttributes.Normal), encoding))
         {
            string line;
            while ((line = sr.ReadLine()) != null)
               yield return line;
         }
      }

      #endregion // ReadAllLinesInternal

      #region ReadAllTextInternal

      /// <summary>Unified method ReadAllTextInternal() to open a file, read all lines of the file, and then close the file.</summary>
      /// /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to open for reading.</param>
      /// <param name="encoding">The <see cref="Encoding"/> applied to the contents of the file.</param>
      /// <returns>A string containing all lines of the file.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SecurityCritical]
      internal static string ReadAllTextInternal(KernelTransaction transaction, string path, Encoding encoding)
      {
         using (StreamReader sr = new StreamReader(OpenInternal(transaction, path, FileMode.Open, 0, FileAccess.Read, FileShare.Read, EFileAttributes.Normal), encoding))
            return sr.ReadToEnd();
      }

      #endregion // ReadAllTextInternal

      #region ReplaceInternal

      /// <summary>Unified method ReplaceInternal() to replace one file with another file, with the option of creating a backup copy of the original file. The replacement file assumes the name of the replaced file and its identity.</summary>
      /// <param name="sourcePath">The name of a file that replaces the file specified by <paramref name="destinationPath"/>.</param>
      /// <param name="destinationPath">The name of the file being replaced.</param>
      /// <param name="destinationBackupPath">The name of the backup file.</param>
      /// <param name="ignoreMetadataErrors">set to <c>true</c> to ignore merge errors (such as attributes and access control lists (ACLs)) from the replaced file to the replacement file; otherwise, <c>false</c>.</param>
      /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
      [SecurityCritical]
      internal static bool ReplaceInternal(string sourcePath, string destinationPath, string destinationBackupPath, bool ignoreMetadataErrors)
      {
         if (string.IsNullOrEmpty(sourcePath))
            throw new ArgumentNullException("sourcePath");

         if (string.IsNullOrEmpty(destinationPath))
            throw new ArgumentNullException("destinationPath");

         if (!NativeMethods.ReplaceFile(destinationPath, sourcePath, destinationBackupPath,
                                        FileSystemRights.ListDirectory |
                                        (ignoreMetadataErrors ? FileSystemRights.CreateFiles & FileSystemRights.Synchronize : 0), IntPtr.Zero, IntPtr.Zero))

            NativeError.ThrowException();

         return true;
      }

      #endregion // ReplaceInternal

      #region WriteAppendAllLinesInternal

      /// <summary>Unified method WriteAppendAllLinesInternal() to create/append a new file by using the specified encoding, writes a collection of strings to the file, and then closes the file.</summary>
      /// <param name="isAppend"><c>true</c> for file Append, <c>false</c> for file Write.</param>
      /// <param name="addNewLine"><c>true</c> to a line terminator, <c>false</c> to ommit the line terminator.</param>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The file to write to.</param>
      /// <param name="contents">The lines to write to the file.</param>
      /// <param name="encoding">The character <see cref="Encoding"/> to use.</param>
      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      [SecurityCritical]
      internal static void WriteAppendAllLinesInternal(bool isAppend, bool addNewLine, KernelTransaction transaction, string path, IEnumerable<string> contents, Encoding encoding)
      {
         if (contents == null)
            throw new ArgumentNullException("contents");

         using (FileStream stream = OpenInternal(transaction, path, (isAppend ? FileMode.OpenOrCreate : FileMode.Create), FileSystemRights.AppendData, FileAccess.Write, FileShare.ReadWrite, EFileAttributes.Normal))
         {
            if (isAppend)
               stream.Seek(0, SeekOrigin.End);

            using (StreamWriter writer = new StreamWriter(stream, encoding))
            {
               if (addNewLine)
                  foreach (string line in contents)
                     writer.WriteLine(line);

               else
                  foreach (string line in contents)
                     writer.Write(line);
            }
         }
      }

      #endregion // WriteAppendAllLinesInternal

      #endregion // Unified Internals

      #endregion // AlphaFS
   }
}