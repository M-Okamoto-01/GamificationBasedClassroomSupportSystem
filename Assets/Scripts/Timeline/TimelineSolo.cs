using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Gilzoide.LottiePlayer;

/// <summary>
/// Timelinesolo
/// 
/// タイムラインに表示される一つずつのオブジェクトを管理する
/// オブジェクト生成時に初期化する
/// 
/// </summary>

public class TimelineSolo : MonoBehaviour
{
    //Master
    private TimelineMaster timelineMaster;

    //自分自身
    public GameObject TimelineObj;

    //変数
    public GameObject UserNameObj;
    public GameObject MessageObj;
    public GameObject ActionButton;
    public GameObject CommentIcon;
    public GameObject CommentCountObj;
    public GameObject HeartIcon;
    public GameObject HeartCountObj;
    public GameObject HeartAnimationObj;
    public int CommentCount;
    public int HeartCount;

    public Sprite HeartNone;
    public Sprite HeartOn;

    //TimelineData
    public string CurrentUUID;

    //上から被せる
    public GameObject CoverArea;

    //Awake
    private void Awake()
    {
        timelineMaster = GameObject.FindGameObjectWithTag("TimelineMaster").GetComponent<TimelineMaster>();
        HeartAnimationObj.GetComponent<ImageLottiePlayer>().color = new Color32(255, 255, 255, 0);
    }

    //値の設定
    //及び更新
    public void ValueInto()
    {
        //更新後のTimelineDataを取得
        TimelineData TLD = timelineMaster.GetTimelineDatafromUUID(CurrentUUID);

        //変数の代入
        CommentCount = TLD.ReplyRegisterDateUUIDList.Length;
        HeartCount = TLD.HeartSenderUUIDList.Length;

        //値の代入
        UserNameObj.GetComponent<TMP_Text>().text = TLD.SenderName;
        MessageObj.GetComponent<TMP_Text>().text = TLD.content;
        CommentCountObj.GetComponent<TMP_Text>().text = CommentCount.ToString();
        HeartCountObj.GetComponent<TMP_Text>().text = HeartCount.ToString();

        if (timelineMaster.CheckHeartFlag(CurrentUUID))
        {
            HeartIcon.GetComponent<Image>().sprite = HeartOn;
            HeartIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            HeartIcon.GetComponent<Image>().sprite = HeartNone;
            HeartIcon.GetComponent<Image>().color = new Color32(188, 188, 188, 255);
        }

        //Webかそうでないかで表示を変える
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) {
            //Webかそうでないかで表示を変える
            UserNameObj.GetComponent<TMP_Text>().fontSize = 30;
            MessageObj.GetComponent<TMP_Text>().fontSize = 30;
            HeartIcon.GetComponent<RectTransform>().SetHeight(30);
            CommentIcon.GetComponent<RectTransform>().SetHeight(30);
            HeartCountObj.GetComponent<TMP_Text>().fontSize = 30;
            CommentCountObj.GetComponent<TMP_Text>().fontSize = 30;
        }
    }

    public void SetHeightSizeFit()
    {
        TimelineObj.GetComponent<RectTransform>().sizeDelta = new Vector2(TimelineObj.GetComponent<RectTransform>().rect.width,
                                                                           GetSumHeight());
        
    }

    public float GetSumHeight()
    {
        //全てのオブジェクトの高さを返す
        float ResHeight = 0;
        ResHeight = UserNameObj.GetComponent<RectTransform>().rect.height;
        Debug.Log(UserNameObj.GetComponent<RectTransform>().rect.height);
        ResHeight = ResHeight + MessageObj.GetComponent<RectTransform>().rect.height;
        ResHeight = ResHeight + ActionButton.GetComponent<RectTransform>().rect.height;
        Debug.Log(ResHeight);
        return ResHeight;
    }

    public void ClickHeart()
    {
        StartCoroutine(ClickHeart_CountMove());
    }

    private IEnumerator ClickHeart_CountMove()
    {
        //ハートをクリックした時に動かす
        if (timelineMaster.CheckHeartFlag(CurrentUUID) == false)
        {
            //いいねする
            StartCoroutine(ClickHeartAnimation());
            yield return StartCoroutine(timelineMaster.ChangeHeartFlag(CurrentUUID, true));
            HeartIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            //いいねをやめる
            HeartAnimationObj.SetActive(false);
            yield return StartCoroutine(timelineMaster.ChangeHeartFlag(CurrentUUID, false));
            HeartIcon.GetComponent<Image>().sprite = HeartNone;
            HeartIcon.GetComponent<Image>().color = new Color32(188, 188, 188, 255);
        }
        //処理用にTimelineDataをもらう
        TimelineData TimeLineData = timelineMaster.GetTimelineDatafromUUID(CurrentUUID);

        //更新処理
        DisplayHeartCount(TimeLineData.HeartSenderUUIDList.Length);
        timelineMaster.UpdateTimelineDatafromTimelineData(TimeLineData);
    }

    private void DisplayHeartCount(int NowHeartCount)
    {
        HeartCount = NowHeartCount;
        HeartCountObj.GetComponent<TMP_Text>().text = HeartCount.ToString();
    }

    public void ClickComment()
    {
        //コメントをクリックした時に動かす
        timelineMaster.OpenDiscription(CurrentUUID);
        CoverArea.GetComponent<TimelineDiscription>().PickupTimelineData(CurrentUUID, this.GetComponent<TimelineSolo>());
        CoverArea.GetComponent<EasySwipeClose>().ProcessStart();
        CoverArea.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0, 0, 0), 0.5f);
    }

    private IEnumerator ClickHeartAnimation()
    {
        //ハートをクリックした時に動かす
        HeartAnimationObj.SetActive(true);
        HeartAnimationObj.GetComponent<ImageLottiePlayer>().StopAllCoroutines();
        HeartAnimationObj.GetComponent<ImageLottiePlayer>().color = new Color32(255, 255, 255, 255);
        HeartAnimationObj.GetComponent<ImageLottiePlayer>().Play();
        yield return new WaitForSeconds(1f);
        HeartAnimationObj.GetComponent<ImageLottiePlayer>().StopAllCoroutines();
        HeartAnimationObj.SetActive(false);
        HeartIcon.GetComponent<Image>().sprite = HeartOn;
    }

}
