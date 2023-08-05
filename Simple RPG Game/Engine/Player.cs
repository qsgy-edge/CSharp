﻿using System;
using System.Collections.Generic;
using System.Xml;

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
        private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        // 创建默认玩家
        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);
            return player;
        }

        // 从XML字符串创建玩家
        public static Player CreatePlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerData = new XmlDocument();
                playerData.LoadXml(xmlPlayerData);
                int currentHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentHitPoints").InnerText);
                int maximumHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/MaximumHitPoints").InnerText);
                int gold = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Gold").InnerText);
                int experiencePoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/ExperiencePoints").InnerText);
                Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints);
                int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);
                foreach (XmlNode node in playerData.SelectNodes("/Player/InventoryItems/InventoryItem"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);
                    for (int i = 0; i < quantity; i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }
                foreach (XmlNode node in playerData.SelectNodes("/Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);
                    PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(id));
                    playerQuest.IsCompleted = isCompleted;
                    player.Quests.Add(playerQuest);
                }
                return player;
            }
            catch
            {
                // 如果出现任何错误，返回默认玩家
                return Player.CreateDefaultPlayer();
            }
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

        // XML 序列化
        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();
            // 创建顶级 XML 节点
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);
            // 创建 Stats 子节点来保存其他玩家统计数据节点
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);
            // 创建其他玩家统计数据节点
            XmlNode currentHitPoints = playerData.CreateElement("CurrentHitPoints");
            currentHitPoints.AppendChild(playerData.CreateTextNode(this.CurrentHitPoints.ToString()));
            stats.AppendChild(currentHitPoints);
            XmlNode maximumHitPoints = playerData.CreateElement("MaximumHitPoints");
            maximumHitPoints.AppendChild(playerData.CreateTextNode(this.MaximumHitPoints.ToString()));
            stats.AppendChild(maximumHitPoints);
            XmlNode gold = playerData.CreateElement("Gold");
            gold.AppendChild(playerData.CreateTextNode(this.Gold.ToString()));
            stats.AppendChild(gold);
            XmlNode experiencePoints = playerData.CreateElement("ExperiencePoints");
            experiencePoints.AppendChild(playerData.CreateTextNode(this.ExperiencePoints.ToString()));
            stats.AppendChild(experiencePoints);
            XmlNode currentLocation = playerData.CreateElement("CurrentLocation");
            currentLocation.AppendChild(playerData.CreateTextNode(this.CurrentLocation.ID.ToString()));
            stats.AppendChild(currentLocation);
            // 创建 InventoryItems 子节点来保存玩家物品节点
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);
            // 创建玩家物品节点
            foreach (InventoryItem item in this.Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");
                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = item.Details.ID.ToString();
                inventoryItem.Attributes.Append(idAttribute);
                XmlAttribute quantityAttribute = playerData.CreateAttribute("Quantity");
                quantityAttribute.Value = item.Quantity.ToString();
                inventoryItem.Attributes.Append(quantityAttribute);
                inventoryItems.AppendChild(inventoryItem);
            }
            // 创建 PlayerQuests 子节点来保存玩家任务节点
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);
            // 创建玩家任务节点
            foreach (PlayerQuest quest in this.Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");
                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = quest.Details.ID.ToString();
                playerQuest.Attributes.Append(idAttribute);
                XmlAttribute isCompletedAttribute = playerData.CreateAttribute("IsCompleted");
                isCompletedAttribute.Value = quest.IsCompleted.ToString();
                playerQuest.Attributes.Append(isCompletedAttribute);
                playerQuests.AppendChild(playerQuest);
            }
            return playerData.InnerXml; // 返回 XML 字符串
        }
    }
}