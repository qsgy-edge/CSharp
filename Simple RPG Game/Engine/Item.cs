namespace Engine
{
    public class Item
    {
        // 物品的 ID
        public int ID { get; set; }

        // 物品的名称
        public string Name { get; set; }

        // 构造函数
        public Item(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}