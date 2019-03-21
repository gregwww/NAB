using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public delegate void OnMaturityUpdate(double val);
    public delegate void OnActionUpdate(int val);

    class Controller
    {
        public Controller()
        {
            CurrentInterestRate = initialInterestRate;

            timer.Enabled = false;
            timer.Interval = timerIntervalMs;
            timer.Tick += (src, args) =>
            {
                ProcessDeposit();
            };
            timer.Start();
        }

        public int CurrentAction
        {
            get { return action; }

            set
            {
                if ((value == action) || (value < 0) || (value >= ActionName.Count()))
                {
                    return;
                }
                action = value;
                if (!(OnActionUpdateEvent is null))
                {
                    OnActionUpdateEvent(action);
                }
                ProcessDeposit();
            }
        }

        public string[] ActionName { get; } = { "Hold", "Buy", "Sell" };

        private void ProcessDeposit()
        {
            switch (action)
            {
                case 0:
                    timer.Enabled = false;
                    break;
                case 1:
                    Buy();
                    if (!timer.Enabled)
                    {
                        timer.Enabled = true;
                    }
                    break;
                case 2:
                    Sell();
                    if (!timer.Enabled)
                    {
                        timer.Enabled = true;
                    }
                    break;
                default:
                    throw new Exception("Invalid 'action' value: " + action.ToString());
            }
        }

        private double Buy()
        {
            // TODO: The spec doesn't state what's the rounding for the deposit size should be:
            // a dollar, thousand ones, hudred grand or a million? I'm assuming the last one.
            //
            int principal = random.Next(minAddDepositSizeMiL, maxAddDepositSizeMil);
            var deposit = new Deposit(million * principal, CurrentInterestRate);
            deposits.Add(deposit);

            var maturity = TotalMaturityAmount();
            if (OnMaturityUpdateEvent != null)
            {
                OnMaturityUpdateEvent(maturity);
            }

            if (maturity >= maxTotalMaturity)
            {
                CurrentAction = 0;
            }
            return maturity;
        }

        private double Sell()
        {
            double maturity = 0;
            int count = deposits.Count;
            if (count == 0)
            {
                CurrentAction = 0;
            }
            else if (count > 0)
            {
                // TODO: The spec doesn't state which deposit we have to sell, I'm assumig the last added one.
                //
                count--;
                deposits.RemoveAt(count);   

                maturity = TotalMaturityAmount();
                if (OnMaturityUpdateEvent != null)
                {
                    OnMaturityUpdateEvent(maturity);
                }

                if ((count == 0) || (maturity <= minTotalMaturity))
                {
                    CurrentAction = 0;
                }
            }
            return maturity;
        }

        public double TotalMaturityAmount()
        {
            double sum = 0;
            foreach (var deposit in deposits)
            {
                sum += deposit.MaturityAmount;
            }
            return sum;
        }

        public void PrePopulateDeposits()
        {
            double maturity = 0;
            while (deposits.Count < 50)
            {
                // We have to generate 50 deposits with total Maturity Amounts of all loans
                // between 70 Million and 100 Million. Rounding deposits principal to Million
                // wouldn't fit in the range and count specified. Thus we rounding here to
                // hundred thousand.
                int principal = random.Next(10, 25);
                var deposit = new Deposit(100000.0 * principal, CurrentInterestRate);
                maturity += deposit.MaturityAmount;
                if (maturity > maxInitialTotalMaturity)
                {
                    maturity -= deposit.MaturityAmount;
                    break;
                }
                deposits.Add(deposit);
            }
            if (OnMaturityUpdateEvent != null)
            {
                OnMaturityUpdateEvent(maturity);
            }
        }

        public ObservableCollection<Deposit> Deposits
        {
            get { return deposits; }
        }

        public double CurrentInterestRate { get; set; }

        private const double initialInterestRate = 0.04;    // 4%
        private const double million = 1000000.0;
        private const double minTotalMaturity = million * 50;
        private const double maxTotalMaturity = million * 120;
        private const double maxInitialTotalMaturity = million * 100;
        private const int timerIntervalMs = 5000;
        private const int minAddDepositSizeMiL = 3;         // including
        private const int maxAddDepositSizeMil = 6;         // excluding

        private int action = 0;
        private ObservableCollection<Deposit> deposits = new ObservableCollection<Deposit>();
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private Random random = new Random();

        public event OnMaturityUpdate OnMaturityUpdateEvent;
        public event OnActionUpdate OnActionUpdateEvent;
    }
}
