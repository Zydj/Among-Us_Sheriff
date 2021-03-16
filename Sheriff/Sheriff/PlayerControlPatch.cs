using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using UnhollowerBaseLib;
using System.Linq;
using Jester;
using InnerNet;

namespace Sheriff
{

    [HarmonyPatch]
    public static class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        static void Postfix(byte ACCJCEHMKLN, MessageReader HFPCBBHJIPJ)
        {
            if (Sheriff.debug)
            {
                Sheriff.log.LogMessage("RPC is:" + ACCJCEHMKLN);
            }

            switch (ACCJCEHMKLN)
            {
                case (byte)CustomRPC.SetSheriff:
                    {
                        if (Sheriff.debug)
                        {
                            Sheriff.log.LogMessage("Setting Sheriff");
                        }

                        PlayerController.InitPlayers();
                        Player p = PlayerController.getPlayerById(HFPCBBHJIPJ.ReadByte());
                        p.components.Add("Sheriff");
                       
                        if (Sheriff.debug)
                        {
                            Sheriff.log.LogMessage("Sheriff is: " + p.playerdata.nameText.Text);
                        }
                        break;
                    }

                case (byte)CustomRPC.SetLocalPlayersSheriff:
                    {
                        if (Sheriff.debug)
                        {
                            Sheriff.log.LogMessage("Setting local players for sheriff");
                        }

                        Sheriff.localPlayers.Clear();
                        Sheriff.localPlayer = PlayerControl.LocalPlayer;

                        var localPlayerBytes = HFPCBBHJIPJ.ReadBytesAndSize();

                        foreach (var id in localPlayerBytes)
                        {
                            foreach (var player in PlayerControl.AllPlayerControls)
                            {
                                if (player.PlayerId == id)
                                {
                                    Sheriff.localPlayers.Add(player);
                                }
                            }
                        }
                        break;
                    }

                case (byte)CustomRPC.SheriffKill:
                    {
                        if (Sheriff.debug)
                        {
                            Sheriff.log.LogMessage("Setting sheriff kill");
                        }

                        Player killer = PlayerController.getPlayerById(HFPCBBHJIPJ.ReadByte());
                        Player target = PlayerController.getPlayerById(HFPCBBHJIPJ.ReadByte());

                        if (Sheriff.debug)
                        {
                            Sheriff.log.LogMessage("Setting sheriff kill");
                            Sheriff.log.LogMessage(killer.playerdata.nameText.Text + " killed " + target.playerdata.nameText.Text);
                        }

                        if (killer.hasComponent("Sheriff"))
                        {
                            killer.playerdata.MurderPlayer(target.playerdata);
                        }
                        break;
                    }
            }
        }

        public static List<Player> getCrewMates(Il2CppReferenceArray<GameData.Nested_1> infection)
        {
            List<Player> Crewmates = new List<Player>();
            foreach (Player player in PlayerController.players)
            {
                bool isInfected = false;
                foreach (GameData.Nested_1 infected in infection)
                {
                    if (player.playerdata.PlayerId == infected.IBJBIALCEKB.PlayerId)
                    {
                        isInfected = true;
                        break;
                    }
                }

                if (!isInfected)
                {
                    Crewmates.Add(player);
                }
            }
            return Crewmates;
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
        public static void Postfix(Il2CppReferenceArray<GameData.Nested_1> FMAOEJEHPAO)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            PlayerController.InitPlayers();

            List<Player> crewmates = getCrewMates(FMAOEJEHPAO);

            var sheriffId = new System.Random().Next(0, crewmates.Count);

            Player sheriff = crewmates[sheriffId];

            while (Jester.PlayerController.getPlayerById((byte)sheriff.PlayerId).hasComponent("Jester"))
            {
                if (Sheriff.debug)
                {
                    Sheriff.log.LogMessage("Player is already jester. Finding a new player");
                }

                sheriffId = new System.Random().Next(0, crewmates.Count);

                sheriff = crewmates[sheriffId];
            }

            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSheriff, Hazel.SendOption.Reliable);
            writer.Write(sheriff.playerdata.PlayerId);
            writer.EndMessage();
            sheriff.components.Add("Sheriff");            

            Sheriff.localPlayers.Clear();
            Sheriff.localPlayer = PlayerControl.LocalPlayer;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Sheriff.localPlayers.Add(player);
            }

            writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetLocalPlayersSheriff, Hazel.SendOption.Reliable);
            writer.WriteBytesAndSize(Sheriff.localPlayers.Select(player => player.PlayerId).ToArray());
            writer.EndMessage();
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static void Postfix(PlayerControl __instance)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            if (AmongUsClient.Instance.GameState != InnerNetClient.Nested_0.Started)
                return;

            if (__instance == null)
            {
                return;
            }

            if (!Sheriff.introDone)
            {
                return;
            }

            if (PlayerController.getLocalPlayer().hasComponent("Sheriff"))
            {
                PlayerControl.LocalPlayer.nameText.Color = Sheriff.sheriffColor;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public static void Prefix(PlayerControl __instance)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            if (__instance.PlayerId == PlayerController.getPlayerByRole("Sheriff").PlayerId)
            {
                __instance.Data.LGEGJEHCFOG = true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public static void Postfix(PlayerControl __instance, PlayerControl PAIBDFDMIGK)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            if (__instance.PlayerId == PlayerController.getPlayerByRole("Sheriff").PlayerId)
            {
                __instance.Data.LGEGJEHCFOG = false;
            }
        }
    }
}
