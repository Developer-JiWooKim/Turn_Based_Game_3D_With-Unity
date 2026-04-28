using System;
using System.Collections.Generic;
using Turn_Based_Game;

public class FightMenu : MenuBase
{
    private readonly Player _player;
    private readonly Func<IDamageable[]> _getAliveTargets;

    public FightMenu(Player player, Func<IDamageable[]> getAliveTargets, Stack<MenuBase> menuStack) : base(menuStack)
    {
        _player = player;
        _getAliveTargets = getAliveTargets;
        Init();
    }

    public override void Init()
    {
        AddMenu(new("단일 공격", () => {
            IDamageable target = UIManager.SelectTarget(_getAliveTargets());
            _player.ExecuteAction(Player.PlayerAction.NormalAttack, target);
            return true;
        }));

        AddMenu(new("전체 공격", () => {
            _player.ExecuteAction(Player.PlayerAction.SplashAttack, _getAliveTargets());
            return true;
        },
        canExecute: () => _player.Stamina >= 20,
        disabledReason: "ST 부족"));

        AddMenu(new("강화 공격", () => {
            IDamageable target = UIManager.SelectTarget(_getAliveTargets());
            _player.ExecuteAction(Player.PlayerAction.EnhancedAttack, target);
            return true;
        },
        canExecute: () => _player.Stamina >= 15,
        disabledReason: "ST 부족"));

        AddMenu(new("뒤로가기", () => { _menuStack.Pop(); return false; }));
    }
}