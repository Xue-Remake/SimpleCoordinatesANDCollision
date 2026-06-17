namespace BagAndShop.ItemTemplate
{
    public interface IBaseInfo<TEntity>
    {
        TEntity ToEntity();
    }
    public interface IHasInfo<TSelf,TInfo> where TSelf : IHasInfo<TSelf, TInfo> where TInfo : IBaseInfo<TSelf>
    {
        TInfo ToInfo();
    }
}
