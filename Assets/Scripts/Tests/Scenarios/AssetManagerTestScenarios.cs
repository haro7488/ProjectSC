using System;
using Sc.Core;
using UnityEngine;

namespace Sc.Tests
{
    /// <summary>
    /// AssetManager 시스템 테스트 시나리오 모음.
    /// </summary>
    public class AssetManagerTestScenarios
    {
        /// <summary>
        /// 테스트 환경 초기화
        /// </summary>
        public void Setup()
        {
            // AssetManager 초기화
            if (AssetManager.HasInstance && !AssetManager.Instance.IsInitialized)
            {
                AssetManager.Instance.Initialize();
            }
        }

        /// <summary>
        /// 테스트 환경 정리
        /// </summary>
        public void Teardown()
        {
            if (AssetManager.HasInstance)
            {
                AssetManager.Instance.ClearCache();
            }
        }

        #region Scenario: Initialization

        /// <summary>
        /// 시나리오: 초기화 테스트
        /// Initialize 호출 → IsInitialized true
        /// </summary>
        public TestResult RunInitializationScenario()
        {
            try
            {
                if (!AssetManager.HasInstance)
                {
                    return TestResult.Fail("AssetManager 인스턴스가 없음");
                }

                bool isInitialized = AssetManager.Instance.IsInitialized;

                if (!isInitialized)
                {
                    bool initResult = AssetManager.Instance.Initialize();
                    isInitialized = AssetManager.Instance.IsInitialized;

                    return new TestResult
                    {
                        Success = initResult && isInitialized,
                        Message = $"Initialize={initResult}, IsInitialized={isInitialized}"
                    };
                }

                return TestResult.Pass($"이미 초기화됨: IsInitialized={isInitialized}");
            }
            catch (Exception e)
            {
                return TestResult.Fail($"예외 발생: {e.Message}");
            }
        }

        #endregion

        #region Scenario: Scope Management

        /// <summary>
        /// 시나리오: Scope 생성/조회
        /// CreateScope → GetScope → 동일 인스턴스 확인
        /// </summary>
        public TestResult RunScopeCreationScenario()
        {
            Setup();

            try
            {
                var scopeName = "TestScope_" + Guid.NewGuid().ToString("N").Substring(0, 8);

                // Scope 생성
                var scope = AssetManager.Instance.CreateScope(scopeName);
                if (scope == null)
                {
                    return TestResult.Fail("Scope 생성 실패");
                }

                bool nameMatch = scope.Name == scopeName;
                bool notReleased = !scope.IsReleased;
                bool assetCountZero = scope.AssetCount == 0;

                // GetScope로 조회
                var retrievedScope = AssetManager.Instance.GetScope(scopeName);
                bool sameInstance = object.ReferenceEquals(scope, retrievedScope);

                bool success = nameMatch && notReleased && assetCountZero && sameInstance;

                // 정리
                AssetManager.Instance.ReleaseScope(scope);

                return new TestResult
                {
                    Success = success,
                    Message = $"Name={nameMatch}, NotReleased={notReleased}, Count={assetCountZero}, SameInstance={sameInstance}"
                };
            }
            catch (Exception e)
            {
                return TestResult.Fail($"예외 발생: {e.Message}");
            }
            finally
            {
                Teardown();
            }
        }

        /// <summary>
        /// 시나리오: Scope 해제
        /// CreateScope → ReleaseScope → GetScope null
        /// </summary>
        public TestResult RunScopeReleaseScenario()
        {
            Setup();

            try
            {
                var scopeName = "ReleaseTestScope_" + Guid.NewGuid().ToString("N").Substring(0, 8);

                // Scope 생성
                var scope = AssetManager.Instance.CreateScope(scopeName);
                if (scope == null)
                {
                    return TestResult.Fail("Scope 생성 실패");
                }

                // 해제
                AssetManager.Instance.ReleaseScope(scope);

                bool isReleased = scope.IsReleased;
                var retrievedScope = AssetManager.Instance.GetScope(scopeName);
                bool scopeNull = retrievedScope == null;

                bool success = isReleased && scopeNull;

                return new TestResult
                {
                    Success = success,
                    Message = $"IsReleased={isReleased}, GetScope=null:{scopeNull}"
                };
            }
            catch (Exception e)
            {
                return TestResult.Fail($"예외 발생: {e.Message}");
            }
            finally
            {
                Teardown();
            }
        }

        /// <summary>
        /// 시나리오: 동일 이름 Scope 중복 생성
        /// 같은 이름으로 CreateScope 두 번 → 동일 인스턴스 반환
        /// </summary>
        public TestResult RunDuplicateScopeScenario()
        {
            Setup();

            try
            {
                var scopeName = "DuplicateScope_" + Guid.NewGuid().ToString("N").Substring(0, 8);

                var scope1 = AssetManager.Instance.CreateScope(scopeName);
                var scope2 = AssetManager.Instance.CreateScope(scopeName);

                bool sameInstance = object.ReferenceEquals(scope1, scope2);

                // 정리
                AssetManager.Instance.ReleaseScope(scope1);

                return new TestResult
                {
                    Success = sameInstance,
                    Message = $"SameInstance={sameInstance}"
                };
            }
            catch (Exception e)
            {
                return TestResult.Fail($"예외 발생: {e.Message}");
            }
            finally
            {
                Teardown();
            }
        }

        #endregion

        #region Scenario: Cache Count

        /// <summary>
        /// 시나리오: 캐시 카운트 확인
        /// 초기 상태 CacheCount == 0
        /// </summary>
        public TestResult RunCacheCountScenario()
        {
            Setup();

            try
            {
                // 캐시 비우기
                AssetManager.Instance.ClearCache();

                int cacheCount = AssetManager.Instance.CacheCount;
                bool isEmpty = cacheCount == 0;

                return new TestResult
                {
                    Success = isEmpty,
                    Message = $"CacheCount={cacheCount}"
                };
            }
            catch (Exception e)
            {
                return TestResult.Fail($"예외 발생: {e.Message}");
            }
            finally
            {
                Teardown();
            }
        }

        #endregion

        #region Run All Scenarios

        /// <summary>
        /// 모든 시나리오 실행
        /// </summary>
        public void RunAllScenarios()
        {
            Debug.Log("[AssetManagerTest] ===== 테스트 시작 =====");

            LogResult("Initialization", RunInitializationScenario());
            LogResult("ScopeCreation", RunScopeCreationScenario());
            LogResult("ScopeRelease", RunScopeReleaseScenario());
            LogResult("DuplicateScope", RunDuplicateScopeScenario());
            LogResult("CacheCount", RunCacheCountScenario());

            Debug.Log("[AssetManagerTest] ===== 테스트 완료 =====");
        }

        private void LogResult(string scenarioName, TestResult result)
        {
            var prefix = $"[AssetManagerTest:{scenarioName}]";
            if (result.Success)
            {
                Debug.Log($"{prefix} PASS - {result.Message}");
            }
            else
            {
                Debug.LogError($"{prefix} FAIL - {result.Message}");
            }
        }

        #endregion
    }
}
