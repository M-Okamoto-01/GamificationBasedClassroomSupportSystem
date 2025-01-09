using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using TMPro;
using Shapes2D;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// TimelineMaster
///
/// タイムラインを操るクラス
/// 
/// </summary>

public class TimelineMaster : MonoBehaviour
{
    // JavaScript関数を呼び出すためのインターフェース
    [DllImport("__Internal")]
    private static extern string GetBrowserTime();

    //タイムラインデータ
    public List<TimelineData> TLDList;
    public List<GameObject> TLDMainObjectList;
    public List<GameObject> TLDRepObjectList;
    
    //ScrollViewのContent
    public GameObject SVContent;
    public GameObject TimelineObj;
    public GameObject UpperArea;
    public GameObject UpperHeader;
    public GameObject UpperSV;
    public GameObject CoverArea;
    public GameObject CoverHeader;
    public GameObject CoverSV;
    public GameObject CoverSB;
    public GameObject UnderArea;
    public GameObject UnderCoverArea;
    public GameObject UnderCoverHeader;
    public GameObject UnderCoverMC;
    public GameObject PublishTextObj;
    public GameObject RoomNameObj;
    public GameObject GoodBadObj;
    //返信用
    public GameObject SVContentRep;
    public GameObject RepPrehab;

    //部屋番号
    public string RoomID;
    public string MeUUID;
    public string UserName;
    public bool OwnerFlag;
    public RoomData[] RoomData;

    public TimelineGraphQL timelineGraphQL;

    //更新用ボタン
    public GameObject ReloadButton;


    //初期化する
    void Start()
    {
        //初期設定
        GetDDData();
        SetScreenSetting();
        //SetAreaSize();
        FirstSet();

        TLDList = new List<TimelineData>();
        TLDRepObjectList = new List<GameObject>();
        
        //ScrollViewのContent
        SVContent = GameObject.FindGameObjectWithTag("SVContentMain");
        SVContentRep = GameObject.FindGameObjectWithTag("SVContent");

        //先にゴミを生成しておく
        TimelineData delTLD = new TimelineData();
        delTLD.RegisterDateUUID = "null";
        delTLD.HeartSenderUUIDList = new string[0];
        delTLD.ReplyRegisterDateUUIDList = new string[0];
        MakeRepObject(delTLD);

        StartCoroutine(GetTimelineData());

        //GoodBad
        GoodBadObj.GetComponent<GoodBadMain>().OnStartImage();

        //Debug.Log(MeUUID);
        StartCoroutine(ReloadAll());

    }

    private IEnumerator ReloadAll()
    {
        while(true)
        {
            //5分ごとにリロードする
            yield return new WaitForSeconds(300);
            //UpperSVが表示されているか
            if(UpperSV.activeSelf == true)
            {
                StartCoroutine(GetTimelineData());
            }
        }
        
    }


    private void SetTestData()
    {
        MeUUID = GetUUID();
        UserName = "テストユーザー";
        RoomID = "00000";
        RoomData = new RoomData[1];
        RoomData[0] = new RoomData();
        RoomData[0].RoomID = "test";
        RoomData[0].RoomName = "テストルーム";
        RoomData[0].OwnerID = "testOwner";
        RoomData[0].OwnerName = "テストオーナー";
        RoomData[0].Keyword = new string[0];
    }

    private void GetDDData()
    {
        //DDDataを取得する
        GameObject DDDataObj = GameObject.FindGameObjectWithTag("DDData");
        if (DDDataObj != null)
        {
            DDData DDData = DDDataObj.GetComponent<DDData>();
            MeUUID = DDData.UserUUID;
            UserName = DDData.UserName;
            RoomID = DDData.RoomData[0].RoomID;
            RoomData = DDData.RoomData;
            OwnerFlag = DDData.OwnerFlag;
            DDDataObj.GetComponent<DDData>().SavePlayerPrefs();
        }
        else
        {
            SetTestData();
        }
    }

    private void FirstSet()
    {
        RoomNameObj.GetComponent<TMP_Text>().text = RoomData[0].RoomName;
    }

