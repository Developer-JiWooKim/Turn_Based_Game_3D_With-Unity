using System;
public class PlayerData : Creature
{

    private const int MAX_STAMINA = 50; // TODO#: 각 개체마다 스테미나를 구성하고 삭제 예정


    private int _maxStamina;
    private int _stamina;

    public int Stamina
    {
        get => _stamina;
        private set
        {
            if (_stamina == value) return;

            _stamina = Math.Clamp(value, 0, _maxStamina);
            OnStaminaChanged?.Invoke(_stamina, _maxStamina);
        }
    }
    public int MaxStamina { get => _maxStamina; }



    public event Action<int, int> OnStaminaChanged;
    public event Action<string> OnActionFailed;



    public PlayerData(string name, int hp, int atk, int healAmount, int speed) : base(name, hp, atk, healAmount, speed)
    {
        Stamina = _maxStamina = MAX_STAMINA; // TODO#: 각 개체마다 스테미나를 구성하고 삭제 예정
    }

    public void UseSkill(IActiveSkill skill, IDamageable[] targets)
    {
        if (!IsAlive) return;

        if (!skill.CanUse(this))
        {
            OnActionFailed?.Invoke($"{skill.Name} 사용 불가");
            return;
        }

        if (skill.StaminaCost > 0)
        {
            Stamina -= skill.StaminaCost;
            SendMessage($"스태미나 -{skill.StaminaCost} (현재: {Stamina}/{MaxStamina})"); // TODO#: 스테미나 UI 업데이트 이벤트 추가 예정
        }

        skill.Execute(this, targets);
    }




    /// <summary>
    /// 도망 실행
    /// </summary>
    public void RunAway()
    {
        IsAlive = false;

        SendMessage("전투에서 도망쳤습니다!");

        NotifyDead();
    }



    /// <summary>
    /// 스테미나 자연회복
    /// </summary>
    public void StaminaRecover()
    {
        int recoverAmount = 5; // TODO#: 스테미나 회복량 조정 필요

        if (_stamina < _maxStamina)
        {
            Stamina += recoverAmount;
            SendMessage($"스테미나가 {recoverAmount} 회복되었습니다. (현재: {_stamina}/{_maxStamina})");
        }
    }

    /// <summary>
    /// 스테미나 포션 사용 TODO#: 아이템 시스템 구현 후 사용될 메소드
    /// </summary>
    public void StaminaRecover(int amount)
    {
        Stamina += amount;
        SendMessage($"스테미나 포션 사용! 스테미나가 {amount} 회복되었습니다. (현재: {_stamina}/{_maxStamina})");
    }

    /// <summary>
    /// 스테미나 완전 회복
    /// </summary>
    public void StaminaInit()
    {
        _stamina = MAX_STAMINA;
        SendMessage($"스테미나가 완전히 회복되었습니다! ({_stamina}/{_maxStamina})");
    }
}