using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScript : MonoBehaviour
{
    public Rigidbody2D playerbody;
    public float speed;
    public float jumpForce;
    float jumps;
    int PlayerState;

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
        if (PlayerState != 2)
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

        if (PlayerState == 2)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                //talk here
                PlayerState = 0;
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow) && (NPC.GetComponent<NPCScript>().alive || !NPC.GetComponent<NPCScript>().alive))
            {
                //absorb life here
                PlayerState = 0;
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                //give life here
                PlayerState = 0;
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                //cancel
                PlayerState = 0;
            }
        }

        if (PlayerState == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            PlayerState = 2;
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
