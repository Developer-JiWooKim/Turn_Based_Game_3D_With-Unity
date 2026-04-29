using System;
public abstract class Monster : Creature
{
    protected float[] healProbabiliyUp = { 30f, 30f, 40f }; // 누적확률 용 flaot 배열
    protected static readonly Random random = new Random();

    protected Monster(string _name, int _hp, int _atk, int _healAmount, int _speed) : base(_name, _hp, _atk, _healAmount, _speed)
    {
    }

    /// <summary>
    /// 몬스터 AI 행동 결정 및 실행
    /// </summary>
    public virtual void AIAction(IDamageable target)
    {
        if (!IsAlive) return;

        CheckPassiveSkills();

        if (ActiveSkills.Count > 0)
        {
            // 가지고 있는 스킬을 확률에 따라 사용하게
        }



        //void deleteFunc() TODO#: 삭제 예정
        //{
        //    int randomAction = random.Next(1, 101);

        //    // 체력이 절반 이하일 때 확률표에 따라 각각의 행동 확률 다르게
        //    if (Hp <= MaxHp / 2)
        //    {
        //        float cumulative = 0f; // 골라진 숫자를 비교할 누적 숫자

        //        for (int i = 0; i < healProbabiliyUp.Length; i++)
        //        {
        //            cumulative += healProbabiliyUp[i];
        //            if (randomAction <= cumulative)
        //            {
        //                // 만약 누적활률을 적용한 행동이 아무행동도 안하는 거거나 그 밖에 알 수 없는 숫자가 되면 행동하지 않는 걸로 결정
        //                selectAction = (i < (int)MonsterAction.Done) ? (MonsterAction)i : MonsterAction.Done;
        //                break;
        //            }
        //        }
        //    }
        //}
    }
}