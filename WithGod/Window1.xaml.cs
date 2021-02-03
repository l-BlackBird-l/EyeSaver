using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WithGod;

namespace SystemUpdate
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Thread CheckConnection;

        public Window1()
        {
            InitializeComponent();

            this.Hide();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length < 2)
            {
                this.Show();
                LoadDate();

            }
            else
            {
                MainWindow window = new MainWindow();
                window.Show();
            }
            if (Properties.Settings.Default.Password.Length != 0)
            {
                EnterPassword.IsOpen = true;
            }

           
        }

       // bool Unblock = true;

        public void Thread_Check()
        {

                try
                {
                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    LoadingBar.Value = 10;
                }));
                string msg = Send_Arduino("ready");

                Dispatcher.BeginInvoke(new ThreadStart(delegate {
                    LoadingBar.Value = 90;
                }));
                if (msg == "")
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {
                            StylFalse.Visibility = Visibility.Visible;
                            StylTrue.Visibility = Visibility.Hidden;
                            ModuleText.Text = "Вiдключено";
                            StylText.Text = "Вiдключено";
                            ModuleFalse.Visibility = Visibility.Visible;
                            ModuleTrue.Visibility = Visibility.Hidden;
                        }));
                    }
                     if (msg.Contains("#rdOne"))
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {

                            StylFalse.Visibility = Visibility.Visible;
                            StylTrue.Visibility = Visibility.Hidden;
                            ModuleText.Text = "Пiдключено";
                            StylText.Text = "Вiдключено";
                            ModuleFalse.Visibility = Visibility.Hidden;
                            ModuleTrue.Visibility = Visibility.Visible;
                        }));
                    }
                     if (msg.Contains("#rdTwo"))
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {
                            StylFalse.Visibility = Visibility.Hidden;
                            StylTrue.Visibility = Visibility.Visible;
                            ModuleText.Text = "Пiдключено";
                            StylText.Text = "Пiдключено";
                            ModuleFalse.Visibility = Visibility.Hidden;
                            ModuleTrue.Visibility = Visibility.Visible;
                        }));

                }

                }
                catch (Exception ex)
                {
                 
            }
            Dispatcher.BeginInvoke(new ThreadStart(delegate {
                Loading.IsOpen = false;
            }));

        }



        public void LoadDate()
        {
            _Pass_Box.Text = Properties.Settings.Default.Password;

            ClickValue.Text = Properties.Settings.Default.Click_Value.ToString();


            if (Properties.Settings.Default.IsMin == true)
            {
                min.IsChecked = true;
                int del = Properties.Settings.Default.Delay;
                del /= 60000;
                _Delay_value.Text = del.ToString();
            }
            else
            {
                sek.IsChecked = true;
                int del = Properties.Settings.Default.Delay;
                del /= 1000;
                _Delay_value.Text = del.ToString();
            }

            if (Properties.Settings.Default.Video_Lenght == 0) _Zero_min.IsChecked = true;
            else if (Properties.Settings.Default.Video_Lenght == 1) _One_min.IsChecked = true;
            else if (Properties.Settings.Default.Video_Lenght == 3) _Three_min.IsChecked = true;
            else _Five_min.IsChecked = true;
        }


        public string Send_Arduino(string Date)
        {

            string msg = "";
            bool connectionFlag = false;

            DateTime time = new DateTime();
            //var lastTime = time.Millisecond;
            Dispatcher.BeginInvoke(new ThreadStart(delegate { LoadingBar.Value = 20; }));
            string[] portnames = SerialPort.GetPortNames();
            Dispatcher.BeginInvoke(new ThreadStart(delegate { LoadingBar.Value = 30; }));
            try
            {
                // MessageBox.Show("try");
                Dispatcher.BeginInvoke(new ThreadStart(delegate { LoadingBar.Value = 40; }));
                foreach (var name in portnames)
                {
                 //   MessageBox.Show("for");
                    var serialPort = new SerialPort();
                    serialPort.PortName = name;
                    serialPort.BaudRate = 9600;
                    serialPort.RtsEnable = true;
                    //serialPort.ReadTimeout = 1000;
                    //  MessageBox.Show(name);
                    Dispatcher.BeginInvoke(new ThreadStart(delegate { LoadingBar.Value = 50; }));
                    try
                    {
                        serialPort.Open();


                        if (serialPort.IsOpen)
                        {
                            serialPort.WriteLine(Date);

                            Thread.Sleep(200);
                            var lastTime = time.Millisecond;

                            while ((time.Millisecond - lastTime) <= 3000)
                            {
                                serialPort.WriteLine(Date);
                                
                                Thread.Sleep(200);
                                msg = serialPort.ReadExisting();
                                if (msg.Contains("#rdOne") || msg.Contains("#rdTwo")) { connectionFlag = true; break; }// MessageBox.Show(msg); break; }
                            } 
                        }
                        serialPort.Close();
                    }
                    catch (Exception ex) { }
                }
                Dispatcher.BeginInvoke(new ThreadStart(delegate { LoadingBar.Value = 60; }));
                if (!connectionFlag) {
                    Dispatcher.BeginInvoke(new ThreadStart(delegate
                    {
                        StylFalse.Visibility = Visibility.Visible;
                        StylTrue.Visibility = Visibility.Hidden;
                        ModuleText.Text = "Вiдключено";
                        StylText.Text = "Вiдключено";
                        ModuleFalse.Visibility = Visibility.Visible;
                        ModuleTrue.Visibility = Visibility.Hidden;
                        Loading.IsOpen = false;
                        txt.Text = "Модуль передавача не підключено.";
                        textDialog.IsOpen = true;
                    }));
                }
            }
            catch (Exception ex)
            {

                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {

                    StylFalse.Visibility = Visibility.Visible;
                    StylTrue.Visibility = Visibility.Hidden;
                    ModuleText.Text = "Вiдключено";
                    StylText.Text = "Вiдключено";
                    ModuleFalse.Visibility = Visibility.Visible;
                    ModuleTrue.Visibility = Visibility.Hidden;
                    Loading.IsOpen = false;
                }));
            }
            Dispatcher.BeginInvoke(new ThreadStart(delegate { LoadingBar.Value = 70; }));
            // MessageBox.Show("end");
            return msg;
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            if (UserPassword.Password == Properties.Settings.Default.Password) { EnterPassword.IsOpen = false; }
            else WrongPass.IsOpen = true;
        }


        private void Accept_Button_Click(object sender, RoutedEventArgs e)
        {

            if (ModuleFalse.Visibility == Visibility.Hidden || StylFalse.Visibility == Visibility.Hidden)
            {
                Settings.Visibility = Visibility.Visible;
                Activ.Visibility = Visibility.Hidden;
            }
            else
            {
                txt.Text = " Один iз модулiв не пiдключено.Перевiрте з'єднання модулiв.";
                textDialog.IsOpen = true;
            }
        }

        private void Ok1_Click(object sender, RoutedEventArgs e)
        {
            WrongPass.IsOpen = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            textDialog.IsOpen = false;
        }

        private void Declare_Button_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Hidden;
            Activ.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (Settings.Visibility == Visibility.Visible)
            {

                Process _process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = "EyeSaver.exe";
                startInfo.Arguments = "NotFirst";
                _process.StartInfo = startInfo;
                _process.Start();
            }
            Application.Current.Shutdown();
        }

        private void _Delay_value_KeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key.GetHashCode() >= 34) && (e.Key.GetHashCode() <= 43)) && !((e.Key.GetHashCode() >= 74) && (e.Key.GetHashCode() <= 83)))
            {
                e.Handled = true;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.Password = _Pass_Box.Text;
                Properties.Settings.Default.Click_Value = Convert.ToInt32(ClickValue.Text);
                if (min.IsChecked == true) Properties.Settings.Default.IsMin = true;
                else Properties.Settings.Default.IsMin = false;

                if(min.IsChecked == true)   Properties.Settings.Default.Delay = Convert.ToInt32(_Delay_value.Text) * 60000;
                else Properties.Settings.Default.Delay = Convert.ToInt32(_Delay_value.Text) * 1000;


                if (_Zero_min.IsChecked == true) Properties.Settings.Default.Video_Lenght = 0;
                else if (_One_min.IsChecked == true) Properties.Settings.Default.Video_Lenght = 1;
                else if (_Three_min.IsChecked == true) Properties.Settings.Default.Video_Lenght = 3;
                else Properties.Settings.Default.Video_Lenght = 5;

                Properties.Settings.Default.Five_Min = true;
                Properties.Settings.Default.Save();

                txt.Text = "Дані збережено.";
                textDialog.IsOpen = true;
            }
            catch(Exception ex)
            {
                txt.Text = ex.Message;
                textDialog.IsOpen = true;
                LoadDate();
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            Loading.IsOpen = true;
            LoadingBar.Value = 0;
            CheckConnection = new Thread(new ThreadStart(Thread_Check));
            CheckConnection.Start();
        }
    }
}
