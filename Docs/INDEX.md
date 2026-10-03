# Toward Limits — 작업공간 인덱스

Toward Limits는 함수 캐릭터와 미적분을 소재로 하는 Unity 게임이다. 기획 페이지에 2025 Fall HAJE GameJAM 출품작으로 소개되어 있다.

## 프로젝트 사양

- Unity Editor: **6000.0.58f2** (`ProjectSettings/ProjectVersion.txt`).
- 주요 패키지: URP 17.0.4, Input System 1.14.2, UniTask, uGUI 2.0.0.
- 활성 빌드 씬 순서: **Title → BaseCamp → Game → Clear**.
- 게임 코드: `Assets/1. Scripts/Runtime`, 빌드 코드: `Assets/1. Scripts/Editor`.

## 문서 탐색

| 필요한 정보 | 문서 |
|---|---|
| 폴더 구조, 설정, 빌드 진입점 | [프로젝트 개요](project/overview.md) |
| 기능별 코드 위치와 현재 구현 | [구현 지도](implementation/code-map.md) |
| 외부 JSON 데이터 형식 | [Override Data 가이드](OverrideData_Format.md) |
| Notion·Slack 위치, 용어와 담당 작업 | [협업 및 9/30 작업 목록](planning/work-context.md) |

문서 기준일: 2026-10-03. 각 문서에 명시한 코드·설정·외부 자료를 기준으로 관련 부분을 갱신한다.
