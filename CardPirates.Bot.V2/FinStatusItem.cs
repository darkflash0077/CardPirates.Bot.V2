using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class FinStatusItem
    {
        public FinStatusItem(List<string> x, int index)
        {
            Index = index;
            UserID = Convert.ToInt32(x[0].Replace(",", ""));
            BankID = Convert.ToSingle(x[1]);
            TGContact = x[2];
            Name = x[3];
            PositiveBalance = Convert.ToDecimal(x[4].Replace(",", ""));
            PayAmount = Convert.ToDecimal(x[6].Replace(",",""));
        }
        public int Index { get; set; }
        public int UserID { get; set; }
        public float BankID { get; set; }
        public string TGContact { get; set; }
        public string Name { get; set; }
        public decimal PositiveBalance { get; set; }
        public decimal PayAmount { get; set; }
    }
}
