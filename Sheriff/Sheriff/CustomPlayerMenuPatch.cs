using System.Linq;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace Sheriff
{
    [HarmonyPatch]
    public class CustomPlayerMenuPatch
    {
        public static void deleteOptions(bool destroy)
        { 
            if (GameOptionsMenuPatch.showSheriffOption != null && GameOptionsMenuPatch.sheriffKillCooldown != null)
            {
                GameOptionsMenuPatch.showSheriffOption.gameObject.SetActive(false);
                GameOptionsMenuPatch.sheriffKillCooldown.gameObject.SetActive(false);

                if (destroy)
                {
                    GameObject.Destroy(GameOptionsMenuPatch.showSheriffOption);
                    GameObject.Destroy(GameOptionsMenuPatch.sheriffKillCooldown);

                    GameOptionsMenuPatch.showSheriffOption = null;
                   GameOptionsMenuPatch.sheriffKillCooldown = null;
                }
            }
        }

        public static void AddOptions()
        {
            if (Sheriff.debug)
            {
                Sheriff.log.LogMessage("Adding options");
            }

            if (GameOptionsMenuPatch.showSheriffOption == null || GameOptionsMenuPatch.sheriffKillCooldown == null)
            {
                ToggleOption showAnonymousVotes = GameObject.FindObjectsOfType<ToggleOption>().ToList().Where(x => x.TitleText.Text == "Anonymous Votes").First();
                GameOptionsMenuPatch.showSheriffOption = GameObject.Instantiate(showAnonymousVotes);                

                NumberOption killCooldown = GameObject.FindObjectsOfType<NumberOption>().ToList().Where(x => x.TitleText.Text == "Kill Cooldown").First();
                GameOptionsMenuPatch.sheriffKillCooldown = GameObject.Instantiate(killCooldown);            

                OptionBehaviour[] options = new OptionBehaviour[GameOptionsMenuPatch.instance.Children.Count + 2];
                GameOptionsMenuPatch.instance.Children.ToArray().CopyTo(options, 0);
                options[options.Length - 2] = GameOptionsMenuPatch.showSheriffOption;
                options[options.Length - 1] = GameOptionsMenuPatch.sheriffKillCooldown;
                GameOptionsMenuPatch.instance.Children = new Il2CppReferenceArray<OptionBehaviour>(options);                
            }
            else
            {
                GameOptionsMenuPatch.showSheriffOption.gameObject.SetActive(true);
                GameOptionsMenuPatch.sheriffKillCooldown.gameObject.SetActive(true);
            }

            GameOptionsMenuPatch.showSheriffOption.TitleText.Text = "Sheriff Role";
            GameOptionsMenuPatch.showSheriffOption.oldValue = Sheriff.sheriffEnabled;
            GameOptionsMenuPatch.showSheriffOption.CheckMark.enabled = Sheriff.sheriffEnabled;

            GameOptionsMenuPatch.sheriffKillCooldown.TitleText.Text = "Sheriff Kill Cooldown";
            GameOptionsMenuPatch.sheriffKillCooldown.Value = Sheriff.sheriffKillCooldown;
            GameOptionsMenuPatch.sheriffKillCooldown.ValueText.Text = Sheriff.sheriffKillCooldown.ToString();            
        }
        
        [HarmonyPatch(typeof(CustomPlayerMenu), nameof(CustomPlayerMenu.Close))]
        public static void Postfix()
        {
            deleteOptions(true);
        }
      
        [HarmonyPatch(typeof(CustomPlayerMenu), nameof(CustomPlayerMenu.OpenTab))]
        public static void Prefix(GameObject CCAHNLMBCOD)
        {
            if (CCAHNLMBCOD.name == "GameGroup" && GameOptionsMenuPatch.instance != null)
            {
                AddOptions();
            }
            else
            {
                deleteOptions(false);
            }
        }
    }
}
