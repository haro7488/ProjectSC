using UnityEngine;
using Sc.Contents.Gacha;
using Sc.Core;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 가챠 탭 컨텐츠 - 가챠 풀 목록으로 네비게이션
    /// </summary>
    public class GachaTabContent : LobbyTabContent
    {
        [Header("UI References")]
        [SerializeField] private Transform _gachaPoolContainer;

        public override void OnTabSelected()
        {
            Refresh();
        }

        public override void Refresh()
        {
            // 가챠 풀 미리보기 표시 (추후 확장)
            // 현재는 탭 선택 시 GachaScreen으로 이동
        }

        /// <summary>
        /// 가챠 화면으로 이동
        /// </summary>
        public void NavigateToGacha()
        {
            Debug.Log("[GachaTabContent] Navigate to Gacha");
            GachaScreen.Open(new GachaState());
        }
    }
}
