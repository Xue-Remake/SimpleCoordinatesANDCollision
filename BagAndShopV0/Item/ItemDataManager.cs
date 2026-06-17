using SqliteCrudLib.Core;
using SqliteCrudLib.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Item
{
    public static class ItemDataManager
    {
        public async static Task<T> GetItem<T>(SqliteDatabase db, int id) where T : class, IItem, new()
        {
            var result = await db.Data.QueryAsync<T>(
                whereClause: "\"Id\" = @itemid",
                parameters: new {itemid = id}
                );
            return result.FirstOrDefault();
        }
    }
}
