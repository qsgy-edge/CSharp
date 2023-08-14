using RLNET;

namespace Rogue.Core
{
    public class Colors
    {
        // 地板相关颜色
        public static RLColor FloorBackground = RLColor.Black;

        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        // 墙相关颜色
        public static RLColor WallBackground = Swatch.SecondaryDarkest;

        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        // 玩家相关颜色
        public static RLColor Player = Swatch.DbLight;

        public static RLColor Gold = Swatch.DbSun;

        // 文本相关颜色
        public static RLColor TextHeading = Swatch.DbLight;

        public static RLColor Text = Swatch.DbLight;
    }
}