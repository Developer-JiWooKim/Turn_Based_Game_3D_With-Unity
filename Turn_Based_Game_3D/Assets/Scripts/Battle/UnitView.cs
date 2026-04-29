using UnityEngine;

public class UnitView : MonoBehaviour
{
    [Header("데이터 연결")]
    public CharacterDataSO characterData;  // 플레이어용
    public EnemyDataSO enemyData;          // 적용
    public bool isPlayer;

    // 런타임 유닛 (BattleManager가 생성한 것과 연결)
    public BattleUnit LinkedUnit { get; private set; }

    [Header("연출 위치")]
    public Transform hitEffectPoint;   // 피격 이펙트 생성 위치
    public Transform castEffectPoint;  // 스킬 시전 이펙트 위치

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // BattleManager에서 유닛 생성 후 호출
    public void Initialize(BattleUnit unit)
    {
        LinkedUnit = unit;
    }

    // ── 애니메이션 ───────────────────────────────────────
    public void PlayAttackAnim()
    {
        _animator?.SetTrigger("Attack");
    }

    public void PlayHitAnim()
    {
        _animator?.SetTrigger("Hit");
    }

    public void PlayDeathAnim()
    {
        _animator?.SetTrigger("Death");
    }

    public void PlaySkillAnim(string triggerName)
    {
        if (!string.IsNullOrEmpty(triggerName))
            _animator?.SetTrigger(triggerName);
    }

    // ── 이펙트 ───────────────────────────────────────────
    public void SpawnHitEffect(GameObject vfxPrefab)
    {
        if (vfxPrefab == null) return;
        Transform point = hitEffectPoint != null ? hitEffectPoint : transform;
        Instantiate(vfxPrefab, point.position, Quaternion.identity);
    }
}