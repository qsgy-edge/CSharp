using Engine;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";
        private Player player;

        public SuperAdventure()
        {
            InitializeComponent();

            player = PlayerDataMapper.CreateFromDatabase();
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

            player.PropertyChanged += PlayerOnPropertyChanged;
            player.OnMessage += DisplayMessage;

            player.MoveTo(player.CurrentLocation);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            player.MoveEast();
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            player.MoveNorth();
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            player.MoveSouth();
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            // 获取当前选中的药水
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;
            // 治疗玩家
            player.UsePotion(potion);
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            // 获取当前选中的武器
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;
            player.UseWeapon(currentWeapon);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            player.MoveWest();
        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }

        private void DisplayMessage(object sender, MessageEventArgs messageEventArgs)
        {
            rtbMessages.Text += messageEventArgs.Message + Environment.NewLine;

            if (messageEventArgs.AddExtraNewLine)
            {
                rtbMessages.Text += Environment.NewLine;
            }

            rtbMessages.ScrollToCaret();
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
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
            if (propertyChangedEventArgs.PropertyName == "CurrentLocation")
            {
                // 显示/隐藏可行动按钮
                btnNorth.Visible = (player.CurrentLocation.LocationToNorth != null);
                btnEast.Visible = (player.CurrentLocation.LocationToEast != null);
                btnSouth.Visible = (player.CurrentLocation.LocationToSouth != null);
                btnWest.Visible = (player.CurrentLocation.LocationToWest != null);
                btnTrade.Visible = (player.CurrentLocation.VendorWorkingHere != null);

                // 显示当前位置的信息
                rtbLocation.Text = player.CurrentLocation.Name + Environment.NewLine;
                rtbLocation.Text += player.CurrentLocation.Description + Environment.NewLine;

                if (player.CurrentLocation.MonsterLivingHere == null)
                {
                    cboWeapons.Visible = false;
                    cboPotions.Visible = false;
                    btnUseWeapon.Visible = false;
                    btnUsePotion.Visible = false;
                }
                else
                {
                    cboWeapons.Visible = player.Weapons.Any();
                    cboPotions.Visible = player.Potions.Any();
                    btnUseWeapon.Visible = player.Weapons.Any();
                    btnUsePotion.Visible = player.Potions.Any();
                }
            }
        }

        private void SuperAdventure_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, player.ToXmlString());
            PlayerDataMapper.SaveToDatabase(player);
        }

        private void btnTrade_Click(object sender, EventArgs e)
        {
            TradingScreen tradingScreen = new TradingScreen(player);
            tradingScreen.StartPosition = FormStartPosition.CenterParent;
            tradingScreen.ShowDialog(this);
        }
    }
}