using Sc.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sc.Common.UI.Tests
{
    /// <summary>
    /// SystemPopup 런타임 테스트용 컨트롤러.
    /// 에디터 플레이 모드에서 다양한 팝업 시나리오를 테스트.
    /// </summary>
    public class SystemPopupTestController : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private int _testGemAmount = 500;
        [SerializeField] private int _testGoldAmount = 10000;
        [SerializeField] private int _testStaminaAmount = 100;

        [Header("UI References (Optional)")]
        [SerializeField] private TMP_Text _logText;

        private void Start()
        {
            // 버튼이 있으면 연결
            ConnectButton("ConfirmPopupBtn", TestConfirmPopup);
            ConnectButton("AlertPopupBtn", TestAlertPopup);
            ConnectButton("CostGemBtn", TestCostGem);
            ConnectButton("CostGoldBtn", TestCostGold);
            ConnectButton("CostStaminaBtn", TestCostStamina);
            ConnectButton("InsufficientBtn", TestInsufficientCost);
            ConnectButton("CustomTextBtn", TestCustomButtonText);
        }

        private void Update()
        {
            // 키보드 단축키로 테스트
            if (Input.GetKeyDown(KeyCode.Alpha1)) TestConfirmPopup();
            if (Input.GetKeyDown(KeyCode.Alpha2)) TestAlertPopup();
            if (Input.GetKeyDown(KeyCode.Alpha3)) TestCostGem();
            if (Input.GetKeyDown(KeyCode.Alpha4)) TestCostGold();
            if (Input.GetKeyDown(KeyCode.Alpha5)) TestCostStamina();
            if (Input.GetKeyDown(KeyCode.Alpha6)) TestInsufficientCost();
            if (Input.GetKeyDown(KeyCode.Alpha7)) TestCustomButtonText();
        }

        #region ConfirmPopup Tests

        /// <summary>
        /// 기본 확인/취소 팝업 테스트
        /// </summary>
        public void TestConfirmPopup()
        {
            Log("TestConfirmPopup 시작");

            ConfirmPopup.Open(new ConfirmState
            {
                Title = "확인",
                Message = "이 작업을 진행하시겠습니까?",
                OnConfirm = () => Log("✅ 확인 버튼 클릭"),
                OnCancel = () => Log("❌ 취소 버튼 클릭")
            });
        }

        /// <summary>
        /// Alert 모드 (단일 버튼) 테스트
        /// </summary>
        public void TestAlertPopup()
        {
            Log("TestAlertPopup 시작");

            ConfirmPopup.Open(new ConfirmState
            {
                Title = "알림",
                Message = "스태미나가 부족합니다.\n충전 후 다시 시도해주세요.",
                ShowCancelButton = false,
                ConfirmText = "확인",
                OnConfirm = () => Log("✅ Alert 확인")
            });
        }

        /// <summary>
        /// 커스텀 버튼 텍스트 테스트
        /// </summary>
        public void TestCustomButtonText()
        {
            Log("TestCustomButtonText 시작");

            ConfirmPopup.Open(new ConfirmState
            {
                Title = "게임 종료",
                Message = "정말 게임을 종료하시겠습니까?\n저장되지 않은 진행 상황이 사라집니다.",
                ConfirmText = "종료",
                CancelText = "계속하기",
                OnConfirm = () => Log("✅ 종료 선택"),
                OnCancel = () => Log("❌ 계속하기 선택")
            });
        }

        #endregion

        #region CostConfirmPopup Tests

        /// <summary>
        /// Gem 소모 확인 팝업 테스트
        /// </summary>
        public void TestCostGem()
        {
            Log("TestCostGem 시작");

            CostConfirmPopup.Open(new CostConfirmState
            {
                Title = "구매 확인",
                Message = "프리미엄 패키지를 구매하시겠습니까?",
                CostType = CostType.Gem,
                CostAmount = 100,
                CurrentAmount = _testGemAmount,
                ConfirmText = "구매",
                OnConfirm = () => Log($"✅ Gem 100 소모 확인 (보유: {_testGemAmount})"),
                OnCancel = () => Log("❌ 구매 취소")
            });
        }

        /// <summary>
        /// Gold 소모 확인 팝업 테스트
        /// </summary>
        public void TestCostGold()
        {
            Log("TestCostGold 시작");

            CostConfirmPopup.Open(new CostConfirmState
            {
                Title = "강화 확인",
                Message = "장비를 강화하시겠습니까?",
                CostType = CostType.Gold,
                CostAmount = 5000,
                CurrentAmount = _testGoldAmount,
                ConfirmText = "강화",
                OnConfirm = () => Log($"✅ Gold 5000 소모 확인 (보유: {_testGoldAmount})"),
                OnCancel = () => Log("❌ 강화 취소")
            });
        }

        /// <summary>
        /// Stamina 소모 확인 팝업 테스트
        /// </summary>
        public void TestCostStamina()
        {
            Log("TestCostStamina 시작");

            CostConfirmPopup.Open(new CostConfirmState
            {
                Title = "출전 확인",
                Message = "스테이지 1-5에 출전합니다.",
                CostType = CostType.Stamina,
                CostAmount = 10,
                CurrentAmount = _testStaminaAmount,
                ConfirmText = "출전",
                OnConfirm = () => Log($"✅ Stamina 10 소모 확인 (보유: {_testStaminaAmount})"),
                OnCancel = () => Log("❌ 출전 취소")
            });
        }

        /// <summary>
        /// 재화 부족 상태 테스트 (빨간색 표시)
        /// </summary>
        public void TestInsufficientCost()
        {
            Log("TestInsufficientCost 시작 (부족 상태)");

            CostConfirmPopup.Open(new CostConfirmState
            {
                Title = "구매 확인",
                Message = "희귀 아이템을 구매하시겠습니까?",
                CostType = CostType.Gem,
                CostAmount = 1000,
                CurrentAmount = 50, // 부족!
                ConfirmText = "구매",
                OnConfirm = () => Log("✅ 부족 상태에서 구매 시도"),
                OnCancel = () => Log("❌ 구매 취소")
            });
        }

        #endregion

        #region Helper Methods

        private void ConnectButton(string name, UnityEngine.Events.UnityAction action)
        {
            var btn = transform.Find(name)?.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(action);
            }
        }

        private void Log(string message)
        {
            Debug.Log($"[SystemPopupTest] {message}");

            if (_logText != null)
            {
                _logText.text = $"{System.DateTime.Now:HH:mm:ss} - {message}\n{_logText.text}";

                // 로그 길이 제한
                if (_logText.text.Length > 1000)
                {
                    _logText.text = _logText.text.Substring(0, 1000);
                }
            }
        }

        #endregion

        #region OnGUI (Fallback UI)

        private void OnGUI()
        {
            // UI 버튼이 없을 때 OnGUI로 대체
            if (transform.childCount > 0) return;

            GUILayout.BeginArea(new Rect(10, 10, 250, 400));
            GUILayout.Label("SystemPopup 테스트", GUI.skin.box);
            GUILayout.Space(5);

            GUILayout.Label("ConfirmPopup:");
            if (GUILayout.Button("[1] 확인/취소")) TestConfirmPopup();
            if (GUILayout.Button("[2] Alert (단일 버튼)")) TestAlertPopup();
            if (GUILayout.Button("[7] 커스텀 텍스트")) TestCustomButtonText();

            GUILayout.Space(10);
            GUILayout.Label("CostConfirmPopup:");
            if (GUILayout.Button("[3] Gem 소모")) TestCostGem();
            if (GUILayout.Button("[4] Gold 소모")) TestCostGold();
            if (GUILayout.Button("[5] Stamina 소모")) TestCostStamina();
            if (GUILayout.Button("[6] 재화 부족")) TestInsufficientCost();

            GUILayout.Space(10);
            GUILayout.Label($"테스트 재화: Gem={_testGemAmount}, Gold={_testGoldAmount}");

            GUILayout.EndArea();
        }

        #endregion
    }
}
