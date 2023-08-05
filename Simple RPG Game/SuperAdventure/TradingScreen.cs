using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperAdventure
{
    public partial class TradingScreen : Form
    {
        private Player currentPlayer;

        public TradingScreen(Player player)
        {
            currentPlayer = player;
            InitializeComponent();
            // 样式，用于显示数字列值
            DataGridViewCellStyle rightAlignedCellStyle = new DataGridViewCellStyle();
            rightAlignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            // 填充玩家库存的数据网格
            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;
            // 此隐藏列包含商品ID，因此我们知道要出售哪个商品
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "名称",
                Width = 100,
                DataPropertyName = "Description"
            });
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "数量",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "价格",
                Width = 35,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });
            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "卖1个",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });
            // 将玩家的库存绑定到数据网格视图
            dgvMyItems.DataSource = currentPlayer.Inventory;
            // 当用户单击一行时，调用此函数
            dgvMyItems.CellClick += dgvMyItems_CellClick;

            // 填充供应商库存的数据网格
            dgvVendorItems.RowHeadersVisible = false;
            dgvVendorItems.AutoGenerateColumns = false;
            // 此隐藏列包含商品ID，因此我们知道要出售哪个商品
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "名称",
                Width = 100,
                DataPropertyName = "Description"
            });
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "价格",
                Width = 35,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });
            dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "买1个",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });
            // 将供应商的库存绑定到数据网格视图
            dgvVendorItems.DataSource = currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;
            // 当用户单击一行时，调用此函数
            dgvVendorItems.CellClick += dgvVendorItems_CellClick;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 第5列是“出售1”按钮
            if (e.ColumnIndex == 4)
            {
                // 获取所选行的商品ID
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;
                // 获取所选行的商品详情
                Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));
                // 如果商品是任务物品，则不允许出售
                if (itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
                {
                    MessageBox.Show("你不能出售 " + itemBeingSold.Name);
                }
                else
                {
                    // 从玩家库存中删除一个商品
                    currentPlayer.RemoveItemFromInventory(itemBeingSold);
                    // 向玩家添加金币
                    currentPlayer.Gold += itemBeingSold.Price;
                }
            }
        }

        private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 第4列是“购买1”按钮
            if (e.ColumnIndex == 3)
            {
                // 获取所选行的商品ID
                var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;
                // 获取所选行的商品详情
                Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));
                // 如果玩家没有足够的金币，则不允许购买
                if (currentPlayer.Gold < itemBeingBought.Price)
                {
                    MessageBox.Show("你没有足够的金币来购买 "+itemBeingBought.Name);
                }
                else
                {
                    // 向玩家添加一个商品
                    currentPlayer.AddItemToInventory(itemBeingBought);
                    // 从玩家金币中减去商品价格
                    currentPlayer.Gold -= itemBeingBought.Price;
                }
            }
        }
    }
}