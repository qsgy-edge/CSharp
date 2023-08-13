using Rogue.Core;
using RogueSharp;

namespace Rogue.System
{
    public class MapGenerator
    {
        private readonly int width;
        private readonly int height;

        private readonly DungeonMap map;

        // 构造函数
        public MapGenerator(int width, int height)
        {
            this.width = width;
            this.height = height;
            map = new DungeonMap();
        }

        // 创建地图
        public DungeonMap CreateMap()
        {
            // 初始化地图
            map.Initialize(width, height);
            foreach (Cell cell in map.GetAllCells())
            {
                map.SetCellProperties(cell.X, cell.Y, true, true, true);
            }

            foreach (Cell cell in map.GetCellsInRows(0, height - 1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }

            foreach (Cell cell in map.GetCellsInColumns(0, width - 1))
            {
                map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }
            return map;
        }
    }
}