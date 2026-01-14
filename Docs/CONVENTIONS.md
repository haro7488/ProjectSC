# 코딩 컨벤션

## 네이밍

| 대상 | 규칙 | 예시 |
|------|------|------|
| 클래스 | PascalCase | `GameManager` |
| 인터페이스 | I + PascalCase | `IBattleState` |
| private 필드 | _camelCase | `_instance` |
| public 프로퍼티 | PascalCase | `Instance` |
| 메서드 | PascalCase | `Initialize()` |
| 상수 | UPPER_SNAKE | `MAX_LEVEL` |
| 이벤트 | On + PascalCase | `OnDamaged` |

## 네임스페이스

```csharp
namespace ProjectSC.Core { }
namespace ProjectSC.Character { }
namespace ProjectSC.Battle { }
// 폴더 구조 반영
```

## 파일 구조 순서

1. using
2. namespace
3. 상수 → static → private 필드 → 프로퍼티
4. Unity 메서드 (Awake, Start, Update)
5. public 메서드 → private 메서드

## 기타

- 파일당 클래스 1개
- `var`: 타입 명확할 때만
- 중괄호: Allman 스타일
- XML 주석: public API만
