using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizResultCell : MonoBehaviour
{
    //オブジェクトの取得
    public GameObject QuizTitleObj;
    public GameObject[] QuizSelections;

    //取得するオブジェクト
    private List<TMP_Text> QuizSelectionNameObjs;
    private List<Image> QuizSelectionBackGroundObjs;
    private List<Slider> QuizSelectionsPercentObjs;
    private List<TMP_Text> QuizSelectionsPercentTextObjs;

    //この問題のデータ
    public static string RoomID;
    public static int QuizID;
    public static int QuizIndex;

    //学生データ
    private QuizData quizData;
    private List<QuizScoreTable> quizScoreTableList;

    void Awake()
    {
        GetSelectionsData();
    }

    //Selectionsから必要なデータを全て取得
    private void GetSelectionsData()
    {
        QuizSelectionNameObjs = new List<TMP_Text>();
        QuizSelectionBackGroundObjs = new List<Image>();
        QuizSelectionsPercentObjs = new List<Slider>();
        QuizSelectionsPercentTextObjs = new List<TMP_Text>();

        for (int i = 0; i < QuizSelections.Length; i++)
        {
            QuizSelectionNameObjs.Add(QuizSelections[i].transform.Find("QuizSelectionName").GetComponent<TMP_Text>());
            QuizSelectionBackGroundObjs.Add(QuizSelections[i].transform.Find("PercentSlider").Find("Fill Area").Find("Fill").GetComponent<Image>());
            QuizSelectionsPercentObjs.Add(QuizSelections[i].transform.Find("PercentSlider").GetComponent<Slider>());
            QuizSelectionsPercentTextObjs.Add(QuizSelections[i].transform.Find("AnswerP").GetComponent<TMP_Text>());
        }
    }

    //クイズデータを設定
    public void SetQuizData(QuizData QD,string RID,int QID,int QI)
    {
        RoomID = RID;
        QuizID = QID;
        QuizIndex = QI;
        quizData = QD;
        SetQuizSelectionsData();
    }

    //クイズの選択肢データをセット
    private void SetQuizSelectionsData()
    {
        QuizTitleObj.GetComponent<TMP_Text>().text = quizData.QuizTitle;

        for (int i = 0; i < QuizSelections.Length; i++)
        {
            QuizSelectionNameObjs[i].text = quizData.Slections[i].Label;
            QuizSelectionsPercentObjs[i].value = 0;
            QuizSelectionsPercentTextObjs[i].text = "0.0%";
            if(quizData.Slections[i].CorrectFlag)
            {
                //正解は赤にする
                QuizSelectionBackGroundObjs[i].color = new Color(1.0f, 0.78f, 0.77f, 1f);
            }
            else
            {
                //不正解は青にする
                QuizSelectionBackGroundObjs[i].color = new Color(0.41f, 0.794f, 1f, 1f);
            }
        }
    }

    //クイズのスコアデータをセット
    public void SetQuizScoreData(List<QuizScoreTable> QSTL)
    {
        //必要であれば初期化
        if(quizScoreTableList == null)
        {
            quizScoreTableList = new List<QuizScoreTable>();
        }

        //リストにデータをセット
        foreach(QuizScoreTable QST in QSTL)
        {
            //同じクイズでなければスキップ
            if( RoomID != QST.RoomID || QuizID != QST.QuizID)
            {
                continue;
            }

            //同じデータがあれば削除
            for(int i = 0; i < quizScoreTableList.Count; i++)
            {
                if(quizScoreTableList[i].UserUUIDQuizIDIndex == QST.UserUUIDQuizIDIndex && quizScoreTableList[i].RoomID == QST.RoomID)
                {
                    quizScoreTableList.RemoveAt(i);
                    break;
                }
            }
            quizScoreTableList.Add(QST);
        }

        //集計データの作成
        ShowResult();
        
    }

    public void SetQuizScoreDataUseStatic(QuizScoreTable QST)
    {
        SetQuizScoreData(QST);
    }

    //クイズのスコアデータをセット
    public void SetQuizScoreData(QuizScoreTable QST)
    {
        Debug.Log(QuizIndex);
        //必要であれば初期化
        if(quizScoreTableList == null)
        {
            quizScoreTableList = new List<QuizScoreTable>();
        }

        //同じクイズでなければスキップ
        if( RoomID != QST.RoomID || QuizID != QST.QuizID)
        {
            return;
        }

        //リストにデータをセット
        //同じデータがあれば削除
        for(int i = 0; i < quizScoreTableList.Count; i++)
        {
            if(quizScoreTableList[i].UserUUIDQuizIDIndex == QST.UserUUIDQuizIDIndex && quizScoreTableList[i].RoomID == QST.RoomID)
            {
                quizScoreTableList.RemoveAt(i);
                break;
            }
        }
        quizScoreTableList.Add(QST);

        //集計データの作成
        ShowResult();
    }

    //クイズの集計データの作成
    private void ShowResult()
    {
        //OriginalIndexごとに集計
        int[] OriginalIndexCount = new int[QuizSelectionNameObjs.Count];
        for(int i = 0; i < OriginalIndexCount.Length; i++)
        {
            OriginalIndexCount[i] = 0;
        }

        //カウント
        foreach(QuizScoreTable QST in quizScoreTableList)
        {
            Debug.Log(QST.Selectindex);
            if(QST.Selectindex.Length == 0)
            {
                continue;
            }
            if(QST.Selectindex[0] < 0 || QST.Selectindex[0] >= OriginalIndexCount.Length)
            {
                continue;
            }
            OriginalIndexCount[QST.Selectindex[0]]++;
        }

        //集計
        int TotalCount = 0;
        for(int i = 0; i < OriginalIndexCount.Length; i++)
        {
            TotalCount += OriginalIndexCount[i];
        }

        //設定
        for(int i = 0; i < OriginalIndexCount.Length; i++)
        {
            QuizSelectionsPercentObjs[i].value = (float)OriginalIndexCount[i] / (float)TotalCount  * 100;
            QuizSelectionsPercentTextObjs[i].text = ((float)OriginalIndexCount[i] / (float)TotalCount * 100).ToString("F1") + "%";
        }

    }
}
