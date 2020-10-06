using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pawn : Chessman {

    private Animator anim;
    private Chessman enemy_target;

    private float attack_rate = 0.0f;

    public override void InitInfo()
    {
        this.maxhp = 15;
        this.hp = 15;
        this.damage = 1;
        this.cost = 1;
    }
    public override void GetInfo(ref string r_id, ref int r_hp, ref int r_maxhp, ref string r_at, ref int r_damage, ref int r_cost)
    {
        r_id = "Pawn";
        r_hp = this.hp;
        r_maxhp = this.maxhp;
        r_at = "Melee\n(Basic)";
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
        yield return new WaitForSeconds(Random.Range(0.1f+attack_rate, 1.0f+attack_rate));
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
        if(this.isWhite && this.CurrentY == 7)
        {
            this.come_to_end_phase = true;
        }
        if(!this.isWhite && this.CurrentY == 0)
        {
            this.come_to_end_phase = true;
        }

    }
    public void DelayedAIAction()
    {
        Chessman c;
        int bw_edge, bw_move_dir;
        if(this.isWhite)
        {
            bw_edge = 7;
            bw_move_dir = 1;
        }
        else
        {
            bw_edge = 0;
            bw_move_dir = -1;
        }
        
        // Check MOVE / ATTACK
        if (CurrentY != bw_edge) // touch the end?
        {
            // check the spot in front of this
            c = BoardManager.Instance.Chessmans[CurrentX, CurrentY + bw_move_dir];
            if (c == null)
            {
                Move();
            }
            else
            {
                if(c.isWhite == this.isWhite) //ally
                {
                    inAction = false;
                    return;
                }
                else //enemy
                {
                    enemy_target = c;
                    Attack();
                }   

            }
        }
        else // touch the end
        {
            // if enemy, then attack player directly
            if (this.isWhite == false)
            {
                attack_rate = 3.0f;
                this.transform.parent.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                enemy_target = null;
                Debug.Log("ATTACK player!!");
                Attack();
            }
            else
            {
                // if player, stay idle
                inAction = false;
            }
            return;
        }

        
        
    }

    private void Move()
    {
        // update BoardManager.Chessmans
        if(this.isWhite)
        {
            BoardManager.Instance.Chessmans[CurrentX, CurrentY + 1] = BoardManager.Instance.Chessmans[CurrentX, CurrentY];  
            BoardManager.Instance.Chessmans[CurrentX, CurrentY] = null;
            this.SetPosition(CurrentX, CurrentY + 1);
        }
        else
        {
            BoardManager.Instance.Chessmans[CurrentX, CurrentY - 1] = BoardManager.Instance.Chessmans[CurrentX, CurrentY];  
            BoardManager.Instance.Chessmans[CurrentX, CurrentY] = null;
            this.SetPosition(CurrentX, CurrentY - 1);
        }
        

        // play anim
        anim = gameObject.GetComponent<Animator>();
        if(this.isWhite)
            anim.SetInteger("to_move", 1);
        else
            anim.SetInteger("to_move", -1);

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

    private void backtoIdle()
    {
        // rest all trans conditions
        anim.SetInteger("to_move", 0);
        anim.SetInteger("to_attack", 0);
    }

    private void updatePositionAfterMove()
    {
        // update parent empty go transform.position
        this.transform.parent.transform.position = this.transform.position;
    }

    
    private void meleeAttack()
    {
        if(enemy_target != null)
        {
            enemy_target.hp -= this.damage;
            Debug.Log(enemy_target.hp);
            // enemy_target.AIAction();
        }
        else
        {
            // no enemy ?
            if(CurrentY == 0 && !this.isWhite)
            {
                BoardManager.Instance.player_hp -= 1;
                this.hp -= 1;
                BloodyScreenFlashScript.Instance.BloodySceenFlash(1);
            }
        }
        anim.SetInteger("to_attack", 0);
    }


    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        Chessman c, c2;

        // white team
        if (isWhite)
        {
            // Diagonal left
            if(CurrentX != 0 && CurrentY != 7)
            {
                c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY + 1];
                if(c != null && !c.isWhite)
                {
                    r[CurrentX - 1, CurrentY + 1] = true;
                }
            }
            // Diagonal right
            if (CurrentX != 7 && CurrentY != 7)
            {
                c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                {
                    r[CurrentX + 1, CurrentY + 1] = true;
                }
            }
            // Middle
            if (CurrentY !=7)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 1];
                if (c == null)
                {
                    r[CurrentX, CurrentY + 1] = true;
                }
            }
            // Middle x 2
            if (CurrentY == 1)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 1];
                c2 = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 2];
                if(c == null && c2 == null)
                {
                    r[CurrentX, CurrentY + 2] = true;
                }
            }
        }
        else
        {
            // Diagonal left
            if (CurrentX != 0 && CurrentY != 0)
            {
                c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    r[CurrentX - 1, CurrentY - 1] = true;
                }
            }
            // Diagonal right
            if (CurrentX != 7 && CurrentY != 0)
            {
                c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    r[CurrentX + 1, CurrentY - 1] = true;
                }
            }
            // Middle
            if (CurrentY != 0)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 1];
                if (c == null)
                {
                    r[CurrentX, CurrentY - 1] = true;
                }
            }
            // Middle x 2
            if (CurrentY == 6)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 1];
                c2 = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 2];
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY - 2] = true;
                }
            }
        }

        return r;
    }
}
