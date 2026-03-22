using System.IO;
using System.Text;
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
        decimal bal = 0;

        string name;
        string surname;
        string PathUserData = @".\Data\user_data.txt";
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(PathUserData) || File.ReadAllLines(PathUserData).Count() == 0)
            {
                using StreamWriter sw = new StreamWriter(PathUserData);
                sw.Close();
                ShowLogin(true);
            } else
            {
                ShowLogin(false);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            name = _txtboxname.Text;
            surname = _txtboxsurname.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                MessageBox.Show("Please enter both name and surname.");
                return;
            }
            else
            {
                try
                {
                    string userData = $"{name},{surname}";
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
    }
}