using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    [Header("캐릭터 정보")]
    public TextMeshProUGUI nameText;
    public Slider hpSlider;
    public Slider mpSlider;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;

    private BattleUnit _unit;

    public void Initialize(BattleUnit unit)
    {
        _unit = unit;
        nameText.text = unit.UnitName;
        Refresh();
    }

    public void Refresh()
    {
        if (_unit == null) return;

        hpSlider.maxValue = _unit.Stats.maxHP;
        hpSlider.value = _unit.Stats.currentHP;
        mpSlider.maxValue = _unit.Stats.maxMP;
        mpSlider.value = _unit.Stats.currentMP;

        hpText.text = $"{_unit.Stats.currentHP} / {_unit.Stats.maxHP}";
        mpText.text = $"{_unit.Stats.currentMP} / {_unit.Stats.maxMP}";
    }
}