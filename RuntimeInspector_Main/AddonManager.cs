using System.Collections.Generic;
using UnityEngine;

public class AddonManager
{
    public static List<Addon> loadedAddons;
    //public static List<Addon> activeCameraMods = new List<Addon>();
    public static GameObject persistantModRoot = new GameObject("PersistantModRoot");

    internal static bool InDesigner;
    internal static bool InLevel;

    private static GameObject gameCamera;
    internal static GameObject GameCameraObject
    {
        get
        {
            if (gameCamera == null)
            {
                if (InLevel)
                {
                    var c = Reflections.CameraManagerScript.InvokeMethod("get_MainCamera",Reflections.CameraManagerScript.InvokeMethod("get_Instance",null,null), null);
                    return gameCamera = ((Camera)c).gameObject;
                }
                else if (InDesigner)
                {
                    var c = Reflections.DesignerCameraController.InvokeMethod("get_Camera",
                                                                                   Reflections.Designer.InvokeMethod("get_CameraController",
                                                                                                                                            Reflections.Designer.InvokeMethod("get_Instance",null, null)
                                                                                                                                            , null)
                                                                                   ,null);
                    return gameCamera = ((Camera)c).gameObject;
                }
            }
            return gameCamera;
        }
        set => gameCamera = value;
    }

    public static void ActivateMods(AddonStart start)
    {
        Debug.Log($"[AddonLoader] Activating Addons in {start}");
        if (start == AddonStart.Designer)
        {
            InLevel = false;
            InDesigner = true;
            GameCameraObject = null;
        }
        else if (start == AddonStart.Level || start == AddonStart.Map)
        {
            InLevel = true;
            InDesigner = false;
            GameCameraObject = null;
        }
        foreach (var mod in loadedAddons)
            for (int i = mod.modStart.Count - 1; i >= 0; i--)
                if (mod.modStart[i] == start)
                {
                    switch (mod.modHost[i])
                    {
                        case AddonHost.Camera:
                            if (GameCameraObject)
                            {
                                if (!GameCameraObject.GetComponent(mod.modScripts[i]))
                                    GameCameraObject.AddComponent(mod.modScripts[i]);
                            Debug.Log($"[AddonLoader] Activating {mod.name} on GameCamera");
                            }
                            else
                                Debug.LogError("[AddonLoader] Failed to aquire Camera");

                            break;
                        case AddonHost.ActivePlane:
                            if (start != AddonStart.Designer)
                                Helper.ActivePlaneScript.AddComponent(mod.modScripts[i]);
                            Debug.Log($"[AddonLoader] Activating {mod.name} on ActivePlane");
                            break;
                        case AddonHost.GameScene:
                            new GameObject(mod.name).AddComponent(mod.modScripts[i]);
                            Debug.Log($"[AddonLoader] Activating {mod.name} in GameScene");
                            break;
                        case AddonHost.Persistant:
                            GameObject g = new GameObject(mod.name);
                            g.AddComponent(mod.modScripts[i]);
                            g.transform.SetParent(persistantModRoot.transform, false);
                            Debug.Log($"[AddonLoader] Activating {mod.name} as Persistant");
                            break;
                    }
                }
    }

}
