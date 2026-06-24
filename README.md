# GTA V Legacy Dismemberment / Gore Mod — Custom Repair Build

GTA V Legacy 싱글플레이 전용 Dismemberment / Gore 모드 커스텀 빌드입니다.

이 프로젝트는 기존 GTA5 Dismemberment Mod / Repair 계열 소스를 기반으로, 현재 GTA V Legacy 환경에서 안정적으로 동작하도록 수정하고 커스텀하기 위한 작업용 저장소입니다.

> 이 모드는 GTA Online용이 아닙니다.  
> GTA Online, 멀티플레이, 안티치트 우회, 온라인 맵 강제 로딩 기능은 지원하지 않습니다.

\---

## 1\. 프로젝트 목적

이 저장소의 목표는 단순히 고어 효과를 더 강하게 만드는 것이 아니라, 다음 조건을 만족하는 안정적인 싱글플레이 고어 모드를 만드는 것입니다.

* GTA V Legacy story mode에서 안정적으로 부팅
* ScriptHookVDotNet 환경에서 Tick 예외 최소화
* `Dismemberment.toml` 기반 설정 확장
* `DismembermentWeapons.cfg` 기반 무기별 고어 트리거 관리
* 외부 `DismembermentASI.asi` 의존성 안전 처리
* 피, 절단, 파티클, 고어 prop/chunk cleanup 안정화
* NOOSE 커스텀 모드의 적 체력/헤드샷/데스게이트 시스템과 충돌 방지
* Immersive Combat 같은 전투 모드와 충돌 최소화

\---

## 2\. 현재 구성 파일

```text
Main.cs
Settings.cs
Utils.cs
AssemblyInfo.cs
Dismemberment.toml
DismembermentWeapons.cfg
DismembermentASI.asi
ReadME.md
```

역할:

* `Main.cs`  
메인 ScriptHookVDotNet 스크립트입니다. Tick 루프, 주변 Ped 검사, 데미지 뼈 감지, 절단 처리, 파티클/prop 생성, cleanup을 담당합니다.
* `Settings.cs`  
`scripts/Dismemberment.toml` 설정을 읽습니다.
* `Utils.cs`  
제외 Ped 확인, PTFX 로드, DLC/rpf 설치 확인, Ped clone 관련 헬퍼를 담당합니다.
* `Dismemberment.toml`  
사용자 설정 파일입니다.
* `DismembermentWeapons.cfg`  
절단 효과를 유발할 무기 이름 목록입니다.
* `DismembermentASI.asi`  
외부 native helper입니다. C# 코드에서 P/Invoke로 호출됩니다. 이 파일은 수정 대상이 아닙니다.

\---

## 3\. 설치 방법

### 필수 준비물

* GTA V Legacy
* ScriptHookV
* ScriptHookVDotNet
* OpenIV
* ASI Loader
* 이 모드의 `.dll`, `.cfg`, `.toml`, `.asi`
* 고어 에셋이 들어 있는 `dlc.rpf`

### 파일 배치

#### scripts 폴더

GTA V Legacy 루트의 `scripts` 폴더에 다음 파일을 넣습니다.

```text
scripts/
├─ Dismemberment.dll
├─ Dismemberment.toml
└─ DismembermentWeapons.cfg
```

#### GTA 루트 폴더

GTA V Legacy 루트 폴더에 다음 파일을 넣습니다.

```text
DismembermentASI.asi
```

#### OpenIV / mods 폴더

OpenIV를 사용해서 `dlc.rpf`를 아래 경로에 넣습니다.

```text
mods\\update\\x64\\dlcpacks\\dismemberment\\dlc.rpf
```

이후 아래 파일을 엽니다.

```text
mods\\update\\update.rpf\\common\\data\\dlclist.xml
```

`</Paths>` 위에 다음 줄을 추가합니다.

```xml
<Item>dlcpacks:/dismemberment/</Item>
```

\---

