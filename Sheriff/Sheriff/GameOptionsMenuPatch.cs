using HarmonyLib;
using UnityEngine;

namespace Sheriff
{
    [HarmonyPatch]
    public static class GameOptionsMenuPatch
    {
        public static ToggleOption showSheriffOption;
        public static NumberOption sheriffKillCooldown;
        public static ToggleOption showClassicSheriffOption;

        public static GameOptionsMenu instance;

        public static float defaultBounds;

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]        
        public static void Postfix(GameOptionsMenu __instance)
        {
            if (Sheriff.debug)
            {
                Sheriff.log.LogMessage("Starting");
            }

            defaultBounds = __instance.GetComponentInParent<Scroller>().YBounds.max;

            instance = __instance;
            CustomPlayerMenuPatch.AddOptions();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static void Postfix1(GameOptionsMenu __instance)
        {
            OptionBehaviour option = __instance.Children[__instance.Children.Count - 4];

            if (showSheriffOption != null && sheriffKillCooldown != null && showClassicSheriffOption != null)
            {
                showSheriffOption.transform.position = option.transform.position - new Vector3(0, 0.5f, 0);
                sheriffKillCooldown.transform.position = option.transform.position - new Vector3(0, 1f, 0);
                showClassicSheriffOption.transform.position = option.transform.position - new Vector3(0, 1.5f, 0);                

                __instance.GetComponentInParent<Scroller>().YBounds.max = defaultBounds + 1.5f;
            }
        }
    }
}
