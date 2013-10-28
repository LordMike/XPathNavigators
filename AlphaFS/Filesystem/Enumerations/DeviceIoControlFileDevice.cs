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

namespace Alphaleonis.Win32.Filesystem
{
   internal enum DeviceIoControlFileDevice : uint
   {
      //Beep = 1,
      //CDRom = 2,
      //CDRomFileSytem = 3,
      //Controller = 4,
      //Datalink = 5,
      //Dfs = 6,
      Disk = 7,
      //DiskFileSystem = 8,
      FileSystem = 9,
      //InPortPort = 10,
      //Keyboard = 11,
      //Mailslot = 12,
      //MidiIn = 13,
      //MidiOut = 14,
      //Mouse = 15,
      //MultiUncProvider = 16,
      //NamedPipe = 17,
      //Network = 18,
      //NetworkBrowser = 19,
      //NetworkFileSystem = 20,
      //Null = 21,
      //ParellelPort = 22,
      //PhysicalNetcard = 23,
      //Printer = 24,
      //Scanner = 25,
      //SerialMousePort = 26,
      //SerialPort = 27,
      //Screen = 28,
      //Sound = 29,
      //Streams = 30,
      //Tape = 31,
      //TapeFileSystem = 32,
      //Transport = 33,
      //Unknown = 34,
      //Video = 35,
      //VirtualDisk = 36,
      //WaveIn = 37,
      //WaveOut = 38,
      //Port8042 = 39,
      //NetworkRedirector = 40,
      //Battery = 41,
      //BusExtender = 42,
      //Modem = 43,
      //Vdm = 44,
      //MassStorage = 45,
      //Smb = 46,
      //Ks = 47,
      //Changer = 48,
      //Smartcard = 49,
      //Acpi = 50,
      //Dvd = 51,
      //FullscreenVideo = 52,
      //DfsFileSystem = 53,
      //DfsVolume = 54,
      //Serenum = 55,
      //Termsrv = 56,
      //Ksec = 57
   }
}