using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 설정 탭 컨텐츠 - 설정 옵션들
    /// </summary>
    public class SettingsTabContent : LobbyTabContent
    {
        [Header("UI References")]
        [SerializeField] private Toggle _bgmToggle;
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Toggle _pushToggle;
        [SerializeField] private Button _accountButton;
        [SerializeField] private Button _supportButton;

        private void Awake()
        {
            if (_bgmToggle != null)
                _bgmToggle.onValueChanged.AddListener(OnBgmToggled);
            if (_sfxToggle != null)
                _sfxToggle.onValueChanged.AddListener(OnSfxToggled);
            if (_pushToggle != null)
                _pushToggle.onValueChanged.AddListener(OnPushToggled);
            if (_accountButton != null)
                _accountButton.onClick.AddListener(OnAccountClicked);
            if (_supportButton != null)
                _supportButton.onClick.AddListener(OnSupportClicked);
        }

        public override void OnTabSelected()
        {
            Refresh();
        }

        public override void Refresh()
        {
            // 현재 설정 값으로 UI 갱신 (추후 확장)
        }

        private void OnBgmToggled(bool isOn)
        {
            Debug.Log($"[SettingsTabContent] BGM: {isOn}");
            // AudioManager.Instance?.SetBgmEnabled(isOn);
        }

        private void OnSfxToggled(bool isOn)
        {
            Debug.Log($"[SettingsTabContent] SFX: {isOn}");
            // AudioManager.Instance?.SetSfxEnabled(isOn);
        }

        private void OnPushToggled(bool isOn)
        {
            Debug.Log($"[SettingsTabContent] Push: {isOn}");
            // 푸시 알림 설정
        }

        private void OnAccountClicked()
        {
            Debug.Log("[SettingsTabContent] Account clicked");
            // 계정 연동 화면
        }

        private void OnSupportClicked()
        {
            Debug.Log("[SettingsTabContent] Support clicked");
            // 고객지원 화면
        }
    }
}
