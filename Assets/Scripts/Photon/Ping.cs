using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Ping : MonoBehaviour
{
    Text text;
    int i;
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        i = PhotonNetwork.GetPing();
        text.text = "Ping：" + i;
    }
}
