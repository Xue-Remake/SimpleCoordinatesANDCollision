using BagAndShop.Item;
using BagAndShop.ItemTemplate;
using SimpleSQLiteORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.ItemTemplate
{
    public static class ItemManager
    {
        public static async Task SaveItem<TEntity, TInfo>(SqliteDataBase db, TEntity item) 
            where TEntity : IHasInfo<TEntity, TInfo>
            where TInfo : IBaseInfo<TEntity>
        {
            TInfo info = item.ToInfo();
            await db.UpsertAsync<TInfo>(info);
        }

        public static async Task<TEntity> LoadItem<TEntity, TInfo>(SqliteDataBase db, int key)
            where TEntity : IHasInfo<TEntity, TInfo>
            where TInfo : IBaseInfo<TEntity>, new()
        {
            TInfo info = await db.GetByKeyAsync<TInfo>(key)
                ?? throw new ItemDataNotFound(db);
            return info.ToEntity();
        }
    }
}
