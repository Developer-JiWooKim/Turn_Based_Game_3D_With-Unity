public interface IDamageable
{
    bool IsAlive { get; set; }
    /// <summary>
    /// TODO#: 몬스터 식별용 인덱스에 대한 논의
    /// 몬스터 식별용 인덱스 TODO: 굳이 식별용 인덱스가 필요한가?
    /// 이 부분은 CLI기반에서는 몬스터의 위치를 파악하기 위해 필요했지만
    /// 3D 게임에서는 몬스터의 위치를 파악하기 위해
    /// 인덱스보다는 Transform이나 Collider를 활용하는 것이 더 적합할 수 있다.
    /// 따라서 이 부분은 게임의 구조와 요구사항에 따라 달라질 수 있다.
    /// </summary>
    int Index { get; set; } 
    void TakeDamage(int damage);
}
