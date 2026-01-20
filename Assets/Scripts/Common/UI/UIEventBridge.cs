using Sc.Core;
using Sc.Event.UI;
using Sc.Foundation;
using UnityEngine;

namespace Sc.Common.UI
{
    /// <summary>
    /// UI 이벤트를 실제 UI 컴포넌트로 연결하는 브릿지.
    /// Core에서 발행된 UI 요청 이벤트를 구독하고 적절한 UI를 표시.
    /// </summary>
    public class UIEventBridge : Singleton<UIEventBridge>
    {
        protected override void OnSingletonAwake()
        {
            SubscribeEvents();
        }

        protected override void OnSingletonDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!EventManager.HasInstance) return;

            EventManager.Instance.Subscribe<ShowLoadingEvent>(OnShowLoading);
            EventManager.Instance.Subscribe<HideLoadingEvent>(OnHideLoading);
            EventManager.Instance.Subscribe<LoadingProgressEvent>(OnLoadingProgress);
            EventManager.Instance.Subscribe<ShowConfirmationEvent>(OnShowConfirmation);
        }

        private void UnsubscribeEvents()
        {
            if (!EventManager.HasInstance) return;

            EventManager.Instance.Unsubscribe<ShowLoadingEvent>(OnShowLoading);
            EventManager.Instance.Unsubscribe<HideLoadingEvent>(OnHideLoading);
            EventManager.Instance.Unsubscribe<LoadingProgressEvent>(OnLoadingProgress);
            EventManager.Instance.Unsubscribe<ShowConfirmationEvent>(OnShowConfirmation);
        }

        #region Loading Events

        private void OnShowLoading(ShowLoadingEvent evt)
        {
            if (!LoadingService.HasInstance)
            {
                Log.Warning("[UIEventBridge] LoadingService 없음", LogCategory.UI);
                return;
            }

            LoadingService.Instance.ShowProgress(evt.InitialProgress, evt.Message);
        }

        private void OnHideLoading(HideLoadingEvent evt)
        {
            if (!LoadingService.HasInstance) return;

            LoadingService.Instance.Hide();
        }

        private void OnLoadingProgress(LoadingProgressEvent evt)
        {
            if (!LoadingService.HasInstance) return;

            if (LoadingService.Instance.IsLoading)
            {
                LoadingService.Instance.UpdateProgress(evt.Progress, evt.StepName);
            }
        }

        #endregion

        #region Confirmation Events

        private void OnShowConfirmation(ShowConfirmationEvent evt)
        {
            var state = new ConfirmState
            {
                Title = evt.Title,
                Message = evt.Message,
                ConfirmText = evt.ConfirmText,
                CancelText = evt.CancelText,
                ShowCancelButton = evt.ShowCancelButton,
                OnConfirm = evt.OnConfirm,
                OnCancel = evt.OnCancel
            };

            ConfirmPopup.Open(state);
        }

        #endregion
    }
}