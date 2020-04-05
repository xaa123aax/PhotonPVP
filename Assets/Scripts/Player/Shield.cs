using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shield : MonoBehaviourPun
{
    public PlayerAnim playerAnim;
    public PlayerManager playerManager;
    public GameObject pos;
    public GameObject Player;
    void FixedUpdate()
    {
        transform.position = pos.transform.position;
        transform.rotation = Player.transform.rotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (playerAnim.gameObject.tag == "RedPlayer")
        {
            if (other.CompareTag("BlueSword"))
            {
                playerManager.CoSp(10);
                other.enabled = false;
                if (playerManager.PlayerSp <= 0)
                {
                    playerAnim.Player_ani.SetBool("BreakShield", true);
                }
                else
                {
                    playerAnim.Player_ani.SetBool("Hurt", true);
                }
            }
            else if (other.CompareTag("BlueBigSword"))
            {
                playerManager.CoSp(30);
                other.enabled = false;
                if (playerManager.PlayerSp <= 0)
                {
                    playerAnim.Player_ani.SetBool("BreakShield", true);
                }
                else
                {
                    playerAnim.Player_ani.SetBool("Hurt", true);
                }
            }
        }
        else if (playerAnim.gameObject.tag == "BluePlayer")
        {
            if (other.CompareTag("RedSword"))
            {
                playerManager.CoSp(10);
                other.enabled = false;
                if (playerManager.PlayerSp <= 0)
                {
                    playerAnim.Player_ani.SetBool("BreakShield", true);
                }
                else
                {
                    playerAnim.Player_ani.SetBool("Hurt", true);
                }
            }
            else if (other.CompareTag("RedBigSword"))
            {
                playerManager.CoSp(30);
                other.enabled = false;
                if (playerManager.PlayerSp <= 0)
                {
                    playerAnim.Player_ani.SetBool("BreakShield", true);
                }
                else
                {
                    playerAnim.Player_ani.SetBool("Hurt", true);
                }
            }
        }
    }
}
