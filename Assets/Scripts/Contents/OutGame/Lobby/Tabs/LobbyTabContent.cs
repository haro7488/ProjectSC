using UnityEngine;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 로비 탭 컨텐츠 베이스 클래스
    /// </summary>
    public abstract class LobbyTabContent : MonoBehaviour
    {
        /// <summary>
        /// 탭 활성화 시 호출
        /// </summary>
        public virtual void OnTabSelected() { }

        /// <summary>
        /// 탭 비활성화 시 호출
        /// </summary>
        public virtual void OnTabDeselected() { }

        /// <summary>
        /// 데이터 갱신
        /// </summary>
        public abstract void Refresh();
    }
}
