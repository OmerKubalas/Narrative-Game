using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;

public class NarrationTrigger : MonoBehaviour
{
    public Narration speech;
    bool OnNarrationTrigger;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!NarrationManager.instance.isPlaying && CharScript.PlayerState == 3 && Input.GetKeyUp(KeyCode.Space) && OnNarrationTrigger)
        {
            CharScript.PlayerState = 0;
            OnNarrationTrigger = false;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            CharScript.PlayerState = 3;
            OnNarrationTrigger = true;
            NarrationManager.instance.PlayNarration(speech);
        }
    }
}
