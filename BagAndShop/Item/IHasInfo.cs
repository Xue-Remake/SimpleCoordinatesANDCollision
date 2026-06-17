namespace BagAndShop.ItemTemplate
{
    public interface IBaseInfo
    {
    }
    public interface IHasInfo<TInfo> where TInfo : IBaseInfo
    {
        TInfo ToInfo();
    }
}
