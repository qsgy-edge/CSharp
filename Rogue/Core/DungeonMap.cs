using RLNET;
using RogueSharp;
using System.Collections.Generic;

namespace Rogue.Core
{
    public class DungeonMap : Map
    {
        // 用于存储所有的房间
        public List<Rectangle> Rooms;

        // 构造函数
        public DungeonMap()
        {
            // 初始化房间列表
            Rooms = new List<Rectangle>();
        }

        // 每次更新地图时，我们都会将所有的地图块绘制到地图控制台上
        public void Draw(RLConsole mapConsole)
        {
            mapConsole.Clear();
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
        }

        // 设置角色的位置，如果目标位置是可行走的，则返回 true
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // 只有当目标位置是可行走的时，才能移动
            if (GetCell(x, y).IsWalkable)
            {
                // 当角色移动时，我们需要将其当前位置设置为可行走的
                SetIsWalkable(actor.X, actor.Y, true);

                // 更新角色的位置
                actor.X = x;
                actor.Y = y;

                // 角色新位置被角色占据，因此我们将其设置为不可行走的
                SetIsWalkable(actor.X, actor.Y, false);

                // 如果角色是玩家，则更新视野范围
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        //  设置地图块的可行走性
        private void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        // 更新玩家的视野范围
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;

            ComputeFov(player.X, player.Y, player.Awareness, true);

            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
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