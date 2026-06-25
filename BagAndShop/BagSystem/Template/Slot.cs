using BagAndShop.ItemSystem;
using BagAndShop.ItemSystem.Template;
using BagAndShop.Manager;
using SimpleSQLiteORM;
using System.Reflection;


namespace BagAndShop.BagSystem.Template
{
    public class Slot : SlotBase, IEntity<Slot, SlotInfo>
    {
        public ItemBase Item { get; set; }
        public int Count { get; set; }
        public bool IsEmpty => (Count <= 0 || Item == Item.NullItem);
        public Slot(int bagId, int index, ItemBase item, int count) : base(bagId, index)
        {
            Item = item;
            Count = count;
        }
        public Slot(int bagId, int index, object item, int count) : base(bagId, index)
        {
            if (item is ItemBase)
            {
                Item = (ItemBase)item;
                Count = count;
            }
        }
        public SlotInfo ToInfo()
        {
            Type t = Item.GetType();
            return new SlotInfo(this.BagID, this.Index, t.FullName ?? throw new Exception($"The Item of Slot:({BagID}, {Index}) has a \"null\" type"), Item.ID, Count);
        }
    }

    public class SlotInfo : IInfo<Slot>
    {
        public int BagID { get; set; } = -1;
        public int Index { get; set; } = -1;
        public string ItemType { get; set; }
        public int ItemID { get; set; }
        public int Count { get; set; }
        public bool IsComplete => (BagID != -1 & Index != -1);
        public SlotInfo(int bid, int idx, string typeStr, int iid, int count)
        {
            BagID = bid;
            Index = idx;
            ItemType = typeStr;
            ItemID = iid;
            Count = count;
        }
        public Slot ToEntity()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            object item = ReflectionFactory.CreateInstanceByType(currentAssembly, ItemType, ItemID);
            return new Slot(BagID, Index, item, Count);
        }
    }
}
