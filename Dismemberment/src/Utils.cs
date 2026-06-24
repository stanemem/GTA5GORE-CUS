using System;
using GTA;
using GTA.Math;
using GTA.Native;

namespace Dismemberment
{
    internal static class Utils
    {
        internal static bool IsValid(this Entity entity)
        {
            try
            {
                return entity != null && entity.Exists();
            }
            catch
            {
                return false;
            }
        }

        internal static bool IsValidPed(this Ped ped)
        {
            return IsValid((Entity)ped);
        }

        internal static bool IsValidProp(this Prop prop)
        {
            return IsValid((Entity)prop);
        }

        internal static bool ExcludedPeds(this Ped ped)
        {
            if (!ped.IsValidPed())
            {
                return true;
            }

            try
            {
                int pedType = Function.Call<int>(Hash.GET_PED_TYPE, ped);
                return pedType == 0 || pedType == 1 || pedType == 2 || pedType == 3 || pedType == 28;
            }
            catch
            {
                return true;
            }
        }

        internal static bool IsMissionEntity(this Entity entity)
        {
            if (!entity.IsValid())
            {
                return false;
            }

            try
            {
                return Function.Call<bool>(Hash.IS_ENTITY_A_MISSION_ENTITY, entity);
            }
            catch
            {
                return false;
            }
        }

        internal static bool IsAllyOfPlayer(this Ped ped)
        {
            if (!ped.IsValidPed())
            {
                return false;
            }

            Ped player = null;
            try
            {
                player = Game.Player.Character;
            }
            catch
            {
                return false;
            }

            if (!player.IsValidPed() || ped.Handle == player.Handle)
            {
                return true;
            }

            try
            {
                Relationship relationship = (Relationship)Function.Call<int>(Hash.GET_RELATIONSHIP_BETWEEN_PEDS, ped, player);
                return relationship == Relationship.Companion
                    || relationship == Relationship.Respect
                    || relationship == Relationship.Like;
            }
            catch
            {
                return false;
            }
        }

        internal static bool RequestPTFXLibrary(string lib, Action<string> log)
        {
            try
            {
                if (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, lib))
                {
                    Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, lib);
                }

                bool loaded = Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, lib);
                log?.Invoke("PTFX request " + lib + ": loaded=" + loaded + ".");
                return loaded;
            }
            catch (Exception ex)
            {
                log?.Invoke("PTFX request failed for " + lib + ". " + ex.GetType().Name + ": " + ex.Message);
                return false;
            }
        }

        internal static void RemovePTFXLibrary(string lib, Action<string> log)
        {
            try
            {
                if (Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, lib))
                {
                    Function.Call(Hash.REMOVE_NAMED_PTFX_ASSET, lib);
                    log?.Invoke("PTFX removed " + lib + ".");
                }
            }
            catch (Exception ex)
            {
                log?.Invoke("PTFX remove failed for " + lib + ". " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        internal static bool IsDLCInstalled(Action<string> log)
        {
            try
            {
                bool installed = Function.Call<bool>(Hash.IS_DLC_PRESENT, 0x83E0D0E0);
                log?.Invoke("Dismemberment DLC/rpf check: installed=" + installed + ".");
                return installed;
            }
            catch (Exception ex)
            {
                log?.Invoke("Dismemberment DLC/rpf check failed. " + ex.GetType().Name + ": " + ex.Message);
                return false;
            }
        }

        internal static Ped CloneMe(this Ped ped, Vector3 coords, float heading, Action<string> log)
        {
            if (!ped.IsValidPed())
            {
                return null;
            }

            try
            {
                Ped clone = Function.Call<Ped>(Hash.CLONE_PED, ped, heading, 0, 1);
                if (!clone.IsValidPed())
                {
                    log?.Invoke("Ped clone failed: clone was invalid.");
                    return null;
                }

                clone.Position = coords;
                clone.Heading = heading;
                return clone;
            }
            catch (Exception ex)
            {
                log?.Invoke("Ped clone failed. " + ex.GetType().Name + ": " + ex.Message);
                return null;
            }
        }

        internal static float GetPhysicsHeading(this Entity entity, Action<string> log)
        {
            if (!entity.IsValid())
            {
                return 0f;
            }

            try
            {
                return Function.Call<float>(Hash.GET_ENTITY_HEADING_FROM_EULERS, entity);
            }
            catch (Exception ex)
            {
                log?.Invoke("Physics heading lookup failed. " + ex.GetType().Name + ": " + ex.Message);
                return entity.Heading;
            }
        }
    }
}
