using Rogue.Core;
using RogueSharp;
using System;
using System.Linq;

namespace Rogue.Systems
{
    public class MapGenerator
    {
        // 地下城大小
        private readonly int width;

        private readonly int height;

        // 房间参数
        private readonly int maxRooms;

        private readonly int roomMaxSize;
        private readonly int roomMinSize;

        private readonly DungeonMap map;

        // 构造函数
        public MapGenerator(int width, int height, int maxRooms, int roomMaxSize, int roomMinSize)
        {
            this.width = width;
            this.height = height;
            this.maxRooms = maxRooms;
            this.roomMaxSize = roomMaxSize;
            this.roomMinSize = roomMinSize;
            map = new DungeonMap();
        }

        // 创建地图
        public DungeonMap CreateMap()
        {
            // 初始化地图
            map.Initialize(width, height);

            //
            for (int r = maxRooms; r > 0; r--)
            {
                // 随机房间大小、位置
                int roomWidth = Game.Random.Next(roomMinSize, roomMaxSize);
                int roomHeight = Game.Random.Next(roomMinSize, roomMaxSize);
                int roomXPosition = Game.Random.Next(0, width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, height - roomHeight - 1);

                // 房间用矩形表示
                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                // 检查房间是否与其他房间重叠
                bool newRoomIntersects = map.Rooms.Any(room => newRoom.Intersects(room));

                // 如果没有重叠，将房间添加到地图
                if (!newRoomIntersects)
                {
                    map.Rooms.Add(newRoom);
                }

                // 遍历房间列表，创建房间
                foreach (Rectangle room in map.Rooms)
                {
                    CreateRoom(room);
                }
            }

            return map;
        }

        // 给定地图上的一个矩形区域将该区域的单元格属性设置为 true
        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    map.SetCellProperties(x, y, true, true, true);
                }
            }
        }
    }
}