using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using Reactor;
using UnityEngine;

namespace Sheriff
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class Sheriff : BasePlugin
    {
        public static bool debug = false;

        public const string Id = "org.bepinex.plugins.Sheriff";
        public static BepInEx.Logging.ManualLogSource log;

        public static bool sheriffEnabled = true;
        public static bool introDone = false;
        public static bool classicSheriff = true;

        public static float sheriffKillCooldown = 25f;
        public static float sheriffKillCooldownValue = sheriffKillCooldown;

        public static PlayerControl localPlayer = null;
        public static List<PlayerControl> localPlayers = new List<PlayerControl>();

        public static Color sheriffColor = new Color(1, (float)(180.0 / 255.0), 0);

        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; private set; }

        public override void Load()
        {
            log = Log;
            log.LogMessage("Sheriff Mod Loaded");

            Harmony.PatchAll();
        }
    }
}