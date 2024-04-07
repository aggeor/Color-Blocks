using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour {
	private static FloatingText popupText;
	private static GameObject canvas;

	public static void Initialize(){
		canvas = GameObject.Find ("Canvas");
		popupText = Resources.Load<FloatingText> ("PopupTextParent");
	}

	public static void CreateFloatingText(string text,Vector2 location){
		
		FloatingText instance = Instantiate(popupText);

		Vector2 screenPosition = Camera.main.WorldToScreenPoint (location);
		instance.transform.SetParent (canvas.transform,false);
		instance.transform.position = screenPosition;
		instance.SetText (text);
	}
}
