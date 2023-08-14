using RLNET;
using Rogue.Interfaces;
using RogueSharp;

namespace Rogue.Core
{
    public class Actor : IActor, IDrawable
    {
        // IActor
        private int attack;

        private int attackChance;
        private int awareness;
        private int defense;
        private int defenseChance;
        private int gold;
        private int health;
        private int maxHealth;
        private string name;
        private int speed;

        public int Attack
        {
            get
            {
                return attack;
            }
            set
            {
                attack = value;
            }
        }

        public int AttackChance
        {
            get
            {
                return attackChance;
            }
            set
            {
                attackChance = value;
            }
        }

        public int Awareness
        {
            get
            {
                return awareness;
            }
            set
            {
                awareness = value;
            }
        }

        public int Defense
        {
            get
            {
                return defense;
            }
            set
            {
                defense = value;
            }
        }

        public int DefenseChance
        {
            get
            {
                return defenseChance;
            }
            set
            {
                defenseChance = value;
            }
        }

        public int Gold
        {
            get
            {
                return gold;
            }
            set
            {
                gold = value;
            }
        }

        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
            }
        }

        public int MaxHealth
        {
            get
            {
                return maxHealth;
            }
            set
            {
                maxHealth = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        //
        public RLColor Color { get; set; }

        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            // 只有当地图块被探索时，才绘制
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            // 只有当人物在玩家的视野范围内时，才绘制
            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            // 否则，我们将绘制普通的地板
            else
            {
                console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
            }
        }
    }
}