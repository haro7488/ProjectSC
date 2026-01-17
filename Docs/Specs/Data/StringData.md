---
type: spec
assembly: Sc.Data, Sc.Core
category: MasterData
status: draft
version: "1.0"
dependencies: [DataManager]
created: 2026-01-17
updated: 2026-01-17
---

# StringData (다국어 텍스트)

## 목적

게임 내 모든 텍스트의 다국어 지원을 위한 마스터 데이터 테이블

---

## 지원 언어

| 코드 | 언어 | 우선순위 |
|------|------|----------|
| `Ko` | 한국어 | P0 (기본) |
| `En` | 영어 | P0 |

---

## 클래스 역할 정의

| 클래스 | 위치 | 역할 | 책임 |
|--------|------|------|------|
| `SystemLanguage` | Unity 내장 | 언어 열거형 | - |
| `StringData` | Sc.Data | 단일 텍스트 SO | ID, 언어별 텍스트 보유 |
| `StringDatabase` | Sc.Data | 텍스트 컬렉션 SO | ID로 StringData 조회 |
| `StringManager` | Sc.Core | 텍스트 조회 매니저 | 현재 언어 설정, 텍스트 조회 |

---

## 상세 정의

### StringData (ScriptableObject)

**위치**: `Assets/Scripts/Data/ScriptableObjects/StringData.cs`

```csharp
[CreateAssetMenu(fileName = "StringData", menuName = "SC/Data/StringData")]
public class StringData : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _ko;
    [SerializeField] private string _en;

    public string Id => _id;
    public string Ko => _ko;
    public string En => _en;

    /// <summary>
    /// 지정된 언어의 텍스트 반환
    /// </summary>
    public string GetText(SystemLanguage language)
    {
        return language switch
        {
            SystemLanguage.Korean => _ko,
            SystemLanguage.English => _en,
            _ => _en // 기본값: 영어
        };
    }
}
```

### StringDatabase (ScriptableObject)

**위치**: `Assets/Scripts/Data/ScriptableObjects/StringDatabase.cs`

```csharp
[CreateAssetMenu(fileName = "StringDatabase", menuName = "SC/Data/StringDatabase")]
public class StringDatabase : ScriptableObject
{
    [SerializeField] private List<StringData> _strings = new();

    private Dictionary<string, StringData> _lookup;

    public void Initialize()
    {
        _lookup = _strings.ToDictionary(s => s.Id, s => s);
    }

    public StringData Get(string id)
    {
        if (_lookup == null) Initialize();
        return _lookup.TryGetValue(id, out var data) ? data : null;
    }

    public string GetText(string id, SystemLanguage language)
    {
        var data = Get(id);
        return data?.GetText(language) ?? id; // 없으면 키 반환
    }
}
```

### StringManager (Sc.Core)

**위치**: `Assets/Scripts/Core/Managers/StringManager.cs`

```csharp
public class StringManager : Singleton<StringManager>
{
    private StringDatabase _database;
    private SystemLanguage _currentLanguage;

    public SystemLanguage CurrentLanguage => _currentLanguage;

    public void Initialize(StringDatabase database, SystemLanguage language)
    {
        _database = database;
        _currentLanguage = language;
        _database.Initialize();
    }

    public void SetLanguage(SystemLanguage language)
    {
        _currentLanguage = language;
        // 언어 변경 이벤트 발행 (UI 갱신용)
        EventManager.Instance.Publish(new LanguageChangedEvent { Language = language });
    }

    /// <summary>
    /// 현재 언어로 텍스트 조회
    /// </summary>
    public static string Get(string id)
    {
        if (!HasInstance) return id;
        return Instance._database.GetText(id, Instance._currentLanguage);
    }

    /// <summary>
    /// 특정 언어로 텍스트 조회
    /// </summary>
    public static string Get(string id, SystemLanguage language)
    {
        if (!HasInstance) return id;
        return Instance._database.GetText(id, language);
    }
}
```

---

## JSON 구조

**위치**: `Assets/Data/MasterData/StringData.json`

```json
[
  {
    "Id": "ui.common.confirm",
    "Ko": "확인",
    "En": "Confirm"
  },
  {
    "Id": "ui.common.cancel",
    "Ko": "취소",
    "En": "Cancel"
  },
  {
    "Id": "ui.lobby.button_gacha",
    "Ko": "소환",
    "En": "Summon"
  },
  {
    "Id": "ui.gacha.cost_single",
    "Ko": "{0} 젬으로 1회 소환",
    "En": "Summon once for {0} Gems"
  },
  {
    "Id": "error.network.timeout",
    "Ko": "서버 응답이 없습니다",
    "En": "Server not responding"
  },
  {
    "Id": "error.game.insufficient_gold",
    "Ko": "골드가 부족합니다",
    "En": "Not enough gold"
  }
]
```

