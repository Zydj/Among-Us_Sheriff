using HarmonyLib;
using Hazel;

namespace Sheriff
{
    [HarmonyPatch]
    public static class KillButtonManagerPatch
    {
        [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
        public static bool Prefix()
        {
            if (!Sheriff.sheriffEnabled)
            {
                return true;
            }

            if (!PlayerController.getLocalPlayer().hasComponent("Sheriff"))
            {
                return true;
            }

            Player sheriff = PlayerController.getPlayerByRole("Sheriff");

            if (HudManagerPatch.killTimer == 0)
            {
                if (sheriff.playerdata.CanMove)
                {
                    if (HudManagerPatch.nearest == null)
                    {
                        return true;
                    }

                    if (Sheriff.debug)
                    {
                        Sheriff.log.LogMessage("Target acquired");
                    }                    

                    if (Sheriff.debug)
                    {
                        Sheriff.log.LogMessage("Sending sheriff kill");
                        Sheriff.log.LogMessage(sheriff.playerdata.nameText.Text + " killed " + HudManagerPatch.nearest.nameText.Text);
                    }

                    MessageWriter writer;

                    writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SheriffKill, Hazel.SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);                    
                    writer.Write(HudManagerPatch.nearest.PlayerId);                   
                    writer.EndMessage();                   
                    PlayerControl.LocalPlayer.MurderPlayer(HudManagerPatch.nearest);

                    if (!HudManagerPatch.nearest.Data.LGEGJEHCFOG)
                    {
                        writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SheriffKill, Hazel.SendOption.Reliable);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.EndMessage();
                        PlayerControl.LocalPlayer.MurderPlayer(PlayerControl.LocalPlayer);
                    }

                    PlayerControl.LocalPlayer.SetKillTimer(HudManagerPatch.killCooldown);
                    HudManagerPatch.killTimer = HudManagerPatch.killCooldown;
                }
            }
            return false;
        }
    }
}
