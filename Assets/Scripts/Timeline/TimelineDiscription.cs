using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Gilzoide.LottiePlayer;

/// <summary>
/// TimelineDiscription
///
/// タイムラインの詳細を確認する
///
/// //これに換装かも
/// https://www.create-forever.games/swipe-window-change/
/// 
/// </summary>

public class TimelineDiscription : MonoBehaviour
{
    //TimelineMaster
    private TimelineMaster timelineMaster;

    //TimelineReplay
    public GameObject TimelineReplyObj;

    //現在のTimelineData
    private string CurrentUUID;
    private TimelineSolo CurrentTLS;
    //移動系のボタン
    public GameObject BackButton;
    public GameObject CoverArea;
    //表示系
    public GameObject UserNameObj;
    public GameObject MessageObj;
    public GameObject TimeTextObj;
    public GameObject LikeCountObj;
    public GameObject HeartImageObj;
    public GameObject HeartAnimationObj;

    //画像系
    public Sprite HeartNone;
    public Sprite HeartOn;

    //Awake
    private void Awake()
    {
        timelineMaster = GameObject.FindGameObjectWithTag("TimelineMaster").GetComponent<TimelineMaster>();
        HeartAnimationObj.GetComponent<ImageLottiePlayer>().color = new Color32(255, 255, 255, 0);
    }

    //戻るボタンを押下したら戻す
    public void ClickBackButton()
    {
        //Cover前に戻す
        this.GetComponent<EasySwipeClose>().ProcessEnd();
        this.GetComponent<CanvasGroup>().DOFade(0, 0.6f);
        CoverArea.GetComponent<RectTransform>().DOAnchorPos(new Vector3(CoverArea.GetComponent<RectTransform>().rect.width, 0, 0), 0.5f);
    }

    //TimelineMasterから受け取る
    public void PickupTimelineData(string UUID,TimelineSolo TLS)
    {
        //自分自身のTimelineDataに記録する
        this.GetComponent<CanvasGroup>().alpha = 0;
        CurrentTLS = TLS;
        CurrentUUID = UUID;
        TimelineReplyObj.GetComponent<TimelineReply>().GetCurrentUUID(UUID);
        //表示
        ShowTimelineDiscription();
        this.GetComponent<CanvasGroup>().DOFade(1, 0.6f);

        //Webかそうでないかで表示を変える
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer) {
            //Webかそうでないかで表示を変える
            UserNameObj.GetComponent<TMP_Text>().fontSize = 30;
            MessageObj.GetComponent<TMP_Text>().fontSize = 30;
            TimeTextObj.GetComponent<TMP_Text>().fontSize = 25;
            HeartImageObj.GetComponent<RectTransform>().SetHeight(30);
            LikeCountObj.GetComponent<TMP_Text>().fontSize = 30;
        }
    }

    //詳細を表示する
    public void ShowTimelineDiscription()
    {
        //Masterから取得
        TimelineData CurrentTLD = timelineMaster.GetTimelineDatafromUUID(CurrentUUID);
        //タイムラインの詳細を表示させる
        UserNameObj.GetComponent<TMP_Text>().text = CurrentTLD.SenderName;
        MessageObj.GetComponent<TMP_Text>().text = CurrentTLD.content;
        TimeTextObj.GetComponent<TMP_Text>().text = CurrentTLD.SendDateTime.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
        LikeCountObj.GetComponent<TMP_Text>().text = CurrentTLD.HeartSenderUUIDList.Length.ToString() + " Like";
        //自分がいいねを押したかどうか
        if(timelineMaster.CheckHeartFlag(CurrentUUID))
        {
            HeartImageObj.GetComponent<Image>().sprite = HeartOn;
            HeartImageObj.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            HeartImageObj.GetComponent<Image>().sprite = HeartNone;
            HeartImageObj.GetComponent<Image>().color = new Color32(188, 188, 188, 255);
        }
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
            HeartImageObj.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            //いいねをやめる
            HeartAnimationObj.SetActive(false);
            yield return StartCoroutine(timelineMaster.ChangeHeartFlag(CurrentUUID, false));
            HeartImageObj.GetComponent<Image>().sprite = HeartNone;
            HeartImageObj.GetComponent<Image>().color = new Color32(188, 188, 188, 255);
        }
        //Masterから取得
        TimelineData CurrentTLD = timelineMaster.GetTimelineDatafromUUID(CurrentUUID);

        //表示の上書き
        DisplayHeartCount(CurrentTLD.HeartSenderUUIDList.Length);
        timelineMaster.UpdateTimelineDatafromTimelineData(CurrentTLD);
        //Timelineの個別のデータを更新
        CurrentTLS.ValueInto();
    }

    private void DisplayHeartCount(int NowHeartCount)
    {
        LikeCountObj.GetComponent<TMP_Text>().text = NowHeartCount.ToString() + " Like";
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
        HeartImageObj.GetComponent<Image>().sprite = HeartOn;
    }
}
