using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIChatAutoSize : MonoBehaviour
{
    //APIChatAreaのサイズを調整する
    public GameObject APIChatArea;
    public GameObject NavigationBar;
    public GameObject HeaderNodge;

    // Start is called before the first frame update
    void Start()
    {
        SetAPIChatAreaSize();
        SetHeaderNodge();
    }

    public void SetAPIChatAreaSize()
    {
        float NavigationBarHeight = NavigationBar.GetComponent<RectTransform>().rect.height;
        //APIChatAreaのサイズを調整する
        RectTransform rectTransform = APIChatArea.GetComponent<RectTransform>();
        float MainHeight = Screen.height - NavigationBarHeight;
        float MainWidth = Screen.width;
        //APIChatAreaのサイズを調整する
        //rectTransform.sizeDelta = new Vector2(MainWidth,MainHeight);
        //APIChatAreaの位置を調整する
        //rectTransform.anchoredPosition = new Vector2(0,NavigationBarHeight/2);
    }

    //Header-NodgeをWebの場合は非表示にする
    public void SetHeaderNodge()
    {
        //Header-NodgeをWebの場合は非表示にする
        if (Application.platform == RuntimePlatform.WebGLPlayer )
        {
            HeaderNodge.SetActive(false);
        }
        #if UNITY_EDITOR
            HeaderNodge.SetActive(false);
        #endif
    }
}
