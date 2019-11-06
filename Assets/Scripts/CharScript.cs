﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

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

    public GameObject spacePopup, optionsPopup, companion, companionPopup, reserveHealthBar;

    GameObject cameraGO;

    // Start is called before the first frame update
    void Start()
    {
        jumps = 1;
        speed = 10;
        jumpForce = 16.5f;
        PlayerState = 0;
        sanity = 4;
        reservehealth = 0;

        cameraGO = GameObject.FindGameObjectWithTag("MainCamera");
        intensity = cameraGO.GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>().intensity.value;
        intensity = 0.2f;
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
            if (Input.GetKeyDown(KeyCode.Space) && lookingatcompanion && reservehealth >= 25 && CompScript.comphealth <= 75 && CompScript.comphealth > 0)
            {
                reservehealth -= 25;
                CompScript.comphealth += 25;
            }
        }
        else
        {
            playerbody.velocity = new Vector2(0, playerbody.velocity.y / speed) * speed;
        }

        if (PlayerState == 1 && Input.GetKeyDown(KeyCode.Space) && lookingatcompanion == false)
        {
            PlayerState = 2;
            SetSpaceOptionsPrompts();
        }

        if (PlayerState == 2)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                //talk here
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && NPC.GetComponent<NPCScript>().alive)
            {
                reservehealth += 25;
                if (NPC.GetComponent<NPCScript>().sick == false)
                {
                    reservehealth += 25;
                    sanity--;
                }
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

        if (PlayerState == 3 && Input.GetKeyUp(KeyCode.Space))
        {
            PlayerState = 1;
            SetSpaceOptionsPrompts();
        }
        if (PlayerState == 3) //just to make optionspopup go away 
        {
            SetSpaceOptionsPrompts();
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
    }

    void SetSpaceOptionsPrompts()
    {
        if (NPC.GetComponent<NPCScript>().alive)
        {
            optionsPopup.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            optionsPopup.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }
        else
        {
            optionsPopup.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            optionsPopup.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        }
        if (reservehealth >=50)
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
            spacePopup.transform.position = new Vector2(999, 999);
            optionsPopup.transform.position = new Vector2(NPC.transform.position.x, NPC.transform.position.y + 4);
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
            SetSpaceOptionsPrompts();
        }
        else if (col.gameObject.tag == "Companion")
        {
            lookingatcompanion = true;
            SetCompanionPrompt();
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "NPC" && PlayerState == 0)
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
    }
}