---

## 키 네이밍 규칙

### 구조

```
{domain}.{category}.{name}
```

### 도메인별 예시

| 도메인 | 용도 | 예시 |
|--------|------|------|
| `ui` | UI 텍스트 | `ui.lobby.button_gacha` |
| `error` | 에러 메시지 | `error.network.timeout` |
| `item` | 아이템 이름/설명 | `item.sword_001.name` |
| `character` | 캐릭터 이름/설명 | `character.char_001.name` |
| `skill` | 스킬 이름/설명 | `skill.skill_001.name` |
| `system` | 시스템 메시지 | `system.maintenance.title` |

### UI 세부 분류

```
ui.common.*       → 공통 (확인, 취소, 닫기)
ui.title.*        → 타이틀 화면
ui.lobby.*        → 로비 화면
ui.gacha.*        → 가챠 화면
ui.shop.*         → 상점 화면
ui.character.*    → 캐릭터 화면
ui.popup.*        → 팝업
```

---

## 포맷팅 지원

### 플레이스홀더

```json
{ "Id": "ui.gacha.cost_single", "Ko": "{0} 젬으로 1회 소환", "En": "Summon once for {0} Gems" }
```

### 사용

```csharp
var template = StringManager.Get("ui.gacha.cost_single");
var text = string.Format(template, 300); // "300 젬으로 1회 소환"
```

### 확장 메서드 (선택적)

```csharp
public static class StringManagerExtensions
{
    public static string GetFormatted(string id, params object[] args)
    {
        var template = StringManager.Get(id);
        return string.Format(template, args);
    }
}
```

---

## 동작 흐름

### 초기화

```
GameBootstrap.Awake()
        │
        ▼
StringManager.Initialize(stringDatabase, SystemLanguage.Korean)
        │
        ├─ StringDatabase.Initialize() (Lookup 생성)
        └─ ErrorMessages.LocalizeFunc = StringManager.Get
```

### 텍스트 조회

```
StringManager.Get("ui.lobby.button_gacha")
        │
        ▼
StringDatabase.GetText(id, CurrentLanguage)
        │
        ▼
StringData.GetText(language)
        │
        ▼
"소환" (Ko) / "Summon" (En)
```

### 언어 변경

```
StringManager.SetLanguage(SystemLanguage.English)
        │
        ├─ _currentLanguage = English
        └─ Publish(LanguageChangedEvent)
                │
                ▼
        [UI들이 구독하여 텍스트 갱신]
```

---

## 사용 패턴

### 기본 사용

```csharp
// 단순 조회
var text = StringManager.Get("ui.common.confirm");

// 포맷팅
var cost = StringManager.Get("ui.gacha.cost_single");
var formatted = string.Format(cost, 300);
```

### UI 컴포넌트에서

```csharp
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _stringId;
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        UpdateText();
        EventManager.Instance.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
    }

    private void OnLanguageChanged(LanguageChangedEvent e)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _text.text = StringManager.Get(_stringId);
    }
}
```

### ErrorMessages 연동

```csharp
// GameBootstrap에서
ErrorMessages.LocalizeFunc = StringManager.Get;

// 이후 에러 발생 시
var result = Result<T>.Failure(ErrorCode.InsufficientGold);
// result.Message → "골드가 부족합니다" (StringManager 경유)
```

---

## 마스터 데이터 파이프라인

```
StringData.json
      │
      ▼ (MasterDataImporter)
StringDatabase.asset
      │
      └─ StringData_001.asset
      └─ StringData_002.asset
      └─ ...
```

### MasterDataImporter 확장

기존 파이프라인에 StringData 임포터 추가 필요.

---

## 주의사항

1. **키 중복 금지**: 동일 ID 여러 번 정의 시 후자 덮어씀
2. **누락 처리**: 키 없으면 키 문자열 그대로 반환 (디버그 용이)
3. **빈 문자열**: 의도적 빈 텍스트는 `""` 명시
4. **특수문자**: JSON에서 이스케이프 필요 (`\"`, `\n` 등)
5. **언어 추가 시**: StringData 필드 추가, GetText switch 확장

---

## 관련 문서

- [Data.md](../Data.md) - 마스터 데이터 개요
- [Error.md](../Foundation/Error.md) - 에러 메시지 연동
- [Event.md](../Event.md) - LanguageChangedEvent
