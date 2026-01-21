# 의사결정 로그

프로젝트의 주요 기술적 결정과 그 배경을 기록합니다.

---

## 문서 구조

카테고리별로 분리된 상세 의사결정 문서:

| 카테고리 | 문서 | 주요 내용 |
|----------|------|----------|
| **Architecture** | [Decisions/Architecture.md](Decisions/Architecture.md) | Assembly 모듈화, 이벤트 통신, LocalServer 분리, AssetManager |
| **UI** | [Decisions/UI.md](Decisions/UI.md) | MVP→Widget, Navigation API, Transition, SystemPopup |
| **Data** | [Decisions/Data.md](Decisions/Data.md) | 서버 중심 아키텍처, Delta 패턴, TimeService |
| **Testing** | [Decisions/Testing.md](Decisions/Testing.md) | 테스트 아키텍처, ISaveStorage, PlayMode, IItemSpawner |
| **Systems** | [Decisions/Systems.md](Decisions/Systems.md) | RewardType, TimeService, Foundation, RewardPopup |
| **Content** | [Decisions/Content.md](Decisions/Content.md) | LiveEvent, 이벤트 재화, PresetGroupId, 시즌패스 |
| **ClaudeCode** | [Decisions/ClaudeCode.md](Decisions/ClaudeCode.md) | 서브에이전트 위임, Skills, Hooks, 컨텍스트 관리 |

---

## 작성 가이드

각 의사결정은 다음 형식으로 기록:

```markdown
## [결정 제목]

**일자**: YYYY-MM-DD | **상태**: 결정됨/검토중/폐기됨

### 컨텍스트
어떤 상황에서 이 결정이 필요했는가?

### 선택지
1. **선택지 A** - 장점 / 단점
2. **선택지 B** - 장점 / 단점

### 결정
어떤 선택을 했는가? 왜?

### 결과
결정 후 어떤 영향이 있었는가?
```

---

## 최근 의사결정 (최신 5개)

### NavigationEnhancement 배지 시스템
**일자**: 2026-01-21 | [상세](Decisions/Systems.md#navigationenhancement-배지-시스템)

IBadgeProvider 인터페이스 + BadgeManager 중앙 집계. 탭별 배지 카운트 실시간 갱신.

### GachaEnhancement 소프트 천장 설계
**일자**: 2026-01-21 | [상세](Decisions/Systems.md#gachaenhancement-소프트-천장-설계)

PitySoftStart 이후 점진적 확률 증가. 서버에서 GetEffectiveSSRRate() 계산, 클라이언트는 표시만.

### CharacterEnhancement 전투력 계산 공식
**일자**: 2026-01-21 | [상세](Decisions/Systems.md#characterenhancement-전투력-계산-공식)

HP/10 + ATK×5 + DEF×3 + SPD×2 + CritRate×100 + CritDamage×50. PowerCalculator 중앙 계산.

### Stage 컴포지션 패턴
**일자**: 2026-01-20 | [상세](Decisions/Content.md#stage-컴포지션-패턴-istagecontentmodule)

StageSelectScreen + IStageContentModule. 컨텐츠별 모듈로 확장성 확보.

### Shop 구매 제한 시스템
**일자**: 2026-01-20 | [상세](Decisions/Content.md#shop-구매-제한-시스템-purchaselimitvalidator)

PurchaseLimitValidator로 Daily/Weekly/Monthly 제한. TimeService LimitType 재사용.

### 에디터 도구 Bootstrap 레벨
**일자**: 2026-01-19 | [상세](Decisions/Architecture.md#에디터-도구-bootstrap-레벨)

None/Partial/Full 레벨 체계화. EditorUIHelpers로 공용 코드 중앙화.

### SystemPopup 하이브리드 구조
**일자**: 2026-01-19 | [상세](Decisions/UI.md#systempopup-하이브리드-구조)

기존 PopupWidget 패턴 + State.Validate(). 클린 아키텍처 핵심 개념만 흡수.

---

## 핵심 원칙 (의사결정에서 도출)

1. **서버 중심 설계** - 포트폴리오여도 라이브 서비스 기준으로 설계
2. **인터페이스 추상화** - 외부 의존성(파일, 네트워크, 시간)은 항상 교체 가능하게
3. **실용적 균형** - 클린 아키텍처 핵심만 흡수, 오버엔지니어링 방지
4. **기존 패턴 일관성** - 새 기능도 기존 패턴과 동일하게 구현
5. **구현은 단순하게, 인터페이스는 확장 가능하게** - YAGNI + 확장성 균형
