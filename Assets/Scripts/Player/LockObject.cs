using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LockObject : MonoBehaviour
{
    public GameObject VirtualTrackObject;
    public GameObject VirtualCamera;
    public GameObject Player;
    public List<GameObject> inViewTarget = null;
    public CinemachineVirtualCamera LockCam;
    public bool isLock;
    private GameObject Target;
    private Collider SearchRange;
    private int PlayerTeam;
    private float LostTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (VirtualCamera.GetComponent<CinemachineVirtualCamera>())
        {
            LockCam = VirtualCamera.GetComponent<CinemachineVirtualCamera>();
            Target = null;
        }
        if (GetComponent<CapsuleCollider>())
        {
            SearchRange = GetComponent<CapsuleCollider>();
        }
        PlayerTeam = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Target != null)
        {
            VirtualTrackObject.transform.LookAt(Target.transform);
            LockCam.LookAt = Target.transform;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Search(this.transform.position, 100, 180);
            if (LockCam.Priority == 10 && inViewTarget[0] != null)
            {
                Target = inViewTarget[0];
                LockCam.Priority = 12;
                isLock = true;
            }
            else
            {
                inViewTarget.Clear();
                LockCam.Priority = 10;
                isLock = false;
            }
        }
        /*if(LockCam.Priority == 12)
        {
            if (IsLost())
            {
                LostTime += Time.deltaTime;
                if (LostTime > 1)
                {
                    inViewTarget.Clear();
                    LockCam.Priority = 10;
                }
            }
            else
            {
                LostTime = 0;
            }
        }*/

    }
    void Search(Vector3 CurrentPos, float SearchRadius, float FOV)
    {
        Collider[] GaChaObj = Physics.OverlapCapsule(this.transform.position + Vector3.up * 10,
            this.transform.position - Vector3.up * 10, SearchRadius, LayerMask.GetMask("Player"));
        for (int i = 0; i < GaChaObj.Length; i++)
        {
            Vector3 ObjPos = GaChaObj[i].transform.position;
            if (Vector3.Angle(this.transform.forward, (ObjPos - this.transform.position)) <= FOV / 2)
            {
                RaycastHit information;
                int layerMask = LayerMask.GetMask("Obstacles", "Player");
                Debug.DrawRay(transform.position, ObjPos - this.transform.position, Color.blue);
                Physics.Raycast(this.transform.position, ObjPos - this.transform.position,
                    out information, Mathf.Infinity, layerMask);
                if (information.collider == GaChaObj[i])
                {
                    if (GaChaObj[i].gameObject.tag != Player.tag)
                    inViewTarget.Add(GaChaObj[i].gameObject);
                }
            }
        }
        Debug.Log(inViewTarget.Count);
        if (inViewTarget.Count > 1)
        {
            for (int i = 1; i < inViewTarget.Count; i++)
            {
                if (Vector3.Distance(inViewTarget[0].transform.position, this.transform.position) >
                    Vector3.Distance(inViewTarget[i].transform.position, this.transform.position))
                {
                    inViewTarget[0] = inViewTarget[i];
                }
            }
        }
    }
    bool IsLost()
    {
        RaycastHit FirstTouch;
        int layerMask = LayerMask.GetMask("Obstacles" , "Player");
        if (Physics.Raycast(this.transform.position, Target.transform.position - this.transform.position,
            out FirstTouch, Mathf.Infinity, layerMask))
        {
            if (FirstTouch.transform != Target.transform)
            {
                return true;
            }
        }
        return false;
    }
}
