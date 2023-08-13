using RLNET;
using RogueSharp;

namespace Rogue.Core
{
    public class DungeonMap : Map
    {
        // 每次更新地图时，我们都会将所有的地图块绘制到地图控制台上
        public void Draw(RLConsole mapConsole)
        {
            mapConsole.Clear();
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            // 当地图块未被探索时，我们不会将其绘制到地图控制台上
            if (!cell.IsExplored)
            {
                return;
            }

            // 如果地图块在玩家的视野范围内，我们将使用浅色绘制地图块
            if (IsInFov(cell.X, cell.Y))
            {
                // 如果地图块是可行走的，我们将绘制一个 '.'，否则我们将绘制一个 '#'
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            // 如果地图块不在玩家的视野范围内，我们将使用深色绘制地图块
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }
    }
}