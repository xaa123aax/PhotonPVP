using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerAnim : MonoBehaviourPunCallbacks, IPunObservable
{
    private PlayerManager playerManager;
    private GameManager GameManager;
    private PlayerMove playerMove;
    public LockObject lockObject;
    public Animator Player_ani;

    public BoxCollider Shield, Sword;
    public GameObject sword, SwordTeam, FlySwordPrefab;
    public bool Laying, isAction, isInvincible, cantChangeDir,isDead;
    public SpUI spui;
    float movespeed = 6f;
    

    PlayerUI playerUI;
    Text ReadyButtonText;
    Rigidbody rg;
    CapsuleCollider col;
    BoxCollider boxc;
    bool isReady;
    public bool throwing;
    private int FlySwordCount;
    public Transform FlySwordSpawn;

    //
    Collider[] nearPlayer;
    public List<GameObject> canDeathAttack = null;
    bool canDeathATK;
    //

    #region Start() Update()
    void Start()
    {
        playerUI = GameObject.Find("PlayerUI " + photonView.Owner.NickName + photonView.ViewID).GetComponent<PlayerUI>();
        ReadyButtonText = GameObject.Find("ReadyButton").GetComponentInChildren<Text>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerMove = GetComponent<PlayerMove>();
        playerManager = GetComponent<PlayerManager>();
        Player_ani = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        boxc = GetComponent<BoxCollider>();
        rg = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        #region Photon部份
        if (!photonView.IsMine)
        {
            Player_ani.applyRootMotion = false;
            return;
        }
        else
        {
            SwordTeam.SetActive(false);
        }
        #endregion
        if (!playerManager.IsDead)
        {
            if (Input.GetMouseButton(1))
            {
                Player_ani.SetBool("Block", true);
            }
            else
            {
                Player_ani.SetBool("Hurt", false);
                Player_ani.SetBool("Blocking", false);
                Player_ani.SetBool("Block", false);
                Shield.enabled = false;
            }
            //
            if (Input.GetMouseButtonDown(0))
            {
                canDeathATK = false;
                canDeathAttack.Clear();
                nearPlayer = Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("Player"));
                for (int i = 0; i < nearPlayer.Length; i++)
                {
                    if (nearPlayer[i].gameObject.tag != gameObject.tag)
                    {
                        if (nearPlayer[i].gameObject.GetComponent<PlayerAnim>().Laying == true)
                        {
                            canDeathAttack.Add(nearPlayer[i].gameObject);
                        }
                    }
                }
                if (canDeathAttack.Count > 0)
                {
                    for (int i = 0; i < canDeathAttack.Count; i++)
                    {
                        if (Vector3.Distance(gameObject.transform.position, canDeathAttack[i].transform.position)
                          < Vector3.Distance(gameObject.transform.position, canDeathAttack[0].transform.position))
                        {
                            canDeathAttack[0] = canDeathAttack[i];
                        }
                        canDeathATK = true;
                    }
                }
                if (canDeathATK)
                {
                    Player_ani.SetBool("DeathAttack", true);
                    transform.LookAt(canDeathAttack[0].transform);
                }
                else
                {
                    if (Input.GetKey(KeyCode.LeftControl) && playerManager.PlayerSp >= 40)
                    {
                        Player_ani.SetBool("HeavyAttack", true);
                    }
                    else if (playerManager.PlayerSp >= 10)
                    {

                        Player_ani.SetBool("Attack", true);
                    }
                }
            }
            //
            if (Input.GetKeyDown(KeyCode.Space) && playerManager.PlayerSp >= 20)
            {
                Player_ani.SetBool("Dodge", true);
            }
            if (Input.GetKeyDown(KeyCode.Q) && throwing == false && FlySwordCount < 3)
            {
                throwing = true;
                Player_ani.SetBool("Throw", true);
            }
            if (Input.GetKeyDown(KeyCode.F1) && !GameManager.isStartGame)
            {
                GameManager.playerReady--;
                ReadyButtonText.text = "準備(R鍵)";
                ReadyButtonText.color = Color.black;
                isReady = !isReady;
                if (gameObject.tag == "RedPlayer")
                {
                    GameManager.photonView.RPC("Number", RpcTarget.All, 3);
                }
                if (gameObject.tag == "BluePlayer")
                {
                    GameManager.photonView.RPC("Number", RpcTarget.All, 4);
                }
                playerUI.playerNameText.color = new Color(255, 255, 255);
                gameObject.transform.position = GameObject.Find("StartTp").GetComponent<Transform>().position;
                SwordTeam.tag = "Sword";
                gameObject.tag = "Player";
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!isReady)
                {
                    GameManager.photonView.RPC("Number", RpcTarget.All, 5);
                    ReadyButtonText.text = "已就緒(R鍵)";
                    ReadyButtonText.color = Color.red;
                    isReady = !isReady;
                }
                else
                {
                    GameManager.photonView.RPC("Number", RpcTarget.All, 6);
                    ReadyButtonText.text = "準備(R鍵)";
                    ReadyButtonText.color = Color.black;
                    isReady = !isReady;
                }
            }
        }
        else
        {
            rg.useGravity = false;
            col.enabled = false;
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(Vector3.up * movespeed * Time.deltaTime, Camera.main.transform);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.Translate(Vector3.down * movespeed * Time.deltaTime, Camera.main.transform);
            }
        }
        if (playerManager.PlayerHp <= 0 && !isDead)
        {
            Player_ani.SetBool("Dead", true);
            if(gameObject.tag == "RedPlayer")
                GameManager.photonView.RPC("Number", RpcTarget.All, 3);
            if (gameObject.tag == "BluePlayer")
                GameManager.photonView.RPC("Number", RpcTarget.All, 4);
            isDead = true;
        }
    }
    public void Fire()
    {
        FlySwordCount++;
        GameObject FlySword = (GameObject)Instantiate(
             FlySwordPrefab,
                FlySwordSpawn.position,
                FlySwordSpawn.rotation);


        FlySword.GetComponent<Rigidbody>().velocity = FlySword.transform.forward * 10;
        if (gameObject.tag == "RedPlayer")
            FlySword.tag = "RedSword";
        else if (gameObject.tag == "BluePlayer")
            FlySword.tag = "BlueSword";
        Destroy(FlySword, 2.0f);


    }
    #endregion

    #region Trigger判定
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (gameObject.tag == "RedPlayer")
        {
            if (other.CompareTag("BlueSword") && !isInvincible)
            {
                Player_ani.SetBool("Hurt", true);
                Player_ani.SetBool("Blocking", false);
                playerManager.CoHp(10);
            }
            else if (other.CompareTag("BlueBigSword") && !isInvincible)
            {
                Player_ani.SetBool("KnockDown", true);
                playerManager.CoHp(20);
            }
            else if (other.CompareTag("BlueDeathSword") && isInvincible)
            {
                Player_ani.SetBool("Hurt", true);
                Laying = false;
                playerManager.CoHp(30);
            }
        }
        else if (gameObject.tag == "BluePlayer")
        {
            if (other.CompareTag("RedSword") && !isInvincible)
            {
                Player_ani.SetBool("Hurt", true);
                Player_ani.SetBool("Blocking", false);
                playerManager.CoHp(10);
            }
            else if (other.CompareTag("RedBigSword") && !isInvincible)
            {
                Player_ani.SetBool("KnockDown", true);
                playerManager.CoHp(20);
            }
            else if (other.CompareTag("RedDeathSword") && isInvincible)
            {
                Player_ani.SetBool("Hurt", true);
                Laying = false;
                playerManager.CoHp(30);
            }
        }
        if (other.CompareTag("RedTeam"))
        {
            playerUI.playerNameText.color = new Color(255, 0, 0);
            SwordTeam.tag = "RedSword";
            gameObject.tag = "RedPlayer";
            GameManager.photonView.RPC("Number", RpcTarget.All, 1);
            gameObject.transform.position = GameObject.Find("RedTp").GetComponent<Transform>().position;
        }
        if (other.CompareTag("BlueTeam"))
        {
            playerUI.playerNameText.color = new Color(0, 0, 255);
            SwordTeam.tag = "BlueSword";
            gameObject.tag = "BluePlayer";
            GameManager.photonView.RPC("Number", RpcTarget.All, 2);
            gameObject.transform.position = GameObject.Find("BlueTp").GetComponent<Transform>().position;
        }
    }
    #endregion

    #region IPunObservable implementation  Photon傳值
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.tag);
            stream.SendNext(SwordTeam.tag);
            stream.SendNext(Laying);
            stream.SendNext(playerUI.playerNameText.color.r);
            stream.SendNext(playerUI.playerNameText.color.g);
            stream.SendNext(playerUI.playerNameText.color.b);
        }
        else
        {
            this.tag = (string)stream.ReceiveNext();
            SwordTeam.tag = (string)stream.ReceiveNext();
            Laying = (bool)stream.ReceiveNext();
            playerUI.playerNameText.color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }
    #endregion

    #region 動畫事件
    public void isActionTrue()
    {
        isAction = true;
    }
    public void ComboSp()
    {
        if (SwordTeam.tag.Contains("Blue"))
        {
            SwordTeam.tag = "BlueSword";
        }
        else if (SwordTeam.tag.Contains("Red"))
        {
            SwordTeam.tag = "RedSword";
        }
        isActionTrue();
        playerManager.CoSp(10);
    }
    public void HeavyAttackSp()
    {
        if (SwordTeam.tag.Contains("Blue"))
        {
            SwordTeam.tag = "BlueBigSword";
        }
        else if (SwordTeam.tag.Contains("Red"))
        {
            SwordTeam.tag = "RedBigSword";
        }
        isActionTrue();
        playerManager.CoSp(50);
    }
    public void AttackFalse()
    {
        Player_ani.SetBool("Attack", false);
        Player_ani.SetBool("HeavyAttack", false);
    }
    public void SwordTrue()
    {
        Sword.enabled = true;
    }
    public void SwordFalse()
    {
        Sword.enabled = false;
    }
    public void DodgeSp()
    {
        playerMove.CharMove(this.transform, playerMove.PlayerCamera, playerMove.RelativeDir, playerMove.nowSpeed, true);
        Shield.enabled = false;
        sword.SetActive(true);
        isInvincible = true;
        isActionTrue();
        playerManager.CoSp(20);
    }
    public void isInvincibleFalse()
    {
        isInvincible = false;
    }
    public void DodgingFalse()
    {
        Player_ani.SetBool("Dodging", false);
        Player_ani.SetBool("Dodge", false);
    }
    public void DodgingTrue()
    {
        Player_ani.SetBool("Dodging", true);
    }
    public void KnockDownFalse()
    {
        Player_ani.SetBool("Hurt", false);
        sword.SetActive(true);
        isInvincible = true;
        Player_ani.SetBool("KnockDown", false);
        Player_ani.SetBool("KnockDownIng", true);
    }
    public void Throw()
    {
        ChangeDir();
        isActionTrue();
        sword.SetActive(false);
    }
    public void AllEnd()
    {
        cantChangeDir = false;
        Player_ani.SetBool("Hurt", false);
        sword.SetActive(true);
        isInvincibleFalse();
        isAction = false;
        spui.Slow = 1;
        AttackFalse();
        DodgingFalse();
        DeathAttackFalse();
        Player_ani.SetBool("KnockDownIng", false);
        Player_ani.SetBool("Throw", false);
        throwing = false;
    }
    public void Defense()
    {
        isAction = false;
        spui.Slow = 0.5f;
        Player_ani.SetBool("Blocking", true);
        Shield.enabled = true;
        DodgingFalse();
    }
    public void DeathAttack()
    {
        isInvincible = true;
        transform.LookAt(canDeathAttack[0].transform);
        if (SwordTeam.tag.Contains("Blue"))
        {
            SwordTeam.tag = "BlueDeathSword";
        }
        else if (SwordTeam.tag.Contains("Red"))
        {
            SwordTeam.tag = "RedDeathSword";
        }
    }
    public void DeathAttackFalse()
    {
        Player_ani.SetBool("DeathAttack", false);
    }
    public void HurtFalse()
    {
        sword.SetActive(true);
        SwordFalse();
        Player_ani.SetBool("Hurt", false);
        DodgingTrue();
    }
    public void BreakShieldFalse()
    {
        Player_ani.SetBool("BreakShield", false);
    }
    public void LayingTrue()
    {
        boxc.enabled = true;
        col.enabled = false;
        Laying = true;
    }
    public void StandUp()
    {
        boxc.enabled = false;
        col.enabled = true;
        Player_ani.SetBool("Hurt", false);
        Laying = false;
    }
    public void Die()
    {
        sword.SetActive(true);
        Player_ani.SetBool("Die", true);
    }
    public void ChangeDir()
    {
        if (lockObject.isLock)
        {
            this.transform.LookAt(lockObject.inViewTarget[0].transform.position);
            cantChangeDir = true;
        }
        else
        {
            playerMove.CharMove(this.transform, playerMove.PlayerCamera, playerMove.RelativeDir, playerMove.nowSpeed, true);
        }
    }
    #endregion
}
