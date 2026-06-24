# Current Status — GTA V Legacy Dismemberment/Gore Mod

## 1. Project Identity

This is the GTA V Legacy single-player Dismemberment/Gore mod customization project.

This is separate from the custom NOOSE mod.

The goal is to stabilize and customize gore/dismemberment behavior while keeping compatibility with:

- GTA V Legacy story mode.
- ScriptHookV.
- ScriptHookVDotNet.
- The user's custom NOOSE mod.
- EnableAllInteriors where relevant to the wider GTA install.
- Immersive Combat.
- External gore assets and `DismembermentASI.asi`.

## 2. Current Source Structure

Known files:

```text
Main.cs
Settings.cs
Utils.cs
AssemblyInfo.cs
Dismemberment.toml
DismembermentWeapons.cfg
DismembermentASI.asi
ReadMe.md
```

Likely roles:

- `Main.cs`: main ScriptHookVDotNet script, Tick loop, damage detection, gore/dismemberment execution, cleanup.
- `Settings.cs`: loads and stores values from `scripts/Dismemberment.toml`.
- `Utils.cs`: helper methods for PTFX loading, excluded peds, DLC/rpf checks, ped clone helpers.
- `AssemblyInfo.cs`: assembly metadata.
- `Dismemberment.toml`: user config.
- `DismembermentWeapons.cfg`: weapon trigger list.
- `DismembermentASI.asi`: external native helper used by the script.
- `ReadMe.md`: original credits and notes.

## 3. Important External Dependency

`DismembermentASI.asi` is an external binary dependency.

Current README credits:

- `DismembermentASI.asi` is from CamxxCore.
- Files inside `.rpf` are from SpringBunny and Ze Krush.

Project rule:

- Do not modify, patch, or reverse engineer `DismembermentASI.asi`.
- Treat it as a black-box runtime helper.
- Wrap calls defensively.
- If ASI calls fail, log and skip optional visual effects.

## 4. Current Behavior Summary

Based on the source layout, the mod likely does the following:

1. Loads settings from `scripts/Dismemberment.toml`.
2. Loads allowed dismemberment weapons from `scripts/DismembermentWeapons.cfg`.
3. Checks whether required gore DLC/rpf assets are installed.
4. Requests particle FX assets.
5. Subscribes to ScriptHookVDotNet Tick.
6. Scans nearby peds around the player.
7. Checks recently damaged bones and weapon damage.
8. Applies dismemberment logic.
9. Spawns cap props, chunks, and blood effects.
10. Uses `DismembermentASI.asi` for bone drawing / hidden bone visuals.
11. Tracks and cleans gore props/chunks.

## 5. Known Risk Areas

### Constructor Risk

The script may perform file IO, config loading, particle requests, and Tick subscription directly in the constructor.

Risk:

- Missing files or bad config can crash script construction.
- Heavy constructor work can destabilize ScriptHookVDotNet startup.

Recommended direction:

- Keep constructor lightweight.
- Defer heavy initialization until world/player ready.
- Add fallback and logging.

### Config Risk

Current config appears minimal.

Known settings include:

```toml
bDismemberTorso=false
bPedPainSound=true
```

Risk:

- Missing or invalid config may crash.
- There are not enough kill switches for compatibility.

Recommended direction:

- Add master enable.
- Add safe mode.
- Add logging toggles.
- Add NOOSE/Immersive Combat compatibility settings.
- Add cleanup limits.

### Weapon Config Risk

`DismembermentWeapons.cfg` is required for weapon trigger names.

Risk:

- Missing file can crash if read directly.
- Invalid weapon names may create bad behavior.
- Empty file may disable all functionality without clear logs.

Recommended direction:

- Missing file should use fallback defaults.
- Invalid names should be skipped and logged.
- Weapon groups should be added later.

### Tick Risk

The script likely scans nearby peds on Tick.

Risk:

- Tick exceptions can spam logs or crash script behavior.
- Deleted peds/props can be accessed after invalidation.
- Repeated processing can trigger duplicate gore.

Recommended direction:

- Add defensive null/Exists checks.
- Throttle expensive work.
- Add processed-ped tracking.
- Clean tracking entries.

### Damage Evidence Risk

The script may clear ped weapon damage or damaged bone state after processing.

Risk:

- This can break NOOSE custom headshot damage, because NOOSE may also inspect recent damaged bone/weapon state.

Recommended direction:

- Add `bPreserveDamageEvidence=true` by default.
- Avoid clearing damage evidence globally in compatibility mode.
- Use internal tracking to prevent duplicate gore.

### Living Ped Kill Risk

The current dismemberment logic may kill living peds for melee dismemberment.

Risk:

- NOOSE high-health enemies, Armored enemies, Bosses, and Juggernauts can die early.
- Immersive Combat balance can be broken.

Recommended direction:

- Add `bOnlyDismemberDeadPeds=true`.
- Add `bAllowMeleeDismemberLivingPeds=false`.
- Default gore behavior should be mostly post-death.

### Cleanup Risk

