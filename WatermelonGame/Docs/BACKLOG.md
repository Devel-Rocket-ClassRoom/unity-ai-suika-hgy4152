# 수박게임 개발 백로그

> GDD v1.1 기준 | 오늘 하루 완성 목표

---

## EPIC 1 — 프로젝트 셋업

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| S-01 | Unity 2D 프로젝트 설정 (카메라 Orthographic, 해상도 9:16) | 높음 | [ ] |
| S-02 | 씬 3개 생성: Title / Game / Result | 높음 | [ ] |
| S-03 | Physics 2D 글로벌 설정 (Gravity: -30, Layer 분리) | 높음 | [ ] |
| S-04 | 폴더 구조 생성: Scripts / Prefabs / Scenes / Audio / Materials | 보통 | [ ] |

---

## EPIC 2 — 게임 박스(Container)

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| B-01 | 좌벽 / 우벽 / 바닥 EdgeCollider2D 배치 | 높음 | [ ] |
| B-02 | 상자 시각화: SpriteRenderer 갈색 테두리 (#4E342E) | 보통 | [ ] |
| B-03 | Danger Line 오브젝트 생성 (빨간 가로선 + Trigger 판정) | 높음 | [ ] |
| B-04 | 상자 내벽 Physics Material: Bounciness 0.2, Friction 0.5 | 높음 | [ ] |

---

## EPIC 3 — 과일 프리팹

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| F-01 | 원형 Sprite 생성 (흰 원 텍스처 1장 공유) | 높음 | [ ] |
| F-02 | Fruit.cs 스크립트 작성 (FruitLevel, isMerging 필드) | 높음 | [ ] |
| F-03 | 과일 11종 프리팹 생성 (레벨별 색상·반지름 설정) | 높음 | [ ] |
| F-04 | CircleCollider2D radius를 GDD 반지름 값으로 설정 | 높음 | [ ] |
| F-05 | Rigidbody2D 설정: GravityScale 3.0 / LinearDrag 0.5 / AngularDrag 1.0 | 높음 | [ ] |
| F-06 | Collision Detection: Continuous 설정 (소형 과일 누락 방지) | 보통 | [ ] |

---

## EPIC 4 — 드롭 메커니즘

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| D-01 | FruitSpawner.cs: 마우스 X축 → 과일 X 위치 이동 (Clamp 적용) | 높음 | [ ] |
| D-02 | 클릭/스페이스 입력 → isKinematic=false 전환 (드롭) | 높음 | [ ] |
| D-03 | 드롭 후 0.5초 또는 velocity < 0.1 감지 → 다음 과일 스폰 | 높음 | [ ] |
| D-04 | Drop Guide Line: 현재 과일 X 위치 세로 점선 렌더링 | 보통 | [ ] |
| D-05 | 드롭 가능 과일 Lv.1~5 랜덤 선택 로직 | 높음 | [ ] |
| D-06 | Next 과일 큐 관리 (현재 + 다음 1개 미리 결정) | 높음 | [ ] |

---

## EPIC 5 — 합체 로직

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| M-01 | OnCollisionEnter2D: 같은 레벨 충돌 감지 | 높음 | [ ] |
| M-02 | Merge(): 두 과일 중간 지점 계산 → 상위 과일 Instantiate | 높음 | [ ] |
| M-03 | 합체된 두 과일 Destroy | 높음 | [ ] |
| M-04 | isMerging 플래그로 동일 프레임 이중 합체 방지 | 높음 | [ ] |
| M-05 | Lv.11(수박) + Lv.11 합체 → 두 오브젝트 소멸 처리 | 높음 | [ ] |
| M-06 | 연쇄 합체(Chain) 횟수 카운트 로직 | 보통 | [ ] |
| M-07 | 합체 시 Scale 팝업 애니메이션 (Coroutine, 0.1s) | 낮음 | [ ] |

---

## EPIC 6 — 점수 시스템

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| SC-01 | ScoreManager.cs: 합체 시 N×(N+1)/2 점수 누적 | 높음 | [ ] |
| SC-02 | 연쇄 2회 × 1.5, 3회+ × 2.0 보너스 적용 | 보통 | [ ] |
| SC-03 | 수박 최초 완성 +200 보너스 (1회성 플래그) | 보통 | [ ] |
| SC-04 | PlayerPrefs에 Best Score 저장/불러오기 | 높음 | [ ] |

---

## EPIC 7 — 게임 오버

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| GO-01 | Danger Line Trigger: OnTriggerStay2D로 초과 과일 감지 | 높음 | [ ] |
| GO-02 | 3초 카운트다운 코루틴 → 게임 오버 발동 | 높음 | [ ] |
| GO-03 | 합체 중(isMerging=true)인 과일은 판정 제외 | 보통 | [ ] |
| GO-04 | GameOver(): Time.timeScale = 0 → Result 씬 전환 | 높음 | [ ] |

---

## EPIC 8 — UI

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| UI-01 | 상단 HUD: Current Score / Best Score TextMeshPro | 높음 | [ ] |
| UI-02 | Next Panel: 다음 과일 원 미리보기 | 높음 | [ ] |
| UI-03 | Pause 버튼: Time.timeScale 토글 | 보통 | [ ] |
| UI-04 | Danger Line 빨간 경고 이펙트 (초과 시 점멸) | 낮음 | [ ] |
| UI-05 | Title 씬: 제목 텍스트 + PLAY 버튼 | 높음 | [ ] |
| UI-06 | Result 씬: 최종 점수 / Best 점수 / 재시작 / 타이틀 버튼 | 높음 | [ ] |
| UI-07 | 게임 오버 패널 페이드 인 (CanvasGroup alpha) | 보통 | [ ] |
| UI-08 | 카메라 Shake 코루틴 (연쇄 합체 시) | 낮음 | [ ] |

---

## EPIC 9 — 씬 관리 / 마무리

| ID | 태스크 | 우선순위 | 완료 |
|----|--------|----------|------|
| SM-01 | GameManager.cs: GameState Enum (Title/Playing/Paused/GameOver) | 높음 | [ ] |
| SM-02 | SceneManager로 씬 전환 (Title↔Game↔Result) | 높음 | [ ] |
| SM-03 | 재시작 시 모든 과일 오브젝트 제거 + 점수 리셋 | 높음 | [ ] |
| SM-04 | 배경 크림색(#FFF8E1) 설정 | 낮음 | [ ] |

---

## 우선순위 요약 (오늘 완성 순서)

```
[1] S-01~04  → 프로젝트 셋업
[2] B-01~04  → 상자 배치
[3] F-01~06  → 과일 프리팹 11종
[4] D-01~06  → 드롭 입력
[5] M-01~05  → 합체 핵심 로직
[6] GO-01~04 → 게임 오버
[7] SC-01~04 → 점수
[8] UI-01~06 → 기본 UI
[9] 낮음 우선순위 항목은 시간 남으면 처리
```

---

> 낮음 우선순위 항목(M-07, UI-04, UI-07, UI-08): 핵심 플레이에 영향 없음. 폴리싱 단계에서 처리.
