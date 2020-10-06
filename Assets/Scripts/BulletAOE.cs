using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAOE : MonoBehaviour {

	// Use this for initialization
	public int damage;
	public Chessman target;
	public bool isHeal;
	public int aoe_size = 3;
	public GameObject single_bullet;
	private List<GameObject> bulletlist;
	private List<Vector3Int> target_offset_list;

	public float vel = 1.0f, acel, force;
	private Vector3 target_pos;
	private int target_x, target_y;

	public float life_time = 2.0f;

	// Use this for initialization
	void Start () {
		bulletlist = new List<GameObject>();
		target_offset_list = new List<Vector3Int>();

		if(target != null)
		{
			target_pos = target.transform.position;
			target_x = target.CurrentX;
			target_y = target.CurrentY;
		}
		else
		{
			Object.Destroy(this.gameObject);
		}

		int aoe_range = (aoe_size - 1) / 2;
		for(int i = 0; i < aoe_size; i++)
		{
			for (int j = 0; j < aoe_size; j++)
			{
				GameObject go = Instantiate(single_bullet, target.transform.position + Vector3.up * 5.0f, Quaternion.identity) as GameObject;
				go.transform.SetParent(this.transform);
				bulletlist.Add(go);
				target_offset_list.Add(new Vector3Int(i-aoe_range, 0, j-aoe_range));
				print(bulletlist[bulletlist.Count-1].transform.position);
			}
		}
	}	
	
	// Update is called once per frame
	void Update () {

		life_time -= Time.deltaTime;
		if(life_time < 0)
		{
			Object.Destroy(this.gameObject);
		}

		if(target == null)
		{
			// Object.Destroy(this.gameObject);
		}
		else
		{
			for (int i = 0; i < bulletlist.Count; i++)
			{
				if(bulletlist[i] != null)
				{
					bulletlist[i].transform.position = Vector3.MoveTowards(bulletlist[i].transform.position, target_pos - Vector3.up*1.0f + target_offset_list[i], vel * Time.deltaTime);
					Chessman surrounding_target = null;
					Debug.Log(target_x + " : " + target_offset_list[i].x + " : " + target_y + " : " + target_offset_list[i].z);
					if(target_x + target_offset_list[i].x >=0 && target_x + target_offset_list[i].x <=7
							&& target_y + target_offset_list[i].z >=0 && target_y + target_offset_list[i].z <=7)
					{
						surrounding_target = BoardManager.Instance.Chessmans[target_x + target_offset_list[i].x, target_y + target_offset_list[i].z];
					}
					
					if(surrounding_target == null)
					{
						// pass, only have anim
					}
					else
					{
						if(Vector3.Distance(bulletlist[i].transform.position, surrounding_target.transform.position) < 0.25f)
						{
							if(isHeal)
							{
								surrounding_target.GetHeal(this.damage);
							}
							else
							{
								surrounding_target.GetDamage(this.damage, 1);
							}
							Object.Destroy(bulletlist[i]);
							bulletlist[i] = null;
						}
					}
				}
				
				
			}
			
		}
	}
}
