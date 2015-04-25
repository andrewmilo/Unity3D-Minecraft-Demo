using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(LootableObject))]
public class LootableObjectEditor : Editor {

	List<string> itemNames = new List<string>();

	public override void OnInspectorGUI()
	{
		LootableObject lo = target as LootableObject;

		EditorGUILayout.PropertyField (serializedObject.FindProperty("elementID"));

		EditorGUILayout.PropertyField (serializedObject.FindProperty("stack"));
	}
}