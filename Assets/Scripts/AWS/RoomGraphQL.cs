using System;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

public class RoomGraphQL : MonoBehaviour
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
    /// RoomIDとOwnerIDを送ると、その部屋の情報を取得する
    /// </summary>
    /// <returns>
    /// Task<MatchingData>
    ///
    /// 同期のMatchingDataを返す
    /// </returns>
    public IEnumerator GetsoloData(string RoomID, string OwnerID,Action<RoomData> callback)
    {
        string Query = "query MyQuery {getRoom(RoomID: \\\"" + RoomID + "\\\",OwnerID: \\\"" + OwnerID + "\\\") " + GetQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null) 
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        RoomDatasoloData response = JsonConvert.DeserializeObject<RoomDatasoloData>(result);
        //MatchingDataを返す
        callback?.Invoke(response.data.getRoom);
    }

    /// <summary>
    /// GetMatchingAllData
    ///
    /// 待機している全ての人の情報を取得
    /// </summary>
    /// <returns>
    /// Task<MatchingData>
    ///
    /// 同期のMatchingDataを返す
    /// </returns>
    public IEnumerator GetAllData(string RoomID,Action<RoomData[]> callback)
    {
        string Query = "query MyQuery { getRoomAll(RoomID: \\\"" + RoomID + "\\\") { items " + GetQLName() + "}}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null) 
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        RoomDataItemsData response = JsonConvert.DeserializeObject<RoomDataItemsData>(result);
        //MatchingDataを返す
        callback?.Invoke(response.data.getRoomAll.items);
    }

    /// <summary>
    /// MutationInsertMatchingData
    ///
    /// MatchingDataを送ると追加してくれる
    /// 既に登録済みの場合はUpdate
    /// Insert,Update
    /// </summary>
    public IEnumerator MutationInsert(RoomData TD,Action<bool> callback)
    {
        string Query = "mutation MyMutation { " +
                       "createRoom(input:" + GetQLData(TD) + ")" + GetQLName() + "}";
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
    public IEnumerator MutationUpdate(RoomData RD,Action<bool> callback)
    {
        string Query = "mutation MyMutation {updateRoom(input:" + GetQLData(RD) + ")" + GetQLName() + "}";
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
    /// MutationDeleteMatchingData
    ///
    /// UUID を送ると削除する
    /// </summary>
    public IEnumerator MutationDelete(string RoomID,string OwnerID,Action<bool> callback)
    {
        string Query = "mutation MyMutation {" +
                       "deleteRoom(input: { RoomID: \\\"" + RoomID + "\\\", OwnerID: \\\"" + OwnerID + "\\\"}) " + GetQLName() + "}";
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

    private IDisposable _subscription;

    /// <summary>
    /// マッチングしたかを確認する
    /// </summary>
    /// <param name="MD">
    /// websocketから取得できるMatchingData
    /// </param>
    private void TimelineGet(RoomData RD)
    {
            _subscription.Dispose();
    }

    /// <summary>
    /// クラス名のリストを文字列で返す
    /// </summary>
    private string GetQLName()
    {
        //結果を格納する
        string TimelineName = "{";
        int nowR = 0;
        RoomData TLD = new RoomData();
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
    private string GetQLData(RoomData TLDQ)
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

    private string ConvStringLine(string str)
    {
        //改行をJSON用に変換する
        str = str.Replace("\r\n", "\\n");
        str = str.Replace("\n", "\\n");
        str = str.Replace("\r", "\\n");
        return str;
    }
}

public class RoomDataResponse
{
    public RoomData getRoom { get; set; }
}

public class RoomDataReciever
{
    public RoomData onCreateRoom { get; set; }
}

//RoomDatasolo
public class RoomDatasoloData
{
    public RoomDatasolo data { get; set; }
}

public class RoomDatasolo
{
    public RoomData getRoom { get; set; }
}

//RoomDataItems
public class RoomDataItemsData
{
    public RoomDataAllData data { get; set; }
}

public class RoomDataItems
{
    public RoomData[] items { get; set; }
}

public class RoomDataAllData
{
    public RoomDataItems getRoomAll { get; set; }
}