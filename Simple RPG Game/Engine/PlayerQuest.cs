using System.ComponentModel;

namespace Engine
{
    // 玩家任务类
    public class PlayerQuest : INotifyPropertyChanged
    {
        // 任务详情
        private Quest details;

        public Quest Details
        {
            get { return details; }
            set
            {
                details = value;
                OnPropertyChanged("Details");
            }
        }

        // 是否完成
        private bool isCompleted;

        // 是否可重复
        private bool isRepeatable;

        public bool IsCompleted
        {
            get
            { return isCompleted; }
            set
            {
                isCompleted = value;
                OnPropertyChanged("IsCompleted");
                OnPropertyChanged("Name");
            }
        }

        public bool IsRepeatable
        {
            get
            {
                return isRepeatable;
            }
            set
            {
                isRepeatable = value;
                OnPropertyChanged("IsRepeatable");
            }
        }

        // 任务名称
        public string Name
        {
            get { return Details.Name; }
        }

        // 构造函数
        public PlayerQuest(Quest details, bool isRepeatable)
        {
            Details = details;
            IsCompleted = false;
            IsRepeatable = isRepeatable;
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