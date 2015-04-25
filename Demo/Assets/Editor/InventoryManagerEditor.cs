#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InventoryManager))]
public class InventoryManagerEditor : Editor {

	GameObject ob;
	string[] DisabledStrings;
	int selectedDisable;

	public override void OnInspectorGUI()
	{
		InventoryManager invUI = (InventoryManager)target;

		//Script in Inspector
		serializedObject.Update();
		SerializedProperty script = serializedObject.FindProperty("m_Script");
		EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
		serializedObject.ApplyModifiedProperties();

		if(GUILayout.Button ("Open Inventory Database"))
			InventoryDatabaseWindow.Init ();
		if(GUILayout.Button ("Open Tooltip Settings"))
			TooltipWindow.Init ();

		invUI.character = EditorGUILayout.ObjectField ("Character" ,invUI.character, typeof(GameObject), true) as GameObject;

		invUI.cameraComponent = EditorGUILayout.ObjectField ("Character Camera" ,invUI.cameraComponent, typeof(Camera), true) as Camera;
		
		invUI.dropTransform = EditorGUILayout.ObjectField ("Drop Transform" ,invUI.dropTransform, typeof(Transform), true) as Transform;
		
		invUI.dropOffset = EditorGUILayout.FloatField ("Drop Offset" ,invUI.dropOffset);

		invUI.stackingActive = EditorGUILayout.Toggle ("Stacking Active" ,invUI.stackingActive);

		if(GUI.changed)
			EditorUtility.SetDirty (target);
	}
}
#endif
