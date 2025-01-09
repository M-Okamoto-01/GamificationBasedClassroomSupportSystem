using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Gilzoide.LottiePlayer;

public class CorrectMaster : MonoBehaviour
{
    public GameObject CircleObject;
    public GameObject CrossObject;
    public GameObject CrackerObject;
    public GameObject CrackerPaper;

    // Start is called before the first frame update

    void Awake()
    {
        CircleObject.GetComponent<RectTransform>().localScale = new Vector3(0.0f,0.0f,0.0f);
        CrossObject.GetComponent<RectTransform>().localScale = new Vector3(0.0f,0.0f,0.0f);
        CrackerObject.GetComponent<CanvasGroup>().alpha = 0.0f;
        CrackerPaper.GetComponent<CanvasGroup>().alpha = 0.0f;
        this.SetActive(false);
    }

    public IEnumerator SetCorrect(QuizSlection quizSlection,QuizMaster quizMaster,int OriginalIndex)
    {
        if(quizSlection.CorrectFlag)
        {
            CircleObject.SetActive(true);
            CrossObject.SetActive(false);
            StartCoroutine(MoveCracker());

        }
        else
        {
            CircleObject.SetActive(false);
            CrossObject.SetActive(true);
        }

        GameObject targetObj = quizSlection.CorrectFlag ? CircleObject : CrossObject;
        yield return StartCoroutine(MoveAnimation(targetObj));
        if(quizSlection.CorrectFlag)
        {
            //正解の場合はポイントを取得
           yield return StartCoroutine(quizMaster.PointGet(quizSlection,OriginalIndex));
        }
        else
        {
            //不正解の場合は0点     
            quizMaster.SendScoreData(quizSlection,0,OriginalIndex);
        }

        yield return new WaitForSeconds(4f);
        this.SetActive(false);
    }

    private IEnumerator MoveCracker()
    {
        CrackerObject.GetComponent<ImageLottiePlayer>().StopAllCoroutines();
        CrackerPaper.GetComponent<ImageLottiePlayer>().StopAllCoroutines();
        CrackerObject.GetComponent<CanvasGroup>().alpha = 1.0f;
        CrackerPaper.GetComponent<CanvasGroup>().alpha = 1.0f;
        CrackerObject.GetComponent<ImageLottiePlayer>().Play();
        CrackerPaper.GetComponent<ImageLottiePlayer>().Play();
        yield return new WaitForSeconds(2.5f);
        CrackerObject.GetComponent<CanvasGroup>().DOFade(0.0f,0.5f);
        CrackerPaper.GetComponent<CanvasGroup>().DOFade(0.0f,0.5f);
        CrackerObject.GetComponent<ImageLottiePlayer>().StopAllCoroutines();
        CrackerPaper.GetComponent<ImageLottiePlayer>().StopAllCoroutines();
    }

    private IEnumerator MoveAnimation(GameObject targetObj)
    {
        targetObj.GetComponent<RectTransform>().localScale = new Vector3(0.0f,0.0f,0.0f);
        this.GetComponent<CanvasGroup>().alpha = 1.0f;
        targetObj.GetComponent<RectTransform>().DOScale(new Vector3(1.3f,1.3f,1.0f),0.7f);
        yield return new WaitForSeconds(2f);
        this.GetComponent<CanvasGroup>().DOFade(0.0f,0.5f);
        yield return new WaitForSeconds(0.5f);
    }

}
