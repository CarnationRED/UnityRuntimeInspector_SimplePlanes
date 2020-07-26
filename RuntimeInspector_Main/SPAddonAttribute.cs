[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SPAddonAttribute : System.Attribute
{
    public AddonStart start;
    public AddonHost host;
    public SPAddonAttribute(AddonStart s, AddonHost h = AddonHost.GameScene)
    {
        start = s;
        host = h;
    }

}
