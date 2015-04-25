using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(TextObject), true)]
public class TextObjectEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		TextObject textOb = (TextObject)target;

		textOb.selectedGameObject = (GameObject) EditorGUILayout.ObjectField ("GameObject", textOb.selectedGameObject, typeof(GameObject), true);

		if(textOb.selectedGameObject != null)
		{
			MonoBehaviour[] components = textOb.selectedGameObject.GetComponents<MonoBehaviour>();

			string[] componentsNames = new string[components.Length];
			for(int i = 0; i < componentsNames.Length; i++)
			{
				componentsNames[i] = components[i].GetType ().ToString();
			}

			textOb.selectedMonoBehaviourIndex = EditorGUILayout.Popup ("Components", textOb.selectedMonoBehaviourIndex, componentsNames);

			textOb.selectedMonoBehaviourName = componentsNames[textOb.selectedMonoBehaviourIndex];

			if(components[textOb.selectedMonoBehaviourIndex] != null)
			{
				string[] fieldNames = new string[components[textOb.selectedMonoBehaviourIndex].GetType ().GetFields ().Length + 1];

				fieldNames[0] = "None";
				for(int i = 1; i < fieldNames.Length; i++)
				{
					fieldNames[i] = components[textOb.selectedMonoBehaviourIndex].GetType ().GetFields ()[i - 1].Name;
				}

				textOb.selectedField = EditorGUILayout.Popup ("Fields", textOb.selectedField, fieldNames);
				textOb.selectedFieldName = fieldNames[textOb.selectedField];
				MonoBehaviour mb = (MonoBehaviour) textOb.selectedGameObject.GetComponent(textOb.selectedMonoBehaviourName);

				if(textOb.selectedField > 0)
					textOb.textComponent.text = char.ToUpper (textOb.selectedFieldName[0]) + textOb.selectedFieldName.Substring (1) + textOb.format + mb.GetType ().GetField(textOb.selectedFieldName).GetValue (mb).ToString ();

				textOb.format = EditorGUILayout.TextField ("Format", textOb.format);
			}
		}
	}
}
