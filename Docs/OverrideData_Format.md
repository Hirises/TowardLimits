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

  "unit_c_ATKSPD_buff": 0.0,
  "unit_c_DMG_buff": 0,
  "unit_c_buffDuration": 3.0,
  "unit_x2_damageRatio": 0.5,
  "unit_x2_skillDamage": 40,
  "unit_x2_skillBulletSpeed": 15.0,
  "unit_x3_attackCount": 3,
  "unit_abs_healAmount": 10
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
| `unit_c_ATKSPD_buff` | `float` | `0.0` | 공격속도 버프 (UnitC 전용; 합연산) |
| `unit_c_DMG_buff` | `int` | `0` | 데미지 버프 (UnitC 전용; 합연산) |
| `unit_c_buffDuration` | `float` | `3.0` | 버프 지속시간 (UnitC 전용; 초) |
| `unit_x2_damageRatio` | `float` | `0.5` | 보조 탄막 피해 비율 (UnitX2 전용) |
| `unit_x2_skillDamage` | `int` | `40` | 스킬 탄환 피해량 (UnitX2 전용; 계수·일반 공격 버프와 독립) |
| `unit_x2_skillBulletSpeed` | `float` | `15.0` | 스킬 탄환 속도 (UnitX2 전용; 일반 탄속·탄속 버프와 독립) |
| `unit_x3_attackCount` | `int` | `3` | 관통 횟수 (UnitX3 전용) |
| `unit_abs_healAmount` | `int` | `10` | 아군 회복량 (UnitABS 전용) |

유닛 전용 필드는 `unit_c_`, `unit_x2_`, `unit_x3_`, `unit_abs_` 접두사를 사용합니다. 외부 JSON의 기존 전용 필드 키도 표의 새 이름으로 변경하여 사용합니다. 새 필드명은 이 정의가 포함된 빌드에서 지원됩니다.

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
  "damage": 10,
  "persuade": 5,
  "speed": 2.0,
  "color": { "r": 1.0, "g": 0.0, "b": 0.0, "a": 1.0 },

  "rangeAttack": false,
  "stopAtMiddle": false,
  "attackWhileMoving": false,
  "attackSpeed": 1.0,
  "rangeDamage": 10,
  "bulletSpeed": 2.0,
  "cancel_count": 1,

  "enemy_polarbear_snowballType": 5,

  "enemy_captainpolarbear_throwInterval": 1.0,
  "enemy_captainpolarbear_throwDamage": 50,
  "enemy_captainpolarbear_throwCount": 1,

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
| `damage` | `int` | 기본 충돌 피해량 |
| `persuade` | `int` | 설득 수치 |
| `speed` | `float` | 기본 이동 속도 |
| `color` | `Color` | RGBA 색상, 0.0~1.0 |
| `rangeAttack` | `bool` | 주기 공격 활성화 여부. 충돌 피해는 별도로 적용 |
| `stopAtMiddle` | `bool` | 적 콜라이더의 좌우 범위에 라인 중심이 포함된 모든 라인을 검사. 하나라도 유닛이 있으면 중앙선 도달 시 정지하고, 해당 라인이 전부 비면 전진 재개 |
| `attackWhileMoving` | `bool` | `true`이면 소환 즉시 첫 발사 후 이동·정지 및 라인 유닛 유무와 관계없이 반복 공격. `false`이면 정지 중에만 공격. `rangeAttack=true`일 때 적용 |
| `attackSpeed` | `float` | 초당 발사 횟수. 0 이하에서는 발사 대기. 정지 전용 공격은 정지 후 한 주기 뒤 첫 발사 |
| `rangeDamage` | `int` | 기본 원거리 투사체 피해량 |
| `bulletSpeed` | `float` | 일반 탄환 자체 속도. 발사 시 적 이동 속도를 가산(정지 시 0). 스테이지 증가 없음. 극곰 눈덩이 제외 |
| `cancel_count` | `int` | 일반 적 탄환이 상쇄되기까지 필요한 아군 탄환 명중 수(최소 1, 0 이하는 1로 적용). 명중한 아군 탄환은 매번 소멸하며, 아군 직접 명중 시 남은 횟수와 관계없이 원래 피해를 주고 소멸. 극곰 눈덩이는 기존 적 체력 판정 사용. 생략하면 해당 적 기본값 유지 |
| `enemy_polarbear_snowballType` | `int` | 일반 극곰 전용 소환 눈덩이: `5` = 기존 1라인 `Snowball`, `7` = 2라인 `Snowball2Line` |
| `enemy_captainpolarbear_throwInterval` | `float` | 대장 극곰 전용 던지기 주기(게임 시간 기준 초). 0 이하이면 던지기 대기. 공격 시작 후 한 주기 뒤 첫 던지기 |
| `enemy_captainpolarbear_throwDamage` | `int` | 던진 눈덩이 1개당 착지 피해. `rangeDamage`·`rangeDamage_add`와 독립된 고정값 |
| `enemy_captainpolarbear_throwCount` | `int` | 한 주기에 동시에 던지는 눈덩이 수. 전체 배치 칸에서 서로 다른 칸을 추첨하며, 전체 칸 수를 초과하면 전체 칸 수만큼 생성. 0 이하이면 생성 수 0 |
| `health_add` | `int` | 스테이지당 체력 증가량 |
| `damage_add` | `int` | 스테이지당 충돌 피해 증가량 |
| `rangeDamage_add` | `int` | 스테이지당 원거리 투사체 피해 증가량 |
| `speed_add` | `float` | 스테이지당 속도 증가량 |

