using System.Collections.Generic;
using System.Linq;
using Sc.Common.UI;
using Sc.Common.UI.Attributes;
using Sc.Common.UI.Widgets;
using Sc.Contents.Shop.Widgets;
using Sc.Core;
using Sc.Data;
using Sc.Event.OutGame;
using Sc.Event.UI;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Shop
{
    /// <summary>
    /// 상점 화면
    /// </summary>
    [ScreenTemplate(ScreenTemplateType.Standard)]
    public class ShopScreen : ScreenWidget<ShopScreen, ShopScreen.ShopState>
    {
        /// <summary>
        /// 상점 화면 상태
        /// </summary>
        public class ShopState : IScreenState
        {
            /// <summary>
            /// 상점 데이터 Provider
            /// </summary>
            public IShopProvider Provider { get; set; }

            /// <summary>
            /// 초기 선택 탭 (ProductType)
            /// </summary>
            public ShopProductType InitialTab { get; set; } = ShopProductType.Currency;
        }

        [Header("Shopkeeper Display")]
        [SerializeField] private Image _shopkeeperImage;
        [SerializeField] private TMP_Text _dialogueText;

        [Header("Tab")]
        [SerializeField] private TabGroupWidget _tabGroup;
        [SerializeField] private ShopTabButton[] _tabButtons;

        [Header("Product Grid")]
        [SerializeField] private Transform _productContainer;
        [SerializeField] private ShopProductItem _productItemPrefab;
        [SerializeField] private ShopProductCard _productCardPrefab;
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Category Shortcuts")]
        [SerializeField] private Transform _categoryShortcutContainer;
        [SerializeField] private CategoryShortcut[] _categoryShortcuts;

        [Header("Currency Display")]
        [SerializeField] private TMP_Text _goldText;
        [SerializeField] private TMP_Text _gemText;

        [Header("Footer")]
        [SerializeField] private TMP_Text _refreshTimerText;
        [SerializeField] private Toggle _selectAllToggle;
        [SerializeField] private TMP_Text _selectAllText;
        [SerializeField] private Button _bulkPurchaseButton;

        [Header("Empty State")]
        [SerializeField] private GameObject _emptyStateObject;
        [SerializeField] private TMP_Text _emptyStateText;

        [Header("Navigation")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;

        private ShopState _currentState;
        private IShopProvider _provider;
        private List<ShopProductType> _availableTypes;
        private ShopProductType _currentType;
        private readonly List<ShopProductItem> _productItems = new();
        private readonly List<ShopProductCard> _productCards = new();
        private readonly HashSet<ShopProductCard> _selectedCards = new();
        private bool _isPurchasing;
        private bool _isSelectMode;

        protected override void OnInitialize()
        {
            Debug.Log("[ShopScreen] OnInitialize");

            if (_tabGroup != null)
            {
                _tabGroup.Initialize();
                _tabGroup.OnTabChanged += OnTabChanged;
            }

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }

            if (_selectAllToggle != null)
            {
                _selectAllToggle.onValueChanged.AddListener(OnSelectAllToggleChanged);
            }

            if (_bulkPurchaseButton != null)
            {
                _bulkPurchaseButton.onClick.AddListener(OnBulkPurchaseClicked);
            }

            InitializeTabButtons();
            InitializeCategoryShortcuts();
        }

        private void InitializeTabButtons()
        {
            if (_tabButtons == null) return;

            for (int i = 0; i < _tabButtons.Length; i++)
            {
                var tab = _tabButtons[i];
                if (tab != null)
                {
                    tab.OnClicked += OnTabButtonClicked;
                }
            }
        }

        private void InitializeCategoryShortcuts()
        {
            if (_categoryShortcuts == null) return;

            foreach (var shortcut in _categoryShortcuts)
            {
                if (shortcut != null)
                {
                    shortcut.OnClicked += OnCategoryShortcutClicked;
                }
            }
        }

        protected override void OnBind(ShopState state)
        {
            _currentState = state ?? new ShopState();
            _provider = _currentState.Provider;

            Debug.Log($"[ShopScreen] OnBind - Provider: {_provider?.ShopId ?? "null"}");

            // Provider 없으면 기본 NormalShopProvider 사용
            if (_provider == null)
            {
                var database = DataManager.Instance?.GetDatabase<ShopProductDatabase>();
                _provider = new NormalShopProvider(database);
            }

            // Header 설정
            ScreenHeader.Instance?.Configure("shop_main");

            SetupTabs();
        }

        protected override void OnShow()
        {
            Debug.Log("[ShopScreen] OnShow");

            // DataManager 이벤트 구독
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged += OnUserDataChanged;
            }

            // 구매 이벤트 구독
            EventManager.Instance?.Subscribe<ProductPurchasedEvent>(OnProductPurchased);
            EventManager.Instance?.Subscribe<ProductPurchaseFailedEvent>(OnProductPurchaseFailed);

            // Header Back 이벤트 구독
            EventManager.Instance?.Subscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);

            RefreshCurrency();
            RefreshProductList();
        }

        protected override void OnHide()
        {
            Debug.Log("[ShopScreen] OnHide");

            // DataManager 이벤트 해제
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged -= OnUserDataChanged;
            }

            // 구매 이벤트 해제
            EventManager.Instance?.Unsubscribe<ProductPurchasedEvent>(OnProductPurchased);
            EventManager.Instance?.Unsubscribe<ProductPurchaseFailedEvent>(OnProductPurchaseFailed);

            // Header Back 이벤트 해제
            EventManager.Instance?.Unsubscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);
        }

        public override ShopState GetState() => _currentState;

        #region Tab Setup

        private void SetupTabs()
        {
            _availableTypes = _provider?.GetAvailableTypes() ?? new List<ShopProductType>();

            if (_availableTypes.Count == 0)
            {
                Debug.LogWarning("[ShopScreen] No available product types");
                ShowEmptyState("상품이 없습니다.");
                return;
            }

            // 탭 라벨 생성
            var labels = _availableTypes.Select(GetTypeLabel).ToList();
            _tabGroup?.SetupTabButtons(labels);

            // 초기 탭 선택
            int initialIndex = 0;
            if (_currentState.InitialTab != default)
            {
                initialIndex = _availableTypes.IndexOf(_currentState.InitialTab);
                if (initialIndex < 0) initialIndex = 0;
            }

            _tabGroup?.SelectTab(initialIndex);
        }

        private string GetTypeLabel(ShopProductType type)
        {
            return type switch
            {
                ShopProductType.Currency => "재화",
                ShopProductType.Package => "패키지",
                ShopProductType.CharacterPiece => "캐릭터",
                ShopProductType.Item => "아이템",
                ShopProductType.Stamina => "스태미나",
                ShopProductType.EventShop => "이벤트",
                _ => type.ToString()
            };
        }

        private void OnTabChanged(int index)
        {
            if (index < 0 || index >= _availableTypes.Count) return;

            _currentType = _availableTypes[index];
            Debug.Log($"[ShopScreen] Tab changed to: {_currentType}");

            // 탭 버튼 선택 상태 업데이트
            UpdateTabButtonSelection(index);

            RefreshProductList();
        }

        private void OnTabButtonClicked(int index)
        {
            _tabGroup?.SelectTab(index);
        }

        private void UpdateTabButtonSelection(int selectedIndex)
        {
            if (_tabButtons == null) return;

            for (int i = 0; i < _tabButtons.Length; i++)
            {
                _tabButtons[i]?.SetSelected(i == selectedIndex);
            }
        }

        #endregion

        #region Product List

        private void RefreshProductList()
        {
            ClearProductItems();

            var products = _provider?.GetProducts(_currentType) ?? new List<ShopProductData>();

            if (products.Count == 0)
            {
                ShowEmptyState("상품이 없습니다.");
                return;
            }

            HideEmptyState();

            foreach (var product in products)
            {
                CreateProductItem(product);
            }

            // 스크롤 초기화
            if (_scrollRect != null)
            {
                _scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private void CreateProductItem(ShopProductData productData)
        {
            if (_productItemPrefab == null || _productContainer == null) return;

            var itemGo = Instantiate(_productItemPrefab, _productContainer);
            var item = itemGo.GetComponent<ShopProductItem>();
            if (item != null)
            {
                item.Initialize();

                // 구매 기록 조회
                var record = _provider?.GetPurchaseRecord(productData.Id);
                item.Setup(productData, record, OnProductClicked);
                _productItems.Add(item);
            }
        }

        private void ClearProductItems()
        {
            foreach (var item in _productItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }

            _productItems.Clear();

            foreach (var card in _productCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }

            _productCards.Clear();
            _selectedCards.Clear();
        }

        private void OnProductClicked(ShopProductData product)
        {
            Debug.Log($"[ShopScreen] Product clicked: {product.Id}");

            if (_isPurchasing)
            {
                Debug.Log("[ShopScreen] Already purchasing");
                return;
            }

            // 재화 확인 팝업 표시
            ShowPurchaseConfirm(product);
        }

        #endregion

        #region Purchase Flow

        private void ShowPurchaseConfirm(ShopProductData product)
        {
            var currentAmount = GetCurrentCurrency(product.CostType);

            var state = new CostConfirmState
            {
                Title = "구매 확인",
                Message = $"{product.NameKey}을(를) 구매하시겠습니까?",
                CostType = product.CostType,
                CostAmount = product.Price,
                CurrentAmount = currentAmount,
                ConfirmText = "구매",
                CancelText = "취소",
                OnConfirm = () => ExecutePurchase(product),
                OnCancel = () => Debug.Log("[ShopScreen] Purchase cancelled")
            };

            CostConfirmPopup.Open(state);
        }

        private int GetCurrentCurrency(CostType costType)
        {
            if (DataManager.Instance?.IsInitialized != true) return 0;

            var currency = DataManager.Instance.Currency;
            return costType switch
            {
                CostType.Gold => (int)currency.Gold,
                CostType.Gem => currency.TotalGem,
                CostType.EventCurrency => 0, // TODO: 이벤트 재화는 EventCurrencyData에서 별도 조회 필요
                _ => 0
            };
        }

        private void ExecutePurchase(ShopProductData product)
        {
            if (_isPurchasing) return;

            if (NetworkManager.Instance == null || !NetworkManager.Instance.IsInitialized)
            {
                Debug.LogError("[ShopScreen] NetworkManager not initialized");
                return;
            }

            _isPurchasing = true;
            UpdatePurchaseState();

            var request = ShopPurchaseRequest.Create(product.Id, 1);
            Debug.Log($"[ShopScreen] Sending purchase request: {product.Id}");
            NetworkManager.Instance.Send(request);
        }

        private void OnProductPurchased(ProductPurchasedEvent evt)
        {
            Debug.Log($"[ShopScreen] Purchase completed: {evt.ProductId}");

            _isPurchasing = false;
            RefreshCurrency();
            RefreshProductList();
            UpdatePurchaseState();

            // 보상 팝업 표시 (선택적)
            if (evt.Rewards != null && evt.Rewards.Count > 0)
            {
                // TODO: RewardPopup 연동
                Debug.Log($"[ShopScreen] Rewards: {evt.Rewards.Count} items");
            }
        }

        private void OnProductPurchaseFailed(ProductPurchaseFailedEvent evt)
        {
            Debug.LogWarning($"[ShopScreen] Purchase failed: {evt.ErrorCode} - {evt.ErrorMessage}");

            _isPurchasing = false;
            UpdatePurchaseState();

            // TODO: 에러 팝업 표시
        }

        private void UpdatePurchaseState()
        {
            foreach (var item in _productItems)
            {
                item?.SetInteractable(!_isPurchasing);
            }

            foreach (var card in _productCards)
            {
                card?.SetInteractable(!_isPurchasing);
            }
        }

        #endregion

        #region Select All / Bulk Purchase

        private void OnSelectAllToggleChanged(bool isOn)
        {
            _isSelectMode = isOn;

            UpdateSelectAllText();
            UpdateSelectMode();

            if (isOn)
            {
                SelectAllCards();
            }
            else
            {
                DeselectAllCards();
            }
        }

        private void UpdateSelectAllText()
        {
            if (_selectAllText != null)
            {
                _selectAllText.text = _isSelectMode ? "모두 선택 ON" : "모두 선택 OFF";
            }
        }

        private void UpdateSelectMode()
        {
            foreach (var card in _productCards)
            {
                card?.SetSelectMode(_isSelectMode);
            }
        }

        private void SelectAllCards()
        {
            _selectedCards.Clear();
            foreach (var card in _productCards)
            {
                if (card != null && !card.IsSoldOut)
                {
                    card.SetSelected(true);
                    _selectedCards.Add(card);
                }
            }
        }

        private void DeselectAllCards()
        {
            foreach (var card in _productCards)
            {
                card?.SetSelected(false);
            }
            _selectedCards.Clear();
        }

        private void OnCardSelected(ShopProductCard card, bool isSelected)
        {
            if (isSelected)
            {
                _selectedCards.Add(card);
            }
            else
            {
                _selectedCards.Remove(card);
            }

            Debug.Log($"[ShopScreen] Selected cards: {_selectedCards.Count}");
        }

        private void OnBulkPurchaseClicked()
        {
            if (_selectedCards.Count == 0)
            {
                Debug.Log("[ShopScreen] No cards selected for bulk purchase");
                return;
            }

            if (_isPurchasing)
            {
                Debug.Log("[ShopScreen] Already purchasing");
                return;
            }

            // 선택된 상품들의 총 비용 계산
            int totalCost = 0;
            CostType costType = CostType.Gold;

            foreach (var card in _selectedCards)
            {
                if (card?.ProductData != null)
                {
                    totalCost += card.ProductData.Price;
                    costType = card.ProductData.CostType;
                }
            }

            var currentAmount = GetCurrentCurrency(costType);

            var state = new CostConfirmState
            {
                Title = "일괄 구매 확인",
                Message = $"{_selectedCards.Count}개 상품을 구매하시겠습니까?",
                CostType = costType,
                CostAmount = totalCost,
                CurrentAmount = currentAmount,
                ConfirmText = "구매",
                CancelText = "취소",
                OnConfirm = ExecuteBulkPurchase,
                OnCancel = () => Debug.Log("[ShopScreen] Bulk purchase cancelled")
            };

            CostConfirmPopup.Open(state);
        }

        private void ExecuteBulkPurchase()
        {
            if (_isPurchasing || _selectedCards.Count == 0) return;

            // TODO: 일괄 구매 요청 구현
            // 현재는 개별 구매로 처리
            foreach (var card in _selectedCards.ToList())
            {
                if (card?.ProductData != null)
                {
                    ExecutePurchase(card.ProductData);
                }
            }
        }

        #endregion

        #region Category Shortcuts

        private void OnCategoryShortcutClicked(CategoryShortcut shortcut)
        {
            if (shortcut == null) return;

            var targetCategory = shortcut.TargetCategory;
            Debug.Log($"[ShopScreen] Category shortcut clicked: {targetCategory}");

            // 해당 카테고리 탭으로 이동
            int tabIndex = _availableTypes?.IndexOf(targetCategory) ?? -1;
            if (tabIndex >= 0)
            {
                _tabGroup?.SelectTab(tabIndex);
            }
        }

        #endregion

        #region Currency Display

        private void RefreshCurrency()
        {
            if (DataManager.Instance?.IsInitialized != true) return;

            var currency = DataManager.Instance.Currency;

            if (_goldText != null)
            {
                _goldText.text = $"{currency.Gold:N0}";
            }

            if (_gemText != null)
            {
                _gemText.text = $"{currency.TotalGem:N0}";
            }
        }

        private void OnUserDataChanged()
        {
            RefreshCurrency();
        }

        #endregion

        #region Empty State

        private void ShowEmptyState(string message)
        {
            if (_emptyStateObject != null)
            {
                _emptyStateObject.SetActive(true);
            }

            if (_emptyStateText != null)
            {
                _emptyStateText.text = message;
            }
        }

        private void HideEmptyState()
        {
            if (_emptyStateObject != null)
            {
                _emptyStateObject.SetActive(false);
            }
        }

        #endregion

        #region Navigation

        private void OnBackClicked()
        {
            Debug.Log("[ShopScreen] Back clicked");
            NavigationManager.Instance?.Back();
        }

        private void OnHeaderBackClicked(HeaderBackClickedEvent evt)
        {
            OnBackClicked();
        }

        #endregion

        protected override void OnRelease()
        {
            if (_tabGroup != null)
            {
                _tabGroup.OnTabChanged -= OnTabChanged;
            }

            if (_selectAllToggle != null)
            {
                _selectAllToggle.onValueChanged.RemoveListener(OnSelectAllToggleChanged);
            }

            if (_bulkPurchaseButton != null)
            {
                _bulkPurchaseButton.onClick.RemoveListener(OnBulkPurchaseClicked);
            }

            CleanupTabButtons();
            CleanupCategoryShortcuts();
            ClearProductItems();

            _provider = null;
            _availableTypes = null;
        }

        private void CleanupTabButtons()
        {
            if (_tabButtons == null) return;

            foreach (var tab in _tabButtons)
            {
                if (tab != null)
                {
                    tab.OnClicked -= OnTabButtonClicked;
                }
            }
        }

        private void CleanupCategoryShortcuts()
        {
            if (_categoryShortcuts == null) return;

            foreach (var shortcut in _categoryShortcuts)
            {
                if (shortcut != null)
                {
                    shortcut.OnClicked -= OnCategoryShortcutClicked;
                }
            }
        }
    }
}