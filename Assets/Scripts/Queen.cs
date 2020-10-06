using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Chessman
{
    private Animator anim;
    private Chessman enemy_target;
    public GameObject bullet;
    public override void InitInfo()
    {
        this.maxhp = 15;
        this.hp = 15;
        this.damage = 10;
        this.cost = 5;
        enemy_target = null;
    }
    public override void GetInfo(ref string r_id, ref int r_hp, ref int r_maxhp, ref string r_at, ref int r_damage, ref int r_cost)
    {
        r_id = "Queen";
        r_hp = this.hp;
        r_maxhp = this.maxhp;
        r_at = "Range\n(Burst)";
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
        yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
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
        anim.SetInteger("to_attack", 1);

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

        Chessman c;

        // Rook
        for (int i = CurrentX + 1; i < 8; i++)
        {
            if (i >= 8)
                break;
            c = BoardManager.Instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }
        for (int i = CurrentX - 1; i >= 0; i--)
        {
            if (i < 0)
                break;
            c = BoardManager.Instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }
        for (int i = CurrentY + 1; i < 8; i++)
        {
            if (i >= 8)
                break;
            c = BoardManager.Instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }
        for (int i = CurrentY - 1; i >= 0; i--)
        {
            if (i < 0)
                break;
            c = BoardManager.Instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (c.isWhite != isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        //Bishop
        for (int i = CurrentX + 1, j = CurrentY + 1; i < 8; i++, j++)
        {
            if (i >= 8 || j >= 8)
                break;
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
                break;
            }
        }
        for (int i = CurrentX + 1, j = CurrentY - 1; i < 8; i++, j--)
        {
            if (i >= 8 || j < 0)
                break;
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
                break;
            }
        }

        for (int i = CurrentX - 1, j = CurrentY + 1; i >= 0; i--, j++)
        {
            if (i < 0 || j >= 8)
                break;
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
                break;
            }
        }

        for (int i = CurrentX - 1, j = CurrentY - 1; i >= 0; i--, j--)
        {
            if (i < 0 || j < 0)
                break;
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
                break;
            }
        }


        return r;
    }

}
