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
            if (_stageButton != null)
                _stageButton.onClick.AddListener(OnStageClicked);
            if (_shopButton != null)
                _shopButton.onClick.AddListener(OnShopClicked);
            if (_eventButton != null)
                _eventButton.onClick.AddListener(OnEventClicked);
        }

        public override void Refresh()
        {
            // 배너, 공지 등 갱신 (추후 확장)
        }

        private void OnStageClicked()
        {
            Debug.Log("[HomeTabContent] Stage clicked");
            InGameContentDashboard.Open(new InGameContentDashboard.DashboardState());
        }

        private void OnShopClicked()
        {
            Debug.Log("[HomeTabContent] Shop clicked");
            ShopScreen.Open(new ShopScreen.ShopState());
        }

        private void OnEventClicked()
        {
            Debug.Log("[HomeTabContent] Event clicked");
            LiveEventScreen.Open(new LiveEventState());
        }
    }
}
