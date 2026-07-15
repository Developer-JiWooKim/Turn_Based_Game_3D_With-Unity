using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.MyAssets.Scripts.TitleScene
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private Button _startBtn;
        private Button _optionBtn;
        private Button _quitBtn;

        public event Action OnStartClicked;
        public event Action OnOptionClicked;
        public event Action OnQuitClicked;

        void Awake()
        {
            VisualElement root = _uiDocument.rootVisualElement;
            if (root == null)
            {
                Debug.LogError("TitleUI Awake(): VisualElement root is null");
                return;
            }

            _startBtn = root.Q<Button>("start-btn");
            _optionBtn = root.Q<Button>("option-btn");
            _quitBtn = root.Q<Button>("quit-btn");

            _startBtn.clicked += () => OnStartClicked?.Invoke();
            _optionBtn.clicked += () => OnOptionClicked?.Invoke();
            _quitBtn.clicked += () => OnQuitClicked?.Invoke();
        }
    }
}
