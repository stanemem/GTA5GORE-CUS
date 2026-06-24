# Build and Test — GTA V Legacy Dismemberment/Gore Mod

## 1. Project Target

This document is for the GTA V Legacy single-player Dismemberment/Gore mod.

This is not the NOOSE project.

The mod appears to use:

- ScriptHookVDotNet C# script logic.
- `Main.cs` as the main script entry point.
- `Settings.cs` for `scripts/Dismemberment.toml`.
- `Utils.cs` for helper methods.
- `DismembermentWeapons.cfg` for weapon trigger names.
- `DismembermentASI.asi` as an external native helper.
- Required `.rpf` gore assets installed separately.

## 2. Build Notes

Before building, inspect the repository and project file.

Do not assume the ScriptHookVDotNet version until the references are verified.

Build requirements are likely:

- GTA V Legacy.
- ScriptHookV.
- ScriptHookVDotNet matching the existing project references.
- Existing project references only.
- `DismembermentASI.asi` must remain an external runtime dependency.
- Required `.rpf` gore assets must be installed in the correct GTA V mod location.

Do not:

- Do not add NOOSE project references.
- Do not add NativeUI or LemonUI unless the project already uses them.
- Do not modify `DismembermentASI.asi`.
- Do not add external reference source files to the project.
- Do not add GTA Online, multiplayer, MP map loading, or raw online startup natives.

## 3. Expected Runtime Files

The release/test package should include:

```text
scripts/
├─ Dismemberment.dll
├─ Dismemberment.toml
├─ DismembermentWeapons.cfg
└─ Dismemberment_runtime.log   # generated at runtime if logging is implemented

GTA root or ASI loading path:
└─ DismembermentASI.asi

OpenIV / mods folder:
└─ required gore .rpf assets
```

The exact output DLL name depends on the project file. Keep the existing assembly name unless intentionally changed.

## 4. First Boot Test

1. Start GTA V Legacy story mode.
2. Confirm the game reaches free roam.
3. Check `ScriptHookVDotNet.log`.
4. Check `scripts/Dismemberment_runtime.log` if implemented.
5. Confirm there are no constructor exceptions.
6. Confirm missing optional assets or invalid config do not crash the script.
7. Confirm `DismembermentASI.asi` is loaded or safely skipped/logged if unavailable.

Expected result:

- Game reaches story mode.
- No crash during script construction.
- No repeated Tick exceptions.
- Log clearly states config, weapon cfg, ASI, and asset status.

## 5. Config Fallback Test

Test with `scripts/Dismemberment.toml` present and valid.

Then test these failure cases:

1. Rename `Dismemberment.toml`.
2. Corrupt one config value.
3. Remove optional config keys.
4. Use invalid numeric values.

Expected result:

- The script must not crash.
- Invalid values must clamp or fallback.
- Missing config must generate safe defaults or run with fallback defaults.
- Log must explain the fallback.

Recommended safe defaults:

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
bEnableBloodParticles=true
bEnableBrainChunks=true

