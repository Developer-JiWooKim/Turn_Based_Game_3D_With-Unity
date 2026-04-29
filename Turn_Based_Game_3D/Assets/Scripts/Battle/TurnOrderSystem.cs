using System.Collections.Generic;
using System.Linq;

public class TurnOrderSystem
{
    private List<BattleUnit> _turnQueue = new List<BattleUnit>();

    // 전투 시작 시 speed 기준으로 정렬
    public void Initialize(List<BattleUnit> allUnits)
    {
        _turnQueue = allUnits
            .OrderByDescending(u => u.Stats.speed)
            .ToList();
    }

    // 현재 턴 유닛 반환
    public BattleUnit GetCurrentUnit()
    {
        return _turnQueue[0];
    }

    // 턴 넘기기 (현재 유닛을 맨 뒤로)
    public void AdvanceTurn()
    {
        BattleUnit current = _turnQueue[0];
        _turnQueue.RemoveAt(0);
        _turnQueue.Add(current);
    }

    // 죽은 유닛 제거
    public void RemoveDeadUnits()
    {
        _turnQueue.RemoveAll(u => u.IsDead);
    }

    // 턴 순서 리스트 반환 (UI 표시용)
    public List<BattleUnit> GetTurnQueue()
    {
        return new List<BattleUnit>(_turnQueue);
    }
}