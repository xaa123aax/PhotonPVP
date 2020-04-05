using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public HpUI hpui;
    public SpUI spui;
    public float PlayerHp = 100;
    public float PlayerSp = 100;

    #region Photon部份
    public GameObject playerUiPrefab;
    public static GameObject LocalPlayerInstance;
    public GameObject PlayerMainCamera;
    public GameObject PlayerFreeCamera;
    public GameObject PlayerUI;
    public GameObject VirtualTrack;
    public GameObject CMvcam1;
    public SkinnedMeshRenderer playmode1;
    public SkinnedMeshRenderer playmode2;
    public SkinnedMeshRenderer playmode3;
    public SkinnedMeshRenderer playmode4;
    public RectTransform playhp;

    public bool IsDead;
    bool Lock_Cursor;

    public void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        if (this.playerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            playhp = GameObject.Find("PlayerUI " + photonView.Owner.NickName + photonView.ViewID).GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            PlayerMainCamera.SetActive(false);
            PlayerFreeCamera.SetActive(false);
            PlayerUI.SetActive(false);
            VirtualTrack.SetActive(false);
            CMvcam1.SetActive(false);
        }
        Lock_Cursor = true;
        IsDead = false;
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (this.PlayerHp <= 0f)
            {
                playmode1.enabled = false;
                playmode2.enabled = false;
                playmode3.enabled = false;
                playmode4.enabled = false;
                playhp.localScale = Vector3.zero;
                IsDead = true;
                // GameManager.Instance.LeaveRoom();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
                Lock_Cursor = !Lock_Cursor;
            if (Lock_Cursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
    #endregion
    public void CoHp(float damage)
    {
        PlayerHp -= damage;
        hpui.cohp(damage);
    }
    public void CoSp(float damage)
    {
        PlayerSp -= damage;
        spui.CoSp(damage);
        if (PlayerSp < 0)
        {
            PlayerSp = 0;
        }
    }
    #region IPunObservable implementation  Photon部分

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.PlayerHp);
            stream.SendNext(playmode1.enabled);
            stream.SendNext(playmode2.enabled);
            stream.SendNext(playmode3.enabled);
            stream.SendNext(playmode4.enabled);
            stream.SendNext(playhp.localScale.x);
            stream.SendNext(playhp.localScale.y);
            stream.SendNext(playhp.localScale.z);
        }
        else
        {
            this.PlayerHp = (float)stream.ReceiveNext();
            playmode1.enabled = (bool)stream.ReceiveNext();
            playmode2.enabled = (bool)stream.ReceiveNext();
            playmode3.enabled = (bool)stream.ReceiveNext();
            playmode4.enabled = (bool)stream.ReceiveNext();
            playhp.localScale = new Vector3((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
        }
    }

    #endregion
}
