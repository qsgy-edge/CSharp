using System.Linq;
using System.Collections.Generic;

namespace Engine
{
    public class Monster : LivingCreature
    {
        // 怪物 ID
        public int ID { get; set; }

        // 怪物名称
        public string Name { get; set; }

        // 怪物最大伤害值
        public int MaximumDamage { get; set; }

        // 怪物奖励经验值
        public int RewardExperiencePoints { get; set; }

        // 怪物奖励金币
        public int RewardGold { get; set; }

        // 该类型怪物可能拥有的所有可能物品（带百分比）
        public List<LootItem> LootTable { get; set; }

        // 该怪物所拥有的物品
        internal List<InventoryItem> LootItems { get; }

        //构造函数
        public Monster(int id, string name, int maximumDamage, int rewardExperiencePoints, int rewardGold, int currentHitPoints, int maximumHitPoints) : base(currentHitPoints, maximumHitPoints)
        {
            ID = id;
            Name = name;
            MaximumDamage = maximumDamage;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            LootTable = new List<LootItem>();
            LootItems = new List<InventoryItem>();
        }

        internal Monster NewInstanceOfMonster()
        {
            Monster monster = new Monster(ID, Name, MaximumDamage, RewardExperiencePoints, RewardGold, CurrentHitPoints, MaximumHitPoints);

            // 将物品添加到 lootedItems 列表中，将随机数与掉落百分比进行比较
            foreach (LootItem lootItem in LootTable.Where(LootItem => RandomNumberGenerator.NumberBetween(1, 100) <= LootItem.DropPercentage))
            {
                monster.LootItems.Add(new InventoryItem(lootItem.Details, 1));
            }

            // 如果没有随机选择任何物品，则添加默认的战利品物品
            if (monster.LootItems.Count == 0)
            {
                foreach (LootItem lootItem in LootTable.Where(x => x.IsDefaultItem))
                {
                    monster.LootItems.Add(new InventoryItem(lootItem.Details, 1));
                }
            }

            return monster;
        }
    }
}