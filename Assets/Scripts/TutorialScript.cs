using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour {
	public static TutorialScript Instance { set; get; }
	public Sprite[] pages;
	public GameObject pages_go;
	public GameObject page_num_go;
	public int current_page_idx = 0;
	public int total_page_num = 16;

	private void Start() {
		Instance = this;
	}

	private void Update() {
		pages_go.GetComponent<Image>().sprite = pages[current_page_idx];
		page_num_go.GetComponent<Text>().text = "Page (" + (current_page_idx+1).ToString() + "/" + total_page_num.ToString() + ")";
	}

}
