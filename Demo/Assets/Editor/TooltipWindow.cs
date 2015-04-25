using UnityEngine;
using System.Collections;
using UnityEditor;

public class TooltipWindow : EditorWindow {

	//Prefab
	TooltipSettings settingsPrefab;

	[MenuItem("Database/Tooltip Settings %#t")]
	public static void Init () 
	{
		TooltipWindow window = EditorWindow.GetWindow<TooltipWindow>();
		window.Show ();
		window.autoRepaintOnSceneChange = true;
	}

	public void OnFocus()
	{
		if(settingsPrefab == null)
		{
			GameObject go = (GameObject) Resources.Load ("TooltipSettings");

			if(go != null)
				settingsPrefab = go.GetComponent<TooltipSettings>();
		}
	}

	void OnGUI() 
	{
		if(settingsPrefab != null)
		{
			GUILayout.Space (15);

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.tooltipEnabled = EditorGUILayout.Toggle ("Tooltip Enabled" , settingsPrefab.tooltipEnabled);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.tooltipBackgroundTexture = EditorGUILayout.ObjectField ("Tooltip Background" ,settingsPrefab.tooltipBackgroundTexture, typeof(Texture2D), true) as Texture2D;
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.minTooltipWidth = EditorGUILayout.FloatField("Constant Width", settingsPrefab.minTooltipWidth);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.minTooltipHeight = EditorGUILayout.FloatField("Constant Height", settingsPrefab.minTooltipHeight);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.nameFoldout = EditorGUILayout.Foldout (settingsPrefab.nameFoldout, "Name");
			GUILayout.EndHorizontal();

			if(settingsPrefab.nameFoldout)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.nameIndentation = EditorGUILayout.FloatField("Indentation", settingsPrefab.nameIndentation);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.nameSpacing = EditorGUILayout.FloatField("Vertical Spacing", settingsPrefab.nameSpacing);
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.descriptionFoldout = EditorGUILayout.Foldout (settingsPrefab.descriptionFoldout, "Description");
			GUILayout.EndHorizontal();

			if(settingsPrefab.descriptionFoldout)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.descriptionIndentation = EditorGUILayout.FloatField("Indentation", settingsPrefab.descriptionIndentation);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.descriptionSpacing = EditorGUILayout.FloatField("Vertical Spacing", settingsPrefab.descriptionSpacing);
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.typeFoldout = EditorGUILayout.Foldout (settingsPrefab.typeFoldout, "Type");
			GUILayout.EndHorizontal();
			
			if(settingsPrefab.typeFoldout)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.typeIndentation = EditorGUILayout.FloatField("Indentation", settingsPrefab.typeIndentation);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.typeSpacing = EditorGUILayout.FloatField("Vertical Spacing", settingsPrefab.typeSpacing);
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			settingsPrefab.actionFoldout = EditorGUILayout.Foldout (settingsPrefab.actionFoldout, "Action");
			GUILayout.EndHorizontal();
			
			if(settingsPrefab.actionFoldout)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.actionIndentation = EditorGUILayout.FloatField("Indentation", settingsPrefab.actionIndentation);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Space (30);
				settingsPrefab.actionSpacing = EditorGUILayout.FloatField("Vertical Spacing", settingsPrefab.actionSpacing);
				GUILayout.EndHorizontal();
			}

	//			serializedObject.Update ();
	//			GUILayout.BeginHorizontal();
	//			GUILayout.Space (15);
	//			EditorGUILayout.PropertyField (serializedObject.FindProperty ("tooltipNameFontStyle"));
	//			GUILayout.EndHorizontal();
	//			
	//			GUILayout.BeginHorizontal();
	//			GUILayout.Space (15);
	//			EditorGUILayout.PropertyField (serializedObject.FindProperty ("tooltipDescriptionFontStyle"));
	//			GUILayout.EndHorizontal();
	//			
	//			GUILayout.BeginHorizontal();
	//			GUILayout.Space (15);
	//			EditorGUILayout.PropertyField (serializedObject.FindProperty ("tooltipTypeFontStyle"));
	//			GUILayout.EndHorizontal();
				
			//serializedObject.ApplyModifiedProperties ();
		}
	}
}
