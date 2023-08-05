using System.ComponentModel;

namespace Engine
{
    public class LivingCreature : INotifyPropertyChanged
    {
        // 当前生命值
        private int currentHitPoints;

        public int CurrentHitPoints
        {
            get { return currentHitPoints; }
            set
            {
                currentHitPoints = value;
                OnPropertyChanged("CurrentHitPoints");
            }
        }

        // 最大生命值
        public int MaximumHitPoints { get; set; }

        // 构造函数
        public LivingCreature(int currentHitPoints, int maximumHitPoints)
        {
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
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