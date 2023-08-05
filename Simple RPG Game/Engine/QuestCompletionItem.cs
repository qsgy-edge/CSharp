using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Engine
{
  // 任务奖励类
  public class QuestCompletionItem
  {
    // 物品详情
    public Item Details { get; set; }
    // 物品数量
    public int Quantity { get; set; }
    // 构造函数
    public QuestCompletionItem(Item details, int quantity)
    {
      Details = details;
      Quantity = quantity;
    }
  }
}