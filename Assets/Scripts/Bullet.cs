using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public int damage;
	public Chessman target;
	public bool isHeal;

	public float vel = 1.0f, acel, force;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(target == null)
		{
			Object.Destroy(this.gameObject);
		}
		else
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, vel * Time.deltaTime);

			if(Vector3.Distance(this.transform.position, target.transform.position) < 0.25f)
			{
				if(isHeal)
				{
					target.GetHeal(this.damage);
				}
				else
				{
					target.GetDamage(this.damage, 1);
				}
				Object.Destroy(this.gameObject);
			}
		}
	}
}
