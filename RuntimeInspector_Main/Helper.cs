using UnityEngine;

public static class Helper
{
    public static Vector3 FloatingOriginNew;
    public static Vector3 FloatingOriginOld;
    private static GameObject activePlane;
    private static GameObject mainCockpitObject;
    public static GameObject MainCockpitObject
    {
        get
        {
            if (mainCockpitObject == null)
            {
                //var g = Reflections.get_PlayerControlledAircraft.Invoke(Reflections.get_CurrentLevel.Invoke(null, null), null);
                var g = Reflections.LevelBase.InvokeMethod("get_PlayerControlledAircraft", Reflections.LevelBase.InvokeMethod("get_CurrentLevel", null, null), null);
                return mainCockpitObject = (Reflections.AircraftScript.InvokeMethod("get_MainCockpit", g, null) as MonoBehaviour).gameObject;
            }
            return mainCockpitObject;
        }
    }
    public static GameObject ActivePlaneScript
    {
        get
        {
            if (activePlane == null)
            {
                //var g = Reflections.get_PlayerControlledAircraft.Invoke(Reflections.get_CurrentLevel.Invoke(null, null), null);
                var g = Reflections.LevelBase.InvokeMethod("get_PlayerControlledAircraft", Reflections.LevelBase.InvokeMethod("get_CurrentLevel", null, null), null);
                return activePlane = ((MonoBehaviour)g).gameObject;
            }
            return activePlane;
        }
    }
    public static GameObject GameCameraObject => AddonManager.GameCameraObject;
    public static Camera mainCamera;
    public static Camera MainCamera
    {
        get
        {
            if (!mainCamera)
            {
                if (InLevel)
                {
                    var c = Reflections.CameraManagerScript.InvokeMethod("get_MainCamera", Reflections.CameraManagerScript.InvokeMethod("get_Instance", null, null), null);
                    mainCamera = (Camera)c;
                }
                else if (InDesigner)
                {
                    var c = Reflections.DesignerCameraController.InvokeMethod("get_Camera", Reflections.Designer.InvokeMethod("get_CameraController", Reflections.Designer.InvokeMethod("get_Instance", null, null), null), null);
                    mainCamera = (Camera)c;
                }
            }
            return mainCamera;
        }
    }
    public static Camera planeCamera;
    public static Camera PlaneCamera
    {
        get
        {
            if (InDesigner) return MainCamera;
            if (!planeCamera)
            {
                var c = Reflections.CameraManagerScript.GetField("_planeCamera", Reflections.CameraManagerScript.InvokeMethod("get_Instance", null, null));
                return planeCamera = (Camera)c;
            }
            return planeCamera;
        }
    }
    static object cameraManagerScript;
    public static object CameraManagerScript
    {
        get
        {
            if (cameraManagerScript == null)
            {
                cameraManagerScript = Reflections.CameraManagerScript.InvokeMethod("get_Instance", null, null);
            }
            return cameraManagerScript;
        }
    }
    public static bool InLevel => AddonManager.InLevel;
    public static bool InDesigner => AddonManager.InDesigner;
    public delegate void OnEnterLevelHandler();
    public static event OnEnterLevelHandler OnEnterLevel;
    public delegate void OnEnterDesignerHandler();
    public static event OnEnterDesignerHandler OnEnterDesigner;
    public delegate void OnExitLevelHandler();
    public static event OnExitLevelHandler OnExitLevel;
    public delegate void OnExitDesignerHandler();
    public static event OnExitDesignerHandler OnExitDesigner;
    public delegate void OnFloatOriginChangeHandler(Vector3 oldOrigin, Vector3 newOrigin);
    public static event OnFloatOriginChangeHandler OnFloatOriginChange;
    internal static void InvokeOnExitLevel()
    {
        if (OnExitLevel != null) OnExitLevel.Invoke();
    }
    internal static void InvokeOnExitDesigner()
    {
        if (OnExitDesigner != null) OnExitDesigner.Invoke();
    }
    internal static void InvokeOnEnterLevel()
    {
        if (OnEnterLevel != null) OnEnterLevel.Invoke();
    }
    internal static void InvokeOnEnterDesigner()
    {
        if (OnEnterDesigner != null) OnEnterDesigner.Invoke();
    }
    internal static void InvokeOnFloatOriginChange()
    {
        if (OnFloatOriginChange != null) OnFloatOriginChange.Invoke(FloatingOriginOld, FloatingOriginNew);
    }
    public static void LevelCameraChanged()
    {
        Debug.Log("********LevelCameraChanged");
        AddonManager.GameCameraObject = null;
        AddonManager.ActivateMods(AddonStart.Level);
        Helper.InvokeOnEnterLevel();
    }


    public static float UnsignedMax(float a, float b)
    {
        var aa = a > 0 ? a : -a;
        var ab = b > 0 ? b : -b;
        return aa > ab ? a : b;
    }

    public static float AngleAroundAxis(Vector3 from, Vector3 to, Vector3 axis) => Vector3.SignedAngle(Vector3.ProjectOnPlane(from, axis), Vector3.ProjectOnPlane(to, axis), axis);

    public static Vector3 GetWorldCornersCenter(this RectTransform t)
    {
        Vector3[] v = new Vector3[4];
        t.GetWorldCorners(v);
        return Vector3.Lerp(v[0], v[2], .5f);

    }
}