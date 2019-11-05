using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;

public class NPCScript : MonoBehaviour
{
    public bool alive; //took life, give life
    public bool sick;
    bool InRange;
    public GameObject spacePopup, OptionsPopup;

    //NarrationSpeeches
    public Narration regularSpeech;
    public Narration specialConditionSpeech;
    public Narration aliveGiveLifeSpeech;
    public Narration aliveTakeLifeSpeech;
    public Narration deadGiveLifeSpeech;

    // Start is called before the first frame update
    void Start()
    {
        if (sick)
        {
            GetComponent<SpriteRenderer>().color = new Color32(120, 215, 70, 255);
        }
        else
        { 
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
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
        if(InRange && CharScript.PlayerState == 1)
        {
            spacePopup.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 2);
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
            OptionsPopup.transform.position = new Vector2(999,999);
        }
        if (InRange && CharScript.PlayerState == 2)
        {
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
            spacePopup.transform.position = new Vector2(999, 999);
            OptionsPopup.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 4);
        }

        if (alive && Input.GetKeyDown(KeyCode.UpArrow) && CharScript.PlayerState == 2)
        {
            //talk
            NarrationManager.instance.PlayNarration(regularSpeech);
            OptionsPopup.transform.position = new Vector2(999, 999);
        }

        if (alive && Input.GetKeyDown(KeyCode.LeftArrow) && CharScript.PlayerState == 2)
        {
            alive = false;
            //absorb
            NarrationManager.instance.PlayNarration(aliveTakeLifeSpeech);
            OptionsPopup.transform.position = new Vector2(999, 999);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && CharScript.PlayerState == 2)
        {
            if(alive != true)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
                NarrationManager.instance.PlayNarration(deadGiveLifeSpeech);
            }
            else
            {
                NarrationManager.instance.PlayNarration(aliveGiveLifeSpeech);
            }

            alive = true;
            //grant
            OptionsPopup.transform.position = new Vector2(999, 999);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && CharScript.PlayerState == 2)
        {
            //cancel
            OptionsPopup.transform.position = new Vector2(999, 999);
        }

        if (InRange && !NarrationManager.instance.isPlaying && CharScript.PlayerState == 1)
        {
            spacePopup.transform.position = new Vector2(transform.position.x, transform.position.y + 2);
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

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && CharScript.PlayerState != 2)
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
