using System;
using UnityEngine.SceneManagement;
using Assets.MyAssets.Scripts.FadeScreenEffect;

namespace Assets.MyAssets.Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        // TODO#: GameManager가 게임 클리어와 게임 오버를 들고 있어야 되는가? 어차피 게임 클리어 제어는 따로 만들어서 관리하는게 좋을듯해보임
        // 게임 매니저의 역할은 씬을 제어, 씬을 넘기면서 비동기로 넘기기 및 페이드 효과를 주기
        // StatePattern을 만들어두는것도 좋아보임
        //     UI 구분 용(옵션, 퍼즈, 결과 등등 ), 씬 구분용(Title, Setting(WeaponSelect -> Setting 씬으로 변경 예정)), 턴제 게임에서 턴관리? 용으로도 쓰일듯
        // 플레이어 입력은 PlayerInput(유니티 내장 컴포넌트)를 사용하는 방식으로 입력을 제어하는 PlayerInputHandlerl.cs를 만들어서 사용할듯

        /// <summary>
        /// 게임 매니저의 역할
        /// 비동기 씬 로드
        /// 씬 전환 전에 페이드 효과(자식으로 FadeController를 가지고 있고 이 오브젝트? 컴포넌트? 를 이용해서 화면 페이드 효과)
        /// 게임 종료(게임 플레이 종료 x, 프로그램 종료)
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void OnDestroy()
        {
            // base.OnDestroy();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public async void LoadScene(string sceneName)
        {
            await FadeController.Instance.FadeOutAsync();
            SceneManager.LoadScene(sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (FadeController.Instance == null) return;

            _ = FadeController.Instance.FadeInAsync();
        }

        public void GameQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }


        // 승리 조건 없음(무한 진행) — 스테이지 클리어 시 항상 다음 전투로 이동
        public void NextStage()
        {
            StageManager.Instance.NextStage();
            LoadScene("BattleScene");
        }
        public event Action OnGameOver;
        public void GameOver() => OnGameOver?.Invoke();
        public void ResetStage()
        {
            StageManager.Instance.ResetStage();
            LoadScene("BattleScene");
        }

        // 202600713 리팩토링 작업 시작
    }
}
