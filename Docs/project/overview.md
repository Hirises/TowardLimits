# 프로젝트 개요

[인덱스](../INDEX.md)

## 폴더 구성

| 경로 | 내용 |
|---|---|
| `Assets/0. Scenes` | Title, BaseCamp, Game, Clear 씬 |
| `Assets/1. Scripts/Runtime` | 게임 실행 코드 |
| `Assets/1. Scripts/Editor` | Unity Editor 빌드 코드 |
| `Assets/2. Sprites` | 스프라이트 |
| `Assets/3. Prefabs` | 프리팹 |
| `Assets/4. Materials`, `Assets/Materials` | 머티리얼 |
| `Assets/5. Data` | CutSceneData, EnemyData, StageData, UnitData, WaveData |
| `Assets/6. Fonts`, `Assets/7. Animations` | 폰트와 애니메이션 |
| `Assets/Resources` | ResourceHolder, 웨이브 JSON, DOTween 설정 |
| `Assets/Settings` | PC·Mobile URP Asset과 Renderer, Volume 설정 |
| `Assets/Plugins` | Demigiant, Sirenix 플러그인 |
| `Packages`, `ProjectSettings` | 패키지와 프로젝트 설정 |
| `Docs` | 작업공간 연속성 문서와 데이터 가이드 |

`Library`, `Temp`, `Logs`와 IDE 프로젝트 파일은 Unity의 생성 산출물이다. `.gitignore`에 관련 제외 규칙이 있다.

## 설정 및 빌드

- Editor 버전: `ProjectSettings/ProjectVersion.txt`.
- 패키지 버전: `Packages/manifest.json`. UniTask와 Cursor IDE 패키지는 Git URL로 등록되어 있다.
- 빌드 씬: `ProjectSettings/EditorBuildSettings.asset`에 Title, BaseCamp, Game, Clear가 활성화되어 있다.
- 입력 액션: `Assets/InputSystem_Actions.inputactions`가 빌드 설정에 연결되어 있다.
- 빌드 진입점: `Assets/1. Scripts/Editor/BuildScript.cs`의 `BuildScript.BuildCustom`.
- 빌드 인자: `-customBuildTarget`, `-customOutputPath`. 타깃 분기는 windows, android, ios, macos, webgl을 처리한다. 지원 플랫폼의 실제 빌드 가능 여부는 해당 Unity 모듈과 실행 환경에서 검증한다.
- 빌드 코드는 활성 씬 목록을 사용한다. Slack에서 Jenkins 빌드 봇과 Windows 빌드 결과 공유를 확인했다.

## 확인 범위

폴더 목록, 패키지·빌드 설정 및 주요 코드 일부를 읽어 확인했다. 씬 직렬화 내용, 프리팹 연결, 렌더링 결과와 플레이 동작은 Unity Editor에서 검증한다. Test Framework 패키지는 등록되어 있으며 게임 자체의 테스트 실행 절차는 추가 확인 대상이다.
