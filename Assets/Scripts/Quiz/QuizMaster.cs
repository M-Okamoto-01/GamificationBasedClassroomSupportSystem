using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QuizMaster : MonoBehaviour
{
    //Inspectorから設定
    public GameObject QuizPanel;
    public GameObject NoQuizPanel;
    public TMP_Text CountTitle;
    public GameObject TimerObject;
    public Slider Timer;
    public GameObject QuestionImagePanel;
    public Image QuestionImage;
    public TMP_Text QuizTitle;
    public GameObject[] AnserButtons;
    public TMP_Text PointText;
    public TMP_Text RankText;

    //クイズデータ
    private List<QuizData> QuizDatasMain = new List<QuizData>();
    //クイズゲームのデータ
    private int QuestionCount = 0;
    public int NowPoint = 0;
    private int Rank = 0;
    public bool WaitFlag = false;

    public GameObject GraphQLObject;

    //部屋番号
    public string RoomID;
    public string MeUUID;
    public string UserName;
    public bool OwnerFlag;
    public RoomData[] RoomData;

    //QuizUIの表示を考える
    public GameObject TeacherArea;
    public GameObject TeacherArea_SelectArea;
    public GameObject TeacherArea_EditArea;
    public GameObject TeacherArea_ShowArea;
    public GameObject WatingArea;
    public GameObject WatingArea_TeacherArea;
    public GameObject WatingArea_StudentArea;
    public GameObject WatingArea_StudentArea_NoQuizArea;
    public GameObject WatingArea_StudentArea_StudentWait;
    public GameObject QuizMain;
    public GameObject CorrectObject;
    public GameObject Student_EndButton;
    public GameObject WaitText;

    //クイズ情報
    public QuizTable NowQuizTable;

    public bool QuizStartFlag = false;

    public GameObject RankArea;


    // Start is called before the first frame update
    void Awake()
    {
        QuizStartFlag = false;
        GetDDData();
        initArea();
        init_AreaSel();
        WaitText.SetActive(false);
    }

    void Start()
    {
        //クイズがはじめっているかどうかで分岐
        if(QuizStartFlag == false)
        {
            if(OwnerFlag)
            {
                //Init_Teacher();
            }
            else
            {
                //Init_Student();
            }
        }
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
        }
        else
        {
            SetTestData();
        }
    }

    private void SetTestData()
    {
        MeUUID = "TestUser";
        UserName = "テストユーザー";
        RoomID = "00000";
        RoomData = new RoomData[1];
        RoomData[0] = new RoomData();
        RoomData[0].RoomID = "test";
        RoomData[0].RoomName = "テストルーム";
        RoomData[0].OwnerID = "testOwner";
        RoomData[0].OwnerName = "テストオーナー";
        RoomData[0].Keyword = new string[0];
        OwnerFlag = false;
    }

    public void LoadQuizData(QuizGameTable qgt)
    {
        //クイズデータの取得
        StartCoroutine(GraphQLObject.GetComponent<QuizGraphQL>().GetQuiz(qgt.RoomID,qgt.QuizID, (output) => {
            if (output != null)
            {
                NowQuizTable = output;
                //クイズデータをリストに追加
                QuizDatasMain = new List<QuizData>();
                foreach (var quizDataItem in output.QuizMainData.QuizDatas)
                {
                    QuizDatasMain.Add(quizDataItem);
                }
                StartQuiz();
            }
        }));
    }

    //スタートクイズ
    public void StartQuiz()
    {
        //クイズを開始
        QuestionCount = 0;
        NowPoint = 0;
        Rank = 1;
        WatingArea.SetActive(false);
        ShowPoint();
        ShowQuiz();

        RankArea.SetActive(true);
        RankArea.GetComponent<RankMaster>().initQuiz();
        RankArea.SetActive(false);

    }

    public void StartQuiz_Teacher()
    {
        //クイズを開始
        WatingArea.SetActive(false);
        TeacherArea.SetActive(true);
        TeacherArea_SelectArea.SetActive(true);
        TeacherArea_ShowArea.SetActive(true);
        TeacherArea_ShowArea.GetComponent<QuizResultMaster>().GetQuizData(NowQuizTable);
        //待機の解除
        StartCoroutine(GraphQLObject.GetComponent<QuizGraphQL>().MutationQuizGameDelete(RoomID, (result) => { }));
    }

    //クイズの表示
    public void ShowQuiz()
    {
        //クイズデータがない場合はNoQuizPanelを表示
        if (QuizDatasMain.Count == 0)
        {
            NoQuizPanel.SetActive(true);
            QuizPanel.SetActive(false);
            RankArea.SetActive(false);
            QuizStartFlag = false;
            return;
        }
        else
        {
            NoQuizPanel.SetActive(false);
            QuizPanel.SetActive(true);
            RankArea.SetActive(false);
            QuizStartFlag = true;
        }

        if (QuestionCount >= QuizDatasMain.Count)
        {
            //クイズが終了した場合
            RankArea.SetActive(true);
            Student_EndButton.SetActive(true);
            QuizStartFlag = false;
            return;
        }
        else
        {
            Student_EndButton.SetActive(false);
            QuizStartFlag = true;
        }

        if (QuestionCount != 0)
        {
            //クイズのランクを取得
            Rank = RankArea.GetComponent<RankMaster>().GetRank_toUseStatic();
        }
        ShowPoint();

        //クイズのカウントを表示
        CountTitle.text = (QuestionCount + 1) + " / " + QuizDatasMain.Count;
        //クイズデータを取得
        QuizData quizData = QuizDatasMain[QuestionCount];
        //OriginalIndexを初期化
        for (int i = 0; i < quizData.Slections.Length; i++)
        {
            quizData.Slections[i].OriginalIndex = i;
        }

        //Selectionsをシャッフル
        //シャッフル前の選択肢を取得
        for (int i = 0; i < quizData.Slections.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, quizData.Slections.Length);
            QuizSlection temp = quizData.Slections[i];
            quizData.Slections[i] = quizData.Slections[randomIndex];
            quizData.Slections[randomIndex] = temp;
        }

        //クイズのタイトルを設定
        QuizTitle.text = quizData.QuizTitle;

        //クイズの画像を設定
        if (quizData.ImageURL != null && quizData.ImageURL != "")
        {
            QuestionImagePanel.SetActive(true);
            QuestionImage.sprite = Resources.Load<Sprite>(quizData.ImageURL);
        }
        else
        {
            QuestionImagePanel.SetActive(false);
        }

        //クイズの選択肢を設定
        for (int i = 0; i < AnserButtons.Length; i++)
        {
            Debug.Log("OriginalIndex:" + quizData.Slections[i].OriginalIndex.ToString());
            AnserButtons[i].GetComponent<SelectionsMaster>().SetQuizData(quizData.Slections[i], quizData.ImageURL,this,quizData.Slections[i].OriginalIndex);
            //Selectを解除する
            StartCoroutine(ResetButtonInteractable(AnserButtons[i].GetComponent<Button>()));
        }
        //タイマーをスタート
        TimerObject.GetComponent<QuizTimer>().StartTimer(this);
        WaitFlag = true;

        QuestionCount++;
    }

    private IEnumerator ResetButtonInteractable(Button button)
    {
        button.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(0.1f);
        button.GetComponent<Button>().interactable = true;
    }

    //ポイントを表示
    public void ShowPoint()
    {
        PointText.text = NowPoint.ToString();
        RankText.text = "Rank." + Rank;
    }

    //クイズの選択肢がクリックされた時
    public void OnClickSelection(QuizSlection quizSlection,int OriginalIndex)
    {
        //クイズの選択肢がクリックされた時の処理
        WaitFlag = false;
        TimerObject.GetComponent<QuizTimer>().StopTimer();
        float stoptime = TimerObject.GetComponent<QuizTimer>().GetTime();
        for (int i = 0; i < AnserButtons.Length; i++)
        {
            AnserButtons[i].GetComponent<SelectionsMaster>().ShowCorrect();
        }
        CorrectObject.SetActive(true);
        StartCoroutine(CorectExcute(quizSlection,OriginalIndex,stoptime));
        
    }

    public IEnumerator CorectExcute(QuizSlection quizSlection,int OriginalIndex,float stoptime)
    {
        //答えの表示の時間差分を求める
        DateTime startTime = DateTime.Now;
        //答えの正否を表示
        WaitText.SetActive(true);
        yield return StartCoroutine(CorrectObject.GetComponent<CorrectMaster>().SetCorrect(quizSlection,this,OriginalIndex));
        //答えの表示の時間差分を求める
        DateTime endTime = DateTime.Now;
        TimeSpan ts = endTime - startTime;
        stoptime -= (float)ts.TotalSeconds;
        //ランクを表示する前に、足並みをほぼ合わせる
        yield return new WaitForSeconds(stoptime);
        WaitText.SetActive(false);
        //ランクの表示
        ShowAllRank();
        //5秒待つ
        yield return new WaitForSeconds(5.0f);
        RankArea.SetActive(false);
        //次のクイズを表示
        ShowQuiz();
    }

    public IEnumerator SkipQuiz()
    {
        WaitText.SetActive(false);
        //ランクの表示
        ShowAllRank();
        //5秒待つ
        yield return new WaitForSeconds(5.0f);
        RankArea.SetActive(false);
        //次のクイズを表示
        ShowQuiz();
    }

    public IEnumerator PointGet(QuizSlection quizSlection,int OriginalIndex)
    {
        //ポイント取得時の処理
        int getPoint = 0;
        yield return StartCoroutine(TimerObject.GetComponent<QuizTimer>().GetPoint(this, PointText, (point) => {
            getPoint = point;
        } ));
        //自分のポイントの更新
        QuizWaitingTable quizWaitingTable = new QuizWaitingTable();
        quizWaitingTable.RoomID = RoomID;
        quizWaitingTable.QuizID = NowQuizTable.QuizID;
        quizWaitingTable.TargetQuizIndex = 1;
        quizWaitingTable.UserUUID = MeUUID;
        quizWaitingTable.Point = NowPoint;
        StartCoroutine(GraphQLObject.GetComponent<QuizGraphQL>().MutationQuizWaitingUpsert(quizWaitingTable, (result) => { }));
        SendScoreData(quizSlection,getPoint,OriginalIndex);
        //RankMasterにも通知
        RankArea.GetComponent<RankMaster>().UpdateUserPoint_toUseStatic(quizWaitingTable);
        
    }

    //教員に回答情報を連携する
    public void SendScoreData(QuizSlection quizSlection,int point,int OriginalIndex)
    {
        //クイズの回答情報の送信
        QuizScoreTable quizScoreTable = new QuizScoreTable();
        quizScoreTable.RoomID = RoomID;
        quizScoreTable.QuizID = NowQuizTable.QuizID;
        quizScoreTable.UserUUID = MeUUID;
        quizScoreTable.QuizIndex = QuestionCount - 1;
        quizScoreTable.UserUUIDQuizIDIndex = MeUUID + "_" + NowQuizTable.QuizID.ToString("0000") + "_" + (QuestionCount - 1).ToString("0000");
        quizScoreTable.Selectindex = new int[1] {OriginalIndex};
        quizScoreTable.CorrectFlag = quizSlection.CorrectFlag;
        quizScoreTable.Point = point;
        StartCoroutine(GraphQLObject.GetComponent<QuizGraphQL>().MutationQuizScoreUpsert(quizScoreTable, (result) => { }));
    }

    public void StopQuizWating()
    {
        //クイズ待機を終了
        StartCoroutine(GraphQLObject.GetComponent<QuizGraphQL>().MutationQuizGameDelete(RoomID, (result) => { }));
        initArea();
        init_AreaSel();
    }

    public void ShowAllRank()
    {
        RankArea.SetActive(true);
        if(QuestionCount <= 1)
        {
            RankArea.GetComponent<RankMaster>().initQuiz();
        }
        StartCoroutine(RankArea.GetComponent<RankMaster>().ShowAnimation());
    }

    private void initArea()
    {
        //クイズの表示を初期化
        TeacherArea.SetActive(false);
        TeacherArea_SelectArea.SetActive(false);
        TeacherArea_EditArea.SetActive(false);
        TeacherArea_ShowArea.SetActive(false);
        WatingArea.SetActive(false);
        WatingArea_TeacherArea.SetActive(false);
        WatingArea_StudentArea.SetActive(false);
        WatingArea_StudentArea_NoQuizArea.SetActive(false);
        WatingArea_StudentArea_StudentWait.SetActive(false);
        QuizMain.SetActive(false);
        RankArea.SetActive(false);
        CorrectObject.SetActive(false);
    }

    private void init_AreaSel()
    {
        if(OwnerFlag)
        {
            //教員だった場合
            Init_Teacher();
        }
        else
        {
            //学生だった場合
            Init_Student();
        }
    }

    public void Start_QuizWating_Teacher(QuizTable quizTable)
    {
        if(quizTable != null)
        {
            initArea();
            //クイズ待機画面を表示
            NowQuizTable = quizTable;
            WatingArea.SetActive(true);
            WatingArea_TeacherArea.SetActive(true);
            WatingArea_TeacherArea.GetComponent<TeacherWaitArea>().quizMaster = this;
            StartCoroutine(WatingArea_TeacherArea.GetComponent<TeacherWaitArea>().SetQuizTitle(0));
        }
    }

    public void Init_Teacher()
    {
        initArea();
        
        TeacherArea.SetActive(true);
        TeacherArea_SelectArea.SetActive(true);
    }

    public void EndStudent()
    {
        Debug.Log("EndStudent");
        QuizStartFlag = false;
        Navigation_InitStudent();
    }

    public void Navigation_InitStudent()
    {
        Debug.Log("Navigation_InitStudent");
        if(OwnerFlag == false && QuizStartFlag == false)
        {
            Init_Student();
        }
        
    }

    public void Init_Student()
    {
        Debug.Log("Init_Student");
        initArea();
        //今のはじまっていないクイズを取得
        StartCoroutine(GraphQLObject.GetComponent<QuizGraphQL>().GetQuizGame(RoomID, (output) => {
            if (output != null)
            {
                WatingArea.SetActive(true);
                WatingArea_StudentArea.SetActive(true);
                WatingArea_StudentArea_StudentWait.SetActive(true);
                StartCoroutine(WatingArea_StudentArea.GetComponent<StudentWaitArea>().ImportQuizData(output));
                //WatingArea_StudentArea.GetComponent<StudentWaitArea>().quizMaster = this;
                //WatingArea_StudentArea.GetComponent<StudentWaitArea>().NowQuizTable = NowQuizTable;
            }
            else
            {
                WatingArea.SetActive(true);
                WatingArea_StudentArea.SetActive(true);
                WatingArea_StudentArea_NoQuizArea.SetActive(true);
            }
        }));
    }

    public void UpdateRank(int NewRank)
    {
        Rank = NewRank;
        RankText.text = "Rank." + Rank;
    }
}


