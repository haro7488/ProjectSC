using UnityEngine;
using UnityEngine.UI;
using Sc.Contents.Event;
using Sc.Contents.Shop;
using Sc.Contents.Stage;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 홈 탭 컨텐츠 - 퀵 메뉴 (스테이지, 상점, 이벤트)
    /// </summary>
    public class HomeTabContent : LobbyTabContent
    {
        [Header("UI References")]
        [SerializeField] private Button _stageButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _eventButton;

        private void Awake()
        {
            Debug.Log($"[HomeTabContent] Awake - Stage:{_stageButton != null}, Shop:{_shopButton != null}, Event:{_eventButton != null}");
            
            if (_stageButton != null)
            {
                _stageButton.onClick.AddListener(OnStageClicked);
                Debug.Log($"[HomeTabContent] StageButton interactable:{_stageButton.interactable}, enabled:{_stageButton.enabled}");
            }
            if (_shopButton != null)
            {
                _shopButton.onClick.AddListener(OnShopClicked);
                Debug.Log($"[HomeTabContent] ShopButton interactable:{_shopButton.interactable}, enabled:{_shopButton.enabled}");
            }
            if (_eventButton != null)
            {
                _eventButton.onClick.AddListener(OnEventClicked);
                Debug.Log($"[HomeTabContent] EventButton interactable:{_eventButton.interactable}, enabled:{_eventButton.enabled}");
            }
        }

        private void OnEnable()
        {
            Debug.Log("[HomeTabContent] OnEnable - 탭 활성화됨");
        }

        private void OnDisable()
        {
            Debug.Log("[HomeTabContent] OnDisable - 탭 비활성화됨");
        }

        public override void Refresh()
        {
            Debug.Log("[HomeTabContent] Refresh 호출됨");
        }

        private void OnStageClicked()
        {
            Debug.Log("[HomeTabContent] >>> Stage 버튼 클릭됨! <<<");
            InGameContentDashboard.Open(new InGameContentDashboard.DashboardState());
        }

        private void OnShopClicked()
        {
            Debug.Log("[HomeTabContent] >>> Shop 버튼 클릭됨! <<<");
            ShopScreen.Open(new ShopScreen.ShopState());
        }

        private void OnEventClicked()
        {
            Debug.Log("[HomeTabContent] >>> Event 버튼 클릭됨! <<<");
            LiveEventScreen.Open(new LiveEventState());
        }
    }
}
