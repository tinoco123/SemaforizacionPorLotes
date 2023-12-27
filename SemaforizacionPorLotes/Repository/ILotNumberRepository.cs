using SemaforoPorLotes.Models;

namespace SemaforoPorLotes.Repository
{
    public interface ILotNumberRepository
    {
        int GetLotNumberQuantity(int lotNumberId);
        bool UpdateLotNumberQuantity(int lotNumberId, int quantity, string date);
        int GetLotNumberId(int itemId, string lotNumber);
        bool SaveLotNumber(LotNumber lotNumber);
        bool DeleteDataFromInitialDate(string initialDate);
    }
}
