using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

	public GameObject block;

	public int length = 5, width = 5, height = 5;

	// Use this for initialization
	void Start () 
	{
		if(block != null)
		{
			Vector3 startPosition = block.transform.position;

			for(int x = 0; x < width; x++)
			{
				for(int y = 0; y < height; y++)
				{
					for(int z = 0; z < length; z++)
					{
						GameObject child = Instantiate(block, startPosition - new Vector3(x, y, z), block.transform.rotation) as GameObject;
						child.transform.SetParent (this.transform, true);
					}
				}
			}
		}
	}
}
