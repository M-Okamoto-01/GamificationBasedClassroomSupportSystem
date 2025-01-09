using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using AOT;
using System;
using Shapes2D;
using Gilzoide.LottiePlayer;
using DG.Tweening;


public class RankMaster : MonoBehaviour
{
    //Rankの表記を管理するクラス
    public static List<GameObject> RankCells;
    public static GameObject RankCellPrefab;
    public static GameObject RankContent;

    public static QuizMaster QuizMaster;

    public GameObject quizWebSocket;

    public QuizGraphQL QuizGraphQL;

    public GameObject CrownObj;
    public GameObject GreatObj;

    void Awake()
    {
        //Asssts/Scripts/Quiz/RankCell
        RankCells = new List<GameObject>();
        RankCellPrefab = Resources.Load<GameObject>("Prefab/Quiz/RankCell");
        RankContent = GameObject.FindGameObjectWithTag("RankContent");
        QuizMaster = GameObject.FindGameObjectWithTag("QuizArea").GetComponent<QuizMaster>();
    }

    public void initQuiz()
    {
        if (RankCells == null)
        {
            RankCells = new List<GameObject>();
        }
        GetAllUserPoint();

        SubscribeUserCount();
    }

    public IEnumerator ShowAnimation()
    {
        //ある程度のラグを設ける
        yield return new WaitForSeconds(1f);
        //順位よりアニメーションを実行
        DoAnimation();
    }

    //ポイントを取得
    public void GetAllUserPoint()
    {
        List<QuizWaitingTable> quizWaitingTables;
        StartCoroutine(QuizGraphQL.GetQuizWatingAll(QuizMaster.RoomID, (result) =>
        {
            quizWaitingTables = result.ToList();
            SetRankMaster(quizWaitingTables);
        }));
    }

    private void DoAnimation()
    {
        //順位よりアニメーションを実行
        int Rank = GetRank();
        GameObject targetObj = new GameObject();
        if (Rank == 1)
        {
            targetObj = CrownObj;
            GreatObj.GetComponent<CanvasGroup>().alpha = 0.0f;
        }
        else if (Rank <= 5)
        {
            targetObj = GreatObj;
            CrownObj.GetComponent<CanvasGroup>().alpha = 0.0f;
        }

        if(Rank <= 5)
        {
            StartCoroutine(MoveLottieAnimation(targetObj));
        }

    }

    private IEnumerator MoveLottieAnimation(GameObject targetObj)
    {
        targetObj.GetComponent<ImageLottiePlayer>().StopAllCoroutines();
        targetObj.GetComponent<CanvasGroup>().alpha = 1.0f;
        targetObj.GetComponent<ImageLottiePlayer>().Play();
        yield return new WaitForSeconds(2f);
        targetObj.GetComponent<CanvasGroup>().DOFade(0.0f,0.5f);
    }

    public static void SetRankMaster(List<QuizWaitingTable> quizWaitingTables)
    {
        //Rankの表記を初期化
        List<GameObject> cellsToDestroy = new List<GameObject>(RankCells);
        foreach (var rankCell in cellsToDestroy)
        {
            Destroy(rankCell);
        }
        RankCells.Clear();

        //waitingTableのQuizIDがNowQuizTableのQuizIDと一致するものだけを抽出
        quizWaitingTables = quizWaitingTables.Where(x => x.QuizID == QuizMaster.NowQuizTable.QuizID).ToList();

        //降順に並び替え
        quizWaitingTables.Sort((a, b) => b.Point - a.Point);

        //Rankを別配列に格納
        int[] ranks = new int[quizWaitingTables.Count];
        int startRank = 0;
        int prePoint = -10000;
        for (int i = 0; i < quizWaitingTables.Count; i++)
        {
            if (prePoint != quizWaitingTables[i].Point)
            {
                startRank = i + 1;
                prePoint = quizWaitingTables[i].Point;
            }
            ranks[i] = startRank;
        }

        //Rankの表記を作成
        foreach (var quizWaitingTable in quizWaitingTables)
        {
            var rankCell = Instantiate(RankCellPrefab, RankContent.transform);
            if (quizWaitingTable.UserUUID == QuizMaster.MeUUID)
            {
                rankCell.GetComponent<RankCell>().SetRankCell(quizWaitingTable, ranks[quizWaitingTables.IndexOf(quizWaitingTable)], "あなた");
            }
            else
            {
                rankCell.GetComponent<RankCell>().SetRankCell(quizWaitingTable, ranks[quizWaitingTables.IndexOf(quizWaitingTable)], "他ユーザ");
            }

            RankCells.Add(rankCell);
        }

        if (GameObject.FindGameObjectWithTag("QuizArea").activeSelf)
        {
            //ランクの更新を通知
            GameObject.FindGameObjectWithTag("QuizArea").GetComponent<QuizMaster>().UpdateRank(GetRank());
        }
    }

