namespace Engine
{
    public class HealingPotion : Item
    {
        // 药水的治疗量
        public int AmountToHeal { get; set; }

        public HealingPotion(int id, string name, int amountToHeal,int price) : base(id, name,price)
        {
            AmountToHeal = amountToHeal;
        }
    }
}