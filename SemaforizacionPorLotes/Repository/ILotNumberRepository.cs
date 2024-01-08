using SemaforoPorLotes.Models;

namespace SemaforoPorLotes.Repository
{
    public interface ILotNumberRepository
    {
        int GetLotNumberId(int itemId, string lotNumber);
        bool SaveLotNumber(LotNumber lotNumber);
    }
}
