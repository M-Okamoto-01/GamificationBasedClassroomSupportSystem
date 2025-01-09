using System;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;


/// <summary>
/// GetGraphQL
/// 
/// AWS AppSyncを用いたGraphQLを扱う
/// subscriptionすることでwebsocketも使用可能
///
/// bool値はUnityだと大文字Falseになるため
/// MD.partnerflag.ToString().ToLower()が必要
/// </summary>

public class TimelineGraphQL : MonoBehaviour
{
    [SerializeField]  private string GraphQLEndPoint = "GraphQLEndPoint";
    [SerializeField]  private string RealTimeEndPoint = "RealTimeEndPoint";
    [SerializeField]  private string APIKey =       "APIKey";
    [SerializeField]  private string Host =         "Host";
    [SerializeField]  private string RealTimeHost = "RealTimeHost";

    public GraphQLManager graphQLManager;

    void Start()
    {
        graphQLManager = new GraphQLManager();
    }

    /// <summary>
    /// GetsoloData
    ///
    /// RoomIDとRegisterDateUUIDを送ると、その部屋のTimelineを1件取得する
    /// </summary>
    /// <returns>
    /// Task<MatchingData>
    ///
    /// 同期のMatchingDataを返す
    /// </returns>
    public IEnumerator GetsoloData(string RoomID, string RegisterDateUUID,Action<TimelineData> callback)
    {
        string Query = "query MyQuery {getTimelinesolo(RoomID: \\\"" + RoomID + "\\\",RegisterDateUUID: \\\"" + RegisterDateUUID + "\\\") " + GetTimelineDataQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        } 
        //Jsonパースする
        TimelineResponseData response = JsonConvert.DeserializeObject<TimelineResponseData>(result);
        //TimelineaDataを返す
        callback?.Invoke(response.data.getTimelinesolo);
    }

    /// <summary>
    /// GetAllData
    ///
    /// RoomIDを送ると、その部屋のTimelineを全件取得する
    /// </summary>
    /// <returns>
    /// Task<MatchingData>
    ///
    /// 同期のMatchingDataを返す
    /// </returns>
    public IEnumerator GetAllData(string RoomID,Action<TimelineData[]> callback)
    {
        string Query = "query MyQuery { getTimeline(RoomID: \\\"" + RoomID + "\\\") { items " + GetTimelineDataQLName() + "}}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        TimelineAllDataData response = JsonConvert.DeserializeObject<TimelineAllDataData>(result);
        //TimelineDataを返す
        callback?.Invoke(response.data.getTimeline.items);
    }

    /// <summary>
    /// MutationInsert
    ///
    /// TimelineDataを送ると追加してくれる
    /// </summary>
    public IEnumerator MutationInsert(TimelineData TD,Action<bool> callback)
    {
        string Query = "mutation MyMutation { " +
                       "createTimeline(input:" + GetTimelineDataQLValue(TD) + ")" + GetTimelineDataQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(false);
            yield break;
        }
        else
        {
            callback?.Invoke(true);
        }
    }

    /// <summary>
    /// MutationUpdateMatchingData
    ///
    /// MatchingDataを送ると追加してくれる
    /// 既に登録済みの場合はUpdate
    /// Insert,Update
    /// </summary>
    public IEnumerator MutationUpdate(TimelineData TD,Action<bool> callback)
    {
        string Query = "mutation MyMutation {updateTimeline(input:" + GetTimelineDataQLValue(TD) + ")" + GetTimelineDataQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(false);
            yield break;
        }
        else
        {
            callback?.Invoke(true);
        }
    }

    /// <summary>
    /// MutationReplyUpdate
    /// TimelineのReplyを送ると、Replyを追加してくれる
    /// </summary>
    /// <param name="ARRDUL"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationReplyUpdate(AppendReplyRegisterDateUUIDList ARRDUL,Action<bool> callback)
    {
        string Query = "mutation MyMutation {appendReplyRegisterDateUUID(input:" + GetValue(ARRDUL) + ")" + GetTimelineDataQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(false);
            yield break;
        }
        else
        {
            callback?.Invoke(true);
        }

    }

    /// <summary>
    /// MutationHeartUpdate
    /// TimelineのHeartを送ると、Heartを追加してくれる
    /// </summary>
    /// <param name="ARRDUL"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationHeartUpdate(AppendReplyRegisterDateUUIDList ARRDUL,Action<TimelineData> callback)
    {
        string Query = "mutation MyMutation {appendHeartSenderUUIDList(input:" + GetValue(ARRDUL) + ")" + GetTimelineDataQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            //Jsonパースする
            HeartSenderUUIDListData response = JsonConvert.DeserializeObject<HeartSenderUUIDListData>(result);
            //TimelineaDataを返す
            callback?.Invoke(response.data.appendHeartSenderUUIDList);
        }
    }

    /// <summary>
    /// MutationHeartDeleteUpdate
    /// TimelineのHeartを送ると、Heartを削除してくれる
    /// </summary>
    /// <param name="ARRDUL"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationHeartDeleteUpdate(DeleteHeartSenderUUIDList ARRDUL,Action<TimelineData> callback)
    {
        string Query = "mutation MyMutation {deleteHeartSenderUUIDList(input:" + GetValue(ARRDUL) + ")" + GetTimelineDataQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            //Jsonパースする
            DeleteHeartSenderUUIDListData response = JsonConvert.DeserializeObject<DeleteHeartSenderUUIDListData>(result);
            //TimelineaDataを返す
            callback?.Invoke(response.data.deleteHeartSenderUUIDList);
        }
    }


    /// <summary>
    /// MutationDelete
    ///
    /// UUID を送ると削除する
    /// </summary>
    public IEnumerator MutationDelete(string RoomID,string RegisterDateUUID,Action<bool> callback)
    {
        string Query = "mutation MyMutation {" +
                       "deleteTimeline(input: { RoomID: \\\"" + RoomID + "\\\", RegisterDateUUID: \\\"" + RegisterDateUUID + "\\\"}) " + GetTimelineDataQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(false);
            yield break;
        }
        else
        {
            callback?.Invoke(true);
        }
    }

    /// <summary>
    /// クラス名のリストを文字列で返す
    /// </summary>
    private string GetTimelineDataQLName()
    {
        //結果を格納する
        string TimelineName = "{";
        int nowR = 0;
        TimelineData TLD = new TimelineData();
        Type t = TLD.GetType();
        foreach (PropertyInfo f in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (nowR > 0) TimelineName = TimelineName + ",";
            TimelineName = TimelineName + f.Name;

            nowR = nowR + 1;
        }
        TimelineName = TimelineName + "}";

        return TimelineName;
    }

    /// <summary>
    /// クラス名と値のリストを文字列で返す(JSON)
    /// </summary>
    private string GetTimelineDataQLValue(TimelineData TLDQ)
    {
        //結果を格納する
        string TimelineName = "{";
        int nowR = 0;

        Type t = TLDQ.GetType();
        foreach (PropertyInfo f in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (nowR > 0) TimelineName += ",";

            if (f.PropertyType == typeof(DateTime))
            {
                TimelineName += $"{f.Name}:\\\"{((DateTime)f.GetValue(TLDQ)).ToString("yyyy-MM-ddThh:mm:ss")}Z\\\"";
            }
            else if (f.PropertyType == typeof(string[]))
            {
                TimelineName += $"{f.Name}:[";
                string[] str = (string[])f.GetValue(TLDQ);
                for (int i = 0; i < str.Length; i++)
                {
                    if (i > 0) TimelineName += ",";
                    TimelineName += $"\\\"{ConvStringLine(str[i])}\\\"";
                }
                TimelineName += "]";
            }
            else
            {
                TimelineName += $"{f.Name}:\\\"{ConvStringLine(f.GetValue(TLDQ).ToString())}\\\"";
            }

            nowR++;
        }
        TimelineName += "}";

        return TimelineName;
    }

    /// <summary>
    /// クラス名と値のリストを文字列で返す(JSON)
    /// </summary>
    private string GetValue(AppendReplyRegisterDateUUIDList TLDQ)
    {
        //結果を格納する
        string TimelineName = "{";
        int nowR = 0;

        Type t = TLDQ.GetType();
        foreach (PropertyInfo f in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (nowR > 0) TimelineName += ",";
            TimelineName += $"{f.Name}:\\\"{ConvStringLine(f.GetValue(TLDQ).ToString())}\\\"";      
            nowR++;
        }
        TimelineName += "}";

        return TimelineName;
    }

     /// <summary>
    /// クラス名と値のリストを文字列で返す(JSON)
    /// </summary>
    private string GetValue(DeleteHeartSenderUUIDList TLDQ)
    {
        //結果を格納する
        string TimelineName = "{";
        int nowR = 0;

        Type t = TLDQ.GetType();
        foreach (PropertyInfo f in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (nowR > 0) TimelineName += ",";
            if(f.PropertyType == typeof(int))
            {
                TimelineName += $"{f.Name}:{f.GetValue(TLDQ)}";
            }
            else
            {
                TimelineName += $"{f.Name}:\\\"{ConvStringLine(f.GetValue(TLDQ).ToString())}\\\"";      
            }    
            nowR++;
        }
        TimelineName += "}";

        return TimelineName;
    }

    private string ConvStringLine(string str)
    {
        //改行をJSON用に変換する
        str = str.Replace("\r\n", "\\n");
        str = str.Replace("\n", "\\n");
        str = str.Replace("\r", "\\n");
        return str;
    }
}



public class TimelineReciever
{
    public TimelineData onCreateTimeline { get; set; }
}

//TimelineData
public class TimelineResponseData
{
    public TimelineResponse data { get; set; }
}

public class TimelineResponse
{
    public TimelineData getTimelinesolo { get; set; }
}

//TimelineAllData
public class TimelineAllDataData
{
    public TimelineAllData data { get; set; }
}

public class TimelineAllData
{
    public Items getTimeline { get; set; }
}

public class Items
{
    public TimelineData[] items { get; set; }
}

//HeartSenderUUIDList
public class HeartSenderUUIDListData
{
    public HeartSenderUUIDList data { get; set; }
}

public class HeartSenderUUIDList
{
    public TimelineData appendHeartSenderUUIDList { get; set; }
}

//DeleteHeartSenderUUIDList
public class DeleteHeartSenderUUIDListData
{
    public DelHeartSenderUUIDList data { get; set; }
}

public class DelHeartSenderUUIDList
{
    public TimelineData deleteHeartSenderUUIDList { get; set; }
}

