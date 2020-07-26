#define RuntimeInspector
using Jundroo.SimplePlanes.ModTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
[RequireComponent(typeof(PersistentObject))]
public class AddonLoaderScript : MonoBehaviour
{
    public static string GameDirectory = string.Empty;
    public static string AddonDirectory = string.Empty;
#if RuntimeInspector
    public const string AddonDirectoryName = @"Addons\RuntimeInspector";
    public const string DllName = "RuntimeInspector.dll";
    public const string BundleName = "runtimeinspector_main.bundle";
    public const string PickerBundleName = "runtimeinspector_colorpicker.bundle";
    public const string Licence1Name = "License-Runtime Inspector(The SimplePlanes Mod).txt";
    public const string Licence2Name = "License-Runtime Inspector(Unity Assets).txt";
    public const string Licence3Name = "License-Unity_ColorWheel.txt";
#else
    public const string AddonDirectoryName = "Addons";
#endif
    public TextAsset dllAsset;
    public TextAsset bundleAsset;
    public TextAsset pickerBundleAsset;
    public TextAsset license1Asset;
    public TextAsset license2Asset;
    public TextAsset license3Asset;
    private void Awake()
    {
        Assembly.GetExecutingAssembly();
        if (GameDirectory == string.Empty)
        {
            GameDirectory = Application.dataPath + "/..";
            AddonDirectory = Path.Combine(GameDirectory, AddonDirectoryName);

        }
#if RuntimeInspector
        #region Extract RuntimeInspector DLL and Assets
        try
        {
            if (!Directory.Exists(AddonDirectory))
            {
                Directory.CreateDirectory(AddonDirectory);
            }
            if (!File.Exists(Path.Combine(AddonDirectory, DllName))
               || !File.Exists(Path.Combine(AddonDirectory, BundleName))
               || !File.Exists(Path.Combine(AddonDirectory, PickerBundleName))
               || !File.Exists(Path.Combine(AddonDirectory, Licence1Name))
               || !File.Exists(Path.Combine(AddonDirectory, Licence2Name))
               || !File.Exists(Path.Combine(AddonDirectory, Licence3Name)))
            {
                Debug.Log("RuntimeInspector.dll writing: " + Path.Combine(AddonDirectory, DllName));
                if (!File.Exists(Path.Combine(AddonDirectory, DllName)))
                    using (FileStream fileStream = File.Create(Path.Combine(AddonDirectory, DllName)))
                    {
                        fileStream.Write(dllAsset.bytes, 0, dllAsset.bytes.Length);
                    }
                if (!File.Exists(Path.Combine(AddonDirectory, BundleName)))
                    using (FileStream fs = File.Create(Path.Combine(AddonDirectory, BundleName)))
                    {
                        fs.Write(bundleAsset.bytes, 0, bundleAsset.bytes.Length);
                    }
                if (!File.Exists(Path.Combine(AddonDirectory, PickerBundleName)))
                    using (FileStream fs = File.Create(Path.Combine(AddonDirectory, PickerBundleName)))
                    {
                        fs.Write(bundleAsset.bytes, 0, pickerBundleAsset.bytes.Length);
                    }
                if (!File.Exists(Path.Combine(AddonDirectory, Licence1Name)))
                    using (FileStream fileStream = File.Create(Path.Combine(AddonDirectory, Licence1Name)))
                    {
                        fileStream.Write(license1Asset.bytes, 0, license1Asset.bytes.Length);
                    }
                if (!File.Exists(Path.Combine(AddonDirectory, Licence2Name)))
                    using (FileStream fileStream = File.Create(Path.Combine(AddonDirectory, Licence2Name)))
                    {
                        fileStream.Write(license2Asset.bytes, 0, license2Asset.bytes.Length);
                    }
                if (!File.Exists(Path.Combine(AddonDirectory, Licence3Name)))
                    using (FileStream fileStream = File.Create(Path.Combine(AddonDirectory, Licence3Name)))
                    {
                        fileStream.Write(license3Asset.bytes, 0, license3Asset.bytes.Length);
                    }
            }
            Destroy(dllAsset);
            Destroy(pickerBundleAsset);
            Destroy(bundleAsset);
            Destroy(license1Asset);
            Destroy(license2Asset);
            Destroy(license3Asset);
        }
        catch (Exception e)
        {
            Debug.LogError("RuntimeInspector CAN'T LOAD!!!!!!\n" + e.Message);
        }
        #endregion
#endif
        Debug.Log("[RuntimeInspector] Directory: " + AddonDirectory);
        DirectoryInfo d = new DirectoryInfo(AddonDirectory);

        #region Start up RuntimeInspector
        if (d.Exists)
        {
#if RuntimeInspector
            FileInfo dll = new FileInfo(Path.Combine(AddonDirectory, DllName));
            AddonManager.loadedAddons = new List<Addon>();
            Debug.Log("[RuntimeInspector] Loading DlL: " + dll.Name);
            Assembly ass = Assembly.LoadFile(dll.FullName);
            if (ass == null) return;
            Type type = ass.GetType("AddonInit", true, true);
            if (type != null)
            {
                var resourceLoader = ServiceProvider.Instance.ResourceLoader;
                FieldInfo _assetBundle = resourceLoader.GetType()
                                    .GetField("_assetBundle", BindingFlags.NonPublic | BindingFlags.Instance);
              //((AssetBundle)_assetBundle.GetValue(resourceLoader)).Unload(false);
              //_assetBundle.SetValue(resourceLoader, null);
                type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                Debug.Log($"[RuntimeInspector] Initializing AddonInit of RuntimeInspector");
            }

            var types = ass.GetTypes();
            if (types == null) return;
            List<Type> scripts = new List<Type>();
            foreach (var t in types)
            {
                //Debug.Log("type in " + dll.Name + ": " + t.FullName);
                if (t.GetTypeInfo().IsSubclassOf(typeof(MonoBehaviour)))
                    scripts.Add(t);
            }
            if (scripts.Count > 0)
            {
                Addon mod = new Addon(scripts, dll.Name);
                if (mod.modScripts.Count > 0)
                {
                    AddonManager.loadedAddons.Add(mod);
                    Debug.Log("[RuntimeInspector] Addon Loaded: " + mod.name);
                }
                //else
                //    Debug.Log($"[RuntimeInspector] WARNNING: Addon {dll.Name} does not contain a MonoBehaviour that has SPAddonAttribute");
            }
#else
            var infos = d.GetFiles("*.dll", SearchOption.AllDirectories);
            if (infos != null)
            {
                AddonManager.loadedAddons = new List<Addon>();
                foreach (var dll in infos)
                {
                    Debug.Log("[AddonLoader] Loading DLL: " + dll.Name);
                    Assembly ass = Assembly.LoadFile(dll.FullName);
                    if (ass == null) continue;
                    Type type = ass.GetType("AddonInit", true, true);
                    if (type != null)
                    {
                        type.GetConstructor(new Type[] { }).Invoke(new object[] { });
                        Debug.Log($"[AddonLoader] Initializing AddonInit of Addon [{dll.Name}]");
                    }

                    var types = ass.GetTypes();
                    if (types == null) continue;
                    List<Type> scripts = new List<Type>();
                    foreach (var t in types)
                    {
                        //Debug.Log("type in " + dll.Name + ": " + t.FullName);
                        if (t.GetTypeInfo().IsSubclassOf(typeof(MonoBehaviour)))
                            scripts.Add(t);
                    }
                    if (scripts.Count > 0)
                    {
                        Addon mod = new Addon(scripts, dll.Name);
                        if (mod.modScripts.Count > 0)
                        {
                            AddonManager.loadedAddons.Add(mod);
                            Debug.Log("[AddonLoader] Addon Loaded: " + mod.name);
                        }
                        else
                            Debug.Log($"[AddonLoader] WARNNING: Addon {dll.Name} does not contain a MonoBehaviour that has SPAddonAttribute");
                    }
                }
            }
#endif
        }
#if RuntimeInspector
#else
        DontDestroyOnLoad(AddonManager.persistantModRoot);
        AddonManager.ActivateMods(AddonStart.Persistant);
#endif
        #endregion

    }
    void Start()
    {
        #region Not working!
        //ServiceProvider.Instance.GameState.DesignerEntered += (object sender, EventArgs e) =>
        //{
        //    Debug.Log("******ServiceProvider.Instance.GameState.DesignerEntered");
        //    AddonManager.ActivateMods(AddonStart.Designer);
        //};
        //ServiceProvider.Instance.GameState.LevelEntered += (object sender, LevelChangedEventArgs args) => AddonManager.ActivateMods(AddonStart.Level);
        //ServiceProvider.Instance.GameState.MapEntered += (object sender, MapChangedEventArgs args) => AddonManager.ActivateMods(AddonStart.Map);
        #endregion
        enabled = false;
        PersistentObject po = GetComponent<PersistentObject>();
        DestroyImmediate(this);
        Destroy(po);
    }


    private void OnGUI()
    {
        //GUI.Box(new Rect(200, 140, 200, 20), "InDesigner: " + ServiceProvider.Instance.GameState.IsInDesigner);
        //GUI.Box(new Rect(200, 120, 200, 20), "[AddonLoader] Key: " + Input.anyKeyDown);
        //  GUI.Box(new Rect(200, 100, 200, 20), "[AddonLoader] addons loded: " + AddonManager.loadedAddons.Count);
    }
}
