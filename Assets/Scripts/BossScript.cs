using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;

public class BossScript : MonoBehaviour
{
    public bool alive;
    bool InRange;

    static public bool alchemistsHelped, alchemistsAlive;

    public GameObject AlchemistChief, Alchemist1, Alchemist2;


    //NarrationSpeeches
    public Narration killSpeech; //caseUp
    public Narration failToKillSpeech;

    public Narration switchSpeech; //caseRight

    public Narration alchemistsHelpedSpeech; // caseLeft
    public Narration theOtherSpeech;

    public Narration sacrificeSpeech; //caseDown

    //Animations
    int BossAnimationState;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!NarrationManager.instance.isPlaying && alive)
        {
            BossAnimationState = 0; //idle animation
        }

        if (InRange && alive && Input.GetKeyDown(KeyCode.UpArrow) && CharScript.PlayerState == 12)
        {
            //talk
            CharScript.PlayerState = 13;
            if (CharScript.numberOfAliveSickNPCs == 0)
            {
                NarrationManager.instance.PlayNarration(killSpeech);
                CharScript.ending = 1;
            }
            else
            {
                NarrationManager.instance.PlayNarration(failToKillSpeech);
            }
            BossAnimationState = 2; //talk
        }

        if (InRange && alive && Input.GetKeyDown(KeyCode.LeftArrow) && CharScript.PlayerState == 12)
        {
            CharScript.PlayerState = 13;
            if (CharScript.minersOut && AlchemistChief.GetComponent<NPCScript>().alive && Alchemist1.GetComponent<NPCScript>().alive && Alchemist2.GetComponent<NPCScript>().alive)
            {
                AlchemistChief.GetComponent<BoxCollider2D>().enabled = false;
                Alchemist1.GetComponent<BoxCollider2D>().enabled = false;
                Alchemist2.GetComponent<BoxCollider2D>().enabled = false;
                NarrationManager.instance.PlayNarration(alchemistsHelpedSpeech);
                CharScript.ending = 4;
            }
            else
            {
                NarrationManager.instance.PlayNarration(theOtherSpeech);
            }
        }

        if (InRange && Input.GetKeyDown(KeyCode.RightArrow) && CharScript.PlayerState == 12)
        {
            CharScript.PlayerState = 13;
            NarrationManager.instance.PlayNarration(switchSpeech);
            CharScript.ending = 2;
        }

        if (InRange && Input.GetKeyDown(KeyCode.DownArrow) && CharScript.PlayerState == 12)
        {
            CharScript.PlayerState = 13;
            NarrationManager.instance.PlayNarration(sacrificeSpeech);
            CharScript.ending = 3;
        }

       // SetAnimationState();
    }

    void SetAnimationState()
    {
        switch (BossAnimationState)
        {
            case 0:
                anim.SetInteger("BossAnimationState", 0); //idle
                break;

            case 1:
                anim.SetInteger("BossAnimationState", 1); //dead
                break;

            case 2:
                anim.SetInteger("BossAnimationState", 2); //talk
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
        if (col.gameObject.tag == "Player" && CharScript.PlayerState != 12)
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
