using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CompScript : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D companionbody;
    public GameObject comphealthbar;
    public float speed = 10;
    public float jumpForce = 16.5f;
    public float jumps = 1;
    public static float comphealth = 100;
    public int companionstate = 1;

    //Animations
    int CompAnimationState;
    Animator anim;

    void Start()
    {
        comphealth = 100;
        jumps = 1;

        anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (companionbody.velocity.x == 0 && companionbody.velocity.y == 0)
        {
            CompAnimationState = 0; //idle anim
        }

        if (companionstate == 1)
        {
            MoveNearPlayer();
            
            if (this.gameObject.transform.position.y < Player.transform.position.y - 0.6f && jumps == 1)
            {
                StartCoroutine(Jump());
            }
            if (jumps == -1)
            {
                MoveToPlayer();
            }
            if (this.gameObject.transform.position.y > Player.transform.position.y - 0.4f)
            {
                MoveToPlayer();
            }

            if (this.gameObject.transform.position.y > Player.transform.position.y + 7 || this.gameObject.transform.position.y < Player.transform.position.y - 7)
            {
                this.gameObject.transform.position = Player.transform.position;
            }
            if (this.gameObject.transform.position.x > Player.transform.position.x + 5 || this.gameObject.transform.position.x < Player.transform.position.x - 5)
            {
                this.gameObject.transform.position = Player.transform.position;
            }
        }

        comphealthbar.GetComponent<RectTransform>().transform.localScale = new Vector3(comphealth / 100, 0.1f, 1);
        comphealthbar.transform.position = new Vector2(this.gameObject.transform.position.x - 1, this.gameObject.transform.position.y + 1.5f);

        if (comphealth > 100)
        {
            comphealth = 100;
        }
        if (comphealth <= 0)
        {
            comphealth = 0.000001f; //(if set as 0, this if case will keep looping)
            companionbody.velocity = new Vector2(0, companionbody.velocity.y / speed) * speed;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, 0);
            companionstate = 0;
            CharScript.sanity = 0;
            CompAnimationState = 3; //dead anim
        }

        SetAnimationState();
    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.3f);
        if (jumps == 1)
        {
            CompAnimationState = 2; //jump anim
            companionbody.velocity = new Vector2(companionbody.velocity.x, 0);
            companionbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumps = 0;
        }
    }

    void MoveNearPlayer()
    {
        if (this.gameObject.transform.position.x + 3 < Player.transform.position.x)
        {
            companionbody.velocity = new Vector2(1, companionbody.velocity.y / speed) * speed;
            comphealth -= 3 * Time.deltaTime;
            CompAnimationState = 1; //walk anim
        }
        else if (this.gameObject.transform.position.x - 3 > Player.transform.position.x)
        {
            companionbody.velocity = new Vector2(-1, companionbody.velocity.y / speed) * speed;
            comphealth -= 3 * Time.deltaTime;
            CompAnimationState = 1; //walk anim
        }
        else
        {
            companionbody.velocity = new Vector2(0, companionbody.velocity.y / speed) * speed;
        }
    }

    void MoveToPlayer() //while jumping
    {
        if (this.gameObject.transform.position.x < Player.transform.position.x - 0.05f)
        {
            companionbody.velocity = new Vector2(1, companionbody.velocity.y / speed) * speed;
            comphealth -= 1 * Time.deltaTime;
        }
        else if (this.gameObject.transform.position.x > Player.transform.position.x + 0.05f)
        {
            companionbody.velocity = new Vector2(-1, companionbody.velocity.y / speed) * speed;
            comphealth -= 1 * Time.deltaTime;
        }
        else
        {
            companionbody.velocity = new Vector2(0, companionbody.velocity.y / speed) * speed;
        }
    }

    void SetAnimationState()
    {
        switch (CompAnimationState)
        {
            case 0:
                anim.SetInteger("CompAnimationState", 0); //idle
                break;

            case 1:
                anim.SetInteger("CompAnimationState", 1); //walk
                break;

            case 2:
                anim.SetInteger("CompAnimationState", 2); //jump
                break;

            case 3:
                anim.SetInteger("CompAnimationState", 3); //dead
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground" && col.gameObject.transform.position.y < this.gameObject.transform.position.y - 1.3f)
        {
            jumps = 1;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground" && col.gameObject.transform.position.y < this.gameObject.transform.position.y - 1.3f)
        {
            jumps = 1;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            jumps = -1;
        }
    }
}