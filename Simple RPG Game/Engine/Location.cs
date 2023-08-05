namespace Engine
{
    public class Location
    {
        // 地点 ID
        public int ID { get; set; }

        // 地点名称
        public string Name { get; set; }

        // 地点描述
        public string Description { get; set; }

        // 地点物品
        public Item ItemRequiredToEnter { get; set; }

        // 地点任务
        public Quest QuestAvailableHere { get; set; }

        // 地点怪物
        public Monster MonsterLivingHere { get; set; }

        // 地点
        public Location LocationToNorth { get; set; }

        public Location LocationToSouth { get; set; }
        public Location LocationToWest { get; set; }
        public Location LocationToEast { get; set; }

        // 构造函数
        public Location(int id, string name, string description, Item itemRequiredToEnter = null, Quest questAvailableHere = null, Monster monsterLivingHere = null)
        {
            ID = id;
            Name = name;
            Description = description;
            ItemRequiredToEnter = itemRequiredToEnter;
            QuestAvailableHere = questAvailableHere;
            MonsterLivingHere = monsterLivingHere;
        }
    }
}