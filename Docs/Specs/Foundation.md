---
type: spec
assembly: Sc.Foundation
category: System
status: draft
version: "1.0"
dependencies: []
created: 2026-01-17
updated: 2026-01-17
---

# Sc.Foundation

## 목적

게임 전체에서 사용되는 최하위 기반 시스템 (싱글턴, 이벤트, 로깅, 에러 처리)

## 의존성

- 참조: 없음 (최하위 레이어)
- 참조됨: Sc.Core, Sc.Data, Sc.Event, Sc.Packet, Sc.Common, 모든 Contents

## 클래스 역할 정의

### 기존 구현

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| `Singleton<T>` | 전역 싱글턴 베이스 | 단일 인스턴스 보장, DontDestroyOnLoad | 비즈니스 로직 |
| `EventManager` | 이벤트 버스 | 이벤트 발행/구독 관리 | 이벤트 정의, 핸들러 로직 |

### Phase 0 추가

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| `LogLevel` | 로그 레벨 열거형 | Debug/Info/Warning/Error 분류 | - |
| `LogCategory` | 로그 카테고리 열거형 | System/Network/UI/Battle 등 분류 | - |
| `Log` | 로깅 정적 유틸리티 | 로그 출력 진입점, 레벨/카테고리 필터링 | 출력 대상 관리 |
| `ILogOutput` | 로그 출력 인터페이스 | 출력 대상 추상화 | 구체적 출력 로직 |
| `UnityLogOutput` | Unity 콘솔 출력 | Debug.Log 호출 | 파일/원격 출력 |
| `LogConfig` | 로그 설정 | 카테고리별 레벨 설정, 출력 대상 관리 | - |
| `ErrorCode` | 에러 코드 열거형 | 에러 종류 분류 | 에러 메시지 |
| `ErrorMessages` | 에러 메시지 매핑 | ErrorCode → 사용자 메시지 변환 | 에러 처리 로직 |
| `Result<T>` | 결과 래퍼 구조체 | 성공/실패 명시적 전달 | 에러 복구 |

### 서비스/저장소 (Phase A~E 추가)

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| `Services` | ServiceLocator 패턴 | 전역 서비스 등록/조회 | 비즈니스 로직 |
| `ISaveStorage` | 저장소 추상화 | Save/Load/Exists/Delete 계약 | 구체적 저장 방식 |
| `FileSaveStorage` | 파일 저장소 구현 | persistentDataPath 파일 저장 | 클라우드/메모리 저장 |

---

## 관계도

```
┌─────────────────────────────────────────────────────────┐
│                    Sc.Foundation                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌─────────────┐     ┌──────────────┐                  │
│  │ Singleton<T>│     │ EventManager │                  │
│  └─────────────┘     └──────────────┘                  │
│        ▲                    ▲                          │
│        │ 상속               │ 사용                      │
│        │                    │                          │
│  ┌─────┴─────┬─────────────┴───────┐                  │
│  │  모든 Manager (Core, Contents)   │                  │
│  └──────────────────────────────────┘                  │
│                                                         │
│  ┌─────────────────────────────────────────────┐       │
│  │              로깅 시스템                      │       │
│  │  ┌─────────┐  ┌───────────┐  ┌───────────┐  │       │
│  │  │LogLevel │  │LogCategory│  │ LogConfig │  │       │
│  │  └─────────┘  └───────────┘  └─────┬─────┘  │       │
│  │                                    │        │       │
│  │  ┌─────────────────────────────────▼─────┐  │       │
│  │  │                 Log                   │  │       │
│  │  └─────────────────────────────────┬─────┘  │       │
│  │                                    │        │       │
│  │  ┌────────────┐    ┌───────────────▼─────┐  │       │
│  │  │ ILogOutput │◄───│   UnityLogOutput    │  │       │
│  │  └────────────┘    └─────────────────────┘  │       │
│  └─────────────────────────────────────────────┘       │
│                                                         │
│  ┌─────────────────────────────────────────────┐       │
│  │              에러 처리                       │       │
│  │  ┌───────────┐  ┌───────────────┐           │       │
│  │  │ ErrorCode │  │ ErrorMessages │           │       │
│  │  └─────┬─────┘  └───────┬───────┘           │       │
│  │        │                │                   │       │
│  │        ▼                ▼                   │       │
│  │  ┌─────────────────────────────────────┐   │       │
│  │  │            Result<T>                │   │       │
│  │  └─────────────────────────────────────┘   │       │
│  └─────────────────────────────────────────────┘       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## Services (ServiceLocator)

간단한 ServiceLocator 패턴 구현으로 전역 서비스 관리.

```csharp
// 등록
Services.Register<IUserService>(new UserService());