## 4\. 추천 설정 방향

이 커스텀 빌드는 호환성을 우선합니다.

특히 NOOSE 커스텀 모드와 같이 사용할 경우, 고어모드는 적을 직접 죽이는 모드가 아니라 죽은 Ped 또는 안전한 조건의 Ped에 시각 효과를 적용하는 모드로 동작해야 합니다.

추천 기본 방향:

```toml
\[General]
bEnableDismemberment=true
bSafeMode=false
bDebugLogging=true
bLogGoreEvents=false

\[Compatibility]
bNooseCompatibilityMode=true
bImmersiveCombatCompatibilityMode=true
bOnlyDismemberDeadPeds=true
bAllowMeleeDismemberLivingPeds=false
bPreserveDamageEvidence=true
bIgnorePlayer=true
bIgnoreAllies=true
bIgnoreMissionCriticalPeds=true

\[Gore]
bDismemberTorso=false
bPedPainSound=true
bEnableHeadDismemberment=true
bEnableLimbDismemberment=true
bEnableBloodParticles=true
bEnableBrainChunks=true
fBloodParticleScale=1.0
iMinChunks=3
iMaxChunks=12

\[Cleanup]
iMaxActiveCaps=30
iMaxActiveChunks=50
iChunkReleaseDelayMs=2000
iCleanupIntervalMs=1000
```

실제 config key 이름은 구현 상태에 따라 달라질 수 있습니다.  
Codex 작업 시 위 설정을 기준으로 안전한 config 확장을 진행합니다.

\---

## 5\. NOOSE 호환 주의사항

사용자의 GTA V Legacy 환경에는 별도의 커스텀 NOOSE 모드가 존재할 수 있습니다.

NOOSE는 다음 시스템을 자체적으로 제어할 수 있습니다.

* 적 체력
* 적 방탄복
* 커스텀 헤드샷 데미지
* 치명타 비활성화
* 고체력 적 사망 게이트
* Juggernaut 가상 체력
* 적 AI / 관계 그룹
* 미션 cleanup

따라서 이 고어모드는 다음을 하지 않아야 합니다.

* 전역 무기 데미지 변경
* 전역 플레이어 데미지 변경
* 살아 있는 고체력 NOOSE 적을 고어 연출 때문에 강제 사망 처리
* NOOSE가 읽어야 할 데미지 뼈/무기 기록을 무조건 삭제
* NOOSE 내부 파일 수정
* NOOSE 내부 타입에 강하게 의존

안전한 원칙:

> 전투/데미지/사망 판정은 GTA, NOOSE, 전투 모드가 담당하고,  
> 고어모드는 가능한 한 사망 이후의 시각 효과를 담당한다.

\---

## 6\. Immersive Combat 호환 주의사항

Immersive Combat은 무기 데미지, 래그돌, 전투 감각, 적 생존성에 영향을 줄 수 있습니다.

이 모드와 같이 사용할 때는 다음 원칙을 지킵니다.

* 전역 데미지 수정 금지
* 전역 래그돌 수정 금지
* 플레이어 데미지 수정 금지
* 고어 효과는 가능한 한 post-death visual effect로 유지
* 위험한 기능은 config로 끄고 켤 수 있게 유지

\---

## 7\. 알려진 외부 충돌 후보

### ActionGear

사용자 환경에서 ActionGear는 NOOSE / ScriptHookVDotNet 런타임과 충돌하는 것으로 의심되었습니다.

크래시가 발생하면 다음 순서로 테스트하십시오.

1. 고어모드 단독 실행
2. 고어모드 + NOOSE 실행
3. 고어모드 + Immersive Combat 실행
4. ActionGear 제거 후 재테스트

이 프로젝트는 ActionGear를 패치하거나 우회하지 않습니다.

### NVE / mods 폴더 / 온라인 맵

사용자 환경에서 NVE/mods 폴더는 MP maps / online map loading과 충돌할 수 있었습니다.

