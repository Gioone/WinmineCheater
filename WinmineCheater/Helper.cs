using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinmineCheater
{
    internal static class Helper
    {
        internal static bool IsMinesDistributionWindowOpened { get; set; }

        /// <summary>
        /// Get MineSweeper PID.
        /// </summary>
        /// <returns>MineSweeper process PID. 0 found failed.</returns>
        internal static int GetMineSweeperPid()
        {
            Process[] processes = Process.GetProcessesByName("MineSweeper");
            foreach (Process process in processes)
            {
                return process.Id;
            }
            return 0;
        }

        /// <summary>
        /// Get Winmine PID.
        /// </summary>
        /// <returns>Winmine process PID. 0 found failed.</returns>
        internal static int GetWinminePid()
        {
            Process[] processes = Process.GetProcessesByName("Winmine");
            foreach (Process process in processes)
            {
                return process.Id;
            }
            return 0;
        }

        /// <summary>
        /// Reading a byte from Winmine.exe memory.
        /// </summary>
        /// <param name="baseAddress">Address</param>
        /// <param name="iPid">Process ID</param>
        /// <returns>Reading result.</returns>
        public static byte ReadMemoryValueByte(int baseAddress, int iPid)
        {
            try
            {
                byte[] buffer = new byte[1];
                //獲取緩沖區地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打開一個已存在的進程對象  0x1F0FFF 最高權限
                IntPtr hProcess = Win32Api.OpenProcess(0x1F0FFF, false, iPid);
                //將制定內存中的值讀入緩沖區
                Win32Api.ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, 1, IntPtr.Zero);
                Win32Api.CloseHandle(hProcess);
                // Reading a byte from unmanaged memory.
                return Marshal.ReadByte(byteAddress);
            }
            catch
            {
                return 0;
            }
        }
        public static int ReadMemoryValue(int baseAddress, int iPid)
        {
            try
            {
                byte[] buffer = new byte[4];
                //獲取緩沖區地址
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                //打開一個已存在的進程對象  0x1F0FFF 最高權限
                IntPtr hProcess = Win32Api.OpenProcess(0x1F0FFF, false, iPid);
                //將制定內存中的值讀入緩沖區
                Win32Api.ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, 4, IntPtr.Zero);
                //關閉操作
                Win32Api.CloseHandle(hProcess);
                //從非托管內存中讀取一個 32 位帶符號整數。
                return Marshal.ReadInt32(byteAddress);
            }
            catch
            {
                return 0;
            }
        }
    }
}
