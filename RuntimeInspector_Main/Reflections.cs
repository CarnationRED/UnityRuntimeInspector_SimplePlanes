using System;
using System.Reflection;
using System.Linq;
using Jundroo.SimplePlanes.ModTools.Events;
using UnityEngine;
using Boo.Lang;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;

public class Reflections
{
    public static ReflectedType AircraftScript;
    public static ReflectedType CameraManagerScript;
    public static ReflectedType LevelBase;
    public static ReflectedType Designer;
    public static ReflectedType DesignerCameraController;
    public static ReflectedType LevelCameraController;
    public static ReflectedType GameWorld;
    #region legacy
    /*  //public static MethodInfo get_CurrentLevel;
//public static MethodInfo get_PlayerControlledAircraft;
//public static MethodInfo get_CameraManagerScript_Instance;
//public static MethodInfo get_CameraManagerScript_MainCamera;
//public static FieldInfo get_CameraManagerScript_planeCamera;
//public static FieldInfo get_CameraManagerScript_cameras;
//public static MethodInfo get_Designer_Instance;
//public static MethodInfo get_Designer_CameraController;
//public static MethodInfo get_CameraController_Camera;
//private static MethodInfo get_GameWorld_Instance;
//public static MethodInfo get_GameWorld_FloatingOriginOffset;*/
    #endregion
    public static Assembly gameMainAssembly = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .Where(x => x.GetName().Name.ToLower() == "assembly-csharp")
                                    .FirstOrDefault();

