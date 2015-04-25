using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour {

	public int damage;
	public float radius;
	public ParticleSystem explosionVisual;

	public IEnumerator Detonate (float delayInSeconds)
	{
		yield return new WaitForSeconds(delayInSeconds);

		Vector3 pos = this.transform.position;

		Instantiate( explosionVisual, pos, Quaternion.identity );
		DestroyImmediate(this.gameObject);

		Collider[] colliders = Physics.OverlapSphere(pos, radius);

		foreach(Collider c in colliders)
		{
			Health health = c.GetComponent<Health>();
			if(health != null)
			{
				health.health -= damage;

				if(health.health <= 0)
					DestroyImmediate(health.gameObject);
			}
		}
	}
}
