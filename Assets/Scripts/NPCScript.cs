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
    public bool aliveGiveLifeSpeechDone = false;
    bool activeRegularSpeech;
    public int regularSpeechDone = 0;

    //ExtraSpecialConditions
    public static bool sacrificeSickoKiller, collapsedMine;
    public bool minersOutOption, alchemistsOutOption, sendWorkerOption, destroyBoulderOption;


    //Animations
    int NPCAnimationState;
    public Animator anim;
    public bool tempBool;
    //extra 
    GameObject player;
    float NPCscale;
    public bool rightie; //colliders

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        NPCscale = transform.localScale.x;
        //if (sick && this.gameObject.GetComponent<SpriteRenderer>() != null) //erase all this stuff later on, keep it for now just in case
        //{
        //    GetComponent<SpriteRenderer>().color = new Color32(120, 215, 70, 255);
        //}
        //else if (!sick && this.gameObject.GetComponent<SpriteRenderer>() != null)
        //{ 
        //    GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        //}

        if (alive)
        {
            //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            NPCAnimationState = 0; //idle
        }
        else
        {
            //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
           // transform.position = new Vector3(transform.position.x, transform.position.y-1, 0);
            NPCAnimationState = 4; //dead
        }

        anim.enabled = false;
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
            NPCAnimationState = 1; //talk

            //special condition for random speech only occurs if we press up

            //ALCHEMIST2 DIES WHEN TALKING TO MAYOR IF KILLER NOT HEALED OR KILLED
            if (npcSituation == "Mayor")
            {
                GameObject.Find("MineGate").transform.position = new Vector2(120, 119);
                if (!GameObject.Find("SickoKiller").GetComponent<NPCScript>().alive && GameObject.Find("SickoKiller").GetComponent<BoxCollider2D>().enabled)
                {
                    GameObject.Find("SickoKiller").transform.position = new Vector2(999, 999);
                }
                else if (GameObject.Find("SickoKiller").GetComponent<NPCScript>().alive && !GameObject.Find("SickoKiller").GetComponent<NPCScript>().aliveGiveLifeSpeechDone)
                {
                    CharScript.alchemist2Died = true;
                    GameObject.Find("SickoKiller").transform.position = new Vector2(999, 999);
                    GameObject.Find("SickoKiller").GetComponent<NPCScript>().alive = false;
                    GameObject.Find("Alchemist2").GetComponent<NPCScript>().alive = false;
                    GameObject.Find("Alchemist2").GetComponent<BoxCollider2D>().enabled = false;
                    GameObject.Find("Alchemist2").transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    GameObject.Find("Alchemist2").transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    GameObject.Find("Alchemist2").transform.position = new Vector2(130, 9.5f);
                    GameObject.Find("Alchemist2").transform.rotation = Quaternion.Euler(0, 0, 70);
                    //CHANGE SPRITE TO SEVERED HEAD
                }
                else if (GameObject.Find("SickoKiller").GetComponent<NPCScript>().alive && GameObject.Find("SickoKiller").GetComponent<NPCScript>().aliveGiveLifeSpeechDone)
                {
                    GameObject.Find("SickoKiller").transform.position = new Vector2(207, 10);
                    GameObject.Find("SickoKiller").transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    GameObject.Find("SickoKiller").transform.position = new Vector2(207, 10);
                    GameObject.Find("SickoKiller").GetComponent<NPCScript>().alive = false;
                    GameObject.Find("SickoKiller").GetComponent<BoxCollider2D>().enabled = false;
                    GameObject.Find("KillerHelpedTrigger").transform.position = new Vector2(200, 15);
                    CharScript.sacrificeSickoKiller = true;
                    //DISABLED COLLIDER INSTEAD OF SCRIPT TO MAKE HIM DIE
                }
            }
            if (npcSituation == "Worshipper")
            {
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //priest alive
                {
                    this.GetComponent<NarrationList>().Play();
                }
                else
                {
                    NarrationManager.instance.PlayNarration(regularSpeech);
                }
            }
            else if (npcSituation == "DrunkMiner")
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
            else if (npcSituation == "SickWorker" && sendWorkerOption)
            {
                NarrationManager.instance.PlayNarration(regularSpeech);
                //transform.position = new Vector2(999, 999);
                alive = false;
                GetComponent<BoxCollider2D>().enabled = false;
                CharScript.sentWorker = true;
            }

            else if (npcSituation == "WorkerChief" && destroyBoulderOption && !CharScript.destroyBoulder)
            {
                CharScript.destroyBoulder = true;
                GameObject.Find("Boulder").transform.position = new Vector2(999, 999);
                NarrationManager.instance.PlayNarration(regularSpeech);
                destroyBoulderOption = false;
            }
            else if (npcSituation == "WorkerChief" && !destroyBoulderOption && CharScript.destroyBoulder)
            {
                CharScript.boulderWasDestroyed = true;
                NarrationManager.instance.PlayNarration(regularSpeech);
            }

            else if (npcSituation == "AlchemistChief" && destroyBoulderOption && !CharScript.destroyBoulder)
            {
                CharScript.destroyBoulder = true;
                GameObject.Find("Boulder").transform.position = new Vector2(999, 999);
                NarrationManager.instance.PlayNarration(regularSpeech);
                destroyBoulderOption = false;
            }
            else if (npcSituation == "AlchemistChief" && !destroyBoulderOption && CharScript.destroyBoulder)
            {
                CharScript.boulderWasDestroyed = true;
                NarrationManager.instance.PlayNarration(regularSpeech);
            }
            else
            {
                NarrationManager.instance.PlayNarration(regularSpeech);
            }
            activeRegularSpeech = true;
        }
        if (activeRegularSpeech && CharScript.PlayerState == 1) //if regularSpeech happened and we finished reading it, become true
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
            CharScript.PlayerAnimationState = 5; //player take life animation
            NarrationManager.instance.PlayNarration(aliveTakeLifeSpeech);
            //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            //transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
           // GetComponent<BoxCollider2D>().size = new Vector2(sizeY, sizeX); //wrote this code to rotate collider, but not so helpful
           // GetComponent<BoxCollider2D>().offset = new Vector2(GetComponent<BoxCollider2D>().offset.x, GetComponent<BoxCollider2D>().offset.y - 1);
            NPCAnimationState = 4; //dead animation

            player.GetComponent<CharScript>().PlayerAudioSource.clip = player.GetComponent<CharScript>().takeLifeSound;
            player.GetComponent<CharScript>().PlayerAudioSource.Play();

            if (npcSituation == "KilledMiner")
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.6f);
            }
            if (npcSituation == "SickoKiller")
            {
                transform.rotation = Quaternion.Euler(0, 0, 20);
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.3f);
            }
        }

        if (InRange && Input.GetKeyDown(KeyCode.RightArrow) && CharScript.PlayerState == 2)
        {
            if (alive && CharScript.reservehealth >= 20)
            {
                CharScript.reservehealth -= 20;
                CharScript.sanity++;
                CharScript.PlayerState = 3;
                CharScript.PlayerAnimationState = 4; //player give life animation/
                NPCAnimationState = 2; //alive give life animation
                NarrationManager.instance.PlayNarration(aliveGiveLifeSpeech);
                aliveGiveLifeSpeechDone = true;
                //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                player.GetComponent<CharScript>().PlayerAudioSource.clip = player.GetComponent<CharScript>().giveLifeSound;
                player.GetComponent<CharScript>().PlayerAudioSource.Play();
            }
            else if (!alive && CharScript.reservehealth >= 60)
            {
                CharScript.reservehealth -= 60;
                CharScript.sanity += 2;
                //transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
                CharScript.PlayerAnimationState = 4; //player give life animation
                NPCAnimationState = 3; //dead give life animation
                //PushPlayerBack();

                NarrationManager.instance.PlayNarration(deadGiveLifeSpeech);
                alive = true;
                CharScript.PlayerState = 3;
                //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                player.GetComponent<CharScript>().PlayerAudioSource.clip = player.GetComponent<CharScript>().giveLifeSound;
                player.GetComponent<CharScript>().PlayerAudioSource.Play();

                if (npcSituation == "KilledMiner")
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y - 0.6f);
                }
                if (npcSituation == "SickoKiller")
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform.position = new Vector2(transform.position.x, transform.position.y - 0.3f);
                }
            }

        }

        //conditional buttons and actions

        if (CharScript.minersOut)
        {
            alchemistsOutOption = false;
        }
        if (CharScript.alchemistsOut)
        {
            minersOutOption = false;
        }

        if (InRange && Input.GetKeyDown(KeyCode.C) && CharScript.PlayerState == 2)
        {
            if (npcSituation == "WorkerChief" && minersOutOption)
            {
                CharScript.PlayerState = 0;
                CharScript.SetSpaceOptionsPromptsBool = true;
                NPCAnimationState = 0; //talk
                CharScript.minersOut = true;
                //Destroy(this.gameObject);
                transform.position = new Vector2(999, 999);
                GameObject.Find("SickWorker").transform.position = new Vector2(999, 999);
                GameObject.Find("SickWorker").GetComponent<NPCScript>().alive = false;
                GameObject.Find("MineGatekeeper").transform.position = new Vector2(999, 999);
                GameObject.Find("KilledMiner").transform.position = new Vector2(999, 999);
                GameObject.Find("DrunkMiner").transform.position = new Vector2(999, 999);
                GameObject.Find("DrunkMiner").GetComponent<NPCScript>().alive = false;
            }

            if (npcSituation == "AlchemistChief" && alchemistsOutOption)
            {
                CharScript.PlayerState = 0;
                CharScript.SetSpaceOptionsPromptsBool = true;
                NPCAnimationState = 0; //talk

                CharScript.alchemistsOut = true;

                transform.position = new Vector2(999, 999);
                GameObject.Find("Alchemist1").transform.position = new Vector2(999, 999);
                if (GameObject.Find("Alchemist2").GetComponent<BoxCollider2D>().enabled)
                {
                    GameObject.Find("Alchemist2").transform.position = new Vector2(999, 999);
                }
            }
        }


        //NarrationCases (we may customize any npc speech to our liking here)
        switch (npcSituation)
        {
            case "Mother":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if son is alive
                {
                    regularSpeech.phrases[0].text = "Please, help my son! Food is nowhere to be found, he is so sick that he's hallucinating!";
                    regularSpeech.phrases[1].text = "I'd offer you my life if that's what it takes.";
                    deadGiveLifeSpeech.phrases[0].text = "You've reunited me with my son once more.. Thank you.";
                }
                else //if he is dead
                {
                    regularSpeech.phrases[0].text = "My son! What have you done?!";
                    regularSpeech.phrases[1].text = "";
                    deadGiveLifeSpeech.phrases[0].text = "What is the point of life when it has rid you of all you love...";
                }
                break;

            case "Son":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if mother is alive
                {
                    regularSpeech.phrases[0].text = "*cough* I am very sick... My mom keeps saying I got a flu... but they're usually not this painful.";
                    regularSpeech.phrases[1].text = "I can also hear a voice sometimes. My mom says it must be a ha-lucy-nay-shun... What is that?";
                    deadGiveLifeSpeech.phrases[0].text = "Mother... I missed her very much. *cough* But I didn't miss the voice in my head...";
                }
                else //if mother is dead
                {
                    regularSpeech.phrases[0].text = "No, mom! Don't leave me alone with the voice! *cough* *cough*";
                    regularSpeech.phrases[1].text = "";
                    deadGiveLifeSpeech.phrases[0].text = "You've brought me back to life... No, mom! Don't leave me alone with the voice!";
                }
                break;

            case "Beggar":
                if (killedOnce) //if killed and brought back
                {
                    regularSpeech.phrases[0].text = "Mister alchemist sir! I'm not causing any trouble, please spare me some change!";
                }
                break;

            case "SickoKiller":
                if (SpokeWith("SickoKiller") >= 1)
                {
                    regularSpeech.phrases[0].text = "Guess I'll take a short rest. You may want to move along before I get hungry again.";
                    regularSpeech.phrases[1].text = "When I'm outta energy, that's when the voice becomes the strongest... *cough* And people start looking the tastiest...";
                    regularSpeech.phrases[2].text = "";
                }
                if (aliveGiveLifeSpeechDone)
                {
                    regularSpeech.phrases[0].text = "Thanks dude, I'm feeling way less hungry. Won't have to worry about feeding myself for a while.";
                    regularSpeech.phrases[1].text = "*cough* I'll keep going east when I'm done taking a rest.";
                    regularSpeech.phrases[2].text = "";
                }
                break;

            case "SickWorker":
                if (SpokeWith("SickWorker") == 0)
                {
                    //in the editor
                }
                else if (SpokeWith("SickWorker") >= 1 && SpokeWith("WorkerChief") == 0) //if killed and brought back
                {
                    regularSpeech.phrases[0].text = "Hello again, any news about the mine situation? *cough*";
                    regularSpeech.phrases[1].text = "";
                    regularSpeech.phrases[2].text = "";
                    regularSpeech.phrases[3].text = "";
                }
                else if (SpokeWith("WorkerChief") >= 1)
                {
                    regularSpeech.phrases[0].text = "Hello again, any news about the mine situation? *cough*";
                    regularSpeech.phrases[1].text = "Huh, so they really do need another worker!";
                    regularSpeech.phrases[2].text = "*cough* Great, I'll head there straight away!";
                    regularSpeech.phrases[3].text = "";
                    sendWorkerOption = true;
                    //transform.position = new Vector3(177, 10, 0);
                }

                if  (CharScript.sentWorker && !NarrationManager.instance.isPlaying && CharScript.PlayerState == 0)
                {
                    transform.position = new Vector2(999, 999);
                }

                //TO-DO: Add 
                break;

            case "MayorRighthandman":
                if (SpokeWith("MayorsRighthandman") >= 1)
                {
                    regularSpeech.phrases[0].text = "Sick people aren't welcome. Alchemists are though.";
                    regularSpeech.phrases[1].text = "";
                    regularSpeech.phrases[2].text = "";
                }
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
                    regularSpeech.phrases[0].text = "";
                    regularSpeech.phrases[1].text = "";
                }
                else //if he is dead
                {
                    regularSpeech.phrases[0].text = "Heretics! I got word you killed the priest with your wicked alchemy!";
                    regularSpeech.phrases[1].text = "I have nothing to say to you. Begone!";
                }
                break;

            case "Wife":
                if (conditionedByNPC.GetComponent<NPCScript>().alive) //if husband is alive
                {
                    regularSpeech.phrases[0].text = "My husband! It's a miracle! Maybe we should move to a safer place now...";
                    regularSpeech.phrases[1].text = "";
                    regularSpeech.phrases[2].text = "";
                    regularSpeech.phrases[3].text = "";

                    aliveTakeLifeSpeech.phrases[0].text = "No! I have so much to live for!";
                }
                else //if he is dead
                {
                    regularSpeech.phrases[0].text = "*crying* My husband... I got worried when he didn't come back from the mayor's mansion,";
                    regularSpeech.phrases[1].text = "so I started walking here and this is what I find...";
                    regularSpeech.phrases[2].text = "The mayor says there's a murderer on the loose, that he may be the one who killed him.";
                    regularSpeech.phrases[3].text = "Don't we have a gatekeeper whose job is to prevent such incidents...?";

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
                    regularSpeech.phrases[0].text = "Now I remember... I guess that's what he does to those who know too much.";
                    regularSpeech.phrases[1].text = "My wife and I should be moving out of here... Know this, the mayor is a huge hypocrite. ";
                    regularSpeech.phrases[2].text = "He's kicking out the sick, saying they're a burden on society, but he's actually sick himself.";

                    //spokeWithHusband
                }
                break;

            case "Mayor":
                if (SpokeWith("Mayor") == 0) //if never spoken with mayor
                {
                    //in editor
                }
                else if (SpokeWith("Mayor") >= 1 && !(SpokeWith("Corpse2") >= 1 || SpokeWith("Husband") >= 1))
                {
                    regularSpeech.phrases[0].text = "Have you seen your alchemist friends yet?";
                    regularSpeech.phrases[1].text = "No? What are you waiting for?";
                    regularSpeech.phrases[2].text = "";
                    regularSpeech.phrases[3].text = "";
                    regularSpeech.phrases[4].text = "";
                    regularSpeech.phrases[5].text = "";
                }
                else if (SpokeWith("Mayor") >= 1 && (SpokeWith("Corpse2") >= 1 || SpokeWith("Husband") >= 1))
                {
                    regularSpeech.phrases[0].text = "Oh? So you've heard some rumors about me? Pay them no mind, that's all they are, just rumors. ";
                    regularSpeech.phrases[1].text = "By the way, could you tell me who exactly told you those things?";
                    regularSpeech.phrases[2].text = "I'm just wondering.";
                    regularSpeech.phrases[3].text = "";
                    regularSpeech.phrases[4].text = "";
                    regularSpeech.phrases[5].text = "";
                }
                //TO-DO: add in alchemists part
                break;

            case "MineGatekeeper":
                //if (SpokeWith("Mayor") == 0)
                //{
                //    regularSpeech.phrases[0].text = "Where do you think you're going!? You need a permission slip to enter through here.";
                //    regularSpeech.phrases[1].text = "Go talk with the mayor.";
                //}
                if (SpokeWith("Mayor") >= 1)
                {
                    regularSpeech.phrases[0].text = "You're welcome to come right in with your permission slip.";
                    regularSpeech.phrases[1].text = "";
                    //TO-DO: Destroy Gate
                }
                break;

            case "WorkerChief":
                if (SpokeWith("AlchemistChief") >= 1 && !CharScript.sentWorker)
                {
                    regularSpeech.phrases[0].text = "You spoke with the alchemists? Well you shouldn't listen to them.";
                    regularSpeech.phrases[1].text = "We're all honest hard working men down here. Please just help us find another worker.";
                }
                else if (CharScript.sentWorker && !CharScript.alchemistsOut) //alchemists not driven out
                {
                    regularSpeech.phrases[0].text = "Thanks for bringing us that guy. We can work a bit more effectively now.";
                    regularSpeech.phrases[1].text = "Now only if those shady alchemists were gone, then we could truly focus and break this boulder.";
                }
                else if (CharScript.sentWorker && CharScript.alchemistsOut && !CharScript.destroyBoulder)
                {
                    regularSpeech.phrases[0].text = "Thank you for everything, sir. We'll get this done real fast for you.";
                    regularSpeech.phrases[1].text = "";
                    Debug.Log(CharScript.alchemistsOut);
                    destroyBoulderOption = true;
                    CharScript.canKickMiners = false;
                }
                if (CharScript.alchemistsOut && CharScript.destroyBoulder && CharScript.boulderWasDestroyed)
                {
                    regularSpeech.phrases[0].text = "All in a day's work. No need to thank us.";
                    regularSpeech.phrases[1].text = "";
                }


                if (SpokeWith("AlchemistChief") >= 2)
                {
                    //option spawns to drive out the miners
                    minersOutOption = true;
                    if (!CharScript.canKickMiners && !CharScript.alchemistsOut)
                    {
                        CharScript.canKickMiners = true;
                    }
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

            case "Alchemist1":
                if (SpokeWith("AlchemistChief") == 0 && !CharScript.destroyBoulder && conditionedByNPC.GetComponent<NPCScript>().alive && CharScript.alchemist2Died)
                {
                    regularSpeech.phrases[0].text = "My buddy got killed by a deranged murderer outside... The murderer ate his innards.";
                    regularSpeech.phrases[1].text = "He didn't know we were alchemists. I incinerated him with fire potions. Now he's ash... Serves him right...";
                }
                if (SpokeWith("AlchemistChief") >= 1 && !CharScript.destroyBoulder && conditionedByNPC.GetComponent<NPCScript>().alive)
                {
                    regularSpeech.phrases[0].text = "I'm not sure if I agree with the chief...";
                    regularSpeech.phrases[1].text = "This may be our chance to reconnect with the townsfolk. Maybe we should help.";
                }
                if (CharScript.destroyBoulder && CharScript.minersOut && conditionedByNPC.GetComponent<NPCScript>().alive)
                {
                    regularSpeech.phrases[0].text = "It's always nice to have more alchemists around.";
                    regularSpeech.phrases[1].text = "Make yourselves at home.";
                }
                if (!conditionedByNPC.GetComponent<NPCScript>().alive) //if alchemistChief is dead
                {
                    regularSpeech.phrases[0].text = "So you've chosen to abuse your power against out chief...";
                    regularSpeech.phrases[1].text = "It was foolish of me to trust you, you were never one of us.";
                }
                break;

            case "Alchemist2":
                if (SpokeWith("AlchemistChief") == 0 && !CharScript.destroyBoulder && conditionedByNPC.GetComponent<NPCScript>().alive)
                {
                    regularSpeech.phrases[0].text = "Those miner folks sure are noisy! Not even within a cavern can we do our studies in peace...";
                    regularSpeech.phrases[1].text = "";
                }
                if (SpokeWith("AlchemistChief") >= 2 && !CharScript.destroyBoulder && conditionedByNPC.GetComponent<NPCScript>().alive)
                {
                    regularSpeech.phrases[0].text = "The chief is right, I don't trust that mayor a bit! How come he only needs us now!?";
                    regularSpeech.phrases[1].text = "I'll bet you 3 potions he just wants to use us.";
                }
                else if (CharScript.destroyBoulder && CharScript.minersOut && conditionedByNPC.GetComponent<NPCScript>().alive)
                {
                    regularSpeech.phrases[0].text = "Finally, some quiet. Thanks for getting rid of those pesky miners.";
                    regularSpeech.phrases[1].text = "";
                }
                else if (!conditionedByNPC.GetComponent<NPCScript>().alive) //if alchemistChief is dead
                {
                    regularSpeech.phrases[0].text = "The chief! What have you done!?";
                    regularSpeech.phrases[1].text = "Alchemists like you are the ones who give us a bad reputation. Go away!";
                }
                break;

            case "AlchemistChief":
                if (SpokeWith("AlchemistChief") == 0)
                {
                    //in the editor
                }
                if (SpokeWith("AlchemistChief") >= 1 && !CharScript.minersOut)
                {
                    regularSpeech.phrases[0].text = "I'm not going to change my mind. Enough is enough. They'll have to kill me first to get me out!";
                    regularSpeech.phrases[1].text = "It's the miners who should leave us in peace! I'm sure my pupils will back me up on this.";
                    regularSpeech.phrases[2].text = "";
                    regularSpeech.phrases[3].text = "";
                    regularSpeech.phrases[4].text = "";
                    regularSpeech.phrases[5].text = "";
                }
                if (CharScript.minersOut && !CharScript.destroyBoulder)
                {
                    regularSpeech.phrases[0].text = "You got rid of those miners for us. Thank you. From now on, we'll be there when you need us.";
                    regularSpeech.phrases[1].text = "Let us show our appreciation by clearing your path. We had been crafting some explosives that may be useful.";
                    regularSpeech.phrases[2].text = "";
                    regularSpeech.phrases[3].text = "";
                    regularSpeech.phrases[4].text = "";
                    regularSpeech.phrases[5].text = "";

                    destroyBoulderOption = true;
                    CharScript.canKickAlchemists = false;
                }
                if (CharScript.minersOut && CharScript.destroyBoulder && CharScript.boulderWasDestroyed)
                {
                    regularSpeech.phrases[0].text = "Hello, friend. Good to see you.";
                    regularSpeech.phrases[1].text = "";
                    regularSpeech.phrases[2].text = "";
                    regularSpeech.phrases[3].text = "";
                    regularSpeech.phrases[4].text = "";
                    regularSpeech.phrases[5].text = "";
                }
                if (!alive)
                {
                    alchemistsOutOption = true;
                    if (!CharScript.boulderWasDestroyed && !CharScript.minersOut)
                    {
                        CharScript.canKickAlchemists = true;
                    }
                }
                if (alive)
                {
                    alchemistsOutOption = false;
                    CharScript.canKickAlchemists = false;
                }
                break;



            default:
                break;
        }

        if (tempBool)
            SetAnimationState();


        //let the npc face the player during speech
        if (rightie)
        {
            if (player.transform.localScale.x > 0 && alive && InRange)
            {
                transform.localScale = new Vector2(NPCscale, transform.localScale.y);
            }
            else if (player.transform.localScale.x < 0 && alive && InRange)
            {
                transform.localScale = new Vector2(-NPCscale, transform.localScale.y);
            }

        }
        else
        {
            if (player.transform.localScale.x > 0 && alive && InRange)
            {
                transform.localScale = new Vector2(-NPCscale, transform.localScale.y);
            }
            else if (player.transform.localScale.x < 0 && alive && InRange)
            {
                transform.localScale = new Vector2(NPCscale, transform.localScale.y);
            }

        }
    }

    void SetAnimationState()
    {
        switch (NPCAnimationState)
        {
            case 0:
                anim.SetInteger("NPCAnimationState", 0); //idle
                break;
            case 1:
                anim.SetInteger("NPCAnimationState", 1); //talk
                break;
            case 2:
                anim.SetInteger("NPCAnimationState", 2); //alive give life
                break;
            case 3:
                anim.SetInteger("NPCAnimationState", 3); //dead give life
                break;
            case 4:
                anim.SetInteger("NPCAnimationState", 4); //dead
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
        if (col.gameObject.tag == "PlayerZone")
        {
            anim.enabled = true;
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
        if (col.gameObject.tag == "PlayerZone")
        {
            anim.enabled = false;
        }
    }
}
