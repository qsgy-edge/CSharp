using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Xml;

namespace Engine
{
    public class Player : LivingCreature
    {
        // 玩家的位置
        private Location currentLocation;

        private Monster currentMonster;

        // 玩家的经验值
        private int experiencePoints;

        // 玩家的金币
        private int gold;

        // 构造函数
        private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        // 玩家的事件
        public event EventHandler<MessageEventArgs> OnMessage;

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

        public static Player CreatePlayerFromDatabase(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int currentLocationID)
        {
            Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints);
            player.MoveTo(World.LocationByID(currentLocationID));
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
        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            InventoryItem existingItemInInventory = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);
            if (existingItemInInventory == null)
            {
                // 玩家没有物品，添加物品
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                // 玩家有物品，增加物品数量
                existingItemInInventory.Quantity += quantity;
            }
            // 通知界面更新
            RaiseInventoryChangedEvent(itemToAdd);
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
            if (location.DoesNotHaveAnItemRequiredToEnter)
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

        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
            {
                MoveTo(CurrentLocation.LocationToEast);
            }
        }

        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
            {
                MoveTo(CurrentLocation.LocationToNorth);
            }
        }

        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
            {
                MoveTo(CurrentLocation.LocationToSouth);
            }
        }

        public void MoveTo(Location location)
        {
            // 判定地点是否需要特定物品才能进入
            if (PlayerDoesNotHaveTheRequiredItemToEnter(location))
            {
                RaiseMessage("你需要 " + location.ItemRequiredToEnter.Name + "才能进入。");
                return;
            }

            // 更新玩家位置
            CurrentLocation = location;

            // 治疗玩家
            CompletelyHeal();

            // 判定地点是否有任务
            if (location.HasAQuest)
            {
                if (PlayerDoesNotHaveThisQuest(location.QuestAvailableHere))
                {
                    GiveQuestToPlayer(location.QuestAvailableHere);
                }
                else
                {
                    if (PlayerHasNotCompleted(location.QuestAvailableHere) &&
                       PlayerHasAllQuestCompletionItemsFor(location.QuestAvailableHere))
                    {
                        GivePlayerQuestRewards(location.QuestAvailableHere);
                    }
                }
            }

            SetTheCurrentMonsterForTheCurrentLocation(location);
        }

        private void GivePlayerQuestRewards(Quest questAvailableHere)
        {
            RaiseMessage("");
            RaiseMessage("你完成了 " + questAvailableHere.Name + " 任务");
            RaiseMessage("你获得了：");
            RaiseMessage(questAvailableHere.RewardExperiencePoints + " 经验值");
            RaiseMessage(questAvailableHere.RewardGold + " 金币");
            RaiseMessage(questAvailableHere.RewardItem.Name,true);

            AddExperiencePoints(questAvailableHere.RewardExperiencePoints);
            gold += questAvailableHere.RewardGold;

            RemoveQuestCompletionItems(questAvailableHere);
            AddItemToInventory(questAvailableHere.RewardItem);

            MarkQuestCompleted(questAvailableHere);
        }

        private void CompletelyHeal()
        {
            CurrentHitPoints = MaximumHitPoints;
        }

        private bool PlayerHasAllQuestCompletionItemsFor(Quest questAvailableHere)
        {
            // 判定玩家是否有完成任务必须的所有物品
            foreach(QuestCompletionItem qci in questAvailableHere.QuestCompletionItems)
            {
                // 检查玩家拥有的所有物品，查看是否满足要求
                if (!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity>=qci.Quantity))
                {
                    return false;
                }
            }

            return true;
        }

        private void GiveQuestToPlayer(Quest questAvailableHere)
        {
            RaiseMessage("你接受了 " + questAvailableHere.Name + " 任务");
            RaiseMessage(questAvailableHere.Description);
            RaiseMessage("为了完成任务，你需要：");

            foreach (QuestCompletionItem qci in questAvailableHere.QuestCompletionItems)
            {
                    RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.Name);
            }

            RaiseMessage("");
            Quests.Add(new PlayerQuest(questAvailableHere));
        }

        private bool PlayerHasNotCompleted(Quest questAvailableHere)
        {
            return Quests.Any(playerQuest => playerQuest.Details.ID == questAvailableHere.ID && !playerQuest.IsCompleted);
        }

        private bool PlayerDoesNotHaveThisQuest(Quest questAvailableHere)
        {
            return Quests.All(playerQuest=>playerQuest.Details.ID!=questAvailableHere.ID);
        }

        private bool PlayerDoesNotHaveTheRequiredItemToEnter(Location location)
        {
            return !HasRequiredItemToEnterThisLocation(location);
        }

        private void SetTheCurrentMonsterForTheCurrentLocation(Location location)
        {
            currentMonster = location.NewInstanceOfMonsterLivingHere();

            if(currentMonster!=null)
            {
                RaiseMessage("你遇到了 " + location.MonsterLivingHere.Name);
            }
        }

        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
            {
                MoveTo(CurrentLocation.LocationToWest);
            }
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);
            if (item != null)
            {
                // 玩家有物品，减少物品数量
                item.Quantity -= quantity;
                // 如果物品数量为0，移除物品
                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }
                // 通知界面更新
                RaiseInventoryChangedEvent(itemToRemove);
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

        public void UsePotion(HealingPotion potion)
        {
            // 显示信息
            RaiseMessage("你喝了 " + potion.Name);

            HealPlayer(potion.AmountToHeal);

            // 从玩家物品中移除药水
            RemoveItemFromInventory(potion, 1);

            // 怪物攻击
            LetTheMonsterAttack();
        }

        private void LetTheMonsterAttack()
        {
            int damagedToPlayer = RandomNumberGenerator.NumberBetween(0, currentMonster.MaximumDamage);

            RaiseMessage(currentMonster.Name + " 对你造成了 " + damagedToPlayer + " 点伤害");

            CurrentHitPoints -= damagedToPlayer;

            if(IsDead)
            {
                RaiseMessage(currentMonster.Name + " 杀死了你");

                MoveHome();
            }
        }

        private void HealPlayer(int hitPointsToHeal)
        {
            CurrentHitPoints = Math.Min(CurrentHitPoints + hitPointsToHeal, MaximumHitPoints);
        }

        public void UseWeapon(Weapon weapon)
        {
            // 计算伤害
            int damageToMonster = RandomNumberGenerator.NumberBetween(weapon.MinimumDamage, weapon.MaximumDamage);

            if (damageToMonster == 0)
            {
                RaiseMessage("你未命中 " + currentMonster.Name);
            }
            else
            {
                // 减少怪物生命值
                currentMonster.CurrentHitPoints -= damageToMonster;
                // 显示信息
                RaiseMessage("你对 " + currentMonster.Name + " 造成了 " + damageToMonster + " 点伤害。");
            }

            // 判定怪物是否死亡
            if (currentMonster.IsDead)
            {
                LootTheCurrentMonster();

                // 移动玩家到当前位置
                MoveTo(CurrentLocation);
            }
            else
            {
                // 怪物没有死亡，怪物攻击
                LetTheMonsterAttack();
            }
        }

        private void LootTheCurrentMonster()
        {
            RaiseMessage("");
            RaiseMessage("你击败了 " + currentMonster.Name);
            RaiseMessage("你获得了 " + currentMonster.RewardExperiencePoints + " 经验值");
            RaiseMessage("你获得了 " + currentMonster.RewardGold + " 金币");

            AddExperiencePoints(currentMonster.RewardExperiencePoints);
            Gold += currentMonster.RewardGold;

            // 将怪物的战利品交给玩家
            foreach(InventoryItem inventoryItem in currentMonster.LootItems)
            {
                AddItemToInventory(inventoryItem.Details);

                RaiseMessage(string.Format("你获得了 {0} {1}", inventoryItem.Quantity, inventoryItem.Description));
            }

            RaiseMessage("");
        }

        private void MoveHome()
        {
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
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
    }
}