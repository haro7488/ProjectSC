# ProjectSC

서브컬쳐 수집형 RPG 포트폴리오 | Unity 2022.x / C# / UniTask / Addressables

## 핵심 규칙

| 항목 | 규칙 |
|------|------|
| Assembly | `Sc.` 접두사 |
| 네임스페이스 | `Sc.{폴더명}` |
| 명명 | 인터페이스 `I`, private `_` |
| 구조 | 파일당 클래스 1개, 이벤트 기반 통신 |

## 서브에이전트 위임

> 메인 = 검토/의사결정, 서브 = 탐색/문서화/구현

| 작업 | 위임 | 모델 |
|------|------|------|
| 3+ 파일 탐색 | `Explore` | sonnet |
| 아키텍처 분석 | `code-explorer` | sonnet |
| 설계/문서 | `code-architect` | **opus** |
| 기능 구현 | `general-purpose` | sonnet |
| 코드 리뷰 | `code-reviewer` | **opus** |

**메인 직접**: 단일 파일, Serena 심볼 조회, 최종 승인

**결과 포맷**: `## 완료 보고` → 수행/결과/확인필요

**컨텍스트**: 완료 보고만 보존, 상세는 파일 경로 참조

## Skills (온디맨드)

| 스킬 | 용도 |
|------|------|
| `/explore` | 탐색 프로토콜 |
| `/implement` | 구현 프로토콜 |
| `/document` | 문서화 프로토콜 |

## Commands

| 커맨드 | 용도 |
|--------|------|
| `/analyze {대상}` | 분석 → 설계안 |
| `/impl {기능}` | 분석 → 구현 → 검증 |
| `/review {대상}` | 코드 리뷰 |

## 컨텍스트 관리

| 명령 | 시점 |
|------|------|
| `/clear` | 작업 전환 |
| `/compact` | 70% 용량 또는 20회 반복 |

## 작업 절차

1. `Docs/PROGRESS.md` 확인
2. 관련 스펙 확인 (`Docs/Specs/{Assembly}.md`)
3. TodoWrite 진행 추적
4. 완료 시 Progress 업데이트

## 커밋

```
Add feature       ← 영어 (동사 원형)
- 설명 한국어     ← 본문
```

## 참조

| 문서 | 용도 |
|------|------|
| `.claude/USAGE_GUIDE.md` | **사용법 가이드** |
| `Docs/PROGRESS.md` | 작업 상태 |
| `Docs/ARCHITECTURE.md` | 폴더/의존성 |
| `.claude/MULTI_SESSION.md` | 멀티세션 |
