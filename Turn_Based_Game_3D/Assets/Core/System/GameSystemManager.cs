using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
public class GameSystemManager : MonoBehaviour
{
    #region === 필드 및 프로퍼티 ===

    public int CurrentTurn { get; private set; }
    public int CurrentStageLevel { get; private set; }

    private bool _isGameOver;
    private CancellationTokenSource _battleCts;

    private PlayerData _player;
    private List<Monster> _monsters;

    // 메뉴
    //private Stack<MenuBase> _menuStack;
    //private MainMenu _mainMenu;
    //private FightMenu _fightMenu;

    // MonsterSpawner 참조
    [SerializeField] private MonsterSpawner monsterSpawner;

    #endregion

    #region === 이벤트 ===

    public event Action<int, PlayerData, Monster[]> OnStageStarted;
    public event Action OnStageCleared;
    public event Action OnGameOver;

    public event Action<int> OnTurnStarted;
    public event Action OnPlayerTurnStarted;
    public event Action OnMonsterTurnStarted;
    public event Action OnTurnEnded;

    #endregion

    #region === 초기화 ===

    public void Initialize(PlayerData player)
    {
        _player = player;
        CurrentTurn = 1;
        CurrentStageLevel = 1;
        _isGameOver = false;

        //// 메뉴 초기화
        //_menuStack = new Stack<MenuBase>();

        //Func<IDamageable[]> aliveTargets = () => _monsters?.Where(m => m.IsAlive).Cast<IDamageable>().ToArray() ?? Array.Empty<IDamageable>();

        //_fightMenu = new FightMenu(_player, aliveTargets, _menuStack);
        //_mainMenu = new MainMenu(_player, _fightMenu, _menuStack);

        SubscribePlayerEvents();
    }

    #endregion

    #region === 이벤트 구독 ===

    private void SubscribePlayerEvents()
    {
        //_player.OnActionFailed += HandleActionFailed;
        //_player.OnDead += HandlePlayerDead;
    }

    #endregion

    #region === 이벤트 핸들러 ===

    private void HandleActionFailed(string reason) { }

    private void HandlePlayerDead()
    {
        _isGameOver = true;
        _battleCts?.Cancel();
    }

    #endregion

    #region === 게임 전체 흐름 ===

    public void RunGame()
    {
        while (!_isGameOver)
        {
            StartStage();
        }

        OnGameOver?.Invoke();
    }

    #endregion

    #region === 스테이지 관리 ===

    private void StartStage()
    {
        SpawnMonsters();

        OnStageStarted?.Invoke(CurrentStageLevel, _player, _monsters.ToArray());
        Thread.Sleep(1000);

        RunBattle();

        HandleStageResult();
    }

    private void SpawnMonsters()
    {
        if (monsterSpawner == null)
        {
            Debug.LogError("MonsterSpawner가 할당되지 않았습니다!");
            return;
        }

        _monsters = monsterSpawner.SpawnEnemies().ToList();
    }

    private void RunBattle()
    {
        CurrentTurn = 1;
        _battleCts = new CancellationTokenSource();

        while (!_battleCts.Token.IsCancellationRequested && _monsters.Any(mon => mon.IsAlive))
        {
            ExecuteTurn(_battleCts.Token);
        }
    }

    private void HandleStageResult()
    {
        //if (_player.IsAlive)
        //{
        //    OnStageCleared?.Invoke();
        //    Thread.Sleep(1500);

        //    CurrentStageLevel++;
        //    _player.StaminaInit();

        //    Thread.Sleep(1500);
        //}
        //else
        //{
        //    _isGameOver = true;
        //}
    }

    #endregion

    #region === 턴 관리 ===

    private void ExecuteTurn(CancellationToken token)
    {
        OnTurnStarted?.Invoke(CurrentTurn);
        Thread.Sleep(500);

        PlayerTurn();

        if (token.IsCancellationRequested) return;
        if (!_monsters.Any(mon => mon.IsAlive)) return;

        MonsterTurn(token);

        if (token.IsCancellationRequested) return;

        EndTurn();
    }

    private void EndTurn()
    {
        //_player.StaminaRecover();

        OnTurnEnded?.Invoke();

        CurrentTurn++;
    }

    #endregion

    #region === 플레이어 턴 ===

    private void PlayerTurn()
    {
        OnPlayerTurnStarted?.Invoke();

        //// 매 턴 스택 초기화 후 메인 메뉴 push
        //_menuStack.Clear();
        //_menuStack.Push(_mainMenu);

        //while (_menuStack.Count > 0)
        //{
        //    // TODO: UIManager를 구현하여 아래 코드를 활성화하세요
        //    // MenuAction selected = UIManager.SelectMenu(_menuStack.Peek().Actions);

        //    // if (!selected.CanExecute())
        //    // {
        //    //     UIManager.ShowMessage(selected.DisabledReason);
        //    //     continue;
        //    // }

        //    // if (selected.Execute())
        //    // {
        //    //     Thread.Sleep(500);
        //    //     return;
        //    // }

        //    // 임시 구현: 첫 번째 메뉴 선택
        //    MenuAction firstAction = _menuStack.Peek().Actions[0];
        //    if (firstAction.CanExecute())
        //    {
        //        if (firstAction.Execute())
        //        {
        //            Thread.Sleep(500);
        //            return;
        //        }
        //    }
        //}
    }

    #endregion

    #region === 몬스터 턴 ===

    private void MonsterTurn(CancellationToken token)
    {
        OnMonsterTurnStarted?.Invoke();

        foreach (var monster in _monsters.Where(m => m.IsAlive))
        {
            if (token.IsCancellationRequested) return;

            //monster.AIAction(_player);
            //Thread.Sleep(800);
        }
    }

    #endregion

    #region === 접근자 ===

    public Monster[] GetCurrentMonsters() => _monsters.ToArray();
    public PlayerData GetPlayer() => _player;

    #endregion
}