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

public class QuizGraphQL:MonoBehaviour
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

    // Get〇〇Solo　は1件だけデータを取得する
    // Get〇〇All は全件データを取得する
    // Mutation〇〇 はデータを更新する
    // Mutation〇〇Upsert はデータをUpsertする
    // Mutation〇〇Delete はデータを削除する
    // Subscription〇〇 はデータを監視する

    //QuizWatingTableについて
    /// <summary>
    /// GetQuizWatingSolo
    /// QuizWatingTableを1件取得
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="UserUUID"></param>
    /// <param name="callback"></param>
    /// <returns>callback QuizWaitingTable</returns>
    public IEnumerator GetQuizWatingSolo(string RoomID,string UserUUID,Action<QuizWaitingTable> callback)
    {
        //クエリ変数を作成
        object variables = new { RoomID = RoomID, UserUUID = UserUUID };
        string Query = @"query MyQuery($RoomID: String!, $UserUUID: String!) {
                        getQuizWating(RoomID: $RoomID, UserUUID: $UserUUID) { " + QuizWaitingTableDef + " } }";
                        
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        QuizWaitingTableDatasolo response = JsonConvert.DeserializeObject<QuizWaitingTableDatasolo>(result);
        if(response.data.getQuizWating == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        if (response.data.getQuizWating.RoomID == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            callback?.Invoke(response.data.getQuizWating);
        }
    }

    /// <summary>
    /// GetQuizWatingAll
    /// 
    /// クイズ待機ユーザーを全て取得
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="UserUUID"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetQuizWatingAll(string RoomID,Action<QuizWaitingTable[]> callback)
    {
        //クエリ変数を作成
        object variables = new { RoomID = RoomID};
        string Query = @"query MyQuery($RoomID: String!) {getQuizWatingAll(RoomID: $RoomID) {
                        items { " + QuizWaitingTableDef + " } } }";
                        
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        QuizWaitingTableDataAll response = JsonConvert.DeserializeObject<QuizWaitingTableDataAll>(result);
        //itemsの配列が空の場合はnullを返す
        if (response.data.getQuizWatingAll.items.Length < 0)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            callback?.Invoke(response.data.getQuizWatingAll.items);
        }
    }

    /// <summary>
    /// クイズ待機ユーザーを登録する
    /// また、ポイントの更新にも使う
    /// </summary>
    /// <param name="quiz"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationQuizWaitingUpsert(QuizWaitingTable quiz,Action<bool> callback)
    {
        //クエリ変数を作成
        object variables = new { input = quiz };
        string Query = @"mutation MyMutation($input: CreateQuizWatingInput!) {createQuizWating(input: $input) { " + QuizWaitingTableDef + " } }";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
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
    /// クイズの待機ユーザーを削除する
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="UserUUID"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationQuizWaitingDelete(string RoomID,string UserUUID,Action<bool> callback)
    {
        string Query = @"mutation MyMutation($RoomID: String!, $UserUUID: String!) {
                        deleteQuizWating(input: {RoomID: $RoomID, UserUUID: $UserUUID}) { " + QuizWaitingTableDef + " } }";
        object variables = new { RoomID = RoomID, UserUUID = UserUUID };
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
    /// クイズの受付管理データを消す
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="UserUUID"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationQuizGameDelete(string RoomID,Action<bool> callback)
    {
        string Query = "mutation MyMutation { deleteQuizGameModel(input: {RoomID: \\\"" + RoomID + "\\\"}) { " + QuizGameTableDef + " } }";
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

    //QuizTableについて
    /// <summary>
    /// クイズを取得する
    /// RoomIDとQuizIDを指定すると一意のクイズを取得する
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="QuizID"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetQuiz(string RoomID,int QuizID,Action<QuizTable> callback)
    {
        //クエリ変数を作成
        object variables = new { RoomID = RoomID, QuizID = QuizID };
        string Query = @"query MyQuery($RoomID: String!, $QuizID: Int!) {
                        getQuizTable(RoomID: $RoomID, QuizID: $QuizID) { " + QuizTableDef + " } }";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        QuizDataTableDatasolo response = JsonConvert.DeserializeObject<QuizDataTableDatasolo>(result);
        if(response.data.getQuizTable == null)
        {
            callback?.Invoke(null);
            yield break;
        }

        if (response.data.getQuizTable.RoomID == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            callback?.Invoke(response.data.getQuizTable);
        }
    }

    /// <summary>
    /// ルームに登録されている全てのクイズを取得する
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetQuizAll(string RoomID,Action<QuizTable[]> callback)
    {
        //クエリ変数を作成
        graphQLManager = new GraphQLManager();
        object variables = new { RoomID = RoomID};
        string Query = @"query MyQuery($RoomID: String!) {getQuizTableAll(RoomID: $RoomID) {
                        items { " + QuizTableDef + " } } }";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));

        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        QuizDataTableDataAll response = JsonConvert.DeserializeObject<QuizDataTableDataAll>(result);
        //itemsの配列が空の場合はnullを返す
        if (response.data.getQuizTableAll.items.Length < 0)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            callback?.Invoke(response.data.getQuizTableAll.items);
        }
    }

    /// <summary>
    /// クイズの登録と更新を行う
    /// </summary>
    /// <param name="quiz"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationQuizUpsert(QuizTable quiz,Action<bool> callback)
    {
        //クエリ変数を作成
        object variables = new { input = quiz };
        string Query = @"mutation MyMutation($input: CreateQuizTableInput!) {createQuizTable(input: $input) { " + QuizTableDef + " } }";
        string result = "";
        
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        Debug.Log(result);
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

    //QuizGameTableについて
    /// <summary>
    /// クイズが開始しているかを取得する
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="QuizID"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetQuizGame(string RoomID,Action<QuizGameTable> callback)
    {
        graphQLManager = new GraphQLManager();
        //クエリ変数を作成
        object variables = new { RoomID = RoomID};
        string Query = @"query MyQuery($RoomID: String!) { getQuizGameModel(RoomID: $RoomID) { " + QuizGameTableDef + " } }";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        QuizGameTableDataSolo response = JsonConvert.DeserializeObject<QuizGameTableDataSolo>(result);
        if(response.data.getQuizGameModel == null)
        {
            callback?.Invoke(null);
            yield break;
        }
       
        if (response.data.getQuizGameModel.RoomID == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            callback?.Invoke(response.data.getQuizGameModel);
        }
    }

    /// <summary>
    /// クイズの開始を登録する
    /// </summary>
    /// <param name="quiz"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationQuizGameUpsert(QuizGameTable quiz,Action<bool> callback)
    {
        //クエリ変数を作成
        object variables = new { input = quiz };
        string Query = @"mutation MyMutation($input: CreateQuizGameModelInput!) {createQuizGameModel(input: $input) { " + QuizGameTableDef + " } }";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        
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

    //QuizScoreTableについて
    /// <summary>
    /// クイズの回答情報を取得する
    /// </summary>
    /// <param name="RoomID"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GetQuizScoreAll(string RoomID,Action<QuizScoreTable[]> callback)
    {
        graphQLManager = new GraphQLManager();
        //クエリ変数を作成
        object variables = new { RoomID = RoomID};
        string Query = @"query MyQuery($RoomID: String!) { getQuizScoreTableAll(RoomID: $RoomID) { items { " + QuizScoreTableDef + " } } }";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        //エラー時にはnullを返す
        if (result == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        //Jsonパースする
        QuizScoreTableData response = JsonConvert.DeserializeObject<QuizScoreTableData>(result);
        if(response.data.getQuizScoreTableAll == null)
        {
            callback?.Invoke(null);
            yield break;
        }
       
        if (response.data.getQuizScoreTableAll.items.Length == 0)
        {
            callback?.Invoke(null);
            yield break;
        }
        else
        {
            callback?.Invoke(response.data.getQuizScoreTableAll.items);
        }
    }

    /// <summary>
    /// クイズの回答情報を登録する
    /// </summary>
    /// <param name="quizScore"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator MutationQuizScoreUpsert(QuizScoreTable quizScore,Action<bool> callback)
    {
        //クエリ変数を作成
        object variables = new { input = quizScore };
        string Query = @"mutation MyMutation($input: CreateQuizScoreTableInput!) {createQuizScoreTable(input: $input) { " + QuizScoreTableDef + " } }";
        string result = "";
        yield return StartCoroutine(graphQLManager.ExecuteGraphQLQuery(GraphQLEndPoint, APIKey, Query,variables,(output) => result = output));
        
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

    private string ConvStringLine(string str)
    {
        //改行をJSON用に変換する
        str = str.Replace("\r\n", "\\n");
        str = str.Replace("\n", "\\n");
        str = str.Replace("\r", "\\n");
        return str;
    }

    /// <summary>
    /// 以下にqueryの返り値を格納
    /// </summary>
    string QuizWaitingTableDef = "RoomID UserUUID QuizID TargetQuizIndex Point";
    string QuizGameTableDef = "RoomID QuizID TargetQuizIndex StartTime StartFlag";
    string QuizTableDef = "RoomID QuizID QuizMainData { title QuizDatas { QuizTitle Slections { Label CorrectFlag } QuestionType ImageURL CorrectAnswer } }";
    string QuizScoreTableDef = "UserUUID QuizID QuizIndex CorrectFlag Point RoomID Selectindex UserUUIDQuizIDIndex";
}

//QuizWatingTable
public class QuizWaitingTable
{
    public string RoomID;
    public string UserUUID;
    public int QuizID;
    public int TargetQuizIndex;
    public int Point;
}
//QuizWatingTableAll
public class QuizWaitingTableDatas
{
    public QuizWaitingTable[] items;
}

public class QuizWaitingTableDefAll
{
    public QuizWaitingTableDatas getQuizWatingAll;
}

public class QuizWaitingTableDataAll
{
    public QuizWaitingTableDefAll data;
}
//QuizWatingTableSolo
public class QuizWaitingTableDatasolo
{
    public QuizWaitingTableDefsolo data;
}

public class QuizWaitingTableDefsolo
{
    public QuizWaitingTable getQuizWating;
}

//QuizGameTable
public class QuizGameTable
{
    public string RoomID;
    public int QuizID;
    public int TargetQuizIndex;
    public DateTime StartTime;
    public int StartFlag;
}

public class QuizGameTableDataSolo
{
    public QuizGameDefSolo data;
}

public class QuizGameDefSolo
{
    public QuizGameTable getQuizGameModel;
}



//QuizTable
//QuizDataTableAll
public class QuizDataTableDataAll
{
    public QuizDataTableDefAll data;
}

public class QuizDataTableDefAll
{
    public QuizDataTableDatas getQuizTableAll;
}

public class QuizDataTableDatas
{
    public QuizTable[] items;
}

//QuizDataTableSolo
public class QuizDataTableDatasolo
{
    public QuizDataTableDefsolo data;
}

public class QuizDataTableDefsolo
{
    public QuizTable getQuizTable;
}

//QuizDataMaster
public class QuizTable
{
    public string RoomID;
    public int QuizID;
    public QuizDataMaster QuizMainData;
}


[System.Serializable]
public class QuizDataMaster
{
    public string title { get; set; } = "";
    public QuizData[] QuizDatas { get; set; } = new QuizData[0];
}

[System.Serializable]
public class QuizData
{
    public string QuizTitle { get; set; }
    public QuizSlection[] Slections { get; set; }
    public string QuestionType { get; set; }
    public string ImageURL { get; set; }
    public int? CorrectAnswer { get; set; }
}

//Slection
[System.Serializable]
public class QuizSlection
{
    public string Label { get; set; }
    public bool CorrectFlag { get; set; }
    public int OriginalIndex { get; set; }
}

//QuizScoreTable
[System.Serializable]
public class QuizScoreTable
{
    public string UserUUID { get; set; }
    public string UserUUIDQuizIDIndex { get; set; }
    public int QuizID { get; set; }
    public int QuizIndex { get; set; }
    public bool CorrectFlag { get; set; }
    public int Point { get; set; }
    public string RoomID { get; set; }
    public int[] Selectindex { get; set; }
}

//QuizScoreTableAll
public class QuizScoreTableDatas
{
    public QuizScoreTable[] items;
}

public class QuizScoreTableDefAll
{
    public QuizScoreTableDatas getQuizScoreTableAll;
}

public class QuizScoreTableData
{
    public QuizScoreTableDefAll data;
}