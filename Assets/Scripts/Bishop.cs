using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman
{
    private Animator anim;
    private Chessman enemy_target;
    public GameObject bullet;
    public override void InitInfo()
    {
        this.maxhp = 10;
        this.hp = 10;
        this.damage = 9;
        this.cost = 4;
        enemy_target = null;
    }
    public override void GetInfo(ref string r_id, ref int r_hp, ref int r_maxhp, ref string r_at, ref int r_damage, ref int r_cost)
    {
        r_id = "Bishop";
        r_hp = this.hp;
        r_maxhp = this.maxhp;
        r_at = "Heal\n(Sup)";
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
        yield return new WaitForSeconds(Random.Range(0.1f, 1.0f));
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
        this.come_to_end_phase = true;
    }
    public void DelayedAIAction()
    {
        Chessman c;
        List<GameObject> active_enemies = null;
 
        if(this.isWhite)
        {
            active_enemies = BoardManager.Instance.activechessman_white;
        }
        else
        {
            active_enemies = BoardManager.Instance.activechessman_black;
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
            Debug.Log("HEAL!!");
            Heal();
        } 
        else
        {
            inAction = false;
            return;
        }     
    }

    private void Heal()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetInteger("to_heal", 1);
    }

    private void RealHeal()
    {
        // play anim
        ParticleSystem heal_particle = this.transform.Find("Heal_anim").GetComponent<ParticleSystem>();
        heal_particle.Emit(40);

        // launch bullet
        GameObject go = Instantiate(bullet, transform.position, Quaternion.identity);
        Bullet b = go.GetComponent<Bullet>();
        b.damage = this.damage;
        b.target = enemy_target;
        b.isHeal = true;
        b.vel = 100.0f;
        anim.SetInteger("to_heal", 0);
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        Chessman c;
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
