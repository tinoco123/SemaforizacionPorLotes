namespace SemaforoPorLotes.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }

        public Item(int id, string itemName, string itemDesc)
        {
            Id = id;
            ItemName = itemName;
            ItemDesc = itemDesc;
        }

        public Item(string itemName, string itemDesc)
        {
            ItemName = itemName;
            ItemDesc = itemDesc;
        }

        public Item(int id, string itemName)
        {
            Id = id;
            ItemName = itemName;
        }
    }
}
