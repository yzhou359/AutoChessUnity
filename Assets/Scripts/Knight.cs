using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
    private Animator anim;
    private Chessman enemy_target;
    public GameObject bullet;
    public override void InitInfo()
    {
        this.maxhp = 10;
        this.hp = 10;
        this.damage = 1;
        this.cost = 2;
        enemy_target = null;
    }
    public override void GetInfo(ref string r_id, ref int r_hp, ref int r_maxhp, ref string r_at, ref int r_damage, ref int r_cost)
    {
        r_id = "Knight";
        r_hp = this.hp;
        r_maxhp = this.maxhp;
        r_at = "Range\n(Basic)";
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
        yield return new WaitForSeconds(Random.Range(0.01f, 0.3f));
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
            Debug.Log("ATTACK!!");
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
        // play anim
        anim = gameObject.GetComponent<Animator>();
        if(this.isWhite)
            anim.SetInteger("to_attack", 1);
        else
            anim.SetInteger("to_attack", -1);
    }

    private void RangeAttack()
    {
        // launch bullet
        GameObject go = Instantiate(bullet, transform.position, Quaternion.identity);
        Bullet b = go.GetComponent<Bullet>();
        b.damage = this.damage;
        b.target = enemy_target;
        b.isHeal = false;
        b.vel = 10.0f;
        anim = gameObject.GetComponent<Animator>();
        anim.SetInteger("to_attack", 0);
    }


    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        KnightMove(CurrentX - 1, CurrentY + 2, ref r);
        KnightMove(CurrentX - 1, CurrentY - 2, ref r);
        KnightMove(CurrentX + 1, CurrentY + 2, ref r);
        KnightMove(CurrentX + 1, CurrentY - 2, ref r);

        KnightMove(CurrentX - 2, CurrentY + 1, ref r);
        KnightMove(CurrentX - 2, CurrentY - 1, ref r);
        KnightMove(CurrentX + 2, CurrentY + 1, ref r);
        KnightMove(CurrentX + 2, CurrentY - 1, ref r);

        return r;
    }

    public void KnightMove(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if(x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BoardManager.Instance.Chessmans[x, y];
            if (c == null)
            {
                r[x, y] = true;
            }
            else if(isWhite != c.isWhite)
            {
                r[x, y] = true;
            }
        }
    }

}
