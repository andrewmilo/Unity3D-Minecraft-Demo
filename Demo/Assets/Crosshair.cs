using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

	public Texture crosshairTexture;
	private Rect position;

	void Update () {
		position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - crosshairTexture.height) /2, crosshairTexture.width, crosshairTexture.height);
	}

	void OnGUI()
	{
		GUI.DrawTexture(position, crosshairTexture);
	}
}
