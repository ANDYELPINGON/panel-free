﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ruhan
{

    public class Qᴜᴀɴᴛᴜᴍ 
    {
        [DllImport("kernel32.dll")]
        private static extern void GetSystemInfo(out Qᴜᴀɴᴛᴜᴍ .SYSTEM_INFO lpSystemInfo);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32")]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, UIntPtr lpAddress, IntPtr dwSize, Qᴜᴀɴᴛᴜᴍ .MemoryProtection flNewProtect, out Qᴜᴀɴᴛᴜᴍ .MemoryProtection lpflOldProtect);
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out Qᴜᴀɴᴛᴜᴍ .MEMORY_BASIC_INFORMATION64 lpBuffer, UIntPtr dwLength);
        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out Qᴜᴀɴᴛᴜᴍ .MEMORY_BASIC_INFORMATION32 lpBuffer, UIntPtr dwLength);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] IntPtr lpBuffer, UIntPtr nSize, out ulong lpNumberOfBytesRead);
        public string LoadCode(string name, string file)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            bool flag = file != "";
            if (flag)
            {
                uint privateProfileString = Qᴜᴀɴᴛᴜᴍ .GetPrivateProfileString("codes", name, "", stringBuilder, (uint)stringBuilder.Capacity, file);
            }
            else
            {
                stringBuilder.Append(name);
            }
            return stringBuilder.ToString();
        }

        public byte[] readMemory(string code, long length, string file = "")
        {
            byte[] array = new byte[length];
            UIntPtr code2 = this.GetCode(code, file, 8);
            bool flag = !Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, code2, array, (UIntPtr)(checked((ulong)length)), IntPtr.Zero);
            byte[] result;
            if (flag)
            {
                result = null;
            }
            else
            {
                result = array;
            }
            return result;
        }

        private Dictionary<long, int> repeat = new Dictionary<long, int>();
        // Save the repeat dictionary to a file, if you want 
        public void SaveRepeatDictionary(string filePath)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                foreach (var entry in repeat)
                {
                    file.WriteLine($"{entry.Key:X},{entry.Value:X}");
                }
            }
        }
        // Load the repeat dictionary from a file, if you want 
        public void LoadRepeatDictionary(string filePath)
        {
            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(',');
                    long address = Convert.ToInt64(parts[0], 16);
                    int firstByte = Convert.ToInt32(parts[1], 16); repeat[address] = firstByte;
                }
            }
        }









        public Task<IEnumerable<long>> AoBScanMultithreadedWithTasks(long start, long end, string searchPattern, bool readable, bool writable, bool executable, string file = "")
        {


            return Task.Run(async () =>
            {
                List<long> foundAddresses = new List<long>();

                // Load AoB pattern from file if provided
                string[] patternParts;
                if (!string.IsNullOrEmpty(file))
                {
                    string text = LoadCode(searchPattern, file);
                    patternParts = text.Split(' ');
                }
                else
                {
                    patternParts = searchPattern.Split(' ');
                }

                // Create AoB pattern and mask arrays
                byte[] pattern = new byte[patternParts.Length];
                byte[] mask = new byte[patternParts.Length];
                bool[] ignore00 = new bool[patternParts.Length];

                for (int i = 0; i < patternParts.Length; i++)
                {
                    string part = patternParts[i];

                    if (part == "??")
                    {
                        pattern[i] = 0x00; // Wildcard for any byte
                        mask[i] = 0;       // Ignore this byte
                        ignore00[i] = false;
                    }
                    else if (part == "!!")
                    {
                        pattern[i] = 0x00; // Special wildcard for non-00
                        mask[i] = 0;       // Mask for non-00
                        ignore00[i] = true;
                    }
                    else
                    {
                        mask[i] = 0xFF;    // Match exact byte
                        pattern[i] = Convert.ToByte(part, 16);
                        ignore00[i] = false;
                    }
                }

                // Collect memory regions to scan
                List<MEMORY_BASIC_INFORMATION> memoryRegions = new List<MEMORY_BASIC_INFORMATION>();
                MEMORY_BASIC_INFORMATION memInfo;
                UIntPtr address = new UIntPtr((ulong)start);

                Qᴜᴀɴᴛᴜᴍ .SYSTEM_INFO system_INFO = default(Qᴜᴀɴᴛᴜᴍ .SYSTEM_INFO);
                Qᴜᴀɴᴛᴜᴍ .GetSystemInfo(out system_INFO);
                UIntPtr minimumApplicationAddress = system_INFO.minimumApplicationAddress;
                UIntPtr maximumApplicationAddress = system_INFO.maximumApplicationAddress;

                if (start < (long)minimumApplicationAddress.ToUInt64())
                    start = (long)minimumApplicationAddress.ToUInt64();
                if (end > (long)maximumApplicationAddress.ToUInt64())
                    end = (long)maximumApplicationAddress.ToUInt64();

                // Loop over the memory regions
                while (VirtualQueryEx(pHandle, address, out memInfo) != UIntPtr.Zero && address.ToUInt64() < (ulong)end)
                {
                    if (memInfo.State == 0x1000 && (memInfo.Protect & 0x100) == 0 && (memInfo.Protect & 1) == 0)
                    {
                        bool isReadable = (memInfo.Protect & 0x2) > 0 && readable;
                        bool isWritable = (memInfo.Protect & 0x4) > 0 && writable;
                        bool isExecutable = (memInfo.Protect & 0x20) > 0 && executable;

                        if (isReadable || isWritable || isExecutable)
                        {
                            memoryRegions.Add(memInfo);
                        }
                    }
                    address = new UIntPtr((ulong)(address.ToUInt64() + (ulong)memInfo.RegionSize));
                }

                // Create a list of tasks for multithreaded scanning
                List<Task> tasks = new List<Task>();

                foreach (var region in memoryRegions)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        byte[] buffer = new byte[(int)region.RegionSize];
                        if (ReadProcessMemory(pHandle, region.BaseAddress, buffer, (UIntPtr)region.RegionSize, IntPtr.Zero))
                        {
                            // Search for the pattern in this memory region
                            for (long i = 0; i < region.RegionSize - pattern.Length; i++)
                            {
                                bool found = true;

                                for (int j = 0; j < pattern.Length; j++)
                                {
                                    if (mask[j] != 0 && buffer[i + j] != (pattern[j] & mask[j]))
                                    {
                                        found = false;
                                        break;
                                    }

                                    if (ignore00[j] && buffer[i + j] == 0x00)
                                    {
                                        found = false;
                                        break;
                                    }
                                }

                                if (found)
                                {
                                    long foundAddress = (long)region.BaseAddress + i;
                                    int firstByte = buffer[i];

                                    // Check for repeat
                                    lock (repeat)
                                    {
                                        if (!repeat.ContainsKey(foundAddress) || repeat[foundAddress] != firstByte)
                                        {
                                            repeat[foundAddress] = firstByte;
                                            lock (foundAddresses)
                                            {
                                                foundAddresses.Add(foundAddress);
                                            }

                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }));
                }

                // Wait for all tasks to complete
                await Task.WhenAll(tasks);

                return foundAddresses.AsEnumerable();
            });





        }










        public string MSize()
        {
            bool is64Bit = this.Is64Bit;
            string result;
            if (is64Bit)
            {
                result = "x16";
            }
            else
            {
                result = "x8";
            }
            return result;
        }
        public void CloseProcess()
        {
            IntPtr intPtr = this.pHandle;
            bool flag = false;
            if (!flag)
            {
                Qᴜᴀɴᴛᴜᴍ .CloseHandle(this.pHandle);
                this.theProc = null;
            }
        }
        private bool _is64Bit;
        public bool Is64Bit
        {
            get
            {
                return this._is64Bit;
            }
            private set
            {
                this._is64Bit = value;
            }
        }
        private unsafe long[] CompareScan(MemoryRegionResult item, byte[] aobPattern, byte[] mask)
        {
            bool flag = mask.Length != aobPattern.Length;
            if (flag)
            {
                throw new ArgumentException("aobPattern.Length != mask.Length");
            }
            IntPtr intPtr = Marshal.AllocHGlobal((int)item.RegionSize);
            ulong num;
            Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, item.CurrentBaseAddress, intPtr, (UIntPtr)((ulong)item.RegionSize), out num);
            int num2 = 0 - aobPattern.Length;
            List<long> list = new List<long>();
            do
            {
                num2 = this.FindPattern((byte*)intPtr.ToPointer(), (int)num, aobPattern, mask, num2 + aobPattern.Length);
                bool flag2 = num2 >= 0;
                if (flag2)
                {
                    list.Add((long)((ulong)item.CurrentBaseAddress + (ulong)((long)num2)));
                }
            }
            while (num2 != -1);
            Marshal.FreeHGlobal(intPtr);
            return list.ToArray();
        }
        private unsafe int FindPattern(byte* body, int bodyLength, byte[] pattern, byte[] masks, int start = 0)
        {
            int num = -1;
            bool flag = bodyLength <= 0 || pattern.Length == 0 || start > bodyLength - pattern.Length || pattern.Length > bodyLength;
            int result;
            if (flag)
            {
                result = num;
            }
            else
            {
                for (int i = start; i <= bodyLength - pattern.Length; i++)
                {
                    bool flag2 = (body[i] & masks[0]) == (pattern[0] & masks[0]);
                    if (flag2)
                    {
                        bool flag3 = true;
                        for (int j = 1; j <= pattern.Length - 1; j++)
                        {
                            bool flag4 = (body[i + j] & masks[j]) == (pattern[j] & masks[j]);
                            if (!flag4)
                            {
                                flag3 = false;
                                break;
                            }
                        }
                        bool flag5 = !flag3;
                        if (!flag5)
                        {
                            num = i;
                            break;
                        }
                    }
                }
                result = num;
            }
            return result;
        }
        public UIntPtr VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out Qᴜᴀɴᴛᴜᴍ .MEMORY_BASIC_INFORMATION lpBuffer)
        {
            bool flag = this.Is64Bit || IntPtr.Size == 8;
            UIntPtr result;
            if (flag)
            {
                Qᴜᴀɴᴛᴜᴍ .MEMORY_BASIC_INFORMATION64 memory_BASIC_INFORMATION = default(Qᴜᴀɴᴛᴜᴍ .MEMORY_BASIC_INFORMATION64);
                UIntPtr uintPtr = Qᴜᴀɴᴛᴜᴍ .Native_VirtualQueryEx(hProcess, lpAddress, out memory_BASIC_INFORMATION, new UIntPtr((uint)Marshal.SizeOf(memory_BASIC_INFORMATION)));
                lpBuffer.BaseAddress = memory_BASIC_INFORMATION.BaseAddress;
                lpBuffer.AllocationBase = memory_BASIC_INFORMATION.AllocationBase;
                lpBuffer.AllocationProtect = memory_BASIC_INFORMATION.AllocationProtect;
                lpBuffer.RegionSize = (long)memory_BASIC_INFORMATION.RegionSize;
                lpBuffer.State = memory_BASIC_INFORMATION.State;
                lpBuffer.Protect = memory_BASIC_INFORMATION.Protect;
                lpBuffer.Type = memory_BASIC_INFORMATION.Type;
                result = uintPtr;
            }
            else
            {
                Qᴜᴀɴᴛᴜᴍ .MEMORY_BASIC_INFORMATION32 memory_BASIC_INFORMATION2 = default(Qᴜᴀɴᴛᴜᴍ .MEMORY_BASIC_INFORMATION32);
                UIntPtr uintPtr = Qᴜᴀɴᴛᴜᴍ .Native_VirtualQueryEx(hProcess, lpAddress, out memory_BASIC_INFORMATION2, new UIntPtr((uint)Marshal.SizeOf(memory_BASIC_INFORMATION2)));
                lpBuffer.BaseAddress = memory_BASIC_INFORMATION2.BaseAddress;
                lpBuffer.AllocationBase = memory_BASIC_INFORMATION2.AllocationBase;
                lpBuffer.AllocationProtect = memory_BASIC_INFORMATION2.AllocationProtect;
                lpBuffer.RegionSize = (long)((ulong)memory_BASIC_INFORMATION2.RegionSize);
                lpBuffer.State = memory_BASIC_INFORMATION2.State;
                lpBuffer.Protect = memory_BASIC_INFORMATION2.Protect;
                lpBuffer.Type = memory_BASIC_INFORMATION2.Type;
                result = uintPtr;
            }
            return result;
        }
        public static void notify(string message)
        {
            Process.Start(new ProcessStartInfo("cmd.exe", $"/c start cmd /C \"color b && title Error && echo {message} && timeout /t 5\"")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            });
            Environment.Exit(0);
        }
        public UIntPtr Get64BitCode(string name, string path = "", int size = 16)
        {
            bool flag = path != "";
            string text;
            if (flag)
            {
                text = this.LoadCode(name, path);
            }
            else
            {
                text = name;
            }
            bool flag2 = text == "";
            UIntPtr result;
            if (flag2)
            {
                result = UIntPtr.Zero;
            }
            else
            {
                bool flag3 = text.Contains(" ");
                if (flag3)
                {
                    text.Replace(" ", string.Empty);
                }
                string text2 = text;
                bool flag4 = text.Contains("+");
                if (flag4)
                {
                    text2 = text.Substring(text.IndexOf('+') + 1);
                }
                byte[] array = new byte[size];
                bool flag5 = !text.Contains("+") && !text.Contains(",");
                if (flag5)
                {
                    result = new UIntPtr(Convert.ToUInt64(text, 16));
                }
                else
                {
                    bool flag6 = text2.Contains(',');
                    if (flag6)
                    {
                        List<long> list = new List<long>();
                        string[] array2 = text2.Split(new char[]
                        {
                            ','
                        });
                        foreach (string text3 in array2)
                        {
                            string text4 = text3;
                            bool flag7 = text3.Contains("0x");
                            if (flag7)
                            {
                                text4 = text3.Replace("0x", "");
                            }
                            bool flag8 = !text3.Contains("-");
                            long num;
                            if (flag8)
                            {
                                num = long.Parse(text4, NumberStyles.AllowHexSpecifier);
                            }
                            else
                            {
                                text4 = text4.Replace("-", "");
                                num = long.Parse(text4, NumberStyles.AllowHexSpecifier);
                                num *= -1L;
                            }
                            list.Add(num);
                        }
                        long[] array4 = list.ToArray();
                        bool flag9 = text.Contains("base") || text.Contains("main");
                        if (flag9)
                        {
                            Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, (UIntPtr)((ulong)((long)this.mainModule.BaseAddress + array4[0])), array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                        }
                        else
                        {
                            bool flag10 = !text.Contains("base") && !text.Contains("main") && text.Contains("+");
                            if (flag10)
                            {
                                string[] array5 = text.Split(new char[]
                                {
                                    '+'
                                });
                                IntPtr value = IntPtr.Zero;
                                bool flag11 = !array5[0].ToLower().Contains(".dll") && !array5[0].ToLower().Contains(".exe") && !array5[0].ToLower().Contains(".bin");
                                if (flag11)
                                {
                                    value = (IntPtr)long.Parse(array5[0], NumberStyles.HexNumber);
                                }
                                else
                                {
                                    try
                                    {
                                        value = this.modules[array5[0]];
                                    }
                                    catch
                                    {
                                        Debug.WriteLine("Module " + array5[0] + " was not found in module list!");
                                        Debug.WriteLine("Modules: " + string.Join<KeyValuePair<string, IntPtr>>(",", this.modules));
                                    }
                                }
                                Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, (UIntPtr)((ulong)((long)value + array4[0])), array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                            }
                            else
                            {
                                Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, (UIntPtr)((ulong)array4[0]), array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                            }
                        }
                        long num2 = BitConverter.ToInt64(array, 0);
                        UIntPtr uintPtr = (UIntPtr)0UL;
                        for (int j = 1; j < array4.Length; j++)
                        {
                            uintPtr = new UIntPtr(Convert.ToUInt64(num2 + array4[j]));
                            Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, uintPtr, array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                            num2 = BitConverter.ToInt64(array, 0);
                        }
                        result = uintPtr;
                    }
                    else
                    {
                        long num3 = Convert.ToInt64(text2, 16);
                        IntPtr value2 = IntPtr.Zero;
                        bool flag12 = text.Contains("base") || text.Contains("main");
                        if (flag12)
                        {
                            value2 = this.mainModule.BaseAddress;
                        }
                        else
                        {
                            bool flag13 = !text.Contains("base") && !text.Contains("main") && text.Contains("+");
                            if (flag13)
                            {
                                string[] array6 = text.Split(new char[]
                                {
                                    '+'
                                });
                                bool flag14 = !array6[0].ToLower().Contains(".dll") && !array6[0].ToLower().Contains(".exe") && !array6[0].ToLower().Contains(".bin");
                                if (flag14)
                                {
                                    string text5 = array6[0];
                                    bool flag15 = text5.Contains("0x");
                                    if (flag15)
                                    {
                                        text5 = text5.Replace("0x", "");
                                    }
                                    value2 = (IntPtr)long.Parse(text5, NumberStyles.HexNumber);
                                }
                                else
                                {
                                    try
                                    {
                                        value2 = this.modules[array6[0]];
                                    }
                                    catch
                                    {
                                        Debug.WriteLine("Module " + array6[0] + " was not found in module list!");
                                        Debug.WriteLine("Modules: " + string.Join<KeyValuePair<string, IntPtr>>(",", this.modules));
                                    }
                                }
                            }
                            else
                            {
                                value2 = this.modules[text.Split(new char[]
                                {
                                    '+'
                                })[0]];
                            }
                        }
                        result = (UIntPtr)((ulong)((long)value2 + num3));
                    }
                }
            }
            return result;
        }
        public UIntPtr GetCode(string name, string path = "", int size = 8)
        {
            bool is64Bit = this.Is64Bit;
            UIntPtr result;
            if (is64Bit)
            {
                bool flag = size == 8;
                if (flag)
                {
                    size = 16;
                }
                result = this.Get64BitCode(name, path, size);
            }
            else
            {
                bool flag2 = path != "";
                string text;
                if (flag2)
                {
                    text = this.LoadCode(name, path);
                }
                else
                {
                    text = name;
                }
                bool flag3 = text == "";
                if (flag3)
                {
                    result = UIntPtr.Zero;
                }
                else
                {
                    bool flag4 = text.Contains(" ");
                    if (flag4)
                    {
                        text.Replace(" ", string.Empty);
                    }
                    bool flag5 = !text.Contains("+") && !text.Contains(",");
                    if (flag5)
                    {
                        result = new UIntPtr(Convert.ToUInt32(text, 16));
                    }
                    else
                    {
                        string text2 = text;
                        bool flag6 = text.Contains("+");
                        if (flag6)
                        {
                            text2 = text.Substring(text.IndexOf('+') + 1);
                        }
                        byte[] array = new byte[size];
                        bool flag7 = text2.Contains(',');
                        if (flag7)
                        {
                            List<int> list = new List<int>();
                            string[] array2 = text2.Split(new char[]
                            {
                                ','
                            });
                            foreach (string text3 in array2)
                            {
                                string text4 = text3;
                                bool flag8 = text3.Contains("0x");
                                if (flag8)
                                {
                                    text4 = text3.Replace("0x", "");
                                }
                                bool flag9 = !text3.Contains("-");
                                int num;
                                if (flag9)
                                {
                                    num = int.Parse(text4, NumberStyles.AllowHexSpecifier);
                                }
                                else
                                {
                                    text4 = text4.Replace("-", "");
                                    num = int.Parse(text4, NumberStyles.AllowHexSpecifier);
                                    num *= -1;
                                }
                                list.Add(num);
                            }
                            int[] array4 = list.ToArray();
                            bool flag10 = text.Contains("base") || text.Contains("main");
                            if (flag10)
                            {
                                Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, (UIntPtr)((ulong)((long)((int)this.mainModule.BaseAddress + array4[0]))), array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                            }
                            else
                            {
                                bool flag11 = !text.Contains("base") && !text.Contains("main") && text.Contains("+");
                                if (flag11)
                                {
                                    string[] array5 = text.Split(new char[]
                                    {
                                        '+'
                                    });
                                    IntPtr value = IntPtr.Zero;
                                    bool flag12 = !array5[0].ToLower().Contains(".dll") && !array5[0].ToLower().Contains(".exe") && !array5[0].ToLower().Contains(".bin");
                                    if (flag12)
                                    {
                                        string text5 = array5[0];
                                        bool flag13 = text5.Contains("0x");
                                        if (flag13)
                                        {
                                            text5 = text5.Replace("0x", "");
                                        }
                                        value = (IntPtr)int.Parse(text5, NumberStyles.HexNumber);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            value = this.modules[array5[0]];
                                        }
                                        catch
                                        {
                                            Debug.WriteLine("Module " + array5[0] + " was not found in module list!");
                                            Debug.WriteLine("Modules: " + string.Join<KeyValuePair<string, IntPtr>>(",", this.modules));
                                        }
                                    }
                                    Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, (UIntPtr)((ulong)((long)((int)value + array4[0]))), array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                                }
                                else
                                {
                                    Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, (UIntPtr)((ulong)((long)array4[0])), array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                                }
                            }
                            uint num2 = BitConverter.ToUInt32(array, 0);
                            UIntPtr uintPtr = (UIntPtr)0UL;
                            for (int j = 1; j < array4.Length; j++)
                            {
                                uintPtr = new UIntPtr(Convert.ToUInt32((long)((ulong)num2 + (ulong)((long)array4[j]))));
                                Qᴜᴀɴᴛᴜᴍ .ReadProcessMemory(this.pHandle, uintPtr, array, (UIntPtr)((ulong)((long)size)), IntPtr.Zero);
                                num2 = BitConverter.ToUInt32(array, 0);
                            }
                            result = uintPtr;
                        }
                        else
                        {
                            int num3 = Convert.ToInt32(text2, 16);
                            IntPtr value2 = IntPtr.Zero;
                            bool flag14 = text.ToLower().Contains("base") || text.ToLower().Contains("main");
                            if (flag14)
                            {
                                value2 = this.mainModule.BaseAddress;
                            }
                            else
                            {
                                bool flag15 = !text.ToLower().Contains("base") && !text.ToLower().Contains("main") && text.Contains("+");
                                if (flag15)
                                {
                                    string[] array6 = text.Split(new char[]
                                    {
                                        '+'
                                    });
                                    bool flag16 = !array6[0].ToLower().Contains(".dll") && !array6[0].ToLower().Contains(".exe") && !array6[0].ToLower().Contains(".bin");
                                    if (flag16)
                                    {
                                        string text6 = array6[0];
                                        bool flag17 = text6.Contains("0x");
                                        if (flag17)
                                        {
                                            text6 = text6.Replace("0x", "");
                                        }
                                        value2 = (IntPtr)int.Parse(text6, NumberStyles.HexNumber);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            value2 = this.modules[array6[0]];
                                        }
                                        catch
                                        {
                                            Debug.WriteLine("Module " + array6[0] + " was not found in module list!");
                                            Debug.WriteLine("Modules: " + string.Join<KeyValuePair<string, IntPtr>>(",", this.modules));
                                        }
                                    }
                                }
                                else
                                {
                                    value2 = this.modules[text.Split(new char[]
                                    {
                                        '+'
                                    })[0]];
                                }
                            }
                            result = (UIntPtr)((ulong)((long)((int)value2 + num3)));
                        }
                    }
                }
            }
            return result;
        }
        public bool WriteMemory(string code, string type, string write, string file = "", Encoding stringEncoding = null)
        {
            byte[] array = new byte[4];
            int num = 4;
            UIntPtr code2 = this.GetCode(code, file, 8);
            bool flag = type.ToLower() == "float";
            if (flag)
            {
                array = BitConverter.GetBytes(Convert.ToSingle(write));
                num = 4;
            }
            else
            {
                bool flag2 = type.ToLower() == "int";
                if (flag2)
                {
                    array = BitConverter.GetBytes(Convert.ToInt32(write));
                    num = 4;
                }
                else
                {
                    bool flag3 = type.ToLower() == "byte";
                    if (flag3)
                    {
                        array = new byte[]
                        {
                            Convert.ToByte(write, 16)
                        };
                        num = 1;
                    }
                    else
                    {
                        bool flag4 = type.ToLower() == "2bytes";
                        if (flag4)
                        {
                            array = new byte[]
                            {
                                (byte)(Convert.ToInt32(write) % 256),
                                (byte)(Convert.ToInt32(write) / 256)
                            };
                            num = 2;
                        }
                        else
                        {
                            bool flag5 = type.ToLower() == "bytes";
                            if (flag5)
                            {
                                bool flag6 = write.Contains(",") || write.Contains(" ");
                                if (flag6)
                                {
                                    bool flag7 = write.Contains(",");
                                    string[] array2;
                                    if (flag7)
                                    {
                                        array2 = write.Split(new char[]
                                        {
                                            ','
                                        });
                                    }
                                    else
                                    {
                                        array2 = write.Split(new char[]
                                        {
                                            ' '
                                        });
                                    }
                                    int num2 = array2.Count<string>();
                                    array = new byte[num2];
                                    for (int i = 0; i < num2; i++)
                                    {
                                        array[i] = Convert.ToByte(array2[i], 16);
                                    }
                                    num = array2.Count<string>();
                                }
                                else
                                {
                                    array = new byte[]
                                    {
                                        Convert.ToByte(write, 16)
                                    };
                                    num = 1;
                                }
                            }
                            else
                            {
                                bool flag8 = type.ToLower() == "double";
                                if (flag8)
                                {
                                    array = BitConverter.GetBytes(Convert.ToDouble(write));
                                    num = 8;
                                }
                                else
                                {
                                    bool flag9 = type.ToLower() == "long";
                                    if (flag9)
                                    {
                                        array = BitConverter.GetBytes(Convert.ToInt64(write));
                                        num = 8;
                                    }
                                    else
                                    {
                                        bool flag10 = type.ToLower() == "string";
                                        if (flag10)
                                        {
                                            bool flag11 = stringEncoding == null;
                                            if (flag11)
                                            {
                                                array = Encoding.UTF8.GetBytes(write);
                                            }
                                            else
                                            {
                                                array = stringEncoding.GetBytes(write);
                                            }
                                            num = array.Length;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Qᴜᴀɴᴛᴜᴍ .WriteProcessMemory(this.pHandle, code2, array, (UIntPtr)((ulong)((long)num)), IntPtr.Zero);
        }
        public bool IsAdmin()
        {
            bool result;
            using (WindowsIdentity current = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
                result = windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return result;
        }
        public bool OpenProcess(int pid)
        {
            bool flag = !this.IsAdmin();
            if (flag)
            {
                Debug.WriteLine("WARNING: You are NOT running this program as admin! Visit https://discord.gg/emmWnPRrYX");
                notify("WARNING: You are NOT running this program as admin! For More Help Visit https://discord.gg/emmWnPRrYX");
            }
            bool flag2 = pid <= 0;
            bool result;
            if (flag2)
            {
                Debug.WriteLine("ERROR: OpenProcess given proc ID 0.");
                result = false;
            }
            else
            {
                bool flag3 = this.theProc != null && this.theProc.Id == pid;
                if (flag3)
                {
                    result = true;
                }
                else
                {
                    try
                    {
                        this.theProc = Process.GetProcessById(pid);
                        bool flag4 = this.theProc != null && !this.theProc.Responding;
                        if (flag4)
                        {
                            Debug.WriteLine("ERROR: OpenProcess: Process is not responding or null.");
                            result = false;
                        }
                        else
                        {
                            this.pHandle = Qᴜᴀɴᴛᴜᴍ .OpenProcess(2035711U, true, pid);
                            Process.EnterDebugMode();
                            bool flag5 = this.pHandle == IntPtr.Zero;
                            if (flag5)
                            {
                                Debug.WriteLine("ERROR: OpenProcess has failed opening a handle to the target process (GetLastWin32ErrorCode: " + Marshal.GetLastWin32Error().ToString() + ")");
                                Process.LeaveDebugMode();
                                this.theProc = null;
                                result = false;
                            }
                            else
                            {
                                this.mainModule = this.theProc.MainModule;
                                this.GetModules();
                                bool flag6;
                                this.Is64Bit = (Environment.Is64BitOperatingSystem && Qᴜᴀɴᴛᴜᴍ .IsWow64Process(this.pHandle, out flag6) && !flag6);
                                string str = "Program is operating at Administrative level. Process #";
                                Process process = this.theProc;
                                Debug.WriteLine(str + ((process != null) ? process.ToString() : null) + " is open and modules are stored.");
                                result = true;
                            }
                        }
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        public void GetModules()
        {
            bool flag = this.theProc == null;
            if (!flag)
            {
                this.modules.Clear();
                foreach (object obj in this.theProc.Modules)
                {
                    ProcessModule processModule = (ProcessModule)obj;
                    bool flag2 = !string.IsNullOrEmpty(processModule.ModuleName) && !this.modules.ContainsKey(processModule.ModuleName);
                    if (flag2)
                    {
                        this.modules.Add(processModule.ModuleName, processModule.BaseAddress);
                    }
                }
            }
        }
        public Task<IEnumerable<long>> AoBScan2(string search, bool writable = false, bool executable = false, string file = "")
        {
            return this.AoBScan(0L, long.MaxValue, search, writable, executable, file);
        }
        public Task<IEnumerable<long>> AoBScan(long start, long end, string search, bool writable, bool executable, string file = "")
        {
            return this.AoBScanMultithreadedWithTasks(start, end, search, true, writable, executable, file);
        }
        public bool ChangeProtection(string code, Qᴜᴀɴᴛᴜᴍ .MemoryProtection newProtection, out Qᴜᴀɴᴛᴜᴍ .MemoryProtection oldProtection, string file = "")
        {
            UIntPtr code2 = this.GetCode(code, file, 8);
            bool flag = code2 == UIntPtr.Zero || this.pHandle == IntPtr.Zero;
            bool result;
            if (flag)
            {
                oldProtection = (Qᴜᴀɴᴛᴜᴍ .MemoryProtection)0U;
                result = false;
            }
            else
            {
                result = Qᴜᴀɴᴛᴜᴍ .VirtualProtectEx(this.pHandle, code2, (IntPtr)(this.Is64Bit ? 8 : 4), newProtection, out oldProtection);
            }
            return result;
        }
        private Dictionary<string, IntPtr> modules = new Dictionary<string, IntPtr>();
        private ProcessModule mainModule;
        public Process theProc = null;
        private uint MEM_PRIVATE = 131072U;
        private uint MEM_IMAGE = 16777216U;
        public IntPtr pHandle;
        [Flags]
        public enum ThreadAccess
        {
            TERMINATE = 1,
            SUSPEND_RESUME = 2,
            GET_CONTEXT = 8,
            SET_CONTEXT = 16,
            SET_INFORMATION = 32,
            QUERY_INFORMATION = 64,
            SET_THREAD_TOKEN = 128,
            IMPERSONATE = 256,
            DIRECT_IMPERSONATION = 512
        }
        public struct MEMORY_BASIC_INFORMATION32
        {
            public UIntPtr BaseAddress;

            public UIntPtr AllocationBase;

            public uint AllocationProtect;

            public uint RegionSize;

            public uint State;

            public uint Protect;

            public uint Type;
        }
        public struct MEMORY_BASIC_INFORMATION64
        {
            public UIntPtr BaseAddress;

            public UIntPtr AllocationBase;

            public uint AllocationProtect;

            public uint __alignment1;

            public ulong RegionSize;

            public uint State;

            public uint Protect;

            public uint Type;

            public uint __alignment2;
        }
        [Flags]
        public enum MemoryProtection : uint
        {
            Execute = 16U,
            ExecuteRead = 32U,
            ExecuteReadWrite = 64U,
            ExecuteWriteCopy = 128U,
            NoAccess = 1U,
            ReadOnly = 2U,
            ReadWrite = 4U,
            WriteCopy = 8U,
            GuardModifierflag = 256U,
            NoCacheModifierflag = 512U,
            WriteCombineModifierflag = 1024U
        }
        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;

            private ushort reserved;

            public uint pageSize;

            public UIntPtr minimumApplicationAddress;

            public UIntPtr maximumApplicationAddress;

            public IntPtr activeProcessorMask;

            public uint numberOfProcessors;

            public uint processorType;

            public uint allocationGranularity;

            public ushort processorLevel;

            public ushort processorRevision;
        }
        public struct MEMORY_BASIC_INFORMATION
        {
            public UIntPtr BaseAddress;

            public UIntPtr AllocationBase;

            public uint AllocationProtect;

            public long RegionSize;

            public uint State;

            public uint Protect;

            public uint Type;
        }
        private int x;

    }


    internal struct MemoryRegionResult
    {
        public UIntPtr CurrentBaseAddress { get; set; }

        public long RegionSize { get; set; }

        public UIntPtr RegionBase { get; set; }
    }

}
