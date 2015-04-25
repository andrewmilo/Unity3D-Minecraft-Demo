using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(InventoryDatabase))]
public class ItemDatabaseEditor : Editor {

	// Use this for initialization
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		if(GUILayout.Button ("Open Item Database"))
			InventoryDatabaseWindow.Init ();
	}
}
