using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";
        private Monster currentMonster;
        private Player player;

        public SuperAdventure()
        {
            InitializeComponent();

            // 判定玩家数据文件是否存在
            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                player = Player.CreateDefaultPlayer();
            }
            // 绑定玩家属性到 UI
            lblHitPoints.DataBindings.Add("Text", player, "CurrentHitPoints");
            lblGold.DataBindings.Add("Text", player, "Gold");
            lblExperience.DataBindings.Add("Text", player, "ExperiencePoints");
            lblLevel.DataBindings.Add("Text", player, "Level");

            // 绑定库存到 UI
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.AutoGenerateColumns = false;
            dgvInventory.DataSource = player.Inventory;
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "名称",
                Width = 197,
                DataPropertyName = "Description"
            });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "数量",
                DataPropertyName = "Quantity"
            });

            // 绑定任务列表到 UI
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.AutoGenerateColumns = false;
            dgvQuests.DataSource = player.Quests;
            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "名称",
                Width = 197,
                DataPropertyName = "Name"
            });
            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "完成?",
                DataPropertyName = "IsCompleted"
            });

            // 绑定武器列表到 UI
            cboWeapons.DataSource = player.Weapons;
            cboWeapons.DisplayMember = "武器名称";
            cboWeapons.ValueMember = "Id";
            if (player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = player.CurrentWeapon;
            }
            cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

            // 绑定药水列表到 UI
            cboPotions.DataSource = player.Potions;
            cboPotions.DisplayMember = "药水名称";
            cboPotions.ValueMember = "Id";
            player.PropertyChanged += PlayerOnPropertyChanged;

            MoveTo(player.CurrentLocation);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToEast);
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToNorth);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToSouth);
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            // 获取当前选中的药水
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;
            // 治疗玩家
            player.CurrentHitPoints = (player.CurrentHitPoints + potion.AmountToHeal);
            // 治疗量不得超过玩家的最大生命值
            if (player.CurrentHitPoints > player.MaximumHitPoints)
            {
                player.CurrentHitPoints = player.MaximumHitPoints;
            }
            // 从库存中移除药水
            player.RemoveItemFromInventory(potion, 1);
            // 显示消息
            rtbMessages.Text += "你喝了 " + potion.Name + Environment.NewLine;
            // 怪物对玩家造成伤害
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, currentMonster.MaximumDamage);
            // 显示消息
            rtbMessages.Text += "怪物对你造成了 " + damageToPlayer.ToString() + " 点伤害。" + Environment.NewLine;
            // 对玩家造成伤害
            player.CurrentHitPoints -= damageToPlayer;
            // 判定玩家是否死亡
            if (player.CurrentHitPoints <= 0)
            {
                // 显示消息
                rtbMessages.Text += "你被打败了。" + Environment.NewLine;
                // 移动玩家到“家”
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }

            ScrollToBottomOfMessages();
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            // 获取当前选中的武器
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;
            // 计算对怪物造成的伤害
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);
            // 对怪物造成伤害
            currentMonster.CurrentHitPoints -= damageToMonster;
            // 显示消息
            rtbMessages.Text += "你对 " + currentMonster.Name + " 造成了 " + damageToMonster.ToString() + " 点伤害。" + Environment.NewLine;
            // 判定怪物是否已经死亡
            if (currentMonster.CurrentHitPoints <= 0)
            {
                // 怪物已经死亡
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "你杀死了 " + currentMonster.Name + Environment.NewLine;
                // 给予玩家经验值
                player.AddExperiencePoints(currentMonster.RewardExperiencePoints);
                rtbMessages.Text += "你获得了 " + currentMonster.RewardExperiencePoints.ToString() + " 点经验值。" + Environment.NewLine;
                // 给予玩家金币
                player.Gold += currentMonster.RewardGold;
                rtbMessages.Text += "你获得了 " + currentMonster.RewardGold.ToString() + " 金币。" + Environment.NewLine;
                // 获得怪物的随机掉落物品
                List<InventoryItem> lootedItems = new List<InventoryItem>();
                // 添加物品到掉落物品列表
                foreach (LootItem lootItem in currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }
                // 如果没有掉落物品
                if (lootedItems.Count == 0)
                {
                    // 添加默认掉落物品
                    foreach (LootItem lootItem in currentMonster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }
                // 将掉落物品添加到玩家的库存中
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    player.AddItemToInventory(inventoryItem.Details);

                    rtbMessages.Text += "你获得了 " + inventoryItem.Quantity.ToString() + " 个 " + inventoryItem.Details.Name + Environment.NewLine;
                }
                // 添加空行
                rtbMessages.Text += Environment.NewLine;
                // 移动玩家到当前位置
                MoveTo(player.CurrentLocation);
            }
            else
            {
                // 怪物仍然活着
                // 计算对玩家造成的伤害
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, currentMonster.MaximumDamage);
                // 显示消息
                rtbMessages.Text += "怪物对你造成了 " + damageToPlayer.ToString() + " 点伤害。" + Environment.NewLine;
                // 对玩家造成伤害
                player.CurrentHitPoints -= damageToPlayer;

                // 判定玩家是否死亡
                if (player.CurrentHitPoints <= 0)
                {
                    // 显示消息
                    rtbMessages.Text += "你被打败了。" + Environment.NewLine;
                    // 移动玩家到“家”
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
            ScrollToBottomOfMessages();
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToWest);
        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }

        private void MoveTo(Location newLocation)
        {
            // 判定地点是否需要特定物品才能进入
            if (newLocation.ItemRequiredToEnter != null)
            {
                // 判定玩家是否拥有特定物品
                if (!player.HasRequiredItemToEnterThisLocation(newLocation))
                {
                    // 玩家没有特定物品，显示消息
                    rtbMessages.Text += "你必须拥有 " + newLocation.ItemRequiredToEnter.Name + " 才能进入这里。" + Environment.NewLine;
                    return;
                }
            }

            // 更新玩家位置
            player.CurrentLocation = newLocation;

            // 显示/隐藏移动按钮
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            // 显示地点名称和描述
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            // 治疗玩家
            player.CurrentHitPoints = player.MaximumHitPoints;

            // 判定地点是否有任务
            if (newLocation.QuestAvailableHere != null)
            {
                // 判定玩家是否已经拥有任务，任务是否完成
                bool playerAlreadyHasQuest = player.HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = player.CompletedThisQuest(newLocation.QuestAvailableHere);

                // 如果玩家已经拥有任务
                if (playerAlreadyHasQuest)
                {
                    // 如果玩家未完成任务
                    if (!playerAlreadyCompletedQuest)
                    {
                        // 判定玩家是否拥有完成任务所需的物品
                        bool playerHasAllItemsToCompleteQuest = player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere);

                        // 玩家拥有完成任务所需的所有物品
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            // 显示消息
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "你完成了 '" + newLocation.QuestAvailableHere.Name + "' 的任务。" + Environment.NewLine;

                            // 从玩家的物品清单中移除完成任务所需的物品
                            player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            // 给予任务奖励
                            rtbMessages.Text += "你获得了： " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " 经验值" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " 金币" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            player.AddExperiencePoints(newLocation.QuestAvailableHere.RewardExperiencePoints);
                            player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            // 将奖励物品添加到玩家的库存中
                            player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

                            // 将任务标记为已完成
                            player.MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    // 该玩家还没有获得该任务

                    // 显示消息
                    rtbMessages.Text += "你获得了 " + newLocation.QuestAvailableHere.Name + " 任务。" + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "为了完成任务，你需要：" + Environment.NewLine;
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                    }
                    rtbMessages.Text += Environment.NewLine;

                    // 将任务添加到任务清单
                    player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // 判定地点是否有怪物
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "你遇到了一只 " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                // 创建怪物
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = player.Weapons.Any();
                cboPotions.Visible = player.Potions.Any();
                btnUseWeapon.Visible = player.Weapons.Any();
                btnUsePotion.Visible = player.Potions.Any();
            }
            else
            {
                currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }
            ScrollToBottomOfMessages();
        }

        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Weapons")
            {
                cboWeapons.DataSource = player.Weapons;
                if (!player.Weapons.Any())
                {
                    cboWeapons.Visible = false;
                    btnUseWeapon.Visible = false;
                }
            }
            if (propertyChangedEventArgs.PropertyName == "Potions")
            {
                cboPotions.DataSource = player.Potions;
                if (!player.Potions.Any())
                {
                    cboPotions.Visible = false;
                    btnUsePotion.Visible = false;
                }
            }
        }

        private void ScrollToBottomOfMessages()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void SuperAdventure_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, player.ToXmlString());
        }
    }
}