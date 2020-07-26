using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

public class Addon
{
    public string name;
    public string path;
    public List<Type> modScripts;
    public List<AddonStart> modStart;
    public List<AddonHost> modHost;
    public Addon(List<Type> scripts, string name)
    {
        modScripts = new List<Type>(scripts.Count);
        modStart = new List<AddonStart>(scripts.Count);
        modHost = new List<AddonHost>(scripts.Count);
        this.name = name;
        foreach (var s in scripts)
        {
            var attrObj =s. GetCustomAttribute<SPAddonAttribute>();
            if (attrObj != null)
            {
                modScripts.Add(s);
                modStart.Add(attrObj.start);
                modHost.Add(attrObj.host);
            }
        }
    }
}
