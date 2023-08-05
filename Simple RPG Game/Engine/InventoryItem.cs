using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine
{
  // 库存物品类
  public class InventoryItem
  {
    // 物品详情
    public Item Details { get; set; }
    // 物品数量
    public int Quantity { get; set; }
    // 构造函数
    public InventoryItem(Item details, int quantity)
    {
      Details = details;
      Quantity = quantity;
    }
  }
}