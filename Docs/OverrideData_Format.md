# Override Data JSON 형식 가이드

게임 데이터를 외부 JSON 파일로 오버라이드하는 방법을 설명합니다.
`OVERRIDE_DATA` 심볼이 정의되어 있을 때만 동작하며, 모든 파일은 `Application.persistentDataPath` 하위에 위치해야 합니다.

인게임에서 `F1`키를 눌러 현재 적용중인 Override Data를 확인할 수 있습니다.

> **참고**: Unity `JsonUtility` 기반이므로 enum은 **정수(int)** 값으로 작성합니다.

---

## 목차

1. [Common Settings](#1-common-settings)
2. [Unit Data](#2-unit-data)
3. [Enemy Data](#3-enemy-data)
4. [Wave Data](#4-wave-data)
5. [Stage Data](#5-stage-data)
6. [Enum 참조표](#enum-참조표)
7. [주의사항](#주의사항)

---

## 1. Common Settings

| 항목 | 내용 |
|------|------|
| **경로** | `OverrideData/common_settings.json` |
| **방식** | `FromJsonOverwrite` — 기존 값에 **병합** (명시한 필드만 덮어씀) |

### 형식

```json
{
  "initalUnitlist": [0, 1, 2]
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| `initalUnitlist` | `int[]` | 초기 보유 유닛 목록. [UnitType](#unittype) 참조 |

---

## 2. Unit Data

| 항목 | 내용 |
|------|------|
| **경로** | `OverrideData/Unit/{UnitType 이름}.json` |
| **파일명 예시** | `UnitC.json`, `UnitX.json`, `UnitX2.json` 등 |
| **방식** | `FromJsonOverwrite` — 기존 값에 **병합** (명시한 필드만 덮어씀) |

### 형식

```json
{
  "unitType": 1,
  "unitName": "상수함수",
  "unitDescription": "유닛 설명 텍스트",
  "unitColor": { "r": 1.0, "g": 0.5, "b": 0.0, "a": 1.0 },

  "maxHealth": 100,
  "attack": 10,
  "attackSpeed": 1.0,
  "attackRange": 100,
  "bulletSpeed": 15.0,

  "derivativeTo": [
    { "unitType": 0, "probability": 1.0, "amount": 1 }
  ],
  "integralTo": [
    { "unitType": 2, "probability": 0.5, "amount": 1 }
  ],

  "ATKSPD_buff": 0.0,
  "DMG_buff": 0,
  "damageRatio": 0.5,
  "attackCount": 3
}
```

| 필드 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| `unitType` | `int` | — | [UnitType](#unittype) |
| `unitName` | `string` | — | 유닛 이름 |
| `unitDescription` | `string` | — | 유닛 설명 |
| `unitColor` | `Color` | `(1,1,1,1)` | 유닛 고유의 RGBA 색상, 0.0~1.0 |
| `maxHealth` | `int` | `100` | 최대 체력 |
| `attack` | `int` | `10` | 공격력 |
| `attackSpeed` | `float` | `1.0` | 공격 속도 |
| `attackRange` | `int` | `100` | 공격 사거리 |
| `bulletSpeed` | `float` | `15.0` | 투사체 속도 |
| `derivativeTo` | `CalculusResultElement[]` | — | 미분 시 결과 유닛 목록 |
| `integralTo` | `CalculusResultElement[]` | — | 적분 시 결과 유닛 목록 |
| `ATKSPD_buff` | `float` | `0.0` | 공격속도 버프 (UnitC 전용; 합연산) |
| `DMG_buff` | `int` | `0` | 데미지 버프 (UnitC 전용; 합연산) |
| `damageRatio` | `float` | `0.5` | 보조 탄막 피해 비율 (UnitX2 전용) |
| `attackCount` | `int` | `3` | 관통 횟수 (UnitX3 전용) |

#### CalculusResultElement

```json
{ "unitType": 2, "probability": 0.5, "amount": 1 }
```

| 필드 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| `unitType` | `int` | — | 결과 [UnitType](#unittype) |
| `probability` | `float` | `1.0` | 확률 (0.0~1.0) |
| `amount` | `int` | `1` | 생성 수량 |

> **Tip**: `FromJsonOverwrite`이므로 변경하고 싶은 필드만 작성하면 나머지는 원본 값이 유지됩니다.
> `fullFront`, `chibiIcon`, `unitBehavior`는 Unity 에셋 참조이므로 JSON으로 오버라이드할 수 없습니다.

---

## 3. Enemy Data

| 항목 | 내용 |
|------|------|
| **경로** | `OverrideData/Enemy/{EnemyType 이름}.json` |
| **파일명 예시** | `EIRNormal.json`, `PolarBear.json` 등 |
| **방식** | `FromJsonOverwrite` — 기존 값에 **병합** (명시한 필드만 덮어씀) |

### 형식

```json
{
  "enemyType": 0,
  "health": 50,
  "zeroDEF": 0,
  "infDEF": 0,
  "rangeAttack": false,
  "stopAtMiddle": false,
  "attackWhileMoving": false,
  "attackSpeed": 1.0,
  "damage": 10,
  "rangeDamage": 10,
  "persuade": 5,
  "speed": 2.0,
  "color": { "r": 1.0, "g": 0.0, "b": 0.0, "a": 1.0 },

  "health_add": 5,
  "damage_add": 2,
  "rangeDamage_add": 2,
  "speed_add": 0.1
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| `enemyType` | `int` | [EnemyType](#enemytype) |
| `health` | `int` | 기본 체력 |
| `zeroDEF` | `int` | Zero 타입 공격에 대한 방어력 |
| `infDEF` | `int` | Infinite 타입 공격에 대한 방어력 |
| `rangeAttack` | `bool` | 주기 공격 활성화 여부. 충돌 피해는 별도로 적용 |
| `stopAtMiddle` | `bool` | 같은 라인에 유닛이 있으면 중앙선 도달 시 정지. 라인이 비면 전진 재개 |
| `attackWhileMoving` | `bool` | `true`이면 소환 즉시 첫 발사 후 이동·정지 및 라인 유닛 유무와 관계없이 반복 공격. `false`이면 정지 중에만 공격. `rangeAttack=true`일 때 적용 |
| `attackSpeed` | `float` | 초당 발사 횟수. 0 이하에서는 발사 대기. 정지 전용 공격은 정지 후 한 주기 뒤 첫 발사 |
| `damage` | `int` | 기본 충돌 피해량 |
| `rangeDamage` | `int` | 기본 원거리 투사체 피해량 |
| `persuade` | `int` | 설득 수치 |
| `speed` | `float` | 기본 이동 속도 |
| `color` | `Color` | RGBA 색상, 0.0~1.0 |
| `health_add` | `int` | 스테이지당 체력 증가량 |
| `damage_add` | `int` | 스테이지당 충돌 피해 증가량 |
| `rangeDamage_add` | `int` | 스테이지당 원거리 투사체 피해 증가량 |
| `speed_add` | `float` | 스테이지당 속도 증가량 |

> **실제 적용값**: `health + health_add × stage`, 충돌 피해 `damage + damage_add × stage`, 원거리 피해 `rangeDamage + rangeDamage_add × stage`, `speed + speed_add × stage`
>
> `enemyBehavior`는 Unity 에셋 참조이므로 JSON으로 오버라이드할 수 없습니다.

`damage`와 `damage_add`는 충돌 공격에 적용하고, `rangeDamage`와 `rangeDamage_add`는 원거리 공격에 적용합니다. 기존 JSON에서 원거리 공격력을 설정하던 경우 새 원거리 필드에 값을 지정합니다. 생략한 필드는 해당 적의 기본 에셋 값을 유지하며, 충돌 필드에서 원거리 필드로 자동 복사되지 않습니다. 기본 에셋 6종의 원거리 값은 분리 전 공격력과 증가량으로 초기화되어 있습니다.

---

## 4. Wave Data

Wave 데이터는 두 가지 오버라이드 경로를 가집니다. **우선순위**에 따라 동작이 다릅니다.

| 모드 | 경로 | 동작 |
|------|------|------|
| **전체 대체** | `OverrideData/Wave/*.json` | 원본 데이터를 **모두 무시**하고 JSON 파일들로만 구성 |
| **추가** | `OverrideData/WaveAdditional/*.json` | 원본 데이터에 JSON 파일들을 **추가** |

> **동작 우선순위**: `Wave` 디렉토리가 존재하면 전체 대체가 적용되고, `WaveAdditional`은 **무시**됩니다.
> `Wave` 디렉토리가 없을 때만 `WaveAdditional`이 동작합니다.

### 형식 (Wave / WaveAdditional 공통)

파일명은 자유입니다 (예: `wave_01.json`).

```json
{
  "polar": 0,
  "stageRange": { "x": 1, "y": 5 },
  "difficulty": { "x": 1, "y": 3 },
  "forFinalBoss": false,
  "enemyList": [
    {
      "lane": 0,
      "emitOnce": true,
      "startTime": 0.0,
      "count": 1,
      "interval": 0.0,
      "enemyType": 0
    },
    {
      "lane": 1,
      "emitOnce": false,
      "startTime": 2.0,
      "count": 5,
      "interval": 1.5,
      "enemyType": 1
    }
  ]
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| `polar` | `int` | [Polar](#polar) — 웨이브 방향 |
| `stageRange` | `Vector2Int` | `{"x": 최소, "y": 최대}` — 등장 스테이지 범위 |
| `difficulty` | `Vector2Int` | `{"x": 최소, "y": 최대}` — 난이도 계수 |
| `forFinalBoss` | `bool` | 마지막 보스 전용 웨이브 여부 |
| `enemyList` | `WaveChartData[]` | 적 소환 정보 배열 |

#### WaveChartData

| 필드 | 타입 | 설명 |
|------|------|------|
| `lane` | `int` | 소환 레인 |
| `emitOnce` | `bool` | `true` = 1회 소환, `false` = 반복 소환 |
| `startTime` | `float` | 소환 시작 시각 (초) |
| `count` | `int` | 반복 소환 시 총 수량 (`emitOnce=false`일 때) |
| `interval` | `float` | 반복 소환 간격 (초, `emitOnce=false`일 때) |
| `enemyType` | `int` | [EnemyType](#enemytype) |

---

## 5. Stage Data

Stage 데이터는 두 가지 오버라이드 경로를 가집니다. **우선순위**에 따라 동작이 다릅니다.

| 모드 | 경로 | 동작 |
|------|------|------|
| **전체 대체** | `OverrideData/Stage/*.json` | 원본 데이터를 **모두 무시**하고 JSON 파일들로만 구성 |
| **추가** | `OverrideData/StageAdditional/*.json` | 원본 데이터에 JSON 파일들을 **추가** |

> **동작 우선순위**: `Stage` 디렉토리가 존재하면 전체 대체가 적용되고, `StageAdditional`은 **무시**됩니다.
> `Stage` 디렉토리가 없을 때만 `StageAdditional`이 동작합니다.

### 형식 (Stage / StageAdditional 공통)

파일명은 자유입니다 (예: `stage_1-1.json`).

```json
{
  "stageNumber": { "x": 1, "y": 1 },
  "direction": 1,
  "waveCount": 3,
  "DT": 100,
  "prove": 10
}
```

| 필드 | 타입 | 설명 |
|------|------|------|
| `stageNumber` | `Vector2Int` | `{"x": 챕터, "y": 스테이지}` — 스테이지 번호 |
| `direction` | `int` | [Polar](#polar) — 스테이지 방향 |
| `waveCount` | `int` | 웨이브 수 |
| `DT` | `int` | 보상 DT |
| `prove` | `int` | 보상 Prove |

---

## Enum 참조표

### UnitType

| 값 | 이름 |
|----|------|
| 0 | `None` |
| 1 | `UnitC` |
| 2 | `UnitX` |
| 3 | `UnitX2` |
| 4 | `UnitX3` |
| 5 | `UnitABS` |

### EnemyType

| 값 | 이름 |
|----|------|
| 0 | `EIRNormal` |
| 1 | `EIRBlue` |
| 2 | `EIRYellow` |
| 3 | `EIRPurple` |
| 4 | `PolarBear` |
| 5 | `Snowball` |

### Polar

| 값 | 이름 | 의미 |
|----|------|------|
| 0 | `Both` | 양쪽 |
| 1 | `North` | 적분 |
| 2 | `South` | 미분 |

---

## 주의사항

- **`FromJsonOverwrite` (Unit, Enemy, Common Settings)**
  - 기존 ScriptableObject/모델에 병합되므로, **변경할 필드만** 작성하면 됩니다.
  - 파일명이 enum 이름과 정확히 일치해야 합니다 (예: `UnitX2.json`, `EIRNormal.json`).

- **`FromJson` — 전체 대체 (Wave, Stage)**
  - 디렉토리가 존재하면 원본 데이터를 **전부 대체**합니다.
  - 모든 필드를 빠짐없이 작성해야 합니다.

- **`FromJson` — 추가 (WaveAdditional, StageAdditional)**
  - 원본 데이터는 유지하면서 JSON 파일들을 **추가**합니다.
  - 전체 대체(`Wave`/`Stage`) 디렉토리가 존재하면 Additional은 무시됩니다.
  - JSON 형식은 전체 대체와 동일하며, 모든 필드를 빠짐없이 작성해야 합니다.

- **Unity 에셋 참조 필드**는 JSON으로 오버라이드할 수 없습니다:
  - `UnitModel`: `fullFront`, `chibiIcon`, `unitBehavior`
  - `EnemyModel`: `enemyBehavior`

- **Color 형식**: `{ "r": 0.0, "g": 0.0, "b": 0.0, "a": 1.0 }` (각 값 0.0~1.0)

- **Vector2Int 형식**: `{ "x": 0, "y": 0 }`
