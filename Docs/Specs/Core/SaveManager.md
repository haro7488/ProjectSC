---
type: spec
assembly: Sc.Core
category: Manager
status: draft
version: "2.0"
dependencies: [Singleton, EventManager, ErrorCode]
created: 2025-01-14
updated: 2026-01-17
---

# 세이브 시스템 (Save System)

## 목적

게임 유저 데이터의 로컬 저장/로드, 버전 마이그레이션 지원

---

## 클래스 역할 정의

| 클래스 | 위치 | 역할 | 책임 | 비책임 |
|--------|------|------|------|--------|
| `ISaveStorage` | Sc.Core | 저장소 인터페이스 | 저장/로드 추상화 | 구체적 저장 방식 |
| `FileSaveStorage` | Sc.Core | 파일 저장소 | 로컬 파일 저장/로드 | 클라우드 저장 |
| `ISaveMigration` | Sc.Core | 마이그레이션 인터페이스 | 버전 간 변환 정의 | 마이그레이션 실행 |
| `SaveMigrator` | Sc.Core | 마이그레이션 실행기 | 버전 체크, 순차 마이그레이션 | 저장/로드 |
| `SaveManager` | Sc.Core | 세이브 매니저 | 저장/로드 조율, 마이그레이션 트리거 | 데이터 구조 정의 |

---

## 상세 정의

### ISaveStorage

**위치**: `Assets/Scripts/Core/Save/ISaveStorage.cs`

```csharp
public interface ISaveStorage
{
    void Save(string key, string json);
    string Load(string key);
    bool Exists(string key);
    void Delete(string key);
    void DeleteAll();
    string[] GetAllKeys();
}
```

### FileSaveStorage

**위치**: `Assets/Scripts/Core/Save/FileSaveStorage.cs`

**역할**: 로컬 파일 시스템 기반 저장소

```csharp
public class FileSaveStorage : ISaveStorage
{
    private readonly string _basePath;
    private const string Extension = ".sav";

    public FileSaveStorage()
    {
        _basePath = Application.persistentDataPath;
    }

    public void Save(string key, string json);
    public string Load(string key);
    public bool Exists(string key);
    public void Delete(string key);
    public void DeleteAll();
    public string[] GetAllKeys();
}
```

**저장 경로**:
```
[persistentDataPath]/
    ├─ user.sav              (유저 데이터)
    ├─ user.sav.backup       (마이그레이션 백업)
    └─ settings.sav          (설정)
```

### ISaveMigration

**위치**: `Assets/Scripts/Core/Save/ISaveMigration.cs`

```csharp
public interface ISaveMigration
{
    /// <summary>원본 버전</summary>
    int FromVersion { get; }

    /// <summary>대상 버전</summary>
    int ToVersion { get; }

    /// <summary>JSON 문자열 변환</summary>
    string Migrate(string json);
}
```

### SaveMigrator

**위치**: `Assets/Scripts/Core/Save/SaveMigrator.cs`

**역할**: 마이그레이션 순차 실행, 백업 관리

```csharp
public static class SaveMigrator
{
    // 수동 등록된 마이그레이션 목록 (버전 순서대로)
    private static readonly List<ISaveMigration> _migrations = new()
    {
        new Migration_1_2(),  // v1 → v2: EventCurrency 추가
        // 새 마이그레이션 추가 시 여기에 등록
    };

    /// <summary>
    /// 마이그레이션 필요 여부 확인
    /// </summary>
    public static bool NeedsMigration(int currentVersion, int targetVersion);

    /// <summary>
    /// 마이그레이션 실행 (자동 백업 포함)
    /// </summary>
    public static Result<string> Migrate(string json, int fromVersion, int toVersion);

    /// <summary>
    /// 백업 생성
    /// </summary>
    internal static void CreateBackup(ISaveStorage storage, string key);

    /// <summary>
    /// 백업에서 복원
    /// </summary>
    public static Result<string> RestoreFromBackup(ISaveStorage storage, string key);
}
```

