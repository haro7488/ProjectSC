using UnityEngine;
using Sc.Contents.Character;
using Sc.Core;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 캐릭터 탭 컨텐츠 - 캐릭터 목록으로 네비게이션
    /// </summary>
    public class CharacterTabContent : LobbyTabContent
    {
        [Header("UI References")]
        [SerializeField] private Transform _characterListContainer;

        public override void OnTabSelected()
        {
            Refresh();
        }

        public override void Refresh()
        {
            // 캐릭터 목록 미리보기 표시 (추후 확장)
            // 현재는 탭 선택 시 CharacterListScreen으로 이동
        }

        /// <summary>
        /// 캐릭터 목록 화면으로 이동
        /// </summary>
        public void NavigateToCharacterList()
        {
            Debug.Log("[CharacterTabContent] Navigate to CharacterList");
            CharacterListScreen.Open(new CharacterListState());
        }
    }
}
