using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public GameObject Player;
    void FixedUpdate()
    {
        transform.position = Player.transform.position;
    }
}
