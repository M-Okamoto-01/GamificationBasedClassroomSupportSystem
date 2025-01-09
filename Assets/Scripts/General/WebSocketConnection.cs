using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using UnityEngine;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http.Websocket;


/// <summary>
/// WebSocketConnection
/// 
/// AWS AppSyncを用いたGraphQLを扱う
/// subscriptionすることでwebsocketも使用可能
///
/// bool値はUnityだと大文字Falseになるため
/// MD.partnerflag.ToString().ToLower()が必要
/// </summary>

public class WebSocketConnection
{

    //--- Types ---
    private class AppSyncHeader
    {
        [JsonProperty("host")] public string Host { get; set; }

        [JsonProperty("x-api-key")] public string ApiKey { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(ToJson()));
        }
    }

    public class AuthorizedAppSyncHttpRequest : GraphQLHttpRequest
    {
        private readonly string _authorization;

        public AuthorizedAppSyncHttpRequest(GraphQLRequest request, string authorization) : base(request)
            => _authorization = authorization;

        public override HttpRequestMessage ToHttpRequestMessage(GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
        {
            HttpRequestMessage result = base.ToHttpRequestMessage(options, serializer);
            result.Headers.Add("X-Api-Key", _authorization);
            return result;
        }
    }

    private IDisposable _subscription;
    
    //https://github.com/graphql-dotnet/graphql-client/issues/591
    //https://edom18.hateblo.jp/entry/2021/12/11/105140#subscriptionでリアルタイム通知を受け取る
    //https://stackoverflow.com/questions/50596390/cannot-return-null-for-non-nullable-type-person-within-parent-messages-g
    /// <summary>
    /// AddSubscription
    ///
    /// Subscription登録をする
    /// </summary>
    public async Task AddSubscription(string Host,string RealTimeEndPoint, string APIKey,string UserUUID, string subscriptionQuery,string GraphQLEndPoint,string returnType,Action<string, string> returnFunc,SynchronizationContext context)
    {
        //websocket実行済みであれば削除する
        if (_subscription != null) _subscription.Dispose();

        GraphQLHttpClient graphQLClient = new GraphQLHttpClient(GraphQLEndPoint, new NewtonsoftJsonSerializer());

        AppSyncHeader appSyncHeader = new AppSyncHeader
        {
            Host = Host,
            ApiKey = APIKey,
        };

        string header = appSyncHeader.ToBase64String();

        graphQLClient.Options.WebSocketEndPoint = new Uri(RealTimeEndPoint + $"?header={header}&payload=e30=");
        graphQLClient.Options.WebSocketProtocol = WebSocketProtocols.GRAPHQL_WS;
        graphQLClient.Options.PreprocessRequest = (req, client) =>
        {
            GraphQLHttpRequest result = new AuthorizedAppSyncHttpRequest(req, APIKey)
            {
                ["data"] = JsonConvert.SerializeObject(req),
                ["extensions"] = new
                {
                    authorization = appSyncHeader,
                }
            };
            return Task.FromResult(result);
        };

        await graphQLClient.InitializeWebsocketConnection();

        GraphQLRequest request = new GraphQLRequest
        {
            Query = subscriptionQuery,
        };

        Debug.Log("Subscription Start");

        var subscriptionStream = graphQLClient.CreateSubscriptionStream<System.Object>(request, ex => { Debug.Log(ex); });
        _subscription = subscriptionStream.Subscribe(
            response => {
                //MainThreadで実行する
                context.Post(_ =>
                {
                    //今のthreadを取得
                    returnFunc(returnType,JsonConvert.SerializeObject(response.Data));
                }, null);
            },
            exception => Debug.Log(exception),
            () => Debug.Log("Completed."));
    }

    public void CloseWebSocketConnection()
    {
        if (_subscription != null) _subscription.Dispose();
    }
}
