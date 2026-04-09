using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FinanceTracker
{
    public partial class MainWindow : Window
    {
        List<Item> ListAllTransaction = new List<Item>();
        List<Income> ListMonthIncome = new List<Income>();
        List<Subscription> ListNearRenewals = new List<Subscription>();
        List<Subscription> ListSubscription = new List<Subscription>();
        List<Expense> ListExpenses = new List<Expense>();
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

            bool isSubDataEmpty = !File.Exists(PathSubsciptionData) || File.ReadAllLines(PathSubsciptionData).Length == 0;

            if (!File.Exists(PathUserData) || File.ReadAllLines(PathUserData).Length == 0)
            {
                File.WriteAllText(PathUserData, "");
                File.WriteAllText(PathSubsciptionData, "");
                File.WriteAllText(PathActivityData, "");
                ShowLogin(true);
            }
            else
            {
                ShowLogin(false);
                Task.Run(async () => await BackGround());
            }

            LoadData();
            LoadTransactions();

            if (isSubDataEmpty)
            {
                AddTransaction("Gas", "Car fuel", DateTime.Today.AddDays(-1), 50.00M);
                AddSubscription("Netflix", "Streaming service", DateTime.Today.AddDays(10), 15.99M, true);
            }

            _expensesBox.Visibility = Visibility.Hidden;
        }

        // =========================
        // USER DATA
        // =========================
        private void SaveUserData()
        {
            try
            {
                File.WriteAllText(PathUserData, $"{name};{surname};{bal}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user data: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(PathUserData))
                {
                    string[] userInfo = File.ReadAllLines(PathUserData)[0].Split(';');

                    name = userInfo[0];
                    surname = userInfo[1];
                    bal = decimal.Parse(userInfo[2]);

                    _txtName.Text = $"{name} {surname}";
                    _txtboxbalance.Text = $"€ {bal:F2}";
                }
            }
            catch
            {
                MessageBox.Show("Error loading user data.");
            }
        }

        // =========================
        // TRANSACTIONS
        // =========================
        private void LoadTransactions()
        {
            if (!File.Exists(PathSubsciptionData)) return;

            foreach (string line in File.ReadAllLines(PathSubsciptionData))
            {
                string[] data = line.Split(';');

                if (data.Length >= 5 && data[0] == "Transaction")
                {
                    ListExpenses.Add(new Expense(
                        data[1],
                        data[2],
                        DateTime.Parse(data[3]),
                        decimal.Parse(data[4])
                    ));
                }
                else if (data.Length == 6 && data[0] == "Subscription")
                {
                    ListSubscription.Add(new Subscription(
                        data[1],
                        data[2],
                        DateTime.Parse(data[3]),
                        decimal.Parse(data[4]),
                        bool.Parse(data[5])
                    ));
                }
            }
        }

        private void AddTransaction(string name, string description, DateTime date, decimal price)
        {
            Expense exp = new Expense(name, description, date, price);
            ListExpenses.Add(exp);

            bal -= price; // 🔥 aggiorna saldo

            File.AppendAllText(PathSubsciptionData,
                $"Transaction;{exp.Name};{exp.Description};{exp.Date};{exp.Price}\n");

            SaveUserData();
        }

        private void AddSubscription(string name, string description, DateTime date, decimal price, bool payed)
        {
            Subscription sub = new Subscription(name, description, date, price, payed);
            ListSubscription.Add(sub);

            File.AppendAllText(PathSubsciptionData,
                $"Subscription;{sub.Name};{sub.Description};{sub.Date};{sub.Price};{sub.Payed}\n");
        }

        // =========================
        // UI + LOGIN
        // =========================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            name = _txtboxname.Text;
            surname = _txtboxsurname.Text;

            if (!decimal.TryParse(_txtSaldo.Text, out bal))
            {
                MessageBox.Show("Invalid balance.");
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                MessageBox.Show("Insert name and surname.");
                return;
            }

            SaveUserData();

            ShowLogin(false);
            LoadData();
            Task.Run(async () => await BackGround());
        }

        private void ShowLogin(bool show)
        {
            _loginGrid.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            _pageGrid.Visibility = show ? Visibility.Hidden : Visibility.Visible;
        }

        private void UpdateBal()
        {
            _txtboxbalance.Text = $"€ {bal:F2}";
            _txtboxbalance.Foreground = bal < 0 ? Brushes.Red :
                                       bal > 0 ? Brushes.Green :
                                       Brushes.Black;

            _txtSubsription.Text = ListSubscription.Count.ToString();
            _txtNearRenwals.Text = ListNearRenewals.Count.ToString();

            SaveUserData(); // 🔥 sempre sincronizzato
        }

        // =========================
        // BACKGROUND
        // =========================
        private async Task BackGround()
        {
            while (true)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    UpdateBal();
                    CheckRenewals();
                    CheckMonthlySpent();
                    RebuildTransactionList();
                });

                await Task.Delay(1000); // 🔥 evita CPU 100%
            }
        }

        private void RebuildTransactionList()
        {
            ListAllTransaction.Clear();
            ListAllTransaction.AddRange(ListExpenses);
            ListAllTransaction.AddRange(ListSubscription);
        }

        private void CheckMonthlySpent()
        {
            DateTime today = DateTime.Today;

            monthlySpent = 0;

            foreach (var sub in ListSubscription)
            {
                if (sub.Date.Month == today.Month && sub.Date.Year == today.Year)
                {
                    MonthTransaction.Add(sub);
                    monthlySpent += sub.Price;
                }
            }

            foreach (var exp in ListExpenses)
            {
                if (exp.Date.Month == today.Month && exp.Date.Year == today.Year)
                {
                    MonthTransaction.Add(exp);
                    monthlySpent += exp.Price;
                }
            }

            _txtMonthTransaction.Text = $"-€ {monthlySpent:F2}";
            _txtMonthTransaction.Foreground = Brushes.Red;

        }

        private void CheckRenewals()
        {
            ListNearRenewals.Clear();
            DateTime today = DateTime.Today;

            foreach (var sub in ListSubscription)
            {
                double days = (sub.Date - today).TotalDays;

                if (days >= 0 && days <= 14)
                {
                    if (sub.Date == today)
                    {
                        bal -= sub.Price;
                        sub.Date = sub.Date.AddMonths(1);

                        SaveUserData();
                    }

                    ListNearRenewals.Add(sub);
                }
            }
        }

        private void BtnSeeMore_Click(object sender, RoutedEventArgs e)
        {
            _expensesBox.Visibility = Visibility.Visible;
            Button btn = sender as Button;

            switch (btn.Name)
            {
                case "_btnExpences":
                    _itemsList.ItemsSource = MonthTransaction.OrderByDescending(x => x.Date);
                    break;

                case "_btnRenewal":
                    _itemsList.ItemsSource = ListNearRenewals.OrderBy(x => x.Date);
                    break;

                case "_btnSubscription":
                    _itemsList.ItemsSource = ListSubscription;
                    break;
            }

            _viewMoreGrid.Visibility = Visibility.Visible;
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            _expensesBox.Visibility = Visibility.Hidden;
            _itemsList.ItemsSource = null;
        }
    }
}