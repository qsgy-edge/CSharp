namespace Engine
{
  // 玩家任务类
  public class PlayerQuest
  {
    // 任务详情
    public Quest Details { get; set; }
    // 是否完成
    public bool IsCompleted { get; set; }
    // 构造函数
    public PlayerQuest(Quest details)
    {
      Details = details;
      IsCompleted = false;
    }
  }
}