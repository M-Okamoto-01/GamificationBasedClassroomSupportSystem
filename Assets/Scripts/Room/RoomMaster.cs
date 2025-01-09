using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using System;

public class RoomMaster : MonoBehaviour
{
    //WebGLのみ
    //------------
    private GameObject SVContentObj;
    public GameObject Number_StudentObj;
    public GameObject UserName_StudentObj;
    public GameObject Number_TeacherJoinObj;
    public GameObject Number_TeacherNumJoinObj;
    public GameObject DDDataObj;
    public GameObject LastPanelTextObj;
    public GameObject Teacher_RoomNameObj;
    public GameObject Teacher_NameObj;
    public GameObject[] StudentPanelList;
    public GameObject[] TeacherPanelList;
    public GameObject[] TeacherJoinPanelList;
    public GameObject[] TeacherCreatePanelList;

    public GameObject BackButtonObj;
    public GameObject NoticePanelObj;
    public NoticePanel noticePanel;

    public RoomGraphQL roomGraphQL;

    public GameObject YesNoPanelObj;
    
    //メインスレッドを取得
    private SynchronizationContext mainThread;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        SVContentObj = GameObject.FindGameObjectWithTag("RoomSVContent");
        noticePanel = NoticePanelObj.GetComponent<NoticePanel>();
        mainThread = SynchronizationContext.Current;
        Allfalse();
        StartCoroutine(CheckPlayerPrefs());
    }

    /// <summary>
    /// パネルを移動する
    /// </summary>
    public void MoveNextPanel()
    {
        mainThread.Post(_ =>
        {
            //  メインスレッドで実行したいコード
            Vector2 PreVec = SVContentObj.GetComponent<RectTransform>().anchoredPosition;
            SVContentObj.GetComponent<RectTransform>().DOAnchorPos(PreVec + new Vector2(-800,0), 0.5f);
        }, null);
    }

    private IEnumerator CheckPlayerPrefs()
    {
        //PlayerPrefsに値があるか確認する
        DDDataObj.GetComponent<DDData>().LoadPlayerPrefs();
        if (DDDataObj.GetComponent<DDData>().ExitPlayerPrefs())
        {
            if(DDDataObj.GetComponent<DDData>().RoomData != null)
            {
                if(DDDataObj.GetComponent<DDData>().RoomData.Length != 0)
                {
                    RoomData[] roomData = null;
                    yield return StartCoroutine(roomGraphQL.GetAllData(DDDataObj.GetComponent<DDData>().RoomData[0].RoomID,(output) => roomData = output));
                    if(roomData == null)
                    {
                        yield break;
                    }

                    if(roomData.Length != 0)
                    {
                        DDDataObj.GetComponent<DDData>().RoomData = roomData;
                        //復元するか確認する
                        Debug.Log("復元しますか？");
                        YesNoPanelObj.SetActive(true);
                        string NoticeText_YesNo = "前回のログイン情報で\n復元しますか？";
                        string UserType = DDDataObj.GetComponent<DDData>().OwnerFlag ? "教員" : "学生";
                        string ContentText_YesNo = "ニックネーム：" + DDDataObj.GetComponent<DDData>().UserName + "\n" + "ルーム名：" + DDDataObj.GetComponent<DDData>().RoomData[0].RoomName + "\n" + "\n" + "ユーザータイプ：" + UserType;
                        YesNoPanelObj.GetComponent<YesNoPanel>().ShowYesNoPanel(NoticeText_YesNo, ContentText_YesNo, 1);
                        YesNoPanelObj.GetComponent<YesNoPanel>().roomMaster = this;
                    }
                }
            }
        }
    }

    public void OnClickYes()
    {
        //Homeシーンに移動する
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeUI");
    }

    public void OnClickNo()
    {
        //PlayerPrefsを削除する
        DDDataObj.GetComponent<DDData>().DeletePlayerPrefs();
    }

    public void CheckRoomNum_Student_Button()
    {
        StartCoroutine(CheckRoomNum_Student());
    }

    private IEnumerator CheckRoomNum_Student()
    {
        //入力された値を取得
        string Value = Number_StudentObj.GetComponent<NumberInput>().GetValue();
        //入力された値が5文字か
        if (Value.Length == 5)
        {
            noticePanel.ShowFlag(NoticeType.Loding, "ルームを確認しています", false);
            //ルームが存在しているか確認する
            RoomData[] roomData = null;
            yield return StartCoroutine(roomGraphQL.GetAllData(Value,(output) => roomData = output));
            if(roomData == null)
            {
                noticePanel.ShowFlag(NoticeType.NO, "ルームが見つかりませんでした", true);
                yield break;
            }
            
            if(roomData.Length != 0)
            {
                noticePanel.ShowFlag(NoticeType.OK, "ルームが見つかりました", true);
                //存在していたら、ルームに入る
                MoveNextPanel();
                //DDDataに値を保存する
                DDDataObj.GetComponent<DDData>().RoomData = roomData;
            }
            else
            {
                noticePanel.ShowFlag(NoticeType.NO, "ルームが見つかりませんでした", true);
                //存在していなかったら、エラーを表示する
                Debug.Log("存在していないルームです");
                Number_StudentObj.GetComponent<NumberInput>().ResetValue();
            }
        }
        else
        {
            noticePanel.ShowFlag(NoticeType.NO, "ルーム番号は5桁です", true);
        }
    }

    public void CheckRoomNum_Teacher_Button()
    {
        StartCoroutine(CheckRoomNum_Teacher());
    }

    private IEnumerator CheckRoomNum_Teacher()
    {
        //入力された値を取得
        string Value = Number_TeacherJoinObj.GetComponent<NumberInput>().GetValue();
        //入力された値が5文字か
        if (Value.Length == 5)
        {
            //ルームが存在しているか確認する
            noticePanel.ShowFlag(NoticeType.Loding, "ルームを確認しています", false);   
            RoomData[] roomData = null;
            yield return StartCoroutine(roomGraphQL.GetAllData(Value,(output) => roomData = output));
            if(roomData == null)
            {
                noticePanel.ShowFlag(NoticeType.NO, "ルームが見つかりませんでした", true);
                yield break;
            }

            if(roomData.Length != 0)
            {
                noticePanel.ShowFlag(NoticeType.OK, "ルームが見つかりました", true);
                //存在していたら、ルームに入る
                MoveNextPanel();
                //DDDataに値を保存する
                DDDataObj.GetComponent<DDData>().RoomData = roomData;
            }
            else
            {
                noticePanel.ShowFlag(NoticeType.NO, "ルームが見つかりませんでした", true);
                //存在していなかったら、エラーを表示する
                Debug.Log("存在していないルームです");
                Number_TeacherJoinObj.GetComponent<NumberInput>().ResetValue();
            }
        }
        else
        {
            noticePanel.ShowFlag(NoticeType.NO, "ルーム番号は5桁です", true);
        }
    }

    public void CheckRoomNum_TeacherNum()
    {
        //入力された値を取得
        string Value = Number_TeacherNumJoinObj.GetComponent<NumberInput>().GetValue();
        //入力された値が5文字か
        if (Value.Length == 5)
        {
            //教員IDが一致しているか確認する
            RoomData[] roomData = DDDataObj.GetComponent<DDData>().RoomData;
            RoomData TargetRoom = new RoomData();
            bool IsMatch = false;
            foreach (RoomData room in roomData)
            {
                if (room.OwnerID == Value)
                {
                    IsMatch = true;
                    TargetRoom = room;
                }
            }   
            if(IsMatch)
            {
                noticePanel.ShowFlag(NoticeType.OK, "教員IDが確認できました", true);
                //存在していたら、ルームに入る
                DDDataObj.GetComponent<DDData>().UserUUID = TargetRoom.OwnerID;
                DDDataObj.GetComponent<DDData>().UserName = TargetRoom.OwnerName;
                DDDataObj.GetComponent<DDData>().OwnerFlag = true;
                MoveNextPanel();
            }
            else
            {
                noticePanel.ShowFlag(NoticeType.NO, "教員IDが確認できませんでした", true);
                //存在していなかったら、エラーを表示する
                Number_TeacherNumJoinObj.GetComponent<NumberInput>().ResetValue();
            }
        }
    }

    public void CheckUserName_Student()
    {
        //入力された値を取得
        string Value = UserName_StudentObj.GetComponent<TMP_InputField>().text;
        //入力された値が0文字でないか
        if (Value.Length != 0)
        {
            //DDDataに値を保存する
            DDDataObj.GetComponent<DDData>().UserName = Value;
            DDDataObj.GetComponent<DDData>().OwnerFlag = false;
            //UUIDを生成する
            DDDataObj.GetComponent<DDData>().UserUUID = System.Guid.NewGuid().ToString();
            MoveNextPanel();
        }
    }

    public void CheckRoomName_Tacher()
    {
        //入力された値を取得
        string Value = Teacher_RoomNameObj.GetComponent<TMP_InputField>().text;
        //入力された値が0文字でないか
        if (Value.Length != 0)
        {
            RoomData[] roomData = new RoomData[1];
            roomData[0] = new RoomData();
            roomData[0].RoomName = Value;
            //DDDataに値を保存する
            DDDataObj.GetComponent<DDData>().RoomData = roomData;
            MoveNextPanel();
        }
    }

    public void CheckUserName_Tacher_Button()
    {
        StartCoroutine(CheckUserName_Tacher());
    }
    
    private IEnumerator CheckUserName_Tacher()
    {
        //入力された値を取得
        string Value = Teacher_NameObj.GetComponent<TMP_InputField>().text;
        //入力された値が0文字でないか
        if (Value.Length != 0)
        {
            RoomData[] roomData = DDDataObj.GetComponent<DDData>().RoomData;
            roomData[0].OwnerName = Value;
            //UUIDを生成する
            noticePanel.ShowFlag(NoticeType.Loding, "ルーム情報を取得しています", false);
            string NewRoomUUID = "";
            yield return StartCoroutine(Create_RoomUUID((output) => NewRoomUUID = output));
            roomData[0].RoomID = NewRoomUUID;
            roomData[0].OwnerID = Create_RandomFive();
            noticePanel.ShowFlag(NoticeType.OK, "ルーム情報を取得しました", true);
            DDDataObj.GetComponent<DDData>().UserUUID = roomData[0].OwnerID;
            //DDDataに値を保存する
            DDDataObj.GetComponent<DDData>().RoomData = roomData;
            DDDataObj.GetComponent<DDData>().UserName = Value; 
            
            yield return StartCoroutine(FinalIn_Teacher_Create());
            MoveNextPanel();
            
        }
    }

    private IEnumerator Create_RoomUUID(Action<string> callback)
    {
        string Res_RoomUUID = "";
        while(Res_RoomUUID == "")
        {
            //5桁のランダムな数字を生成する
            string RoomUUID = Create_RandomFive();
            //RoomUUIDが重複していないか確認する
            RoomData[] roomData = null;
            yield return StartCoroutine(roomGraphQL.GetAllData(RoomUUID,(output) => roomData = output));
            if(roomData == null)
            {
                Res_RoomUUID = RoomUUID;
            }
            if(roomData.Length == 0)
            {
                Res_RoomUUID = RoomUUID;
            }
        }

        callback?.Invoke(Res_RoomUUID);
    }

    private string Create_RandomFive()
    {
        string RoomUUID = "";
        for (int i = 0; i < 5; i++)
        {
            RoomUUID += UnityEngine.Random.Range(0, 9);
        }
        return RoomUUID;
    }


    public void JoinRoom()
    {
        //Homeシーンに移動する
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeUI");
    }

    public void ResetAll()
    {
        //Allfalse();
        SVContentObj.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,0), 0.5f);
        YesNoPanelObj.SetActive(false);
    }

    public void FinalIn_Student()
    {
        RoomData[] roomData = DDDataObj.GetComponent<DDData>().RoomData;
        DDDataObj.GetComponent<DDData>().OwnerFlag = false;
        LastPanelTextObj.GetComponent<TMP_Text>().text = "ルーム番号：" + roomData[0].RoomID + "\n" + "ルーム名：" + roomData[0].RoomName + "\n" + "ニックネーム：" + DDDataObj.GetComponent<DDData>().UserName;
    }

    public void FinalIn_Teacher()
    {
        RoomData[] roomData = DDDataObj.GetComponent<DDData>().RoomData;
        DDDataObj.GetComponent<DDData>().OwnerFlag = true;
        LastPanelTextObj.GetComponent<TMP_Text>().text = "ルーム番号：" + roomData[0].RoomID + "\n" + "ルーム名：" + roomData[0].RoomName + "\n" + "教員ID：" + DDDataObj.GetComponent<DDData>().UserUUID + "\n" + "ニックネーム：" + DDDataObj.GetComponent<DDData>().UserName;
    }

    public IEnumerator FinalIn_Teacher_Create()
    {
        //Appsyncにデータを送信する
        noticePanel.ShowFlag(NoticeType.Loding, "ルームを作成しています", false);
        RoomData[] roomData = DDDataObj.GetComponent<DDData>().RoomData;
        bool Res = false;
        yield return StartCoroutine(roomGraphQL.MutationInsert(roomData[0],(output) => Res = output));
        noticePanel.ShowFlag(NoticeType.OK, "ルームを作成しました", true);
        LastPanelTextObj.GetComponent<TMP_Text>().text = "ルーム番号：" + roomData[0].RoomID + "\n" + "ルーム名：" + roomData[0].RoomName + "\n" + "教員ID：" + DDDataObj.GetComponent<DDData>().UserUUID + "\n" + "ニックネーム：" + DDDataObj.GetComponent<DDData>().UserName;
        DDDataObj.GetComponent<DDData>().OwnerFlag = true;
    }

    public void ClickStudent()
    {
        //SetActiveを切り替える
        foreach (GameObject Panel in StudentPanelList)
        {
            Panel.SetActive(true);
        }
        //それ以外を非表示にする
        foreach (GameObject Panel in TeacherPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherJoinPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherCreatePanelList)
        {
            Panel.SetActive(false);
        }
    }

    public void ClickTeacher()
    {
        //SetActiveを切り替える
        foreach (GameObject Panel in TeacherPanelList)
        {
            Panel.SetActive(true);
        }
        //それ以外を非表示にする
        foreach (GameObject Panel in StudentPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherJoinPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherCreatePanelList)
        {
            Panel.SetActive(false);
        }
    }

    public void ClickTeacherJoin()
    {
        //SetActiveを切り替える
        foreach (GameObject Panel in TeacherJoinPanelList)
        {
            Panel.SetActive(true);
        }
        //それ以外を非表示にする
        foreach (GameObject Panel in StudentPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherPanelList)
        {
            Panel.SetActive(true);
        }
        foreach (GameObject Panel in TeacherCreatePanelList)
        {
            Panel.SetActive(false);
        }
    }

    public void ClickTeacherCreate()
    {
        //SetActiveを切り替える
        foreach (GameObject Panel in TeacherCreatePanelList)
        {
            Panel.SetActive(true);
        }
        //それ以外を非表示にする
        foreach (GameObject Panel in StudentPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherPanelList)
        {
            Panel.SetActive(true);
        }
        foreach (GameObject Panel in TeacherJoinPanelList)
        {
            Panel.SetActive(false);
        }
    }

    public void Allfalse()
    {
        foreach (GameObject Panel in StudentPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherJoinPanelList)
        {
            Panel.SetActive(false);
        }
        foreach (GameObject Panel in TeacherCreatePanelList)
        {
            Panel.SetActive(false);
        }

        BackButtonObj.SetActive(false);
    }

    public void onClickBack()
    {
        //もともとの位置を取得
        Vector2 PreVec = SVContentObj.GetComponent<RectTransform>().anchoredPosition;
        if(PreVec.x < -100)SVContentObj.GetComponent<RectTransform>().DOAnchorPos(PreVec + new Vector2(800,0), 0.5f);
    }

    public void CheckFirst()
    {
        //もともとの位置を取得
        Vector2 PreVec = SVContentObj.GetComponent<RectTransform>().anchoredPosition;
        if(PreVec.x > -100)BackButtonObj.SetActive(false);
        else BackButtonObj.SetActive(true);
    }

}
