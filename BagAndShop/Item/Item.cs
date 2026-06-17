using BagAndShop.Manager;
using SimpleSQLiteORM;

namespace BagAndShop.ItemTemplate
{
    public class Item : ItemBase, IEntity<Item,ItemInfo>
    {
        private static readonly Item NullItem = new Item(-1001, "empty", 0, 0, 0, "null", -1, CategoryTag.Empty, StatusTag.Empty);
        public string Description { get; private set; }
        public int Rarity { get; private set; } = -1;
        public CategoryTag Category { get; set; } = CategoryTag.Empty;
        public StatusTag Status { get; set; } = StatusTag.Empty;

        public ItemInfo Information { get => this.ToInfo(); }

        public Item(int id, string name, int price, int maxStack, double weight, string description, int rarity, CategoryTag category, StatusTag status) : base(id, name, price, maxStack, weight)
        {
            Description = description;
            Rarity = rarity;
            Category = category;
            Status = status;
        }
        public Item()
        {
            Description = "null";
        }
        public Item GetNullItem() => NullItem.CopyTo();
        private Item CopyTo()
        {
            return new Item(this.ID, this.Name, this.Price, this.MaxStack, this.Weight, this.Description, this.Rarity, this.Category, this.Status);
        }
        public ItemInfo ToInfo()
        {
            return new ItemInfo(ID, Name, Price, Weight, MaxStack, Rarity, Category.ToString(), Status.ToString(), Description);
        }
    }
    [DbTable("Items")]
    public class ItemInfo : IInfo<Item>
    {
        [DbKey]
        public int ID { get; set; } = -1001;
        [DbColumn]
        public string Name { get; set; } = "empty";
        [DbColumn]
        public int Price { get; set; }
        [DbColumn]
        public Double Weight { get; set; }
        [DbColumn]
        public int MaxStack { get; set; }
        [DbColumn]
        public int Rarity { get; set; }
        [DbColumn]
        public string Category { get; set; } = CategoryTag.Empty.ToString();
        [DbColumn]
        public string Status { get; set; } = StatusTag.Empty.ToString();
        [DbColumn]
        public string Description { get; set; } = "null";

        public Item ToEntity()
        {
            return new Item(ID, Name, Price, MaxStack, Weight, Description, Rarity,TagConverter.StringToCategory(Category), TagConverter.StringToStatus(Status));
        }

        public ItemInfo(int id, string name, int price, double weight, int maxStack, int rarity, string category, string status, string description)
        {
            ID = id;
            Name = name;
            Price = price;
            Weight = weight;
            MaxStack = maxStack;
            Rarity = rarity;
            Category = category;
            Status = status;
            Description = description;
        }
        public ItemInfo()
        { }
    }
}
