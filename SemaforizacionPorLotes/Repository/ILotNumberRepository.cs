using SemaforoPorLotes.Models;

namespace SemaforoPorLotes.Repository
{
    public interface ILotNumberRepository
    {
        int GetLotNumberQuantity(int lotNumberId);
        bool UpdateLotNumberQuantity(int lotNumberId, int quantity);
        bool UpdateLotNumberQuantity(int lotNumberId, int quantity, int initialQuantity);
        int GetLotNumberId(int itemId, string lotNumber);
        bool SaveLotNumber(LotNumber lotNumber);        
    }
}
