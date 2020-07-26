using RuntimeInspectorNamespace;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TargetPicker : MonoBehaviour
{
    private float currentRayLength;

    private float initialRayLength = 1000f;

    private float maxRayLength = 9000f;

    public bool hitLastFrame;

    public RaycastHit hitInfo;

    public static KeyCode KeyCodePick = KeyCode.Mouse2;

    public static bool EnablePick;

    public static bool AlwaysRaycast;

    public bool Enabled;

    public static KeyCode ActivateKey1 = KeyCode.LeftAlt;

    public static KeyCode ActivateKey2 = KeyCode.I;

    public GameObject pnl;

    public RuntimeHierarchy hie;

    public RuntimeInspector ins;

    public Toggle tglPik;

    public Toggle tglHvr;

    public Toggle tglIns;

    public Toggle tglHie;
    public Toggle tglUi;

    public InputBlocker blck1;
    public InputBlocker blck2;

    private bool ToggleActivate()
    {
        return Input.GetKey(TargetPicker.ActivateKey1) && Input.GetKeyDown(TargetPicker.ActivateKey2);
    }
    private GameObject uiRoot;
    internal static TargetPicker Instance;
    internal int uiLayer;

    internal GameObject UIRoot
    {
        get
        {
            if (!uiRoot)
            {
                uiRoot = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(p => p.name.StartsWith("UI Root"));
            }
            return uiRoot;
        }
    }
    private GameObject outLiner;
    private GameObject OutLiner
    {
        get
        {
            if (!outLiner)
            {
                outLiner = new GameObject("OutLiner");
                outLiner.transform.SetParent(transform);
                outLiner.SetActive(false);
                outLiner.AddComponent<QuickOutline>().FindRenderableObject = true;
            }
            return outLiner;
        }
    }
    private void Start()
    {
        Instance = this;
        _ = UIRoot;
        tglPik.onValueChanged.AddListener(delegate (bool b)
        {
            EnablePick = tglPik.isOn;
            tglHvr.interactable = TargetPicker.EnablePick;
            if (!EnablePick)
                tglHvr.isOn = false;
            tglUi.isOn = tglPik.isOn;
            if (!tglHvr.isOn&&outLiner)
            {
                outLiner.transform.SetParent(transform);
                outLiner.SetActive(false);
            }
        });
        tglHvr.onValueChanged.AddListener(delegate (bool b)
        {
            AlwaysRaycast = tglHvr.isOn;
        });
        tglIns.onValueChanged.AddListener(delegate (bool b)
        {
            ins.gameObject.SetActive(tglIns.isOn);
        });
        tglHie.onValueChanged.AddListener(delegate (bool b)
        {
            hie.gameObject.SetActive(tglHie.isOn);
        });
        tglUi.onValueChanged.AddListener(delegate (bool b)
        {
            UIRoot.SetActive(!tglUi.isOn);
            InputBlocker.uirootenabled = !tglUi.isOn;
            //blck1.enabled = tglUi.isOn;
            //blck2.enabled = tglUi.isOn;
        });
    }
    RaycastHit hit;
    private void Update()
    {
        if (ToggleActivate())
        {
            Enabled = !Enabled;
            pnl.SetActive(Enabled);
            hie.gameObject.SetActive(Enabled);
            ins.gameObject.SetActive(Enabled);
           if(!Enabled) UIRoot.SetActive(true);
            if (!Enabled && outLiner)
            {
                outLiner.transform.SetParent(transform);
                outLiner.SetActive(false);
            }
        }
        if (Enabled)
        {
            hitLastFrame = false;
            if ((EnablePick && Input.GetKeyDown(KeyCodePick)) || AlwaysRaycast)
            {
                if (UIRoot.activeSelf) tglUi.isOn = true;
                Camera camera = (Camera.main == null) ? Camera.main : Camera.current;
                currentRayLength = Mathf.Clamp(currentRayLength, initialRayLength, maxRayLength);
                if ((Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out var raycastHit, float.PositiveInfinity, 0b1111111111111111111111)))
                {
                    hitLastFrame = true;
                    hitInfo = raycastHit;
                }
                else
                {
                    if (!hitLastFrame)
                    {
                        hitInfo = default;
                        currentRayLength = Mathf.Clamp(currentRayLength + 500f, initialRayLength, maxRayLength);
                    }
                }
            }
            if (hitLastFrame)
            {
                hie.Select(hitInfo.transform, true);
                OutLiner.transform.SetParent(hitInfo.transform);
                OutLiner.GetComponent<QuickOutline>().enabled = true; ;
                OutLiner.SetActive(true);
            }
        }
    }
    //private void OnGUI()
    //{
    //    GUI.Label(new UnityEngine.Rect(500, 500, 500, 20), "" + Camera.main.transform.position.ToString());
    //    GUI.Label(new UnityEngine.Rect(500, 520, 500, 20), "hitLastFrame?: " + hitLastFrame);
    //    GUI.Label(new UnityEngine.Rect(500, 540, 500, 20), "Enabled?: " + Enabled);
    //    GUI.Label(new UnityEngine.Rect(500, 560, 500, 20), "hits?: " + uiLayer);
    //    if (hit.transform) GUI.Label(new UnityEngine.Rect(500, 580, 500, 20), "hits?: " + hit.transform + "\troot: " + hit.transform.root);
    //}
    private void OnDisable()
    {
        currentRayLength = 0f;
        if (outLiner)
        {
            outLiner.transform.SetParent(transform);
            outLiner.SetActive(false);
        }
    }
}