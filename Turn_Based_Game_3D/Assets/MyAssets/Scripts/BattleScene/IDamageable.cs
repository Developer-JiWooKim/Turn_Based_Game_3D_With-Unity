namespace Assets.MyAssets.Scripts.BattleScene
{

public interface IDamageable
{
    int CurrentHp { get; }
    bool IsDead { get;  }

    void TakeDamage(int damage);
}

}
