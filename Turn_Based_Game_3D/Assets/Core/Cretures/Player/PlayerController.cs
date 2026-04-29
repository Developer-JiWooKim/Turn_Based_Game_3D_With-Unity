using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerData _playerData;
    private Animator _animator;

    public PlayerData PlayerData => _playerData;

    public void Init(PlayerData playerData)
    {
        _animator = GetComponent<Animator>();
        _playerData = playerData;

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _playerData.OnDamaged += HandleDamaged;
        _playerData.OnDead += HandleDead;
    }

    private void UnsubscribeEvents()
    {
        _playerData.OnDamaged -= HandleDamaged;
        _playerData.OnDead -= HandleDead;
    }

    private void HandleDamaged()
    {
        // 피격 애니메이션
        PlayAnimation("Hit");
    }

    private void HandleDead()
    {
        // 사망 애니메이션
        PlayAnimation("Die");
    }

    public void PlayAnimation(string stateName)
    {
        _animator?.Play(stateName);
    }

    public void UseSkill(IActiveSkill skill, IDamageable[] targets)
    {
        PlayAnimation("Attack");
        
        _playerData.UseSkill(skill, targets);
    }

    public void RunAway()
    {
        _playerData.RunAway();
    }

    private void OnDestroy()
    {
        if (_playerData != null)
        {
            UnsubscribeEvents();
        }
    }
}
