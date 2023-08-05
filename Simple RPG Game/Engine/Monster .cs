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

        // 怪物掉落物
        public List<LootItem> LootTable { get; set; }

        //构造函数
        public Monster(int id, string name, int maximumDamage, int rewardExperiencePoints, int rewardGold, int currentHitPoints, int maximumHitPoints) : base(currentHitPoints, maximumHitPoints)
        {
            ID = id;
            Name = name;
            MaximumDamage = maximumDamage;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            LootTable = new List<LootItem>();
        }
    }
}