[Cleanup]
iMaxActiveCaps=30
iMaxActiveChunks=50
iChunkReleaseDelayMs=2000
iCleanupIntervalMs=1000
```

## 6. Weapon Config Fallback Test

Test with `scripts/DismembermentWeapons.cfg` present.

Then test:

1. Rename `DismembermentWeapons.cfg`.
2. Add invalid weapon names.
3. Leave the file empty.

Expected result:

- The script must not crash.
- Invalid weapon names must be skipped or logged.
- Missing/empty weapon cfg must fallback to a small safe default weapon list.
- Log must state whether the weapon cfg loaded or fallback was used.

## 7. Basic Gore Test

Test in normal single-player free roam with no NOOSE mission active.

1. Spawn or encounter a normal NPC.
2. Kill the NPC with a configured shotgun/sniper/explosive weapon.
3. Confirm dismemberment can trigger.
4. Confirm blood particles/chunks/caps appear if assets are installed.
5. Wait for cleanup.
6. Reload scripts or leave the area.

Expected result:

- Normal NPC gore still works.
- No infinite repeated gore on the same ped.
- No unbounded prop/chunk accumulation.
- Cleanup removes invalid or old props safely.

## 8. Melee Safety Test

This is critical for compatibility.

1. Set `bOnlyDismemberDeadPeds=true`.
2. Set `bAllowMeleeDismemberLivingPeds=false`.
3. Hit a living high-health ped with a melee weapon.
4. Confirm the gore mod does not kill the ped only for visual dismemberment.

Expected result:

- Living peds are not force-killed by gore logic by default.
- Melee dismemberment only occurs post-death unless explicitly enabled.

## 9. NOOSE Compatibility Test

Run this only after the gore mod works standalone.

1. Install the current custom NOOSE build.
2. Keep `bNooseCompatibilityMode=true`.
3. Keep `bOnlyDismemberDeadPeds=true`.
4. Keep `bAllowMeleeDismemberLivingPeds=false`.
5. Keep `bPreserveDamageEvidence=true`.
6. Start a basic NOOSE ground mission.
7. Fight normal enemies.
8. Fight high-health enemies if available.
9. Test headshots.
10. Check `NOOSE_runtime.log`.
11. Check `Dismemberment_runtime.log`.

Expected result:

- NOOSE starts normally.
- NOOSE enemies are not killed early by gore logic.
- NOOSE custom headshot damage still logs and applies.
- NOOSE enemy death gate/durable health behavior is not broken.
- Gore can trigger after enemy death without controlling enemy death logic.

## 10. Immersive Combat Compatibility Test

Run only after standalone and NOOSE compatibility tests pass.

1. Enable Immersive Combat.
2. Keep `bImmersiveCombatCompatibilityMode=true`.
3. Test normal combat.
4. Test shotgun/sniper/explosive kills.
5. Test ragdoll behavior.
6. Check logs.

Expected result:

- No global weapon damage override.
- No global player damage override.
- No broad ragdoll override.
- Gore remains mostly visual/post-death.

## 11. Cleanup / Reload Test

1. Trigger gore several times.
2. Reload ScriptHookVDotNet scripts.
3. Move far away and return.
4. Start a new combat encounter.
5. Repeat for several minutes.

Expected result:

- No stale prop/entity exceptions.
- No collection modification exceptions.
- No uncontrolled cap/chunk growth.
- `Aborted` cleanup should be idempotent.
- Invalid entities should be removed from tracking lists safely.

## 12. External Conflict Test

Known environment notes:

- ActionGear appears to conflict with the user's NOOSE/SHVDN runtime setup.
- NVE/mods folder can crash MP maps in this user's environment.
- This gore mod must not patch ActionGear or NVE.

If crashes occur:

1. Test gore mod alone.
2. Test gore mod + NOOSE.
3. Test gore mod + Immersive Combat.
4. Remove ActionGear and retest.
5. Avoid online-map/NVE assumptions.

Do not blame the gore mod until the minimal mod stack has been tested.

## 13. Required Logs to Collect

For crash or startup failure:

- `ScriptHookVDotNet.log`
- `scripts/Dismemberment_runtime.log`
- Windows Event Viewer crash info if available
- Any ASI loader log if present

For NOOSE compatibility issues:

- `ScriptHookVDotNet.log`
- `scripts/Dismemberment_runtime.log`
- `scripts/NOOSE_runtime.log`
- `scripts/NOOSE_config.log`

For asset issues:

- Whether `.rpf` gore assets are installed
- Whether `DismembermentASI.asi` is installed
- Whether particle FX requests succeeded or failed

## 14. Acceptance Checklist

A build is acceptable only if:

- It builds successfully.
- GTA V Legacy reaches story mode.
- Missing config does not crash.
- Missing weapon cfg does not crash.
- Missing optional effect assets do not crash.
- Normal NPC gore still works.
- Living high-health enemies are not force-killed by default.
- NOOSE custom headshot/death gate behavior remains intact.
- Props/chunks/effects are capped and cleaned.
- Script reload/abort does not leave persistent garbage.
- Logs are clear enough to diagnose failures.
