using System;
using System.Collections.Generic;
using Turn_Based_Game;

public class MainMenu : MenuBase
{
    private readonly Player _player;
    private readonly FightMenu _fightMenu;

    public MainMenu(Player player, FightMenu fightMenu, Stack<MenuBase> menuStack) : base(menuStack)
    {
        _player = player;
        _fightMenu = fightMenu;

        Init();
    }

    // 메뉴 부분 수정할 때 이 부분에서
    public override void Init()
    {
        AddMenu( new("싸운다", () => { _menuStack.Push(_fightMenu); return false; }) );

        AddMenu(new("방어", () => {
            _player.ExecuteAction(Player.PlayerAction.Defence, null);
            return true;
        }));

        AddMenu(new("회복", () => {
            _player.ExecuteAction(Player.PlayerAction.Heal, null);
            return true;
        },
        canExecute: () => _player.Hp < _player.MaxHp,
        disabledReason: "Full Hp"));

        AddMenu(new("도망친다", () => {
            _player.ExecuteAction(Player.PlayerAction.RunAway, null);
            return true;
        }));
    }
}