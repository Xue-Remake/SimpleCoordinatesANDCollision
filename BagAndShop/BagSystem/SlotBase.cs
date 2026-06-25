namespace BagAndShop.BagSystem
{
    public abstract class SlotBase
    {
        public int BagID { get; set; }
        public int Index { get; set; }

        public SlotBase(int bagID, int index)
        {
            BagID = bagID;
            Index = index;
        }
        public SlotBase()
        {
            BagID = -1;
            Index = -1;
        }
    }
}