이 고어모드는 MP map loading을 추가하지 않습니다.  
NVE 관련 hack도 추가하지 않습니다.

\---

## 8\. 테스트 방법

### 단독 테스트

1. GTA V Legacy story mode 실행
2. free roam 진입 확인
3. `ScriptHookVDotNet.log` 확인
4. `scripts/Dismemberment\_runtime.log` 확인
5. 일반 NPC를 설정된 무기로 처치
6. 절단, 피, chunk, cap prop 표시 확인
7. 일정 시간 후 cleanup 확인
8. 스크립트 reload 또는 게임 재시작 후 잔여 prop 확인

### 설정 누락 테스트

다음 파일을 임시로 이름 변경 후 실행합니다.

```text
scripts/Dismemberment.toml
scripts/DismembermentWeapons.cfg
```

기대 결과:

* 게임이 크래시하지 않음
* 기본값 fallback
* 로그에 누락/복구 내용 기록

### NOOSE 호환 테스트

1. NOOSE 정상 작동 확인
2. 고어모드 활성화
3. NOOSE 일반 지상 미션 시작
4. 일반 적 / 고체력 적 / 헤드샷 테스트
5. `NOOSE\_runtime.log`와 `Dismemberment\_runtime.log` 확인

기대 결과:

* NOOSE 적이 고어모드 때문에 조기 사망하지 않음
* NOOSE 커스텀 헤드샷 데미지가 정상 기록됨
* 고어 효과는 사망 후 정상 작동

\---

## 9\. 로그

권장 로그 파일:

```text
scripts/Dismemberment\_runtime.log
```

기록하면 좋은 내용:

* constructor entered/exited
* config loaded/fallback
* weapon cfg loaded/fallback
* ASI call availability
* DLC/rpf asset check
* PTFX load result
* SafeMode state
* NOOSE compatibility state
* cleanup summary
* optional gore error

`bLogGoreEvents=true`가 아닌 이상, 매번 고어 이벤트를 과도하게 기록하지 않는 것이 좋습니다.

\---

## 10\. Codex / 개발자 작업 규칙

Codex가 이 저장소를 수정할 때 지켜야 할 규칙:

* 먼저 `AGENTS.md`, `docs/`, `BUILD\_AND\_TEST.md`, `CURRENT\_STATUS.md`를 읽을 것
* `DismembermentASI.asi` 수정 금지
* 외부 참조 폴더는 reference로만 사용할 것
* GTA Online / multiplayer / MP map loading 추가 금지
* NVE-specific hack 추가 금지
* ActionGear patch 추가 금지
* 전역 데미지 변경 금지
* 플레이어 데미지 변경 금지
* missing config / missing cfg / missing asset으로 crash 금지
* Tick에서 optional gore error가 script 전체를 죽이지 않게 할 것
* 모든 Ped/Prop/Entity 접근 전 null/Exists 체크
* cleanup은 idempotent하게 유지
* collection을 foreach 중 수정하지 말 것

\---

## 11\. 빌드 완료 기준

수정본은 다음 조건을 만족해야 합니다.

* 프로젝트 빌드 성공
* GTA V Legacy story mode 진입 성공
* missing config에서 crash 없음
* missing weapon cfg에서 crash 없음
* 일반 NPC 고어 효과 정상
* NOOSE 고체력 적 조기 사망 없음
* NOOSE 헤드샷/데스게이트 로직 유지
* chunk/cap prop 무한 누적 없음
* 스크립트 reload/abort cleanup 안전
* 로그로 문제 원인 추적 가능

\---

## 12\. Credits / Original Sources

This custom repair project is based on the GTA5 Dismemberment Mod / Repair lineage.

Original references:

* GTA5 Dismemberment Mod
* GTA5-DismembermentMod source
* DismembermentASI.asi external helper
* Required `.rpf` gore assets

Respect the original authors and asset creators.  
Do not redistribute third-party assets without permission.

