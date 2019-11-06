using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;

public class NPCScript : MonoBehaviour
{
    public bool alive; //took life, give life
    public bool sick;
    bool InRange;

    //NarrationSpeeches
    public Narration regularSpeech;
    public Narration aliveGiveLifeSpeech;
    public Narration aliveTakeLifeSpeech;
    public Narration deadGiveLifeSpeech;

    //NarrationConditions
    public string npcSituation;
    public GameObject conditionedByNPC; //the npc which conditions this npc (ex: mother conditions son)


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

        if (InRange && alive && Input.GetKeyDown(KeyCode.UpArrow) && CharScript.PlayerState == 2)
        {
            //talk
            CharScript.PlayerState = 3;
            NarrationManager.instance.PlayNarration(regularSpeech);
        }

        if (InRange && alive && Input.GetKeyDown(KeyCode.LeftArrow) && CharScript.PlayerState == 2)
        {
            CharScript.reservehealth += 25;
            if (sick == false)
            {
                CharScript.reservehealth += 25;
            }
            alive = false;
            //absorb
            CharScript.PlayerState = 3;
            NarrationManager.instance.PlayNarration(aliveTakeLifeSpeech);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
        }

        if (InRange && Input.GetKeyDown(KeyCode.RightArrow) && CharScript.PlayerState == 2 && CharScript.reservehealth >= 50)
        {
            CharScript.reservehealth -= 50;
            if(alive)
            {
                NarrationManager.instance.PlayNarration(aliveGiveLifeSpeech);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
                NarrationManager.instance.PlayNarration(deadGiveLifeSpeech);
                alive = true;
            }
            //grant
            CharScript.PlayerState = 3;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }


        //NarrationCases (we may customize any npc speech to our liking here)
        switch (npcSituation)
        {
            case "Mother":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if son is alive
                {
                    regularSpeech.phrases[0].text = "Please, help my son! Food is nowhere to be found, he is very sick! I offer my life if you must!";
                    deadGiveLifeSpeech.phrases[0].text = "You've reunited me with my son once more.. Thank you.";
                }
                else //if he is dead
                {
                    regularSpeech.phrases[0].text = "My son! What have you done?!";
                    deadGiveLifeSpeech.phrases[0].text = "What is the point of life when it has rid you of all you love...";
                }
                break;

            case "Son":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if mother is alive
                {
                    regularSpeech.phrases[0].text = "I am very sick... My mom keeps saying it's because I got a flu... but they're usually not this painful.";
                    deadGiveLifeSpeech.phrases[0].text = "Mother.. I missed her very much. The sickness is still roaming around.";
                }
                else //if mother is dead
                {
                    regularSpeech.phrases[0].text = "Oh no, mom! I have nobody left!";
                    deadGiveLifeSpeech.phrases[0].text = "You've brought me back to life.. It's meaningless without my mom.";
                }
                break;

            default:
                break;
        }

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            InRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && CharScript.PlayerState != 2)
        {
            InRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            InRange = false;
        }
    }
}
