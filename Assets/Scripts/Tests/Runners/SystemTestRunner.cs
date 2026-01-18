using System.Collections.Generic;
using UnityEngine;
using Sc.Foundation;

namespace Sc.Tests
{
    /// <summary>
    /// 시스템 테스트 러너 베이스 클래스.
    /// 각 시스템별 테스트 환경 구성과 정리를 담당.
    /// </summary>
    public abstract class SystemTestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] protected bool _useMockServices = true;
        [SerializeField] protected bool _isolateFromScene = false;

        protected GameObject _testRoot;
        protected TestCanvas _testCanvas;
        protected GameObject _controlPanel;

        private List<GameObject> _disabledObjects = new();
        private bool _isTestActive = false;

        /// <summary>
        /// 테스트 활성화 여부
        /// </summary>
        public bool IsTestActive => _isTestActive;

        /// <summary>
        /// 테스트 환경 설정
        /// </summary>
        public virtual void SetupTest()
        {
            if (_isTestActive)
            {
                Debug.LogWarning($"[SystemTest] {GetSystemName()} 테스트가 이미 실행 중입니다.");
                return;
            }

            _isTestActive = true;

            // 1. 테스트 루트 생성
            _testRoot = new GameObject($"[TEST] {GetSystemName()}");

            // 2. 기존 씬 격리 (선택적)
            if (_isolateFromScene)
            {
                DisableSceneObjects();
            }

            // 3. Mock 서비스 등록
            if (_useMockServices)
            {
                RegisterMockServices();
            }

            // 4. 테스트 Canvas 생성
            _testCanvas = TestCanvasFactory.Create(_testRoot.transform);

            // 5. 컨트롤 패널 생성
            CreateControlPanel();

            // 6. 시스템별 셋업
            OnSetup();

            Debug.Log($"[SystemTest] {GetSystemName()} 테스트 시작");
        }

        /// <summary>
        /// 테스트 환경 정리
        /// </summary>
        public virtual void TeardownTest()
        {
            if (!_isTestActive)
            {
                return;
            }

            OnTeardown();
            Services.Clear();
            RestoreSceneObjects();

            if (_testRoot != null)
            {
                DestroyImmediate(_testRoot);
                _testRoot = null;
            }

            _controlPanel = null;
            _isTestActive = false;

            Debug.Log($"[SystemTest] {GetSystemName()} 테스트 종료");
        }

        /// <summary>
        /// 시스템 이름 (로그용)
        /// </summary>
        protected abstract string GetSystemName();

        /// <summary>
        /// 시스템별 셋업 로직
        /// </summary>
        protected abstract void OnSetup();

        /// <summary>
        /// 시스템별 정리 로직
        /// </summary>
        protected abstract void OnTeardown();

        /// <summary>
        /// Mock 서비스 등록 (선택적 오버라이드)
        /// </summary>
        protected virtual void RegisterMockServices() { }

        /// <summary>
        /// 컨트롤 패널 UI 생성
        /// </summary>
        protected abstract void CreateControlPanel();

        /// <summary>
        /// 결과 로그 출력
        /// </summary>
        protected void LogResult(TestResult result)
        {
            if (result.Success)
            {
                Debug.Log($"[{GetSystemName()}] {result}");
            }
            else
            {
                Debug.LogError($"[{GetSystemName()}] {result}");
            }
        }

        /// <summary>
        /// 결과 로그 출력 (시나리오명 포함)
        /// </summary>
        protected void LogResult(string scenarioName, TestResult result)
        {
            var prefix = $"[{GetSystemName()}:{scenarioName}]";
            if (result.Success)
            {
                Debug.Log($"{prefix} {result}");
            }
            else
            {
                Debug.LogError($"{prefix} {result}");
            }
        }

        private void DisableSceneObjects()
        {
            foreach (var go in FindObjectsOfType<GameObject>())
            {
                if (go.activeInHierarchy && go != gameObject && go.transform.parent == null)
                {
                    go.SetActive(false);
                    _disabledObjects.Add(go);
                }
            }
        }

        private void RestoreSceneObjects()
        {
            foreach (var go in _disabledObjects)
            {
                if (go != null)
                {
                    go.SetActive(true);
                }
            }
            _disabledObjects.Clear();
        }

        private void OnDestroy()
        {
            if (_isTestActive)
            {
                TeardownTest();
            }
        }
    }
}
