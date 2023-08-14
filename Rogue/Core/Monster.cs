using RLNET;

namespace Rogue.Core
{
    public class Monster : Actor
    {
        public void DrawStats(RLConsole statConsole, int position)
        {
            int yPosition = 13 + (position * 2);

            // 怪物符号
            statConsole.Print(1, yPosition, Symbol.ToString(), Color);
            int width = (int)(((double)Health / (double)MaxHealth) * 16.0);
            int remainingWidth = 16 - width;

            // 绘制怪物血条
            statConsole.SetBackColor(3, yPosition, width, 1, Swatch.Primary);
            statConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest);

            // 在血条上绘制怪物名称
            statConsole.Print(2, yPosition, $": {Name}", Colors.Text);
        }
    }
}