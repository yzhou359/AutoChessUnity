using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{
    private Animator anim;
    private Chessman enemy_target;
    public GameObject bullet;
    public override void InitInfo()
    {
        this.maxhp = 20;
        this.hp = 20;
        this.damage = 10;
        this.cost = 6;
        enemy_target = null;
    }
    public override void GetInfo(ref string r_id, ref int r_hp, ref int r_maxhp, ref string r_at, ref int r_damage, ref int r_cost)
    {
        r_id = "King";
        r_hp = this.hp;
        r_maxhp = this.maxhp;
        r_at = "Range\n(AOE)";
        r_damage = this.damage;
        r_cost = this.cost;

        this.hp = r_hp;
        this.damage = r_damage;
        this.cost = r_cost;
    }

    public override void AIAction()
    {
        RealtimeAIAction();
        if(this.inAction != true)
            StartCoroutine(SleepForAWhile());
    }

    IEnumerator SleepForAWhile()
    {
        this.inAction = true;
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        DelayedAIAction();
    }

    public void RealtimeAIAction()
    {
        // Check HP
        if(this.hp <= 0)
        {
            BoardManager.Instance.Chessmans[CurrentX, CurrentY] = null;
            Debug.Log("I'M DEAD.");
            BoardManager.Instance.activechessman.Remove(this.gameObject);
            if(this.isWhite)
            {
                BoardManager.Instance.activechessman_white.Remove(this.gameObject);
            }
            else
            {
                BoardManager.Instance.activechessman_black.Remove(this.gameObject);
            }
            Object.Destroy(this.transform.parent.gameObject);
        }

        // Check to end phase
        if(this.isWhite && BoardManager.Instance.activechessman_black.Count == 0)
        {
            this.come_to_end_phase = true;
        }
        if(!this.isWhite && BoardManager.Instance.activechessman_white.Count == 0)
        {
            this.come_to_end_phase = true;
        }
    }
    public void DelayedAIAction()
    {
        Chessman c;
        List<GameObject> active_enemies = null;
 
        if(this.isWhite)
        {
            active_enemies = BoardManager.Instance.activechessman_black;
        }
        else
        {
            active_enemies = BoardManager.Instance.activechessman_white;
        }

        if(active_enemies.Count > 0)
        {
            int random_enemy_idx = Random.Range(0, active_enemies.Count);
            c = active_enemies[random_enemy_idx].GetComponent<Chessman>();
            if(c != null)
            {
                enemy_target = c;
            }
        }

        if(enemy_target != null)
        {
            // attack
            Debug.Log("Attack!!");
            Attack();
        } 
        else
        {
            inAction = false;
            return;
        }     
    }

    private void Attack()
    {
        anim = gameObject.GetComponent<Animator>();
        if(isWhite)
        {
            anim.SetInteger("to_attack", 1);
        }
        else
        {
            anim.SetInteger("to_attack", -1);
        }
    }

    private void RealAttack()
    {
        // launch bullet
        GameObject go = Instantiate(bullet, transform.position, Quaternion.identity);
        BulletAOE b = go.GetComponent<BulletAOE>();
        b.damage = this.damage;
        b.target = enemy_target;
        b.isHeal = false;
        b.vel = 5.0f;
        anim.SetInteger("to_attack", 0);
        

        // play anim
        GameObject book = this.transform.Find("book_low_sketch").gameObject;
        ParticleSystem attack_particle = book.transform.Find("flash").GetComponent<ParticleSystem>();
        attack_particle.Emit(5);
        
    }
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];
        Chessman c;

        for (int i = CurrentX - 1; i <= CurrentX + 1; i++)
        {
            for (int j = CurrentY - 1; j <= CurrentY + 1; j++)
            {
                if (i < 0 || i >= 8 || j < 0 || j >= 8)
                    continue;

                c = BoardManager.Instance.Chessmans[i, j];
                if (c == null)
                {
                    r[i, j] = true;
                }
                else
                {
                    if (c.isWhite != isWhite)
                    {
                        r[i, j] = true;
                    }
                }
            }
        }

        return r;

    }
}
