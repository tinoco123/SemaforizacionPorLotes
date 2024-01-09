using SemaforizacionPorLotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemaforizacionPorLotes.Repository
{
    public interface ITxnHistoryRepository
    {
        bool InsertTxnHistory(TxnHistory txnHistory);
        bool DeleteHistoryFromInitialDate(string date);
    }
}
