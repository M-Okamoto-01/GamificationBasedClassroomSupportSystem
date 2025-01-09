//Reference:https://qiita.com/gtk2k/items/1c7aa7a202d5f96ebdbf#jsからcの関数を呼び出す方法
mergeInto(LibraryManager.library, {
  CreateWebSocketConnection: function(unity_host, unity_realtimeEndpoint, unity_apiKey,unity_UserUUID,unity_subscriptionQuery,returnType,returnFunc) {
    // C#から渡された(ポインタ)文字列を取得する
    var host = UTF8ToString(unity_host);
    var realtimeEndpoint = UTF8ToString(unity_realtimeEndpoint);
    var apiKey = UTF8ToString(unity_apiKey);
    var subscId = UTF8ToString(unity_UserUUID);
    var subscriptionQuery = UTF8ToString(unity_subscriptionQuery);
    var ReturnType = UTF8ToString(returnType);

    // AppSyncのヘッダー・URLを作成する
    var header = { host: host, "x-api-key": apiKey };  // Cognitoの認証を使う場合は、Authorization: IdToken
    var url = realtimeEndpoint + "?header=" + btoa( JSON.stringify( header ) ) + "&payload=" + btoa( JSON.stringify( {} ) );
    console.log( "url: ", url );
    // WebSocketを作成する
    var socket = new WebSocket( url, [ 'graphql-ws' ] );

    // クエリを作成しておく
    var QueryJSON = JSON.stringify( {
      id: subscId,
      payload: {
        data: JSON.stringify( { query: subscriptionQuery, variables: {} } ),
        extensions: {
          authorization: {
            "x-api-key": apiKey,
            host: host
          }
        }
      },
      type: "start"
    } );

    // WebSocketのイベントを設定する
    socket.addEventListener( "open", ( event ) => {
        socket.send( JSON.stringify( { "type": "connection_init" } ) );
    } );

    // サーバーからメッセージを受け取った時の処理
    socket.addEventListener( "message", ( event ) => {
      var eventDataJson = JSON.parse( event.data );
      if( eventDataJson.type == "connection_ack" )	socket.send(QueryJSON);
      else if( eventDataJson.type == "data" )			 console.log( "Message from server ", eventDataJson );
      else if( eventDataJson.type == "complete" )	 console.log( "subscription done." );
    } );

    //メッセージを受信したとき
    socket.onmessage = function(event) {
      //Jsonをパースする
      var eventDataJson = JSON.parse(event.data);
      //payloadが存在しない場合は、何もしない
      if (!eventDataJson.payload){
        console.log("payloadが存在しない");
        return;
      } 
      //payloadを取得する
      //dataが存在しない場合は、何もしない
      if (!eventDataJson.payload.data) return;
      //eventDataJson.payload.dataを文字列に変換する
      var str = JSON.stringify(eventDataJson.payload.data);
      console.log(str);
      // C#にメッセージを返す
      // stringはエンコードが必要
      var encoder = new TextEncoder();
      // data
      var strBuffer = encoder.encode(str + String.fromCharCode(0)); // 文字列はnull文字終端にする
      var strPtr = _malloc(strBuffer.length);
      HEAP8.set(strBuffer, strPtr);
      //Type
      var typeBuffer = encoder.encode(ReturnType + String.fromCharCode(0)); // 文字列はnull文字終端にする
      var typePtr = _malloc(typeBuffer.length);
      HEAP8.set(typeBuffer, typePtr);
      // Call
      Runtime.dynCall('vii', returnFunc, [typePtr, strPtr]);
      // free
      _free(strPtr);
      _free(typePtr);
    };

    socket.onerror = function(error) {
      console.error("WebSocketエラー: ", error);
    };

    socket.onclose = function() {
      console.log("WebSocket接続が閉じられました");
    };
  },

  CloseWebSocketConnection: function() {
    if (socket) {
      socket.close();
      console.log("WebSocket接続を閉じました");
    }
  }

});

