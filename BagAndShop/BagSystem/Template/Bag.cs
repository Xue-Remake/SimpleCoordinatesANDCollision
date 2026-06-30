using BagAndShop.Manager;
using SimpleSQLiteORM;

namespace BagAndShop.BagSystem.Template
{
    public class SlotBag : BagBase, IEntity<SlotBag, SlotBagData>
    {
        private List<Slot> slots;
        private int MaxSlotCount;
        public SlotBag(int id, string name, List<Slot> slots, int maxSlotCount) : base(id, name)
        {
            this.slots = slots;
            MaxSlotCount = maxSlotCount;
        }
        public SlotBag(int id, string name, int maxSlotCount) : base(id, name)
        {
            MaxSlotCount = maxSlotCount;
            slots = new();
        }

        public SlotBagData ToData()
        {
            return new SlotBagData(ID, Name, MaxSlotCount);
        }
    }
    [DbTable("SlotBag")]
    public class SlotBagData : IData<SlotBag>
    {
        [DbKey]
        public int BagID { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public int MaxSlotCount { get; set; }
        public SlotBagData()
        {
            BagID = -101;
            Name = "empty";
        }
        public SlotBagData(int bagID, string name, int maxSlotCount)
        {
            BagID = bagID;
            Name = name;
            MaxSlotCount = maxSlotCount;
        }

        public SlotBag ToEntity()
        {
            return new SlotBag(BagID, Name, MaxSlotCount);
        }
    }
}