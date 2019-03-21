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
            CurrentInterestRate = currentInterestRate;

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
            int principal = random.Next(minAddDepositSizeMiL, maxAddDepositSizeMil);
            var deposit = new Deposit(million * principal, CurrentInterestRate);
            deposits.Add(deposit);

            var maturity = TotalMaturityAmount();
            if (!(OnMaturityUpdateEvent is null))
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
                // TODO: The spec doesn't state what deposit we have to sell, I'm assumig the last one
                count--;
                deposits.RemoveAt(count);   

                maturity = TotalMaturityAmount();
                if (!(OnMaturityUpdateEvent is null))
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
            while (deposits.Count() < 50)
            {
                if (Buy() > maxInitialTotalMaturity)
                {
                    Sell();
                    break;
                }
            }
        }

        public ObservableCollection<Deposit> Deposits
        {
            get { return deposits; }
        }

        public double CurrentInterestRate { get; set; }

        private const double currentInterestRate = 0.04;
        private const double million = 1000000.0;
        private const double minTotalMaturity = million * 50;
        private const double maxTotalMaturity = million * 120;
        private const double maxInitialTotalMaturity = million * 100;
        private const int timerIntervalMs = 5000;
        private const int minAddDepositSizeMiL = 3; // inclufing
        private const int maxAddDepositSizeMil = 6; // excluding

        private int action = 0;
        private ObservableCollection<Deposit> deposits = new ObservableCollection<Deposit>();
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private Random random = new Random();

        public event OnMaturityUpdate OnMaturityUpdateEvent;
        public event OnActionUpdate OnActionUpdateEvent;
    }
}