> **실제 적용값**: `health + health_add × stage`, 충돌 피해 `damage + damage_add × stage`, 원거리 피해 `rangeDamage + rangeDamage_add × stage`, `speed + speed_add × stage`
>
> `enemyBehavior`는 Unity 에셋 참조이므로 JSON으로 오버라이드할 수 없습니다.

적 전용 필드는 `enemy_적이름_` 접두사를 사용하며 적 이름은 소문자로 작성합니다. 일반 극곰은 `enemy_polarbear_`, 대장 극곰은 `enemy_captainpolarbear_`를 사용합니다. `PolarBear.json`에서 `enemy_polarbear_snowballType`을 생략하면 기본 1라인 눈덩이를 소환합니다. 눈덩이 능력치는 `Snowball.json`과 `Snowball2Line.json`에서 각각 설정합니다. 새 필드는 해당 정의가 포함된 빌드에서 지원됩니다.

### 일반 극곰 눈덩이 설정 예시

`OverrideData/Enemy/PolarBear.json`

```json
{
  "enemy_polarbear_snowballType": 5
}
```

### 대장 극곰 공격 설정 예시

`OverrideData/Enemy/CaptainPolarBear.json`

```json
{
  "attackSpeed": 1.0,

  "enemy_captainpolarbear_throwInterval": 1.0,
  "enemy_captainpolarbear_throwDamage": 50,
  "enemy_captainpolarbear_throwCount": 1
}
```

대장 극곰은 `attackSpeed`에 따라 2라인 눈덩이를 굴리며 던지기는 독립 주기로 함께 실행합니다. 두 공격의 시작·중단 조건은 `rangeAttack`·`stopAtMiddle`·`attackWhileMoving`을 공유합니다. `attackSpeed`가 0 이하이어도 던지기는 설정된 주기를 따릅니다. 던지기 주기는 배속·감속·일시정지를 따르며, 이전 눈덩이가 낙하·착지 중이어도 다음 주기에 새 눈덩이를 생성합니다. 대장 극곰 사망·파괴 시 진행 중인 낙하 공격을 모두 취소합니다. 생략한 필드는 대장 극곰의 기본값을 유지합니다.

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
| 6 | `CaptainPolarBear` |
| 7 | `Snowball2Line` |

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
