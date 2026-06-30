using BagAndShop.Informations;
using BagAndShop.ItemSystem;
using BagAndShop.Manager;
using SimpleSQLiteORM;
using System.Reflection;


namespace BagAndShop.BagSystem.Template
{
    public class Slot : SlotBase, IEntity<Slot, SlotData>
    {
        public ItemBase Item { get; set; }
        public int Count { get; private set; }
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
            else throw new Exception("The object" + item + "is not an item");
        }

        #region 操作逻辑
        public async Task LoadFromDB(SqliteDataBase db)
        {
            Type itemType = Item.GetType();
            var item = await db.GetByKeyAsync(itemType, Item.ID);
            if (item is ItemBase)
            {
                Item = (ItemBase)item;
            }
            else
            {
                //TODO: throw a Information
            }
        }
        public SlotInformation AddCount(int add)
        {
            // 1.计算空余容量
            int remain = Item.MaxStack - Count;
            // 2.判断是否可以加入
            if (add > remain)
            {
                // 3.是则加入传回正确的information，否则传回失败Information
                return SlotInformation.GetAddFailed(this, this.Item, add);
            }
            else
            {
                Count += add;
                return SlotInformation.GetAddCompleted(this, this.Item);
            }
        }
        public SlotInformation TryToAddCount(int add)
        {
            // 1.计算空余数量
            int remain = Item.MaxStack - Count;
            // 2.判断是否可以完全加入
            if (add > remain)
            {
                // 3.是则加入，否则加入空余数量，并传回一个失败Information
                Count = Item.MaxStack;
                int remaining = add - remain;
                return SlotInformation.GetAddFailed(this, this.Item, remaining);
            }
            else
            {
                Count += add;
                return SlotInformation.GetAddCompleted(this, this.Item);
            }
        }
        public SlotInformation RemoveCount(int rmv)
        {
            // 1.判断count - rmv >= 0?
            if (Count - rmv >= 0)
            {
                // 2.是则去除，否则传回失败Information
                Count -= rmv;
                return SlotInformation.GetRemoveCompleted(this, this.Item);
            }
            else
            {
                return SlotInformation.GetRemoveFailed(this, this.Item, rmv);
            }
        }
        public SlotInformation TryToRemoveCount(int rmv)
        {
            // 1.判断count - rmv >= 0?
            if (Count - rmv >= 0)
            {
                // 2.是则去除，否则归零，并传回失败Information
                Count -= rmv;
                return SlotInformation.GetRemoveCompleted(this, this.Item);
            }
            else
            {
                int remaining = rmv - Count;
                ItemBase it = Item;
                ToEmpty();
                return SlotInformation.GetRemoveFailed(this, it, remaining);
            }
        }

        private void ToEmpty()
        {
            Count = 0;
            Item = new ItemBase(1001, "empty", 0, 0, 0);
        }
        #endregion

        public SlotData ToData()
        {
            Type t = Item.GetType();
            return new SlotData(this.BagID, this.Index, t.FullName ?? throw new Exception($"The Item of Slot:({BagID}, {Index}) has a \"null\" type"), Item.ID, Count);
        }
    }
    [DbTable("Slots")]
    public class SlotData : IData<Slot>
    {
        [DbKey]
        public int BagID { get; set; } = -1;
        [DbKey]
        public int Index { get; set; } = -1;
        [DbColumn]
        public string ItemType { get; set; }
        [DbColumn]
        public int ItemID { get; set; }
        [DbColumn]
        public int Count { get; set; }
        public bool IsComplete => (BagID != -1 & Index != -1);
        public SlotData(int bid, int idx, string typeStr, int iid, int count)
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
