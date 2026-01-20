using System.Linq;
using Cysharp.Threading.Tasks;
using Sc.Core;
using Sc.Data;
using Sc.Foundation;
using Sc.LocalServer;
using UnityEngine;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 만료된 이벤트 재화를 범용 재화로 전환하는 Task.
    /// 유예 기간 만료 시 자동 실행.
    /// </summary>
    public class EventCurrencyConversionTask : ILobbyEntryTask
    {
        private readonly EventCurrencyConverter _converter;
        private readonly DataManager _dataManager;

        public string TaskName => "이벤트 재화 전환";
        public int Priority => 20;

        public EventCurrencyConversionTask(EventCurrencyConverter converter, DataManager dataManager)
        {
            _converter = converter;
            _dataManager = dataManager;
        }

        public UniTask<bool> CheckRequiredAsync()
        {
            // 항상 체크 (converter 내부에서 판단)
            // EventCurrencyConverter가 만료된 이벤트가 있는지 확인
            return UniTask.FromResult(true);
        }

        public UniTask<Result<LobbyTaskResult>> ExecuteAsync()
        {
            if (_converter == null)
            {
                Debug.LogWarning("[EventCurrencyConversionTask] Converter가 null");
                return UniTask.FromResult(
                    Result<LobbyTaskResult>.Failure(ErrorCode.InvalidParameter, "EventCurrencyConverter가 초기화되지 않음"));
            }

            if (_dataManager == null || !_dataManager.IsInitialized)
            {
                Debug.LogWarning("[EventCurrencyConversionTask] DataManager가 초기화되지 않음");
                return UniTask.FromResult(
                    Result<LobbyTaskResult>.Failure(ErrorCode.InvalidState, "DataManager가 초기화되지 않음"));
            }

            // UserSaveData 복사 (ref 전달을 위해)
            var userData = _dataManager.GetUserDataCopy();

            // 재화 전환 실행
            var results = _converter.ConvertExpiredCurrencies(ref userData);

            if (results.Count == 0)
            {
                Debug.Log("[EventCurrencyConversionTask] 전환할 재화 없음");
                return UniTask.FromResult(Result<LobbyTaskResult>.Success(LobbyTaskResult.Empty()));
            }

            // 데이터 저장
            try
            {
                // SaveManager를 통한 저장 (로컬)
                var saveResult = SaveManager.Instance.Save(userData);
                if (saveResult.IsFailure)
                {
                    Debug.LogError($"[EventCurrencyConversionTask] 저장 실패: {saveResult.Message}");
                    return UniTask.FromResult(
                        Result<LobbyTaskResult>.Failure(saveResult.Error, saveResult.Message));
                }

                // DataManager 갱신 (이벤트 발행됨)
                _dataManager.UpdateUserData(userData);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EventCurrencyConversionTask] 저장 예외: {ex.Message}");
                return UniTask.FromResult(
                    Result<LobbyTaskResult>.Failure(ErrorCode.SaveFailed, ex.Message));
            }

            // 보상 정보로 변환
            var rewards = results.Select(r => new RewardInfo(
                RewardType.Currency,
                r.TargetCurrencyId,
                r.TargetAmount
            )).ToArray();

            Debug.Log($"[EventCurrencyConversionTask] {results.Count}개 재화 전환 완료");

            return UniTask.FromResult(
                Result<LobbyTaskResult>.Success(LobbyTaskResult.WithRewards("이벤트 재화 전환", rewards)));
        }
    }
}
