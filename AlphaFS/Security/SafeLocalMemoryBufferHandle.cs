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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Alphaleonis.Win32.Security
{
   /// <summary>IntPtr wrapper which can be used as result of Marshal.AllocHGlobal operation. Calls Marshal.FreeHGlobal when disposed or finalized.</summary>
   internal sealed class SafeLocalMemoryBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
   {
      #region Constructor

      /// <summary>Creates new instance with zero IntPtr.</summary>
      [SecurityPermission(SecurityAction.LinkDemand)]
      public SafeLocalMemoryBufferHandle() : base(true)
      {
      }

      #endregion // Constructor

      #region Methods

      #region CopyFrom

      /// <summary>Copies data from a one-dimensional, managed 8-bit unsigned integer array to the unmanaged memory pointer referenced by this instance.</summary>
      /// <param name="source">The one-dimensional array to copy from. </param>
      /// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
      /// <param name="length">The number of array elements to copy.</param>
      [SecurityPermission(SecurityAction.LinkDemand)]
      public void CopyFrom(byte[] source, int startIndex, int length)
      {
         Marshal.Copy(source, startIndex, handle, length);
      }

      #endregion // CopyFrom

      #region CopyTo

      [SecurityPermission(SecurityAction.LinkDemand)]
      public void CopyTo(byte[] destination, int destinationOffset, int length)
      {
         if (destination == null)
            throw new ArgumentNullException("destination");

         if (destinationOffset < 0)
            throw new ArgumentOutOfRangeException("destinationOffset", Resources.SafeGlobalMemoryBufferHandle_CopyTo_Destination_offset_must_not_be_negative);

         if (length < 0)
            throw new ArgumentOutOfRangeException("length", Resources.SafeGlobalMemoryBufferHandle_CopyTo_Length_must_not_be_negative_);

         if (destinationOffset + length > destination.Length)
            throw new ArgumentException("Destination buffer not large enough for the requested operation.");

         if (length > Capacity)
            throw new ArgumentOutOfRangeException("length", Resources.SafeGlobalMemoryBufferHandle_CopyTo_Source_offset_and_length_outside_the_bounds_of_the_array);

         Marshal.Copy(handle, destination, destinationOffset, length);
      }

      #endregion // CopyTo

      #region ToByteArray

      public byte[] ToByteArray(int startIndex, int length)
      {
         if (IsInvalid)
            return null;

         byte[] arr = new byte[length];
         Marshal.Copy(handle, arr, startIndex, length);
         return arr;
      }

      #endregion // ToByteArray

      #region ReleaseHandle

      /// <summary>Called when object is disposed or finalized.</summary>
      protected override bool ReleaseHandle()
      {
         return NativeMethods.LocalFree(handle) == IntPtr.Zero;
      }
      
      #endregion // ReleaseHandle

      #endregion // Methods

      #region Properties

      #region Capacity

      public int Capacity
      {
         get { return mCapacity; }
      }

      #endregion // Capacity

      #endregion // Properties

      #region Fields

      private readonly int mCapacity;

      #endregion // Fields
   }
}