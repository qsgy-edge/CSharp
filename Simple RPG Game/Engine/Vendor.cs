using System.ComponentModel;
using System.Linq;

namespace Engine
{
    public class Vendor : INotifyPropertyChanged
    {
        // 商人名称
        public string Name { get; set; }

        // 商人库存
        public BindingList<InventoryItem> Inventory { get; set; }

        // 构造函数
        public Vendor(string name)
        {
            Name = name;
            Inventory = new BindingList<InventoryItem>();
        }

        // 添加物品到库存
        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);
            if (item == null)
            {
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                item.Quantity += quantity;
            }
            OnPropertyChanged("Inventory");
        }

        // 从库存移除物品
        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);
            if (item != null)
            {
                item.Quantity -= quantity;
                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }
                OnPropertyChanged("Inventory");
            }
        }

        // 事件
        public event PropertyChangedEventHandler PropertyChanged;

        // 事件处理函数
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}