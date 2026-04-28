using System;

namespace Turn_Based_Game
{
    public class Creature : IDamageable
    {
        // Private
        private string _name;
        private int _hp;
        private int _maxHp;
        private int _atk;
        private int _healAmount;
        private bool _guard;

        // Properties
        public string Name { get { return _name; } protected set { _name = value; } }
        public int Hp
        {
            get => _hp;
            protected set
            {
                if (_hp == value) return;

                _hp = Math.Clamp(value, 0, _maxHp);

                NotifyHpChanged();

                if (_hp <= 0 && IsAlive)
                {
                    Die();
                }
            }
        }
        public int MaxHp { get { return _maxHp; } protected set { _maxHp = value; } }
        public int Atk { get { return _atk; } protected set { _atk = value; } }
        public int HealAmount { get { return _healAmount; } protected set { _healAmount = value; } }

        public bool Guard { get { return _guard; } set { _guard = value; } }
        public bool IsAlive { get; set; } = true;

        public int Index { get; set; }  // 몬스터 식별용 인덱스(IDamageable 인터페이스 구현)

        //events
        public event Action<string> OnMessageSent;      // 메세지 출력 이벤트
        public event Action<int, int> OnHpChanged;      // HP가 변했을 때 발생하는 이벤트(현재, 최대)
        public event Action OnDamaged;                  // 공격 받았을 때 발생하는 시작 연출
        public event Action OnDead;                     // 사망 시 발생하는 이벤트

        public Creature(string name, int hp, int atk, int healAmount)
        {
            // 생성될 때 초기값 설정
            _name = name;
            _hp = MaxHp = hp;
            _atk = atk;
            _healAmount = healAmount;

            Guard = false;  // 처음 생성되면 가드 비활성화
        }

        // 자식 클래스들이 메시지를 보낼 때 사용할 전용 메서드
        protected void SendMessage(string message) => OnMessageSent?.Invoke(message);

        protected void NotifyHpChanged() => OnHpChanged?.Invoke(Hp, MaxHp);

        protected void NotifyDamaged() => OnDamaged?.Invoke();

        protected void NotifyDead() => OnDead?.Invoke();

        public virtual void Attack(IDamageable target)
        {
            SendMessage($"{Name} 이/가 {Atk}의 위력으로 공격");

            target.TakeDamage(Atk);
        }

        public void Defence()
        {
            Guard = true;
            SendMessage($"{Name} 이/가 방어태세를 갖춥니다.");
        }

        public virtual void Heal()
        {
            Hp = Math.Min(Hp + HealAmount, MaxHp);

            SendMessage($"{Name} 이/가 {HealAmount}의 체력을 회복해 체력이 {Hp}가 되었습니다.");
        }

        public virtual void TakeDamage(int damage)
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

            SendMessage($"{Name} 이/가 {damage}의 데미지를 입었습니다.");
            NotifyDamaged();    // 데미지를 받았을 때 수행할 연출(몬스터는 피격 연출)

            // 데미지 적용 및 관련 행동 실행
            Hp -= damage;

        }

        protected virtual void Die()
        {
            if (!IsAlive) return;

            IsAlive = false;

            SendMessage($"{Name} 이/가 쓰러졌습니다.");

            OnDead?.Invoke();                           // 사망 연출 신호 보냄
        }
    }
}