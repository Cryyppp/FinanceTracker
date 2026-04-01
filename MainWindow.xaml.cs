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
        List<Item> ListAllTransaction = new List<Item>();
        List<Income> ListMonthIncome = new List<Income>();
        List<Subscription> ListNearRenewals = new List<Subscription>();
        List<Subscription> ListSubscription = new List<Subscription>();
        List<Item> MonthTransaction = new List<Item>();

        decimal monthlySpent = 0.00M;
        decimal bal = 0.00M;
        string name;
        string surname;
        string PathUserData = @"../../../Data/UserData.txt";
        string PathSubsciptionData = @"../../../Data/SubscriptionData.txt";
        string PathActivityData = @"../../../Data/ActivityData.txt";
        public MainWindow()
        {
            InitializeComponent();

            ListSubscription.Add(new Subscription("Spotify", "Spotify renwal", DateTime.Today.AddDays(5), 12.99m));
            ListSubscription.Add(new Subscription("Youtube", "Spotify", DateTime.Today.AddDays(19), 12.99m));
            ListSubscription.Add(new Subscription("Netflix", "Netflix renwal", DateTime.Today, 12.99m));
            ListSubscription.Add(new Subscription("Amazon Prime", "Amazon Prime renwal", DateTime.Today.AddDays(10), 12.99m));
            ListSubscription.Add(new Subscription("Disney+", "Disney+ renwal", DateTime.Today.AddDays(15), 12.99m));
            ListSubscription.Add(new Subscription("HBO Max", "HBO Max renwal", DateTime.Today.AddDays(7), 12.99m));
            ListSubscription.Add(new Subscription("Apple Music", "Apple Music renwal", DateTime.Today.AddDays(3), 12.99m));
            ListSubscription.Add(new Subscription("Google One", "Google One renwal", DateTime.Today.AddDays(12), 12.99m));
            ListSubscription.Add(new Subscription("Dropbox", "Dropbox renwal", DateTime.Today.AddDays(8), 12.99m));


            _expensesBox.Visibility = Visibility.Hidden;

            if (!File.Exists(PathUserData) || File.ReadAllLines(PathUserData).Count() == 0)
            {
                using StreamWriter sw = new StreamWriter(PathUserData);
                sw.Close();
                try
                {
                    StreamWriter swSub = new StreamWriter(PathSubsciptionData);
                    swSub.Close();
                    StreamWriter swAct = new StreamWriter(PathActivityData);
                    swAct.Close();
                } catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while initializing data files: {ex.Message}");
                }
                ShowLogin(true);
            }
            else
            {
                ShowLogin(false);
                Task.Run(async () => await BackGround());
            }
            LoadData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            name = _txtboxname.Text;
            surname = _txtboxsurname.Text;
            bal = decimal.Parse(_txtSaldo.Text);

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
            Task.Run(async () => await BackGround());
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
            _txtSubsription.Text = ListSubscription.Count.ToString();
            _txtNearRenwals.Text = ListNearRenewals.Count.ToString();
        }

        private async Task BackGround()
        {
            while (true)
            {
                await Dispatcher.InvokeAsync(UpdateBal);
                await Dispatcher.InvokeAsync(CheckRenewals);
                await Dispatcher.InvokeAsync(CheckMonthlySpent);
                ListAllTransaction.Clear();
                ListAllTransaction.AddRange(ListMonthIncome);
                ListAllTransaction.AddRange(ListSubscription);
            }

        }

        private async Task CheckMonthlySpent()
        {
            DateTime today = DateTime.Today;
            foreach (Item item in ListSubscription)
            {
                if (today.Date.Month == item.Date.Month - 1  && !MonthTransaction.Contains(item))
                {
                    MonthTransaction.Add(item);
                    monthlySpent += item.Price;
                }
            }
            _txtMonthTransaction.Text = $"-€ {monthlySpent:F2}";
            _txtMonthTransaction.Foreground = Brushes.Red;

        }

        private void CheckRenewals()
        {
            ListNearRenewals.Clear();
            DateTime today = DateTime.Today;
            foreach (Subscription sub in ListSubscription)
            {
                if ((sub.Date - today).TotalDays <= 14 && (sub.Date - today).TotalDays >= 0)
                {
                    if(sub.Date == today)
                    {
                        bal -= sub.Price;
                        sub.Date = DateTime.Today.AddMonths(1);
                    }
                    ListNearRenewals.Add(sub);
                }
            }
        }

        private void BtnSeeMore_Click(object sender, RoutedEventArgs e)
        {
            _expensesBox.Visibility = Visibility.Visible;
            Button btn = sender as Button;
            string expenseInfo = btn.Name.ToString();
            switch(expenseInfo)
            {
                case "_btnExpences":
                    ShowExpences();
                    break;
                case "_btnIncome":
                    break;
                case "_btnRenewal":
                    ShowAllRenwals();
                    break;
                case "_btnSubscription":
                    ShowAllSubscriptions();
                    break;
            }
        }

        private void ShowExpences()
        {
            List<Item> TmonthTransaction = MonthTransaction.OrderByDescending(time => time.Date.Date).ToList();
            _itemsList.ItemsSource = TmonthTransaction;
            _viewMoreGrid.Visibility = Visibility.Visible;
        }
        private void ShowAllRenwals()
        {

            List<Subscription> Tnearest = ListNearRenewals.OrderBy(time => time.Date.Date).ToList();
            
            _itemsList.ItemsSource = Tnearest;
            _viewMoreGrid.Visibility = Visibility.Visible;
        }

        private void ShowAllSubscriptions()
        {
            _itemsList.ItemsSource = ListSubscription;
            _viewMoreGrid.Visibility = Visibility.Visible;
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            _expensesBox.Visibility = Visibility.Hidden;
            _itemsList.ItemsSource = null;
        }
    }
}