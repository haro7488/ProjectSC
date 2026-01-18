using System.Collections.Generic;

namespace Sc.Tests
{
    /// <summary>
    /// 테스트용 저장소 Mock.
    /// 메모리 기반으로 데이터를 저장하며, 테스트 간 격리 가능.
    /// </summary>
    public class MockSaveStorage : ISaveStorage
    {
        private readonly Dictionary<string, string> _storage = new();

        /// <summary>
        /// 저장된 키 개수
        /// </summary>
        public int Count => _storage.Count;

        /// <summary>
        /// 모든 데이터 삭제
        /// </summary>
        public void Clear()
        {
            _storage.Clear();
        }

        /// <summary>
        /// 저장된 모든 키 조회
        /// </summary>
        public IEnumerable<string> GetAllKeys()
        {
            return _storage.Keys;
        }

        public bool Save(string key, string data)
        {
            _storage[key] = data;
            return true;
        }

        public string Load(string key)
        {
            return _storage.TryGetValue(key, out var data) ? data : null;
        }

        public bool Exists(string key)
        {
            return _storage.ContainsKey(key);
        }

        public bool Delete(string key)
        {
            return _storage.Remove(key);
        }
    }
}
