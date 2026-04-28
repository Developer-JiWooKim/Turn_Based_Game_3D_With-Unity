using System;

namespace Turn_Based_Game
{
    public class MenuAction
    {
        public string Label { get; }
        public string DisabledReason { get; } // 비활성화 이유 (스태미나 부족 등)

        private readonly Func<bool> _execute;
        private readonly Func<bool> _canExecute; // null이면 항상 실행 가능

        public MenuAction(string label, Func<bool> execute, Func<bool> canExecute = null, string disabledReason = "")
        {
            Label = label;
            DisabledReason = disabledReason;
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute() => _canExecute == null || _canExecute();

        public bool Execute()
        {
            if (!CanExecute()) return false; // 실행 불가면 false
            return _execute();
        }
    }
}