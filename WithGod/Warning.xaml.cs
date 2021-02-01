using System.Windows;

namespace SystemUpdate
{
    /// <summary>
    /// Логика взаимодействия для Warning.xaml
    /// </summary>
    public partial class Warning : Window
    {
        public Warning()
        {
            InitializeComponent();

            Admin.IsOpen = true;
            if (Properties.Settings.Default.Five_Min == false) Plus_five.IsEnabled = false;
        }

        void Event_On_Clicks(bool Task)
        {
            Admin.IsOpen = false;

            if(Task)  txt.Text =  "Блокування відбудеться через 30 секунд!";
            else txt.Text = "Блокування відбудеться через 10 секунд!";
            textDialog.IsOpen = true;

            this.DialogResult = Task;
            this.Close();
        }
 
        private void Plus_five_Click(object sender, RoutedEventArgs e)
        {
            Event_On_Clicks(true);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            textDialog.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Event_On_Clicks(false);
        }
    }
}
