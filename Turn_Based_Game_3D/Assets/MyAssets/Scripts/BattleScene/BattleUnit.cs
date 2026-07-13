using System;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class BattleUnit : IDamageable
{
    public enum UnitType
    {
        Player,
        Ally,
        Enemy,
    }

    private int _maxHp;
    private int _maxStamina;

    private int _currentHp;
    private int _currentStamina;

    private string _name;

    private int _speed;

    private int _atk;
    private int _def;

    private int _critRate;
    private int _critDamageBonus;
    private int _resistance;

    public int MaxHp => _maxHp;
    public int MaxStamina => _maxStamina;

    public int CurrentHp => _currentHp;
    public int CurrentStamina => _currentStamina;

    public int Speed => _speed;

    public int Atk => _atk;
    public int Def => _def;

    public int CritRate => _critRate;
    public int CritDamageBonus => _critDamageBonus;
    public int Resistance => _resistance;

    public string Name => _name;

    public bool IsDead => _currentHp <= 0;

    public bool IsPlayer { get; private set; }

    public BattleUnit(PlayerData data)
    {
        IsPlayer = true;

        _name = data.PlayerName;

        _maxHp = _currentHp = data.playerStat.Hp;
        _maxStamina = _currentStamina = data.playerStat.Stamina;

        _speed = data.playerStat.Speed;
        _atk = data.playerStat.AttackPower;
        _def = data.playerStat.DefenseValue;

        _critRate = data.playerStat.CritRate;
        _critDamageBonus = data.playerStat.CritDamageBonus;
        _resistance = data.playerStat.Resistance;
    }

    public BattleUnit(EnemyData data)
    {
        IsPlayer = false;

        _name = data.EnemyName;

        _maxHp = _currentHp = data.enemyStat.Hp;
        _maxStamina = _currentStamina = data.enemyStat.Stamina;

        _speed = data.enemyStat.Speed;
        _atk = data.enemyStat.AttackPower;
        _def = data.enemyStat.DefenseValue;

        _critRate = data.enemyStat.CritRate;
        _critDamageBonus = data.enemyStat.CritDamageBonus;
        _resistance = data.enemyStat.Resistance;
    }

    public virtual void TakeDamage(int damage)
    {
        _currentHp = _currentHp - damage <= 0 ? 0 : _currentHp - damage;
    }

    //TODO#: 사용할지 안할지 모름
    public void Heal(int amount)
    {
        _currentHp = _currentHp + amount >= _maxHp ? _maxHp : _currentHp + amount;
    }

    public void UseStamina(int amount)
    {
        _currentStamina = _currentStamina - amount <= 0 ? 0 : _currentStamina - amount;
    }

    public void RestoreStamina(int amount)
    {
        _currentStamina = _currentStamina + amount >= _maxStamina ? _maxStamina : _currentStamina + amount;
    }

    // 턴 시작 시 스태미나 회복
    public void RecoverStaminaPerTurn(int amount)
    {
        RestoreStamina(amount);
    }

    /// <summary>
    /// 로그라이크 런 버프/파티 시너지/몬스터 디버프를 통일 적용한다. bonus의 부호에 따라 증가(버프)/감소(디버프) 모두 처리.
    /// Hp 변동분은 최대체력과 함께 현재체력에도 즉시 반영된다(버프는 즉시 회복, 디버프는 즉시 감소).
    /// </summary>
    public void ApplyStatBonus(StatData bonus)
    {
        _atk += bonus.AttackPower;
        _speed += bonus.Speed;
        _def += bonus.DefenseValue;
        _resistance += bonus.Resistance;
        _critRate += bonus.CritRate;
        _critDamageBonus += bonus.CritDamageBonus;

        if (bonus.Hp != 0)
        {
            _maxHp += bonus.Hp;
            _currentHp = Math.Clamp(_currentHp + bonus.Hp, 0, _maxHp);
        }
    }

    /// <summary>
    /// 스테이지간 HP 이어받기 등, 현재 체력을 외부 값으로 직접 지정할 때 사용.
    /// </summary>
    public void SetCurrentHp(int hp)
    {
        _currentHp = Math.Clamp(hp, 0, _maxHp);
    }
}

}
