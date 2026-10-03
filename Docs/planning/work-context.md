# 협업 및 작업 맥락

[인덱스](../INDEX.md)

## 자료 위치와 용어

- [Toward Limits 기획](https://app.notion.com/p/04a1a56a23cd8291bc6c81f6ee89e212): 캐릭터 기획, 시스템 기획, 대사/지문, 구현용 메모, 회의록과 브레인스토밍의 상위 페이지.
- [Notion TODO List](https://app.notion.com/p/3ee1a56a23cd808aa130fc1f72aae22a): 9/30 회의 기반 조의준 작업 8개를 체크박스로 등록한 페이지.
- [Slack #project_towardlimits](https://haje.slack.com/archives/C09UA5374KH): HAJE 워크스페이스의 프로젝트 채널.
- 사용자 본인의 Slack 이름은 **hirises**, 이름은 **조의준**이다. 사용자 확인에 근거한다.
- **dt**는 회의의 잔여량 표시 대상이며 코드에서는 `PlayerData.DT`, `CombatUI.UpdateDT()`로 탐색한다.
- **대장 극곰**은 회의에서 일반 극곰과 구별되는 공격 패턴이 제안된 적을 뜻한다. 실제 구현 타입은 작업 착수 시 확인한다.

## 2026-09-30 회의 작업

회의는 한국 시간 약 20:00~20:51에 진행됐다. 아래는 당시 요청 및 진행 의사이며 현재 완료 상태는 미확인이다. Notion TODO List에는 2026-10-03에 전부 미체크로 등록했다.

| 작업 | 합의·조건 | 근거 |
|---|---|---|
| 베이스캠프 변경사항 반영 | 다음 주까지 진행 약속. 완성 아트 수령 후 적용 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790766313266309) |
| 이동 중 적 공격 지원 | 노란 적이 이동하면서 공격하도록 설정 추가·수정 약속 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790768287133379) |
| 프로그래머 TODO 정리 | 구두 논의와 누적 작업을 별도로 문서화하겠다는 약속 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790767687653779) |
| 대장 극곰 공격 패턴 | 개발 의사 확인. X자 공격·포효 전체 공격은 후보. 패턴·기한 미확정 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790766993706699) |
| 코드 리팩터링 | 여유 있을 때 본인 우선순위에 따라 진행 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790768637163829) |
| dt 잔여량 색상 | 1개 노란색, 0개 빨간색. 이전 미완료 요청 재확인, 기한 미확정 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790767331861529) |
| 이차함수 고유 스킬 | 다른 모양의 강한 탄 발사. 기존 탄막 아트 재확인 필요, 기한 미확정 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790767380653659) |
| Enemy Data 필드명 | 회의 직후 투사체 데미지를 `attackDamage`로 바꾸자는 제안에 동의 | [Slack](https://haje.slack.com/archives/C09UA5374KH/p1790770042179319) |

싱크함수 윤곽 수정은 짱존, 도감 제작은 개다래, 보스 중심 밸런싱은 e.s. 담당으로 정리됐다. 조의준은 완성된 베이스캠프 아트의 게임 반영을 맡는다.

## 후속 작업의 참조 순서

구현 작업은 [구현 지도](../implementation/code-map.md)의 관련 위치와 최신 코드·에셋을 확인하고, 기획 세부 내용은 해당 Notion 하위 문서와 연결된 Slack 대화를 읽는다. 완료 여부는 실제 검증 결과에 따라 갱신한다. 기획 상위 페이지와 9/30 대화는 읽기 확인됐으며 상세 하위 기획의 내용은 작업별로 확인한다.
