# 구현 지도

[인덱스](../INDEX.md)

아래 코드 경로의 기준은 `Assets/1. Scripts/Runtime/`이다. 파일명은 현재 저장소 표기를 유지한다.

## 주요 진입점

| 기능 | 코드 | 확인한 역할 |
|---|---|---|
| 전체 게임 상태 | `GameManager.cs` | 씬 사이에 유지되는 싱글톤. 데이터 초기화, PlayerData 보관, 방향별 스테이지 선택, 게임 속도 설정 |
| 전투 | `CombatManager.cs` | 슬롯·적·탄환 참조와 Placement, Combat, Purchase 단계 처리 |
| 베이스캠프 | `Outgame/BaseCampManager.cs` | 인벤토리·유닛 강화 UI 설정, 진행 수치 표시, 북쪽·남쪽 선택 후 Game 씬 로딩 |
| 씬 및 컷씬 | `Outgame/TitleSceneManager.cs`, `LoadingScene.cs`, `CutsceneManager.cs`, `ClearSceneManager.cs` | 타이틀·로딩·컷씬·클리어 관련 탐색 위치 |
| 데이터 로딩 | `Loader/DataFetcher.cs` | 공통 설정, Stage·Wave·Unit·Enemy 모델 로딩과 캐시 |
| 리소스·플레이어 상태 | `Data/ResourceHolder.cs`, `Data/PlayerData.cs` | 리소스 및 플레이어 데이터 탐색 위치 |
| 유닛 | `Unit/` | UnitC, UnitX, UnitX2, UnitX3, UnitABS와 기본 행동·탄환 |
| 적 | `Enemy/` | EIR 색상별 적, PolarBear, SnowBall과 기본 행동·탄환 |
| UI | `UI/` | 배치, 인벤토리, 강화, 전투 표시, 지도, 디버그 화면 |
| 공통 엔티티·효과 | `Entity/`, `VFX/` | 피해 인터페이스, 생명체, 시각 효과 |

## 확인한 현재 구현

### 데이터

`GameManager.Awake()`에서 `DataFetcher.FetchData()`를 호출한다. `DataFetcher`는 ResourceHolder에 등록된 데이터를 사용하며 `OVERRIDE_DATA` 심볼이 활성화되면 `Application.persistentDataPath/OverrideData` 아래 JSON을 읽는다. Unit·Enemy 등의 오버라이드에 `JsonUtility.FromJsonOverwrite`를 사용한다. enum 이름을 작은따옴표로 감싼 값을 정수로 바꾸는 `DesugarEnumName` 전처리도 있다.

형식과 경로의 상세 참조는 [Override Data 가이드](../OverrideData_Format.md)이다. 필드 변경 시 `Loader/*Model.cs`, 데이터 에셋과 가이드의 일치 여부를 확인한다.

### 적 공격

`Enemy/DefaultEnemyBehavior.cs`는 적을 이동시키고, `data.rangeAttack`이 켜진 적이 중앙선에 도달하면 이동을 멈추고 공격 루프를 시작한다. 공격 대기 시간은 `1f / data.attackSpeed`이다. 유닛 충돌 피해에는 `data.GetDamage()`를 사용한다.

`Loader/EnemyModel.cs`에는 `rangeAttack`, `attackSpeed`, `damage`, `damage_add`가 있으며 `GetDamage()`는 스테이지 증가분을 반영한다. 9/30의 `attackDamage` 변경 제안은 [작업 목록](../planning/work-context.md)에 기록되어 있다. 실제 읽은 모델에는 `damage` 필드가 한 개 있으므로 변경 착수 시 투사체 코드와 외부 데이터 정의를 함께 대조한다.

### 이차함수

`Unit/UnitX2.cs`의 일반 공격은 자신의 열에 탄을 발사하고, 유효한 인접 열에도 탄을 발사한다. 현재 `SkillLoop`는 계수(`status.level`)만큼 `OnShoot()`를 반복 호출한다. 다른 모양의 강한 탄을 발사하는 기획은 별도의 예정 작업으로 기록되어 있다.

### dt 표시

`UI/CombatUI.cs`의 `UpdateDT()`는 `PlayerData.DT`가 9000을 초과하면 무한 표시를 활성화하고, 그 외에는 숫자를 표시한다. 잔여량에 따른 노란색·빨간색 표시는 [작업 목록](../planning/work-context.md)의 요청 사항이다.

확인 기준: 2026-10-03 로컬 코드 읽기. 씬에서의 실제 동작과 작업 완료 여부는 기능별 실행 검증으로 판단한다.
