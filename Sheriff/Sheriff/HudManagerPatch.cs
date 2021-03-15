using HarmonyLib;
using InnerNet;

namespace Sheriff
{
    [HarmonyPatch]
    public static class HudManagerPatch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        static void Postfix(HudManager __instance)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            if (AmongUsClient.Instance.GameState != InnerNetClient.Nested_0.Started)
                return;

            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            if (PlayerController.LocalPlayer.hasComponent("Sheriff"))
            {
                string currentTasks = __instance.TaskText.Text;

                currentTasks = currentTasks.Replace("[FFB400FF]Kill the imposter.\n[FFFFFFFF]", "");
                __instance.TaskText.Text = "[FFB400FF]Kill the imposter.\n[FFFFFFFF]" + currentTasks;
            }
        }
    }
}
