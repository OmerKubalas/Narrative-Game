using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public bool alive; //took life, give life
    public bool sick;
    bool InRange;
    int NPCState;
    public string[] dialogue;
    public GameObject spacePopup, OptionsPopup;

    // Start is called before the first frame update
    void Start()
    {
        NPCState = 0;
        if (sick)
        {
            GetComponent<SpriteRenderer>().color = new Color(120, 215, 70);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

        if (alive)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x, transform.position.y-1, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && InRange)
        {
            spacePopup.transform.position = new Vector2(999, 999);
            if (alive)
            {
                OptionsPopup.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                OptionsPopup.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            }
            else
            {
                OptionsPopup.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
                OptionsPopup.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            }
            OptionsPopup.transform.position = new Vector2(transform.position.x, transform.position.y + 4);
            NPCState = 1;
        }
        if (NPCState == 1 && alive && Input.GetKeyDown(KeyCode.UpArrow))
        {
            //talk
            OptionsPopup.transform.position = new Vector2(999, 999);
            NPCState = 0;
        }
        if (NPCState == 1 && alive && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            alive = false;
            //absorb
            OptionsPopup.transform.position = new Vector2(999, 999);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
            NPCState = 0;
        }
        if (NPCState == 1 && Input.GetKeyDown(KeyCode.RightArrow))
        {
            alive = true;
            //grant
            OptionsPopup.transform.position = new Vector2(999, 999);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            transform.position = new Vector3(transform.position.x, transform.position.y+1, 0);
            NPCState = 0;
        }
        if (NPCState == 1 && Input.GetKeyDown(KeyCode.DownArrow))
        {
            //cancel
            OptionsPopup.transform.position = new Vector2(999, 999);
            NPCState = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spacePopup.transform.position = new Vector2(transform.position.x, transform.position.y + 2);
            InRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spacePopup.transform.position = new Vector2(999, 999);
            InRange = false;
        }
    }
}
