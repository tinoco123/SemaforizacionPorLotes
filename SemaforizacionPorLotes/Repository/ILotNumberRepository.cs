using SemaforoPorLotes.Models;

namespace SemaforoPorLotes.Repository
{
    public interface ILotNumberRepository
    {
        int GetLotNumberQuantity(int lotNumberId);
        bool UpdateLotNumberQuantity(int lotNumberId, int quantity);
        int GetLotNumberId(int itemId, string lotNumber);
        bool SaveLotNumber(LotNumber lotNumber);        
    }
}
