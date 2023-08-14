using RLNET;
using Rogue.Core;
using Rogue.Systems;

namespace Rogue
{
    public static class Game
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

        // 游戏是否需要渲染
        private static bool renderRequired = true;

        // 玩家控制系统
        public static CommandSystem CommandSystem { get; private set; }

        // 玩家
        public static Player Player { get; set; }

        // 地牢地图
        public static DungeonMap DungeonMap { get; private set; }

        // 随机数生成器
        public static IRandom Random { get; private set; }

        public static void Main(string[] args)
        {
            // 生成随机种子
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            // 设置游戏的标题和字体
            string fontFileName = "terminal8x8.png";
            string consoleTitle = $"RougeSharp - Level 1 - Seed {seed}";

            // 设置游戏界面的字体、大小和标题
            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8, 8, 1f, consoleTitle);

            // 初始化子界面
            mapConsole = new RLConsole(mapWidth, mapHeight);
            messageConsole = new RLConsole(messageWidth, messageHeight);
            statConsole = new RLConsole(statWidth, statHeight);
            inventoryConsole = new RLConsole(inventoryWidth, inventoryHeight);

            InitChildrenConsole();

            // 初始化玩家
            Player = new Player();

            CommandSystem = new CommandSystem();

            // 初始化地牢地图
            MapGenerator mapGenerator = new MapGenerator(mapWidth, mapHeight);
            DungeonMap = mapGenerator.CreateMap();

            // 更新玩家的视野
            DungeonMap.UpdatePlayerFieldOfView();

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

            // 将地牢地图绘制到地图界面上
            DungeonMap.Draw(mapConsole);

            // 将玩家绘制到地图界面上
            Player.Draw(mapConsole, DungeonMap);

            // 将主界面绘制到屏幕上
            rootConsole.Draw();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            GetPlayerInput();
        }

        private static void GetPlayerInput()
        {
            bool didPlayerAct = false;
            RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();

            if (keyPress != null)
            {
                if (keyPress.Key == RLKey.W)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                }
                else if (keyPress.Key == RLKey.S)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                }
                else if (keyPress.Key == RLKey.A)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                }
                else if (keyPress.Key == RLKey.D)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                }
                else if (keyPress.Key == RLKey.Escape)
                {
                    rootConsole.Close();
                }
            }

            if (didPlayerAct)
            {
                renderRequired = true;
            }
        }

        private static void InitChildrenConsole()
        {
            // 设置子界面
            messageConsole.SetBackColor(0, 0, messageWidth, messageHeight, Swatch.DbDeepWater);
            messageConsole.Print(1, 1, "Messages", Colors.TextHeading);

            statConsole.SetBackColor(0, 0, statWidth, statHeight, Swatch.DbOldStone);
            statConsole.Print(1, 1, "Stats", Colors.TextHeading);

            inventoryConsole.SetBackColor(0, 0, inventoryWidth, inventoryHeight, Swatch.DbWood);
            inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);
        }
    }
}