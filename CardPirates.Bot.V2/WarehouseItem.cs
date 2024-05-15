using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardPirates.Bot.V2
{
    internal class WarehouseItem
    {
        public WarehouseItem(List<string> value, string status, int index)
        {
            Index = index;
            Place = value[0];
            Project = value[1];
            User = value[2];
            TGContact = value[3];
            Unit = value[4];
            if (value?.Count == 6) {
                Count = value[5];
            }
            Status = status.Replace("=","").Trim();
        }
        public int Index { get; set; }
        public string Place { get; set; }
        public string Project { get; set; }
        public string User { get; set; }
        public string TGContact { get; set; }
        public string Unit { get; set; }
        public string? Count { get; set; }
        public string Status { get; set; }

    }
}
