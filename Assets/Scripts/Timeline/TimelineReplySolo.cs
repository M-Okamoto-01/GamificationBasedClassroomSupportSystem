using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Gilzoide.LottiePlayer;

/// <summary>
/// TimelineReplySolo
///
/// Timelineでの返信に使用される
/// このクラスに値を送ることで表示される。
/// </summary>

public class TimelineReplySolo : MonoBehaviour
{
    //Master
    private TimelineMaster timelineMaster;

    //このリプライが持っているTimelineData
    public string MeUUID;
    //変数
    private int HeartCount = 0;
    public GameObject UserNameObj;
    public GameObject MessageObj;
    public GameObject HeartCountObj;
    public GameObject HeartIcon;
    public Sprite HeartOn;
    public Sprite HeartNone;
    public GameObject HeartAnimationObj;


    //Awake
    private void Awake()
    {
        timelineMaster = GameObject.FindGameObjectWithTag("TimelineMaster").GetComponent<TimelineMaster>();
        HeartAnimationObj.GetComponent<ImageLottiePlayer>().color = new Color32(255, 255, 255, 0);
    }

    public void InputData(TimelineData TLD)
    {
        //作成された後にデータを引き継ぐ
        MeUUID = TLD.RegisterDateUUID;

        //各種セットしていく
        //変数の代入
        if(TLD.HeartSenderUUIDList != null)
        {
            HeartCount = TLD.HeartSenderUUIDList.Length;
        }
        else
        {
            HeartCount = 0;
        }

        //値の代入
        UserNameObj.GetComponent<TMP_Text>().text = TLD.SenderName;
        MessageObj.GetComponent<TMP_Text>().text = TLD.content;
        HeartCountObj.GetComponent<TMP_Text>().text = HeartCount.ToString() + " Like";

        if (timelineMaster.CheckHeartFlag(MeUUID))
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
            HeartCountObj.GetComponent<TMP_Text>().fontSize = 30;
        }
    }

    public void ClickHeart()
    {
        StartCoroutine(ClickHeart_CountMove());
    }

    private IEnumerator ClickHeart_CountMove()
    {
        //ハートをクリックした時に動かす
        if (timelineMaster.CheckHeartFlag(MeUUID) == false)
        {
            //いいねする
            StartCoroutine(ClickHeartAnimation());
            yield return StartCoroutine(timelineMaster.ChangeHeartFlag(MeUUID, true));
            HeartIcon.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            //いいねをやめる
            HeartAnimationObj.SetActive(false);
            yield return StartCoroutine(timelineMaster.ChangeHeartFlag(MeUUID, false));
            HeartIcon.GetComponent<Image>().sprite = HeartNone;
            HeartIcon.GetComponent<Image>().color = new Color32(188, 188, 188, 255);
        }
        //処理用にTimelineDataをもらう
        TimelineData TimeLineData = timelineMaster.GetTimelineDatafromUUID(MeUUID);

        //更新処理
        DisplayHeartCount(TimeLineData.HeartSenderUUIDList.Length);
        timelineMaster.UpdateTimelineDatafromTimelineData(TimeLineData);
    }

    private void DisplayHeartCount(int NowHeartCount)
    {
        HeartCount = NowHeartCount;
        HeartCountObj.GetComponent<TMP_Text>().text = HeartCount.ToString() + " Like"; ;
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
