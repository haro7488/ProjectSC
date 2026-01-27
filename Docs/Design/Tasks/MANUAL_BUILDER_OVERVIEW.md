# ManualBuilder 구현 작업 개요

> **목표**: Reference 이미지 + 스펙 문서 기반 ManualBuilder 구현
> **전략**: 독립적인 8개 작업으로 분리하여 병렬 진행 가능

---

## 작업 목록

| # | Screen | 작업 지시서 | 난이도 | 의존성 |
|---|--------|-------------|--------|--------|
| 1 | CharacterListScreen | [TASK_01_CharacterListScreen.md](TASK_01_CharacterListScreen.md) | 중 | 없음 |
| 2 | CharacterDetailScreen | [TASK_02_CharacterDetailScreen.md](TASK_02_CharacterDetailScreen.md) | 상 | 없음 |
| 3 | ShopScreen | [TASK_03_ShopScreen.md](TASK_03_ShopScreen.md) | 중 | 없음 |
| 4 | GachaScreen | [TASK_04_GachaScreen.md](TASK_04_GachaScreen.md) | 중 | 없음 |
| 5 | LiveEventScreen | [TASK_05_LiveEventScreen.md](TASK_05_LiveEventScreen.md) | 중 | 없음 |
| 6 | StageSelectScreen | [TASK_06_StageSelectScreen.md](TASK_06_StageSelectScreen.md) | 상 | 없음 |
| 7 | PartySelectScreen | [TASK_07_PartySelectScreen.md](TASK_07_PartySelectScreen.md) | 상 | 없음 |
| 8 | InventoryScreen | [TASK_08_InventoryScreen.md](TASK_08_InventoryScreen.md) | 중 | 없음 |

### 이미 구현된 ManualBuilder

| Screen | 파일 | 상태 |
|--------|------|------|
| TitleScreen | `TitleScreenPrefabBuilder.cs` | ✅ 완료 |
| InGameContentDashboard | `InGameContentDashboardPrefabBuilder.cs` | ✅ 완료 |
| LobbyScreen | `LobbyScreenPrefabBuilder.Generated.cs` | ✅ Generated |

---

## 실행 방법

### 단일 작업 실행
```bash
claude "Docs/Design/Tasks/TASK_01_CharacterListScreen.md 작업 진행해줘"
```

### 병렬 실행 (3개 터미널)
```bash
# Terminal 1
claude "Docs/Design/Tasks/TASK_01_CharacterListScreen.md 작업 진행해줘"

# Terminal 2
claude "Docs/Design/Tasks/TASK_03_ShopScreen.md 작업 진행해줘"

# Terminal 3
claude "Docs/Design/Tasks/TASK_06_StageSelectScreen.md 작업 진행해줘"
```

---

## ManualBuilder 구현 규칙

### 파일 위치
```
Assets/Scripts/Editor/Wizard/Generators/{ScreenName}PrefabBuilder.cs
```

### 네임스페이스
```csharp
namespace Sc.Editor.Wizard.Generators
```

### 필수 구조
```csharp
public static class {ScreenName}PrefabBuilder
{
    // 컬러 상수 (테마)
    #region Colors
    private static readonly Color BgDeep = ...;
    #endregion

    // 메인 빌드 메서드 (필수)
    public static GameObject Build()
    {
        var root = CreateRoot();
        // ... UI 구성
        ConnectSerializedFields(root, ...);
        return root;
    }

    // UI 생성 헬퍼 메서드들
    private static GameObject CreateRoot() { ... }
    private static void CreateBackground(GameObject parent) { ... }
    // ...

    // SerializeField 연결 (필수)
    private static void ConnectSerializedFields(GameObject root, ...) { ... }
}
```

### PrefabBuilderRegistry 연동

빌더는 자동으로 `PrefabBuilderRegistry`에 의해 검색됩니다:
- 클래스명: `{ScreenName}PrefabBuilder`
- 네임스페이스: `Sc.Editor.Wizard.Generators`
- 메서드: `public static GameObject Build()`

---

## 참조 리소스

### 예시 ManualBuilder
- `Assets/Scripts/Editor/Wizard/Generators/TitleScreenPrefabBuilder.cs` (497줄)
- `Assets/Scripts/Editor/Wizard/Generators/InGameContentDashboardPrefabBuilder.cs` (780줄)

### UI 헬퍼
- `Assets/Scripts/Editor/AI/EditorUIHelpers.cs` - 공통 UI 생성 유틸리티

### 스펙 문서
- `Docs/Specs/{Assembly}.md` - 각 화면의 UI 레이아웃 구조

### 레퍼런스 이미지
- `Docs/Design/Reference/{ScreenName}.jpg`

---

## 진행 상태

- [ ] TASK_01: CharacterListScreen
- [ ] TASK_02: CharacterDetailScreen
- [ ] TASK_03: ShopScreen
- [ ] TASK_04: GachaScreen
- [ ] TASK_05: LiveEventScreen
- [ ] TASK_06: StageSelectScreen
- [ ] TASK_07: PartySelectScreen
- [ ] TASK_08: InventoryScreen

---

## 완료 기준

### 각 작업 완료 조건
1. ✅ `{ScreenName}PrefabBuilder.cs` 생성
2. ✅ `Build()` 메서드 구현
3. ✅ 스펙 문서의 Prefab 계층 구조 반영
4. ✅ SerializeField 연결 완료
5. ✅ 컴파일 에러 없음

### 전체 완료 조건
1. ✅ 8개 ManualBuilder 모두 구현
2. ✅ PrefabGenerator로 프리팹 생성 가능
3. ✅ PrefabSync로 JSON Spec 생성 가능
