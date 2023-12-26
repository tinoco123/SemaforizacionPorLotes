using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemaforizacionPorLotes.Models
{
    public class InventoryAdjustmentLineRet
    {
        public string ItemName { get; set; }
        public string LotNumber { get; set; }

        public InventoryAdjustmentLineRet(string itemName, string lotNumber)
        {
            ItemName = itemName;
            LotNumber = lotNumber;
        }
    }
}
