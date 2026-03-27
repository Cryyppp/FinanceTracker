using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        decimal bal = 0.00M;
        string name;
        string surname;
        string PathUserData = @"../../../Data/UserData.txt";
        public MainWindow()
        {

            InitializeComponent();
            Task.Run(async () => await BackGround());
            if (!File.Exists(PathUserData) || File.ReadAllLines(PathUserData).Count() == 0)
            {
                using StreamWriter sw = new StreamWriter(PathUserData);
                sw.Close();
                ShowLogin(true);
            }
            else
            {
                ShowLogin(false);
            }
            LoadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            name = _txtboxname.Text;
            surname = _txtboxsurname.Text;
            bal = decimal.Parse(_txtboxbalance.Text);

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                MessageBox.Show("Please enter both name and surname.");
                return;
            }
            else
            {
                try
                {
                    string userData = $"{name};{surname};{bal}";
                    StreamWriter sw = new StreamWriter(PathUserData, true);
                    sw.Write(userData);
                    sw.Close();
                    ShowLogin(false);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving user data: {ex.Message}");
                }
            }
            LoadData();

        }
        private void ShowLogin(bool var)
        {
            switch (var)
            {
                case true:
                    _loginGrid.Visibility = Visibility.Visible;
                    _pageGrid.Visibility = Visibility.Hidden;
                    break;
                case false:
                    _loginGrid.Visibility = Visibility.Hidden;
                    _pageGrid.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void LoadData()
        {
            try
            {
                if (File.Exists(PathUserData) && File.ReadAllLines(PathUserData).Count() > 0)
                {
                    string[] userData = File.ReadAllLines(PathUserData);
                    string[] userInfo = userData[0].Split(';');
                    name = userInfo[0];
                    surname = userInfo[1];
                    bal = decimal.Parse(userInfo[2]);
                    _txtName.Text = $"{name} {surname}";
                    _txtboxbalance.Text = $"€ {bal:F2}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading user data: {ex.Message}");
            }
        }
        private void UpdateBal()
        {
            _txtboxbalance.Text = $"€ {bal:F2}";
            if (bal < 0)
            {
                _txtboxbalance.Foreground = Brushes.Red;
            }
            else if (bal == 0)
            {
                _txtboxbalance.Foreground = Brushes.Black;
            }
            else
            {
                _txtboxbalance.Foreground = Brushes.Green;
            }
        }

        private async Task BackGround()
        {
            while (true)
            {
                await Dispatcher.InvokeAsync(UpdateBal);
                await Task.Delay(1000);
            }
        }
    }
}