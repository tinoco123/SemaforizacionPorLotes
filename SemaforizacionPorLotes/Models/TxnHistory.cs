using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemaforizacionPorLotes.Models
{
    class TxnHistory
    {
        public int LotNumberId { get; set; }
        public int Quantity { get; set; }
        public string Date { get; set; }

        public TxnHistory(int lotNumberId, int quantity, string date)
        {
            LotNumberId = lotNumberId;
            Quantity = quantity;
            Date = date;
        }
    }
}
