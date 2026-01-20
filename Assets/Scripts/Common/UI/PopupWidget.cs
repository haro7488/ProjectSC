using System;
using Cysharp.Threading.Tasks;
using Sc.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sc.Common.UI
{
    /// <summary>
    /// Popup 베이스 클래스 (비제네릭).
    /// 구현 클래스: ConfirmPopup, RewardPopup 등 (Widget 접미사 없음)
    /// </summary>
    public abstract class PopupWidget : Widget
    {
        /// <summary>
        /// ESC 키 처리. true 반환 시 닫기 허용.
        /// </summary>
        public virtual bool OnEscape() => true;

        /// <summary>
        /// Popup 상태 캡슐화 (State + Order).
        /// </summary>
        public abstract class Context : INavigationContext
        {
            public abstract PopupWidget View { get; }
            public abstract Type PopupType { get; }

            // INavigationContext 구현
            public NavigationContextType ContextType => NavigationContextType.Popup;
            public Type WidgetType => PopupType;
            Widget INavigationContext.View => View;

            public abstract UniTask Load();
            public abstract UniTask Enter();
            public abstract void Resume();
            public abstract void Pause();
            public abstract UniTask Exit();

            /// <summary>
            /// Popup의 OnEscape()에 위임.
            /// </summary>
            public virtual bool HandleEscape() => View?.OnEscape() ?? true;

            /// <summary>
            /// Popup 전환 애니메이션 반환.
            /// </summary>
            public abstract PopupTransition GetTransition();
        }
    }

    /// <summary>
    /// Popup 제네릭 베이스 클래스.
    /// 구현 예시: public class ConfirmPopup : PopupWidget&lt;ConfirmPopup, ConfirmState&gt;
    /// </summary>
    /// <typeparam name="TPopup">Popup 타입 (ConfirmPopup 등)</typeparam>
    /// <typeparam name="TState">State 타입</typeparam>
    public abstract class PopupWidget<TPopup, TState> : PopupWidget
        where TPopup : PopupWidget<TPopup, TState>
        where TState : class, IPopupState
    {
        #region Lifecycle Hooks

        /// <summary>
        /// State로 UI 초기화. 서브클래스에서 오버라이드.
        /// </summary>
        protected virtual void OnBind(TState state)
        {
        }

        /// <summary>
        /// 현재 상태 반환. 서브클래스에서 오버라이드.
        /// </summary>
        public virtual TState GetState() => default;

        #endregion

        #region Context

        /// <summary>
        /// Popup Context 구현.
        /// </summary>
        public new class Context : PopupWidget.Context
        {
            private TPopup _popup;
            private TState _state;
            private readonly int _sortingOrder;
            private readonly PopupTransition _transition;
            private AssetScope _scope;
            private bool _isInstantiated;
            private bool _isLoading;

            public override PopupWidget View => _popup;
            public override Type PopupType => typeof(TPopup);
            public TState State => _state;
            public int SortingOrder => _sortingOrder;

            internal Context(TState state, int sortingOrder, PopupTransition transition)
            {
                _state = state;
                _sortingOrder = sortingOrder;
                _transition = transition ?? PopupTransition.Default;
            }

            public override PopupTransition GetTransition() => _transition;

            public override async UniTask Load()
            {
                // Race condition 방지
                if (_isLoading || _popup != null) return;
                _isLoading = true;

                try
                {
                    // 1. 씬에서 먼저 찾기 (기존 방식 하위 호환)
                    _popup = FindObjectOfType<TPopup>(true);

                    if (_popup != null)
                    {
                        _isInstantiated = false;
                        return;
                    }

                    // 2. Addressables에서 로드
                    if (!AssetManager.HasInstance)
                    {
                        Debug.LogError($"[PopupWidget] AssetManager가 초기화되지 않음");
                        return;
                    }

                    // Canvas 체크
                    var parent = NavigationManager.Instance?.PopupCanvas?.transform;
                    if (parent == null)
                    {
                        Debug.LogError($"[PopupWidget] PopupCanvas가 없어서 {typeof(TPopup).Name} 로드 불가");
                        return;
                    }

                    _scope = AssetManager.Instance.CreateScope($"Popup_{typeof(TPopup).Name}");
                    var result = await AssetManager.Instance.LoadPopupPrefabAsync<TPopup>(_scope);

                    if (!result.IsSuccess)
                    {
                        Debug.LogError($"[PopupWidget] {typeof(TPopup).Name} 로드 실패: {result.Error}");
                        CleanupScope();
                        return;
                    }

                    // 3. 프리팹 인스턴스화
                    var prefab = result.Value;
                    var instance = Instantiate(prefab, parent);
                    instance.name = typeof(TPopup).Name;

                    _popup = instance.GetComponent<TPopup>();
                    _isInstantiated = true;

                    if (_popup == null)
                    {
                        Debug.LogError($"[PopupWidget] 프리팹에 {typeof(TPopup).Name} 컴포넌트가 없음");
                        Destroy(instance);
                        _isInstantiated = false;
                        CleanupScope();
                    }
                }
                finally
                {
                    _isLoading = false;
                }
            }

            private void CleanupScope()
            {
                if (_scope != null && AssetManager.HasInstance)
                {
                    AssetManager.Instance.ReleaseScope(_scope);
                    _scope = null;
                }
            }

            public override async UniTask Enter()
            {
                _popup?.Initialize();
                _popup?.OnBind(_state);
                await UniTask.CompletedTask;
            }

            public override void Resume()
            {
                _popup?.Show();
            }

            public override void Pause()
            {
                _popup?.Hide();
            }

            public override async UniTask Exit()
            {
                // 상태 저장
                var currentState = _popup?.GetState();
                if (currentState != null)
                    _state = currentState;

                // UI 숨김
                _popup?.Hide();

                // Addressables로 로드된 인스턴스 정리
                if (_isInstantiated && _popup != null)
                {
                    Object.Destroy(_popup.gameObject);
                    _popup = null;
                }

                // Scope 해제 (에셋 RefCount 감소)
                if (_scope != null)
                {
                    AssetManager.Instance?.ReleaseScope(_scope);
                    _scope = null;
                }

                await UniTask.CompletedTask;
            }

            #region Builder

            public struct Builder
            {
                private readonly TState _state;
                private int _sortingOrder;
                private PopupTransition _transition;

                public Builder(TState state)
                {
                    _state = state;
                    _sortingOrder = 0;
                    _transition = null;
                }

                public Builder SetOrder(int order)
                {
                    _sortingOrder = order;
                    return this;
                }

                public Builder SetTransition(PopupTransition transition)
                {
                    _transition = transition;
                    return this;
                }

                public Context Build() => new(_state, _sortingOrder, _transition);

                public static implicit operator Context(Builder builder) => builder.Build();
            }

            #endregion
        }

        #endregion

        #region Static Entry Points

        /// <summary>
        /// Context Builder 생성.
        /// </summary>
        public static Context.Builder CreateContext(TState state = default) => new(state);

        /// <summary>
        /// Popup 열기 (Push).
        /// 사용법: ConfirmPopup.Open(new ConfirmState { Message = "확인?" });
        /// </summary>
        public static void Open(TState state = default, PopupTransition transition = null)
        {
            var builder = CreateContext(state);
            if (transition != null)
                builder.SetTransition(transition);

            NavigationManager.Instance?.Push(builder.Build());
        }

        /// <summary>
        /// Popup 열기 (Push) - 별칭.
        /// </summary>
        public static void Push(TState state = default, PopupTransition transition = null)
            => Open(state, transition);

        #endregion
    }
}