using SqliteCrudLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Bag
{
    public enum BagType
    {
        SlotBag,
        WeightBag
    }
    [DbTable("Bag")]
    public class BagInfo
    {
        [DbKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public string type { get; set; }
        public int SlotCount { get; set; }
        public int MaxSlotCount { get; set; }

        public BagInfo(int id, string name, BagType type, int slotCount, int maxSlotCount)
        {
            ID = id;
            Name = name;
            switch (type)
            {
                case BagType.SlotBag: this.type = "SlotBag";break;
                case BagType.WeightBag: this.type = "WeightBag";break;
                default: throw new Exception("The backpack type does not exist");
            }
            SlotCount = slotCount;
            MaxSlotCount = maxSlotCount;
        }
    }
    [DbTable("Slot")]
    public class SlotInfo
    {
        [DbKey]
        public int SlotId { get; set; }
        [DbColumn("ID")]
        public int BagID { get; set; }
        public int BagIndex { get; set; }
        public int ItemID { get; set; }
        public int Count { get; set; }
        public SlotInfo(int bagID, int bagIndex, int itemID, int count)
        {
            BagID = bagID;
            BagIndex = bagIndex;
            ItemID = itemID;
            Count = count;
        }

        private static int CombineNumbers(int first, int second)
        {
            if (second == 0)
                return first * 10; // 0 的位数视为 1，否则会丢失一位
            int digits = (int)Math.Floor(Math.Log10(Math.Abs(second)) + 1);
            return first * (int)Math.Pow(10, digits) + second;
        }
    }
}
