using BagAndShop.Item;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Bag
{
    /// <summary>
    /// 用于封装查询背包结果的信息。
    /// </summary>
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
        public static CheckInfo NotFound()
        {
            return new CheckInfo(false, -1, 0, 0);
        }
    }
    /// <summary>
    /// 表示背包中的一个槽位，包含物品及其数量。
    /// </summary>
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
        public Bag(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; protected set; }
        public string Name { get; protected set; }
        protected List<Slot> Slots = new();

        public abstract Task<bool> AddItem<T>(T item, int count) where T : class, IItem;
        public abstract Task<bool> RemoveItem<T>(T item, int count) where T : class, IItem;
        public abstract Task<bool> MoveItem(int index, int targetIndex);

        /// <summary>
        /// 查询背包内指定的物品 item 的结果
        /// </summary>
        /// <typeparam name="T">物品类型。</typeparam>
        /// <param name="item">要查找的物品。</param>
        /// <param name="startIdx">开始搜索的槽位索引。</param>
        /// <returns>包含查询结果的 <see cref="CheckInfo"/> 任务。</returns>
        protected Task<CheckInfo> CheckFromItem<T>(T item, int startIdx) where T : class, IItem
        {
            int idx = Slots.FindIndex(startIdx, x => x.Item.GetId() == item.GetId());
            if (idx == -1) return Task.FromResult(CheckInfo.NotFound());
            else
                return Task.FromResult(new CheckInfo(!(Slots[idx].IsEmpty), idx, Slots[idx].Count, Slots[idx].Item.GetMaxStack()));
        }
    }
    /// <summary>
    /// 基于槽位数量的背包实现。每个槽位可存放一种物品，物品不能跨槽位合并。
    /// </summary>
    public class SlotBag : Bag
    {
        public SlotBag(int id, string name, int slotCount) : base(id, name)
        {
            SlotCount = slotCount;
        }

        public int SlotCount { get; set; }
        /// <summary>
        /// 向基于槽位的背包中添加物品。首先尝试添加到已有相同物品的槽位，若不能完全添加或没有该物品，则尝试使用空槽位。
        /// 如果最终仍有剩余未添加的物品，将抛出 <see cref="BagException"/> 异常。
        /// </summary>
        /// <typeparam name="T">物品类型。</typeparam>
        /// <param name="item">要添加的物品。</param>
        /// <param name="count">要添加的数量。</param>
        /// <returns>如果至少完成了一部分添加操作，返回 true；否则抛出异常。</returns>
        /// <exception cref="BagException">当所有槽位都无法容纳所有剩余物品时抛出，异常的 <c>RemainCount</c> 属性表示剩余未添加的数量。</exception>
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
        /// <summary>
        /// 从基于槽位的背包中移除指定数量的物品。会遍历所有含有该物品的槽位并依次减少数量，如果不足则移除失败并抛出异常。
        /// </summary>
        /// <typeparam name="T">物品类型。</typeparam>
        /// <param name="item">要移除的物品。</param>
        /// <param name="count">要移除的总数量。</param>
        /// <returns>如果移除成功，返回 true。</returns>
        /// <exception cref="RemoveException">当所有同名物品总和少于要移除的数量时抛出，异常中包含剩余需求和当前已有数量。</exception>
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
                int toRemove = count;
                for (int i = 0; i < Slots.Count && toRemove > 0; i++)
                {
                    if (Slots[i].Item.GetId() == item.GetId())
                    {
                        int deduct = Math.Min(toRemove, Slots[i].Count);
                        Slots[i].Count -= deduct;
                        toRemove -= deduct;
                        if (Slots[i].Count <= 0)
                            ClearSlots(i);
                    }
                }
                if (toRemove > 0)
                    throw new RemoveException("Not enough items", toRemove, 0);
                else return true;
            }
            else
            {
                throw new RemoveException("Not enough items", count, sum);
            }
        }
        /// <summary>
        /// 交换两个槽位的物品。
        /// </summary>
        /// <param name="index">第一个槽位索引。</param>
        /// <param name="targetIndex">第二个槽位索引。</param>
        /// <returns>总是返回 true，表示交换完成。</returns>
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
        private async Task<bool> AddItemToEmptySlot<T>(T item, int count, int startIndex) where T : class, IItem
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
        private void ClearSlots(int index)
        {
            Slots[index].Item = new NullItem();
            Slots[index].Count = 0;
        }
        #endregion
    }
    /// <summary>
    /// 基于重量限制的背包实现。物品可以堆叠在同一槽位中，总重量不能超过最大承重。
    /// </summary>
    public class WeightBag : Bag
    {
        public WeightBag(int id, string name, double maxWeight, double totalWeight) : base(id, name)
        {
            MaxWeight = maxWeight;
            TotalWeight = totalWeight;
        }

        public Double MaxWeight { get; set; }
        public Double TotalWeight { get; set; }

        /// <summary>
        /// 向重量背包中添加物品。如果总重量超出限制，则只添加部分并在异常中返回剩余数量。
        /// </summary>
        /// <typeparam name="T">物品类型。</typeparam>
        /// <param name="item">要添加的物品。</param>
        /// <param name="count">希望添加的数量。</param>
        /// <returns>若全部添加成功返回 true；若只添加了部分则抛出 <see cref="BagException"/>。</returns>
        /// <exception cref="BagException">当总重量不足容纳所有物品时抛出，异常包含剩余数量。</exception>
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
                    throw new BagException("Not all items have been added", count - addcount);
                }
                else
                {
                    Slots.Add(new Slot(item, addcount));
                    UpdateTotalWeight(item.GetWeight(), 0, addcount);
                    throw new BagException("Not all items have been added", count - addcount);
                }
            }
        }
        /// <summary>
        /// 从重量背包中移除指定数量的物品。若物品总量不足则抛出异常。
        /// </summary>
        /// <typeparam name="T">物品类型。</typeparam>
        /// <param name="item">要移除的物品。</param>
        /// <param name="count">移除数量。</param>
        /// <returns>移除成功返回 true；若物品数量不足则抛出异常。</returns>
        /// <exception cref="BagException">物品数量不足时抛出。</exception>
        public override async Task<bool> RemoveItem<T>(T item, int count)
        {
            CheckInfo info = await CheckFromItem<T>(item, 0);
            if (!info.has) return false;
            else if (info.count < count)
                throw new BagException("Not enough items", count);
            else
            {
                if (info.count == count)
                {
                    Slots.RemoveAt(info.index);
                    UpdateTotalWeight(item.GetWeight(), info.count, 0);
                    return true;
                }
                else
                {
                    Slots[info.index].Count -= count;
                    UpdateTotalWeight(item.GetWeight(), info.count, info.count - count);
                    return true;
                }
            }
        }
        /// <summary>
        /// 交换两个槽位的物品。
        /// </summary>
        /// <param name="index">第一个槽位索引。</param>
        /// <param name="targetIndex">第二个槽位索引。</param>
        /// <returns>操作成功返回 true。</returns>
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
