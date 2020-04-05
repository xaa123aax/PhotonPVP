// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
#pragma warning disable 649

    /// <summary>
    /// Launch manager. Connect, join a random room or create one if none or all full.
    /// </summary>
    public class Launcher : MonoBehaviourPunCallbacks
    {

        #region Private Serializable Fields

        [Tooltip("Ui Panel讓用戶輸入名稱，連接和播放")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("Ui Text用於通知用戶連接進度")]
        [SerializeField]
        private Text feedbackText;

        [Tooltip("每個房間的最大玩家人數")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The UI Loader Anime")]
        [SerializeField]
        private LoaderAnime loaderAnime;

        #endregion

        #region Private Fields
        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (loaderAnime == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.", this);
            }

            // #Critical
            // 這確保我們可以在主客戶端上使用PhotonNetwork.LoadLevel（）並且同一房間中的所有客戶端自動同步它們的級別
            PhotonNetwork.AutomaticallySyncScene = true;

        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 開始連接過程。
        /// - 如果已連接，我們嘗試加入隨機房間
        /// - 如果尚未連接，請將此應用程序實例連接到Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            // 我們希望確保每次連接時日誌都清晰，如果連接失敗，我們可能會嘗試多次失敗。
            feedbackText.text = "";

            // 跟踪加入房間的意願，因為當我們從遊戲中回來時，我們會得到一個我們連接的回調，所以我們需要知道該做什麼然後
            isConnecting = true;

            // 隱藏播放按鈕以獲得視覺一致性
            controlPanel.SetActive(false);

            // 啟動加載器動畫以獲得視覺效果。
            if (loaderAnime != null)
            {
                loaderAnime.StartLoaderAnimation();
            }

            // 我們檢查是否已連接，如果是，我們加入，否則我們啟動與服務器的連接。
            if (PhotonNetwork.IsConnected)
            {
                LogFeedback("Joining Room...");
                // #此時我們需要嘗試加入隨機房。 如果失敗，我們將在OnJoinRandomFailed（）中收到通知，我們將創建一個。
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {

                LogFeedback("Connecting...");

                // #至關重要的是，我們必須首先連接到Photon Online Server。
                PhotonNetwork.GameVersion = this.gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        /// <summary>
        /// 記錄播放器的UI視圖中的反饋，而不是開發人員的Unity編輯器內部。
        /// </summary>
        /// <param name="message">Message.</param>
        void LogFeedback(string message)
        {
            // 我們不假設有一個feedbackText定義。
            if (feedbackText == null)
            {
                return;
            }

            // 將新消息添加為新行並位於日誌底部。
            feedbackText.text += System.Environment.NewLine + message;
        }

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks
        // below, we implement some callbacks of PUN
        // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


        /// <summary>
        /// Called after the connection to the master is established and authenticated
        /// </summary>
        public override void OnConnectedToMaster()
        {
            // we don't want to do anything if we are not attempting to join a room. 
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
                Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
            }
        }

        /// <summary>
        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        /// </summary>
        /// <remarks>
        /// Most likely all rooms are full or no rooms are available. <br/>
        /// </remarks>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        }


        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnected(DisconnectCause cause)
        {
            LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);
            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
            loaderAnime.StopLoaderAnimation();

            isConnecting = false;
            controlPanel.SetActive(true);

        }

        /// <summary>
        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        /// </summary>
        /// <remarks>
        /// This method is commonly used to instantiate player characters.
        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        ///
        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
        /// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
        /// enough players are in the room to start playing.
        /// </remarks>
        public override void OnJoinedRoom()
        {
            LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

            // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");

                // #Critical
                // Load the Room Level. 
                PhotonNetwork.LoadLevel(1);

            }
        }

        #endregion

    }
}