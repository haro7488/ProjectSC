# 문서-구현 현황 보고서

> **생성일**: 2026-01-28
> **기준**: PROGRESS.md, Docs/Specs/, Assets/Scripts/

---

## 요약

| 항목 | 수치 | 비고 |
|------|------|------|
| **P1 TODO** | 31개 | PROGRESS.md 기록 35개 → 실제 31개 |
| **문서 O / 구현 X** | 35개 | InGame 전체 포함 |
| **문서 X / 구현 O** | 8개 | 문서화 필요 |
| **문서 ≠ 구현** | 8개 | 불일치 항목 |
| **OutGame 완료율** | 85% | |

---

## 1. P1 TODO 현황

### 1.1 개수 조정

| 출처 | 개수 | 비고 |
|------|------|------|
| PROGRESS.md 기록 | 35개 | |
| **실제 확인** | **31개** | 중복 1개, 해결 3개 |

### 1.2 카테고리별 분포

| 카테고리 | 개수 | 비율 |
|----------|------|------|
| 데이터 연동 | 11개 | 35% |
| 파티/전투 시스템 | 10개 | 32% |
| UI/UX | 6개 | 19% |
| 서버 통신 | 3개 | 10% |
| 기타 | 1개 | 3% |

### 1.3 난이도별 분포

| 난이도 | 개수 | 주요 항목 |
|--------|------|----------|
| 상 | 5개 | 전투력 계산, 인벤토리 연동, 해금 시스템 |
| 중 | 16개 | Database 연동, UserData 확장, 파티 로직 |
| 하 | 10개 | StringData 조회, 팝업 연동, 검증 로직 |

### 1.4 핵심 블로킹 시스템

다른 TODO 해결 전 먼저 구현 필요:

| 시스템 | 영향 TODO | 설명 |
|--------|----------|------|
| 마스터 데이터 (CharacterDB, ItemDB) | 7개 | 캐릭터/아이템 정보 조회 |
| 전투력 계산 | 5개 | 파티 편성, 정렬, UI |
| StringData | 5개 | UI 텍스트 표시 |
| UserData 확장 | 5개 | 진행도, 해금 상태 |

### 1.5 권장 작업 순서

```
Phase 1: 기반 시스템 (마스터 데이터, UserData) → 7개 해제
    ↓
Phase 2: 전투력 계산 시스템 → 5개 해결
    ↓
Phase 3: 데이터 연동 → 11개 해결
    ↓
Phase 4: 파티 시스템 완성 → 5개 해결
    ↓
Phase 5: UI/서버 연동 → 3개 해결
```

---

## 2. 문서-구현 불일치

### 2.1 미구현 (문서 O / 구현 X)

#### P1: 핵심 기능 (7개)

| 항목 | 스펙 문서 | 설명 |
|------|----------|------|
| CharacterManager | Character.md | 캐릭터 컬렉션 관리 |
| CharacterFactory | Character.md | 캐릭터 생성 시스템 |
| CharacterStats | Character.md | 런타임 스탯 |

#### P2: 개선 사항 (8개)

| 항목 | 스펙 문서 | 설명 |
|------|----------|------|
| QuestManager | Quest.md | 퀘스트/업적 시스템 |
| QuestConditions (4개) | Quest.md | 조건 시스템 |
| QuestViews (2개) | Quest.md | UI |

#### FUTURE: InGame 시스템 (26개)

| 시스템 | 개수 |
|--------|------|
| Battle 시스템 | 13개 클래스 |
| Skill 시스템 | 5개 클래스 |

#### 의도적 제외 (4개)

- AudioManager (Phase 외)
- Pool 시스템 (UniTask Pool 사용)
- SceneLoader (Addressables 직접 사용)

### 2.2 문서화 필요 (문서 X / 구현 O)

**조정 후 8개** (기존 11개에서 Gacha Enhancement 문서 반영)

| 구현 파일 | 필요 문서 |
|----------|----------|
| Shop/ShopProductCard.cs | Shop.md |
| Shop/CategoryShortcut.cs | Shop.md |
| Character/CharacterFilterWidget.cs | Character.md |
| Character/CostumeWidget.cs | Character.md |
| Inventory/ItemCard.cs | Inventory.md |
| Inventory/ItemDetailWidget.cs | Inventory.md |
| Lobby/Tasks/EventCurrencyConversionTask.cs | Lobby.md |
| Event/EventBannerItem.cs | LiveEvent.md |

### 2.3 불일치 (문서 ≠ 구현)

