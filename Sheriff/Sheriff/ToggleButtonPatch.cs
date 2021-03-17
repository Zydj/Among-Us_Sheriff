using HarmonyLib;

namespace Sheriff
{
    [HarmonyPatch]
    public static class ToggleButtonPatch
    {
        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        public static bool Prefix(ToggleOption __instance)
        {
            if (__instance.TitleText.Text == "Sheriff Role")
            {
                Sheriff.sheriffEnabled = !Sheriff.sheriffEnabled;
                PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);

                __instance.oldValue = Sheriff.sheriffEnabled;
                __instance.CheckMark.enabled = Sheriff.sheriffEnabled;

                return false;
            }
            else if (__instance.TitleText.Text == "Classic Sheriff")
            {
                Sheriff.classicSheriff = !Sheriff.classicSheriff;
                PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);

                __instance.oldValue = Sheriff.classicSheriff;
                __instance.CheckMark.enabled = Sheriff.classicSheriff;

                return false;
            }

            return true;
        }
    }
}