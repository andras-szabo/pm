using UnityEngine;

public class CustomCompassMarker : MonoBehaviour 
{
	public HUDMarker HUDMarker { get; private set; }
	public UnityEngine.UI.Graphic widget;

	public void Setup(HUDMarker hudMarker)
	{
		HUDMarker = hudMarker;
		widget.color = hudMarker.Color;
	}
}
