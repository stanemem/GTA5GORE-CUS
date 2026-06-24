# AGENTS.md — GTA V Legacy Dismemberment/Gore Mod

## Project Context

This repository customizes and stabilizes a GTA V Legacy single-player Dismemberment/Gore mod.

The mod is a ScriptHookVDotNet C# script with an external native ASI helper:

* `Main.cs`: main ScriptHookVDotNet script, Tick loop, damage detection, dismemberment logic.
* `Settings.cs`: config loading from `scripts/Dismemberment.toml`.
* `Utils.cs`: helper methods for excluded peds, PTFX loading, DLC detection, cloning.
* `DismembermentASI.asi`: external native helper used through P/Invoke.
* `DismembermentWeapons.cfg`: weapon names that can trigger dismemberment.
* `.rpf` assets: required gore props/models/effects.

This project is for GTA V Legacy single-player only.

## External ASI Boundary

`DismembermentASI.asi` is an external binary dependency.

Do not modify, patch, reverse engineer, or replace the ASI unless explicitly instructed by the user.

Treat these P/Invoke calls as risky external calls:

* `AddBoneDraw(int handle, int start, int end)`
* `RemoveBoneDraw(int handle)`

Wrap ASI calls defensively where possible. If ASI calls fail, log and skip optional gore visuals rather than crashing the script.

## Compatibility Context

The user also maintains a separate custom NOOSE mod in the same GTA V Legacy environment.

NOOSE may modify:

* Enemy HP
* Enemy armor
* Custom headshot damage
* Critical hit behavior
* Durable enemy death gates
* Juggernaut virtual health
* Enemy AI/combat behavior
* Enemy cleanup

Immersive Combat may also affect damage, ragdoll, and combat feel.

Therefore this gore mod must avoid broad damage hacks and must not break NOOSE enemy durability or headshot systems.

## Hard Rules

Do not:

* Add GTA Online support.
* Add multiplayer support.
* Add MP map loading.
* Add raw online startup natives.
* Add NVE-specific hacks.
* Patch ActionGear.
* Modify external ASI binaries.
* Globally modify weapon damage.
* Globally modify player damage.
* Kill living high-health enemies as a side effect of visual gore unless explicitly config-enabled.
* Clear ped damage evidence in a way that can break NOOSE custom headshot handling.
* Spawn unbounded gore props/chunks/effects.
* Access invalid or deleted peds/props/entities without checking safety first.
* Leave optional Tick logic able to crash the script.

## Required Coding Principles

Use defensive coding everywhere.

For every Ped, Prop, Entity, or Vehicle:

* Check null.
* Check `Exists()` where available.
* Avoid stale handles.
* Use safe helper methods for repeated entity operations.
* Never assume a ped is still valid between detection and gore application.

For Tick logic:

* Keep it lightweight.
* Avoid nested expensive work where possible.
* Do not throw from optional gore logic.
* Catch and log non-critical errors.
* Add throttled logging to avoid spam.

For config:

* Missing config must not crash the mod.
* Invalid config values must clamp or fallback.
* Missing `DismembermentWeapons.cfg` must fallback to safe defaults.
* Add master kill switches and compatibility switches.

For cleanup:

* Track spawned props/chunks.
* Use max active limits.
* Remove invalid entries safely using reverse loops.
* Do not modify a collection during `foreach`.
* Make cleanup idempotent.

## Recommended Config Defaults

Default behavior should prioritize compatibility:

* `EnableDismemberment=true`
* `SafeMode=false`
* `NooseCompatibilityMode=true`
* `ImmersiveCombatCompatibilityMode=true`
* `OnlyDismemberDeadPeds=true`
* `AllowMeleeDismemberLivingPeds=false`
* `PreserveDamageEvidence=true`
* `EnableTorsoDismemberment=false`
* `EnableVehicleImpactDismemberment=false`
* `EnableDebugLogging=true`
* `LogGoreEvents=false`
* `MaxActiveCaps=30`
* `MaxActiveChunks=50`
* `ChunkReleaseDelayMs=2000`

## Logging

Add or maintain:

* `scripts/Dismemberment_runtime.log`

Log:

* Constructor entered/exited.
* Config loaded/fallback.
* Weapon cfg loaded/fallback.
* DLC/rpf check result.
* ASI availability or failure.
* PTFX request result.
* Safe mode state.
* Compatibility mode state.
* Cleanup summaries.
* Optional gore errors.

Avoid logging every gore event unless `LogGoreEvents=true`.

## Build Rules

Before coding, inspect the actual project file and references.

Do not assume SHVDN2 or SHVDN3 until verified from the repository.

Do not add NativeUI or LemonUI unless already required.

Do not add external reference source files to the project.

Use external reference folders only as API reference.

## Acceptance Tests

A patch is acceptable only if:

1. The project builds.
2. Missing `DismembermentWeapons.cfg` does not crash the mod.
3. Missing/invalid config does not crash the mod.
4. The game reaches story mode without constructor exceptions.
5. Killing normal NPCs with configured weapons still triggers gore.
6. NOOSE high-health enemies are not killed early by gore logic.
7. NOOSE custom headshot behavior is not broken by clearing damage evidence.
8. Props/chunks are cleaned and capped.
9. Aborting/reloading scripts does not leave persistent gore props behind.
10. Logs explain fallback or skipped behavior clearly.
