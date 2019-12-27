using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using Narrate;

public class CharScript : MonoBehaviour
{
    public Rigidbody2D playerbody;
    public float speed;
    public float jumpForce;
    public static float reservehealth;
    float jumps;
    public static int PlayerState;
    public static int sanity;
    public static float intensity;
    bool lookingatcompanion;

    GameObject NPC;

    public GameObject spacePopup, optionsPopup, bossOptionsPopup, companion, companionPopup, reserveHealthBar, Boss;

    GameObject cameraGO;

    int PlayerAnimationState = 0;
    Animator anim;
    float jumpAnimationBlend;

    //types of NPC
    public static int numberOfAliveSickNPCs;
    //public static List<GameObject> AliveSickNPCs = new List<GameObject>(); //if we want to access alive sick npcs

    public static int ending = 0;


    //conditionalBools
    public static bool SetSpaceOptionsPromptsBool, sentWorker, minersOut, alchemistsOut, destroyBoulder, boulderWasDestroyed, canKickMiners, canKickAlchemists, alchemist2Died, sacrificeSickoKiller;

    // Start is called before the first frame update
    void Start()
    {

        boulderWasDestroyed = false;
        jumps = 1;
        speed = 10;
        PlayerState = 0;
        sanity = 4;
        reservehealth = 0;

        cameraGO = GameObject.FindGameObjectWithTag("MainCamera");
        intensity = cameraGO.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.value;
        intensity = 0.2f;

        anim = this.GetComponent<Animator>();

        UpdateNPCStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerState != 2 && PlayerState != 3 && PlayerState != 12 && PlayerState != 13)
        {
            playerbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), playerbody.velocity.y / speed) * speed;

            if (playerbody.velocity.x == 0)
            {
               PlayerAnimationState = 0; //idle
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && jumps > 0)
            {
                jumpAnimationBlend = 0;
                jumps = 0;
                PlayerAnimationState = 2; //jump
                playerbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                PlayerAnimationState = 2;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (transform.localScale.x > 0)
                {
                    transform.position += new Vector3(8.1f, 0, 0); //add pixels to make him move correctly
                }
                transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
                PlayerAnimationState = 1; //walk
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (transform.localScale.x < 0)
                {
                    transform.position -= new Vector3(8.1f, 0, 0);
                }
                transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                PlayerAnimationState = 1; //walk
            }
            if (Input.GetKeyDown(KeyCode.Space) && lookingatcompanion && reservehealth >= 20 && CompScript.comphealth <= 80 && CompScript.comphealth > 0)
            {
                reservehealth -= 20;
                CompScript.comphealth += 20;
            }
        }
        else
        {
            playerbody.velocity = new Vector2(0, playerbody.velocity.y / speed) * speed;
        }
        //FOR NPCS
        if (PlayerState == 1 && Input.GetKeyDown(KeyCode.Space) && lookingatcompanion == false)
        {
            PlayerState = 2;
            PlayerAnimationState = 0;
            SetSpaceOptionsPrompts();
        }
        //FOR BOSS
        if (PlayerState == 11 && Input.GetKeyDown(KeyCode.Space) && lookingatcompanion == false)
        {
            PlayerState = 12;
            SetBossSpaceOptionsPrompts();
        }
        //FOR NPCS
        if (PlayerState == 2)
        {
            PlayerAnimationState = 0;
            if (Input.GetKeyDown(KeyCode.UpArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                //talk here
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                //if (NPC.GetComponent<NPCScript>().sick == false)
                //{
                //    sanity--;
                //}
                //absorb life here
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) && reservehealth >= 50)
            {
                //give life here
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                sanity++;
                //cancel
                PlayerState = 1;
                SetSpaceOptionsPrompts();
            }
        }
        //FOR BOSS
        if (PlayerState == 12)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //talk here
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //absorb life here
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //give life here
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {

            }
        }

        if (!NarrationManager.instance.isPlaying && PlayerState == 3 && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerState = 0;
            UpdateNPCStats();
            SetSpaceOptionsPrompts();
        }
        if (!NarrationManager.instance.isPlaying && PlayerState == 13 && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerState = 11;
            SetBossSpaceOptionsPrompts();
            LoadEndingScene();
        }
        if (PlayerState == 3) //just to make optionspopup go away 
        {
            SetSpaceOptionsPrompts();
        }
        if (PlayerState == 13) //just to make optionspopup go away 
        {
            SetBossSpaceOptionsPrompts();
        }

        reserveHealthBar.GetComponent<RectTransform>().transform.localScale = new Vector3(reservehealth / 100, 0.1f, 1);
        reserveHealthBar.transform.position = new Vector2(this.gameObject.transform.position.x - 1, this.gameObject.transform.position.y + 2.5f);

        if (reservehealth > 100)
        {
            reservehealth = 100;
        }

        //the whole sanity thing
        {
            if (sanity > 4)
            {
                sanity = 4;
            }
            if (sanity == 4)
            {
                if (intensity > 0.2f)
                {
                    intensity = intensity - 0.05f * Time.deltaTime;
                }
                else if (intensity < 0.2f)
                {
                    intensity = intensity + 0.05f * Time.deltaTime;
                }
            }
            else if (sanity == 3)
            {
                if (intensity > 0.325f)
                {
                    intensity = intensity - 0.05f * Time.deltaTime;
                }
                else if (intensity < 0.325f)
                {
                    intensity = intensity + 0.05f * Time.deltaTime;
                }
            }
            else if (sanity == 2)
            {
                if (intensity > 0.425f)
                {
                    intensity = intensity - 0.07f * Time.deltaTime;
                }
                else if (intensity < 0.425f)
                {
                    intensity = intensity + 0.07f * Time.deltaTime;
                }
            }
            else if (sanity == 1)
            {
                if (intensity > 0.575f)
                {
                    intensity = intensity - 0.05f * Time.deltaTime;
                }
                else if (intensity < 0.575f)
                {
                    intensity = intensity + 0.05f * Time.deltaTime;
                }
            }
            else if (sanity < 1)
            {
                //dead animation
                if (intensity < 50f)
                {
                    intensity = intensity + 0.1f * Time.deltaTime;
                }
                if (intensity > 0.60f)
                {
                    intensity = intensity + 0.5f * Time.deltaTime;
                }
                if (intensity > 1f)
                {
                    intensity = intensity + 1f * Time.deltaTime;
                }
                if (intensity > 20f)
                {
                    intensity = intensity + 1f * Time.deltaTime;
                }
            }
            if (intensity > 6f)
            {
                SceneManager.LoadScene("GameOverScene");
            }
            cameraGO.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.value = intensity;
        }

        if(NarrationManager.instance.isPlaying)
        {
            PlayerAnimationState = 0;
        }

        SetAnimationState();

        if (SetSpaceOptionsPromptsBool)
        {
            SetSpaceOptionsPrompts();
            SetSpaceOptionsPromptsBool = false;
        }
    }

    void SetSpaceOptionsPrompts()
    {
        if (NPC != null && NPC.GetComponent<NPCScript>().alive)
        {
            optionsPopup.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            optionsPopup.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }
        else
        {
            optionsPopup.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            optionsPopup.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        }
        if (reservehealth >= 20 && NPC.GetComponent<NPCScript>().alive || reservehealth >= 60 && !NPC.GetComponent<NPCScript>().alive)
        {
            optionsPopup.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }
        else
        {
            optionsPopup.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        }
        if (PlayerState == 0 || PlayerState == 3)
        {
            spacePopup.transform.position = new Vector2(999, 999);
            optionsPopup.transform.position = new Vector2(999, 999);
        }
        if (PlayerState == 1)
        {
            spacePopup.transform.position = new Vector2(NPC.transform.position.x, NPC.transform.position.y + 2);
            optionsPopup.transform.position = new Vector2(999, 999);
        }
        if (PlayerState == 2)
        {
            optionsPopup.transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = false;
            optionsPopup.transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = false;

            spacePopup.transform.position = new Vector2(999, 999);
            optionsPopup.transform.position = new Vector2(NPC.transform.position.x, NPC.transform.position.y + 4);

            if (canKickMiners && NPC.name == "WorkerChief")
            {
                optionsPopup.transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = true;
            }
            else if (canKickAlchemists && NPC.name == "AlchemistChief")
            {
                optionsPopup.transform.GetChild(5).GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    void SetBossSpaceOptionsPrompts()
    {
        if (PlayerState == 0 || PlayerState == 13)
        {
            spacePopup.transform.position = new Vector2(999, 999);
            bossOptionsPopup.transform.position = new Vector2(999, 999);
        }
        if (PlayerState == 11)
        {
            spacePopup.transform.position = new Vector2(Boss.transform.position.x, Boss.transform.position.y + 2);
            bossOptionsPopup.transform.position = new Vector2(999, 999);
        }
        if (PlayerState == 12)
        {
            spacePopup.transform.position = new Vector2(999, 999);
            bossOptionsPopup.transform.position = new Vector2(Boss.transform.position.x, Boss.transform.position.y + 4);
        }
    }

    void SetCompanionPrompt()
    {
        if (lookingatcompanion)
        {
            spacePopup.transform.position = new Vector2(companion.transform.position.x, companion.transform.position.y + 2.5f);
            companionPopup.transform.position = new Vector2(companion.transform.position.x, companion.transform.position.y + 3.5f);
        }
        if (lookingatcompanion == false)
        {
            spacePopup.transform.position = new Vector2(999, 999);
            companionPopup.transform.position = new Vector2(999, 999);
        }
    }

    void SetAnimationState()
    {
        switch (PlayerAnimationState)
        {
            case 0:
                anim.SetInteger("PlayerAnimationState", 0); //idle
                break;

            case 1:
                anim.SetInteger("PlayerAnimationState", 1); //walk
                break;

            case 2:
                if (jumpAnimationBlend <= 1.0f)
                {
                    jumpAnimationBlend += 0.045f;
                }
                anim.SetInteger("PlayerAnimationState", 2); //jump
                anim.SetFloat("Blend", jumpAnimationBlend);
                break;

            case 3:
                anim.SetInteger("PlayerAnimationState", 3); //dead? didn't set 3rd case to anything yet
                break;
        }
    }

    void UpdateNPCStats()
    {
        numberOfAliveSickNPCs = 0;
        //AliveSickNPCs.Clear();
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("NPC").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("NPC")[i].GetComponent<NPCScript>().alive && GameObject.FindGameObjectsWithTag("NPC")[i].GetComponent<NPCScript>().sick)
            {
                numberOfAliveSickNPCs++;
                //AliveSickNPCs.Add(GameObject.FindGameObjectsWithTag("NPC")[i]); //if we want to access the alive sick npcs we can reach them from here
            }
        }
    }

    void LoadEndingScene()
    {
        if (ending == 1)
        {
            SceneManager.LoadScene("KillGodScene");
        }
        if (ending == 2)
        {
            SceneManager.LoadScene("SwitchScene");
        }
        if (ending == 3)
        {
            SceneManager.LoadScene("SacrificeScene");
        }
        if (ending == 4)
        {
            SceneManager.LoadScene("OtherEnding");
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Ground")
        {
            jumps = 1;
            PlayerAnimationState = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPC" && PlayerState == 0)
        {
            PlayerState = 1;
            NPC = col.gameObject;
            SetSpaceOptionsPrompts();
        }
        else if (col.gameObject.tag == "Companion")
        {
            lookingatcompanion = true;
            SetCompanionPrompt();
        }
        if (col.gameObject.tag == "Boss" && PlayerState == 0)
        {
            PlayerState = 11;
            SetBossSpaceOptionsPrompts();
            //Set Boulder Position
            if (GameObject.Find("DrunkMiner").GetComponent<NPCScript>().alive)
            {
                GameObject.Find("Boulder").transform.position = new Vector2(185, 12.5f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "NPC" && PlayerState == 0 && NarrationManager.instance.isPlaying == false)
        {
            PlayerState = 1;
            NPC = col.gameObject;
            SetSpaceOptionsPrompts();
        }
        else if (col.gameObject.tag == "Companion")
        {
            lookingatcompanion = true;
            SetCompanionPrompt();
        }
        else if (col.gameObject.tag == "Boss" && PlayerState == 0)
        {
            PlayerState = 11;
            SetBossSpaceOptionsPrompts();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "NPC" && PlayerState == 1)
        {
            PlayerState = 0;
            SetSpaceOptionsPrompts();
        }
        if (col.gameObject.tag == "Companion")
        {
            lookingatcompanion = false;
            SetCompanionPrompt();
        }
        if (col.gameObject.tag == "Boss" && PlayerState == 11)
        {
            PlayerState = 0;
            SetBossSpaceOptionsPrompts();
        }
    }
}