Gore caps/chunks/props can accumulate.

Risk:

- Collection modification during foreach.
- Invalid prop access.
- Unbounded entity growth.
- Script reload leaves garbage behind.

Recommended direction:

- Use reverse-loop cleanup.
- Use `prop == null || !prop.Exists()` checks.
- Add max active caps/chunks.
- Make Aborted cleanup idempotent.

## 6. Current Recommended Defaults

Use compatibility-first defaults:

```toml
[General]
bEnableDismemberment=true
bSafeMode=false
bDebugLogging=true
bLogGoreEvents=false

[Compatibility]
bNooseCompatibilityMode=true
bImmersiveCombatCompatibilityMode=true
bOnlyDismemberDeadPeds=true
bAllowMeleeDismemberLivingPeds=false
bPreserveDamageEvidence=true
bIgnorePlayer=true
bIgnoreAllies=true
bIgnoreMissionCriticalPeds=true

[Gore]
bDismemberTorso=false
bPedPainSound=true
bEnableHeadDismemberment=true
bEnableLimbDismemberment=true
bEnableBloodParticles=true
bEnableBrainChunks=true
fBloodParticleScale=1.0
iMinChunks=3
iMaxChunks=12

[Cleanup]
iMaxActiveCaps=30
iMaxActiveChunks=50
iChunkReleaseDelayMs=2000
iCleanupIntervalMs=1000
```

## 7. Compatibility Context

### NOOSE

NOOSE may already control:

- Enemy HP.
- Enemy armor.
- Custom headshot damage.
- Critical hit behavior.
- Durable enemy death gate.
- Juggernaut virtual health.
- Relationship groups.
- Enemy combat AI.
- Enemy cleanup.

Therefore this gore mod must not:

- Globally modify weapon damage.
- Globally modify player damage.
- Kill living NOOSE enemies for visual gore.
- Clear damage evidence needed by NOOSE.
- Patch NOOSE files or rely on NOOSE internals unless explicitly requested.

### Immersive Combat

Immersive Combat may affect:

- Weapon damage.
- Combat behavior.
- Ragdoll.
- Enemy survivability.

Therefore this gore mod should:

- Stay mostly visual.
- Prefer post-death effects.
- Avoid global damage/ragdoll changes.
- Keep risky behavior config-gated.

### ActionGear

Known external conflict from the user's environment:

- ActionGear appears to conflict with the user's NOOSE/SHVDN runtime setup.
- If crashes happen with the full mod stack, retest with ActionGear removed.

Project rule:

- Do not patch ActionGear.
- Do not add ActionGear-specific code unless explicitly requested later.

### NVE / MP Maps

Known environment finding from NOOSE testing:

- NVE/mods folder can crash MP/online map loading in the user's environment.
- This gore mod must not add MP map loading.
- Do not add NVE-specific hacks.

## 8. Current Priority Plan

### Phase 1 — Audit and Stabilization

- Verify project references and SHVDN version.
- Make config loading safe.
- Make weapon cfg loading safe.
- Add runtime log.
- Guard ASI calls.
- Add safe entity checks.
- Fix cleanup loops.
- Add max active gore prop/chunk limits.
- Avoid constructor exceptions.

### Phase 2 — Compatibility Mode

- Add NOOSE compatibility defaults.
- Add Immersive Combat compatibility defaults.
- Prevent living high-health ped kill by default.
- Preserve damage evidence by default.
- Replace damage clearing dependency with internal processed tracking.

### Phase 3 — Config Expansion

- Add feature toggles.
- Add intensity controls.
- Add cleanup controls.
- Add weapon group controls.

### Phase 4 — Release Package

- Package DLL, ASI, toml, cfg, and docs.
- Add config reference.
- Add troubleshooting guide.
- Add compatibility notes.
- Add changelog.

## 9. Known Good Direction

The safest design principle:

The gore mod should make dead or dying peds look better. It should not become the authority for who dies, how much damage weapons do, or how high-health enemies are defeated.

In other words:

- Combat mods decide damage.
- NOOSE decides NOOSE enemy durability.
- The gore mod decides visual gore effects after safe conditions are met.

## 10. Current Next Codex Task

Recommended next Codex task:

```text
Read AGENTS.md and docs/ first.

Do a low-risk audit and stabilization pass for the GTA V Legacy Dismemberment/Gore mod.

Focus on:
- safe config fallback
- safe weapon cfg fallback
- safe ASI boundary
- safe Tick/entity checks
- safe cleanup loops
- runtime logging
- compatibility risk review

Do not add new gore features yet.
Do not modify DismembermentASI.asi.
Do not add global damage changes.
Do not add GTA Online, MP map loading, NVE hacks, or ActionGear patches.

After inspection, either:
1. produce a patch plan if the repository needs clarification, or
2. implement only the low-risk stabilization patch.

Acceptance test:
- project builds
- game reaches story mode
- missing config does not crash
- missing weapon cfg does not crash
- normal NPC gore still works
- NOOSE high-health enemies are not killed early
- cleanup is capped and safe
```
