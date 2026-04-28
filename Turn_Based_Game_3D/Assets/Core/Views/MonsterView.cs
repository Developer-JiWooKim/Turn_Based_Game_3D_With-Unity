using UnityEngine;
using UnityEngine.UI;

namespace Turn_Based_Game
{
    public class MonsterView : MonoBehaviour
    {
        private Monster monster;

        [SerializeField] private Slider hpSlider;
        [SerializeField] private Text monsterNameText;
        [SerializeField] private Animator animator;

        public Monster Monster => monster;

        public void Init(Monster monsterData)
        {
            monster = monsterData;

            // UI 초기화
            if (monsterNameText != null)
            {
                monsterNameText.text = monster.Name;
            }

            UpdateHpUI();

            // 이벤트 구독
            monster.OnHpChanged += OnMonsterHpChanged;
            monster.OnDamaged += OnMonsterDamaged;
            monster.OnDead += OnMonsterDead;
            monster.OnMessageSent += OnMonsterMessage;
        }

        private void UpdateHpUI()
        {
            if (hpSlider != null && monster != null)
            {
                hpSlider.maxValue = monster.MaxHp;
                hpSlider.value = monster.Hp;
            }
        }

        private void OnMonsterHpChanged(int currentHp, int maxHp)
        {
            UpdateHpUI();
        }

        private void OnMonsterDamaged()
        {
            // 피격 애니메이션 재생
            if (animator != null)
            {
                animator.SetTrigger("TakeDamage");
            }

            // 위치 흔들기 등의 연출
            StartCoroutine(DamageEffectCoroutine());
        }

        private void OnMonsterDead()
        {
            // 사망 애니메이션 재생
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }

            // 선택적: 객체 비활성화 또는 삭제
            // Destroy(gameObject, 2f);
        }

        private void OnMonsterMessage(string message)
        {
            // 메시지 표시 (UI 또는 콘솔)
            Debug.Log($"[{monster.Name}] {message}");
        }

        private System.Collections.IEnumerator DamageEffectCoroutine()
        {
            Vector3 originalPos = transform.localPosition;
            float shakeDuration = 0.1f;
            float shakeAmount = 0.1f;

            float elapsed = 0f;
            while (elapsed < shakeDuration)
            {
                transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPos;
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (monster != null)
            {
                monster.OnHpChanged -= OnMonsterHpChanged;
                monster.OnDamaged -= OnMonsterDamaged;
                monster.OnDead -= OnMonsterDead;
                monster.OnMessageSent -= OnMonsterMessage;
            }
        }
    }
}
