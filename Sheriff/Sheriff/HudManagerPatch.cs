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
        public static float killTimer = 10f;
        public static float killCooldown = 10f;
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

                    if (PlayerControl.LocalPlayer.CanMove)
                    {
                        PlayerControl sheriff = PlayerControl.LocalPlayer;

                        killTimer = Math.Max(0, killTimer - Time.deltaTime);
                        if (killTimer <= 0)
                        {
                            killTimer = 0;
                        }

                        __instance.KillButton.SetCoolDown(killTimer, killCooldown);

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
            }
        }
    }
}
