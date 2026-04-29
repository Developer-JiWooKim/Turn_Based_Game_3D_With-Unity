using System;

/// <summary>
/// UIManager 스텁 클래스
/// 실제 UI 구현은 나중에 완성해주세요
/// </summary>
public static class UIManager
{
    public static IDamageable SelectTarget(IDamageable[] targets)
    {
        // TODO: 실제 UI 선택 로직 구현
        if (targets != null && targets.Length > 0)
        {
            return targets[0];  // 임시: 첫 번째 타겟 반환
        }
        return null;
    }

    public static void ShowMessage(string message)
    {
        // TODO: 실제 UI 메시지 표시 구현
        Console.WriteLine(message);
    }
}