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
        public int Level { get; set; }

        // 玩家的位置
        public Location CurrentLocation { get; set; }

        // 玩家的物品
        public List<InventoryItem> Inventory { get; set; }

        // 玩家的任务
        public List<PlayerQuest> Quests { get; set; }

        // 构造函数
        public Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int level) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Level = level;

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
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    // 玩家有物品
                    return true;
                }
            }
            // 玩家没有物品
            return false;
        }

        // 判定玩家是否有该地点的任务
        public bool HasThisQuest(Quest quest)
        {
            foreach (PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    // 玩家有任务
                    return true;
                }
            }
            // 玩家没有任务
            return false;
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
                bool foundItemInPlayersInventory = false;
                // 检查玩家的物品
                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        // 玩家有物品
                        foundItemInPlayersInventory = true;
                        if (ii.Quantity < qci.Quantity)
                        {
                            // 玩家没有足够的物品
                            return false;
                        }
                    }
                }
                // 玩家没有物品
                if (!foundItemInPlayersInventory)
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
                // 检查玩家的物品
                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        // 玩家有物品
                        ii.Quantity -= qci.Quantity;
                        break;
                    }
                }
            }
        }

        // 给与玩家任务奖励
        public void AddItemToInventory(Item itemToAdd)
        {
            // 检查玩家的物品
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == itemToAdd.ID)
                {
                    // 玩家有物品，数量加一
                    ii.Quantity++;
                    return;
                }
            }
            // 玩家没有物品，添加物品
            Inventory.Add(new InventoryItem(itemToAdd, 1));
        }

        // 将任务标记为完成
        public void MarkQuestCompleted(Quest quest)
        {
            // 检查玩家的任务
            foreach (PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    // 玩家有任务，标记为完成
                    pq.IsCompleted = true;
                    return;
                }
            }
        }
    }
}