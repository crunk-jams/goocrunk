using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class GunCalibrateButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Button button;
	private bool hovering = false;
	static Vector2? calibratedPos = null;

	public void Awake()
	{
		button = GetComponent<Button>();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.A)
		 || Input.GetKeyDown(KeyCode.B)
		 || Input.GetKeyDown(KeyCode.C)
		 || Input.GetKeyDown(KeyCode.D))
		{
			if (hovering)
			{
				Debug.Log("Hit button");
				button.onClick.Invoke();
			}

		}
	}

	public void OnPointerEnter(PointerEventData pointerEvent)
	{
		hovering = true;
	}

	public void OnPointerExit(PointerEventData pointEvent)
	{
		hovering = false;
	}
}
