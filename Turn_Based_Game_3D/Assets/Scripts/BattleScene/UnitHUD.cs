using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUD : MonoBehaviour
{
    [SerializeField] private Image           _hpBarFill;
    [SerializeField] private TextMeshProUGUI _nameText;

    public void SetUnitName(string unitName)
    {
        _nameText.text = unitName;
    }

    public void UpdateHp(int currentHp, int maxHp)
    {
        float ratio = (float)currentHp / maxHp;

        _hpBarFill.fillAmount = ratio;

        _hpBarFill.color = 
            ratio > 0.5f    ?   Color.green :  // 체력이 절반 이상 -> 초록색
            ratio > 0.25f   ?   Color.yellow : // 1/4 이상일때 -> 노란색
                                Color.red;     // 그 이하는 빨간색 
    }

    public void ShowDamagePopup(int damage)
    {
        // TODO#: 데미지 팝업 UI 구현 예정
        Debug.Log($"{damage} 데미지!");
    }

    public void Reset()
    {
        _nameText.text = "";
        _hpBarFill.fillAmount = 1f;
        _hpBarFill.color = Color.green;
    }
}
