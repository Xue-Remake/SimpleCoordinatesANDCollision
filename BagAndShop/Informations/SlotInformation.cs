using BagAndShop.BagSystem.Template;
using BagAndShop.ItemSystem;

namespace BagAndShop.Informations
{
    public class SlotInformation : Information
    {
        public Slot slot { get; set; }
        public int RemainCount { get; set; }
        public ItemBase Item { get; set; }
        public bool Completed { get; set; }
        public SlotInformation(Slot slot, ItemBase item, int count, bool completed, string message) : base(message)
        {
            this.slot = slot;
            RemainCount = count;
            Item = item;
            Completed = completed;
        }
        public static SlotInformation GetAddFailed(Slot slot, ItemBase item, int count)
        {
            return new SlotInformation(slot, item, count, false, "Add failed: remaining");
        }
        public static SlotInformation GetRemoveFailed(Slot slot, ItemBase item, int count)
        {
            return new SlotInformation(slot, item, count, false, "Remove failed: remaining");
        }
        public static SlotInformation GetAddCompleted(Slot slot, ItemBase item)
        {
            return new SlotInformation(slot, item, 0, true, "Add Completed");
        }
        public static SlotInformation GetRemoveCompleted(Slot slot, ItemBase item)
        {
            return new SlotInformation(slot, item, 0, true, "Remove Completed");
        }
    }
}
