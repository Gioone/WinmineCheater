using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WinmineCheater
{
    /// <summary>
    /// MinesDistributionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MinesDistributionWindow : Window
    {
        private readonly object _lockIsStopTrdRefreshUi = new();
        private bool _isStopTrdRefreshUi = false;
        public bool IsStopTrdRefreshUi
        {
            get
            {
                return _isStopTrdRefreshUi;
            }
            set
            {
                lock (_lockIsStopTrdRefreshUi)
                {
                    _isStopTrdRefreshUi = value;
                }
            }
        }
        private Thread _trdRefreshUi;
        private byte[,] arrMines;
        public MinesDistributionWindow(int rows, int columns)
        {
            InitializeComponent();
            arrMines = new byte[rows, columns];
            Height = rows * 25;
            Width = columns * 22;
            ReadGridMines(rows, columns);
            InitUi(rows, columns);
            _trdRefreshUi = new Thread(RefreshUi)
            {
                IsBackground = true
            };
            _trdRefreshUi.Start();
        }

        private void RefreshUi()
        {
            while (true)
            {
                if (IsStopTrdRefreshUi) return;
                if (MainWindow.Pid == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Grid.Children.Clear();
                    Grid.RowDefinitions.Clear();
                    Grid.ColumnDefinitions.Clear();
                    byte rows = Helper.ReadMemoryValueByte(Address.ROW_ADDRESS, MainWindow.Pid);
                    byte columns = Helper.ReadMemoryValueByte(Address.COLUMN_ADDRESS, MainWindow.Pid);
                    // Height = rows * 25;
                    // Width = columns * 22;
                    for (int gridRow = 0; gridRow < rows; gridRow++)
                    {
                        RowDefinition row = new()
                        {
                            Height = GridLength.Auto
                        };
                        Grid.RowDefinitions.Add(row);
                    }
                    for (int gridColumn = 0; gridColumn < columns; gridColumn++)
                    {
                        ColumnDefinition column = new()
                        {
                            Width = GridLength.Auto
                        };
                        Grid.ColumnDefinitions.Add(column);
                    }
                    arrMines = new byte[rows, columns];
                    ReadGridMines(rows, columns);
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            Label lbl = GenerateGridLabel(arrMines[i, j]);
                            
                            Grid.Children.Add(lbl);
                            lbl.SetValue(Grid.RowProperty, i);
                            lbl.SetValue(Grid.ColumnProperty, j);
                        }
                    }
                });
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Generate grid label.
        /// </summary>
        /// <param name="value">An array element from reading memory.</param>
        /// <returns><seealso cref="Label" /></returns>
        private Label GenerateGridLabel(byte value)
        {
            Label lbl = new()
            {
                Width = 20,
                Height = 20,
                Padding = new Thickness(3, 3, 3, 3),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = ""
            };
            // Number
            if (value >= 'A' - 1 && value <= 'J')
            {
                int num = value - 64;
                if (num == 0)
                {
                    lbl.Content = "";
                }
                else
                {
                    switch (num)
                    {
                        case 1:
                            lbl.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        case 2:
                            lbl.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        case 3:
                            lbl.Foreground = new SolidColorBrush(Colors.Red);
                            break;
                        case 4:
                            lbl.Foreground = new SolidColorBrush(Colors.DarkBlue);
                            break;
                        case 5:
                            lbl.Foreground = new SolidColorBrush(Colors.Brown);
                            break;
                        case 6:
                            lbl.Foreground = new SolidColorBrush(Colors.LightGreen);
                            break;
                        case 7:
                            lbl.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        case 8:
                            lbl.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                    }
                    lbl.FontWeight = FontWeights.Bold;
                    lbl.Content = num.ToString();
                }
            }
            // Mine
            else if (value == 0xCC || value == 0x8F)
            {
                // lbl.Background = new ImageBrush(img);
                // lbl.Background = new ImageBrush(ByteArrayToBitmapImage(Properties.Resources.Icon));
                lbl.Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(@"Images\Icon.png", UriKind.Relative)),
                };
            }
            // Flag
            else if (value == 0x8E || value == 0x0E)
            {
                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(@"Images\Flag.png", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Uniform
                };
                lbl.Padding = new Thickness(2);
                lbl.Content = img;
            }
            // "?" signature
            else if (value == 0x8D || value == 0x0D)
            {
                lbl.FontWeight = FontWeights.Bold;
                lbl.Content = "?";
                lbl.Foreground = new SolidColorBrush(Colors.Black);
            }
            return lbl;
        }

        private void InitUi(int rows, int columns)
        {
            #region Grid init.
            for (int gridRow = 0; gridRow < rows; gridRow++)
            {
                RowDefinition row = new()
                {
                    Height = GridLength.Auto
                };
                Grid.RowDefinitions.Add(row);
            }
            for (int gridColumn = 0; gridColumn < columns; gridColumn++)
            {
                ColumnDefinition column = new()
                {
                    Width = GridLength.Auto
                };
                Grid.ColumnDefinitions.Add(column);
            }
            #endregion

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Label lbl = new()
                    {
                        Width = 20,
                        Height = 20,
                        Padding = new Thickness(3, 3, 3, 3),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    if (arrMines[i, j] >= 'A' - 1 && arrMines[i, j] <= 'J')
                    {
                        int num = arrMines[i, j] - 64;
                        if (num == 0)
                        {
                            lbl.Content = "";
                        }
                        else
                        {
                            switch (num)
                            {
                                case 1:
                                    lbl.Foreground = new SolidColorBrush(Colors.Blue);
                                    break;
                                case 2:
                                    lbl.Foreground = new SolidColorBrush(Colors.Green);
                                    break;
                                case 3:
                                    lbl.Foreground = new SolidColorBrush(Colors.Red);
                                    break;
                                case 4:
                                    lbl.Foreground = new SolidColorBrush(Colors.DarkBlue);
                                    break;
                                case 5:
                                    lbl.Foreground = new SolidColorBrush(Colors.Brown);
                                    break;
                                case 6:
                                    lbl.Foreground = new SolidColorBrush(Colors.LightGreen);
                                    break;
                                case 7:
                                    lbl.Foreground = new SolidColorBrush(Colors.Black);
                                    break;
                                case 8:
                                    lbl.Foreground = new SolidColorBrush(Colors.Gray);
                                    break;
                            }
                            lbl.FontWeight = FontWeights.Bold;
                            lbl.Content = num.ToString();
                        }
                    }
                    else if (arrMines[i, j] == 0xCC || arrMines[i, j] == 0x8F)
                    {
                        /*BitmapImage img = new(new Uri(@"images\icon.png", UriKind.Relative))
                        {
                            CacheOption = BitmapCacheOption.OnLoad
                        };*/
                        lbl.Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(new Uri(@"Images\Icon.png", UriKind.Relative))
                        };
                        // lbl.Background = new ImageBrush(img);
                    }

                    Grid.Children.Add(lbl);
                    lbl.SetValue(Grid.RowProperty, i);
                    lbl.SetValue(Grid.ColumnProperty, j);
                }
            }
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArr)
        {
            BitmapImage img = new();
            using MemoryStream ms = new(byteArr);
            img.BeginInit();
            img.StreamSource = ms;
            // img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            // img.Freeze();
            return img;
        }

        private void ReadGridMines(int rows, int columns)
        {
            // First row address.
            int rowAddress = Address.MINE_START_ADDRESS;
            for (int i = 0; i < rows; i++)
            {
                // First mine address.
                int mineAddress = rowAddress;
                for (int j = 0; j < columns; j++)
                {
                    byte value = Helper.ReadMemoryValueByte(mineAddress, MainWindow.Pid);
                    arrMines[i, j] = value;
                    mineAddress++;
                }
                // Go to next row.
                rowAddress += 0x20;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsStopTrdRefreshUi = true;
            Helper.IsMinesDeistributionWindowOpend = false;
        }
    }
}
