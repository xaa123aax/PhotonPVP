using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPun
{
    public PlayerManager playerManager;
    private Animator Player_ani;
    public float nowSpeed;
    public Camera PlayerCamera;
    public PlayerAnim player;
    public LockObject lockObject;
    public Vector3 RelativeDir;
    float movespeed = 12f;
    void Start()
    {
        PlayerCamera = Camera.main;
        Player_ani = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        #region Photon部份
        if (!photonView.IsMine)
            return;
        #endregion
        if (!playerManager.IsDead)
        {
            RelativeDir = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            if (Player_ani.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Walk") ||
              Player_ani.GetNextAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Walk"))
            {
                if (Input.GetKey(KeyCode.LeftShift) && playerManager.PlayerSp > 0)
                {
                    if (nowSpeed < Mathf.Max(Mathf.Abs(Input.GetAxisRaw("Horizontal")), Mathf.Abs(Input.GetAxisRaw("Vertical"))))
                    {
                        nowSpeed += 0.02f;
                    }
                    playerManager.CoSp(0.1f);
                    player.isActionTrue();
                }
                else
                {
                    if (nowSpeed > 0)
                    {
                        nowSpeed -= 0.02f;
                    }
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        player.isAction = false;
                    }
                }
                if(!player.cantChangeDir)
                    CharMove(this.transform, PlayerCamera, RelativeDir, nowSpeed, false);
                Player_ani.SetFloat("Speed", nowSpeed);
            }
            else
            {
                if (nowSpeed > 0)
                {
                    nowSpeed = 0;
                    Player_ani.SetFloat("Speed", nowSpeed);
                }
            }
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                Player_ani.SetBool("Walk", true);
            }
            else
            {
                Player_ani.SetBool("Walk", false);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * movespeed * Time.deltaTime, Camera.main.transform);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * movespeed * Time.deltaTime, Camera.main.transform);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * movespeed * Time.deltaTime, Camera.main.transform);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * movespeed * Time.deltaTime, Camera.main.transform);
            }
        }
    }
    public void CharMove(Transform CharTarget, Camera Cam, Vector3 Direct, float Speed, bool IsMoving)
    {
        Vector3 CamForward = Cam.transform.forward;
        Vector3 CamRight = Cam.transform.right;
        Vector3 CamCoordToWorldCoord_Dir = new Vector3(Direct.x * CamForward.x + Direct.z * CamRight.x, 0,
            Direct.z * CamRight.z + Direct.x * CamForward.z);
        CamCoordToWorldCoord_Dir.Normalize();
        if (!IsMoving)
        {
            CharTarget.LookAt
            (
                Vector3.Lerp(CharTarget.position + CharTarget.forward,
                CharTarget.position + CamCoordToWorldCoord_Dir, 0.5f)
            );
            CharTarget.position += CamCoordToWorldCoord_Dir * (3 + Speed * 3) * Time.deltaTime;
        }
        else if (IsMoving)
        {
            CharTarget.LookAt(CharTarget.position + CamCoordToWorldCoord_Dir);
        }
    }
}