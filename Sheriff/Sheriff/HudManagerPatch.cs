using HarmonyLib;
using InnerNet;
using System;
using UnityEngine;

namespace Sheriff
{

    [HarmonyPatch]
    public static class HudManagerPatch
    {
        public static HudManager HUD;
        public static KillButtonManager KillButton;
        public static float killTimer;
        public static PlayerControl nearest;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        static void Postfix(HudManager __instance)
        {
            if (!Sheriff.sheriffEnabled)
            {
                return;
            }

            if (AmongUsClient.Instance.GameState != InnerNetClient.Nested_0.Started)
                return;

            if (!Sheriff.introDone)
            {
                return;
            }

            if (PlayerController.LocalPlayer.hasComponent("Sheriff"))
            {
                string currentTasks = __instance.TaskText.Text;

                currentTasks = currentTasks.Replace("[FFB400FF]Kill the imposter.\n[FFFFFFFF]", "");
                __instance.TaskText.Text = "[FFB400FF]Kill the imposter.\n[FFFFFFFF]" + currentTasks;

                if (!PlayerControl.LocalPlayer.Data.AKOHOAJIHBE)
                {
                    __instance.KillButton.gameObject.SetActive(true);
                    __instance.KillButton.isActive = true;

                    if (PlayerControl.LocalPlayer.CanMove && Sheriff.introDone)
                    {
                        PlayerControl sheriff = PlayerControl.LocalPlayer;

                        Sheriff.sheriffKillCooldown = Math.Max(0, Sheriff.sheriffKillCooldown - Time.deltaTime);

                        if (Sheriff.sheriffKillCooldown <= 0)
                        {
                            Sheriff.sheriffKillCooldown = 0;
                        }

                        __instance.KillButton.SetCoolDown(Sheriff.sheriffKillCooldown, Sheriff.sheriffKillCooldownValue);

                        float range = GameOptionsData.KillDistances[Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)];

                        Vector2 sheriffPosition = PlayerControl.LocalPlayer.GetTruePosition();
                        Vector2 targetPosition;

                        nearest = null;

                        foreach (PlayerControl target in PlayerControl.AllPlayerControls)
                        {
                            if (target.CanMove && target.PlayerId != sheriff.PlayerId && !target.Data.AKOHOAJIHBE && !target.inVent)
                            {
                                targetPosition = target.GetTruePosition() - sheriffPosition;

                                float magnitude = targetPosition.magnitude;

                                if (magnitude <= range && !PhysicsHelpers.AnyNonTriggersBetween(sheriffPosition, targetPosition.normalized, magnitude, Constants.ShipAndObjectsMask))
                                {
                                    nearest = target;
                                    range = magnitude;
                                }
                            }
                        }

                        if (nearest != null)
                        {
                            __instance.KillButton.SetTarget(nearest);
                        }
                        if (Input.GetKeyInt(KeyCode.Q))
                        {
                            __instance.KillButton.PerformKill();
                        }
                    }
                }
                else if (PlayerControl.LocalPlayer.Data.AKOHOAJIHBE && !PlayerControl.LocalPlayer.Data.LGEGJEHCFOG)
                {
                    __instance.KillButton.gameObject.SetActive(false);
                    __instance.KillButton.isActive = false;
                }
            }
        }
    }
}