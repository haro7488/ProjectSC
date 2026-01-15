# 진행 상황

## 상태 범례
- ⬜ 대기 | 🔨 진행 중 | ✅ 완료

---

## 현재 Phase: 1 - 기반 레이어 구현

### Phase 0 - 프로젝트 구조 설정 ✅

### 기반 레이어

| Assembly | 구조 | 구현 | 설명 |
|----------|------|------|------|
| Sc.Data | ✅ | ⬜ | 순수 데이터 정의 |
| Sc.Event | ✅ | ⬜ | 클라이언트 내부 이벤트 |
| Sc.Packet | ✅ | ⬜ | 서버 통신 인터페이스 |
| Sc.Core | ✅ | ⬜ | 핵심 시스템 |
| Sc.Common | ✅ | 🔨 | 공통 모듈 (UI 시스템 진행 중) |

### Editor (빌드 제외)

| Assembly | 도구 | 상태 | 설명 |
|----------|------|------|------|
| Sc.Editor.AI | UITestSceneSetup | ✅ | UI 테스트 씬/프리팹 자동 생성 |
| Sc.Editor.AI | NavigationDebugWindow | ✅ | Navigation 상태 시각화 윈도우 |

### Contents - Shared

| Assembly | 구조 | 구현 |
|----------|------|------|
| Sc.Contents.Character | ✅ | ⬜ |
| Sc.Contents.Inventory | ✅ | ⬜ |

### Contents - InGame

| Assembly | 구조 | 구현 |
|----------|------|------|
| Sc.Contents.Battle | ✅ | ⬜ |
| Sc.Contents.Skill | ✅ | ⬜ |

### Contents - OutGame

| Assembly | 구조 | 구현 |
|----------|------|------|
| Sc.Contents.Lobby | ✅ | ⬜ |
| Sc.Contents.Gacha | ✅ | ⬜ |
| Sc.Contents.Shop | ✅ | ⬜ |
| Sc.Contents.Quest | ✅ | ⬜ |

---

## 현재 진행 중인 작업

### Navigation 가시성 시스템 구현 ✅

**배경**: Screen/Popup 통합 스택 구조에서 가시성 제어 필요

**완료**:
- [x] Navigation.md 문서화 (v2.0) - 통합 스택, 가시성 규칙
- [x] UISystem.md 문서화 (v3.1) - Canvas.enabled 기반 가시성
- [x] INavigationContext 인터페이스 생성
- [x] NavigationManager 통합 스택 구조 변경
- [x] UITestSetup 통합 스택 구조 변경
- [x] Widget.cs - Canvas.enabled 기반 Show/Hide 구현
- [x] NavigationManager.cs - RefreshVisibility() 추가
- [x] UITestSetup.cs - RefreshVisibility() 추가

**핵심 규칙**:
```
1. 가시성 경계 = 마지막 Screen 인덱스
2. index >= boundary → Show, else → Hide
3. Canvas.enabled 사용 (SetActive 금지)
4. Push/Pop 후 RefreshVisibility() 호출
```

**관련 문서**: [Navigation.md](Specs/Navigation.md), [UISystem.md](Specs/Common/UISystem.md)

---

## 작업 로그

### 2025-01-16
- [x] Navigation 가시성 시스템 구현 완료
  - [x] Widget.cs - Canvas.enabled 기반 Show/Hide (Canvas 없으면 SetActive fallback)
  - [x] NavigationManager.cs - RefreshVisibility() (마지막 Screen 기준 가시성)
  - [x] UITestSetup.cs - RefreshVisibility() 동일 패턴 적용

### 2025-01-15
- [x] UI 시스템 테스트 환경 구축
  - [x] TestScreen, TestPopup 클래스 생성
  - [x] UITestSetup 테스트 러너 생성
  - [x] UITestSceneSetup Editor 스크립트 생성
  - [x] AI 도구 문서 생성 (Docs/Specs/Editor/AITools.md)
- [x] CLAUDE.md에 Progress 추적 필수 지침 추가
- [x] UI 테스트 버튼 동작 구현
  - [x] TestScreen/TestPopup 버튼 연결
  - [x] UITestSetup에서 스택 기반 Screen/Popup 관리
- [x] NavigationDebugWindow 에디터 윈도우 생성
- [x] Navigation 통합 스택 구조로 변경
  - [x] INavigationContext 인터페이스 생성
  - [x] ScreenWidget.Context, PopupWidget.Context 인터페이스 구현
  - [x] NavigationManager 통합 스택 변경
  - [x] UITestSetup 통합 스택 변경
  - [x] NavigationDebugWindow 통합 스택 표시
- [x] 가시성 규칙 설계 및 문서화
  - [x] Navigation.md v2.0 업데이트
  - [x] UISystem.md v3.1 업데이트

### 2025-01-14
- [x] 프로젝트 문서 구조 설정
- [x] CLAUDE.md 생성
- [x] Assembly 기반 아키텍처 설계
- [x] 스펙 문서 작성 완료
- [x] 폴더 구조 생성
- [x] Assembly Definition 파일 생성 (13개)
- [x] Event/Packet 분리 (클라이언트 이벤트 vs 서버 통신)
- [x] Sc.Event Assembly 추가
- [x] Sc.Packet 서버 통신 인터페이스 재설계
