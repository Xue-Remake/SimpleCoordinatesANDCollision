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
    public class CheckInfo
    {
        public bool has;
        public int index;
        public int count;
        public int maxstack;
        public CheckInfo(bool hs, int idx, int ct, int mx)
        {
            has = hs;
            index = idx;
            count = ct;
            maxstack = mx;
        }
    }
    public class Slot
    {
        public IItem Item { get; set; }
        public int Count { get; set; }
        public bool IsEmpty => (Count <= 0 || Item is NullItem);
        public Slot(IItem item, int count)
        {
            Item = item;
            Count = count;
        }
    }
    public abstract class Bag
    {
        protected List<Slot> Slots = new();

        public abstract Task<bool> AddItem<T>(T item, int count) where T : class, IItem;
        public abstract Task<bool> RemoveItem<T>(T item, int count) where T : class, IItem;
        public abstract Task<bool> MoveItem(int index, int targetIndex);
        
        /// <summary>
        /// 查询背包内指定的物品 item 的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        protected async Task<CheckInfo> CheckFromItem<T>(T item, int startIdx) where T : class, IItem
        {
            int idx = Slots.FindIndex(startIdx, x => x.Item == item);
            if (idx == -1) return new(false, -1, 0, 0);
            else
                return new(!(Slots[idx].IsEmpty), idx, Slots[idx].Count, Slots[idx].Item.GetMaxStack());
        }

    }

    public class SlotBag : Bag
    {
        public int SlotCount { get; set; }

        public override async Task<bool> AddItem<T>(T item, int count)
        {
            bool ATExist = false;
            bool ATEmpty = false;
            try
            {
                ATExist = await AddItemToExistSlot<T>(item, count, 0);
            }
            catch (BagException ex)
            {
                try { ATEmpty = await AddItemToEmptySlot<T>(item, ex.RemainCount, 0); }
                catch (BagException ex2)
                {
                    throw new BagException($"The task is not complete; there are {ex2.RemainCount} items left to add.", ex2.RemainCount);
                }
            }
            return ATEmpty || ATExist;
        }
        public override async Task<bool> RemoveItem<T>(T item, int count)
        {
            List<CheckInfo> infos = new();
            try
            {
                int infoIndex = 0;
                int TMPidx = 0;
                while (true)
                {
                    infos.Add(await CheckFromItem<T>(item, TMPidx));
                    if (!infos[infoIndex].has) break;
                    TMPidx = infos[infoIndex].index;
                    infoIndex++;
                }
            }
            catch (Exception)
            {
                throw new RemoveException("An unknown exception occurred whilst executing RemoveItem()");
            }
            int sum = infos.Sum(x => x.count);
            if (count <= sum)
            {
                var addcount = sum - count;
                foreach (var info in infos)
                {
                    await ClearSlots(info.index);
                }
                return await AddItem<T>(item, addcount);
            }
            else
            {
                throw new RemoveException("The RemoveItem() method failed: there are not enough items", count, sum);
            }
        }
        public async override Task<bool> MoveItem(int index, int targetIndex)
        {
            Slot tmp = Slots[targetIndex];
            Slots[targetIndex] = Slots[index];
            Slots[index] = tmp;
            return true;
        }

        #region 辅助方法
        private async Task<bool> AddItemToExistSlot<T>(T item, int count, int startIndex) where T : class, IItem
        {
            CheckInfo info = await CheckFromItem<T>(item, startIndex);
            if (!info.has) throw new BagException("No slot containing the item was found", count);

            else if (info.count + count <= info.maxstack)
            {
                Slots[info.index].Count = info.count + count;
                return true;
            }
            else
            {
                Slots[info.index].Count = info.maxstack;
                return await AddItemToExistSlot<T>(item, count - (info.maxstack - info.count), info.index + 1);
            }
        }
        private async Task<bool> AddItemToEmptySlot<T>(T item, int count, int startIndex) where T:class, IItem
        {
            int index = Slots.FindIndex(startIndex, x => x.IsEmpty);
            if (index == -1)
                throw new BagException("No more empty slot", count);

            if (item.GetMaxStack() >= count)
            {
                Slots[index].Item = item;
                Slots[index].Count = count;
                return true;
            }
            else
            {
                Slots[index].Item = item;
                Slots[index].Count = item.GetMaxStack();
                return await AddItemToEmptySlot<T>(item, count - item.GetMaxStack(), index + 1);
            }
        }
        private async Task ClearSlots(int index)
        {
            Slots[index].Item = new NullItem();
            Slots[index].Count = 0;
        }
        #endregion
    }

    public class WeightBag : Bag
    {
        public Double MaxWeight { get; set; }
        public Double TotalWeight { get; set; }
        public override async Task<bool> AddItem<T>(T item, int count)
        {
            double weight = item.GetWeight();
            if (weight * count + TotalWeight <= MaxWeight)
            {
                CheckInfo info = await CheckFromItem<T>(item, 0);
                if (info.has)
                {
                    Slots[info.index].Count += count;
                    UpdateTotalWeight(item.GetWeight(), info.count, info.count + count);
                    return true;
                }
                else
                {
                    Slots.Add(new Slot(item, count));
                    UpdateTotalWeight(item.GetWeight(), 0, count);
                    return true;
                }
            }
            else
            {
                int addcount = ((int)((MaxWeight - TotalWeight) / weight));
                CheckInfo info = await CheckFromItem<T>(item, 0);
                if (info.has)
                {
                    Slots[info.index].Count += addcount;
                    UpdateTotalWeight(item.GetWeight(), info.count, info.count + addcount);
                    throw new BagException();//TODO: 完成message和传出剩余的未添加的数量
                }
                else
                {
                    Slots.Add(new Slot(item, addcount));
                    UpdateTotalWeight(item.GetWeight(), 0, addcount);
                    throw new BagException();//TODO: 完成message和传出剩余的未添加的数量
                }
            }
        }
        public override async Task<bool> RemoveItem<T>(T item, int count)
        {
            CheckInfo info = await CheckFromItem<T>(item, 0);
            if (!info.has) return false;
            else if (info.count < count)
                throw new BagException(); //TODO: message，以及待处理的数量
            else
            {
                if (info.count == count) { Slots.RemoveAt(info.index); return true; }
                else
                {
                    Slots[info.index].Count -= count;
                    UpdateTotalWeight(item.GetWeight(), info.count, info.count - count);
                    return true;
                }
            }
        }
        public override async Task<bool> MoveItem(int index, int targetIndex)
        {
            Slot tmp = Slots[targetIndex];
            Slots[targetIndex] = Slots[index];
            Slots[index] = tmp;
            return true;
        }

        private async Task UpdateTotalWeight<T>(T lastItem, int lastCount, T newItem, int newCount) where T : class, IItem
        {
            TotalWeight -= (lastItem.GetWeight()) * (lastCount);
            TotalWeight += (newItem.GetWeight()) * newCount;
        }
        private void UpdateTotalWeight(double weight, int lastCount, int newCount)
        {
            double data = weight * (newCount - lastCount);
            if (data < -TotalWeight)
            {
                throw new Exception("The updated weight is less than the total weight; please check the code logic.");
            }
            TotalWeight += data;
        }
    }
}
