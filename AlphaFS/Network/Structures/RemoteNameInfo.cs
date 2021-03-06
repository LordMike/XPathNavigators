﻿/* Copyright (c) 2008-2012 Peter Palotas, Alexandr Normuradov
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Alphaleonis.Win32.Network
{
   internal static partial class NativeMethods
   {
      ///<summary>REMOTE_NAME_INFO - The RemoteNameInfo structure contains path and name information for a network resource.</summary>
      [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct RemoteNameInfo
      {
         /// <summary>Pointer to the null-terminated UNC name string that identifies a network resource.</summary>
         [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] [MarshalAs(UnmanagedType.LPWStr)] public string UniversalName;

         /// <summary>Pointer to a null-terminated string that is the name of a network connection.</summary>
         [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] [MarshalAs(UnmanagedType.LPWStr)] public string ConnectionName;

         /// <summary>Pointer to a null-terminated name string.</summary>
         [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")] [MarshalAs(UnmanagedType.LPWStr)] public string RemainingPath;
      }
   }
}