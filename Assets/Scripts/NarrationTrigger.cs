using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;

public class NarrationTrigger : MonoBehaviour
{
    public Narration speech;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            CharScript.PlayerState = 3;
            NarrationManager.instance.PlayNarration(speech);
            Destroy(this.gameObject);
        }
    }
}
