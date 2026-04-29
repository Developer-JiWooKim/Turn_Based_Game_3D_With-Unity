using System;
using System.Collections.Generic;


public class Creature : IDamageable
{
    // 기본 스탯 및 상태
    private string _name;
    private int _healAmount;    // TODO#: 아마 삭제 예정
    private bool _guard;        // TODO#: 아마 삭제 예정
    private int _hp;
    private int _maxHp;
    private int _atk;
    private int _speed;

    public string Name { get { return _name; } protected set { _name = value; } }
    public int Hp
    {
        get => _hp;
        protected set
        {
            if (_hp == value) return;           // 값이 변하지 않으면 아무 작업도 하지 않음

            _hp = Math.Clamp(value, 0, _maxHp); // HP가 0 미만으로 떨어지지 않도록 보장

            NotifyHpChanged();                  // HP가 변경되었음을 알리는 이벤트 발생

            if (_hp <= 0 && IsAlive)            // HP가 0 이하로 떨어졌고 아직 사망하지 않은 경우 사망 처리
            {
                Die();
            }
        }
    }
    public int MaxHp { get { return _maxHp; } protected set { _maxHp = value; } }
    public int Atk { get { return _atk; } protected set { _atk = value; } }
    public int HealAmount { get { return _healAmount; } protected set { _healAmount = value; } } // TODO#: 아마 삭제 예정
    public bool Guard { get { return _guard; } set { _guard = value; } }                         // TODO#: 아마 삭제 예정
    public bool IsAlive { get; set; } = true;
    public int Index { get; set; }  // 몬스터 식별용 인덱스(IDamageable 인터페이스 구현) TODO#: 아마 삭제 예정
    public int Speed { get { return _speed; } protected set { _speed = value; } }

    //스킬 관련
    protected List<IActiveSkill> _activeSkills;
    protected List<IPassiveSkill> _passiveSkills;
    public IReadOnlyList<IActiveSkill> ActiveSkills => _activeSkills;
    public IReadOnlyList<IPassiveSkill> PassiveSkills => _passiveSkills;


    //이벤트들
    public event Action<string> OnMessageSent;      // TODO#: 현재 게임에서 전투 로그를 발생시키는 이벤트를 할 것인가?
    public event Action<int, int> OnHpChanged;      // HP가 변했을 때 발생하는 이벤트(현재, 최대)
    public event Action OnDamaged;                  // 공격 받았을 때 발생하는 시작 연출
    public event Action OnDead;                     // 사망 시 발생하는 이벤트

    public Creature(string name, int hp, int atk, int healAmount, int speed)
    {
        // 생성될 때 초기값 설정
        _name = name;
        _hp = MaxHp = hp;
        _atk = atk;
        _healAmount = healAmount;
        _speed = speed;

        Guard = false;  // 처음 생성되면 가드 비활성화
    }

    // 자식 클래스들이 메시지를 보낼 때 사용할 전용 메서드
    protected void SendMessage(string message) => OnMessageSent?.Invoke(message);

    protected void NotifyHpChanged() => OnHpChanged?.Invoke(Hp, MaxHp);

    protected void NotifyDamaged() => OnDamaged?.Invoke();

    protected void NotifyDead() => OnDead?.Invoke();

    protected void AddActiveSkill(IActiveSkill skill)
    {
        _activeSkills?.Add(skill);
    }
    protected void AddPassiveSkill(IPassiveSkill skill)
    {
        _passiveSkills?.Add(skill);
    }

    public void CheckPassiveSkills()
    {
        foreach (var passive in PassiveSkills)
        {
            passive.TryApply(this);
        }
    }

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
        if (!IsAlive) return;

        // TODO#: 삭제 예정
        if (Guard)
        {
            SendMessage($"{Name}이(가) 공격을 완벽히 방어했습니다!");
            Guard = false;  // 방어 성공 시 해제
            return;
        }

        // TODO#: 현재는 데미지를 입기전 메세지를 출력하는데 이 부분을 전투로그로 기록하고 데미지 입는 연출을 바로 수행하는걸로 바꿀듯
        SendMessage($"{Name} 이/가 {damage}의 데미지를 입었습니다.");

        //TODO#: 현재는 데미지를 입기전 메세지를 출력하는데 이 부분을 전투로그로 기록하고 데미지 입는 연출을 바로 수행하는걸로 바꿀듯
        // 데미지를 입는 연출을 수행하는 이벤트 발생, 피격시 애니메이션, 피격 효과음, 화면 흔들림 등 다양한 연출을 이 이벤트에 구독하여 구현할 수 있음
        NotifyDamaged();    // 데미지를 받았을 때 수행할 연출, 

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