using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScript : MonoBehaviour
{
    public Rigidbody2D playerbody;
    public float speed;
    public float jumpForce;
    float jumps;
    public static int PlayerState;

    GameObject NPC;
    
    // Start is called before the first frame update
    void Start()
    {
        jumps = 1;
        speed = 10;
        jumpForce = 16.5f;
        PlayerState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(PlayerState);
        if (PlayerState != 2 && PlayerState != 3)
        {
            playerbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), playerbody.velocity.y / speed) * speed;

            if (Input.GetKeyDown(KeyCode.UpArrow) && jumps > 0)
            {
                jumps = 0;
                playerbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.localScale = new Vector3(1, 3, 1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.localScale = new Vector3(-1, 3, 1);
            }
        }
        else
        {
            playerbody.velocity = new Vector2(0, playerbody.velocity.y / speed) * speed;
        }

        if (PlayerState == 1 && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerState = 2;
        }

        if (PlayerState == 2)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                //talk here
                PlayerState = 3;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                //absorb life here
                PlayerState = 3;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //give life here
                PlayerState = 3;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //cancel
                PlayerState = 1;
            }
        }

        if (PlayerState == 3 && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerState = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Ground" && col.gameObject.transform.position.y < this.gameObject.transform.position.y - 1.9f)
        {
            jumps = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPC" && PlayerState == 0)
        {
            PlayerState = 1;
            NPC = col.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "NPC" && PlayerState == 0)
        {
            PlayerState = 1;
            NPC = col.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPC" && PlayerState == 1)
        {
            PlayerState = 0;
        }
    }
}