### SaveManager

**위치**: `Assets/Scripts/Core/Managers/SaveManager.cs`

**역할**: 저장/로드 통합 관리

```csharp
public class SaveManager : Singleton<SaveManager>
{
    private ISaveStorage _storage;
    private const int CurrentVersion = 2;  // 현재 데이터 버전

    public void Initialize(ISaveStorage storage = null)
    {
        _storage = storage ?? new FileSaveStorage();
    }

    public Result<T> Load<T>(string key = "user") where T : IVersioned, new();
    public Result<bool> Save<T>(T data, string key = "user") where T : IVersioned;
    public bool Exists(string key);
    public void Delete(string key);
    public void DeleteAll();
}
```

### IVersioned

**위치**: `Assets/Scripts/Core/Save/IVersioned.cs`

```csharp
public interface IVersioned
{
    int Version { get; set; }
}
```

### UserSaveData 수정

**위치**: `Assets/Scripts/Data/Structs/UserSaveData.cs`

```csharp
[Serializable]
public class UserSaveData : IVersioned
{
    public int Version { get; set; } = 2;  // 현재 버전

    public UserProfile Profile;
    public UserCurrency Currency;
    public List<OwnedCharacter> Characters;
    public List<OwnedItem> Items;
    // ...
}
```

---

## 인터페이스

### SaveManager

| 메서드 | 시그니처 | 설명 |
|--------|----------|------|
| `Initialize` | `void Initialize(ISaveStorage storage = null)` | 저장소 설정 |
| `Load<T>` | `Result<T> Load<T>(string key = "user")` | 로드 + 마이그레이션 |
| `Save<T>` | `Result<bool> Save<T>(T data, string key = "user")` | 저장 |
| `Exists` | `bool Exists(string key)` | 존재 여부 |
| `Delete` | `void Delete(string key)` | 삭제 |
| `DeleteAll` | `void DeleteAll()` | 전체 삭제 |

### SaveMigrator

| 메서드 | 시그니처 | 설명 |
|--------|----------|------|
| `NeedsMigration` | `bool NeedsMigration(int current, int target)` | 마이그레이션 필요 여부 |
| `Migrate` | `Result<string> Migrate(string json, int from, int to)` | 마이그레이션 실행 |
| `RestoreFromBackup` | `Result<string> RestoreFromBackup(...)` | 백업 복원 |

---

## 동작 흐름

### 로드 + 마이그레이션

```
SaveManager.Load<UserSaveData>("user")
              │
              ▼
┌─────────────────────────────┐
│ _storage.Exists("user")?    │
│   ├─ No → return new T()    │
│   └─ Yes ↓                  │
└─────────────────────────────┘
              │
              ▼
┌─────────────────────────────┐
│ _storage.Load("user")       │
│ → JSON 문자열               │
└─────────────────────────────┘
              │
              ▼
┌─────────────────────────────┐
│ 버전 추출 (JSON 파싱)        │
│ savedVersion vs CurrentVersion │
└─────────────────────────────┘
              │
    ┌─────────┴─────────┐
    │ 같음              │ 다름
    ▼                   ▼
 역직렬화         ┌─────────────────┐
    │            │ 백업 생성        │
    │            │ user.sav.backup │
    │            └────────┬────────┘
    │                     ▼
    │            ┌─────────────────┐
    │            │ SaveMigrator    │
    │            │ .Migrate()      │
    │            └────────┬────────┘
    │                     │
    │            ┌────────┴────────┐
    │            │ 성공            │ 실패
    │            ▼                 ▼
    │         역직렬화        백업 복원 시도
    │            │                 │
    └────────────┴─────────────────┘
                 │
                 ▼
            Result<T> 반환
```

### 마이그레이션 순차 실행

