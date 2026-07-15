# CLAUDE.md

이 파일은 Claude Code가 이 저장소에서 작업할 때 따라야 할 작업 규칙입니다.
프로젝트 기획/디자인 내용은 `README.md`를 참고하세요.
코드 구조, 폴더 배치 등은 `/init`으로 스캔한 내용을 우선하고, 이 파일과 충돌하면 이 파일의 규칙을 따릅니다.

## 프로젝트 기본 정보

- 엔진: Unity 6.4
- 장르: 턴제 JRPG 커맨드 배틀 + 로그라이크 무한 타워
- UI: 화면 UI는 UI Toolkit, 월드스페이스 UI(체력바 등)는 uGUI

## 작업 원칙

1. **최적화와 코드 단순성의 균형**
   - 최적화를 신경 쓰되, 그것 때문에 스크립트 개수나 코드 줄 수를 과하게 늘리는 방식은 지양
   - 가독성과 유지보수성을 우선하고, 성능이 실제로 문제가 되는 지점부터 최적화

2. **디자인 패턴 제안**
   - 기획/설계 단계에서 적용 가능한 디자인 패턴이 있으면 먼저 제안하기
   - 예: 턴 진행 → State Machine, 스탯/스킬 데이터 → ScriptableObject, 로직-연출 분리 → 이벤트/옵저버 패턴

3. **로직과 연출(뷰) 분리**
   - 전투 규칙(데미지 계산, 턴 순서, 상태 관리 등)은 순수 C# 클래스로 작성, Unity API에 의존하지 않기
   - MonoBehaviour는 연출(애니메이션, 이펙트, UI 갱신)에만 사용
   - Core(로직)에서 View(연출)로는 이벤트를 통해서만 통신

4. **데이터는 ScriptableObject로**
   - 캐릭터/몬스터 스탯, 로그라이크 선택지, 스폰 패턴 등은 하드코딩하지 않고 SO 기반으로 설계

5. **큰 변경 전에는 계획을 먼저 요약**
   - 여러 파일에 걸친 리팩터링이나 새 시스템 도입 전에는 계획을 먼저 정리해서 확인받고 진행

6. **Unity 6.4 기준**
   - Deprecated된 API나 구버전 방식 사용하지 않기

## 하지 말아야 할 것

- 매직 넘버/하드코딩된 밸런싱 값 추가하지 않기 (SO 데이터로 분리)
- 최적화를 이유로 불필요하게 스크립트를 쪼개거나 코드량을 늘리지 않기
- 사전 논의 없이 큰 구조 변경 진행하지 않기

## 개발 환경

- Unity **6000.4.9f1** (`ProjectSettings/ProjectVersion.txt` 참고), Unity Hub/Editor로 프로젝트를 열어서 작업
- 별도 CLI 빌드/린트/테스트 스크립트나 CI 파이프라인은 구성되어 있지 않음 — 빌드·플레이 테스트는 Unity Editor에서 직접 수행
- `com.unity.test-framework` 패키지는 설치되어 있지만 아직 테스트 어셈블리/테스트 코드가 없음. 테스트 추가 시 Editor의 Window > General > Test Runner 사용
- `Turn-Based-Game.slnx`는 Unity가 자동 생성한 솔루션 파일로, 대부분의 `.csproj`는 Unity 패키지/에디터 어셈블리이며 실제 게임 코드는 `Assembly-CSharp.csproj`(= `Assets/MyAssets/Scripts/`)에 해당

## 코드 컨벤션

- 스크립트는 `Assets/MyAssets/Scripts/` 하위 폴더 구조를 그대로 반영한 네임스페이스를 사용한다: `namespace Assets.MyAssets.Scripts.<폴더명>` (예: `Assets/MyAssets/Scripts/BattleScene/BattleController.cs` → `namespace Assets.MyAssets.Scripts.BattleScene`)
- 새 폴더를 추가하면 그에 대응하는 네임스페이스도 함께 추가하고, 다른 네임스페이스의 타입을 참조할 때는 `using Assets.MyAssets.Scripts.<폴더명>;`을 명시한다
- `Assets/InputSystem_Actions.inputactions`가 자동 생성하는 `InputSystem_Actions.cs`는 손으로 수정하지 않는다 (재생성 시 덮어써짐). 네임스페이스가 필요하면 해당 에셋의 Import Settings(`wrapperCodeNamespace`)에서 지정한다

## 현재 코드 상태 (2026-07 기준)

- `Assets/MyAssets/Scripts/`에 실질 게임 로직이 구현되어 있음: `BattleScene/`(전투 로직·뷰·연출), `Manager/`(GameManager·PlayerDataManager·StageManager), `Scriptable/`(PlayerData·EnemyData·WeaponData·StageData·SkillData 등 SO), `Struct/`(StatData), `TitleScene/`, `WeaponSelectScene/`, `FadeScreenEffect/` 폴더 구성. 각 폴더는 폴더명과 동일한 네임스페이스를 사용
- 씬은 `Assets/Scenes/`에 `TitleScene.unity` / `WeaponSelectScene.unity` / `BattleScene.unity` 3개가 존재 (README가 정의한 3씬 구조와 일치). 그 외 `AnimationWorkScene.unity`는 애니메이션 작업용 임시 씬
- 다만 기존 코드는 원래 EXP/골드/레벨업 기반 RPG 프로토타입으로 시작되어, README의 로그라이크 무한 타워 기획(파티 최대 4인, 런 단위 로그라이크 성장, 영구 포인트 가중치 배분 등)과는 아직 간극이 큼. 세부 리팩터링 로드맵은 별도 계획 문서로 관리 중
- `Assets/InputSystem_Actions.inputactions`는 Unity 기본 템플릿에 `SelectTarget`(마우스 좌클릭) 액션만 추가된 상태이며, 턴제 커맨드 배틀에 맞는 정리가 아직 필요함

## 아키텍처 방향 (README.md 기획 기반)

아래는 기획 문서와 위 "작업 원칙"에서 도출되는 목표 구조이며, 기존 코드를 이 방향으로 리팩터링해 나간다.

- **씬 흐름**: `TitleScene → SelectWeaponScene(캐릭터 선택 + 성향 포인트 배분) → BattleScene`, 세부 화면은 오버레이/팝업으로 처리(씬 전환 아님)
- **Core / View 분리**: 턴 순서(State Machine), 데미지 계산, 스탯/버프 등 전투 규칙은 Unity API 의존 없는 순수 C# 클래스로; MonoBehaviour는 애니메이션·이펙트·UI 갱신 등 연출 전담이며 Core의 이벤트를 구독하는 방식으로만 갱신
- **데이터**: 캐릭터/몬스터 스탯, 로그라이크 선택지(9종), 스폰 패턴은 하드코딩 대신 ScriptableObject로 관리
- **UI 레이어 분리**: 화면 UI(HUD, 메뉴, 팝업)는 UI Toolkit, 월드스페이스 UI(몬스터 체력바 등)는 uGUI로 별도 구현
