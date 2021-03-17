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

            Sheriff.sheriffKillCooldownValue = Math.Min(Sheriff.sheriffKillCooldownValue + 2.5f, 40);

            PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);

            GameOptionsMenuPatch.sheriffKillCooldown.Field_3 = Sheriff.sheriffKillCooldownValue;
            GameOptionsMenuPatch.sheriffKillCooldown.Value = Sheriff.sheriffKillCooldownValue;
            GameOptionsMenuPatch.sheriffKillCooldown.ValueText.Text = Sheriff.sheriffKillCooldownValue.ToString();

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

            Sheriff.sheriffKillCooldownValue = Math.Max(Sheriff.sheriffKillCooldownValue - 2.5f, 10);

            PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);

            GameOptionsMenuPatch.sheriffKillCooldown.Field_3 = Sheriff.sheriffKillCooldownValue;
            GameOptionsMenuPatch.sheriffKillCooldown.Value = Sheriff.sheriffKillCooldownValue;
            GameOptionsMenuPatch.sheriffKillCooldown.ValueText.Text = Sheriff.sheriffKillCooldownValue.ToString();

            return false;
        }
    }
}