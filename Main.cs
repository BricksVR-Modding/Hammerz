using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
namespace Ham
{
    public static class BuildInfo
    {
        public const string Name = "Hammerz"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "A mod to reimplement hanmmers.";
        public const string Author = "ATXLTheAxolotl#2134 | https://discord.com/users/566770844286844953"; // Author of the Mod.  (MUST BE SET)
        public const string Company = "BricksVR Modding"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.1.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = "https://github.com/BricksVR-Modding/Hammerz/releases/"; // Download Link for the Mod.  (Set as null if none)
    }

    public class HamMod : MelonMod
    {
        UnityAction spawnHammer;
        GameObject hammerOBJ;
        AssetBundle bundle;
        GameObject hammer;
        GameObject icon;
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            MelonLogger.Msg(Application.streamingAssetsPath);
            if(Application.platform == RuntimePlatform.Android) bundle = AssetBundle.LoadFromMemory(BVRhammers.Assets.hammerEmojiAndroid);
            else bundle = AssetBundle.LoadFromMemory(BVRhammers.Assets.hammerEmoji);

            icon = bundle.LoadAllAssets()[0].Cast<GameObject>();
            var allOBJ = Resources.FindObjectsOfTypeAll<GameObject>();

            for (var i = 0; i < allOBJ.Length; i++)
            {
                if (allOBJ[i].name == "Background" && allOBJ[i].GetComponent<RectTransform>().parent) if (allOBJ[i].GetComponent<RectTransform>().parent.name == "ColorPicker")
                    {
                        var rt = allOBJ[i].GetComponent<RectTransform>();
                        rt.localScale = new Vector3(1.5f, 1, 1);
                        rt.localPosition = new Vector3(0.5f, -0.134f, 0);
                        //-1.96 -0.134 0
                        var ccp = GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker").GetComponent<CollapsibleColorPicker>();
                        ccp._panelRightOpen = -0.45f;
                        ccp.panelRightClosed = 4.1f;
                        var title = GameObject.Instantiate(GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker/Title"));
                        title.name = "HamText";

                        title.GetComponent<TMPro.TextMeshProUGUI>().text = "Hammer";
                        title.GetComponent<RectTransform>().SetParent(GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker").GetComponent<RectTransform>());
                        title.GetComponent<RectTransform>().SetAsFirstSibling();
                        GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker/Title").GetComponent<RectTransform>().SetAsFirstSibling();
                        GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker/Background").GetComponent<RectTransform>().SetAsFirstSibling();
                        title.GetComponent<RectTransform>().localPosition = new Vector3(2.3f, 2.364f, 0);
                        title.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
                        title.GetComponent<RectTransform>().localScale = new Vector3(4, 4, 4);

                        var hammerButton = GameObject.Instantiate(GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker/CollapseButton"));
                        hammerButton.name = "HammerButton";
                        hammerButton.GetComponent<RectTransform>().SetParent(GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker").GetComponent<RectTransform>());
                        hammerButton.GetComponent<RectTransform>().localPosition = new Vector3(2.35f, 0, 0);
                        hammerButton.GetComponent<RectTransform>().localScale = new Vector3(4, 4, 4);
                        hammerButton.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
                        GameObject.Destroy(hammerButton.GetComponent<UnityEngine.UI.Button>());
                        MelonCoroutines.Start(delayedFixButton(hammerButton));

                        hammer = hammerButton;
                    }
                if (allOBJ[i].name == "Hammer")
                {
                    hammerOBJ = allOBJ[i];
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(Hammer), "OnCollisionEnter")]
        class Patch
        {
            public static bool Prefix(Collision mainCollision, Hammer __instance)
            {
                if (!mainCollision.gameObject.name.Contains("Placed")) return false;
                if (mainCollision.transform.parent) return false;
                GameObject brick = BrickSwapper.SwapToRealBrick(mainCollision.gameObject);
                if (brick == null) return false;

                brick.GetComponent<RealtimeTransform>().RequestOwnership();

                Rigidbody rb = brick.GetComponent<Rigidbody>();

                rb.isKinematic = false;

                float radius = Mathf.Max(3f, 1 * mainCollision.relativeVelocity.magnitude);
                brick.GetComponent<Rigidbody>().AddExplosionForce(4 * mainCollision.relativeVelocity.magnitude, __instance.transform.position, radius, 1f, ForceMode.Impulse);
                return false;
            }
        }

        [HarmonyPatch(typeof(CollapsibleColorPicker), "Animation")]
        class Animation
        {
            public static void Prefix(bool closing)
            {
                var e = GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker/Background");
                var title = GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker/HamText");
                var button = GameObject.Find("BrickPickerMenu/MenuContents/Wrapper/ColorPicker/HammerButton");
                if (closing)
                {
                    e.GetComponent<RectTransform>().localPosition = new Vector3(-1.96f, -0.134f, 0);
                    title.SetActive(false);
                    button.SetActive(false);
                }

                else
                {
                    e.GetComponent<RectTransform>().localPosition = new Vector3(0.6f, -0.134f, 0);
                    title.SetActive(true);
                    button.SetActive(true);
                }

            }
        }
        public void moveHammer()
        {
            hammerOBJ.SetActive(true);
            hammerOBJ.transform.parent = null;
            GameObject.Destroy(hammerOBJ.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>());
            GameObject.Destroy(hammerOBJ.GetComponent<Rigidbody>());

            hammerOBJ.transform.position = hammer.GetComponent<RectTransform>().position;
            MelonCoroutines.Start(delayedAddRigid(hammerOBJ));
        }
        public System.Collections.IEnumerator delayedAddRigid(GameObject ham)
        {
            yield return new WaitForSeconds(0.5f);
            ham.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
        }
        public System.Collections.IEnumerator delayedFixButton(GameObject ham)
        {
            yield return new WaitForSeconds(0.5f);
            spawnHammer = (UnityAction)(System.Action)moveHammer;
            var btn = ham.AddComponent<UnityEngine.UI.Button>();
            ham.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            ham.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(spawnHammer);
            GameObject.Destroy(ham.GetComponent<RectTransform>().GetChild(0).GetChild(0).gameObject);
            GameObject.Destroy(ham.GetComponent<RectTransform>().GetChild(0).GetChild(1).gameObject);
            ham.GetComponent<RectTransform>().GetChild(0).gameObject.AddComponent<SpriteRenderer>().sprite = icon.GetComponent<SpriteRenderer>().sprite;
            ham.GetComponent<RectTransform>().GetChild(0).localScale = new Vector3(0.1f, 0.1f, 0.1f);
            ham.GetComponent<TouchButton>()._button = btn;
            ham.GetComponent<RectTransform>().SetAsFirstSibling();
        }
    }
}