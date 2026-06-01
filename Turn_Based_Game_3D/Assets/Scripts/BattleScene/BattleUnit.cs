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
    
    public int MaxHp => _maxHp;
    public int MaxStamina => _maxStamina;

    public int CurrentHp => _currentHp;
    public int CurrentStamina => _currentStamina;

    public int Speed => _speed;

    public int Atk => _atk;

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
    }

    public BattleUnit(EnemyData data)
    {
        IsPlayer = false;

        _name = data.EnemyName;

        _maxHp = _currentHp = data.enemyStat.Hp;
        _maxStamina = _currentStamina = data.enemyStat.Stamina;

        _speed = data.enemyStat.Speed;
        _atk = data.enemyStat.AttackPower;
    }

    protected void AddBonusStats(int bonusHp, int bonusAtk, int bonusSpd)
    {
        _maxHp      += bonusHp;
        _currentHp  += bonusHp;
        _atk        += bonusAtk;
        _speed      += bonusSpd;
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
}
