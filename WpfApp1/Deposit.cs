using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Deposit
    {
        public Deposit(double principal, double interest, int term = 1)
        {
            if ((principal <= 0) || (term <= 0) || (interest <= 0))
            {
                throw new ArgumentOutOfRangeException("Invalid argument: value is too low");
            }
            Principal = principal;
            Term = term;
            InterestRate = interest;
            Start = DateTime.Now;
        }

        public double Principal { get; }
        public DateTime Start { get; }
        public DateTime End { get { return Start.AddYears(Term); } }
        public double InterestRate { get; }
        public int Term { get; }
        public double MaturityAmount
        {
            get
            {
                return Principal + Principal * InterestRate * Term; // TODO: is this a correct calculation?
            }
        }
    }
}
