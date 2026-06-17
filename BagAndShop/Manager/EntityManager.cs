using SimpleSQLiteORM;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Manager
{
    public static class EntityManager
    {
        public static async Task Save<TEntity, TInfo>(SqliteDataBase db, TEntity item) 
            where TEntity : IEntity<TEntity, TInfo>
            where TInfo : IInfo<TEntity>
        {
            TInfo info = item.ToInfo();
            await db.UpsertAsync<TInfo>(info);
        }
        public static async Task<TEntity> Load<TEntity, TInfo>(SqliteDataBase db, int key)
            where TEntity : IEntity<TEntity, TInfo>
            where TInfo : IInfo<TEntity>, new()
        {
            TInfo info = await db.GetByKeyAsync<TInfo>(key)
                ?? throw new DataNotFoundException(db, "From key");
            return info.ToEntity();
        }
        public static async Task<TEntity> Load<TEntity, TInfo>(SqliteDataBase db, string whereClause)
            where TEntity : IEntity<TEntity, TInfo>
            where TInfo : IInfo<TEntity>, new()
        {
            List<TInfo> infos = await db.QueryAsync<TInfo>(
                where: whereClause
                );
            if (infos.Count <= 0)
            {
                throw new DataNotFoundException(db, "Form where clause");
            }
            else
            {
                // 仅获取第一个数据
                TInfo info = infos[0];
                return info.ToEntity();
            }
        }
        public static async Task<List<TEntity>> LoadList<TEntity, TInfo>(SqliteDataBase db, string whereClause)
            where TEntity : IEntity<TEntity, TInfo>
            where TInfo : IInfo<TEntity>, new()
        {
            List<TInfo> infos = await db.QueryAsync<TInfo>(
                where: whereClause
                );
            if (infos.Count <= 0) return new(); // List查询不抛出异常

            List<TEntity> entities = new();
            foreach (var info in infos)
            {
                entities.Add(info.ToEntity());
            }
            return entities;
        }
    }

    public class DataNotFoundException : Exception
    {
        public string QueryMethod { get; private set; }
        public SqliteDataBase? DataBase;
        public DataNotFoundException() 
        {
            QueryMethod = "From key";
        }
        public DataNotFoundException(SqliteDataBase? dataBase, string method)
        {
            DataBase = dataBase;
            QueryMethod = method;
        }
    }
}
