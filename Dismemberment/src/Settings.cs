using System;
using System.IO;
using GTA;

namespace Dismemberment
{
    internal static class ModSettings
    {
        private const string ConfigPath = "scripts\\Dismemberment.toml";

        internal static void LoadSettings(Action<string> log)
        {
            ApplyDefaults();

            try
            {
                if (!File.Exists(ConfigPath))
                {
                    log?.Invoke("Config missing at scripts\\Dismemberment.toml; using compatibility defaults.");
                }

                toml = ScriptSettings.Load(ConfigPath);
                if (toml == null)
                {
                    log?.Invoke("Config loader returned null; using compatibility defaults.");
                    return;
                }

                enableDismemberment = ReadBool("General", "bEnableDismemberment", enableDismemberment, log);
                safeMode = ReadBool("General", "bSafeMode", safeMode, log);
                debugLogging = ReadBool("General", "bDebugLogging", debugLogging, log);
                logGoreEvents = ReadBool("General", "bLogGoreEvents", logGoreEvents, log);

                nooseCompatibilityMode = ReadBool("Compatibility", "bNooseCompatibilityMode", nooseCompatibilityMode, log);
                immersiveCombatCompatibilityMode = ReadBool("Compatibility", "bImmersiveCombatCompatibilityMode", immersiveCombatCompatibilityMode, log);
                onlyDismemberDeadPeds = ReadBool("Compatibility", "bOnlyDismemberDeadPeds", onlyDismemberDeadPeds, log);
                allowMeleeDismemberLivingPeds = ReadBool("Compatibility", "bAllowMeleeDismemberLivingPeds", allowMeleeDismemberLivingPeds, log);
                preserveDamageEvidence = ReadBool("Compatibility", "bPreserveDamageEvidence", preserveDamageEvidence, log);
                ignorePlayer = ReadBool("Compatibility", "bIgnorePlayer", ignorePlayer, log);
                ignoreAllies = ReadBool("Compatibility", "bIgnoreAllies", ignoreAllies, log);
                ignoreMissionCriticalPeds = ReadBool("Compatibility", "bIgnoreMissionCriticalPeds", ignoreMissionCriticalPeds, log);

                bool legacyTorso = ReadBool("Settings", "bDismemberTorso", dismemberTorso, log);
                bool legacyPainSound = ReadBool("Settings", "bPedPainSound", pedPainSound, log);
                dismemberTorso = ReadBool("Gore", "bDismemberTorso", legacyTorso, log);
                pedPainSound = ReadBool("Gore", "bPedPainSound", legacyPainSound, log);
                enableHeadDismemberment = ReadBool("Gore", "bEnableHeadDismemberment", enableHeadDismemberment, log);
                enableLimbDismemberment = ReadBool("Gore", "bEnableLimbDismemberment", enableLimbDismemberment, log);
                enableBloodParticles = ReadBool("Gore", "bEnableBloodParticles", enableBloodParticles, log);
                enableBrainChunks = ReadBool("Gore", "bEnableBrainChunks", enableBrainChunks, log);
                minChunks = ReadInt("Gore", "iMinChunks", minChunks, 0, 50, log);
                maxChunks = ReadInt("Gore", "iMaxChunks", maxChunks, minChunks, 50, log);

                maxActiveCaps = ReadInt("Cleanup", "iMaxActiveCaps", maxActiveCaps, 0, 200, log);
                maxActiveChunks = ReadInt("Cleanup", "iMaxActiveChunks", maxActiveChunks, 0, 300, log);
                chunkReleaseDelayMs = ReadInt("Cleanup", "iChunkReleaseDelayMs", chunkReleaseDelayMs, 0, 60000, log);
                cleanupIntervalMs = ReadInt("Cleanup", "iCleanupIntervalMs", cleanupIntervalMs, 250, 60000, log);

                log?.Invoke("Config loaded. SafeMode=" + safeMode
                    + ", NooseCompatibilityMode=" + nooseCompatibilityMode
                    + ", ImmersiveCombatCompatibilityMode=" + immersiveCombatCompatibilityMode
                    + ", OnlyDismemberDeadPeds=" + onlyDismemberDeadPeds
                    + ", PreserveDamageEvidence=" + preserveDamageEvidence + ".");
            }
            catch (Exception ex)
            {
                ApplyDefaults();
                log?.Invoke("Config load failed; using compatibility defaults. " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static ScriptSettings toml;

        internal static bool enableDismemberment;

        internal static bool safeMode;

        internal static bool debugLogging;

        internal static bool logGoreEvents;

        internal static bool nooseCompatibilityMode;

        internal static bool immersiveCombatCompatibilityMode;

        internal static bool onlyDismemberDeadPeds;

        internal static bool allowMeleeDismemberLivingPeds;

        internal static bool preserveDamageEvidence;

        internal static bool ignorePlayer;

        internal static bool ignoreAllies;

        internal static bool ignoreMissionCriticalPeds;

        internal static bool dismemberTorso;

        internal static bool pedPainSound;

        internal static bool enableHeadDismemberment;

        internal static bool enableLimbDismemberment;

        internal static bool enableBloodParticles;

        internal static bool enableBrainChunks;

        internal static int minChunks;

        internal static int maxChunks;

        internal static int maxActiveCaps;

        internal static int maxActiveChunks;

        internal static int chunkReleaseDelayMs;

        internal static int cleanupIntervalMs;

        private static void ApplyDefaults()
        {
            enableDismemberment = true;
            safeMode = false;
            debugLogging = true;
            logGoreEvents = false;
            nooseCompatibilityMode = true;
            immersiveCombatCompatibilityMode = true;
            onlyDismemberDeadPeds = true;
            allowMeleeDismemberLivingPeds = false;
            preserveDamageEvidence = true;
            ignorePlayer = true;
            ignoreAllies = true;
            ignoreMissionCriticalPeds = true;
            dismemberTorso = false;
            pedPainSound = true;
            enableHeadDismemberment = true;
            enableLimbDismemberment = true;
            enableBloodParticles = true;
            enableBrainChunks = true;
            minChunks = 3;
            maxChunks = 12;
            maxActiveCaps = 30;
            maxActiveChunks = 50;
            chunkReleaseDelayMs = 2000;
            cleanupIntervalMs = 1000;
        }

        private static bool ReadBool(string section, string key, bool fallback, Action<string> log)
        {
            try
            {
                return toml.GetValue(section, key, fallback);
            }
            catch (Exception ex)
            {
                log?.Invoke("Invalid config value " + section + "." + key + "; using " + fallback + ". " + ex.GetType().Name + ": " + ex.Message);
                return fallback;
            }
        }

        private static int ReadInt(string section, string key, int fallback, int min, int max, Action<string> log)
        {
            int value;

            try
            {
                value = toml.GetValue(section, key, fallback);
            }
            catch (Exception ex)
            {
                log?.Invoke("Invalid config value " + section + "." + key + "; using " + fallback + ". " + ex.GetType().Name + ": " + ex.Message);
                return fallback;
            }

            if (value < min)
            {
                log?.Invoke("Config value " + section + "." + key + "=" + value + " below minimum " + min + "; clamped.");
                return min;
            }

            if (value > max)
            {
                log?.Invoke("Config value " + section + "." + key + "=" + value + " above maximum " + max + "; clamped.");
                return max;
            }

            return value;
        }
    }
}
