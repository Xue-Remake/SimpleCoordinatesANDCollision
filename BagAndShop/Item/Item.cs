using SqliteCrudLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Item
{
    public interface IItem
    {
        string GetName();
        int GetId();
        int GetPrice();
        int GetMaxStack();
        Double GetWeight();
    }
    public class NullItem : IItem
    {
        private static readonly NullItem _instance = new NullItem();
        public static NullItem Instance => _instance;
        public int GetPrice() => 0;
        public double GetWeight() => 0;
        public int GetId() => -1;
        public string GetName() => "null";
        public int GetMaxStack() => 0;
    }
    [DbTable("ItemTemplates")]
    public class ItemTemplate : IItem
    {
        [DbKey]
        public int Id { get; set; }
        [DbColumn("Name")]
        public string Name { get; set; } = string.Empty;
        [DbColumn("Price")]
        public int Price { get; set; }
        [DbColumn("Weight")]
        public double Weight { get; set; }
        [DbColumn("MaxStack")]
        public int MaxStack { get; set; }
        public int GetPrice() => Price;
        public Double GetWeight() => Weight;
        public int GetId() => Id;
        public string GetName() => Name;
        public int GetMaxStack() => MaxStack;
    }
}
