using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{

    #region Public Fields

    static public GameManager Instance;

    #endregion

    #region Private Fields

    private GameObject instance;
    private Text ReadyButtonText;
    [Tooltip("玩家的Prefab")]
    [SerializeField]
    private GameObject playerPrefab = null;

    public GameObject EscMenu;
    public GameObject BlueWinWindows;
    public GameObject RedWinWindows;
    public Animator DoorAnim;
    public Text RedTeamNumberText;
    public Text BlueTeamNumberText;
    public Text NoTeamNumberText;

    public int playerNumber;
    public int playerReady;
    public int RedTeamNumber;
    public int BlueTeamNumber;
    public bool isStartGame;

    private bool esc;

    #endregion

    #region MonoBehaviour CallBacks

    void Start()
    {
        Instance = this;
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Launcher");
            return;
        }
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    void Update()
    {
        playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        RedTeamNumberText.text = "紅隊："+RedTeamNumber;
        BlueTeamNumberText.text = "藍隊：" + BlueTeamNumber;
        NoTeamNumberText.text = "尚未分配隊伍：" + (playerNumber - RedTeamNumber - BlueTeamNumber);

        if (playerNumber == playerReady && RedTeamNumber >=1 && BlueTeamNumber >=1)
        {
            DoorAnim.SetBool("DoorOpen", true);
            isStartGame = true;
        }
        //Esc離開遊戲
        if (Input.GetKeyDown(KeyCode.F12))
        {
            QuitApplication();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (esc)
            {
                esc = false;
            }
            else
            {
                esc = true;
            }
            EscMenu.SetActive(esc);
        }
        if (isStartGame)
        {
            if (RedTeamNumber == 0)
                BlueWinWindows.SetActive(true);
            if (BlueTeamNumber == 0)
            {
                RedWinWindows.SetActive(true);
            }
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName + "玩家 加入了房間！"); // not seen if you're the player connecting

    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    /*//回閃退
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }*/
    public void QuitApplication()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerReady);
            stream.SendNext(RedTeamNumber);
            stream.SendNext(BlueTeamNumber);
        }
        else
        {
            playerReady = (int)stream.ReceiveNext();
            RedTeamNumber = (int)stream.ReceiveNext();
            BlueTeamNumber = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void Number(int cases)
    {
        switch (cases)
        {
            case 1:
                RedTeamNumber++;
                break;
            case 2:
                BlueTeamNumber++;
                break;
            case 3:
                RedTeamNumber--;
                break;
            case 4:
                BlueTeamNumber--;
                break;
            case 5:
                playerReady++;
                break;
            case 6:
                playerReady--;
                break;
        }
    }
}

