public class BattleUnit : IDamageable
{
    public enum UnitType
    {
        Player,
        Ally,
        Enemy,
    }

    private int _maxHp;
    private int _maxMp;

    private int _currentHp;
    private int _currentMp;

    private string _name;

    private int _speed;

    private int _atk;
    
    public int MaxHp => _maxHp;
    public int MaxMp => _maxMp;

    public int CurrentHp => _currentHp;
    public int CurrentMp => _currentMp;

    public int Speed => _speed;

    public int Atk => _atk;

    public string Name => _name;

    public bool IsDead => _currentHp <= 0;

    public bool IsPlayer { get; private set; }


    public BattleUnit(CharacterData data) 
    {
        IsPlayer = true;

        _name = data.CharacterName;

        _maxHp = _currentHp = data.characterStat.Hp;
        _maxMp = _currentMp = data.characterStat.Mp;

        _speed = data.characterStat.Speed;

        _atk = data.characterStat.AttackPower;
    }
    public BattleUnit(EnemyData data) 
    {
        IsPlayer = false;

        _name = data.EnemyName;

        _maxHp = _currentHp = data.enemyStat.Hp;
        _maxMp = _currentMp = data.enemyStat.Mp;

        _speed = data.enemyStat.Speed;

        _atk = data.enemyStat.AttackPower;
    }

    public virtual void TakeDamage(int damage)
    {
        _currentHp = _currentHp - damage <= 0 ? 0 : _currentHp - damage;
    }

    public void Heal(int amount)
    {
        _currentHp = _currentHp + amount >= _maxHp ? _maxHp : _currentHp + amount;
    }

    public void UseMp(int amount)
    {
        _currentMp = _currentMp - amount <= 0 ? 0 : _currentMp - amount;
    }

    public void RestoreMp(int amount)
    {
        _currentMp = _currentMp + amount >= _maxMp ? _maxMp : _currentMp + amount;
    }
}
