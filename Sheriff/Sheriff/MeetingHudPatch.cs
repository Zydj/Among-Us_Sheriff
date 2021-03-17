using HarmonyLib;

namespace Sheriff
{
    [HarmonyPatch]
    public static class MeetingHudPatch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static void Postfix(MeetingHud __instance)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            PlayerControl sheriff = PlayerController.getPlayerControlByRole("Sheriff");
            if (sheriff == null)
            {
                return;
            }

            foreach (PlayerVoteArea playerArea in __instance.playerStates)
            {
                if (sheriff.nameText.Text.Equals(playerArea.NameText.Text) && PlayerControl.LocalPlayer.PlayerId == sheriff.PlayerId)
                {
                    playerArea.NameText.Color = Sheriff.sheriffColor;
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        public static void Postfix()
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            Sheriff.sheriffKillCooldown = Sheriff.sheriffKillCooldownValue;
        }
    }
}
