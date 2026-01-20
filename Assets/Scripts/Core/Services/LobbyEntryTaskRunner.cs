using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using Sc.Event;
using Sc.Foundation;
using Debug = UnityEngine.Debug;

namespace Sc.Core
{
    /// <summary>
    /// 로비 진입 Task 순차 실행기.
    /// 등록된 Task들을 Priority 순으로 실행하고 결과를 PopupQueue에 추가.
    /// </summary>
    public class LobbyEntryTaskRunner
    {
        private readonly List<ILobbyEntryTask> _tasks = new();
        private readonly IPopupQueueService _popupQueue;
        private bool _isRunning;

        /// <summary>팝업 큐 서비스</summary>
        public IPopupQueueService PopupQueue => _popupQueue;

        /// <summary>실행 중 여부</summary>
        public bool IsRunning => _isRunning;

        public LobbyEntryTaskRunner(IPopupQueueService popupQueue)
        {
            _popupQueue = popupQueue;
        }

        /// <summary>
        /// Task 등록 (Priority 순 자동 정렬)
        /// </summary>
        public void RegisterTask(ILobbyEntryTask task)
        {
            if (task == null)
            {
                Log.Warning("[LobbyEntryTaskRunner] null task 등록 시도", LogCategory.Lobby);
                return;
            }

            if (_isRunning)
            {
                Log.Warning("[LobbyEntryTaskRunner] 실행 중에는 Task를 등록할 수 없음", LogCategory.Lobby);
                return;
            }

            _tasks.Add(task);
            _tasks.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            Log.Info($"[LobbyEntryTaskRunner] Task 등록: {task.TaskName} (Priority: {task.Priority})", LogCategory.Lobby);
        }

        /// <summary>
        /// 모든 Task 순차 실행.
        /// - CheckRequiredAsync() → 실행 여부 판단
        /// - ExecuteAsync() → 실행 (실패해도 계속)
        /// - 결과를 PopupQueue에 추가
        /// </summary>
        public async UniTask<TaskRunnerSummary> ExecuteAllAsync()
        {
            if (_isRunning)
            {
                Log.Warning("[LobbyEntryTaskRunner] 이미 실행 중", LogCategory.Lobby);
                return new TaskRunnerSummary();
            }

            if (_tasks.Count == 0)
            {
                Log.Info("[LobbyEntryTaskRunner] 등록된 Task 없음", LogCategory.Lobby);
                return new TaskRunnerSummary();
            }

            _isRunning = true;

            var summary = new TaskRunnerSummary
            {
                TotalTasks = _tasks.Count
            };

            var stopwatch = new Stopwatch();

            try
            {
                foreach (var task in _tasks)
                {
                    stopwatch.Restart();

                    // 1. 실행 필요 여부 체크
                    bool isRequired;
                    try
                    {
                        isRequired = await task.CheckRequiredAsync();
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error($"[LobbyEntryTaskRunner] {task.TaskName} CheckRequired 예외: {ex.Message}", LogCategory.Lobby);
                        summary.SkippedTasks++;
                        continue;
                    }

                    if (!isRequired)
                    {
                        Log.Info($"[LobbyEntryTaskRunner] {task.TaskName} 스킵 (조건 미충족)", LogCategory.Lobby);
                        summary.SkippedTasks++;
                        continue;
                    }

                    // 2. Task 실행
                    Log.Info($"[LobbyEntryTaskRunner] {task.TaskName} 실행 시작", LogCategory.Lobby);

                    Result<LobbyTaskResult> result;
                    try
                    {
                        result = await task.ExecuteAsync();
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error($"[LobbyEntryTaskRunner] {task.TaskName} Execute 예외: {ex.Message}", LogCategory.Lobby);
                        summary.FailedTasks++;
                        PublishTaskCompletedEvent(task.TaskName, false, stopwatch.ElapsedMilliseconds);
                        continue;
                    }

                    stopwatch.Stop();

                    if (result.IsFailure)
                    {
                        Log.Error($"[LobbyEntryTaskRunner] {task.TaskName} 실패: {result.Message}", LogCategory.Lobby);
                        summary.FailedTasks++;
                        PublishTaskCompletedEvent(task.TaskName, false, stopwatch.ElapsedMilliseconds);
                        continue;
                    }

                    // 3. 성공 처리
                    summary.ExecutedTasks++;
                    PublishTaskCompletedEvent(task.TaskName, true, stopwatch.ElapsedMilliseconds);

                    // 4. 팝업 큐에 추가
                    var taskResult = result.Value;
                    if (taskResult != null && taskResult.ShouldShowPopup)
                    {
                        EnqueuePopup(taskResult);
                    }

                    Log.Info($"[LobbyEntryTaskRunner] {task.TaskName} 완료 ({stopwatch.ElapsedMilliseconds}ms)", LogCategory.Lobby);
                }

                // 전체 완료 이벤트 발행
                PublishTasksCompletedEvent(summary);

                return summary;
            }
            finally
            {
                _isRunning = false;
            }
        }

        /// <summary>
        /// 등록된 Task 초기화
        /// </summary>
        public void Clear()
        {
            if (_isRunning)
            {
                Log.Warning("[LobbyEntryTaskRunner] 실행 중에는 초기화할 수 없음", LogCategory.Lobby);
                return;
            }

            _tasks.Clear();
        }

        private void EnqueuePopup(LobbyTaskResult result)
        {
            switch (result.PopupType)
            {
                case PopupType.Reward:
                    _popupQueue.EnqueueReward(result.PopupTitle, result.Rewards);
                    break;

                case PopupType.Notification:
                    _popupQueue.EnqueueNotification(result.PopupTitle, result.PopupMessage);
                    break;
            }
        }

        private void PublishTaskCompletedEvent(string taskName, bool isSuccess, long executionTimeMs)
        {
            if (!EventManager.HasInstance) return;

            EventManager.Instance.Publish(new LobbyEntryTaskCompletedEvent
            {
                TaskName = taskName,
                IsSuccess = isSuccess,
                ExecutionTimeMs = executionTimeMs
            });
        }

        private void PublishTasksCompletedEvent(TaskRunnerSummary summary)
        {
            if (!EventManager.HasInstance) return;

            EventManager.Instance.Publish(new LobbyEntryTasksCompletedEvent
            {
                TotalTasks = summary.TotalTasks,
                ExecutedTasks = summary.ExecutedTasks,
                FailedTasks = summary.FailedTasks
            });
        }
    }

    /// <summary>
    /// Task 실행 요약 정보
    /// </summary>
    public class TaskRunnerSummary
    {
        public int TotalTasks { get; init; }
        public int ExecutedTasks { get; set; }
        public int SkippedTasks { get; set; }
        public int FailedTasks { get; set; }
    }
}
