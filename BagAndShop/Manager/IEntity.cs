namespace BagAndShop.Manager
{
    public interface IInfo<TEntity>
    {
        TEntity ToEntity();
    }
    public interface IEntity<TSelf,TInfo> where TSelf : IEntity<TSelf, TInfo> where TInfo : IInfo<TSelf>
    {
        TInfo ToInfo();
    }
}
