using System;
using System.Collections.Generic;
using Turn_Based_Game;

public abstract class MenuBase
{
    private readonly List<MenuAction> _actions = new(); // 내부적으로 List로 관리
    public MenuAction[] Actions => _actions.ToArray();  // 외부엔 배열로 제공

    protected readonly Stack<MenuBase> _menuStack;

    protected MenuBase(Stack<MenuBase> menuStack)
    {
        _menuStack = menuStack;
    }

    /// <summary>
    /// 메뉴 항목 추가
    /// </summary>
    protected void AddMenu(MenuAction action)
    {
        _actions.Add(action);
    }

    public abstract void Init();
}