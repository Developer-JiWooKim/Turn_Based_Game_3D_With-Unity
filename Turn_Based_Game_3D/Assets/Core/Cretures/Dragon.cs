namespace Turn_Based_Game
{
    public class Dragon : Monster
    {
        public Dragon(string _name, int _hp, int _atk, int _healAmount) : base(_name, _hp, _atk, _healAmount)
        {
        }
        public int BreathProbability = 30; // 브레스 쏠 확률 30%
        public int BreathDamage { get; private set; } = 20;
        public override void Attack(IDamageable target)
        {
            bool isBreath = random.Next(1, 101) <= BreathProbability;
            int finalDamage = Atk;

            if (isBreath)
            {
                SendMessage($"{Name}이 스킬 [브레스]를 시전합니다.");
                finalDamage += BreathDamage;
            }

            SendMessage($"{Name} 이/가 {finalDamage}의 위력으로 공격");
            target.TakeDamage(finalDamage);
        }
    }
}