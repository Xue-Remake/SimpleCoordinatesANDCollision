using BagAndShop.ItemTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.ItemTemplate.Manager
{
    public static class ItemManager
    {
        public static void SaveItem<T, F>() where T : ItemBase, IHasInfo<F> where F : IBaseInfo
        { 
        }

        public static T Load<T, F>() where T : ItemBase, IHasInfo<F> where F : IBaseInfo
        { }
    }
}
