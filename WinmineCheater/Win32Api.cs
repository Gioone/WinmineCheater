using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinmineCheater
{
    internal class Win32Api
    {
        #region WinAPI

        /// <summary>
        /// Simulate mouse click.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="data"></param>
        /// <param name="extraInfo"></param>
        [DllImport("user32.dll")]
        internal static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        /// <summary>
        /// Set cursor position.
        /// </summary>
        /// <param name="x">X point.</param>
        /// <param name="y">Y point.</param>
        /// <returns><see langword="true" /> if successful, <see langword="false" /> if failed.</returns>
        [DllImport("user32.dll")]
        internal static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// Get Windoe RECT.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <param name="lpRect">RECT</param>
        /// <returns><see langword="true" /> if successful, <see langword="false" /> if failed.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        /// <summary>
        /// Active window.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns><see langword="true"/> if active successful. <see langword="false" /> if active failed.</returns>
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// 關閉一個內核對象。其中包括文件、文件映射、進程、線程、安全和同步對象等。
        /// </summary>
        /// <param name="hObject">您需要关闭的句柄</param>
        [DllImport("kernel32.dll")]
        internal static extern void CloseHandle(IntPtr hObject);

        /// <summary>
        /// 從指定內存中讀取字節集資料
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">基地址</param>
        /// <param name="lpBuffer">缓冲区</param>
        /// <param name="nSize">读取大小</param>
        /// <param name="lpNumberOfBytesRead">读取的字节数</param>
        /// <returns>是否读取成功</returns>
        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// 打開一個已存在的進程對象，並返回進程的句柄
        /// </summary>
        /// <param name="dwDesiredAccess">需要用什么权限打开进程</param>
        /// <param name="bInheritHandle">是否继承句柄</param>
        /// <param name="dwProcessId">进程 PID</param>
        /// <returns>Process handle</returns>
        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        /// <summary>
        /// Get process ID.
        /// </summary>
        /// <param name="hwnd">Window Handle</param>
        /// <param name="ID"></param>
        /// <returns>Process ID. <see langword="0" /> if failed. <see langword="Greater 0" /> successful.</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        /// <summary>
        /// Get window handle.
        /// </summary>
        /// <param name="lpClassName">Class name</param>
        /// <param name="lpWindowName">Window name</param>
        /// <returns>Window handle./returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string? lpClassName, string lpWindowName);

        /// <summary>
        /// 写入进程内存
        /// </summary>
        /// <param name="hProcess">进程句柄</param>
        /// <param name="lpBaseAddress">基址</param>
        /// <param name="lpBuffer">缓冲区</param>
        /// <param name="nSize">写入大小</param>
        /// <param name="lpNumberOfBytesWritten">实际写入大小</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, int[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);

        #endregion WinAPI
    }
}
