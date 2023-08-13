using RLNET;

namespace Rogue
{
    public class Game
    {
        // 屏幕高度和宽度以图块数为单位
        private static readonly int screenWidth = 100;

        private static readonly int screenHeight = 70;

        // 用于控制游戏的 RLRootConsole
        private static RLRootConsole rootConsole;

        public static void Main(string[] args)
        {
            // 设置游戏的标题和字体
            string fontFileName = "terminal8x8.png";
            string consoleTitle = "RougeSharp V3 Tutorial - Level 1";

            // 设置游戏界面的字体、大小和标题
            rootConsole = new RLRootConsole(fontFileName, screenWidth, screenHeight, 8, 8, 1f, consoleTitle);

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
            rootConsole.Draw();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            rootConsole.Print(10, 10, "It worked!", RLColor.White);
        }
    }
}