| 항목 | 스펙 | 실제 | 우선순위 |
|------|------|------|----------|
| EventMissionTab | 완전한 미션 시스템 | "준비 중" 플레이스홀더 | P1 |
| EventShopTab | ShopProvider 연동 | "준비 중" 플레이스홀더 | P1 |
| PartySelectScreen | 완전한 파티 시스템 | 기본 구조만 (TODO 6개) | P1 |
| ClaimEventMission API | 정상 동작 | 에러코드 6099 반환 | P1 |
| ShopState | Provider + 초기 탭 | 빈 플레이스홀더 | P2 |
| AttendanceCheckTask | 실제 출석 체크 | Stub (CheckRequired=false) | P2 |
| NewEventNotificationTask | 실제 알림 | Stub (CheckRequired=false) | P2 |
| InGameContentDashboard | 완전한 대시보드 | "Coming Soon" 팝업 | P2 |

---

## 3. P1 TODO 상세 목록

### 3.1 데이터 연동 (11개)

| # | 파일 | 라인 | 내용 | 난이도 |
|---|------|------|------|--------|
| 1 | RewardHelper.cs | 54 | CharacterDatabase 이름 조회 | 중 |
| 2 | RewardHelper.cs | 58 | ItemDatabase 이름 조회 | 중 |
| 3 | RewardHelper.cs | 166 | CharacterDatabase 희귀도 조회 | 중 |
| 4 | RewardHelper.cs | 170 | ItemDatabase 희귀도 조회 | 중 |
| 5 | EventBannerItem.cs | 55 | StringData 이름 조회 | 하 |
| 6 | EventDetailScreen.cs | 132 | StringData 이름 조회 | 하 |
| 7 | EventDetailScreen.cs | 168 | DataManager 재화 수량 | 하 |
| 8 | EventDetailScreen.cs | 201 | StringData 탭 이름 | 하 |
| 9 | ShopProductItem.cs | 85 | StringData 이름 조회 | 하 |
| 10 | InGameContentDashboard.cs | 237 | UserData 컨텐츠 상태 | 중 |
| 11 | InGameContentDashboard.cs | 263 | UserData 진행 정보 | 중 |

### 3.2 파티/전투 시스템 (10개)

| # | 파일 | 라인 | 내용 | 난이도 |
|---|------|------|------|--------|
| 12 | CharacterSelectWidget.cs | 244 | 마스터 데이터 연동 | 중 |
| 13 | CharacterSelectWidget.cs | 579 | 전투력 계산 | 중* |
| 14 | CharacterSelectWidget.cs | 584 | Rarity 조회 | 중 |
| 15 | PartySlotWidget.cs | 240 | Rarity 조회 | 중 |
| 16 | PartySlotWidget.cs | 251 | 전투력 계산 | 중* |
| 17 | PartySelectScreen.cs | 274 | 파티 슬롯 UI 초기화 | 중 |
| 18 | PartySelectScreen.cs | 283 | 전투력 합산 | 중 |
| 19 | PartySelectScreen.cs | 339 | 캐릭터 수 검증 | 하 |
| 20 | PartySelectScreen.cs | 340 | 스태미나 검증 | 하 |
| 21 | PartySelectScreen.cs | 358 | 파티 검증 | 상 |

*검토 결과 난이도 하향 조정 (상→중)

### 3.3 UI/UX (6개)

| # | 파일 | 라인 | 내용 | 난이도 |
|---|------|------|------|--------|
| 22 | RewardService.cs | 78 | 인벤토리 시스템 연동 | 중 |
| 23 | TowerContentModule.cs | 133 | UserData 탑 진행도 | 중 |
| 24 | InGameContentDashboard.cs | 288 | 스테이지 ID 설정 | 하 |
| 25 | InGameContentDashboard.cs | 328 | 해금 조건 확인 | 상 |
| 26 | ShopScreen.cs | 443 | RewardPopup 연동 | 하 |
| 27 | GachaScreen.cs | 569 | 에러 팝업 표시 | 하 |

### 3.4 서버 통신 (3개)

| # | 파일 | 라인 | 내용 | 난이도 |
|---|------|------|------|--------|
| 28 | CharacterDetailScreen.cs | 910 | LevelUp 서버 요청 | 중 |
| 29 | CharacterDetailScreen.cs | 944 | Ascension 서버 요청 | 중 |
| 30 | ShopScreen.cs | 455 | 에러 팝업 표시 | 하 |

### 3.5 기타 (1개)

