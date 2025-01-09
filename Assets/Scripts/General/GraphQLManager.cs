using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Diagnostics;

public class GraphQLManager 
{
    /// <summary>
    /// GraphQL Request
    /// All main thread
    /// Can Excute GraphQL Query on WebGL
    /// </summary>
    /// <param name="GRAPHQL_ENDPOINT"></param>
    /// <param name="API_KEY"></param>
    /// <param name="query"></param>
    /// <returns>
    /// String
    /// Query result
    /// </returns>
    public IEnumerator ExecuteGraphQLQuery(string GRAPHQL_ENDPOINT,string API_KEY,string query,Action<string> callback)
    {
        //query \n → \\n
        query = query.Replace("\\n", "\\\\n");
        //UnityEngine.Debug.Log(query);
        // Create a request object
        UnityWebRequest request = new UnityWebRequest(GRAPHQL_ENDPOINT, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes($"{{ \"query\": \"{query}\" }}");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/graphql");
        request.SetRequestHeader("x-api-key", API_KEY);

        // Send the request and wait for it to complete
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            //UnityEngine.Debug.Log(JsonConvert.SerializeObject(request));
            //エラーが起きた場合は、何も返さない
            callback?.Invoke(null);
        }
        else
        {
            //正常に終了した場合は、結果を返す
            callback?.Invoke(request.downloadHandler.text);
        }

        request.Dispose();
    }

    /// <summary>
    /// GraphQL Request
    /// All main thread
    /// Can Excute GraphQL Query on WebGL
    /// </summary>
    /// <param name="GRAPHQL_ENDPOINT">エンドポイント</param>
    /// <param name="API_KEY">APIキー</param>
    /// <param name="query">クエリ（クエリ変数を記載できるように）</param>
    /// <param name="variables">クエリ変数</param>
    /// <returns>
    /// String
    /// Query result
    /// </returns>
    public IEnumerator ExecuteGraphQLQuery(string GRAPHQL_ENDPOINT, string API_KEY, string query, object variables, Action<string> callback)
    {
        // クエリと変数を含むリクエストボディを作成
        var requestBody = new
        {
            query = query,
            variables = JsonConvert.SerializeObject(variables)
        };

        // JSONにシリアライズ
        string jsonBody = JsonConvert.SerializeObject(requestBody);

        // UnityWebRequestを作成
        UnityWebRequest request = new UnityWebRequest(GRAPHQL_ENDPOINT, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", API_KEY);

        // リクエストを送信し、完了するのを待つ
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            // エラーが起きた場合は、nullを返す
            callback?.Invoke(null);
        }
        else
        {
            // 正常に終了した場合は、結果を返す
            callback?.Invoke(request.downloadHandler.text);
        }

        request.Dispose();
    }
}

