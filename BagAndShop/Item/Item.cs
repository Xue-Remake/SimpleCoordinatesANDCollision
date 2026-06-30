using BagAndShop.Informations;
using BagAndShop.Manager;
using SimpleSQLiteORM;

namespace BagAndShop.ItemSystem.Template
{
    public class Item : ItemBase, IEntity<Item, ItemData>
    {
        public new Item NullItem { get; } = new Item(-1001, "empty", 0, 0, 0, "null", -1, CategoryTag.Empty, StatusTag.Empty);
        public string Description { get; private set; }
        public int Rarity { get; private set; } = -1;
        public CategoryTag Category { get; set; } = CategoryTag.Empty;
        public StatusTag Status { get; set; } = StatusTag.Empty;

        public Item(int id, string name, int price, int maxStack, double weight, string description, int rarity, CategoryTag category, StatusTag status) : base(id, name, price, maxStack, weight)
        {
            Description = description;
            Rarity = rarity;
            Category = category;
            Status = status;
        }
        public Item() : base()
        {
            Description = "null";
        }
        public Item(int id) : base()
        {
            ID = id;
            Description = "null";
        }
        
        public ItemData ToData()
        {
            return new ItemData(ID, Name, Price, Weight, MaxStack, Rarity, Category.ToString(), Status.ToString(), Description);
        }
        public async Task<LoadInformation> LoadFrom(SqliteDataBase db)
        {
            var data = await db.GetByKeyAsync<ItemData>(this.ID);
            if (data == null)
            {
                return LoadInformation.GetFaild(db, this);
            }
            else
            {
                var item = data.ToEntity();
                if (item is Item && item != null)
                {
                    this.Name = item.Name;
                    this.Price = item.Price;
                    this.MaxStack = item.MaxStack;
                    this.Weight = item.Weight;
                    this.Description = item.Description;
                    this.Rarity = item.Rarity;
                    this.Category = item.Category;
                    this.Status = item.Status;

                    return LoadInformation.GetCompleted(db, this);
                }
                else
                {
                    return LoadInformation.GetErrorInformation();
                }
            }
        }
    }
    [DbTable("Items")]
    public class ItemData : IData<Item>
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
            return new Item(ID, Name, Price, MaxStack, Weight, Description, Rarity, TagConverter.StringToCategory(Category), TagConverter.StringToStatus(Status));
        }

        public ItemData(int id, string name, int price, double weight, int maxStack, int rarity, string category, string status, string description)
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
        public ItemData()
        { }
    }
}