// 조회
var service = Services.Get<IUserService>();

// 안전 조회
if (Services.TryGet<IUserService>(out var svc)) { }

// 테스트 정리
Services.Clear();
```

| 메서드 | 설명 |
|--------|------|
| `Register<T>(T service)` | 서비스 등록 (중복 시 덮어씀) |
| `Get<T>()` | 서비스 조회 (없으면 null) |
| `TryGet<T>(out T service)` | 안전한 서비스 조회 |
| `Unregister<T>()` | 서비스 제거 |
| `Clear()` | 전체 초기화 |
| `Count` | 등록된 서비스 수 |

---

## ISaveStorage / FileSaveStorage

저장소 추상화 인터페이스 및 파일 저장소 구현.

```csharp
// 초기화
var storage = new FileSaveStorage();
Services.Register<ISaveStorage>(storage);

// 저장/로드
var saveResult = storage.Save("player", jsonData);
var loadResult = storage.Load("player");

// 존재 확인/삭제
if (storage.Exists("player"))
    storage.Delete("player");
```

| 메서드 | 반환 | 설명 |
|--------|------|------|
| `Save(key, data)` | `Result<bool>` | JSON 데이터 저장 |
| `Load(key)` | `Result<string>` | JSON 데이터 로드 |
| `Exists(key)` | `bool` | 존재 여부 |
| `Delete(key)` | `Result<bool>` | 데이터 삭제 |

**FileSaveStorage 특징:**
- 저장 경로: `Application.persistentDataPath/{key}.json`
- 테스트용 생성자: `FileSaveStorage(string basePath)`
- 에러 코드: `SaveFailed`, `LoadFailed`

---

## 설계 원칙

1. **무의존성**: Foundation은 어떤 Assembly도 참조하지 않음
2. **확장 가능성**: ILogOutput으로 출력 대상 확장 가능 (File, Remote 등)
3. **빌드 최적화**: 릴리즈 빌드에서 Debug 레벨 로그 자동 strip
4. **명시적 에러 처리**: Result<T>로 성공/실패를 타입 수준에서 강제
5. **런타임 설정**: LogConfig로 카테고리별 로그 레벨 런타임 조정 가능

---

## 폴더 구조

```
Assets/Scripts/Foundation/
├── Sc.Foundation.asmdef
├── Singleton.cs              # 기존
├── EventManager.cs           # 기존
├── IsExternalInit.cs         # 기존 (C# 9 지원)
├── Services.cs               # ServiceLocator 패턴
├── ISaveStorage.cs           # 저장소 인터페이스
├── FileSaveStorage.cs        # 파일 저장소 구현
│
├── Logging/                  # Phase 0 추가
│   ├── LogLevel.cs
│   ├── LogCategory.cs
│   ├── Log.cs
│   ├── LogConfig.cs
│   ├── ILogOutput.cs
│   └── UnityLogOutput.cs
│
└── Error/                    # Phase 0 추가
    ├── ErrorCode.cs
    ├── ErrorMessages.cs
    └── Result.cs
```

---

## 상세 문서

| 문서 | 상태 | 설명 |
|------|------|------|
| [Singleton.md](Foundation/Singleton.md) | ⬜ 대기 | 싱글턴 베이스 클래스 |
| [EventManager.md](Foundation/EventManager.md) | ⬜ 대기 | 이벤트 버스 |
| [Logging.md](Foundation/Logging.md) | ✅ 완료 | 로깅 시스템 |
| [Error.md](Foundation/Error.md) | ✅ 완료 | 에러 처리 시스템 |

---

## 상태

| 분류 | 클래스 | 스펙 | 구현 |
|------|--------|------|------|
| 싱글턴 | Singleton<T> | ⬜ | ✅ |
| 이벤트 | EventManager | ⬜ | ✅ |
| 로깅 | LogLevel, LogCategory | ✅ | ✅ |
| 로깅 | Log, LogConfig | ✅ | ✅ |
| 로깅 | ILogOutput, UnityLogOutput | ✅ | ✅ |
| 에러 | ErrorCode, ErrorMessages | ✅ | ✅ |
| 에러 | Result<T> | ✅ | ✅ |
| 서비스 | Services | ✅ | ✅ |
| 저장소 | ISaveStorage, FileSaveStorage | ✅ | ✅ |

---

## 관련 문서

- [Core.md](Core.md) - SaveManager (세이브 시스템)
- [Common.md](Common.md) - LoadingIndicator (로딩 UI)
- [PROGRESS.md](../PROGRESS.md) - Phase 0 체크리스트
