using System.Collections.Generic;

namespace Engine
{
    public class Quest
    {
        // 任务的 ID
        public int ID { get; set; }

        // 任务名称
        public string Name { get; set; }

        // 任务描述
        public string Description { get; set; }

        // 任务完成后的奖励经验值
        public int RewardExperiencePoints { get; set; }

        // 任务完成后的奖励金币
        public int RewardGold { get; set; }

        // 任务完成后的奖励物品
        public Item RewardItem { get; set; }

        // 任务完成所需物品
        public List<QuestCompletionItem> QuestCompletionItems { get; set; }

        // 构造函数
        public Quest(int id, string name, string director, int rewardExperiencePoints, int rewardGold, Item rewardItem = null)
        {
            ID = id;
            Name = name;
            Description = director;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            RewardItem = rewardItem;
            QuestCompletionItems = new List<QuestCompletionItem>();
        }
    }
}