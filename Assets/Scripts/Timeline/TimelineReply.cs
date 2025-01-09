using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// StructList
///
/// 返信を行うクラス
/// 
/// </summary>

public class TimelineReply : MonoBehaviour
{
    private TimelineMaster timelineMaster;
    public GameObject inputFieldTextObj;
    public GameObject inputFieldObj;
    private string CurrentUUID;

    //Awake
    private void Awake()
    {
        timelineMaster = GameObject.FindGameObjectWithTag("TimelineMaster").GetComponent<TimelineMaster>();
        this.GetComponent<Button>().onClick.AddListener(OnReplyClick);
    }

    /// <summary>
    /// GetNowTimelineData
    ///
    /// Triger:Timelinesolo Tap
    /// 今の詳細を開いているTimelineを取得
    /// </summary>
    public void GetCurrentUUID(string UUID)
    {
        CurrentUUID = UUID;
    }

    /// <summary>
    /// OnReplyClick
    ///
    /// Triger:ReplyButton Tap
    /// 返信を行う
    /// </summary>
    public void OnReplyClick()
    {
        //送る文章の取得
        string Replytext = inputFieldObj.GetComponent<TMP_InputField>().text;
        //空だったら無視
        if(Replytext.Replace(" ","").Replace("　","") != "")
        {
            //返信処理
            StartCoroutine(timelineMaster.AddReplyText(CurrentUUID, Replytext));
            inputFieldObj.GetComponent<TMP_InputField>().text = "";
            //高さの初期化
            inputFieldObj.GetComponent<InputFieldManager>().ResetHeight();
        }
    }
}
