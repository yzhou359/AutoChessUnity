using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chessman : MonoBehaviour {

    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isWhite;
    public bool inAction;
    public ParticleSystem range_attack_particle;
    public ParticleSystem heal_particle;

    private void Start() {
        range_attack_particle = this.transform.Find("Sparks").GetComponent<ParticleSystem>();
        heal_particle = this.transform.Find("Heal").GetComponent<ParticleSystem>();
    }

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool [8,8];
    }

    public string chess_id = "default";
    public int hp = 100;
    public int maxhp = 100;
    public string attack_type = "Melee";
    public int damage = 0;
    public int cost = 0;
    public bool come_to_end_phase = false;
    public virtual void GetInfo(ref string r_id, ref int r_hp, ref int r_maxhp, ref string r_at, ref int r_damage, ref int r_cost)
    {}
    public virtual void InitInfo()
    {}

    public virtual void AIAction()
    {}

    public void GetHeal(int damage)
    {
        this.hp += damage;
        if(this.hp > this.maxhp)
            this.hp = this.maxhp;
        heal_particle.Emit(40);
    }

    public void GetDamage(int damage, int attack_type)
    {
        if(attack_type == 1)
        {
            // range attack
            range_attack_particle.Emit(30);
        }
        
        this.hp -= damage;
        // Debug.Log(this.hp);
    }

    private void deactiveChessman()
    {
        inAction = false;
    }

}
