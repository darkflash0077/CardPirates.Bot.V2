using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class KickOrderItem
    {
        public KickOrderItem(List<string> value, int index)
        {
            Index = index;
            Count = 1;
            Project = value[0];
            Name = value[1];
            TGContact = value[2];
            Item = value[3];
            if (value.Count > 4 && value[4] != String.Empty)
            {
                int.TryParse(value[4], out int cnt);
                Count = Math.Max(cnt, 1);
            }
            if (value.Count > 5 && value[5] != String.Empty)
            {
                decimal.TryParse(value[4], out decimal price);
                Price = price;
            }
        }

        public int Index { get; set; }
        public string Project { get; set; }
        public string Name { get; set; }
        public string TGContact { get; set; }
        public string Item { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
