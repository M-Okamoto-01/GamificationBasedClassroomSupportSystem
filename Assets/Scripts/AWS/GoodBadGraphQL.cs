using System;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;


/// <summary>
/// GoodBadGraphQL
/// 
/// AWS AppSyncを用いたGraphQLを扱う
/// subscriptionすることでwebsocketも使用可能
///
/// bool値はUnityだと大文字Falseになるため
/// MD.partnerflag.ToString().ToLower()が必要
/// </summary>

public class GoodBadGraphQL : MonoBehaviour
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
    /// GetAllData
    ///
    /// RoomIDを送ると、その部屋の授業理解度を返す
    /// </summary>
    /// <returns>
    ///
    /// 授業理解度の配列を返す
    /// </returns>
    public IEnumerator GetAllData(string RoomID,Action<GoodBadData[]> callback)
    {
        graphQLManager = new GraphQLManager();
        string Query = "query MyQuery { getGoodBadDataAll(RoomID: \\\"" + RoomID + "\\\") { items " + GetGoodBadQLName() + "}}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        GoodBadAllDatadata response = JsonConvert.DeserializeObject<GoodBadAllDatadata>(result);
        if (response.data.getGoodBadDataAll == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            callback?.Invoke(response.data.getGoodBadDataAll.items);
        }
    }

    /// <summary>
    /// MutationInsert
    ///
    /// 授業理解度の送信
    /// </summary>
    public IEnumerator MutationInsert(GoodBadData GB,Action<bool> callback)
    {
        string Query = "mutation MyMutation { " +
                       "createGoodBadData(input:" + GetGoodBadDataQLValue(GB) + ")" + GetGoodBadQLName() + "}";
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
    /// MutationUpdate
    ///
    /// 授業理解度の更新
    /// </summary>
    public IEnumerator MutationUpdate(GoodBadData GB,Action<bool> callback)
    {
        string Query = "mutation MyMutation {updateGoodBadData(input:" + GetGoodBadDataQLValue(GB) + ")" + GetGoodBadQLName() + "}";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,(output) => result = output));
        //エラー時にはnullを返す
        if (result.IndexOf("Status Code: 400") != -1)
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
    /// MutationDelete
    ///
    /// UUID を送ると削除する
    /// </summary>
    public IEnumerator MutationDelete(string RoomID,string UUID,Action<bool> callback)
    {
        string Query = "mutation MyMutation {" +
                       "deleteGoodBadData(input: { RoomID: \\\"" + RoomID + "\\\", UUID: \\\"" + UUID + "\\\"}) " + GetGoodBadQLName() + "}";
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
    private string GetGoodBadQLName()
    {
        //結果を格納する
        string TimelineName = "{";
        int nowR = 0;
        GoodBadData TLD = new GoodBadData();
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
    private string GetGoodBadDataQLValue(GoodBadData TLDQ)
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
            else if (f.PropertyType == typeof(int))
            {
                TimelineName += $"{f.Name}:{ConvStringLine(f.GetValue(TLDQ).ToString())}";
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

public class GoodBadAllDatadata
{
    public GoodBadAllData data { get; set; }
}

public class GoodBadAllData
{
    public GBItems getGoodBadDataAll { get; set; }
}

public class GBItems
{
    public GoodBadData[] items { get; set; }
}

