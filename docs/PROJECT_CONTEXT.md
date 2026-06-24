\# GTA V Legacy Dismemberment/Gore Mod — Project Context



\## Goal



Customize and stabilize a GTA V Legacy single-player Dismemberment/Gore mod.



The goal is not to make the mod more chaotic first. The first goal is to make it safe, configurable, and compatible with the user's existing GTA V Legacy mod stack.



\## Current Architecture



The current mod appears to use:



\* ScriptHookVDotNet C# script logic.

\* An external `DismembermentASI.asi` native helper.

\* Custom `.rpf` assets for gore caps/chunks.

\* `scripts/Dismemberment.toml` for settings.

\* `scripts/DismembermentWeapons.cfg` for weapon triggers.



Core script behavior:



1\. On script construction:



&#x20;  \* Checks whether the dismemberment DLC/rpf is present.

&#x20;  \* Loads settings.

&#x20;  \* Reads the weapon config file.

&#x20;  \* Requests particle FX assets.

&#x20;  \* Subscribes to Tick and Aborted.



2\. On every Tick:



&#x20;  \* Scans nearby peds around the player.

&#x20;  \* Checks last damaged bone and weapon damage.

&#x20;  \* Calls dismemberment logic when conditions match.

&#x20;  \* Cleans spawned cap/chunk props.



3\. Dismemberment logic:



&#x20;  \* May kill the ped.

&#x20;  \* May clone the ped for melee/head/torso cases.

&#x20;  \* Spawns cap props and brain chunks.

&#x20;  \* Calls ASI bone drawing.

&#x20;  \* Starts blood particle effects.

&#x20;  \* Marks peds/props no longer needed.



\## Known Risk Areas



\### Constructor Risk



The constructor performs file IO, config loading, PTFX requests, and Tick subscription. This should be made safer.



\### Damage Evidence Risk



The current script clears ped weapon damage and last damaged bone data. This can conflict with other mods that inspect the same evidence, especially NOOSE custom headshot logic.



\### Living Ped Kill Risk



The current logic can kill living peds when melee damage triggers dismemberment. This can break high-health enemies from NOOSE.



\### Cleanup Risk



The current prop cleanup logic may remove items while iterating and may access null/deleted props unsafely.



\### ASI Risk



The ASI helper is external and pattern-based. Treat it as a black box and guard calls.



\## Compatibility Goals



\### NOOSE Compatibility



When `NooseCompatibilityMode=true`:



\* Do not kill living NOOSE enemies for gore visuals.

\* Prefer dead-ped-only dismemberment.

\* Do not clear damage evidence globally.

\* Do not override NOOSE enemy health, armor, critical hits, or death gate logic.

\* Avoid touching player squad/teammates.



\### Immersive Combat Compatibility



When `ImmersiveCombatCompatibilityMode=true`:



\* Do not globally change weapon damage.

\* Do not globally change ped ragdoll behavior.

\* Keep gore mostly visual and post-death.



\## Recommended Patch Order



1\. Repository audit and build verification.

2\. Constructor and config hardening.

3\. Safe entity helpers and safe cleanup.

4\. NOOSE compatibility mode.

5\. Expanded config.

6\. Weapon group config.

7\. Debug logging and release packaging.

8\. Optional visual tuning pass.



\## Release Package Target



A clean release package should include:



\* `Dismemberment.dll`

\* `DismembermentASI.asi`

\* `Dismemberment.toml`

\* `DismembermentWeapons.cfg`

\* Required `.rpf` installation instructions

\* `README\_CUSTOM.md`

\* `CONFIG\_REFERENCE.md`

\* `COMPATIBILITY\_NOTES.md`

\* `CHANGELOG\_CUSTOM.md`



