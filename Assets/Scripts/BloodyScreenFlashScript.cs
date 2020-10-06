using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyScreenFlashScript : MonoBehaviour {

	public static BloodyScreenFlashScript Instance { set; get; }
	// Use this for initialization
	void Start () {
		Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void BloodySceenFlash(int to_flash)
	{
		Animator anim = this.GetComponent<Animator>();
		anim.SetInteger("to_flash", to_flash);
	}
}
