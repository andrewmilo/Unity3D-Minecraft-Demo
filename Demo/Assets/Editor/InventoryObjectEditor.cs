using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InventoryObject), true)]
public class InventoryObjectEditor : Editor 
{
	private string[] Items;
	private int item;
	private InventoryElement temp;
	private int selectedType;
	private string[] types;
	private string[] methods;
	private int methodInt;
	private char parsedChar;
	private string stringforChar;
	private int slotCountX;
	private int slotCountY;

	public override void OnInspectorGUI()
	{
		InventoryObject Inv = (InventoryObject)target;

		//Script in Inspector
		serializedObject.Update();
		SerializedProperty script = serializedObject.FindProperty("m_Script");
		EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
		serializedObject.ApplyModifiedProperties();
		
		Inv.displayFoldout = EditorGUILayout.Foldout(Inv.displayFoldout, "Display Settings");
		if(Inv.displayFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			Inv.gameObject.SetActive(EditorGUILayout.Toggle ("Active" , Inv.gameObject.activeSelf));
			GUILayout.EndHorizontal();

			//Script in Inspector
			serializedObject.Update();
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			SerializedProperty toggleKey = serializedObject.FindProperty("ToggleKey");
			EditorGUILayout.PropertyField(toggleKey, true, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();

			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			Inv.disableScriptsFoldout = EditorGUILayout.Foldout(Inv.disableScriptsFoldout, "Scripts to Disable");
			GUILayout.EndHorizontal();
			
			if (Inv.disableScriptsFoldout)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(30);
				Inv.disableGO = EditorGUILayout.ObjectField("Components from: ", (GameObject)Inv.disableGO, typeof(GameObject), true) as GameObject;
				GUILayout.EndHorizontal();
				
				if (Inv.disableGO != null)
				{
					Component[] comps = Inv.disableGO.GetComponents(typeof(Behaviour));
					Inv.disableStrings = new string[comps.Length];
					
					for (int l = 0; l < comps.Length; l++)
					{
						Inv.disableStrings[l] = comps[l].ToString();
					}
					
					GUILayout.BeginHorizontal();
					GUILayout.Space(15);
					Inv.selectedDisable = EditorGUILayout.Popup(Inv.selectedDisable, Inv.disableStrings);
					
					if (GUILayout.Button("Add"))
					{
						Inv.scriptsToDisable.Add((Behaviour)comps[Inv.selectedDisable]);
					}
					GUILayout.EndHorizontal();
				}
				
				for (int n = 0; n < Inv.scriptsToDisable.Count; ++n)
				{
					if(Inv.scriptsToDisable[n] != null)
					{
						GUILayout.BeginHorizontal(GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
						GUILayout.Space(45);
						GUILayout.Label(n + ". ");
						
						GUILayout.TextField(Inv.scriptsToDisable[n].ToString(), GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
						if (GUILayout.Button("Remove", GUILayout.MaxWidth(80), GUILayout.MinWidth(80)))
						{
							Inv.scriptsToDisable[n].enabled = true;
							Inv.scriptsToDisable.RemoveAt(n);
						}
						GUILayout.EndHorizontal();
					}
					else
						Inv.scriptsToDisable.RemoveAt(n);
				}
			}
		}

		Inv.positionFoldout = EditorGUILayout.Foldout(Inv.positionFoldout, "Position");
		if(Inv.positionFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			Inv.percentageOfScreenX = EditorGUILayout.Slider("% of Screen X", Inv.percentageOfScreenX, 0, 1);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			Inv.percentageOfScreenY = EditorGUILayout.Slider("% of Screen Y", Inv.percentageOfScreenY, 0, 1);
			GUILayout.EndHorizontal();
		}

		Inv.backgroundFoldout = EditorGUILayout.Foldout (Inv.backgroundFoldout, "Background");
		
		if(Inv.backgroundFoldout)
		{
			if(Inv.backgroundRawImage != null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space (15);
				EditorGUILayout.PropertyField (serializedObject.FindProperty ("backgroundRawImage"));
				GUILayout.EndHorizontal();
			}
		}

		Inv.slotFoldout = EditorGUILayout.Foldout(Inv.slotFoldout, "Slot Settings");
		if (Inv.slotFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			Inv.SlotSpacing = EditorGUILayout.Vector2Field("Slot Spacing", Inv.SlotSpacing);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			Inv.SlotSize = EditorGUILayout.Vector2Field("Slot Size", Inv.SlotSize);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(GUILayout.MinWidth(180), GUILayout.MaxWidth(180));
			GUILayout.Space(15);
			GUILayout.Label("Horizontal Slots");
			Inv.horizontalSlots = EditorGUILayout.IntField(Inv.horizontalSlots, GUILayout.MinWidth(30), GUILayout.MaxWidth(30));
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal(GUILayout.MinWidth(180), GUILayout.MaxWidth(180));
			GUILayout.Space(15);
			GUILayout.Label("Vertical Slots");
			Inv.verticalSlots = EditorGUILayout.IntField(Inv.verticalSlots, GUILayout.MinWidth(30), GUILayout.MaxWidth(30));
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			Inv.slotTexture = (Texture2D)EditorGUILayout.ObjectField("Slot Textures", Inv.slotTexture, typeof(Texture2D), false);
			GUILayout.EndHorizontal();

			if(GUILayout.Button ("Create Grid"))
			{
				for(int y = 0; y < Inv.verticalSlots; y++)
				{
					for(int x = 0; x < Inv.horizontalSlots; x++)
					{
						//Create new Slot
						GameObject newSlotGO = new GameObject("Slot " + (Inv.slotsTransform.childCount + 1));
						newSlotGO.transform.parent = Inv.slotsTransform;
						Slot addedSlot = newSlotGO.AddComponent<Slot>();
						addedSlot.Init ();
						addedSlot.inventoryObject = Inv;

						addedSlot.backgroundRawImage.texture = Inv.slotTexture;
						addedSlot.rectTransform.sizeDelta = Inv.SlotSize;

						addedSlot.rectTransform.position = new Vector2(Inv.SlotSize.x * x * Inv.SlotSpacing.x + (Inv.percentageOfScreenX * Inv.screenWidth) + Inv.SlotSize.x/2 - Inv.Size ().x/2, 
						                                               Inv.screenHeight - (Inv.SlotSize.y * y * Inv.SlotSpacing.y + (Inv.percentageOfScreenY * Inv.screenHeight) + Inv.SlotSize.y/2 - Inv.Size ().y/2));

						Inv.backgroundRawImage.rectTransform.sizeDelta = Inv.Size ();
					}
				}
			}
		}

		Inv.textFoldout = EditorGUILayout.Foldout(Inv.textFoldout, "UI Settings");
		if(Inv.textFoldout)
		{
			if(GUILayout.Button ("Add Slot"))
			{
				GameObject go = new GameObject("Slot");
				Slot newslot = go.AddComponent<Slot>();
				newslot.inventoryObject = Inv;
				newslot.Init();
				newslot.transform.SetParent (Inv.slotsTransform, false);
				newslot.transform.SetAsLastSibling ();
#if UNITY_EDITOR
				UnityEditor.Selection.activeGameObject = go;
#endif
			}

			if(GUILayout.Button ("Add Field Tracker"))
			{
				GameObject go = new GameObject("Stat");
				TextObject newStatTracker = go.AddComponent<TextObject>();
				newStatTracker.transform.SetParent (Inv.backgroundImageTransform, false);
				newStatTracker.transform.SetAsLastSibling ();
#if UNITY_EDITOR
				UnityEditor.Selection.activeGameObject = go;
#endif
			}

			if(GUILayout.Button ("Add Raw Image"))
			{
				GameObject go = new GameObject("Raw Image");
				UnityEngine.UI.RawImage newRawImage = go.AddComponent<UnityEngine.UI.RawImage>();
				newRawImage.transform.SetParent (Inv.backgroundImageTransform, false);
				go.transform.SetAsFirstSibling ();
#if UNITY_EDITOR
				UnityEditor.Selection.activeGameObject = go;
#endif
			}

			if(GUILayout.Button ("Add Sprite Image"))
			{
				GameObject go = new GameObject("Sprite Image");
				UnityEngine.UI.Image newSprite = go.AddComponent<UnityEngine.UI.Image>();
				newSprite.transform.SetParent (Inv.backgroundImageTransform, false);
				go.transform.SetAsFirstSibling ();
#if UNITY_EDITOR
				UnityEditor.Selection.activeGameObject = go;
#endif
			}
		}

		if(GUI.changed)
			EditorUtility.SetDirty (target);
	}
}
