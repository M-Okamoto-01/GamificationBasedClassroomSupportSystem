using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class YesNoPanel : MonoBehaviour
{
    //Panel Data
    public GameObject YesNoPanelObject;
    public TMP_Text NoticeText;
    public TMP_Text ContentText;

    private int CallBackType;

    public RoomMaster roomMaster;

    public void ShowYesNoPanel(string notice, string content, int callBackType)
    {
        NoticeText.text = notice;
        ContentText.text = content;
        CallBackType = callBackType;
        YesNoPanelObject.SetActive(true);
    }

    public void OnClickYes()
    {
        switch (CallBackType)
        {
            case 1:
                //Yesの処理
                roomMaster.OnClickYes();
                break;
            default:
                break;
        }
        //Panelを閉じる
        gameObject.SetActive(false);
    }

    public void OnClickNo()
    {
        //Panelを閉じる
        roomMaster.OnClickNo();
        gameObject.SetActive(false);
    }


}
