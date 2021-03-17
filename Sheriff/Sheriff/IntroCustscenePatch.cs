using HarmonyLib;

namespace Sheriff
{
    [HarmonyPatch]
    public static class IntroCustscenePatch
    {
        /*[HarmonyPatch(typeof(IntroCutscene.Nested_0), nameof(IntroCutscene.Nested_0.MoveNext))]
        static void Prefix(IntroCutscene.Nested_0 __instance)
        {
            Sheriff.introDone = false;
        }*/
        
        [HarmonyPatch(typeof(IntroCutscene.Nested_0), nameof(IntroCutscene.Nested_0.MoveNext))]
        public static void Postfix(IntroCutscene.Nested_0 __instance)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            if (PlayerController.getLocalPlayer().hasComponent("Sheriff"))
            {
                __instance.__this.Title.Text = "Sheriff";
                __instance.__this.Title.Color = Sheriff.sheriffColor;
                __instance.__this.ImpostorText.Text = "Kill the [FF0000FF]Impostor";
                __instance.__this.BackgroundBar.material.color = Sheriff.sheriffColor;
            }            
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        public static void Postfix()
        {            
            Sheriff.introDone = true;
        }
    }
}
