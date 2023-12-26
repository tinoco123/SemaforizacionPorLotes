namespace SemaforoPorLotes.Models
{
    public class Txn
    {        
        public string TxnId { get; set; }
        public string LotNumber { get; set; }

        public Txn(string txnId, string lotNumber)
        {
            TxnId = txnId;
            LotNumber = lotNumber;
        }

        public Txn()
        {
        }
    }
}
