using SimpleSQLiteORM;

namespace BagAndShop.Manager
{
    /// <summary>
    /// 提供实体对象与数据库之间的数据存取管理类
    /// </summary>
    public static class EntityManager
    {
        /// <summary>
        /// 将实体对象保存（或更新）到数据库
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TInfo">信息模型类型</typeparam>
        /// <param name="db">SQLite 数据库实例</param>
        /// <param name="entity">要保存的实体对象</param>
        public static async Task Save<TEntity, TInfo>(SqliteDataBase db, TEntity entity)
            where TEntity : IEntity<TEntity, TInfo>
            where TInfo : IData<TEntity>
        {
            TInfo info = entity.ToData();
            await db.UpsertAsync<TInfo>(info);
        }
        /// <summary>
        /// 根据主键从数据库加载实体对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TInfo">信息模型类型</typeparam>
        /// <param name="db">SQLite 数据库实例</param>
        /// <param name="key">主键 ID</param>
        /// <returns>加载的实体对象</returns>
        /// <exception cref="DataNotFoundException">当找不到对应主键的数据时抛出</exception>
        public static async Task<TEntity> Load<TEntity, TData>(SqliteDataBase db, int key)
            where TEntity : IEntity<TEntity, TData>
            where TData : IData<TEntity>, new()
        {
            TData info = await db.GetByKeyAsync<TData>(key)
                ?? throw new DataNotFoundException(db, "From key");
            return info.ToEntity();
        }
        /// <summary>
        /// 根据条件子句从数据库加载单个实体对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TInfo">信息模型类型</typeparam>
        /// <param name="db">SQLite 数据库实例</param>
        /// <param name="whereClause">SQL WHERE 查询子句</param>
        /// <returns>查询到的第一个实体对象</returns>
        /// <exception cref="DataNotFoundException">未查询到符合条件的数据时抛出</exception>
        public static async Task<TEntity> Load<TEntity, TData>(SqliteDataBase db, string whereClause)
            where TEntity : IEntity<TEntity, TData>
            where TData : IData<TEntity>, new()
        {
            List<TData> datas = await db.QueryAsync<TData>(
                where: whereClause
                );
            if (datas.Count <= 0)
            {
                throw new DataNotFoundException(db, "Form where clause");
            }
            else
            {
                // 仅获取第一个数据
                TData data = datas[0];
                return data.ToEntity();
            }
        }
        /// <summary>
        /// 根据条件子句从数据库加载实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TInfo">信息模型类型</typeparam>
        /// <param name="db">SQLite 数据库实例</param>
        /// <param name="whereClause">SQL WHERE 查询子句</param>
        /// <returns>包含所有匹配实体的列表，若无匹配则返回空列表</returns>
        public static async Task<List<TEntity>> LoadList<TEntity, TData>(SqliteDataBase db, string whereClause)
            where TEntity : IEntity<TEntity, TData>
            where TData : IData<TEntity>, new()
        {
            List<TData> datas = await db.QueryAsync<TData>(
                where: whereClause
                );
            if (datas.Count <= 0) return new(); // List查询不抛出异常

            List<TEntity> entities = new();
            foreach (var data in datas)
            {
                entities.Add(data.ToEntity());
            }
            return entities;
        }
    }
    /// <summary>
    /// 当数据库查询未找到预期结果时抛出的异常
    /// </summary>
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
