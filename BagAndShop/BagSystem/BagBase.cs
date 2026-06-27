namespace BagAndShop.BagSystem
{
    public abstract class BagBase
    {
        public int ID { get; protected set; }
        public string Name { get; protected set; }

        public BagBase(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
