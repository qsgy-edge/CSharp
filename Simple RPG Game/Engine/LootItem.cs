namespace Engine
{
    // 战利品类
    public class LootItem
    {
        // 物品详情
        public Item Details { get; set; }

        // 掉落概率
        public int DropPercentage { get; set; }

        // 是否为默认掉落物品
        public bool IsDefaultItem { get; set; }

        // 构造函数
        public LootItem(Item details, int dropPercentage, bool isDefaultItem)
        {
            Details = details;
            DropPercentage = dropPercentage;
            IsDefaultItem = isDefaultItem;
        }
    }
}