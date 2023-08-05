using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player player;
        private Monster currentMonster;
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

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

            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            UpdatePlayerStats();
        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }

        private void UpdatePlayerStats()
        {
            lblHitPoints.Text = player.CurrentHitPoints.ToString();
            lblGold.Text = player.Gold.ToString();
            lblExperience.Text = player.ExperiencePoints.ToString();
            lblLevel.Text = player.Level.ToString();
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToNorth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToEast);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToSouth);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToWest);
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

            // 在UI上更新玩家的生命值
            lblHitPoints.Text = player.CurrentHitPoints.ToString();

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

                            player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
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

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            // 刷新玩家状态
            UpdatePlayerStats();

            // 刷新玩家的库存
            UpdateInventoryListInUI();

            // 刷新玩家的任务列表
            UpdateQuestListInUI();

            // 刷新玩家的武器列表
            UpdateWeaponListInUI();

            // 刷新玩家的药水列表

            UpdatePotionListInUI();
        }

        private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                // 玩家没有药水，因此隐藏药水组合框和“使用”按钮
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "药水名称";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }

        private void UpdateWeaponListInUI()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                // 玩家没有武器，因此隐藏武器组合框和“使用”按钮
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                // 删除事件处理程序，以免在更新组合框时触发事件
                cboWeapons.SelectedIndexChanged -= cboWeapons_SelectedIndexChanged;
                // 更新武器组合框
                cboWeapons.DataSource = weapons;
                // 重新添加事件处理程序，以便在玩家选择新武器时触发事件
                cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;
                cboWeapons.DisplayMember = "武器名称";
                cboWeapons.ValueMember = "ID";

                if (player.CurrentWeapon != null)
                {
                    cboWeapons.SelectedItem = player.CurrentWeapon;
                }
                else
                {
                    cboWeapons.SelectedIndex = 0;
                }
            }
        }

        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "任务名称";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "是否完成?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }
        }

        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "物品名称";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "数量";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }
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
                player.ExperiencePoints += currentMonster.RewardExperiencePoints;
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
                // 刷新玩家的信息与物品
                UpdatePlayerStats();
                // 刷新 UI
                UpdateInventoryListInUI();
                UpdateWeaponListInUI();
                UpdatePotionListInUI();
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
                // 刷新玩家的生命值
                lblHitPoints.Text = player.CurrentHitPoints.ToString();

                // 判定玩家是否死亡
                if (player.CurrentHitPoints <= 0)
                {
                    // 显示消息
                    rtbMessages.Text += "你被打败了。" + Environment.NewLine;
                    // 移动玩家到“家”
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
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
            foreach (InventoryItem ii in player.Inventory)
            {
                if (ii.Details.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }
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

            // 刷新玩家 UI
            lblHitPoints.Text = player.CurrentHitPoints.ToString();
            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }

        private void SuperAdventure_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, player.ToXmlString());
        }
    }
}