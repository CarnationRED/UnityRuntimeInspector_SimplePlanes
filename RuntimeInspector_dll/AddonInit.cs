using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

public class AddonInit
{
    public AddonInit()
    {
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "runtimeinspector.bundle");
        AssetBundle assetBundle;
        Debug.Log("RuntimrInspector: Loading AssetBundle in: " + path);
        assetBundle = AssetBundle.LoadFromFile(path);
        if (!assetBundle)
        {
            Debug.LogError("Can't load RuntimInspector, check: " + path);
        }
        else
        {
            GameObject root = null;
            root = assetBundle.LoadAsset<GameObject>("RuntimeInspector_DLL");
            if (root)
            {
                GameObject gameObject2 = Object.Instantiate(root);
                Object.DontDestroyOnLoad(gameObject2.transform.root);
                gameObject2.SetActive(true);
                Debug.Log("RuntimeInspector Root Object loaded");
                QuickOutline.outlineMaskMaterial = assetBundle.LoadAsset<Material>("OutlineMask");
                QuickOutline.outlineFillMaterial = assetBundle.LoadAsset<Material>("OutlineFill");
                Debug.Log("RuntimeInspector Quick Outline shaders loaded: " + QuickOutline.outlineMaskMaterial.name + " And " + QuickOutline.outlineFillMaterial);
            }
            else
            {
                Debug.LogError("Can't load RuntimeInspector from /runtimeinspector.bundle");
                foreach (var s in assetBundle.LoadAllAssets())
                    Debug.Log("Assets in /runtimeinspector.bundle: " + s);

            }
            assetBundle.Unload(false);
        };
    }
}