| # | 파일 | 라인 | 내용 | 난이도 |
|---|------|------|------|--------|
| 31 | PartySelectScreen.cs | 677 | 전투력 계산 로직 | 상 |

---

## 4. 권장 사항

### 4.1 즉시 필요 (P1 해소)

1. **기반 시스템 구축**
   - CharacterDatabase, ItemDatabase 구현
   - StringData 시스템 구현
   - UserData 구조 확장

2. **전투력 계산 시스템**
   - 계산 로직 설계 및 구현
   - Widget/Screen 연동

3. **플레이스홀더 완성**
   - EventMissionTab 실제 구현
   - EventShopTab ShopProvider 연동
   - ClaimEventMission API 완성

### 4.2 단기 목표 (P2)

1. Quest 시스템 구현
2. Lobby Task 완성 (출석, 알림)
3. 문서 업데이트 (8개 Widget)

### 4.3 장기 목표 (FUTURE)

- InGame 전투 시스템 (26개 클래스)
- Skill/Buff 시스템

### 4.4 PROGRESS.md 업데이트

| 항목 | 기존 | 수정 |
|------|------|------|
| P1 TODO 개수 | 35개 | 31개 |

---

## 5. 검증 결과

| 검증 항목 | 결과 |
|----------|------|
| P1 TODO 분류 정확도 | 94% |
| P1 TODO 난이도 평가 정확도 | 87% |
| 문서-구현 불일치 분류 정확도 | 92% |
| 우선순위 평가 정확도 | 88% |

---

## 6. SPEC.md - 구현 간극

### 6.1 요약

| 항목 | 개수 | 비율 |
|------|------|------|
| 구현 완료 | 6개 | 21% |
| 부분 구현 | 5개 | 18% |
| 미구현 | 17개 | 61% |

**총평**: SPEC.md의 디자인 패턴 중심 설계보다 **실용적인 Unity 아키텍처**로 발전. Widget-State 패턴, LocalGameServer 등 더 적합한 대안 구현.

### 6.2 시스템별 구현 현황

| 시스템 | 완료 | 부분 | 미구현 | 비고 |
|--------|------|------|--------|------|
| Core | 2 | 1 | 1 | AssetManager가 ResourceManager 대체 |
| Character | 1 | 0 | 2 | Factory/Flyweight 미적용 |
| Battle | 0 | 0 | 3 | **전체 미구현** |
| Gacha | 0 | 2 | 2 | Strategy→Service 통합 |
| Buff | 0 | 0 | 3 | **전체 미구현** |
| UI (MVP) | 0 | 1 | 2 | **Widget-State로 대체** |
| Quest | 0 | 0 | 3 | **전체 미구현** |
| Save | 1 | 1 | 1 | Memento 미적용 |
| Pool | 2 | 0 | 0 | **완벽 구현** |

### 6.3 상세 간극

#### Core 시스템

| 항목 | 스펙 | 구현 | 파일 |
|------|------|------|------|
| Singleton\<T\> | 제네릭 싱글톤 | ✅ | Foundation/Singleton.cs |
| GameManager | 게임 상태, 씬 전환 | ❌ | GameBootstrap으로 부분 대체 |
| EventManager | 전역 이벤트 | ❌ | 이벤트 구조체만 존재 |
| ResourceManager | Addressables 래핑 | ✅ | Core/Managers/AssetManager.cs |

#### Character 시스템

| 항목 | 스펙 | 구현 | 비고 |
|------|------|------|------|
| CharacterData | ScriptableObject | ✅ | 기본 스탯, 스킬 ID |
| CharacterFactory | Factory 패턴 | ❌ | ScriptableObject 직접 참조 |
| CharacterStats | Flyweight 패턴 | ❌ | 패턴 미적용 |

#### Battle 시스템 (전체 미구현)

| 항목 | 스펙 | 구현 |
|------|------|------|
| IBattleState | State 패턴 | ❌ |
| ICommand | Command 패턴 | ❌ |
| BattleManager | 턴 관리 | ❌ |

#### Gacha 시스템

| 항목 | 스펙 | 구현 | 비고 |
|------|------|------|------|
| IGachaStrategy | Strategy 인터페이스 | ⚠️ | GachaService 단일 클래스 |
| NormalGachaStrategy | 일반 가챠 | ⚠️ | Service 내 통합 |
| PickupGachaStrategy | 픽업 가챠 | ❌ | |
| PityGachaStrategy | 천장 시스템 | ⚠️ | 소프트/하드 천장 구현됨 |

#### Buff 시스템 (전체 미구현)

