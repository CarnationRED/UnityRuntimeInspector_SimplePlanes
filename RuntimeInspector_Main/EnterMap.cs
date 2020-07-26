using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterMap : MonoBehaviour
{
    private void OnEnable()
    {
        AddonManager.ActivateMods(AddonStart.Map);
        AddonManager.ActivateMods(AddonStart.Level);
        Helper.InvokeOnEnterLevel();
    }
    private void OnDisable()
    {
        Helper.InvokeOnExitLevel();
    }

}
