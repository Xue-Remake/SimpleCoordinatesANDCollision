namespace BagAndShop.Informations
{
    /// <summary>
    /// 操作结果信息的基类，包含操作返回的消息文本。
    /// 作为所有具体信息类的父类，提供消息传递的基础能力。
    /// </summary>
    public class Information
    {
        public string? message { get; set; }
        public Information(string message)
        {
            this.message = message;
        }
        public Information()
        {
            message = null;
        }
    }
}
