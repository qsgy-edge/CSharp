using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace Engine
{
    public class Player : LivingCreature
    {
        // 玩家的位置
        private Location currentLocation;

        // 玩家的经验值
        private int experiencePoints;

        // 玩家的金币
        private int gold;

        private Monster currentMonster;

        // 玩家的事件
        public event EventHandler<MessageEventArgs> OnMessage;

        // 构造函数
        private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        public Location CurrentLocation
        {
            get { return currentLocation; }
            set
            {
                currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }

        public Weapon CurrentWeapon { get; set; }

        public int ExperiencePoints
        {
            get { return experiencePoints; }
            private set
            {
                experiencePoints = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Level");
            }
        }

        public int Gold
        {
            get { return gold; }
            set
            {
                gold = value;
                OnPropertyChanged("Gold");
            }
        }

        // 玩家的物品
        public BindingList<InventoryItem> Inventory { get; set; }

        // 玩家的等级
        public int Level
        {
            get
            {
                return ((ExperiencePoints / 100) + 1);
            }
        }

        // 玩家的药水列表
        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(ii => ii.Details is HealingPotion).Select(ii => ii.Details as HealingPotion).ToList(); }
        }

        // 玩家的任务
        public BindingList<PlayerQuest> Quests { get; set; }

        // 玩家的武器列表
        public List<Weapon> Weapons
        {
            get { return Inventory.Where(ii => ii.Details is Weapon).Select(ii => ii.Details as Weapon).ToList(); }
        }

        // 创建默认玩家
        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_CLUB), 1));
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
                if (playerData.SelectSingleNode("/Player/Stats/CurrentWeapon") != null)
                {
                    int currentWeaponID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentWeapon").InnerText);
                    player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);
                }
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

        // 玩家获取经验值
        public void AddExperiencePoints(int experiencePointsToAdd)
        {
            ExperiencePoints += experiencePointsToAdd;
            MaximumHitPoints = (Level * 10);
        }

        // 给与玩家任务奖励
        public void AddItemToInventory(Item itemToAdd)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);
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
            // 通知界面更新
            OnPropertyChanged("Inventory");
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
                if (!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }
            // 玩家有足够的物品
            return true;
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
            return Inventory.Any(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
        }

        // 判定玩家是否有该地点的任务
        public bool HasThisQuest(Quest quest)
        {
            return Quests.Any(pq => pq.Details.ID == quest.ID);
        }

        // 将任务标记为完成
        public void MarkQuestCompleted(Quest quest)
        {
            // 检查玩家的任务
            PlayerQuest playerQuest = Quests.SingleOrDefault(pq => pq.Details.ID == quest.ID);
            if (playerQuest != null)
            {
                // 玩家有任务，标记任务为完成
                playerQuest.IsCompleted = true;
            }
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);
            if (item == null)
            {
                // 玩家没有物品，不做任何事
            }
            else
            {
                // 玩家有物品，减少物品数量
                item.Quantity -= quantity;
                // 如果物品数量为0，移除物品
                if (item.Quantity < 0)
                {
                    item.Quantity = 0;
                }

                // 如果物品数量为0，移除物品
                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }
                // 通知界面更新
                OnPropertyChanged("Inventory");
            }
        }

        // 从玩家的物品中移除任务所需的物品
        public void RemoveQuestCompletionItems(Quest quest)
        {
            // 检查玩家的物品
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);
                if (item != null)
                {
                    // 玩家有物品，移除物品
                    RemoveItemFromInventory(item.Details, qci.Quantity);
                }
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
            if (CurrentWeapon != null)
            {
                XmlNode currentWeapon = playerData.CreateElement("CurrentWeapon");
                currentWeapon.AppendChild(playerData.CreateTextNode(this.CurrentWeapon.ID.ToString()));
                stats.AppendChild(currentWeapon);
            }
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

        private void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }
            if (item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }
        }

        private void RaiseMessage(string message, bool addExtraNewLine = false)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new MessageEventArgs(message, addExtraNewLine));
            }
        }

        public void MoveTo(Location newLocation)
        {
            // 判定地点是否需要特定物品才能进入
            if (!HasRequiredItemToEnterThisLocation(newLocation))
            {
                RaiseMessage("你需要 " + newLocation.ItemRequiredToEnter.Name + "才能进入。");
                return;
            }

            // 更新玩家位置
            CurrentLocation = newLocation;

            // 治疗玩家
            CurrentHitPoints = MaximumHitPoints;

            // 判定地点是否有任务
            if (newLocation.QuestAvailableHere != null)
            {
                // 判定玩家是否已经接受任务，任务是否已经完成
                bool playerAlreadyHasQuest = HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = CompletedThisQuest(newLocation.QuestAvailableHere);

                // 判定玩家是否已经接受任务
                if (playerAlreadyHasQuest)
                {
                    // 判定玩家是否已经完成任务
                    if (!playerAlreadyCompletedQuest)
                    {
                        // 判定玩家是否有任务所需的物品
                        bool playerHasAllItemsToCompleteQuest = HasAllQuestCompletionItems(newLocation.QuestAvailableHere);

                        if (playerHasAllItemsToCompleteQuest)
                        {
                            // 显示任务完成信息
                            RaiseMessage("");
                            RaiseMessage("你完成了 " + newLocation.QuestAvailableHere.Name + " 任务。");

                            // 移除任务所需的物品
                            RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            // 给与玩家任务奖励
                            RaiseMessage("你获得了：");
                            RaiseMessage(newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " 经验值");
                            RaiseMessage(newLocation.QuestAvailableHere.RewardGold.ToString() + " 金币");
                            RaiseMessage(newLocation.QuestAvailableHere.RewardItem.Name, true);

                            AddExperiencePoints(newLocation.QuestAvailableHere.RewardExperiencePoints);
                            Gold += newLocation.QuestAvailableHere.RewardGold;

                            // 玩家奖励入库
                            AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

                            // 标记任务为完成
                            MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    // 玩家没有接受任务，显示任务信息
                    RaiseMessage("你接受了 " + newLocation.QuestAvailableHere.Name + " 任务。");
                    RaiseMessage(newLocation.QuestAvailableHere.Description);
                    RaiseMessage("要完成任务，你需要：");
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.Name);
                    }
                    RaiseMessage("");

                    // 将任务添加到玩家任务列表
                    Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // 判定地点是否有怪物
            if (newLocation.MonsterLivingHere != null)
            {
                RaiseMessage("你遇到了 " + newLocation.MonsterLivingHere.Name + "！");

                // 创建怪物
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);
                currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage, standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    currentMonster.LootTable.Add(lootItem);
                }
            }
            else
            {
                currentMonster = null;
            }
        }

        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
            {
                MoveTo(CurrentLocation.LocationToNorth);
            }
        }

        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
            {
                MoveTo(CurrentLocation.LocationToEast);
            }
        }

        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
            {
                MoveTo(CurrentLocation.LocationToSouth);
            }
        }

        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
            {
                MoveTo(CurrentLocation.LocationToWest);
            }
        }

        public void UsePotion(HealingPotion potion)
        {
            // 增加玩家生命值
            CurrentHitPoints = (CurrentHitPoints + potion.AmountToHeal);

            // 玩家生命值不能超过最大生命值
            if (CurrentHitPoints > MaximumHitPoints)
            {
                CurrentHitPoints = MaximumHitPoints;
            }

            // 从玩家物品中移除药水
            RemoveItemFromInventory(potion, 1);

            // 显示信息
            RaiseMessage("你使用了 " + potion.Name);

            // 怪物攻击
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, currentMonster.MaximumDamage);

            // 显示信息
            RaiseMessage(currentMonster.Name + " 对你造成了 " + damageToPlayer.ToString() + " 点伤害。");

            // 减少玩家生命值
            CurrentHitPoints -= damageToPlayer;

            // 判定玩家是否死亡
            if (CurrentHitPoints <= 0)
            {
                // 玩家死亡
                RaiseMessage("");
                RaiseMessage("你被 " + currentMonster.Name + " 杀死了。");

                // 移动玩家到家
                MoveHome();
            }
        }

        public void UseWeapon(Weapon currentWeapon)
        {
            // 计算伤害
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            // 减少怪物生命值
            currentMonster.CurrentHitPoints -= damageToMonster;

            // 显示信息
            RaiseMessage("你对 " + currentMonster.Name + " 造成了 " + damageToMonster.ToString() + " 点伤害。");

            // 判定怪物是否死亡
            if (currentMonster.CurrentHitPoints <= 0)
            {
                // 怪物死亡
                RaiseMessage("");
                RaiseMessage("你杀死了 " + currentMonster.Name + "！");

                // 给与玩家奖励
                AddExperiencePoints(currentMonster.RewardExperiencePoints);
                RaiseMessage("你获得了 " + currentMonster.RewardExperiencePoints.ToString() + " 经验值。");
                Gold += currentMonster.RewardGold;
                RaiseMessage("你获得了 " + currentMonster.RewardGold.ToString() + " 金币。");

                // 给与玩家物品奖励
                List<InventoryItem> lootedItems = new List<InventoryItem>();
                foreach (LootItem lootItem in currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                // 判定玩家是否有物品奖励
                if (lootedItems.Count == 0)
                {
                    // 玩家没有物品奖励
                    foreach (LootItem lootItem in currentMonster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                // 将物品奖励添加到玩家物品列表
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    AddItemToInventory(inventoryItem.Details);
                    RaiseMessage("你获得了 " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name);
                }

                RaiseMessage("");
                // 移动玩家到当前位置
                MoveTo(CurrentLocation);
            }
            else
            {
                // 怪物没有死亡，计算伤害
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, currentMonster.MaximumDamage);

                // 显示信息
                RaiseMessage(currentMonster.Name + " 对你造成了 " + damageToPlayer.ToString() + " 点伤害。");

                // 减少玩家生命值
                CurrentHitPoints -= damageToPlayer;

                // 判定玩家是否死亡
                if (CurrentHitPoints <= 0)
                {
                    // 玩家死亡
                    RaiseMessage("");
                    RaiseMessage("你被 " + currentMonster.Name + " 杀死了。");

                    // 移动玩家到家
                    MoveHome();
                }
            }
        }

        private void MoveHome()
        {
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
        }
    }
}