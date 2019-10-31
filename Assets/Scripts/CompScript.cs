using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CompScript : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D companionbody;
    public GameObject comphealthbar;
    public float speed = 10;
    public float jumpForce = 16.5f;
    public float jumps = 1;
    public float comphealth = 100;

    void Start()
    {

    }

    void Update()
    {
        if (this.gameObject.transform.position.y > Player.transform.position.y - 0.6f)
        {
            if (this.gameObject.transform.position.x + 3 < Player.transform.position.x)
            {
                companionbody.velocity = new Vector2(1, companionbody.velocity.y / speed) * speed;
                comphealth -= 3 * Time.deltaTime;
            }
            else if (this.gameObject.transform.position.x - 3 > Player.transform.position.x)
            {
                companionbody.velocity = new Vector2(-1, companionbody.velocity.y / speed) * speed;
                comphealth -= 3 * Time.deltaTime;
            }
            else
            {
                companionbody.velocity = new Vector2(0, companionbody.velocity.y / speed) * speed;
            }
        }
        else
        {
            StartCoroutine(Jump());
            if (this.gameObject.transform.position.x < Player.transform.position.x - 0.05f)
            {
                companionbody.velocity = new Vector2(1, companionbody.velocity.y / speed) * speed;
                comphealth -= 3 * Time.deltaTime;
            }
            else if (this.gameObject.transform.position.x > Player.transform.position.x + 0.05f)
            {
                companionbody.velocity = new Vector2(-1, companionbody.velocity.y / speed) * speed;
                comphealth -= 3 * Time.deltaTime;
            }
            else
            {
                companionbody.velocity = new Vector2(0, companionbody.velocity.y / speed) * speed;
            }
        }

        if (jumps == 0)
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

        comphealthbar.GetComponent<RectTransform>().transform.localScale = new Vector3(comphealth / 100, 0.1f, 1);
        comphealthbar.transform.position = new Vector2(this.gameObject.transform.position.x - 1, this.gameObject.transform.position.y + 1.5f);
    }

    IEnumerator Jump()
    {
        if (jumps == 1)
        {
            jumps = 0;
            yield return new WaitForSeconds(0.3f);
            companionbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            jumps = 1;
        }
    }
}