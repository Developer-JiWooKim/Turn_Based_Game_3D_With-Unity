using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class UnitHUD : MonoBehaviour
{        
    [SerializeField] private Image           _hpBarFill;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private GameObject      _damagePopupPrefab;

    private DamagePopup     _damagePopup;
    private RectTransform   _rectTransform;

    private void Awake() => Initialize();
    private void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();

        GameObject popup = Instantiate(_damagePopupPrefab, transform.position, Quaternion.identity);
        _damagePopup = popup.GetComponent<DamagePopup>();
        _damagePopup.gameObject.SetActive(false);
    }

    private void LateUpdate() => TowardCamera();
    private void TowardCamera()
    {
        _rectTransform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void SetUnitName(string unitName)
    {
        _nameText.text = unitName;
    }

    public void UpdateHp(int currentHp, int maxHp)
    {

        Debug.Log($"UnitHUD.UpdateHp - {currentHp}/{maxHp}");

        float ratio = (float)currentHp / maxHp;

        _hpBarFill.fillAmount = ratio;

        _hpBarFill.color = 
            ratio > 0.5f    ?   Color.green :  // 체력이 절반 이상 -> 초록색
            ratio > 0.25f   ?   Color.yellow : // 1/4 이상일때 -> 노란색
                                Color.red;     // 그 이하는 빨간색 

        _hpText.text = $"{currentHp}/{maxHp}";
    }

    public void ShowDamagePopup(int damage)
    {
        if (_damagePopupPrefab == null) return;

        _damagePopup.transform.position = transform.position + Vector3.up;
        _damagePopup.ShowDamagePopup(damage);
    }

    public void Reset()
    {
        _nameText.text = "";
        _hpBarFill.fillAmount = 1f;
        _hpBarFill.color = Color.green;
        _hpText.text = "";
    }
}

}
