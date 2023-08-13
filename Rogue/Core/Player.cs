namespace Rogue.Core
{
    public class Player : Actor
    {
        // 构造函数
        public Player()
        {
            Awareness = 15;
            Name = "Rogue";
            Color = Colors.Player;
            Symbol = '@';
            X = 10;
            Y = 10;
        }
    }
}