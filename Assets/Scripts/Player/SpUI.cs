using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpUI : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerAnim player;
    private float maxSP;
    //設置一個有矩形的位置大小等訊息之物件
    public RectTransform thisBar, SPHurtBar, SPHealthBar;
    //耐力刷新量
    private int SPSpeed = 15;
    //比較慢的漸近sp
    private Vector2 SPSlowBar;
    //開始血量sp
    private Vector2 SPiniBar;
    //0
    private Vector2 SPzero;
    public float Slow;
    void Start()
    {
        Slow = 1;
        maxSP = playerManager.PlayerSp;
        SPHealthBar.sizeDelta = new Vector2(maxSP, thisBar.sizeDelta.y);
        SPHurtBar.sizeDelta = new Vector2(maxSP, thisBar.sizeDelta.y);
        thisBar.sizeDelta = new Vector2(maxSP, thisBar.sizeDelta.y);
        SPiniBar = new Vector2(maxSP, thisBar.sizeDelta.y);
        SPSlowBar = new Vector2(Time.deltaTime * SPSpeed, 0);
        SPzero = new Vector2(0, thisBar.sizeDelta.y);
    }
    void Update()
    {
        //如果黃條>橘條
        if (SPHurtBar.sizeDelta.x > SPHealthBar.sizeDelta.x)
        {
            //慢慢地漸進跟上
            SPHurtBar.sizeDelta -= SPSlowBar * 2;
        }
        //如果黃條<橘條
        else if (SPHurtBar.sizeDelta.x < SPHealthBar.sizeDelta.x)
        {
            //兩個相等
            SPHurtBar.sizeDelta = SPHealthBar.sizeDelta;
        }
        else if(SPHurtBar.sizeDelta.x == SPHealthBar.sizeDelta.x && SPHealthBar.sizeDelta.x < maxSP)
        {
            if (!player.isAction)
            {
                SPHealthBar.sizeDelta += SPSlowBar * 3 * Slow;
                playerManager.PlayerSp = SPHealthBar.sizeDelta.x;
                if (SPHealthBar.sizeDelta.x > maxSP)
                {
                    SPHealthBar.sizeDelta = SPiniBar;
                    playerManager.PlayerSp = maxSP;
                }
            }
        }if (SPHealthBar.sizeDelta.x < 0)
        {
            SPHealthBar.sizeDelta = SPzero;
        }
    }
    /// <summary>
    /// 扣sp
    /// </summary>
    public void CoSp(float co)
    {
        SPHealthBar.sizeDelta -= new Vector2(co, 0);
    }
}