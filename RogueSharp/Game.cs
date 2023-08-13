using RLNET;
using RogueSharp.Core;
using RogueSharp.System;

namespace RogueSharp
{
    public class Game
    {
        // 屏幕高度和宽度以图块数为单位
        private static readonly int screenWidth = 100;

        private static readonly int screenHeight = 70;

        // 用于控制游戏的 RLRootConsole
        private static RLRootConsole rootConsole;

        // 子界面
        // 游戏地图
        private static readonly int mapWidth = 80;

        private static readonly int mapHeight = 48;
        private static RLConsole mapConsole;

        // 游戏消息
        private static readonly int messageWidth = 80;

        private static readonly int messageHeight = 11;
        private static RLConsole messageConsole;

        // 游戏状态
        private static readonly int statWidth = 20;

        private static readonly int statHeight = 70;
        private static RLConsole statConsole;

        // 物品栏
        private static readonly int inventoryWidth = 80;

        private static readonly int inventoryHeight = 11;
        private static RLConsole inventoryConsole;

        //

        public static DungeonMap DungeonMap { get; private set; }

        public static void Main(string[] args)
        {
            // 设置游戏的标题和字体
            string fontFileName = "terminal8x8.png";
            string consoleTitle = "RougeSharp V3 Tutorial - Level 1";

            // 设置游戏界面的字体、大小和标题
            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8, 8, 1f, consoleTitle);

            // 初始化子界面
            mapConsole = new RLConsole(mapWidth, mapHeight);
            messageConsole = new RLConsole(messageWidth, messageHeight);
            statConsole = new RLConsole(statWidth, statHeight);
            inventoryConsole = new RLConsole(inventoryWidth, inventoryHeight);

            MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight);
            DungeonMap = mapGenerator.CreateMap();

            // 设置游戏的事件处理函数

            // 设置游戏的更新函数
            rootConsole.Update += OnRootConsoleUpdate;

            // 设置游戏的渲染函数
            rootConsole.Render += OnRootConsoleRender;

            // 开始游戏循环
            rootConsole.Run();
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            // 将子界面绘制到主界面上
            RLConsole.Blit(mapConsole, 0, 0, mapWidth, mapHeight, rootConsole, 0, inventoryHeight);
            RLConsole.Blit(messageConsole, 0, 0, messageWidth, messageHeight, rootConsole, 0, screenHeight - messageHeight);
            RLConsole.Blit(statConsole, 0, 0, statWidth, statHeight, rootConsole, mapWidth, 0);
            RLConsole.Blit(inventoryConsole, 0, 0, inventoryWidth, inventoryHeight, rootConsole, 0, 0);

            //
            DungeonMap.Draw(mapConsole);

            // 绘制主界面
            rootConsole.Draw();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            // 设置子界面
            mapConsole.SetBackColor(0, 0, mapWidth, mapHeight, RLColor.Black);
            mapConsole.Print(1, 1, "Map", RLColor.White);

            messageConsole.SetBackColor(0, 0, messageWidth, messageHeight, RLColor.Gray);
            messageConsole.Print(1, 1, "Messages", RLColor.White);

            statConsole.SetBackColor(0, 0, statWidth, statHeight, RLColor.Brown);
            statConsole.Print(1, 1, "Stats", RLColor.White);

            inventoryConsole.SetBackColor(0, 0, inventoryWidth, inventoryHeight, RLColor.Cyan);
            inventoryConsole.Print(1, 1, "Inventory", RLColor.White);
        }
    }
}