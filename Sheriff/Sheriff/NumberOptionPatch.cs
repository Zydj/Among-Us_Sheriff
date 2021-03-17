using HarmonyLib;
using System;

namespace Sheriff
{
    [HarmonyPatch]
    public static class NumberOptionPatch
    {
        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
        public static bool Prefix(NumberOption __instance)
        {
            if (__instance.TitleText.Text != "Sheriff Kill Cooldown")
            {
                return true;
            }

            Sheriff.sheriffKillCooldown = Math.Min(Sheriff.sheriffKillCooldown + 2.5f, 40);

            PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);

            GameOptionsMenuPatch.sheriffKillCooldown.Field_3 = Sheriff.sheriffKillCooldown;
            GameOptionsMenuPatch.sheriffKillCooldown.Value = Sheriff.sheriffKillCooldown;
            GameOptionsMenuPatch.sheriffKillCooldown.ValueText.Text = Sheriff.sheriffKillCooldown.ToString();
            
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
        public static bool Prefix1(NumberOption __instance)
        {
            if (__instance.TitleText.Text != "Sheriff Kill Cooldown")
            {
                return true;
            }

            Sheriff.sheriffKillCooldown = Math.Max(Sheriff.sheriffKillCooldown - 2.5f, 10);

            PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);

            GameOptionsMenuPatch.sheriffKillCooldown.Field_3 = Sheriff.sheriffKillCooldown;
            GameOptionsMenuPatch.sheriffKillCooldown.Value = Sheriff.sheriffKillCooldown;
            GameOptionsMenuPatch.sheriffKillCooldown.ValueText.Text = Sheriff.sheriffKillCooldown.ToString();

            return false;
        }
    }
}
