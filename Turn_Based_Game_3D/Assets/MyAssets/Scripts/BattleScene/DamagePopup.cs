using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _damageText;

    private float _duration = 0.8f;

    private void LateUpdate()
    {
        // 카메라를 바라보도록 회전
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void ShowDamagePopup(int damage)
    {
        gameObject.SetActive(true);

        _damageText.text = damage.ToString();
        _damageText.color = Color.white;
        _damageText.alpha = 1f;

        // 위로 올라가면서 사라지는 연출
        transform.DOMove(transform.position + Vector3.up, _duration)
            .SetEase(Ease.OutQuad);

        _damageText.DOFade(0f, _duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }
}

}
