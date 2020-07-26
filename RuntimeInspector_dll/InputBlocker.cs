using UnityEngine.EventSystems;

public class InputBlocker : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        uirootenabled = TargetPicker.Instance.UIRoot.activeSelf;
        TargetPicker.Instance.UIRoot.SetActive(false);
    }

   internal static bool uirootenabled;
    public void OnPointerExit(PointerEventData eventData) => TargetPicker.Instance.UIRoot.SetActive(uirootenabled);
}
