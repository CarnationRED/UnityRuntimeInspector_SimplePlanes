using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDesigner : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        AddonManager.ActivateMods(AddonStart.Designer);
        Helper.InvokeOnEnterDesigner();
    }
    private void OnDisable()
    {
        Helper.InvokeOnExitDesigner();
    }
}
