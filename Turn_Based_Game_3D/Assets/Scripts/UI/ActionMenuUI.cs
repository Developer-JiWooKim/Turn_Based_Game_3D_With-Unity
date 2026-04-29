using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionMenuUI : MonoBehaviour
{
    [Header("메뉴 버튼")]
    public Button attackButton;
    public Button skillButton;
    public Button itemButton;

    [Header("스킬 목록 패널")]
    public GameObject skillPanel;
    public Transform skillButtonContainer;
    public GameObject skillButtonPrefab;

    [Header("타겟 선택 패널")]
    public GameObject targetPanel;
    public Transform targetButtonContainer;
    public GameObject targetButtonPrefab;

    private SkillDataSO _selectedSkill;

    private void Start()
    {
        attackButton.onClick.AddListener(OnAttackPressed);
        skillButton.onClick.AddListener(OnSkillPressed);
        itemButton.onClick.AddListener(OnItemPressed);

        skillPanel.SetActive(false);
        targetPanel.SetActive(false);

        // BattleManager 이벤트 구독
        BattleManager.Instance.OnTurnStart += HandleTurnStart;
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance == null) return;
        BattleManager.Instance.OnTurnStart -= HandleTurnStart;
    }

    // 턴 시작 시 플레이어 턴이면 메뉴 활성화
    private void HandleTurnStart(BattleUnit unit)
    {
        gameObject.SetActive(unit.IsPlayer);
    }

    // ── 공격 버튼 ────────────────────────────────────────
    private void OnAttackPressed()
    {
        _selectedSkill = null;
        ShowTargetPanel(BattleManager.Instance.EnemyUnits);
    }

    // ── 스킬 버튼 ────────────────────────────────────────
    private void OnSkillPressed()
    {
        skillPanel.SetActive(true);
        targetPanel.SetActive(false);

        // 기존 버튼 제거
        foreach (Transform child in skillButtonContainer)
            Destroy(child.gameObject);

        // 스킬 버튼 동적 생성
        // 추후 캐릭터별 스킬 목록으로 교체
        // 임시로 BattleManager playerDataArray[0] 스킬 사용
    }

    private void OnItemPressed()
    {
        Debug.Log("아이템 기능 추후 구현");
    }

    // ── 타겟 패널 ────────────────────────────────────────
    private void ShowTargetPanel(System.Collections.Generic.List<BattleUnit> targets)
    {
        targetPanel.SetActive(true);
        skillPanel.SetActive(false);

        // 기존 버튼 제거
        foreach (Transform child in targetButtonContainer)
            Destroy(child.gameObject);

        // 타겟 버튼 동적 생성
        foreach (var target in targets)
        {
            if (target.IsDead) continue;

            GameObject btn = Instantiate(targetButtonPrefab, targetButtonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = target.UnitName;

            BattleUnit captured = target;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                BattleManager.Instance.ExecutePlayerAction(captured, _selectedSkill);
                targetPanel.SetActive(false);
                gameObject.SetActive(false);
            });
        }
    }
}