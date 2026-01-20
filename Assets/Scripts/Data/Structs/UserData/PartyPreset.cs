using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 파티 프리셋 데이터
    /// </summary>
    [Serializable]
    public struct PartyPreset
    {
        /// <summary>
        /// 프리셋 고유 ID
        /// </summary>
        public string PresetId;

        /// <summary>
        /// 프리셋 그룹 ID (컨텐츠별 구분)
        /// 예: "main_story", "gold_dungeon_fire", "boss_raid_dragon"
        /// </summary>
        public string PresetGroupId;

        /// <summary>
        /// 유저 지정 이름
        /// </summary>
        public string Name;

        /// <summary>
        /// 파티 캐릭터 인스턴스 ID 목록 (최대 4~5명)
        /// </summary>
        public List<string> CharacterInstanceIds;

        /// <summary>
        /// 마지막 수정 시간 (Unix Timestamp)
        /// </summary>
        public long LastModifiedTime;

        /// <summary>
        /// 생성자
        /// </summary>
        public PartyPreset(string presetId, string presetGroupId, string name, List<string> characterInstanceIds, long lastModifiedTime)
        {
            PresetId = presetId;
            PresetGroupId = presetGroupId;
            Name = name;
            CharacterInstanceIds = characterInstanceIds ?? new List<string>();
            LastModifiedTime = lastModifiedTime;
        }

        /// <summary>
        /// 새 프리셋 생성
        /// </summary>
        public static PartyPreset Create(string presetGroupId, string name, List<string> characterInstanceIds)
        {
            return new PartyPreset(
                Guid.NewGuid().ToString(),
                presetGroupId,
                name,
                characterInstanceIds,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            );
        }

        /// <summary>
        /// 기본 프리셋 생성 (빈 파티)
        /// </summary>
        public static PartyPreset CreateDefault(string presetGroupId, string name = "프리셋")
        {
            return new PartyPreset(
                Guid.NewGuid().ToString(),
                presetGroupId,
                name,
                new List<string>(),
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            );
        }

        /// <summary>
        /// 파티 멤버 수
        /// </summary>
        public int MemberCount => CharacterInstanceIds?.Count ?? 0;

        /// <summary>
        /// 빈 프리셋인지 확인
        /// </summary>
        public bool IsEmpty => MemberCount == 0;

        /// <summary>
        /// 캐릭터 추가
        /// </summary>
        public PartyPreset AddCharacter(string characterInstanceId, long modifiedTime)
        {
            var newList = CharacterInstanceIds != null
                ? new List<string>(CharacterInstanceIds)
                : new List<string>();

            if (!newList.Contains(characterInstanceId))
            {
                newList.Add(characterInstanceId);
            }

            return new PartyPreset(PresetId, PresetGroupId, Name, newList, modifiedTime);
        }

        /// <summary>
        /// 캐릭터 제거
        /// </summary>
        public PartyPreset RemoveCharacter(string characterInstanceId, long modifiedTime)
        {
            var newList = CharacterInstanceIds != null
                ? new List<string>(CharacterInstanceIds)
                : new List<string>();

            newList.Remove(characterInstanceId);

            return new PartyPreset(PresetId, PresetGroupId, Name, newList, modifiedTime);
        }

        /// <summary>
        /// 캐릭터 목록 전체 교체
        /// </summary>
        public PartyPreset SetCharacters(List<string> characterInstanceIds, long modifiedTime)
        {
            return new PartyPreset(PresetId, PresetGroupId, Name,
                characterInstanceIds != null ? new List<string>(characterInstanceIds) : new List<string>(),
                modifiedTime);
        }

        /// <summary>
        /// 이름 변경
        /// </summary>
        public PartyPreset Rename(string newName, long modifiedTime)
        {
            return new PartyPreset(PresetId, PresetGroupId, newName, CharacterInstanceIds, modifiedTime);
        }

        /// <summary>
        /// 특정 캐릭터가 포함되어 있는지 확인
        /// </summary>
        public bool ContainsCharacter(string characterInstanceId)
        {
            return CharacterInstanceIds != null && CharacterInstanceIds.Contains(characterInstanceId);
        }

        public override string ToString()
        {
            return $"[{PresetId}] {Name} ({PresetGroupId}) - {MemberCount}명";
        }
    }
}
