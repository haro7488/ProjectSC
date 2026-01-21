# Systems Decisions

공통 시스템 관련 의사결정 기록.

---

## 보상 시스템 (RewardType) 설계

**일자**: 2026-01-17 | **상태**: 결정됨

### 컨텍스트
아웃게임 아키텍처에서 공통 보상 시스템 필요. 수집형 RPG 다양한 보상 타입 처리.

### 선택지
1. **세분화된 RewardType (8개+)** - 타입만으로 분기 가능하나 중복 로직
2. **Thing 기반 통합 (4개)** - 단순하나 세부 구분에 추가 조회 필요
3. **처리 방식 기준 분류** - RewardType 4개 + ItemCategory 6개 (선택)

### 결정
**처리 방식 기준 분류**
```csharp
public enum RewardType { Currency, Item, Character, PlayerExp }
public enum ItemCategory { Equipment, Consumable, Material, CharacterShard, Furniture, Ticket }
```

### 결과
- 보상 처리 로직이 동일한 것을 같은 RewardType으로 분류
- UI 표시용 세분화는 ItemCategory로
- 61개 테스트 작성

### 추가 결정
- **장비**: 인벤토리=수량 기반, 장착=인스턴스 기반
- **스킨**: 별도 캐릭터로 처리 (CharacterData.BaseCharacterId)

---

## TimeService 시간 처리 설계

**일자**: 2026-01-17 | **상태**: 결정됨

### 컨텍스트
상점 구매 제한 (Daily/Weekly/Monthly) 리셋 시간 처리 필요.

### 결정
- **구현**: TimeService 중앙 집중
- **UI 타임존**: 로컬 시간 변환
- **위치**: ITimeService 인터페이스 공유, 클라/서버 각각 구현
- **리셋 기준**: UTC 0시

### 결과
테스트 시 MockTimeService로 시간 조작 가능. 45개 테스트 작성.

---

## Foundation 시스템 범위

**일자**: 2026-01-17 | **상태**: 결정됨

### 컨텍스트
OUTGAME-V1 구현 전 기반 시스템 필요성 논의.

### 결정 (Phase 0에 포함)
1. **Logging** - 디버깅, 릴리즈 빌드 최적화
2. **Error** - ErrorCode 체계, Result<T> 패턴
3. **SaveManager** - 버전 마이그레이션
4. **LoadingIndicator** - 네트워크 중 화면 차단

### 이후로 미룸
오브젝트 풀링, 리소스 로딩, 오디오, 로컬라이제이션, 설정

### 결과
1~4번은 다른 모든 시스템에서 사용하는 기반 인프라.

---

## RewardPopup 좌우 스크롤 카드형

**일자**: 2026-01-17 | **상태**: 결정됨

### 컨텍스트
보상 획득 팝업 UI 방식. 수집형 RPG 5개 게임 비교 분석.

### 선택지
1. **리스트형** - 심플하나 특별한 보상도 평범하게 보임
2. **그리드형** - 카드 컬렉션 느낌, 많은 보상 시 스크롤 필요
3. **좌우 스크롤 카드형** - 각 보상 강조, 모바일 UX 자연스러움 (선택)

### 결정
**좌우 스크롤 카드형 (캐러셀)**
- 정렬: 희귀도 순 (캐릭터 > 장비 > 재료 > 재화)
- 전체 보기: 그리드 형태 서브 팝업 (4열)
- 신규 아이템: NEW 뱃지 + 금색 프레임

---

## RewardPopup 아이콘 로딩 전략

**일자**: 2026-01-19 | **상태**: 결정됨

### 컨텍스트
Addressables 아이콘 로드 시 UI "튀는" 현상 방지 필요.

### 선택지
1. **동기 로드 (Resources)** - 즉시 표시하나 빌드 크기 증가
2. **지연 로드** - 빌드 최적화되나 튀는 현상
3. **프리로드 캐시** - 둘 다 해결 (선택)

### 결정
**프리로드 캐시**
```
OnBind → PreloadAsync → SpawnItems (캐시에서 즉시 표시)
```

### 결과
AssetManager Scope 기반 로딩으로 대체. RewardIconCache 127줄 삭제.

---

## CharacterEnhancement 전투력 계산 공식

**일자**: 2026-01-21 | **상태**: 결정됨

### 컨텍스트
캐릭터 레벨업/돌파 시 전투력(Combat Power) 변화 표시 필요. 공식 투명성 vs 복잡성 트레이드오프.

### 선택지
1. **단순 합산** - HP + ATK + DEF + ... (직관적이나 밸런스 어려움)
2. **가중치 기반** - 스탯별 계수 적용 (선택)
3. **비선형 공식** - 복잡한 계산 (정확하나 이해 어려움)