    //スクリーン設定
    private void SetScreenSetting()
    {
        //FPSの設定
        Application.targetFrameRate = 30;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    /// <summary>
    /// GetTimelineData
    /// 
    /// AppSyncからタイムラインデータを取得する
    /// </summary>
    public IEnumerator GetTimelineData()
    {
        //タイムラインデータを取得する
        AllDeleteTimelineObj();
        TimelineData[] TLDL = null;
        yield return StartCoroutine(timelineGraphQL.GetAllData(RoomID, (output) => TLDL = output));
        if (TLDL == null) yield break;
        //取得したデータを入れる
        TLDList = TLDL.ToList();
        //時間でソートする
        TLDList.Sort((a, b) => b.SendDateTime.CompareTo(a.SendDateTime));
        //取得したデータを元にタイムラインを作成する
        foreach(TimelineData solo in TLDList)
        {
            //返信かどうか確認する
            bool RepFlag = false;
            foreach(TimelineData solo2 in TLDList)
            {
                if (solo2.ReplyRegisterDateUUIDList.Contains(solo.RegisterDateUUID)) RepFlag = true;
            }

            if(RepFlag == false)
            {
                MakeTimeline(solo);
            }
            
        }
    }

    public void AllDeleteTimelineObj()
    {
        foreach(GameObject solo in TLDMainObjectList)
        {
            Destroy(solo);
        }
        TLDMainObjectList = new List<GameObject>();
    }

    private string GetUUID()
    {
        var guid = System.Guid.NewGuid();
        return guid.ToString();
    }

    private void MakeTimeline(TimelineData TD)
    {
        //設置する
        GameObject TimelineSolo = Instantiate(TimelineObj, SVContent.transform, false);
        TimelineSolo.GetComponent<TimelineSolo>().CurrentUUID = TD.RegisterDateUUID;
        TimelineSolo.GetComponent<TimelineSolo>().ValueInto();
        TimelineSolo.GetComponent<TimelineSolo>().CoverArea = CoverArea;
        TLDMainObjectList.Add(TimelineSolo);
        //高さの調節する
        //TimelineSolo.GetComponent<RectTransform>().sizeDelta = new Vector2(TimelineSolo.GetComponent<RectTransform>().sizeDelta.x,
        //TimelineSolo.GetComponent<TimelineSolo>().GetSumHeight());
    }

    /// <summary>
    /// updateTimeline
    ///
    /// Timeline上に作成されているTimelineを更新する
    /// </summary>
    /// <param name="UUID">更新対象のUUID</param>
    private void updateTimeline(string UUID)
    {
        foreach(GameObject solo in TLDMainObjectList)
        {
            TimelineSolo TLS = solo.GetComponent<TimelineSolo>();

            if (TLS != null)
            {
                if(TLS.CurrentUUID == UUID)
                {
                    TLS.ValueInto();
                }
            }
        }
    }

    /// <summary>
    /// SetAreaSize
    ///
    /// 初期時と画面サイズ変更時に対応させる
    /// 
    /// </summary>
    public void SetAreaSize()
    {
        return;

        //画面サイズの取得
        float WidthSize = Screen.width;
        float HeightSize = Screen.height;

        //場所の確定
        UpperArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        CoverArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(WidthSize, 0);
        UnderArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        UnderCoverArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * HeightSize);
        //サイズの設定
        UpperArea.GetComponent<RectTransform>().sizeDelta = new Vector2(WidthSize, HeightSize - 168);
        CoverArea.GetComponent<RectTransform>().sizeDelta = new Vector2(WidthSize, HeightSize - 168);
        UnderCoverArea.GetComponent<RectTransform>().sizeDelta = new Vector2(WidthSize, HeightSize - 168);


        //ヘッダ部の調節
        //SafeAreaで変わる
        float HeaderSize = 100;
        if(Screen.safeArea.height != Screen.height)
        {
            //iPhoneX 以降のサイズ
            HeaderSize = HeightSize - Screen.safeArea.height;
            if (HeaderSize < 120) HeaderSize = 120;
            //アイコンを調節する
            UnderArea.GetComponent<HorizontalLayoutGroup>().padding.top = 75;
            UnderArea.GetComponent<HorizontalLayoutGroup>().padding.bottom = 75;
        }
        else
        {
            //iPhone8 iPadのサイズ
            HeaderSize = 70;
            //アイコンを調節する
            UnderArea.GetComponent<HorizontalLayoutGroup>().padding.top = 20;
            UnderArea.GetComponent<HorizontalLayoutGroup>().padding.bottom = 20;
        }

        //Upper
        SetHeaderSize(UpperHeader,UpperSV,HeaderSize);
        SetHeaderSize(CoverHeader, CoverSV, HeaderSize);
        SetHeaderSize(UnderCoverHeader, UnderCoverMC, HeaderSize);
        //Under
        UnderArea.GetComponent<RectTransform>().sizeDelta = new Vector2(WidthSize, HeaderSize);
        //UnderCoverArea.GetComponent<RectTransform>().sizeDelta = new Vector2(WidthSize, HeaderSize);
    }

