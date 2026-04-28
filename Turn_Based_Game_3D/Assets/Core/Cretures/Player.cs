using System;

namespace Turn_Based_Game
{
    public class Player : Creature
    {
        #region === Enums ===

        public enum PlayerAction
        {
            NormalAttack,
            SplashAttack,
            EnhancedAttack,
            Defence,
            Heal,
            UseItem,
            RunAway,
            Fight  // 서브메뉴 진입용
        }

        #endregion

        #region === Constants ===

        private const int MAX_STAMINA = 50;
        private const int SPLASH_DAMAGE = 10;
        private const int ENHANCED_ATTACK_DAMAGE = 30;

        #endregion

        #region === Fields & Properties ===

        private int _maxStamina;
        private int _stamina;

        public int Stamina
        {
            get => _stamina;
            private set
            {
                if (_stamina == value) return;

                _stamina = Math.Clamp(value, 0, _maxStamina);
                OnStaminaChanged?.Invoke(_stamina, _maxStamina);
            }
        }
        public int MaxStamina { get => _maxStamina; }

        #endregion

        #region === Events ===

        // 기존 이벤트
        public event Action<int, int> OnStaminaChanged;

        // 실패 이벤트
        public event Action<string> OnActionFailed;


        #endregion

        #region === Constructor ===

        public Player(string name, int hp, int atk, int healAmount) : base(name, hp, atk, healAmount)
        {
            Stamina = _maxStamina = MAX_STAMINA;
        }

        #endregion

        #region === Public Methods ===

        /// <summary>
        /// 플레이어 행동 실행 (이벤트 발생)
        /// </summary>
        public void ExecuteAction(PlayerAction action, object target = null)
        {
            if (!IsAlive) return;

            switch (action)
            {
                case PlayerAction.NormalAttack:
                    NormalAttack((IDamageable)target);
                    break;

                case PlayerAction.SplashAttack:
                    SplashAttack((IDamageable[])target);
                    break;

                case PlayerAction.EnhancedAttack:
                    EnhancedAttack((IDamageable)target);
                    break;

                case PlayerAction.Defence:
                    Defence();
                    break;

                case PlayerAction.Heal:
                    Heal();
                    break;

                case PlayerAction.RunAway:
                    RunAway();
                    break;

                case PlayerAction.Fight:
                    // Fight는 서브메뉴 진입용이므로 실제 행동 없음
                    break;
            }
        }

        #endregion

        #region === Attack Actions ===

        /// <summary>
        /// 단일 공격 실행
        /// </summary>
        private void NormalAttack(IDamageable target)
        {
            if (target == null) return;

            // 실제 공격
            Attack(target);
        }

        /// <summary>
        /// 전체 공격 실행
        /// </summary>
        private void SplashAttack(IDamageable[] targets)
        {
            int consumptionStamina = 20;

            if (targets == null || targets.Length == 0)
            {
                OnActionFailed?.Invoke("공격 대상이 없습니다.");
                return;
            }

            // 스테미나 체크
            if (_stamina < consumptionStamina)
            {
                SendMessage("스테미나가 부족해 스킬 사용에 실패했습니다.");
                OnActionFailed?.Invoke("스테미나 부족");
                return;
            }

            // 스테미나 소모
            Stamina -= consumptionStamina;
            SendMessage($"{Name}의 광역 공격! (스테미나 -{consumptionStamina})");

            // 실제 공격
            foreach (IDamageable target in targets)
            {
                target.TakeDamage(SPLASH_DAMAGE);
            }
        }

        /// <summary>
        /// 강화 공격 실행
        /// </summary>
        private void EnhancedAttack(IDamageable target)
        {
            int consumptionStamina = 15;

            if (target == null)
            {
                OnActionFailed?.Invoke("공격 대상이 없습니다.");
                return;
            }

            // 스테미나 체크
            if (_stamina < consumptionStamina)
            {
                SendMessage("스테미나가 부족해 스킬 사용에 실패했습니다.");
                OnActionFailed?.Invoke("스테미나 부족");
                return;
            }

            // 스테미나 소모
            Stamina -= consumptionStamina;
            SendMessage($"{Name}의 강화된 일격! (스테미나 -{consumptionStamina})");

            // 실제 공격
            target.TakeDamage(ENHANCED_ATTACK_DAMAGE);
        }

        #endregion

        #region === Other Actions ===

        /// <summary>
        /// 도망 실행
        /// </summary>
        private void RunAway()
        {
            IsAlive = false; 

            SendMessage("전투에서 도망쳤습니다!");

            NotifyDead();
        }

        #endregion

        #region === Stamina Management ===

        /// <summary>
        /// 스테미나 자연회복
        /// </summary>
        public void StaminaRecover()
        {
            int recoverAmount = 5;

            if (_stamina < _maxStamina)
            {
                Stamina += recoverAmount;
                SendMessage($"스테미나가 {recoverAmount} 회복되었습니다. (현재: {_stamina}/{_maxStamina})");
            }
        }

        /// <summary>
        /// 스테미나 포션 사용 TODO#: 아이템 시스템 구현 후 사용될 메소드
        /// </summary>
        public void StaminaRecover(int amount)
        {
            Stamina += amount;
            SendMessage($"스테미나 포션 사용! 스테미나가 {amount} 회복되었습니다. (현재: {_stamina}/{_maxStamina})");
        }

        /// <summary>
        /// 스테미나 완전 회복
        /// </summary>
        public void StaminaInit()
        {
            _stamina = MAX_STAMINA;
            SendMessage($"스테미나가 완전히 회복되었습니다! ({_stamina}/{_maxStamina})");
        }

        #endregion
    }
}