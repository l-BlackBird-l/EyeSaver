using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using SystemUpdate;

namespace WithGod
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WH_KEYBOARD_LL = 13;

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
        }

        private LowLevelKeyboardProcDelegate m_callback;
        private LowLevelKeyboardProcDelegate m_callback_1;
        private LowLevelKeyboardProcDelegate m_callback_2;
        private LowLevelKeyboardProcDelegate m_callback_3;
        private LowLevelKeyboardProcDelegate m_callback_4;
        private LowLevelKeyboardProcDelegate m_callback_5;
        private LowLevelKeyboardProcDelegate m_callback_6;

        private IntPtr m_hHook;
        private IntPtr m_hHook_1;
        private IntPtr m_hHook_2;
        private IntPtr m_hHook_3;
        private IntPtr m_hHook_4;
        private IntPtr m_hHook_5;
        private IntPtr m_hHook_6;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
      [In] IntPtr hWnd,
      [In] int id,
      [In] uint fsModifiers,
      [In] uint vk);


        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey1(
        [In] IntPtr hWnd,
        [In] int id,
        [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_P = 0x50;
            const uint MOD_CTRL = 0x0001;
            if (!RegisterHotKey(helper.Handle, 2, MOD_CTRL, VK_P))
            {
                // handle error
            }

            // Удалить к чертовой бабушке
            var helper1 = new WindowInteropHelper(this);
            const uint VK_F12 = 0x7B;
            const uint U_CTRL = 0x0001;
            if (!RegisterHotKey(helper1.Handle, 1, U_CTRL, VK_F12))
            {
                // handle error
            }

        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, 2);

            var helper1 = new WindowInteropHelper(this);
            UnregisterHotKey(helper1.Handle, 1);

        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case 2:
                            OnHotKeyPressed();
                            handled = true;
                            break;

                        case 1:
                            OnHotKeyPressed2();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            PlayVideo();
        }

        private void OnHotKeyPressed2()
        {
            close = true;

            System.Windows.Forms.MessageBox.Show("Видалення програми запущено! Дякуємо за те, що використовували наш програмний продукт.");

            Unhook();

            WinD();

            Process.Start(System.IO.Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe")); // Разблокируем процесс експлорер

            Process.GetProcessesByName("taskmgr")[0].Kill();// Убиваем диспетчер задач

            SystemUpdate.Properties.Settings.Default.Block = false;
            SystemUpdate.Properties.Settings.Default.Save();

            SetAutorunValue(false);

            Environment.Exit(1); // Перезапускаем программу
        }

        public IntPtr LowLevelKeyboardHookProc_alt_tab(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.Tab)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(m_hHook, nCode, wParam, lParam);
        }

        public IntPtr LowLevelKeyboardHookProc_win(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(m_hHook_1, nCode, wParam, lParam);
        }

        public IntPtr LowLevelKeyboardHookProc_del(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.Delete)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(m_hHook_3, nCode, wParam, lParam);
        }

        public IntPtr LowLevelKeyboardHookProc_alt(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.Alt)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(m_hHook_4, nCode, wParam, lParam);
        }


        public IntPtr LowLevelKeyboardHookProc_ctrl(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.RControlKey || objKeyInfo.key == Keys.LControlKey)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(m_hHook_2, nCode, wParam, lParam);
        }


        public IntPtr LowLevelKeyboardHookProc_alt_space(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.Space)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(m_hHook_5, nCode, wParam, lParam);
        }

        public IntPtr LowLevelKeyboardHookProc_control_shift_escape(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.LControlKey || objKeyInfo.key == Keys.RControlKey || objKeyInfo.key == Keys.LShiftKey || objKeyInfo.key == Keys.RShiftKey || objKeyInfo.key == Keys.Escape)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(m_hHook_6, nCode, wParam, lParam);
        }

        private delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        [Flags]
        enum MouseFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            Absolute = 0x8000
        };

        public void SetHook()
        {
            m_callback = LowLevelKeyboardHookProc_win;
            m_callback_1 = LowLevelKeyboardHookProc_alt_tab;
            m_callback_2 = LowLevelKeyboardHookProc_ctrl;
            m_callback_3 = LowLevelKeyboardHookProc_del;
            m_callback_4 = LowLevelKeyboardHookProc_alt;
            m_callback_5 = LowLevelKeyboardHookProc_alt_space;
            m_callback_6 = LowLevelKeyboardHookProc_control_shift_escape;

            m_hHook = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_1 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_1, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_2 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_2, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_3 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_3, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_4 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_4, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_5 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_5, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_6 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_6, GetModuleHandle(IntPtr.Zero), 0);

        }
        // Разблокирываем все комбинации
        public void Unhook()
        {
            UnhookWindowsHookEx(m_hHook);
            UnhookWindowsHookEx(m_hHook_1);
            UnhookWindowsHookEx(m_hHook_2);
            UnhookWindowsHookEx(m_hHook_3);
            UnhookWindowsHookEx(m_hHook_4);
            UnhookWindowsHookEx(m_hHook_5);
            UnhookWindowsHookEx(m_hHook_6);
        }

        static class KeyboardSend // Имитация Нажатий
        {
            [DllImport("user32.dll")]
            private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

            private const int KEYEVENTF_EXTENDEDKEY = 1;
            private const int KEYEVENTF_KEYUP = 2;

            public static void KeyDown(Keys vKey)
            {
                keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
            }

            public static void KeyUp(Keys vKey)
            {
                keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }


        public void WinD()
        {
            // Сворачиваем все окназд
            KeyboardSend.KeyDown(Keys.LWin);
            KeyboardSend.KeyDown(Keys.D);
            KeyboardSend.KeyUp(Keys.LWin);
            KeyboardSend.KeyUp(Keys.D);
        }

        public string Port = "";
        SerialPort serialPort;
        public void Read_Arduino()
        {
            serialPort = new SerialPort();
            serialPort.PortName = Port; //dd
            serialPort.Open();
            string msg;

                while (true)
                {
                    msg = serialPort.ReadExisting();
                    if (msg.Contains("#done")) {
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { PlayVideo(); })); serialPort.Close(); Unblock1 = false; break; }
                }
            
        }



        public string Connection_Arduino(string Date, bool st5)
        {
            string msg = "";
            bool connectionFlag = false;

            DateTime time = new DateTime();
            //var lastTime = time.Millisecond;
            string[] portnames = SerialPort.GetPortNames();
            try
            {
                // MessageBox.Show("try");
                foreach (var name in portnames)
                {
                    //   MessageBox.Show("for");
                     serialPort = new SerialPort();
                    serialPort.PortName = name;
                    serialPort.BaudRate = 9600;
                    serialPort.RtsEnable = true;
                    //serialPort.ReadTimeout = 1000;
                    //  MessageBox.Show(name);
                    try
                    {
                        serialPort.Open();


                        if (serialPort.IsOpen)
                        {
                            serialPort.WriteLine(Date);

                            Thread.Sleep(200);
                            if (!st5)
                            {

                                var lastTime = time.Millisecond;

                                while ((time.Millisecond - lastTime) <= 3000)
                                {

                                    msg = serialPort.ReadExisting();
                                    if (msg.Contains("#rdOne") || msg.Contains("#rdTwo")) { connectionFlag = true; serialPort.Close(); Port = name; return msg; }// MessageBox.Show(msg); break; }
                                }

                            }
                            }
                        
                        serialPort.Close();
                    }
                    catch (Exception ex) { }
                }
                if (!connectionFlag)
                {
                   
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            // MessageBox.Show("end");
            return msg;
        }


    


        bool Unblock1 = true;

        public void Thread_Unblock()
        {
            while (Unblock1)
            {
                Thread.Sleep(1);
                Read_Arduino();
            }
        }

        public void Blocker()
        {
            try
            {
                string msg = Connection_Arduino("ready", false);
                string ask =  Connection_Arduino("start" + SystemUpdate.Properties.Settings.Default.Click_Value.ToString(), true);
                WinD();

                SetHook();

               Form1.Visibility = Visibility.Visible;
                
                Process p = new Process();// Создаем новый процесс
                p.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);// Директория к диспетчеру
                p.StartInfo.FileName = "taskmgr.exe";// Название диспетчера
                p.StartInfo.CreateNoWindow = true;// Создаем его без окна
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;// Делаем окно диспетчера задач - скрытим                                                 
                p.Start();// Запускаем диспетчер задача

                var proc = new Process // Убиваем експлорер
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "taskkill.exe",
                        Arguments = "/F /IM explorer.exe",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                this.Focus();

                  Thread UB = new Thread(new ThreadStart(Thread_Unblock));
                 UB.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        bool close = false ;

        public bool SetAutorunValue(bool autorun) { // Добавляем программу в автозапуск 
            string ExePath = System.Windows.Forms.Application.ExecutablePath;
            string name = "EyeSaver";
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try {
                if (autorun) {
                    reg.SetValue(name, ExePath);
                }
                else {
                    reg.DeleteValue(name);
                }
                reg.Close();
            }
            catch {
                return false;
            }
            return true;
        }

        public void Warning()
        {
            int Delay = 10000;

            Warning dialog = new Warning();

            if (dialog.ShowDialog() == true)
            {
                SystemUpdate.Properties.Settings.Default.Five_Min = false;
                SystemUpdate.Properties.Settings.Default.Save();
                Delay += 20000;
            }
            dialog.Close();
            Thread.Sleep(Delay);
            Blocker();
        }

        public MainWindow()
        {
            InitializeComponent();

             //     SetAutorunValue(true);
            int delay = SystemUpdate.Properties.Settings.Default.Delay;
            Thread.Sleep(delay);
                Warning();
        }


        public void UnBlock()
        {
            try
            {
                Unhook();

                WinD();

                close = true;

                //    port.Dispose();


                Process.Start(Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe")); // Разблокируем процесс експлорер

                Process.GetProcessesByName("taskmgr")[0].Kill();// Убиваем диспетчер задач
                Unblock1 = false;
                System.Windows.Application.Current.Shutdown();

                Process _process = new Process();

                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = "EyeSaver.exe";
                startInfo.Arguments = "NotFirst";
                Unblock1 = false;
                _process.StartInfo = startInfo;
                _process.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }


        private void PlayVideo()
        {
            int len = SystemUpdate.Properties.Settings.Default.Video_Lenght;
            string path = null, Download_Path = null, path_Name = null;
            switch(len)
            {
                case 0:
                    UnBlock();
                break;

                case 1:
                    path_Name = "Video_One.mp4";
                    path = System.IO.Path.GetFullPath(path_Name);
                    Download_Path = "https://drive.google.com/uc?export=download&id=1GDgiaxnBag-FU1AlZ6ZwQGdZYb5cfqLA";
                break;

                case 3:
                    path_Name = "Video_Three.mp4";
                    path = System.IO.Path.GetFullPath(path_Name);
                    Download_Path = "https://drive.google.com/uc?export=download&id=1tEw6qY0VvC4rlMGt8OZwMLuHBRjN03tN";
               break;

                case 5:
                    path_Name = "Video_Five.mp4";
                    path = System.IO.Path.GetFullPath(path_Name);
                    Download_Path = "https://drive.google.com/uc?export=download&id=1zXs0fNeCVJH53F7tMIypKyRHYMRU36yp";
               break;
            }
            if (len != 0)
            {
                var uri = new Uri(path);
                if (File.Exists(path))
                {
                    Video.Source = uri;
                    return;
                }

                IPStatus status = IPStatus.Unknown;
                try { status = new Ping().Send("google.com").Status; }
                catch { }

                if (status == IPStatus.Success)
                {
                    WebClient web = new WebClient();
                    web.DownloadFile(Download_Path, path_Name);
                    Video.Source = uri;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (close == false) e.Cancel = true;
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_Pass_Box.Text == SystemUpdate.Properties.Settings.Default.Password)
            {
                try
                {
                    close = true;

                    Unhook();

                    WinD();

                    Process.Start(System.IO.Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe")); // Разблокируем процесс експлорер

                    Process.GetProcessesByName("taskmgr")[0].Kill();// Убиваем диспетчер задач

                    SystemUpdate.Properties.Settings.Default.Block = false;
                    SystemUpdate.Properties.Settings.Default.Save();
                    Unblock1 = false;
                    Connection_Arduino("stop!", true);
                   
                    Environment.Exit(1); // Перезапускаем программу
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                    Environment.Exit(1); // Перезапускаем программу
                }
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            UnBlock();
        }

        private void UnButton_Click(object sender, RoutedEventArgs e)
        {
            Pass.IsOpen = true;
        }

    }
}
