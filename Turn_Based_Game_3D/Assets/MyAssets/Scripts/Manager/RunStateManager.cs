using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.Manager
{

    /// <summary>
    /// 로그라이크 런 스코프 상태(누적 스탯 버프, 다음 스테이지 몬스터 디버프 예약)를 보유한다.
    /// 게임오버 시 ResetRun()으로 초기화되어 다음 런에는 영향을 주지 않는다.
    /// </summary>
    public class RunStateManager : Singleton<RunStateManager>
    {
        private StatData _runBuffs;

        private bool _pendingEnemyStun;
        private int  _pendingEnemyHpDownPercent;
        private int  _pendingEnemyAtkDownPercent;

        public StatData RunBuffs => _runBuffs;

        public void AddRunBuff(StatData bonus)
        {
            _runBuffs.Hp += bonus.Hp;
            _runBuffs.AttackPower += bonus.AttackPower;
            _runBuffs.DefenseValue += bonus.DefenseValue;
            _runBuffs.Speed += bonus.Speed;
            _runBuffs.CritRate += bonus.CritRate;
            _runBuffs.CritDamageBonus += bonus.CritDamageBonus;
            _runBuffs.Resistance += bonus.Resistance;
        }

        public void QueueEnemyStunNextStage() => _pendingEnemyStun = true;
        public void QueueEnemyHpDownNextStage(int percent) => _pendingEnemyHpDownPercent = percent;
        public void QueueEnemyAtkDownNextStage(int percent) => _pendingEnemyAtkDownPercent = percent;

        // 소비 시 값을 반환하고 예약을 즉시 초기화한다 (1회성 보장).
        public bool ConsumeEnemyStun()
        {
            bool value = _pendingEnemyStun;
            _pendingEnemyStun = false;
            return value;
        }

        public int ConsumeEnemyHpDownPercent()
        {
            int value = _pendingEnemyHpDownPercent;
            _pendingEnemyHpDownPercent = 0;
            return value;
        }

        public int ConsumeEnemyAtkDownPercent()
        {
            int value = _pendingEnemyAtkDownPercent;
            _pendingEnemyAtkDownPercent = 0;
            return value;
        }

        public void ResetRun()
        {
            _runBuffs = default;
            _pendingEnemyStun = false;
            _pendingEnemyHpDownPercent = 0;
            _pendingEnemyAtkDownPercent = 0;
        }
    }

}
