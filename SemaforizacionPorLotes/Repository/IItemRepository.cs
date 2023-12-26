using SemaforoPorLotes.Models;

namespace SemaforoPorLotes.Repository
{
    public interface IItemRepository
    {
        int GetItemId(string itemName);
        bool InsertItem(Item item);
    }
}
