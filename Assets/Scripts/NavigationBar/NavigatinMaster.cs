using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigatinMaster : MonoBehaviour
{
    public GameObject TimelineArea;
    public GameObject APIChatArea;
    public GameObject QuizArea;
    public GameObject ReloadButton;
    public GameObject CoverArea;
    
    public void OnClick_Timeline()
    {
        if(QuizArea.GetComponent<QuizMaster>().QuizStartFlag == false)
        {
            TimelineArea.SetActive(true);
            APIChatArea.SetActive(false);
            QuizArea.SetActive(false);
            ReloadButton.SetActive(true);
            CoverArea.GetComponent<TimelineDiscription>().ClickBackButton();
            CoverArea.SetActive(true);
        }
        
    }

    public void OnClick_APIChat()
    {
        if(QuizArea.GetComponent<QuizMaster>().QuizStartFlag == false)
        {
            TimelineArea.SetActive(false);
            APIChatArea.SetActive(true);
            QuizArea.SetActive(false);
            ReloadButton.SetActive(false);
            CoverArea.SetActive(false);
        }
    }

    public void OnClick_Quiz()
    {
        TimelineArea.SetActive(false);
        APIChatArea.SetActive(false);
        QuizArea.SetActive(true);
        ReloadButton.SetActive(false);
        CoverArea.SetActive(false);

        QuizArea.GetComponent<QuizMaster>().Navigation_InitStudent();
    }
}