    public static void UpdateUserPoint(QuizWaitingTable quizWaitingTable)
    {
        //ポイントを更新
        //ユーザー情報の前取得
        List<QuizWaitingTable> NowquizWaitingTables = new List<QuizWaitingTable>();
        foreach (var rankCell in RankCells)
        {
            NowquizWaitingTables.Add(rankCell.GetComponent<RankCell>().MyQuizWaitingTable);
        }

        //更新か追加かどうかを確認
        bool isExist = false;
        foreach (var NowquizWaitingTable in NowquizWaitingTables)
        {
            if (NowquizWaitingTable.UserUUID == quizWaitingTable.UserUUID)
            {
                NowquizWaitingTable.Point = quizWaitingTable.Point;
                isExist = true;
                break;
            }
        }

        if (!isExist)
        {
            //同じクイズのみを追加する
            if(quizWaitingTable.QuizID == QuizMaster.NowQuizTable.QuizID)
            {
                NowquizWaitingTables.Add(quizWaitingTable);
            }
            
        }

        SetRankMaster(NowquizWaitingTables);

    }

    public void UpdateUserPoint_toUseStatic(QuizWaitingTable quizWaitingTable)
    {
        UpdateUserPoint(quizWaitingTable);
    }

    public int GetRank_toUseStatic()
    {
        return GetRank();
    }

    public static int GetRank()
    {
        //ポイントを取得
        int Rank = 0;
        //順位の算出
        List<QuizWaitingTable> NowquizWaitingTables = new List<QuizWaitingTable>();
        foreach (GameObject rankCell in RankCells)
        {
            NowquizWaitingTables.Add(rankCell.GetComponent<RankCell>().MyQuizWaitingTable);
        }
        NowquizWaitingTables.Sort((a, b) => b.Point - a.Point);
        //自分の順位を取得
        int prePoint = -10000;
        foreach (QuizWaitingTable NowquizWaitingTable in NowquizWaitingTables)
        {
            
            if(NowquizWaitingTable.Point != prePoint)
            {
                Rank++;
                prePoint = NowquizWaitingTable.Point;
            }
            if (NowquizWaitingTable.UserUUID == QuizMaster.MeUUID)
            {
                break;
            }
        }

        return Rank;

    }

    //生徒の数のサブスクを開始
    public void SubscribeUserCount()
    {
        ASEndpoint ASE = AppSyncURL.GetASQuiz(QuizMaster.RoomID);
        QuizGraphQL.GetComponent<AppSyncWebSocketManager>().WebGL_WebSocketSubscribe(ASE,QuizMaster.MeUUID,"QuizWatingTable",UpdateUserCount_WebSocket);
    }

    //サブスク用の処理
    [MonoPInvokeCallback(typeof(Action<string,string>))]
    public static void UpdateUserCount_WebSocket(string type,string result)
    {
        //データを更新する
        //resultをGetSub_GoodBadDataに変換する
        GetSub_QuizWatingTable quizwating = JsonConvert.DeserializeObject<GetSub_QuizWatingTable>(result);
        if(quizwating.onCreateQuizWating == null)
        {
            return;
        }
        else 
        {
            UpdateUserPoint(quizwating.onCreateQuizWating);
        }
    }

    [System.Serializable]
    public class GetSub_QuizWatingTable{
        [SerializeField] public QuizWaitingTable onCreateQuizWating;
    }

}
