using UnityEngine;
using System.Collections;

public class CursorSettings : MonoBehaviour {

	public bool lockCursor;
	public bool showCursor;

	// Update is called once per frame
	void Update () 
	{
		if(lockCursor && Cursor.lockState != CursorLockMode.Locked)
			Cursor.lockState = CursorLockMode.Locked;
		else
			Cursor.lockState = CursorLockMode.None;

//		if(showCursor && !Cursor.visible)
//			Cursor.visible = true;
//		else
//			Cursor.visible = false;
	}
}
