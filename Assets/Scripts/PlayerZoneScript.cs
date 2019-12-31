using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZoneScript : MonoBehaviour
{
    public GameObject Player;

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = Player.transform.position;
    }
}
