using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour 
{
    
    public Animator animator;

    float InputX;
    public float InputY;

    int Clicks;
    public int maxJumps = 2;
    public int Jumped = 0;
    bool canClick;

    // Use this for initialization
    void Start () 
    {
        // Get the animator
        animator = this.gameObject.GetComponent<Animator>();

        Clicks = 0;
        canClick = true;

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            print("Combo Started");
            ComboStarter();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoubleJump();
        }



        //else //(Input.GetMouseButtonUp (0))
        //{
        //    print("Back to Idle");
        //    animator.SetInteger("Condition", 0);
        //}

        InputY = Input.GetAxis("Vertical");
        InputX = Input.GetAxis("Horizontal");
        animator.SetFloat("InputX", InputX);
        animator.SetFloat("InputY", InputY);
    }

    void ComboStarter()
    {
        if (canClick)
        {
            Clicks++;
        }

        if (Clicks == 1)
        {
            animator.SetInteger("Condition", 1);
        }
    }

    public void ComboCheck()
    {
        canClick = false;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") && Clicks == 1)
        {
            animator.SetInteger("Condition", 0);
            canClick = true;
            Clicks = 0;
        }

        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") && Clicks >= 2)
        {
            animator.SetInteger("Condition", 2);
            canClick = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") && Clicks == 2)
        {
            animator.SetInteger("Condition", 0);
            canClick = true;
            Clicks = 0;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") && Clicks >= 3)
        {
            animator.SetInteger("Condition", 3);
            canClick = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 3"))
        {
            animator.SetInteger("Condition", 0);
            canClick = true;
            Clicks = 0;
        }
    }

    public void DoubleJump()
    {

        if (Jumped <= maxJumps)
        {
            GoUp();
        }
        else
        {
            return;
        }

        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("1st Jump"))
        //{
        //    animator.SetInteger("Condition", 4);
        //    canClick = true;
        //    Clicks = 0;
        //}
        //else if (animator.GetCurrentAnimatorStateInfo(0).IsName("2nd Jump") && Clicks >= 2)
        //{
        //    animator.SetInteger("Condition", 0);
        //    canClick = true;
        //}

        //else if (animator.GetCurrentAnimatorStateInfo(0).IsName("2nd Jump"))
        //{
        //    animator.SetInteger("Condition", 5);
        //    canClick = true;
        //    Clicks = 0;
        //}
        //else if (animator.GetCurrentAnimatorStateInfo(0).IsName("2nd Jump") && Clicks >= 2)
        //{
        //    animator.SetInteger("Condition", 0);
        //    canClick = true;
        //}
    }

    void GoUp()
    {
        Jumped += 1;
    }
}