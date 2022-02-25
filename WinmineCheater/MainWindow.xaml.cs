using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace WinmineCheater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly object _lockPid = new();
        private static int _pid;
        internal static int Pid
        {
            set
            {
                lock (_lockPid)
                {
                    _pid = value;
                }
            }
            get => _pid;
        }
        private readonly object _lockIsStopTrdCheckGameIsRunning = new();
        private bool _isStopTrdCheckGameIsRunning;
        public bool IsStopTrdCheckGameIsRunning
        {
            get => _isStopTrdCheckGameIsRunning;
            set
            {
                lock (_lockIsStopTrdCheckGameIsRunning)
                {
                    _isStopTrdCheckGameIsRunning = value;
                }
            }
        }

        private readonly object _lockIsEnableControls = new();
        private bool _isEnabledControls;
        public bool IsEnabledControls
        {
            get => _isEnabledControls;
            set
            {
                lock (_lockIsEnableControls)
                {
                    _isEnabledControls = value;
                }
            }
        }
        public MainWindow()
        {
            string str = new string("");
            InitializeComponent();
            var trdCheckGameIsRunning = new Thread(() =>
            {
                while (!IsStopTrdCheckGameIsRunning)
                {
                    Pid = Helper.GetWinminePid();
                    if (Pid > 0)
                    {
                        if (!IsEnabledControls)
                        {
                            Application.Current.Dispatcher.Invoke(EnableControls);
                            IsEnabledControls = true;
                        }
                    }
                    else
                    {
                        if (IsEnabledControls)
                        {
                            Application.Current.Dispatcher.Invoke(DisableControls);
                            IsEnabledControls = false;
                        }
                    }
                    Thread.Sleep(1000);
                }

            })
            {
                IsBackground = true
            };
            trdCheckGameIsRunning.Start();

        }

        /// <summary>
        /// Disable all controls.
        /// </summary>
        private void DisableControls()
        {
            BtnShowMines.IsEnabled = false;
            BtnStartWinmine.IsEnabled = false;
        }

        /// <summary>
        /// Enable all controls.
        /// </summary>
        private void EnableControls()
        {
            BtnShowMines.IsEnabled = true;
            BtnStartWinmine.IsEnabled = true;
        }

        private void BtnShowMines_Click(object sender, RoutedEventArgs e)
        {
            if (Helper.IsMinesDistributionWindowOpened) return;
            IntPtr hProcess = Win32Api.OpenProcess(0x1F0FFF/*Highest permission*/, false, Pid);
            byte rows = Helper.ReadMemoryValueByte(Address.ROW_ADDRESS, Pid);
            byte columns = Helper.ReadMemoryValueByte(Address.COLUMN_ADDRESS, Pid);
            Win32Api.CloseHandle(hProcess);
            new MinesDistributionWindow(rows, columns).Show();
            Helper.IsMinesDistributionWindowOpened = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsStopTrdCheckGameIsRunning = true;
            Application.Current.Shutdown();
        }

        private void BtnStartWinmine_Click(object sender, RoutedEventArgs e)
        {
            StartWinmine();
        }

        private static void StartWinmine()
        {
            // Is enable "?" signature.
            bool isEnableQuestionMark = Helper.ReadMemoryValueByte(Address.IS_ENABLE_QUESTION_MARK, Pid) == 1;
            // Process handle.
            // IntPtr handleProgress = Win32Api.OpenProcess(0x1F0FFF, false, Pid);

            // Get window title.
            string title = Process.GetProcessById(Pid).MainWindowTitle;

            IntPtr hwnd = Win32Api.FindWindow(null, title);

            // Active Winmine window.
            Win32Api.SetForegroundWindow(hwnd);
            RECT rect = new();

            // Get window RECT.
            Win32Api.GetWindowRect(hwnd, ref rect);
            // Get window width.
            /*int windowWidth = rect.Right - rect.Left;
            // Get window height.
            int windowHeight = rect.Bottom - rect.Top;*/
            /*// Get game area width.
            int gameHeight = windowHeight - 101 - 8;
            // Get game area height.
            int gameWidth = windowWidth - 13 - 8;*/

            // Read rows.
            byte rows = Helper.ReadMemoryValueByte(Address.ROW_ADDRESS, Pid);

            // Read columns.
            byte columns = Helper.ReadMemoryValueByte(Address.COLUMN_ADDRESS, Pid);

            byte[,] array = new byte[rows, columns];

            // First row address.
            int rowAddress = Address.MINE_START_ADDRESS;
            for (int i = 0; i < rows; i++)
            {
                // First mine address.
                int mineAddress = rowAddress;
                for (int j = 0; j < columns; j++)
                {
                    byte value = Helper.ReadMemoryValueByte(mineAddress, Pid);
                    array[i, j] = value;
                    mineAddress++;
                }
                // Go to next row.
                rowAddress += 0x20;
            }

            int left = rect.Left + 13 + 7;  // Get first grid's X point.
            int top = rect.Top + 101 + 7;  // Get first grid's Y point.

            
            
            Win32Api.BlockInput(true);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // The grid has not been clicked and it is a normal grid.
                    if (array[i, j] == 0x0F)
                    {
                        // Set cursor position.
                        Win32Api.SetCursorPos(left + j * 16, top + i * 16);
                        // Mouse down.
                        Win32Api.mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                        // Mouse up.
                        // Win32Api.mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                    }
                    // The grid has been signed as flag by player and it is a normal grid.
                    else if (array[i, j] == 0x0E)
                    {
                        if (isEnableQuestionMark)
                        {
                            // Set cursor position.
                            Win32Api.SetCursorPos(left + j * 16, top + i * 16);
                            Win32Api.mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            // Win32Api.mouse_event(MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            Win32Api.mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            // Win32Api.mouse_event(MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            // Mouse down.
                            Win32Api.mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                            // Mouse up.
                            // Win32Api.mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                        }
                        else
                        {
                            // Set cursor position.
                            Win32Api.SetCursorPos(left + j * 16, top + i * 16);
                            Win32Api.mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            // Win32Api.mouse_event(MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            // Mouse down.
                            Win32Api.mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                            // Mouse up.
                            // Win32Api.mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                        }
                    }
                    // The grid has been signed as "?" by player and it is a normal grid.
                    else if (array[i, j] == 0x0D)
                    {
                        // Set cursor position.
                        Win32Api.SetCursorPos(left + j * 16, top + i * 16);
                        Win32Api.mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                        // Win32Api.mouse_event(MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                        // Mouse down.
                        Win32Api.mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
                        // Mouse up.
                        // Win32Api.mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                    }
                }
            }
            Win32Api.BlockInput(false);
        }

    }
}
