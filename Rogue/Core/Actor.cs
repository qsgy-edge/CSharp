using RLNET;
using Rogue.Interfaces;
using RogueSharp;

namespace Rogue.Core
{
    public class Actor : IActor, IDrawable
    {
        //
        public string Name { get; set; }

        public int Awareness { get; set; }

        //
        public RLColor Color { get; set; }

        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            // 只有当地图块被探索时，才绘制
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            // 只有当人物在玩家的视野范围内时，才绘制
            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            // 否则，我们将绘制普通的地板
            else
            {
                console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
            }
        }
    }
}