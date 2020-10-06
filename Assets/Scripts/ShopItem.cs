using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
	public static ShopItem Instance { set; get; }
	public int cost = 0;
	private int[] cost_list = {6, 5, 3, 4, 2, 1};
	private int[] rand_int_to_chess_id_map = {5, 4, 2, 3, 1, 0};
	public int chess_id = -1;
	public Sprite[] texture_sprite;
	public GameObject corresponding_cost_text;

	public GameObject[] sibling_items;


	// Use this for initialization
	void Start () {
		Instance = this;
		this.RandomLoadNewItem(1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RandomLoadNewItem(int round)
	{
		this.chess_id = rand_int_to_chess_id_map[Random.Range(0, Mathf.Min(6, round*2))];
		this.GetComponent<Image>().sprite = texture_sprite[this.chess_id];
		this.cost = this.cost_list[this.chess_id];
		corresponding_cost_text.GetComponent<Text>().text = this.cost.ToString();
	}

	public void RefreshItems()
	{
		if(BoardManager.Instance.player_money < 2)
		{
			return;
		}
		BoardManager.Instance.player_money -= 2;

		this.RandomLoadNewItem(BoardManager.Instance.round);
		for(int i=0; i<2; i++)
		{
			sibling_items[i].GetComponent<ShopItem>().RandomLoadNewItem(BoardManager.Instance.round);
		}

	}

	public void PurchaseCurrentItem()
	{
		if(this.chess_id >=6 || this.chess_id <0)
		{
			return;
		}

		if(BoardManager.Instance.player_money < this.cost)
		{
			return;
		}

		BoardManager.Instance.player_money -= this.cost;
		ButtonManagerScript.Instance.inventory_chessman_num[chess_id] += 1;
		ButtonManagerScript.Instance.inventory_chessman_total_num[chess_id] += 1;
		this.chess_id = 6;
		this.GetComponent<Image>().sprite = texture_sprite[this.chess_id];
		this.cost = 0;
		corresponding_cost_text.GetComponent<Text>().text = "X";
	}
}
