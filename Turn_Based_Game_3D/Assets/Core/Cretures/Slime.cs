using System;

namespace Turn_Based_Game
{
    public class Slime : Monster
    {
        public Slime(string _name, int _hp, int _atk, int _healAmount) : base(_name, _hp, _atk, _healAmount) { }

        public int ReduceValue { get; private set; } = 5;

        public override void TakeDamage(int damage)
        {
            // 생존 여부 판단
            if (!IsAlive) return;

            // 공격 방어 여부 판단
            if (Guard)
            {
                SendMessage($"{Name}이(가) 공격을 완벽히 방어했습니다!");
                Guard = false;  // 방어 성공 시 해제
                return;
            }

            int finalDamage = Math.Max(0, damage - ReduceValue);

            SendMessage($"{Name}의 몸이 말랑말랑해서 {ReduceValue}데미지가 경감되었습니다!");
            SendMessage($"{Name} 이/가 {finalDamage}의 데미지를 입었습니다.");

            NotifyDamaged();

            Hp -= finalDamage;
        }
    }
}