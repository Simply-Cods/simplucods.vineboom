using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace simplycods.vineboom
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInProcess("ULTRAKILL.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "simplycods.vineboom";
        public const string PLUGIN_NAME = "simplycods.vineboom";
        public const string PLUGIN_VERSION = "1.0.0";

        private AssetBundle bundle;
        public static AudioClip theboom;

        private void Awake()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("simplycods.vineboom.Resources.vineboom"))
            {
                bundle = AssetBundle.LoadFromStream(stream);
            }

            theboom = bundle.LoadAsset<AudioClip>("vine boom sound effect");

            new Harmony(PLUGIN_GUID).PatchAll();

            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(ItemIdentifier), "PickUp")]
        public static class SkullPunch
        {
            private static ItemType[] _skulls = new[] { ItemType.SkullBlue, ItemType.SkullRed, ItemType.SkullGreen };

            public static void Postfix(ItemIdentifier __instance)
            {

                if (_skulls.Contains(__instance.itemType))
                {
                    var skull = __instance.GetComponent<Skull>();
                    var audioSource = Traverse.Create(skull).Field("aud").GetValue<AudioSource>();
                    audioSource.clip = theboom;
                    audioSource.Play();
                }
            }
        }
    }
}
