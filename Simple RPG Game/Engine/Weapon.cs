using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Weapon : Item
    {
        // 武器的最小伤害值
        public int MinimumDamage { get; set; }

        // 武器的最大伤害值
        public int MaximumDamage { get; set; }

        // 构造函数
        public Weapon(int id, string name, int minimumDamage, int maximumDamage) : base(id, name)
        {
            MinimumDamage = minimumDamage;
            MaximumDamage = maximumDamage;
        }
    }
}