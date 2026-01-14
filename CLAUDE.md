# ProjectSC

서브컬쳐 수집형 RPG 포트폴리오 프로젝트

## 문서 참조
- [스펙 개요](Docs/Specs/SPEC_INDEX.md) - Assembly 목록 및 상태
- [진행상황](Docs/PROGRESS.md) - 현재 작업 상태
- [아키텍처](Docs/ARCHITECTURE.md) - 폴더 구조, 의존성
- [컨벤션](Docs/CONVENTIONS.md) - 코딩 규칙

## 핵심 규칙
- Assembly 접두사: `Sc.`
- 네임스페이스: `Sc.{폴더명}` (예: `Sc.Core`, `Sc.Contents.Character`)
- 인터페이스: `I` 접두사
- private 필드: `_` 접두사
- 파일당 클래스 1개
- 컨텐츠 간 통신: 이벤트 기반 (직접 참조 최소화)

## 기술 스택
Unity 2022.x / C# / UniTask / Addressables

## 커밋 규칙
- 커밋 메시지에 AI 사용 관련 문구 추가 금지 (Co-Authored-By 등)
