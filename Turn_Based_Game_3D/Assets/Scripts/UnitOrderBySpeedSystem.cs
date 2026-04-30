using System.Collections.Generic;
using UnityEngine;

public class UnitOrderBySpeedSystem
{
    private List<BattleUnit> _units;
    private BattleUnit _currentUnit;

    private int _currentIndex;

    public BattleUnit CurrentUnit => _currentUnit;


    /// <summary>
    /// 새로운 턴이 시작되면 속도에 따라 유닛들을 정렬하고, 첫 번째 유닛을 현재 턴의 유닛으로 설정하는 초기화 메소드
    /// </summary>
    public void Initialize(List<BattleUnit> units)
    {
        _units = units;

        // 속도에 따라 유닛들을 정렬할 때 만약 속도가 같으면 랜덤으로 순서를 결정
        _units.Sort((a, b) => b.Speed.CompareTo(a.Speed) == 0 ? Random.Range(-1, 2) : b.Speed.CompareTo(a.Speed));

        _currentIndex = 0;

        _currentUnit = _units[_currentIndex];
    }

    /// <summary>
    /// 현재 유닛을 반환하는 메소드, 만약 유닛 리스트가 비어있다면 예외 처리를 통해 null을 반환
    /// </summary>
    public BattleUnit GetCurrentUnit()
    {
        try
        {
            return CurrentUnit;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"BattleUnit 리스트에 아무것도 없음: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 현재 유닛이 행동을 마치면 다음 유닛으로 인덱스를 증가, 만약 모든 유닛이 행동했다면 false를 반환하여 턴이 종료되었음을 알림
    /// </summary>
    public bool NextUnit()
    {
        _currentIndex++;

        // 모든 유닛이 행동했다면 턴 종료
        if (_currentIndex >= _units.Count)
        {
            return false;   // 이 메소드를 호출하는 쪽에서 턴 증가 및 유닛 리스트 갱신 등의 처리를 할 수 있도록 false 반환
        }

        _currentUnit = _units[_currentIndex]; // 현재 유닛 갱신
        return true;

    }

    public void RemoveUnit(BattleUnit deadUnit)
    {
        int removedIndex = _units.IndexOf(deadUnit);

        _units.Remove(deadUnit);

        // 제거된 유닛이 현재 인덱스보다 앞이면 인덱스 보정
        if (removedIndex < _currentIndex)
        {
            _currentIndex--;
        }
    }
}
