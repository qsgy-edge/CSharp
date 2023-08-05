using System.Collections.Generic;

namespace Engine
{
    public class Player : LivingCreature
    {
        // 玩家的金币
        public int Gold { get; set; }

        // 玩家的经验值
        public int ExperiencePoints { get; set; }

        // 玩家的等级
        public int Level
        {
            get
            {
                return ((ExperiencePoints / 100) + 1);
            }
        }

        // 玩家的位置
        public Location CurrentLocation { get; set; }

        // 玩家的物品
        public List<InventoryItem> Inventory { get; set; }

        // 玩家的任务
        public List<PlayerQuest> Quests { get; set; }

        // 构造函数
        public Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;

            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        // 判定玩家是否有必须的物品进入某个位置
        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                // 没有物品要求
                return true;
            }
            // 检查玩家的物品
            return Inventory.Exists(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
        }

        // 判定玩家是否有该地点的任务
        public bool HasThisQuest(Quest quest)
        {
            return Quests.Exists(pq => pq.Details.ID == quest.ID);
        }

        // 判定玩家是否完成了该任务
        public bool CompletedThisQuest(Quest quest)
        {
            foreach (PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    // 玩家完成了任务
                    return playerQuest.IsCompleted;
                }
            }
            // 玩家没有任务
            return false;
        }

        // 判定玩家是否有任务所需的物品
        public bool HasAllQuestCompletionItems(Quest quest)
        {
            // 检查玩家的物品
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                if (!Inventory.Exists(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }
            // 玩家有足够的物品
            return true;
        }

        // 从玩家的物品中移除任务所需的物品
        public void RemoveQuestCompletionItems(Quest quest)
        {
            // 检查玩家的物品
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                InventoryItem item = Inventory.Find(ii => ii.Details.ID == qci.Details.ID);
                if (item != null)
                {
                    // 玩家有物品，移除物品
                    item.Quantity -= qci.Quantity;
                }
            }
        }

        // 给与玩家任务奖励
        public void AddItemToInventory(Item itemToAdd)
        {
            InventoryItem item = Inventory.Find(ii => ii.Details.ID == itemToAdd.ID);
            if (item == null)
            {
                // 玩家没有物品，添加物品
                Inventory.Add(new InventoryItem(itemToAdd, 1));
            }
            else
            {
                // 玩家有物品，增加物品数量
                item.Quantity++;
            }
        }

        // 将任务标记为完成
        public void MarkQuestCompleted(Quest quest)
        {
            // 检查玩家的任务
            PlayerQuest playerQuest = Quests.Find(pq => pq.Details.ID == quest.ID);
            if (playerQuest != null)
            {
                // 玩家有任务，标记任务为完成
                playerQuest.IsCompleted = true;
            }
        }
    }
}