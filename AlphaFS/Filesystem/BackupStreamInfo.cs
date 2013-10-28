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
using Microsoft.Win32.SafeHandles;

namespace Alphaleonis.Win32.Filesystem
{
   /// <summary>The <see cref="BackupStreamInfo"/> structure contains stream header data.</summary>
   /// <seealso cref="BackupFileStream"/>
   public sealed class BackupStreamInfo : MarshalByRefObject
   {
      /// <summary>Initializes a new instance of the <see cref="BackupStreamInfo"/> class.</summary>
      /// <param name="streamId">The <see cref="NativeMethods.Win32StreamId"/> stream ID.</param>
      internal BackupStreamInfo(NativeMethods.Win32StreamId streamId)
         :this(streamId, null)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="BackupStreamInfo"/> class.</summary>
      /// <param name="streamId">The <see cref="NativeMethods.Win32StreamId"/> stream ID.</param>
      /// <param name="name">The name.</param>
      internal BackupStreamInfo(NativeMethods.Win32StreamId streamId, string name)
      {
         _mName = name;
         _mSize = (long)streamId.Size;
         _mAttributes = streamId.StreamAttributes;
         _mStreamType = (BackupStreamTypes)streamId.StreamId;
         Source = null;
      }

      /// <summary>Initializes a new instance of the <see cref="BackupStreamInfo"/> class.</summary>
      /// <param name="streamId">The <see cref="NativeMethods.Win32StreamId"/> stream ID.</param>
      /// <param name="name">The name.</param>
      /// <param name="source">The source file where this instance points to.</param>
      internal BackupStreamInfo(NativeMethods.Win32StreamId streamId, string name, string source)
         :this(streamId, name)
      {
         Source = source;
      }

      /// <summary>Initializes a new instance of the <see cref="BackupStreamInfo"/> class.</summary>
      /// <param name="streamId">The <see cref="NativeMethods.Win32StreamId"/> stream ID.</param>
      /// <param name="name">The name.</param>
      /// <param name="handle">The source <see cref="SafeFileHandle"/> handle where this instance points to.</param>
      internal BackupStreamInfo(NativeMethods.Win32StreamId streamId, string name, SafeFileHandle handle)
         : this(streamId, name)
      {
         Source = Path.GetFinalPathNameByHandleInternal(handle, FinalPathFormats.FileNameNormalized);
      }

      /// <summary>Gets the size of the data in the substream, in bytes.</summary>
      /// <value>The size of the data in the substream, in bytes.</value>
      public long Size { get { return _mSize; } }

      /// <summary>Gets a string that specifies the name of the alternative data stream.</summary>
      /// <value>A string that specifies the name of the alternative data stream.</value>
      public string Name { get { return _mName; } }

      /// <summary>Gets the type of the data in the stream.</summary>
      /// <value>The type of the data in the stream.</value>
      public BackupStreamTypes StreamType { get { return _mStreamType; } }

      /// <summary>Gets the attributes of the data to facilitate cross-operating system transfer.</summary>
      /// <value>Attributes of the data to facilitate cross-operating system transfer.</value>
      public BackupStreamAttributes Attributes { get { return _mAttributes; } }

      /// <summary>The source file where this instance points to.</summary>
      public string Source { get; set; }

      private readonly long _mSize;
      private readonly string _mName;
      private readonly BackupStreamTypes _mStreamType;
      private readonly BackupStreamAttributes _mAttributes;
   }
}