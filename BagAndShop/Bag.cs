using BagAndShop.Item;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop
{
    public abstract class Bag
    {
        protected struct CheckInfo
        {
            bool has;
            int index;
            int count;
            int maxstack;
            public CheckInfo(bool hs, int idx, int ct, int mx)
            {
                has = hs;
                index = idx;
                count = ct;
                maxstack = mx;
            }
        }
        protected struct Slot
        {
            public IItem Item { get; set; }
            public int Count { get; set; }
            public bool IsEmpty => Count <= 0 || Item is NullItem;
        }

        protected List<Slot> Slots = new();

        public abstract Task<bool> AddItem(int id, int count);
        public abstract Task<bool> RemoveItem(int id, int count);
        public abstract Task<bool> MoveItem(int index, int targetIndex);
        public abstract Task<IItem> Show(int index);
        protected async Task<CheckInfo> CheckFromId(int id)
        {
            int idx = Slots.FindIndex(x => x.Item.GetId() == id);
            return new((idx != -1) && !(Slots[idx].IsEmpty), idx, Slots[idx].Count, Slots[idx].Item.GetMaxStack());
        }
    }

    public class SlotBag : Bag
    {
        public int SlotCount { get; set; }
        
        public async override Task<bool> AddItem(int id, int count)
        {
            //1. 检查是否存在该格
            //2. 检查count+Slot.Count < MaxCount?
            try
            {
                CheckInfo info = await CheckFromId(id);
                
            }
        }

        private bool hasNullSlot()
        {
            int idx = Slots.FindIndex(x => x.IsEmpty == true);
            if (idx == -1) return false;
            else return true;
        }
    }

    public class WeightBag : Bag
    {
        Double TotalWeight { get; set; }
    }
}
