using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

public class LogObject : MonoBehaviour
{
    private int NowIndex = 0;
    private LogList logList;
    public TimelineMaster timelineMaster;
    private Coroutine SendLogCoroutine;
    // API GatewayのエンドポイントURLを指定
    private string url = "https://t04zp5qo26.execute-api.ap-northeast-1.amazonaws.com/prod/logs";

    

    // Start is called before the first frame update
    void Start()
    {
        //initialize
        logList = new LogList();
        logList.items = new List<LogData>();
        NowIndex = 0;
        SendLogCoroutine = StartCoroutine(SendLog());
    }

    public void AddLogData(string LogType, string Optional){
        LogData logData = new LogData();
        logData.UUID = timelineMaster.MeUUID;
        logData.UserName = timelineMaster.UserName;
        logData.RoomID = timelineMaster.RoomID;
        logData.InsertIndex = NowIndex.ToString();
        logData.DateTime = System.DateTime.Now.ToString();
        logData.LogType = LogType;
        logData.Optional = Optional;
        logList.items.Add(logData);
        NowIndex++;
    }

    private IEnumerator SendLog()
    {
        //繰り返す
        while(true)
        {
            //1分ごとにログを送信
            yield return new WaitForSeconds(60.0f);
            //ログを送信
            if (logList.items.Count > 0)
            {
                //ログを送信
                //Debug.Log("SendLog");
                StartCoroutine(SendLogData(logList));
            }
            //初期化
            logList = new LogList();
            logList.items = new List<LogData>();
            NowIndex = 0;
        }
        
    }

    //RESTAPIを使ってログを送信する
    private IEnumerator SendLogData(LogList data)
    {
        // JSONデータをシリアライズ（Newtonsoft.Jsonを使用）
        string jsonData = JsonConvert.SerializeObject(data);

        // UnityWebRequestにPOSTリクエストを設定
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // リクエストを送信し、完了を待つ
        yield return request.SendWebRequest();

        // レスポンスを確認
        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
        
        yield return null;
    } 

}

public class LogData{
    public string UUID;
    public string UserName;
    public string RoomID;
    public string InsertIndex;
    public string DateTime;
    public string LogType;
    public string Optional;
}

public class LogList{
    public List<LogData> items;
}
