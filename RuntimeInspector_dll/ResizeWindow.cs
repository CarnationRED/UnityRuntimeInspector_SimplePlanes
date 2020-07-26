using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeWindow : MonoBehaviour, IDragHandler
{
    public RectTransform TargetWindow;
    public int MinWidth;
    public int MinHeight;

    public void OnDrag(PointerEventData eventData)
    {
        var sb = TargetWindow.sizeDelta;
        var s = TargetWindow.sizeDelta;
        s += eventData.delta * new Vector2(1, -1);
        var w = Mathf.Max(MinWidth, s.x);
        var h = Mathf.Max(MinHeight, s.y);
        TargetWindow.sizeDelta = new Vector2(w, h);
        TargetWindow.transform.Translate((TargetWindow.sizeDelta - sb)*new Vector2(.5f,-.5f));

    }
}
