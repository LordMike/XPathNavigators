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

namespace Alphaleonis.Win32.Network
{
   /// <summary>The type of the shared resource.</summary>
   [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
   [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
   [Flags]
   public enum ShareTypes : uint
   {
      /// <summary>Disk tree.</summary>
      DiskTree = 0x0,

      /// <summary>Print queue.</summary>
      PrintQueue = 0x1,

      /// <summary>Communication device.</summary>
      Device = 0x2,

      /// <summary>Special share reserved for interprocess communication (IPC$).</summary>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ipc")]
      Ipc = 0x3,

      /// <summary>A temporary share.</summary>
      Temporary = 0x40000000,

      /// <summary>Special.</summary>
      Special = 0x80000000,
   }
}