namespace Sc.Common
{
    /// <summary>
    /// 오브젝트 풀링 가능한 객체 인터페이스
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// 풀에서 꺼낼 때 호출
        /// - 타이머/상태 초기화
        /// - 시각 효과 초기화 (alpha, scale 등)
        /// - 필요한 컴포넌트 활성화
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// 풀에 반환할 때 호출
        /// - 진행 중인 작업 취소 (Tween 등)
        /// - 콜백/이벤트 정리
        /// - 참조 해제
        /// </summary>
        void OnDespawn();
    }
}
