using System.ComponentModel;

namespace Engine
{
    // 库存物品类
    public class InventoryItem : INotifyPropertyChanged
    {
        // 物品详情
        private Item details;

        public Item Details
        {
            get { return details; }
            set
            {
                details = value;
                OnPropertyChanged("Details");
            }
        }

        // 物品数量
        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged("Quantity");
                OnPropertyChanged("Description");
            }
        }

        // 物品描述
        public string Description
        {
            get { return Details.Name; }
        }

        // 构造函数
        public InventoryItem(Item details, int quantity)
        {
            Details = details;
            Quantity = quantity;
        }

        // 事件
        public event PropertyChangedEventHandler PropertyChanged;

        // 事件处理函数
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}