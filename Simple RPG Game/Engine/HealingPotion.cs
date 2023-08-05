using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HealingPotion : Item
    {
        // 药水的治疗量
        public int AmountToHeal { get; set; }

        public HealingPotion(int id, string name, int amountToHeal) : base(id, name)
        {
            AmountToHeal = amountToHeal;
        }
    }
}