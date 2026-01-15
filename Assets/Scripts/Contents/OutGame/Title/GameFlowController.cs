using Sc.Common.UI;
using Sc.Event.OutGame;
using Sc.Foundation;
using UnityEngine;

namespace Sc.Contents.Title
{
    /// <summary>
    /// 게임 흐름 제어 - 초기화 완료 후 화면 전환 처리
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
        [Header("설정")]
        [SerializeField] private bool _autoStartOnInitialized = true;

        private void OnEnable()
        {
            EventManager.Instance?.Subscribe<GameInitializedEvent>(OnGameInitialized);
        }

        private void OnDisable()
        {
            if (EventManager.HasInstance)
            {
                EventManager.Instance.Unsubscribe<GameInitializedEvent>(OnGameInitialized);
            }
        }

        private void OnGameInitialized(GameInitializedEvent evt)
        {
            if (!evt.IsSuccess)
            {
                Debug.LogError($"[GameFlowController] Game initialization failed: {evt.ErrorMessage}");
                return;
            }

            if (_autoStartOnInitialized)
            {
                StartGame();
            }
        }

        /// <summary>
        /// TitleScreen으로 게임 시작
        /// </summary>
        public void StartGame()
        {
            Debug.Log("[GameFlowController] Starting game - pushing TitleScreen");

            if (NavigationManager.Instance == null)
            {
                Debug.LogError("[GameFlowController] NavigationManager not found!");
                return;
            }

            TitleScreen.Open(new TitleState());
        }
    }
}