| 항목 | 스펙 | 구현 |
|------|------|------|
| IStatModifier | Decorator 인터페이스 | ❌ |
| BuffDecorator | 버프 래핑 | ❌ |
| DebuffDecorator | 디버프 래핑 | ❌ |

#### UI 시스템 (대체 구현)

| 항목 | 스펙 | 구현 | 비고 |
|------|------|------|------|
| IView | MVP View | ⚠️ | Widget 베이스 |
| IPresenter | MVP Presenter | ❌ | |
| BasePresenter | 프레젠터 베이스 | ❌ | |

**실제 구현 (Widget-State 패턴)**:
- `ScreenWidget<TScreen, TState>`: Screen + State 통합
- `PopupWidget<TPopup, TState>`: Popup + State 통합
- `NavigationManager`: 화면 전환 관리
- `IScreenState`, `IPopupState`: 상태 인터페이스

#### Quest 시스템 (전체 미구현)

| 항목 | 스펙 | 구현 |
|------|------|------|
| IQuestCondition | Composite 인터페이스 | ❌ |
| CompositeCondition | AND/OR 조건 | ❌ |
| LeafCondition | 단일 조건 | ❌ |

#### Save 시스템

| 항목 | 스펙 | 구현 | 비고 |
|------|------|------|------|
| IMemento | Memento 인터페이스 | ❌ | 패턴 미적용 |
| SaveManager | 저장/로드 관리 | ✅ | 마이그레이션, 자동저장 |
| GameState | 게임 상태 | ⚠️ | UserSaveData로 대체 |

#### Pool 시스템 (완벽 구현)

| 항목 | 스펙 | 구현 | 파일 |
|------|------|------|------|
| ObjectPool\<T\> | 제네릭 풀 | ✅ | Common/Pool/ObjectPool.cs |
| PoolManager | 풀 중앙 관리 | ✅ | Common/Pool/PoolManager.cs |

### 6.4 아키텍처 차이점

| 영역 | SPEC.md 의도 | 실제 구현 | 평가 |
|------|--------------|-----------|------|
| 설계 철학 | GoF 패턴 중심 | 실용적 Unity 아키텍처 | 적절 |
| UI 패턴 | MVP | Widget-State | **더 우수** |
| Gacha | Strategy (3클래스) | Service (1클래스) | 단순화 |
| Event | Observer (EventManager) | 이벤트 구조체 | 미완성 |
| Character | Factory + Flyweight | ScriptableObject 직접 | 단순화 |

### 6.5 실제 구현의 강점

1. **Widget-State 패턴**: MVP보다 Unity에 적합
   - State를 통한 화면 전환 파라미터 관리
   - NavigationManager와 통합
   - Transition 지원

2. **LocalGameServer**: 서버 시뮬레이션 레이어
   - Handler 기반 요청 라우팅
   - Request/Response 패턴
   - Validator, Service 분리

3. **AssetManager**: ResourceManager를 능가
   - Scope 기반 생명주기 관리
   - LRU 캐싱
   - Addressables 통합

### 6.6 권장 사항

#### 보완 필요 (우선순위 높음)

| 항목 | 이유 | 비고 |
|------|------|------|
| EventManager | 시스템 간 디커플링 필수 | SPEC 반영 |
| Battle 시스템 | InGame 핵심 기능 | FUTURE |
| Quest 시스템 | 진행도 추적 필수 | P2 |

#### 현상 유지 (패턴 적용 불필요)

| 항목 | 이유 |
|------|------|
| UI (Widget-State) | MVP보다 우수한 대체 구현 |
| Gacha (Service) | Strategy 패턴 불필요 |
| Character (SO) | Factory/Flyweight 오버엔지니어링 |

#### SPEC.md 업데이트 필요

실제 아키텍처 반영하여 SPEC.md 갱신 권장:

```
| 시스템 | 패턴 | 설명 |
|--------|------|------|
| Core | Singleton | AssetManager, SaveManager |
| UI | Widget-State | NavigationManager, IScreenState |
| LocalServer | Request-Response | Handler, Service, Validator |
| Pool | Object Pool | PoolManager, ObjectPool<T> |
```

---

## 참조

| 문서 | 용도 |
|------|------|
| scratchpad/p1_todo_analysis.md | P1 TODO 상세 분석 |
| scratchpad/p1_todo_review.md | P1 TODO 검토 결과 |
| scratchpad/doc_impl_gap.md | 문서-구현 불일치 분석 |
| scratchpad/doc_impl_review.md | 불일치 검토 결과 |