```
Migrate(json, fromVersion=1, toVersion=3)
              │
              ▼
┌─────────────────────────────────────┐
│ 필요한 마이그레이션 수집             │
│ v1→v2, v2→v3                        │
└─────────────────────────────────────┘
              │
              ▼
┌─────────────────────────────────────┐
│ foreach (migration in ordered)      │
│   json = migration.Migrate(json)    │
└─────────────────────────────────────┘
              │
              ▼
         Result<string>
```

### 저장

```
SaveManager.Save(data, "user")
              │
              ▼
┌─────────────────────────────┐
│ data.Version = CurrentVersion│
│ JsonUtility.ToJson(data)    │
└─────────────────────────────┘
              │
              ▼
┌─────────────────────────────┐
│ _storage.Save("user", json) │
│   ├─ 성공 → Result.Success  │
│   └─ 실패 → Result.Failure  │
└─────────────────────────────┘
```

---

## 백업 정책

### 백업 생성 시점
- 마이그레이션 실행 직전 (자동)

### 백업 파일
- `{key}.sav.backup` (1개만 유지)
- 마이그레이션 성공 시 이전 백업 덮어씀

### 복원 시점
- 마이그레이션 실패 시 자동 시도
- 수동 복원 API 제공

---

## 마이그레이션 예시

### Migration_1_2 (v1 → v2)

```csharp
public class Migration_1_2 : ISaveMigration
{
    public int FromVersion => 1;
    public int ToVersion => 2;

    public string Migrate(string json)
    {
        // JSON 파싱 (JObject 또는 Dictionary)
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        // EventCurrency 필드 추가
        if (!data.ContainsKey("EventCurrency"))
        {
            data["EventCurrency"] = new List<object>();
        }

        // 버전 업데이트
        data["Version"] = 2;

        return JsonConvert.SerializeObject(data);
    }
}
```

### 새 마이그레이션 추가 절차

1. `Migration_X_Y.cs` 클래스 생성
2. `ISaveMigration` 구현
3. `SaveMigrator._migrations`에 등록
4. `SaveManager.CurrentVersion` 업데이트

---

## 사용 패턴

### 초기화

```csharp
// GameBootstrap에서
SaveManager.Instance.Initialize();
```

### 로드

```csharp
var result = SaveManager.Instance.Load<UserSaveData>();

if (result.IsSuccess)
{
    var userData = result.Value;
}
else
{
    Log.Error($"로드 실패: {result.Message}", LogCategory.Data);
}
```

### 저장

```csharp
var result = SaveManager.Instance.Save(userData);

if (result.IsFailure)
{
    Log.Error($"저장 실패: {result.Message}", LogCategory.Data);
}
```

---

## 에러 코드

| ErrorCode | 설명 |
|-----------|------|
| `SaveFailed` (3001) | 파일 저장 실패 |
| `LoadFailed` (3002) | 파일 로드 실패 |
| `ParseFailed` (3003) | JSON 파싱 실패 |
| `MigrationFailed` (3004) | 마이그레이션 실패 |

---

## 주의사항

1. **Dictionary 직렬화**: JsonUtility 미지원, Newtonsoft.Json 또는 래퍼 사용
2. **마이그레이션 순서**: FromVersion 기준 오름차순 등록 필수
3. **버전 건너뛰기 금지**: v1→v3 직접 불가, v1→v2→v3 순차 실행
4. **백업 용량**: 마이그레이션 시 일시적으로 2배 용량 사용
5. **테스트**: 각 마이그레이션은 단위 테스트 권장

---

## 폴더 구조

```
Assets/Scripts/Core/
├── Managers/
│   └── SaveManager.cs
└── Save/
    ├── ISaveStorage.cs
    ├── FileSaveStorage.cs
    ├── IVersioned.cs
    ├── ISaveMigration.cs
    ├── SaveMigrator.cs
    └── Migrations/
        ├── Migration_1_2.cs
        └── Migration_2_3.cs
```

---

## 관련 문서

- [Core.md](../Core.md) - 상위 문서
- [Foundation/Error.md](../Foundation/Error.md) - Result<T>, ErrorCode
- [Data.md](../Data.md) - UserSaveData 구조
