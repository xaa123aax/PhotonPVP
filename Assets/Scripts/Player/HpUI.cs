using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUI : MonoBehaviour
{
    public PlayerManager playerManager;
    //最大血量
    private float maxHealth;
    //設置一個有矩形的位置大小等訊息之物件
    public RectTransform thisBar, HurtBar, HealthBar;
    //血量刷新量
    private int HpSpeed = 10;
    //比較慢的漸近血量
    private Vector2 SlowBar;
    //開始血量
    private Vector2 iniBar;
    //0
    private Vector2 zero;
    void Start()
    {
        maxHealth = playerManager.PlayerHp;
        HealthBar.sizeDelta = new Vector2(maxHealth, thisBar.sizeDelta.y);
        HurtBar.sizeDelta = new Vector2(maxHealth, thisBar.sizeDelta.y);
        thisBar.sizeDelta = new Vector2(maxHealth, thisBar.sizeDelta.y);
        iniBar = new Vector2(maxHealth, thisBar.sizeDelta.y);
        SlowBar = new Vector2(Time.deltaTime * HpSpeed, 0);
        zero = new Vector2(0, thisBar.sizeDelta.y);
    }
    void FixedUpdate()
    {
        //如果紅條>綠條
        if (HurtBar.sizeDelta.x > HealthBar.sizeDelta.x)
        {
            //慢慢地漸進跟上
            HurtBar.sizeDelta -= SlowBar * 2;
        }
        //如果綠條<=紅條
        else if (HurtBar.sizeDelta.x < HealthBar.sizeDelta.x)
        {
            //兩個相等
            HurtBar.sizeDelta = HealthBar.sizeDelta;
            //玩家血量等於紅條
            maxHealth = HealthBar.sizeDelta.x;
        }
    }
    public void cohp(float co)
    {
        if (HealthBar.sizeDelta.x > 0)
        {
            //扣除一次傷害的size
            HealthBar.sizeDelta -= new Vector2(co, 0);
        }
    }
}