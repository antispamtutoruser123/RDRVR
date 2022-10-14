using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;


namespace RDRVR
{


    [HarmonyPatch]
    public class CameraPatches
    {
        static bool ingame = false;
        public static Vector3 startpos,startrot,offset;
        public static RenderTexture rt;
        public static GameObject newUI;
        public static GameObject worldcam;

        public static GameObject DummyCamera, VRCamera,VRPlayer;

        private static readonly string[] canvasesToIgnore =
    {
        "com.sinai.unityexplorer_Root", // UnityExplorer.
        "com.sinai.unityexplorer.MouseInspector_Root", // UnityExplorer.
        "com.sinai.universelib.resizeCursor_Root",
        "IntroCanvas"
    };
        private static readonly string[] canvasesToWorld =
    {
        "OverlayCanvas"
    };

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraManager), "LateUpdate")]
        private static void recenter(CameraManager __instance)
        {
            if(VRCamera)
            Logs.WriteInfo($"CAMERA:  {VRCamera.transform.localPosition} {__instance.tag}");
            if (Input.GetKeyDown("joystick button 1"))
            {
                MyCameraManager.Recenter();
            }
        }
            [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraManager), "Awake")]
        private static void MakeCamera(CameraManager __instance)
        {
            if (!DummyCamera)
            {
                Logs.WriteInfo($"CREATING DUMMY CAMERA:  {__instance.name} {__instance.tag}");

                VRPlayer = __instance.gameObject;
                VRCamera = __instance.transform.Find("interior").gameObject;

                DummyCamera = new GameObject("DummyCamera");
                DummyCamera.transform.localPosition = new Vector3(-.06f, -1.75f, .38f);
                DummyCamera.transform.parent = __instance.transform;
                //   VRCamera.transform.parent = DummyCamera.transform;


                __instance.transform.Find("main").parent = DummyCamera.transform;
                __instance.transform.Find("interior").parent = DummyCamera.transform;
                

                startpos = new Vector3(.05f, 1.72f, -.5f);
                startrot = new Vector3(0, 0, 0);
               

            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void MoveIntroCanvas(CanvasScaler __instance)
        {
            if (IsCanvasToIgnore(__instance.name)) return;

            if(__instance.name == "gameCanvas")
            {
                GameObject gameui = __instance.transform.GetChild(0).gameObject;
                gameui.transform.localPosition = new Vector3(-380f, -90f, -50f);
                gameui.transform.localScale = new Vector3(.5f, .5f, 1f);
            }
            Logs.WriteInfo($"Hiding Canvas:  {__instance.name}");
            var canvas = __instance.GetComponent<Canvas>();

            
        }
     


 


        private static bool IsCanvasToIgnore(string canvasName)
        {
            foreach (var s in canvasesToIgnore)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

        private static bool IsCanvasToWorld(string canvasName)
        {
            foreach (var s in canvasesToWorld)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

    }
}

