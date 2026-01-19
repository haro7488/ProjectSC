namespace Sc.Core
{
    /// <summary>
    /// 에셋 핸들 내부 인터페이스.
    /// Reflection 없이 타입 독립적 해제 지원.
    /// </summary>
    public interface IAssetHandle
    {
        /// <summary>에셋 키</summary>
        string Key { get; }

        /// <summary>현재 참조 수</summary>
        int RefCount { get; }

        /// <summary>유효 여부</summary>
        bool IsValid { get; }

        /// <summary>레퍼런스 감소 및 해제 요청</summary>
        void Release();

        /// <summary>강제 해제 (내부용)</summary>
        void ForceRelease();
    }
}
