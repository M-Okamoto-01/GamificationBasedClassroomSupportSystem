using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class QuizTimer : MonoBehaviour
{
    //Inspectorから設定
    public Slider TargetSlider;
    public Image FillArea;
    public Image BackArea;

    //30秒で回答する
    private float TimerMax = 30.0f;

    //TimerCoroutine
    private Coroutine TimerCoroutine;

    //Color
    private Color FillColor = new Color(0.7368624f, 0.9622642f, 0.7035422f, 1.0f);
    private Color BackColor = new Color(1,1,1,1);
    private Color EndFillColor = new Color(1,0.7098886f,0.7035422f,1);

    void Awake()
    {
        //Sliderの色を設定
        FillArea.color = FillColor;
        BackArea.color = BackColor;
        TargetSlider.value = 1.0f;
    }
    
    public void StartTimer(QuizMaster quizMaster)
    {
        if (TimerCoroutine != null)
        {
            StopTimer();
        }
        TimerCoroutine = StartCoroutine(Timer(quizMaster));
    }

    //タイマーを止める
    public void StopTimer()
    {
        FillArea.DOKill();
        StopCoroutine(TimerCoroutine);
    }

    //残り時間を返す
    public float GetTime()
    {
        return TimerMax * TargetSlider.value;
    }

    private IEnumerator Timer(QuizMaster quizMaster)
    {
        TargetSlider.value = 1.0f;
        FillArea.color = FillColor;

        float time = 0.0f;
        bool ColorChangeFlag = false;
        while (time < TimerMax)
        {
            time += Time.deltaTime;
            TargetSlider.value = 1.0f - (time / TimerMax);
            if (time > TimerMax * 0.7f && ColorChangeFlag == false)
            {
                FillArea.DOColor(EndFillColor, TimerMax * 0.3f);
                ColorChangeFlag = true;
            }
            yield return null;
        }
        //時間制限が来たら次に行く
        //quizMaster.ShowQuiz();
        StartCoroutine(quizMaster.SkipQuiz());
    }

    public IEnumerator GetPoint(QuizMaster quizMaster,TMP_Text PointText,Action<int> callback)
    {
        //現在のポイント
        float Nowpoint = quizMaster.NowPoint;

        StopTimer();
        float point = TargetSlider.value * 100;
        TargetSlider.DOValue(0.0f, 2f);

        float addPoint = point / 20;
        for (int i = 0; i < 20; i++)
        {
            Nowpoint += addPoint;
            PointText.text = ((int)Nowpoint).ToString();
            yield return new WaitForSeconds(0.1f);
        }

        quizMaster.NowPoint = (int)(quizMaster.NowPoint + point);
        PointText.text = quizMaster.NowPoint.ToString();

        callback((int)point);
    }


}
