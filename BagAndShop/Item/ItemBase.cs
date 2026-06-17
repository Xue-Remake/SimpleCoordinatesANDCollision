namespace BagAndShop
{
    public abstract class ItemBase
    {
        public string Name { get; protected set; }
        public int ID { get; protected set; }
        public int Price { get; protected set; }
        public int MaxStack { get; protected set; }
        public Double Weight { get; protected set; }

        public ItemBase(int id, string name, int price, int maxStack, double weight)
        {
            Name = name;
            ID = id;
            Price = price;
            MaxStack = maxStack;
            Weight = weight;
        }
        public ItemBase()
        {
            ID = -1001;
            Name = "empty";
        }
    }
}
