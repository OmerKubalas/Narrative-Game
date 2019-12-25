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

    //SpecialConditions
    bool killedOnce = false;
    bool aliveGiveLifeSpeechDone = false;
    bool activeRegularSpeech;
    public int regularSpeechDone = 0;

    //ExtraSpecialConditions
    public static bool sacrificeSickoKiller, collapsedMine;
    public bool minersOutOption, alchemistsOutOption, destroyBoulder;


    //Animations
    int NPCAnimationState;
    Animator anim;

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
            NPCAnimationState = 0; //idle
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x, transform.position.y-1, 0);
            NPCAnimationState = 1; //dead
        }

        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!NarrationManager.instance.isPlaying && alive)
        {
            NPCAnimationState = 0; //idle animation
        }

        if (InRange && alive && Input.GetKeyDown(KeyCode.UpArrow) && CharScript.PlayerState == 2)
        {
            //talk
            CharScript.PlayerState = 3;
            NPCAnimationState = 2; //talk

            //special condition for random speech only occurs if we press up
            if (npcSituation == "Worshipper")
            {
                if (conditionedByNPC.GetComponent<NPCScript>().alive)
                {
                    this.GetComponent<NarrationList>().Play();
                }
                else
                {
                    NarrationManager.instance.PlayNarration(regularSpeech);
                }
            }
            if (npcSituation == "DrunkMiner")
            {
                if (conditionedByNPC.GetComponent<NPCScript>().alive)
                {
                    this.GetComponent<NarrationList>().Play();
                }
                else
                {
                    NarrationManager.instance.PlayNarration(regularSpeech);
                }
            }

            else
            {
                NarrationManager.instance.PlayNarration(regularSpeech);
            }

            activeRegularSpeech = true;
        }
        if (activeRegularSpeech && CharScript.PlayerState == 0) //if regularSpeech happened and we finished reading it, become true
        {
            regularSpeechDone++;
            activeRegularSpeech = false;
        }

        if (InRange && alive && Input.GetKeyDown(KeyCode.LeftArrow) && CharScript.PlayerState == 2)
        {
            CharScript.reservehealth += 20;
            if (sick == false)
            {
                CharScript.reservehealth += 20;
                CharScript.sanity--;
            }
            alive = false;
            killedOnce = true;
            //absorb
            CharScript.PlayerState = 3;
            NarrationManager.instance.PlayNarration(aliveTakeLifeSpeech);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
            NPCAnimationState = 1; //dead animation
        }

        if (InRange && Input.GetKeyDown(KeyCode.RightArrow) && CharScript.PlayerState == 2)
        {
            if (alive && CharScript.reservehealth >= 20)
            {
                CharScript.reservehealth -= 20;
                NarrationManager.instance.PlayNarration(aliveGiveLifeSpeech);
                aliveGiveLifeSpeechDone = true;
                NPCAnimationState = 0; //idle animation
                CharScript.PlayerState = 3;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (!alive && CharScript.reservehealth >= 60)
            {
                CharScript.reservehealth -= 60;
                CharScript.sanity += 2;
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
                NarrationManager.instance.PlayNarration(deadGiveLifeSpeech);
                NPCAnimationState = 0; //idle animation
                alive = true;
                CharScript.PlayerState = 3;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        //conditional buttons and actions

        if (InRange && Input.GetKeyDown(KeyCode.C) && CharScript.PlayerState == 2)
        {
            CharScript.PlayerState = 3;
            NPCAnimationState = 2; //talk
            if (npcSituation == "WorkerChief" && minersOutOption)
            {
                Destroy(this.gameObject);
                //destroy other miners too
                CharScript.minersOut = true;
            }

            if (npcSituation == "AlchemistChief" && alchemistsOutOption)
            {
                Destroy(this.gameObject);
                //destroy other miners too
                CharScript.alchemistsOut = true;
            }
        }

        if (CharScript.minersOut)
        {
            alchemistsOutOption = false;
        }
        if (CharScript.alchemistsOut)
        {
            minersOutOption = false;
        }

        //if (destroyBoulder)
        //{
        //    //Destroy boulder
        //    destroyBoulder = false;
        //}

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

            case "Beggar":
                if (killedOnce) //if killed and brought back
                {
                    regularSpeech.phrases[0].text = "Mister alchemist sir! I'm not causing any trouble, please spare me some change!";
                }
                break;

            case "SickoKiller":
                if (aliveGiveLifeSpeechDone) //if killed and brought back
                {
                    //we can spawn a different visual here
                    transform.position = new Vector3(200, 10, 0);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    GetComponent<NPCScript>().enabled = false;
                    sacrificeSickoKiller = true;
                }
                break;

            case "SickWorker":
                if (SpokeWith("SickWorker") == 0)
                {
                    regularSpeech.phrases[0].text = "I come from the upper village. Those pompous assholes think they can go around kicking out the sick.";
                    regularSpeech.phrases[1].text = "*cough* They say we're costly to sustain, as if they don't run around squandering their money as it is!";
                    regularSpeech.phrases[2].text = "If only I could enter the mines and work there, but they won't let me in without the mayor's permission either.";
                    regularSpeech.phrases[3].text = "Talk about being caught between a rock and a hard place.";
                }
                if (SpokeWith("SickWorker") >= 1) //if killed and brought back
                {
                    regularSpeech.phrases[0].text = "Hello again, any news about the mine situation? *cough*";
                    regularSpeech.phrases[1].text = "";
                    regularSpeech.phrases[2].text = "";
                    regularSpeech.phrases[3].text = "";
                    regularSpeechDone = 1;

                    if (SpokeWith("WorkerChief") >= 1)
                    {
                        regularSpeech.phrases[1].text = "Huh, so they really do need another worker!";
                        regularSpeech.phrases[2].text = "*cough* Great, I'll head there straight away!";
                        //transform.position = new Vector3(177, 10, 0);
                        regularSpeechDone = 2;
                    }
                }
                
                //TO-DO: Add 
                break;

            case "Corpse1":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if corpse2 alive
                {
                    deadGiveLifeSpeech.phrases[0].text = "Oh, John! We're alive!";
                }
                else //if he is dead
                {
                    deadGiveLifeSpeech.phrases[0].text = "Oh! I'm back! What happened to John!?";
                }
                break;

            case "Corpse2":
                //we spoke to it
                break;

            case "Worshipper":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if priest alive
                {
                   //done
                }
                else //if he is dead
                {
                    regularSpeech.phrases[0].text = "Heretics! I got word you killed the priest with your wicked alchemy! I have nothing to say to you. Begone!";
                    regularSpeech.phrases[1].text = "I have nothing to say to you. Begone!";
                }
                break;

            case "Wife":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if husband is alive
                {
                    regularSpeech.phrases[0].text = "My husband! It's a miracle! Maybe we should move to a safer place now...";
                    regularSpeech.phrases[1].text = "";
                    regularSpeech.phrases[2].text = "";
                    aliveTakeLifeSpeech.phrases[0].text = "No! I have so much to live for!";
                }
                else //if he is dead
                {
                    regularSpeech.phrases[0].text = "*crying* My husband... I got worried when he didn't come back from the mayor's mansion, so I started walking here and this is what I find...";
                    regularSpeech.phrases[1].text = "The mayor says there's a murderer on the loose, that he may be the one who killed him.";
                    regularSpeech.phrases[2].text = "Don't we have a gatekeeper whose job is to prevent such incidents...?";
                    aliveTakeLifeSpeech.phrases[0].text = "I'm coming to your side, darling.";
                }
                break;

            case "Husband":
                if (!aliveGiveLifeSpeechDone) //if killed and brought back
                {
                    regularSpeech.phrases[0].text = "I get a terrible headache whenever I attempt to recall... Can you help me...?";
                    regularSpeechDone = 0;
                }
                else
                {
                    regularSpeech.phrases[0].text = " Oh, now I remember... I guess that's what he does to those who know too much. My wife and I should be moving out of here... Know this, the mayor is a huge hypocrite, he's kicking out the sick, saying they're a burden on society, but he's actually sick himself.";
                    //spokeWithHusband
                }
                break;

            case "Mayor":
                if (SpokeWith("Mayor") == 0) //if never spoken with mayor
                {
                    regularSpeech.phrases[0].text = "An alchemist just walks into my mansion... What are the odds? Don't worry, your kind is very welcome here.";
                    regularSpeech.phrases[1].text = "In fact, I have a small favor to ask of you. *cough* I heard there are other alchemists like you residing underground.";
                    regularSpeech.phrases[2].text = "I'll give you a key to the mines, in turn, would you be as kind as to bring your alchemist friends to me? The miners have been complaining about them, and we need their help for research, *cough* it's for a good cause, you can trust me.";
                }
                else if (SpokeWith("Mayor") >= 1 && !(SpokeWith("Corpse2") >= 1 || SpokeWith("Husband") >= 1))
                {
                    regularSpeech.phrases[0].text = "Have you seen your alchemist friends yet?";
                    regularSpeech.phrases[1].text = "No? What are you waiting for?";
                    regularSpeech.phrases[2].text = "";
                }
                else if (SpokeWith("Mayor") >= 1 && (SpokeWith("Corpse2") >= 1 || SpokeWith("Husband") >= 1))
                {
                    regularSpeech.phrases[0].text = "Oh? So you've heard some rumors about me? Pay them no mind, that's all they are, just rumors. ";
                    regularSpeech.phrases[1].text = "By the way, could you tell me who exactly told you those things?";
                    regularSpeech.phrases[2].text = "I'm just wondering.";
                }
                //TO-DO: add in alchemists part
                break;

            case "MineGatekeeper":
                if (SpokeWith("Mayor") == 0)
                {
                    regularSpeech.phrases[0].text = "Where do you think you're going!? You need a permission slip to enter through here.";
                    regularSpeech.phrases[1].text = "Go talk with the mayor.";
                }
                if (SpokeWith("Mayor") >= 1)
                {
                    regularSpeech.phrases[0].text = "You're welcome to come right in with your permission slip.";
                    regularSpeech.phrases[1].text = "";
                    //TO-DO: Destroy Gate
                }
                break;

            case "WorkerChief":
                if (SpokeWith("AlchemistChief") == 0 && SpokeWith("SickWorker") == 0)
                {
                    regularSpeech.phrases[0].text = "Hello, sir. We're working hard as usual, but we are short on workers.";
                    regularSpeech.phrases[1].text = "If you bring us a worker, it'd be great.";
                }
                else if (SpokeWith("AlchemistChief") >= 1 && SpokeWith("SickWorker") == 0)
                {
                    regularSpeech.phrases[0].text = "You spoke with the alchemists? Well you shouldn't listen to them.";
                    regularSpeech.phrases[1].text = "We're all honest hard working men down here. Please just help us find another worker.";
                }
                else if (SpokeWith("SickWorker") >= 2 && SpokeWith("Alchemists") == 0)
                {
                    regularSpeech.phrases[0].text = "Thanks for bringing us that guy. We can work a bit more effectively now.";
                    regularSpeech.phrases[1].text = "Now only if those shady alchemists were gone, then we could truly focus and break this boulder.";
                }

                if (SpokeWith("AlchemistChief") >= 2)
                {
                    //option spawns to drive out the miners
                    minersOutOption = true;
                }
                //TO-DO: ADD GONE ALCHEMISTS
                break;

            case "DrunkMiner":
                if (alive)
                {
                    collapsedMine = true;
                }
                else
                {
                    collapsedMine = false;
                }
                break;

            case "KilledMiner":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if drunkard is alive
                {
                    regularSpeech.phrases[0].text = "Someone should really get rid of that drunkard!";
                }
                else //if he is dead
                {
                    regularSpeech.phrases[0].text = "I'm glad someone took care of the drunkard running around here. Who lets a drunk man work in a mine anyway!?";
                }
                break;

            case "AlchemistChief":
                if (SpokeWith("AlchemistChief") == 0)
                {
                    regularSpeech.phrases[0].text = "It's rare to see other alchemists nowadays, what's the occasion?";
                }
                if (SpokeWith("AlchemistChief") >= 1)
                {
                    regularSpeech.phrases[0].text = "I'm not going to change my mind. Enough is enough. They'll have to kill me first";
                }
                if (CharScript.minersOut && !destroyBoulder)
                {
                    regularSpeech.phrases[0].text = " You got rid of those miners for us. Thank you. From now on, we'll be there when you need us.";
                    if (SpokeWith("AlchemistChief") >= 3)
                    {
                        destroyBoulder = true;
                        Debug.Log("DestroyBoulder");
                    }
                }
                if (CharScript.minersOut && destroyBoulder)
                {
                    regularSpeech.phrases[0].text = "Hello, friend. Good to see you.";
                }
                if (!alive)
                {
                    alchemistsOutOption = true;
                }
                break;



            default:
                break;
        }

        //SetAnimationState();
    }

    void SetAnimationState()
    {
        switch (NPCAnimationState)
        {
            case 0:
                anim.SetInteger("NPCAnimationState", 0); //idle
                break;

            case 1:
                anim.SetInteger("NPCAnimationState", 1); //dead
                break;

            case 2:
                anim.SetInteger("NPCAnimationState", 2); //talk
                break;
        }
    }

    int SpokeWith(string charName)
    {
        GameObject NPC = GameObject.Find(charName);
        int timesSpoken = NPC.GetComponent<NPCScript>().regularSpeechDone;
        return timesSpoken;

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