    private void SetHeaderSize(GameObject HeaderObj,GameObject ScrollViewObject,float HeaderSize)
    {
        //画面サイズの取得
        float WidthSize = Screen.width;
        float HeightSize = Screen.height;

        //座標
        HeaderObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        ScrollViewObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1 * HeaderSize);

        //サイズの設定
        HeaderObj.GetComponent<RectTransform>().sizeDelta = new Vector2(WidthSize, HeaderSize);
        ScrollViewObject.GetComponent<RectTransform>().sizeDelta = new Vector2(WidthSize, HeightSize - HeaderSize * 2);
    }

    /// <summary>
    /// 基本的にはTimelineのデータはこのクラスが全て持つ
    /// このMethodはUUIDから取得する
    /// </summary>
    public TimelineData GetTimelineDatafromUUID(string UUID)
    {
        foreach(TimelineData solo in TLDList)
        {
            if (solo.RegisterDateUUID == UUID) return solo;
        }

        return new TimelineData();
    }

    /// <summary>
    /// 基本的にはTimelineのデータはこのクラスが全て持つ
    /// このMethodはUUIDからTimelineDataを更新する
    /// </summary>
    public bool UpdateTimelineDatafromTimelineData(TimelineData UpdateTLD)
    {
        int nowindex = 0;
        foreach(TimelineData solo in TLDList)
        {
            if(solo.RegisterDateUUID == UpdateTLD.RegisterDateUUID)
            {
                //一致したため更新する
                TLDList[nowindex] = UpdateTLD;
                //更新の反映
                updateTimeline(UpdateTLD.RegisterDateUUID);
                return true;
            }

            nowindex = nowindex + 1;
        }

        return false;
    }

    /// <summary>
    /// AddReplyText
    ///
    /// 返信を追加する
    /// </summary>
    public IEnumerator AddReplyText(string UUID,string ReplyText)
    {
        DateTime PublichTime = System.DateTime.Now.ToLocalTime();

        //時刻の取得
        #if UNITY_WEBGL && !UNITY_EDITOR
            string WebGLTime = GetBrowserTime();
            Debug.Log("WebGL time from JavaScript: " + WebGLTime);
            if (string.IsNullOrEmpty(WebGLTime))
            {
                Debug.LogError("Failed to get time from JavaScript");
                PublichTime = DateTime.UtcNow;  // エラー時には現在のUTC時刻を使用
            }
            else
            {
                PublichTime = ConvertToDateTime(WebGLTime);
            }
        #endif


        //返信用のTimelineData
        TimelineData TLD = new TimelineData(RoomID, GetUUID() + PublichTime.ToString("yyyymmddhhmmss"), PublichTime, MeUUID, UserName,ReplyText);
        //RepであろうがListに放り込む
        TLDList.Add(TLD);
        //返信用のTimelineDataを作成する
        MakeRepObject(TLD);
        //AppSyncに送信する
        bool tmpData = false;
        yield return StartCoroutine(timelineGraphQL.MutationInsert(TLD, (output) => tmpData = output));
        yield return StartCoroutine(timelineGraphQL.MutationReplyUpdate(new AppendReplyRegisterDateUUIDList(UUID, TLD.RegisterDateUUID, RoomID), (output) => tmpData = output));
        //返信されたことを追加するTimelineData
        TimelineData BaseTLD = GetTimelineDatafromUUID(UUID);
        List<string> NewRepUUID = new List<string>(BaseTLD.ReplyRegisterDateUUIDList);
        NewRepUUID.Add(TLD.RegisterDateUUID);
        BaseTLD.ReplyRegisterDateUUIDList = NewRepUUID.ToArray();
        UpdateTimelineDatafromTimelineData(BaseTLD);
        //スクロールを１番下にする
        CoverSB.GetComponent<Scrollbar>().value = 0;

        StartCoroutine(GetTimelineData());
    }

    /// <summary>
    /// MakeRepObject
    ///
    /// 返信用のリプライオブジェクトの作成
    /// </summary>
    /// <param name="TLD">
    /// 
    /// </param>
    public void MakeRepObject(TimelineData TLD)
    {
        //返信用のTimelineDataを作成する
        GameObject ReplyObject = Instantiate(RepPrehab, SVContentRep.transform);
        ReplyObject.GetComponent<TimelineReplySolo>().InputData(TLD);
        TLDRepObjectList.Add(ReplyObject);
    }

    /// <summary>
    /// DeleteAllRepObject
    ///
    /// ReplyObjectを全て消す
    /// </summary>
    public void DeleteAllRepObject()
    {
        //ReplyObjectを全て消す
        foreach(GameObject solo in TLDRepObjectList)
        {
            Destroy(solo);
        }
    }

    /// <summary>
    /// OpenDiscription
    ///
    /// Timelineの詳細が押下された時に発火
    /// 待機してある詳細を全て消し、生成する
    /// </summary>
    public void OpenDiscription(string TargetUUID)
    {
        //対象のTLDを取得
        TimelineData DTLD = GetTimelineDatafromUUID(TargetUUID);
        //ReplyObjectを全て消す
        DeleteAllRepObject();
        //返信を追加していく
        foreach(string solo in DTLD.ReplyRegisterDateUUIDList)
        {
            //返信のデータを取得
            TimelineData RTLD = GetTimelineDatafromUUID(solo);
            MakeRepObject(RTLD);
        }

    }

    /// <summary>
    /// CheckHeartFlag
    ///
    /// 自分自身がいいねしているか入力する
    /// </summary>
    public bool CheckHeartFlag(string TargetUUID)
    {
        //対象のTLDを取得
        TimelineData DTLD = GetTimelineDatafromUUID(TargetUUID);
        //HeartListをみて、MeUUIDが登録されているか確認する
        if(DTLD.HeartSenderUUIDList != null)
        {
            if (DTLD.HeartSenderUUIDList.Contains(MeUUID))
            {
                return true;
            }
        }

        return false;
    }

    public IEnumerator ChangeHeartFlag(string TargetUUID,bool OnFlag)
    {
        //対象のTLDを取得
        TimelineData DTLD = GetTimelineDatafromUUID(TargetUUID);
        TimelineData tmpData = new TimelineData();
        List<string> tmpList = new List<string>() ;
        if(DTLD.HeartSenderUUIDList != null)
        {
            tmpList = DTLD.HeartSenderUUIDList.ToList();
        }
        //消すか増やすかを決める
        if (OnFlag)
        {
            //増やす
            yield return StartCoroutine(timelineGraphQL.MutationHeartUpdate(new AppendReplyRegisterDateUUIDList(TargetUUID,MeUUID,RoomID), (output) => tmpData = output));
        }
        else
        {
            //消す際は最新のデータを取得し、
            //自分のUUIDを消す
            yield return StartCoroutine(timelineGraphQL.GetsoloData(RoomID,TargetUUID, (output) => tmpData = output));
            //ハートのリストから自分のUUIDのインデックスを探す
            List<int> DeleteTargetIndex = new List<int>();
            int nowindex = 0;
            foreach(string solo in tmpData.HeartSenderUUIDList)
            {
                if(solo == MeUUID)DeleteTargetIndex.Add(nowindex);
                nowindex ++;
            }
            foreach(int solo in DeleteTargetIndex)
            {
                yield return StartCoroutine(timelineGraphQL.MutationHeartDeleteUpdate(new DeleteHeartSenderUUIDList(TargetUUID, solo, RoomID), (output) => tmpData = output));
            }
        }

        //更新する
        DTLD.HeartSenderUUIDList = tmpData.HeartSenderUUIDList;
        DTLD.ReplyRegisterDateUUIDList = tmpData.ReplyRegisterDateUUIDList;
        UpdateTimelineDatafromTimelineData(DTLD);
    }

    public void PublishOpenWindow()
    {
        //UnderCoverAreaを表示する
        UnderCoverArea.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f);
    }

    public void PublishCloseWindow()
    {
        //UnderCoverAreaを非表示にする
        UnderCoverArea.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -1 * Screen.height), 0.5f);
    }

    /// <summary>
    /// PublishComment
    /// 
    /// PublishWindowからのコメントを受け取る
    /// そして投稿する
    /// </summary>
    public IEnumerator PublishComment()
    {
        //PublishTextObjからテキストを取得する
        string PublishText = PublishTextObj.GetComponent<TMP_InputField>().text;
        //空だったら無視
        if (PublishText.Replace(" ", "").Replace("　", "") != "")
        {
            DateTime PublichTime = System.DateTime.UtcNow;

            Debug.Log(PublichTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            //時刻の取得
            #if UNITY_WEBGL && !UNITY_EDITOR
                string WebGLTime = GetBrowserTime();
                Debug.Log("WebGL time from JavaScript: " + WebGLTime);
                if (string.IsNullOrEmpty(WebGLTime))
                {
                    Debug.LogError("Failed to get time from JavaScript");
                    PublichTime = DateTime.UtcNow;  // エラー時には現在のUTC時刻を使用
                }
                else
                {
                    PublichTime = ConvertToDateTime(WebGLTime);
                }
            #endif

            //返信処理
            TimelineData TLD = new TimelineData(RoomID,  GetUUID() + PublichTime.ToString("yyyymmddhhmmss"), PublichTime, MeUUID, UserName, PublishText);
            TLDList.Add(TLD);
            //Timelineの生成
            MakeTimeline(TLD);
            //Timelineの送信
            bool tmpFlag = false;
            yield return StartCoroutine(timelineGraphQL.MutationInsert(TLD, (output) => tmpFlag = output));
            //PublishWindowを閉じる
            PublishCloseWindow();
            //PublishTextObjの初期化
            PublishTextObj.GetComponent<TMP_InputField>().text = "";
            StartCoroutine(GetTimelineData());
        }
    }

    /// <summary>
    /// ConvertToDateTime
    /// 
    /// 文字列をDateTimeに変換する
    /// </summary>
    private DateTime ConvertToDateTime(string Time)
    {
        DateTime ResultTime = new DateTime();
        try
        {
            // ISO 8601形式の文字列をDateTimeに変換
            ResultTime = DateTime.Parse(Time, null, System.Globalization.DateTimeStyles.RoundtripKind);
            Debug.Log("現在の時刻 (C# DateTime): " + Time);
        }
        catch (FormatException ex)
        {
            ResultTime = System.DateTime.Now;
            Debug.LogError("日時のパースに失敗しました: " + ex.Message);
        }
        return ResultTime;
    }

    public void OnClickHeader()
    {
        //UpperSVが表示されているか
        if(UpperSV.activeSelf == true)
        {
            //表示されている場合はタイムラインを一番上にする
            SVContent.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f);
        }

    }

    public void OnClickReload()
    {
        StartCoroutine(ExcuteReloadButton());
    }

    public IEnumerator ExcuteReloadButton()
    {
        ReloadButton.SetActive(false);
        yield return StartCoroutine(GetTimelineData());
        yield return StartCoroutine(GoodBadObj.GetComponent<GoodBadMain>().GetGoodBadData_Reload());
        ReloadButton.SetActive(true);
    }

}
