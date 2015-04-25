using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CharacterActions : MonoBehaviour {

	private GameObject wieldedGameObject;
	public float Distance = 10;
	public float GoDepth = 4;
	private bool dealDamage;
	public Camera CameraComponent;

	// Update is called once per frame
	void Update () 
	{
		if(wieldedGameObject != null)
		{
			wieldedGameObject.transform.position = transform.position + Camera.main.transform.forward * 1.5f + Camera.main.transform.right;
			wieldedGameObject.transform.rotation = transform.rotation;
		}
	}

	public void Wield(InventoryElement element)
	{
		Debug.Log ("Wielded");
		if(element.gameObject != null)
		{
			wieldedGameObject = (GameObject) Instantiate (element.gameObject, transform.position + Camera.main.transform.forward, Camera.main.transform.rotation);
			wieldedGameObject.transform.localScale = new Vector3(.20f,.10f,.10f);
		}
	}

	public void UnWield()
	{
		Debug.Log ("UnWielded");
		Destroy (wieldedGameObject);
	}

	public void Attack()
	{
		Debug.Log ("Attack!");
	}

	public void Block()
	{
		Debug.Log ("Block!");
	}

	public void DamageBlock(string stringDamage)
	{
		RaycastHit hit;
		
		if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f, 1))
		{
			GameObject go = hit.transform.gameObject;

			if(!InventoryManager.inventory.isFull () || !InventoryManager.actionBar.isFull())
			{
				int damage = int.Parse (stringDamage);
				Health _health = go.GetComponent<Health>();
				_health.health -= damage;

				if(_health.health <= 0)
				{
					LootableObject lo = _health.GetComponent<LootableObject>();

					InventoryElement ie = new InventoryElement(InventoryDatabase.GetElement(lo.elementID));

					if(!InventoryManager.inventory.isFull ())
						InventoryManager.inventory.AddItem (ref ie, false);
					else if(!InventoryManager.actionBar.isFull())
						InventoryManager.actionBar.AddItem(ref ie, false);

					Destroy (go);
				}
			}
		}
	}

	public void PlaceBlock(InventoryElement block)
	{
		RaycastHit hit;
		if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f, 1))
		{
			if(hit.transform.tag == "Block")
				Instantiate(block.gameObject, hit.transform.position + hit.normal, block.gameObject.transform.rotation);
		}
	}

	public void TriggerExplosion(string distance, string delay)
	{
		int intDistance;
		int.TryParse (distance, out intDistance);

		int intDelay;
		int.TryParse(delay, out intDelay);

		RaycastHit hit;
		if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, intDistance, 1))
		{
			Explosive explosive = hit.transform.GetComponent<Explosive>();
			if(explosive != null)
				StartCoroutine(explosive.Detonate(intDelay));
		}
	}

	public void Equip(ref InventoryElement e)
	{
		CharacterMenu charMenu = FindObjectOfType<CharacterMenu>();

		if(charMenu != null)
		{
			List<Slot> fitSlots = new List<Slot>();// charMenu.Slots.FindAll (s => s.acceptedTypes.Exists(x => x.ID == e.type.ID || x.isAncestorOf(e.type)));

			foreach(Slot s in charMenu.Slots)
			{
				foreach(ElementType eType in s.acceptedTypes)
				{
					if(eType.ID == e.type.ID || eType.isAncestorOf (e.type))
						fitSlots.Add (s);
				}
			}

			Slot fitSlot = fitSlots.Find(s => s.IsEmpty());

			if(fitSlot != null)
			{
				fitSlot.inventoryElement = e;
				e = null;
			}
			else
			{
				fitSlots[0].inventoryElement = e;
				e = null;
			}
		}
	}
}
