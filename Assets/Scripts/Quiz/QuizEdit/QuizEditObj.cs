using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Shapes2D;

public class QuizEditObj : MonoBehaviour
{
    public TMP_Text QuizTitle;
    public TMP_Text QuizIndex;
    private QuizTable quiztable;

    public GameObject QuizEditView;
    public GameObject QuizScoreView;

    public QuizMaster quizMaster;

    public void ImportData(QuizTable quizData)
    {
        this.quiztable = quizData;
        QuizTitle.text = quizData.QuizMainData.title;
        QuizIndex.text = quizData.QuizMainData.QuizDatas.Length.ToString();
    }

    public void EditStart()
    {
        QuizEditView.SetActive(true);
        QuizEditView.GetComponent<QuizEditCellMaster>().ImportData(quiztable);

    }

    public void ShowScore()
    {
        QuizScoreView.SetActive(true);
        QuizScoreView.GetComponent<QuizResultMaster>().GetQuizData(quiztable);
    }

    public void QuizStart()
    {
        quizMaster.Start_QuizWating_Teacher(quiztable);
    }
}
