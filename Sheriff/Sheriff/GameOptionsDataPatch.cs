using HarmonyLib;

namespace Sheriff
{
    [HarmonyPatch]
    public class GameOptionsDataPatch
    {
        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_5))]
        public static void Postfix(ref string __result)
        {
            if (Sheriff.sheriffEnabled)
            {
                __result += "Show Sheriff: On\n";
            }
            else
            {
                __result += "Show Sheriff: Off\n";
            }

            __result += "Sheriff Kill Cooldown: " + Sheriff.sheriffKillCooldownValue.ToString() + "s\n";

            if (Sheriff.classicSheriff)
            {
                __result += "Classic Sheriff: On\n";
            }
            else
            {
                __result += "Classic Sheriff: Off\n";
            }
        }
    }
}