using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melee_attack_sound : MonoBehaviour {

    int game_phase = 0;

    private AudioSource bgm;

    // Use this for initialization
    void Start () {
        bgm = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (game_phase == 0)
        {
            bgm.Play();
            game_phase = 1;
        }
            
    }
}
