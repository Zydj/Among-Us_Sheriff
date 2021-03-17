using HarmonyLib;
using UnityEngine;

namespace Sheriff
{
    [HarmonyPatch]
    public static class GameOptionsMenuPatch
    {
        public static ToggleOption showSheriffOption;
        public static NumberOption sheriffKillCooldown;
        public static GameOptionsMenu instance;

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public static void Postfix(GameOptionsMenu __instance)
        {
            if (Sheriff.debug)
            {
                Sheriff.log.LogMessage("Starting");
            }

            instance = __instance;
            CustomPlayerMenuPatch.AddOptions();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static void Postfix1(GameOptionsMenu __instance)
        {
            OptionBehaviour option = __instance.Children[__instance.Children.Count - 3];            

            if (showSheriffOption != null && sheriffKillCooldown != null)
            {
                showSheriffOption.transform.position = option.transform.position - new Vector3(0, 0.5f, 0);
                sheriffKillCooldown.transform.position = option.transform.position - new Vector3(0, 1f, 0);
            }
        }
    }
}
