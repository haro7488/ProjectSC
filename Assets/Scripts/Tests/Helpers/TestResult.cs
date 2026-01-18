namespace Sc.Tests
{
    /// <summary>
    /// 테스트 결과를 나타내는 구조체
    /// </summary>
    public struct TestResult
    {
        public bool Success;
        public string Message;
        public float ElapsedMs;

        public static TestResult Pass(string message = null)
        {
            return new TestResult
            {
                Success = true,
                Message = message ?? "PASS"
            };
        }

        public static TestResult Fail(string message)
        {
            return new TestResult
            {
                Success = false,
                Message = message
            };
        }

        public override string ToString()
        {
            var status = Success ? "PASS" : "FAIL";
            var timeInfo = ElapsedMs > 0 ? $" ({ElapsedMs:F1}ms)" : "";
            return $"[{status}] {Message}{timeInfo}";
        }
    }

    /// <summary>
    /// 테스트 시나리오 실행 상태
    /// </summary>
    public enum TestStatus
    {
        NotStarted,
        Running,
        Passed,
        Failed,
        Skipped
    }

    /// <summary>
    /// 테스트 시나리오 정보
    /// </summary>
    public struct TestScenarioInfo
    {
        public string Name;
        public string Description;
        public TestStatus Status;
        public TestResult Result;
    }
}
