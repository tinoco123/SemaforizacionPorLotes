using SemaforoPorLotes.Models;
namespace SemaforoPorLotes.Repository
{
    public interface ITxnRepository
    {
        bool GetTxnId(string txnId);
        bool InsertTxn(string txnId);
    }
}
