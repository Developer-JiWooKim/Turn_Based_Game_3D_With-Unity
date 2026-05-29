using DG.Tweening;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private float _flightSpeed = 20f;

    private System.Threading.Tasks.TaskCompletionSource<bool> _hitCompletionSource;
    private System.Action _onRelease;

    public void Launch(Vector3 targetPosition, 
                       System.Threading.Tasks.TaskCompletionSource<bool> hitCompletionSource, 
                       System.Action onRelease)
    {
        _hitCompletionSource = hitCompletionSource;
        _onRelease = onRelease;

        // 타겟 방향으로 회전
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        // 화살이 날아갈 거리와 속도 계산
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / _flightSpeed;

        transform.DOMove(targetPosition, duration).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"화살 충돌 - other: {other.gameObject.name}, parent: {other.transform.parent?.name}");

        if (other.GetComponent<EnemyUnitView>() == null) return;

        Debug.Log("EnemyUnitView 확인됨, 풀 반납 시작");
        _hitCompletionSource?.SetResult(true);

        transform.DOKill();
        _onRelease?.Invoke();
        gameObject.SetActive(false);
    }
}
