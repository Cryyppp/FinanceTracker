using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace FinanceTracker
{
    public class ChartBarData
    {
        public string Label { get; set; }
        public double Value { get; set; } // Altezza calcolata
        public string AmountFormatted { get; set; }
        public SolidColorBrush Color { get; set; }
    }
    public partial class MainWindow : Window
    {
        List<Item> ListAllTransaction = new List<Item>();
        List<Income> ListMonthIncome = new List<Income>();
        List<Subscription> ListNearRenewals = new List<Subscription>();
        List<Subscription> ListSubscription = new List<Subscription>();
        List<Expense> ListExpenses = new List<Expense>();
        List<Item> MonthTransaction = new List<Item>();
        List<Income> ListStipendio = new List<Income>();

        decimal monthlySpent = 0.00M;
        decimal monthlyIncome = 0.00M;
        decimal bal = 0.00M;
        string name;
        string surname;

        string PathUserData = @"../../../Data/UserData.txt";
        string PathSubsciptionData = @"../../../Data/SubscriptionData.txt";
        string PathActivityData = @"../../../Data/IncomeData.txt";
        string PathStipendioData = @"../../../Data/StipendioData.txt";

        public MainWindow()
        {
            InitializeComponent();

            bool isSubDataEmpty = !File.Exists(PathSubsciptionData) || File.ReadAllLines(PathSubsciptionData).Length == 0;

            if (!File.Exists(PathUserData) || File.ReadAllLines(PathUserData).Length == 0)
            {
                File.WriteAllText(PathUserData, "");
                File.WriteAllText(PathSubsciptionData, "");
                File.WriteAllText(PathActivityData, "");
                File.WriteAllText(PathStipendioData, "");
                ShowLogin(true);
            }
            else
            {
                ShowLogin(false);
                RunTask();
            }

            LoadData();
            LoadTransactions();
            LoadStipendiRicorrenti();

            _expensesBox.Visibility = Visibility.Hidden;
        }

        private void LoadStipendiRicorrenti()
        {
            ListStipendio.Clear();
            if (File.Exists(PathStipendioData))
            {
                foreach (var line in File.ReadAllLines(PathStipendioData))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var data = line.Split(',');
                    if (data.Length >= 5 && bool.TryParse(data[4], out bool recurring) && recurring)
                    {
                        ListStipendio.Add(new Income(
                            data[0],
                            data[1],
                            DateTime.Parse(data[2]),
                            decimal.Parse(data[3]),
                            true
                        ));
                    }
                }
            }
        }
        

        private void UpdateChart()
        {
            // Determina il valore massimo per scalare le barre (altezza max 200px)
            decimal maxVal = Math.Max(monthlyIncome, monthlySpent);
            if (maxVal <= 0) maxVal = 1;

            double scaleFactor = 200.0 / (double)maxVal;

            var bars = new List<ChartBarData>
            {
                new ChartBarData
                {
                    Label = "Entrate",
                    Value = (double)monthlyIncome * scaleFactor,
                    AmountFormatted = $"€ {monthlyIncome:F2}",
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"))
                },
                new ChartBarData
                {
                    Label = "Uscite",
                    Value = (double)monthlySpent * scaleFactor,
                    AmountFormatted = $"€ {monthlySpent:F2}",
                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"))
                }
            };

            _chartContainer.ItemsSource = bars;
        }

        private void SaveStipendiRicorrenti()
        {
            try
            {
                File.WriteAllText(PathStipendioData, "");
                foreach (var stipendio in ListStipendio)
                {
                    File.AppendAllText(PathStipendioData,
                        $"{stipendio.Name},{stipendio.Description},{stipendio.Date},{stipendio.Price},{stipendio.Recurring}\n");
                }
            }
            catch { }
        }

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
                } else return;
            }
            catch
            {
                
            }
        }


        private void LoadTransactions()
        {
            ListSubscription.Clear();
            ListExpenses.Clear();
            ListMonthIncome.Clear();

            if (File.Exists(PathSubsciptionData))
            {
                foreach (string line in File.ReadAllLines(PathSubsciptionData))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
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
                    else if (data.Length >= 6 && data[0] == "Subscription")
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

            if (File.Exists(PathActivityData))
            {
                foreach (string line in File.ReadAllLines(PathActivityData))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
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
                    else if (data.Length >= 5 && data[0] == "Income")
                    {
                        ListMonthIncome.Add(new Income(
                            data[1],
                            data[2],
                            DateTime.Parse(data[3]),
                            decimal.Parse(data[4])
                        ));
                    }
                }
            }

            RebuildTransactionList();
        }

        private void AddIncome(string name, string description, DateTime date, decimal price)
        {
            Income inc = new Income(name, description, date, price);
            ListMonthIncome.Add(inc);

            bal += price;

            File.AppendAllText(PathActivityData,
                $"Income;{inc.Name};{inc.Description};{inc.Date};{inc.Price}\n");

            SaveUserData();
            SaveActivitiesToFile();
        }

        private void AddTransaction(string name, string description, DateTime date, decimal price)
        {
            Expense exp = new Expense(name, description, date, price);
            ListExpenses.Add(exp);

            bal -= price; 

            File.AppendAllText(PathActivityData,
                $"Transaction;{exp.Name};{exp.Description};{exp.Date};{exp.Price}\n");

            SaveUserData();
            SaveActivitiesToFile();
        }

        private void AddSubscription(string name, string description, DateTime date, decimal price, bool payed)
        {
            Subscription sub = new Subscription(name, description, date, price, payed);
            ListSubscription.Add(sub);

            File.AppendAllText(PathSubsciptionData,
                $"Subscription;{sub.Name};{sub.Description};{sub.Date};{sub.Price};{sub.Payed}\n");


            if (payed)
            {
                // registra il pagamento immediatamente come spesa e aggiorna saldo
                bal -= price;
                // aggiungi anche una voce di Expense così viene mostrata nelle spese del mese
                // usa la data odierna per la transazione pagata (il rinnovo può essere in futuro)
                var paidExpense = new Expense(sub.Name, sub.Description, DateTime.Today, sub.Price);
                // evita duplicati
                if (!ListExpenses.Any(e => e.Name == paidExpense.Name && e.Date == paidExpense.Date && e.Price == paidExpense.Price))
                {
                    ListExpenses.Add(paidExpense);
                    // aggiorna il file delle attività (Transaction)
                    File.AppendAllText(PathActivityData,
                        $"Transaction;{paidExpense.Name};{paidExpense.Description};{paidExpense.Date};{paidExpense.Price}\n");
                }
                // salva attività e utente
                SaveActivitiesToFile();
                SaveUserData();
            }
            SaveSubscriptionsToFile();
            // Aggiorna lista e riepiloghi subito
            RebuildTransactionList();
            CheckMonthlySpent();
            // aggiorna UI immediatamente
            _txtMonthTransaction.Text = $"-€ {monthlySpent:F2}";
            _txtMonthTransaction.Foreground = Brushes.Red;
            try { _txtMonthIncome.Text = $"€ {monthlyIncome:F2}"; } catch { }
            UpdateChart();
            UpdateBal();
        }

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
            RunTask();
        }

        private void RunTask()
        {
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


        }

        private async Task BackGround()
        {
            while (true)
            {

                await Dispatcher.InvokeAsync(() =>
                {
                    UpdateBal();
                    CheckRenewals();
                    RebuildTransactionList();
                    CheckStipendio();
                    UpdateChart();
                });


                await CheckMonthlySpentAsync();

                await Task.Delay(1000);
            }
        }

        private void CheckStipendio()
        {
            DateTime today = DateTime.Today;
            bool changed = false;
            foreach (var inc in ListStipendio.ToList())
            {
                // Calcola la data stipendio per il mese corrente
                DateTime stipendioDate = new DateTime(today.Year, today.Month, Math.Min(inc.Date.Day, DateTime.DaysInMonth(today.Year, today.Month)));
                // Se non esiste già una income per questo stipendio in questo mese
                bool alreadyAdded = ListMonthIncome.Any(i => i.Name == inc.Name && i.Date.Month == today.Month && i.Date.Year == today.Year);
                if (!alreadyAdded && today >= stipendioDate)
                {
                    AddIncome(inc.Name, inc.Description, stipendioDate, inc.Price);
                    // Aggiorna la data al mese successivo
                    inc.Date = stipendioDate.AddMonths(1);
                    changed = true;
                }
            }
            if (changed) SaveStipendiRicorrenti();
        }

        private void RebuildTransactionList()
        {
            ListAllTransaction.Clear();
            ListAllTransaction.AddRange(ListExpenses);
            ListAllTransaction.AddRange(ListSubscription);
            ListAllTransaction.AddRange(ListMonthIncome);
            ListAllTransaction.AddRange(ListStipendio);
        }

        private void CheckMonthlySpent()
        {
            DateTime today = DateTime.Today;

            monthlySpent = 0;
            MonthTransaction.Clear();

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
            // compute incomes for month
            monthlyIncome = 0;
            foreach (var inc in ListMonthIncome)
            {
                if (inc.Date.Month == today.Month && inc.Date.Year == today.Year)
                {
                    monthlyIncome += inc.Price;
                }
            }

        }

        private async Task CheckMonthlySpentAsync()
        {
            await Task.Run(() => CheckMonthlySpent());

            // update UI on dispatcher
            await Dispatcher.InvokeAsync(() =>
            {
                _txtMonthTransaction.Text = $"-€ {monthlySpent:F2}";
                _txtMonthTransaction.Foreground = Brushes.Red;
                try
                {
                    _txtMonthIncome.Text = $"€ {monthlyIncome:F2}";
                }
                catch { }

                // aggiorna grafico dopo il calcolo
                try
                {
                    UpdateChart();
                }
                catch { }
            });
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
                        // process renewal payment: deduct, record expense, advance date
                        bal -= sub.Price;

                        var renewalExpense = new Expense(sub.Name, sub.Description, today, sub.Price);
                        if (!ListExpenses.Any(e => e.Name == renewalExpense.Name && e.Date == renewalExpense.Date && e.Price == renewalExpense.Price))
                        {
                            ListExpenses.Add(renewalExpense);
                        }

                        // advance subscription next date
                        sub.Date = sub.Date.AddMonths(1);

                        // persist changes
                        SaveActivitiesToFile();
                        SaveSubscriptionsToFile();
                        SaveUserData();
                    }

                    ListNearRenewals.Add(sub);
                }
            }
        }

        private void SaveSubscriptionsToFile()
        {
            try
            {
                var lines = ListSubscription.Select(s => $"Subscription;{s.Name};{s.Description};{s.Date};{s.Price};{s.Payed}");
                File.WriteAllLines(PathSubsciptionData, lines);
            }
            catch { }
        }

        private void SaveActivitiesToFile()
        {
            try
            {
                var lines = new List<string>();
                lines.AddRange(ListExpenses.Select(e => $"Transaction;{e.Name};{e.Description};{e.Date};{e.Price}"));
                lines.AddRange(ListMonthIncome.Select(i => $"Income;{i.Name};{i.Description};{i.Date};{i.Price}"));
                File.WriteAllLines(PathActivityData, lines);
            }
            catch { }
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
                case "_btnIncome":
                    _itemsList.ItemsSource = ListMonthIncome.OrderByDescending(x => x.Date);
                    break;
            }

            _viewMoreGrid.Visibility = Visibility.Visible;
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            _expensesBox.Visibility = Visibility.Hidden;
            _itemsList.ItemsSource = null;
        }

        private void _btnCloseIncome_Click(object sender, RoutedEventArgs e)
        {
            _addGrid.Visibility = Visibility.Hidden;
            _addSubscription.Visibility = Visibility.Hidden;
            _addIncome.Visibility = Visibility.Hidden;
            _addExpense.Visibility = Visibility.Hidden;
        }
        private void _btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _addGrid.Visibility = Visibility.Visible;
            // show the appropriate panel based on which button was clicked
            if (sender is Button btn)
            {
                _addSubscription.Visibility = Visibility.Collapsed;
                _addIncome.Visibility = Visibility.Collapsed;
                _addExpense.Visibility = Visibility.Collapsed;

                switch (btn.Name)
                {
                    case "_btnaddSubscription":
                    case "_btnAddSubscription":
                        _addSubscription.Visibility = Visibility.Visible;
                        break;
                    case "_btnaddTransaction":
                    case "_btnAddIncome":
                        _addIncome.Visibility = Visibility.Visible;
                        break;
                    case "_btnaddSpent":
                    case "_btnAddExpense":
                        _addExpense.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void _btnSaveIncome_Click(object sender, RoutedEventArgs e)
        {
            string iname = _txtAddIncomeName.Text?.Trim();
            string idesc = _txtAddIncomeDescription.Text?.Trim();
            if (string.IsNullOrEmpty(iname)) { MessageBox.Show("Inserisci il nome dell'entrata."); return; }
            if (!decimal.TryParse(_txtAddIncomeCost.Text, out decimal cost)) { MessageBox.Show("Inserisci un importo valido."); return; }
            DateTime date = _dateAddIncome.SelectedDate ?? DateTime.Today;
            if (_chkIncomeRecurring.IsChecked == true)
            {
                Income inc = new Income(iname, idesc, date, cost, true);
                ListStipendio.Add(inc);
                // persist recurring income using ';' separator
                StreamWriter sw = new StreamWriter(PathStipendioData, true);
                sw.WriteLine($"{inc.Name};{inc.Description};{inc.Date};{inc.Price};{inc.Recurring}");
                sw.Close();

                _addGrid.Visibility = Visibility.Hidden;
                _txtAddIncomeName.Text = _txtAddIncomeDescription.Text = _txtAddIncomeCost.Text = string.Empty;
                _dateAddIncome.SelectedDate = DateTime.Today;
            }
            else
            {
                AddIncome(iname, idesc, date, cost);
                _addGrid.Visibility = Visibility.Hidden;
                _txtAddIncomeName.Text = _txtAddIncomeDescription.Text = _txtAddIncomeCost.Text = string.Empty;
                _dateAddIncome.SelectedDate = DateTime.Today;
            }
            UpdateBal();
        }

        private void _btnSaveExpense_Click(object sender, RoutedEventArgs e)
        {
            string ename = _txtAddExpenseName.Text?.Trim();
            string edesc = _txtAddExpenseDescription.Text?.Trim();
            if (string.IsNullOrEmpty(ename)) { MessageBox.Show("Inserisci il nome della spesa."); return; }
            if (!decimal.TryParse(_txtAddExpenseCost.Text, out decimal cost)) { MessageBox.Show("Inserisci un importo valido."); return; }
            DateTime date = _dateAddExpense.SelectedDate ?? DateTime.Today;
            AddTransaction(ename, edesc, date, cost);
            _addGrid.Visibility = Visibility.Hidden;
            _txtAddExpenseName.Text = _txtAddExpenseDescription.Text = _txtAddExpenseCost.Text = string.Empty;
            _dateAddExpense.SelectedDate = DateTime.Today;
            UpdateBal();
        }

        private void _btnSaveSubscription_Click(object sender, RoutedEventArgs e)
        {
            string sname = _txtAddSubName.Text?.Trim();
            string sdesc = _txtAddSubDescription.Text?.Trim();

            if (string.IsNullOrEmpty(sname))
            {
                MessageBox.Show("Inserisci il nome dell'abbonamento.");
                return;
            }

            if (!decimal.TryParse(_txtAddSubCost.Text, out decimal cost))
            {
                MessageBox.Show("Inserisci un costo valido.");
                return;
            }

            DateTime date = _dateAddSub.SelectedDate ?? DateTime.Today;

            AddSubscription(sname, sdesc, date, cost, false);

            // Aggiorna UI e chiudi
            _addGrid.Visibility = Visibility.Hidden;
            _txtAddSubName.Text = string.Empty;
            _txtAddSubDescription.Text = string.Empty;
            _txtAddSubCost.Text = string.Empty;
            _dateAddSub.SelectedDate = DateTime.Today;

            UpdateBal();
        }
    }
}