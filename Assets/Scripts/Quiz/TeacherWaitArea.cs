using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO.Ports;
using System;
using Newtonsoft.Json;
using AOT;
using Unity.VisualScripting;
using System.Linq;

public class TeacherWaitArea : MonoBehaviour
{
    public QuizMaster quizMaster;
    public GameObject QuizTitle;
    public static GameObject StudentCount;

    public GameObject quizGraphql;

    private static QuizGameTable NowQuizGameTable;

    private int NowUserCount = 0;

    private static List<QuizWaitingTable> WatingUserList;

    public void Start()
    {
        StudentCount = GameObject.FindGameObjectWithTag("StudentCount");
    }

    public IEnumerator SetQuizTitle(int TargetQuizIndex)
    {
        WatingUserList = new List<QuizWaitingTable>();

        QuizTitle.GetComponent<TMP_Text>().text = quizMaster.NowQuizTable.QuizMainData.title;
        QuizGameTable quizGameTable = new QuizGameTable();
        quizGameTable.RoomID = quizMaster.RoomID;
        quizGameTable.QuizID = quizMaster.NowQuizTable.QuizID;
        quizGameTable.TargetQuizIndex = TargetQuizIndex;
        quizGameTable.StartTime = DateTime.Now;
        quizGameTable.StartFlag = -1;
        NowQuizGameTable = quizGameTable;
        
        yield return StartCoroutine(quizGraphql.GetComponent<QuizGraphQL>().MutationQuizGameUpsert(quizGameTable, (result) => { }));
        GetWatingStudentList();
        SubscribeUserCount();
    }


    public void SetWatingUser(QuizWaitingTable[] quizWaitingTable)
    {
        foreach(QuizWaitingTable quizWaiting in quizWaitingTable)
        {
            if(quizWaiting.RoomID == NowQuizGameTable.RoomID && quizWaiting.QuizID == NowQuizGameTable.QuizID && quizWaiting.TargetQuizIndex == NowQuizGameTable.TargetQuizIndex)
            {
                WatingUserList.Add(quizWaiting);
            }
        }

        NowUserCount = WatingUserList.Count;
        SetStudentCount();
    }

    public static void SetWatingUser(QuizWaitingTable quizWaiting)
    {
        
        if(quizWaiting.RoomID == NowQuizGameTable.RoomID && quizWaiting.QuizID == NowQuizGameTable.QuizID && quizWaiting.TargetQuizIndex == NowQuizGameTable.TargetQuizIndex)
        {
            WatingUserList.Add(quizWaiting);
        }

        SetStudentCount();
    }


    public static void SetStudentCount()
    {
        //生徒の数を表示する

        //UserUUIDが重複している場合は削除する
        WatingUserList = WatingUserList.GroupBy(x => x.UserUUID).Select(x => x.First()).ToList();


        StudentCount.GetComponent<TMP_Text>().text = WatingUserList.Count.ToString();
    }

    public void SubscribeUserCount()
    {
        ASEndpoint ASE = AppSyncURL.GetASQuiz(quizMaster.RoomID);
        quizGraphql.GetComponent<AppSyncWebSocketManager>().WebGL_WebSocketSubscribe(ASE,quizMaster.MeUUID,"QuizWatingTable",UpdateUserCount_WebSocket);
    }

    public void GetWatingStudentList()
    {
        //生徒の数を取得する
        StartCoroutine(quizGraphql.GetComponent<QuizGraphQL>().GetQuizWatingAll(quizMaster.RoomID, (result) => {
            if(result != null){
                SetWatingUser(result);
            }
        }));
    }

    //サブスク用の処理
    [MonoPInvokeCallback(typeof(Action<string,string>))]
    public static void UpdateUserCount_WebSocket(string type,string result)
    {
        //データを更新する
        //resultをGetSub_GoodBadDataに変換する
        GetSub_QuizWatingTable quizwating = JsonConvert.DeserializeObject<GetSub_QuizWatingTable>(result);
        if(quizwating.onCreateQuizWating == null){
            return;
        }
        else {
            SetWatingUser(quizwating.onCreateQuizWating);
            SetStudentCount();
            }
        }

    [System.Serializable]
    public class GetSub_QuizWatingTable{
        [SerializeField] public QuizWaitingTable onCreateQuizWating;
    }

    public void onClick_StartQuiz()
    {
        //クイズを開始する
        StartCoroutine(StartQuiz());
    }

    public IEnumerator StartQuiz()
    {
        //クイズを開始する
        NowQuizGameTable.StartFlag = 1;
        NowQuizGameTable.StartTime = DateTime.Now;
        
        yield return StartCoroutine(quizGraphql.GetComponent<QuizGraphQL>().MutationQuizGameUpsert(NowQuizGameTable, (result) => { }));

        //WebSocketを切断する
        //quizGraphql.GetComponent<AppSyncWebSocketManager>().Disconnected();

        //ここに画面遷移のコード
        quizMaster.StartQuiz_Teacher();
    }

    public void onClick_CancelQuiz()
    {
        //クイズをキャンセルする
        quizMaster.StopQuizWating();
    }

    
}
