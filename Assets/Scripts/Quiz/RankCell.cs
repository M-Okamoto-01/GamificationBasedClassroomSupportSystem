using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RankCell : MonoBehaviour
{
    //Rankを表記するセル
    public QuizWaitingTable MyQuizWaitingTable;
    public TMP_Text RankText;
    public TMP_Text UserNameText;
    public TMP_Text PointText;
    public Image RankBK;

    public int MyRank;
    public string MyName;

    public void SetRankCell(QuizWaitingTable quizWaitingTable,int Rank, string userName)
    {
        MyQuizWaitingTable = quizWaitingTable;
        MyName = userName;
        MyRank = Rank;

        if (userName == "あなた")
        {
            RankBK.color = new Color(0, 0.3832f, 1f, 1);
        }
        else
        {
            RankBK.color = new Color(0, 0, 0, 1);
        }

        Repaint();
    }

    private void Repaint()
    {
        RankText.text = "Rank." + MyRank.ToString();
        UserNameText.text = MyName;
        PointText.text = MyQuizWaitingTable.Point.ToString();
    }

    public bool InputUpdate(QuizWaitingTable updateWaitingTable,int Rank)
    {
        if (updateWaitingTable.UserUUID == MyQuizWaitingTable.UserUUID)
        {

            MyQuizWaitingTable = updateWaitingTable;
            MyRank = Rank;
            Repaint();
            return true;
        }
        return false;
    }
}
