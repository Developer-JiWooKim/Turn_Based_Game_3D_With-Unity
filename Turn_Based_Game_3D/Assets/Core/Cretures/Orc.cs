namespace Turn_Based_Game
{
    public class Orc : Monster
    {
        public Orc(string _name, int _hp, int _atk, int _healAmount) : base(_name, _hp, _atk, _healAmount)
        {
        }

        public int BerserkMode { get; private set; } = 10;
        public override void Attack(IDamageable target)
        {
            int finalDamage = Atk;

            if (Hp <= MaxHp / 2)
            {
                SendMessage($"{Name}이(가) 눈이 붉게 충혈되며 분노합니다! (공격력 +10)");
                finalDamage += BerserkMode;
            }

            SendMessage($"{Name}의 강력한 몽둥이 휘두르기!");
            target.TakeDamage(finalDamage);
        }
    }
}