using BagAndShop.BagSystem.Template;
using BagAndShop.ItemSystem;

namespace BagAndShop.Informations
{
    /// <summary>
    /// 背包格子操作的结果信息类，包含目标格子、剩余数量、操作物品以及完成状态等数据。
    /// 继承自 <see cref="Information"/>，用于传递物品添加或移除过程中的成功、失败或完成信息。
    /// </summary>
    public class SlotInformation : Information
    {
        /// <summary>
        /// 操作涉及的目标格子实例。
        /// </summary>
        public Slot slot { get; set; }
        /// <summary>
        /// 操作后剩余的物品数量。添加或移除未完成时可能大于零，完成时为 <c>0</c>。
        /// </summary>
        public int RemainCount { get; set; }
        /// <summary>
        /// 操作涉及的目标物品实例。
        /// </summary>
        public ItemBase Item { get; set; }
        /// <summary>
        /// 表示操作是否已完成。为 <c>true</c> 时表示添加或移除已全部完成。
        /// </summary>
        public bool Completed { get; set; }
        public SlotInformation(Slot slot, ItemBase item, int count, bool completed, string message) : base(message)
        {
            this.slot = slot;
            RemainCount = count;
            Item = item;
            Completed = completed;
        }
        /// <summary>
        /// 创建一个表示添加操作失败（尚有剩余）的 <see cref="SlotInformation"/> 实例。
        /// </summary>
        /// <param name="slot">操作涉及的目标格子。</param>
        /// <param name="item">操作涉及的目标物品。</param>
        /// <param name="count">剩余未添加的数量。</param>
        /// <returns>包含失败信息的 <see cref="SlotInformation"/> 实例。</returns>
        public static SlotInformation GetAddFailed(Slot slot, ItemBase item, int count)
        {
            return new SlotInformation(slot, item, count, false, "Add failed: remaining");
        }
        /// <summary>
        /// 创建一个表示移除操作失败（尚有剩余）的 <see cref="SlotInformation"/> 实例。
        /// </summary>
        /// <param name="slot">操作涉及的目标格子。</param>
        /// <param name="item">操作涉及的目标物品。</param>
        /// <param name="count">剩余未移除的数量。</param>
        /// <returns>包含失败信息的 <see cref="SlotInformation"/> 实例。</returns>
        public static SlotInformation GetRemoveFailed(Slot slot, ItemBase item, int count)
        {
            return new SlotInformation(slot, item, count, false, "Remove failed: remaining");
        }
        /// <summary>
        /// 创建一个表示添加操作已完成的 <see cref="SlotInformation"/> 实例。
        /// </summary>
        /// <param name="slot">操作涉及的目标格子。</param>
        /// <param name="item">操作涉及的目标物品。</param>
        /// <returns>包含完成信息的 <see cref="SlotInformation"/> 实例。</returns>
        public static SlotInformation GetAddCompleted(Slot slot, ItemBase item)
        {
            return new SlotInformation(slot, item, 0, true, "Add Completed");
        }
        /// <summary>
        /// 创建一个表示移除操作已完成的 <see cref="SlotInformation"/> 实例。
        /// </summary>
        /// <param name="slot">操作涉及的目标格子。</param>
        /// <param name="item">操作涉及的目标物品。</param>
        /// <returns>包含完成信息的 <see cref="SlotInformation"/> 实例。</returns>
        public static SlotInformation GetRemoveCompleted(Slot slot, ItemBase item)
        {
            return new SlotInformation(slot, item, 0, true, "Remove Completed");
        }
    }
}
