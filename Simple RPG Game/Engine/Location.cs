using System.Collections.Generic;
using System.Linq;

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
        private readonly SortedList<int, int> monsters = new SortedList<int, int>();

        // 地点商人
        public Vendor VendorWorkingHere { get; set; }

        // 地点
        public Location LocationToNorth { get; set; }
        public Location LocationToSouth { get; set; }
        public Location LocationToWest { get; set; }
        public Location LocationToEast { get; set; }

        // 地点是否有怪物
        public bool HasAMonster
        { get { return monsters.Count > 0; } }

        // 地点是否有任务
        public bool HasAQuest
        { get { return QuestAvailableHere != null; } }

        // 地点是否有准入物品
        public bool DoesNotHaveAnItemRequiredToEnter
        { get { return ItemRequiredToEnter == null; } }

        // 构造函数
        public Location(int id, string name, string description, Item itemRequiredToEnter = null, Quest questAvailableHere = null)
        {
            ID = id;
            Name = name;
            Description = description;
            ItemRequiredToEnter = itemRequiredToEnter;
            QuestAvailableHere = questAvailableHere;
        }

        // 添加怪物
        public void AddMonster(int monsterID, int percentageOfAppearance)
        {
            if (monsters.ContainsKey(monsterID))
            {
                monsters[monsterID] = percentageOfAppearance;
            }
            else
            {
                monsters.Add(monsterID, percentageOfAppearance);
            }
        }

        // 新的怪物实例
        public Monster NewInstanceOfMonsterLivingHere()
        {
            if (!HasAMonster)
            {
                return null;
            }

            // 怪物出现总概率
            int totalPercentages = monsters.Values.Sum();

            // 随机数
            int randomNumber = RandomNumberGenerator.NumberBetween(1, totalPercentages);

            // 怪物出现概率
            int runningTotal = 0;

            // 遍历怪物，找到随机数对应的怪物
            foreach (KeyValuePair<int, int> monster in monsters)
            {
                runningTotal += monster.Value;

                if (randomNumber <= runningTotal)
                {
                    return World.MonsterByID(monster.Key).NewInstanceOfMonster();
                }
            }

            // 如果没有找到怪物，返回列表中的第一个怪物
            return World.MonsterByID(monsters.Keys.First()).NewInstanceOfMonster();
        }
    }
}