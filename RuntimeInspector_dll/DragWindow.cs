using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
internal class DragWindow : MonoBehaviour
{
	[SerializeField]
	private MaskableGraphic targetGraphic;

	[SerializeField]
	private MaskableGraphic targetGraphic2;

	[SerializeField]
	private RectTransform targetWindow;

	private RectTransform t;

	private RectTransform t2;

	private bool dragging;

	private Vector3 lastMousePos ;

	public static Vector3 clickMousePos;

	private float lastHeight;

	private void Start()
	{
		bool flag = this.targetGraphic;
		if (flag)
		{
			this.t = this.targetGraphic.rectTransform;
		}
		bool flag2 = this.targetGraphic2;
		if (flag2)
		{
			this.t2 = this.targetGraphic2.rectTransform;
		}
	}

	private void OnDisable()
	{
		this.dragging = false;
	}

	private void Update()
	{
		bool flag = !this.dragging;
		if (flag)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			if (mouseButtonDown)
			{
				bool flag2 = RectTransformUtility.RectangleContainsScreenPoint(this.t, Input.mousePosition, null) || (this.t2 && RectTransformUtility.RectangleContainsScreenPoint(this.t2, Input.mousePosition, null));
				if (flag2)
				{
					this.lastMousePos = Input.mousePosition;
					this.dragging = true;
					DragWindow.clickMousePos = this.lastMousePos;
				}
			}
		}
		else
		{
			bool keyUp = Input.GetKeyUp((KeyCode)323);
			if (keyUp)
			{
				this.dragging = false;
			}
			else
			{
				RectTransform expr_A0 = this.targetWindow;
				expr_A0.position=expr_A0.position + (Input.mousePosition - this.lastMousePos);
				this.lastMousePos = Input.mousePosition;
			}
		}
	}
}