    static Reflections()
    {
        AircraftScript = new ReflectedType(gameMainAssembly, "Assets.Scripts.Parts.AircraftScript");
        AircraftScript.RegisterMethod("get_MainCockpit");

        LevelBase = new ReflectedType(gameMainAssembly, "Assets.Scripts.Levels.LevelBase");
        LevelBase.RegisterMethod("get_CurrentLevel");
        LevelBase.RegisterMethod("get_PlayerControlledAircraft");

        CameraManagerScript = new ReflectedType(gameMainAssembly, "Assets.Scripts.Levels.Camera.CameraManagerScript");
        CameraManagerScript.RegisterMethod("get_Instance");
        CameraManagerScript.RegisterMethod("get_MainCamera");
        CameraManagerScript.RegisterField("_currentCameraController", BindingFlags.NonPublic | BindingFlags.Instance);
        CameraManagerScript.RegisterField("_cameras", BindingFlags.NonPublic | BindingFlags.Instance);
        CameraManagerScript.RegisterField("_planeCamera", BindingFlags.NonPublic | BindingFlags.Instance);

        Designer = new ReflectedType(gameMainAssembly, "Assets.Game.Design.Designer");
        Designer.RegisterMethod("get_Instance");
        Designer.RegisterMethod("get_CameraController");
        Designer.RegisterField("_cameras", BindingFlags.NonPublic | BindingFlags.Instance);
        Designer.RegisterField("_planeCamera", BindingFlags.NonPublic | BindingFlags.Instance);

        DesignerCameraController = new ReflectedType(gameMainAssembly, "Assets.Game.Design.CameraController");
        DesignerCameraController.RegisterMethod("get_Camera");
        
        LevelCameraController = new ReflectedType(gameMainAssembly, "Assets.Scripts.Levels.Camera.CameraController");
        LevelCameraController.RegisterMethod("get_IsActive");
        LevelCameraController.RegisterMethod("set_IsActive");
        LevelCameraController.RegisterMethod("get_IsSelected");
        LevelCameraController.RegisterMethod("set_IsSelected");

        GameWorld = new ReflectedType(gameMainAssembly, "Assets.Game.GameWorld");
        GameWorld.RegisterMethod("get_Instance");
        GameWorld.RegisterMethod("get_FloatingOriginOffset");
        Helper.FloatingOriginOld = Helper.FloatingOriginNew = (Vector3)(GameWorld.InvokeMethod("get_FloatingOriginOffset", GameWorld.InvokeMethod("get_Instance", null, null), null));
        GameWorld.type
            .GetEvent("FloatingOriginChanged")
            .AddEventHandler(
                GameWorld.InvokeMethod("get_Instance", null, null),
                new EventHandler<FloatingOriginChangedEventArgs>(
                    (object sender, FloatingOriginChangedEventArgs e) =>
                    {
                        Helper.FloatingOriginNew = e.NewFloatingOriginOffset;
                        Helper.FloatingOriginOld = e.OldFloatingOriginOffset;
                        Helper.InvokeOnFloatOriginChange();
                    }));



        #region legacy
        /*Type lvlBase = gameAss.GetType("Assets.Scripts.Levels.LevelBase");
        get_CurrentLevel = lvlBase.GetMethod("get_CurrentLevel");
        get_PlayerControlledAircraft = lvlBase.GetMethod("get_PlayerControlledAircraft");


        Type camMgrSpt = gameAss.GetType("Assets.Scripts.Levels.Camera.CameraManagerScript");
        get_CameraManagerScript_Instance = camMgrSpt.GetMethod("get_Instance");
        get_CameraManagerScript_MainCamera = camMgrSpt.GetMethod("get_MainCamera");
        get_CameraManagerScript_cameras = camMgrSpt.GetField("_cameras", BindingFlags.NonPublic | BindingFlags.Instance);
        get_CameraManagerScript_planeCamera = camMgrSpt.GetField("_planeCamera", BindingFlags.NonPublic | BindingFlags.Instance);
        Type designer = gameAss.GetType("Assets.Game.Design.Designer");
        get_Designer_Instance = designer.GetMethod("get_Instance");
        get_Designer_CameraController = designer.GetMethod("get_CameraController");
        get_CameraController_Camera = gameAss.GetType("Assets.Game.Design.CameraController").GetMethod("get_Camera");
        Type gameworld = gameAss.GetType("Assets.Game.GameWorld");
        get_GameWorld_Instance = gameworld.GetMethod("get_Instance");
        get_GameWorld_FloatingOriginOffset = gameworld.GetMethod("get_FloatingOriginOffset");
        Helper.FloatingOriginNew = (Vector3)get_GameWorld_FloatingOriginOffset.Invoke(get_GameWorld_Instance.Invoke(null, null), null);
        gameworld.GetEvent("FloatingOriginChanged").AddEventHandler(get_GameWorld_Instance.Invoke(null, null), new EventHandler<FloatingOriginChangedEventArgs>(OnFloatingOriginChanged));   */
        #endregion
    }
    #region legacy
    /* static void OnFloatingOriginChanged(object sender, FloatingOriginChangedEventArgs e)
     {
         Helper.FloatingOriginNew = e.NewFloatingOriginOffset;
         Debug.Log("FloatingOriginNew: " + Helper.FloatingOriginNew);
     } */
    #endregion
    public struct ReflectedType
    {
        // string name;
        public Type type;
        public Dictionary<string, MethodInfo> methods;
        public Dictionary<string, FieldInfo> fields;
        public object InvokeMethod(string name, object obj, object[] args) => methods[name]?.Invoke(obj, args);
        public object GetField(string name, object obj) => fields[name]?.GetValue(obj);

        public void SetField(string name, object obj, object value)
        {
            FieldInfo f = fields[name];
            if (f != null) f.SetValue(obj, value);
        }

        public void RegisterMethod(string name)
        {
            var m = type.GetMethod(name);
            if (m != null)
                methods.Add(name, m);
        }
        public void RegisterField(string name, BindingFlags binding)
        {
            var f = type.GetField(name, binding);
            if (f != null)
                fields.Add(name, f);
        }
        public ReflectedType(Assembly assem, string name)
        {
            type = assem.GetType(name, true);
            methods = new Dictionary<string, MethodInfo>();
            fields = new Dictionary<string, FieldInfo>();
        }
    }
}
