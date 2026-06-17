namespace BagAndShop.ItemTemplate
{
    public static class TagConverter
    {
        /// <summary>
        /// 将string转换为CategoryTag枚举类型
        /// </summary>
        /// <param name="str">对应的字符串</param>
        /// <returns>对应的CategoryTag</returns>
        /// <exception cref="Exception">没有对应的Tag时触发的异常</exception>
        public static CategoryTag StringToCategory(string str)
        {
            foreach (var category in Enum.GetValues<CategoryTag>())
            {
                if (category.ToString() == str)
                    return category;
            }
            throw new Exception($"The category tag corresponding to {str} does not exist.");
        }
        /// <summary>
        /// 将string转换为StatusTag枚举类型
        /// </summary>
        /// <param name="str">对应的字符串</param>
        /// <returns>对应的StatusTag</returns>
        /// <exception cref="Exception">没有对应的Tag时触发的异常</exception>
        public static StatusTag StringToStatus(string str)
        {
            foreach (var status in Enum.GetValues<StatusTag>())
            {
                if (status.ToString() == str)
                    return status;
            }
            throw new Exception($"The status tag corresponding to {str} does not exist");
        }
    }
}
