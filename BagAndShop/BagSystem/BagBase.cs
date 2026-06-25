namespace BagAndShop.BagSystem
{
    public abstract class BagBase
    {
        public int ID { get; protected set; }
        public string Name { get; protected set; }

        public BagBase(int iD, string name)
        {
            ID = iD;
            Name = name;
        }
        protected List<SlotBase> Slots = new();
    }
}
