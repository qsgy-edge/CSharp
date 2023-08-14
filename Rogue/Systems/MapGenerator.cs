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

            // 放置玩家
            PlacePlayer();

            // 创建房间之间的通道
            for (int r = 1; r < map.Rooms.Count; r++)
            {
                // 获取当前房间和上一个房间的中心点
                Point previousRoomCenter = map.Rooms[r - 1].Center;
                Point currentRoomCenter = map.Rooms[r].Center;

                // 50%的概率先创建水平通道，再创建垂直通道
                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, currentRoomCenter.X);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
                    CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, currentRoomCenter.Y);
                }
            }
        }

        // 创建水平通道
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                map.SetCellProperties(x, yPosition, true, true, true);
            }
        }

        // 创建垂直通道
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                map.SetCellProperties(xPosition, y, true, true, true);
            }
        }

        // 放置玩家
        private void PlacePlayer()
        {
            Player player = Game.Player ?? new Player();
            player.X = map.Rooms[0].Center.X;
            player.Y = map.Rooms[0].Center.Y;

            map.AddPlayer(player);
        }
    }
}