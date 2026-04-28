using System;
using System.Collections.Generic;

namespace Turn_Based_Game
{
    public abstract class Monster : Creature
    {
        protected enum MonsterAction
        {
            Attack,
            Defence,
            Heal,
            Done
        }
        protected float[] healProbabiliyUp = { 30f, 30f, 40f }; // 누적확률 용 flaot 배열
        protected static readonly Random random = new Random();

        protected Dictionary<MonsterAction, Action<IDamageable>> _monsterActions;

        protected Monster(string _name, int _hp, int _atk, int _healAmount) : base(_name, _hp, _atk, _healAmount)
        {
            InitActions();
        }

        // 행동 정의
        private void InitActions()
        {
            _monsterActions = new Dictionary<MonsterAction, Action<IDamageable>>
            {
                { MonsterAction.Attack, (target) => Attack(target) },
                { MonsterAction.Defence, (target) => Defence() },
                { MonsterAction.Heal, (target) => Heal() }
            };
        }

        public virtual void AIAction(IDamageable target)
        {
            // 몬스터 공통 액션
            // 공격 - 자신의 공격력 만큼 데미지
            // 방어 - 공격을 받을시 데미지 0
            // 회복 - 체력이 50퍼 이하면 체력을 회복할 확률업

            if (!IsAlive) return;

            int randomAction = random.Next(1, 101);
            MonsterAction selectAction = MonsterAction.Done;

            // 체력이 절반 이하일 때 확률표에 따라 각각의 행동 확률 다르게
            if (Hp <= MaxHp / 2)
            {
                float cumulative = 0f; // 골라진 숫자를 비교할 누적 숫자

                for (int i = 0; i < healProbabiliyUp.Length; i++)
                {
                    cumulative += healProbabiliyUp[i];
                    if (randomAction <= cumulative)
                    {
                        // 만약 누적활률을 적용한 행동이 아무행동도 안하는 거거나 그 밖에 알 수 없는 숫자가 되면 행동하지 않는 걸로 결정
                        selectAction = (i < (int)MonsterAction.Done) ? (MonsterAction)i : MonsterAction.Done;
                        break;
                    }
                }
            }
            // 체력이 풀이면 공격, 방어만
            else if (Hp == MaxHp)
            {
                // 0 ~ 1(Defence까지만)
                selectAction = (MonsterAction)random.Next(0, (int)MonsterAction.Heal);
            }
            // 체력이 풀이 아니고, 체력이 절반 이하가 아니면 공격, 방어, 회복 중 하나를 행동
            else
            {
                // 0 ~ 2(Heal까지만)
                selectAction = (MonsterAction)random.Next(0, (int)MonsterAction.Done);
            }

            // 선택된 행동을 찾아 행동 개시
            if (_monsterActions.TryGetValue(selectAction, out var finalAction))
            {
                finalAction.Invoke(target);
            }
            else
            {
                SendMessage($"{Name}이(가) 무엇을 할지 망설이고 있습니다.");
            }
        }
    }
}