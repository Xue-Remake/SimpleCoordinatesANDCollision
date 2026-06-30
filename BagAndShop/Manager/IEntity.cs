using BagAndShop.Informations;
using SimpleSQLiteORM;

namespace BagAndShop.Manager
{
    /// <summary>
    /// 定义从持久化信息对象转换为实体对象的接口
    /// </summary>
    /// <typeparam name="TEntity">目标实体类型</typeparam>
    public interface IData<TEntity>
    {
        TEntity ToEntity();
    }
    /// <summary>
    /// 定义从实体对象转换为持久化信息对象的接口
    /// </summary>
    /// <typeparam name="TSelf">实体自身的类型</typeparam>
    /// <typeparam name="TData">关联的信息对象类型</typeparam>
    public interface IEntity<TSelf, TData> where TSelf : IEntity<TSelf, TData> where TData : IData<TSelf>
    {
        TData ToData();
        Task<LoadInformation> LoadFrom(SqliteDataBase db);
    }
}