### 결정
**가중치 기반 선형 공식**
```csharp
public static class PowerCalculator
{
    public static int Calculate(CharacterStats stats)
    {
        return (int)(
            stats.HP / 10 +
            stats.ATK * 5 +
            stats.DEF * 3 +
            stats.SPD * 2 +
            stats.CritRate * 100 +
            stats.CritDamage * 50
        );
    }
}
```

### 결과
- 중앙 집중 계산으로 일관성 보장
- UI에서 레벨업 전/후 비교 미리보기 가능
- 돌파 시 스탯 보너스도 동일 공식 적용

---

## CharacterEnhancement 재료 시스템

**일자**: 2026-01-21 | **상태**: 결정됨

### 컨텍스트
레벨업 재료 선택 UX. 여러 종류 경험치 아이템 존재.

### 선택지
1. **자동 선택 only** - 단순하나 유저 제어 부족
2. **수동 선택 only** - 유연하나 번거로움
3. **하이브리드** - 자동 선택 + 수동 조정 가능 (선택)

### 결정
**하이브리드 방식**
```csharp
// LevelUpPopup
public void AutoSelectMaterials()
{
    // 낮은 등급부터 채워서 고급 재료 절약
    var sorted = materials.OrderBy(m => m.ExpValue);
    SelectUntilTargetLevel(sorted);
}
```

### 결과
- 자동 선택: 낮은 등급 우선 사용
- +/- 버튼으로 수동 조정
- 목표 레벨 도달 시 자동 제한

---

## GachaEnhancement 소프트 천장 설계

**일자**: 2026-01-21 | **상태**: 결정됨

### 컨텍스트
하드 천장(100연차 확정)만으로는 유저 경험 부족. 점진적 확률 증가 필요.

### 선택지
1. **하드 천장만** - 단순하나 99연차까지 희망 없음
2. **소프트 천장** - 특정 횟수 이후 점진적 확률 증가 (선택)
3. **복합 시스템** - 소프트 + 하드 + 운명 시스템 (복잡)

### 결정
**소프트 천장 시스템**
```csharp
// GachaPoolData 확장
public int PitySoftStart;      // 소프트 천장 시작 (예: 70)
public float PitySoftRateBonus; // 회차당 추가 확률 (예: 0.5%)

// GachaService
public float GetEffectiveSSRRate(int pityCount, GachaPoolData pool)
{
    if (pityCount < pool.PitySoftStart)
        return pool.SSRRate;
    
    var bonusCount = pityCount - pool.PitySoftStart;
    return pool.SSRRate + (bonusCount * pool.PitySoftRateBonus);
}
```

### 결과
- 70연차부터 매 회차 +0.5% (기본 3% → 70연차 3%, 80연차 8%, 90연차 13%...)
- 서버에서 계산, 클라이언트는 표시만
- 테스트 12개로 경계 조건 검증

---

## GachaEnhancement 히스토리 시스템

**일자**: 2026-01-21 | **상태**: 결정됨

### 컨텍스트
뽑기 히스토리 저장 범위와 형식.

### 선택지
1. **최근 N건만** - 저장 용량 제한, 오래된 기록 손실
2. **풀별 최근 N건** - 풀마다 별도 관리 (선택)
3. **전체 저장** - 용량 무제한 증가

### 결정
**풀별 최근 100건**
```csharp
public struct GachaHistoryRecord
{
    public string PoolId;
    public long Timestamp;
    public List<GachaResultItem> Results;
}

// UserSaveData
public List<GachaHistoryRecord> GachaHistory; // 풀별 최근 100건
```

### 결과
- v8 마이그레이션으로 기존 유저 데이터 호환
- 히스토리 화면에서 풀 필터링 가능
- 테스트 16개

---

## NavigationEnhancement 배지 시스템

**일자**: 2026-01-21 | **상태**: 결정됨

### 컨텍스트
탭/버튼에 알림 배지 표시. 여러 시스템에서 배지 제공 필요.

### 선택지
1. **하드코딩** - 각 화면에서 직접 계산
2. **중앙 Manager + Provider** - 확장 가능 (선택)
3. **이벤트 기반** - 변경 시마다 브로드캐스트

### 결정
**BadgeManager + IBadgeProvider 패턴**
```csharp
public interface IBadgeProvider
{
    BadgeType BadgeType { get; }
    int GetBadgeCount();
    event Action OnBadgeCountChanged;
}

public class BadgeManager : Singleton<BadgeManager>
{
    private readonly Dictionary<BadgeType, IBadgeProvider> _providers;
    
    public int GetBadgeCount(BadgeType type) 
        => _providers.TryGetValue(type, out var p) ? p.GetBadgeCount() : 0;
}
```

### 결과
- 3개 Provider 구현: EventBadgeProvider, ShopBadgeProvider, GachaBadgeProvider
- LobbyScreen 탭에 배지 연동
- 새 배지 타입 추가 시 Provider만 구현
