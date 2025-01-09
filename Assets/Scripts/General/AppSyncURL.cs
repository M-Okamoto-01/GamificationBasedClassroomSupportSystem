using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSyncURL
{
    //Timeline
    private static string Time_Host = "Time_Host";
    private static string Time_GraphQLEndPoint = "Time_GraphQLEndPoint";
    private static string Time_RealTimeEndPoint = "Time_RealTimeEndPoint";
    private static string Time_ApiKey = "Time_ApiKey";

    //GoodBad
    private static string GoodBad_Host = "GoodBad_Host";
    private static string GoodBad_GraphQLEndPoint = "GoodBad_GraphQLEndPoint";
    private static string GoodBad_RealTimeEndPoint = "GoodBad_RealTimeEndPoint";
    private static string GoodBad_ApiKey = "GoodBad_ApiKey";

    //Quiz
    private static string Quiz_Host = "Quiz_Host";
    private static string Quiz_GraphQLEndPoint = "Quiz_GraphQLEndPoint";
    private static string Quiz_RealTimeEndPoint = "Quiz_RealTimeEndPoint";
    private static string Quiz_ApiKey = "Quiz_ApiKey";

    public static ASEndpoint GetASTimeline(string RoomID){
        ASEndpoint aSEndpoint = new ASEndpoint();
        aSEndpoint.Host = Time_Host;
        aSEndpoint.GraphQLEndPoint = Time_GraphQLEndPoint;
        aSEndpoint.RealTimeEndpoint = Time_RealTimeEndPoint;
        aSEndpoint.ApiKey = Time_ApiKey;
        aSEndpoint.SubscriptionQuery = "subscription MySubscription {onCreateTimeline(RoomID: \"" + RoomID + 
                                        "\") {HeartSenderUUIDList RegisterDateUUID ReplyRegisterDateUUIDList RoomID SendDateTime SenderName SenderUUID content } }";
        return aSEndpoint;
    }

    public static ASEndpoint GetASGoodBad(string RoomID){
        ASEndpoint aSEndpoint = new ASEndpoint();
        aSEndpoint.Host = GoodBad_Host;
        aSEndpoint.GraphQLEndPoint = GoodBad_GraphQLEndPoint;
        aSEndpoint.RealTimeEndpoint = GoodBad_RealTimeEndPoint;
        aSEndpoint.ApiKey = GoodBad_ApiKey;
        aSEndpoint.SubscriptionQuery = "subscription MySubscription { onUpdateGoodBadData(RoomID: \"" + RoomID + 
                                        "\") {Evaluation UUID RoomID } }";
        return aSEndpoint;
    }

    public static ASEndpoint GetASQuiz(string RoomID){
        ASEndpoint aSEndpoint = new ASEndpoint();
        aSEndpoint.Host = Quiz_Host;
        aSEndpoint.GraphQLEndPoint = Quiz_GraphQLEndPoint;
        aSEndpoint.RealTimeEndpoint = Quiz_RealTimeEndPoint;
        aSEndpoint.ApiKey = Quiz_ApiKey;
        aSEndpoint.SubscriptionQuery = "subscription MySubscription { onCreateQuizWating(RoomID:  \"" + RoomID + 
                                        "\") { Point QuizID RoomID TargetQuizIndex UserUUID } }";
        return aSEndpoint;
    }

    public static ASEndpoint GetASQuizGame(string RoomID){
        ASEndpoint aSEndpoint = new ASEndpoint();
        aSEndpoint.Host = Quiz_Host;
        aSEndpoint.GraphQLEndPoint = Quiz_GraphQLEndPoint;
        aSEndpoint.RealTimeEndpoint = Quiz_RealTimeEndPoint;
        aSEndpoint.ApiKey = Quiz_ApiKey;
        aSEndpoint.SubscriptionQuery = "subscription MySubscription { onCreateQuizGameModel(RoomID:  \"" + RoomID + 
                                        "\") { QuizID RoomID StartFlag StartTime TargetQuizIndex } }";
        return aSEndpoint;
    }

    public static ASEndpoint GetASQuizScore(string RoomID){
        ASEndpoint aSEndpoint = new ASEndpoint();
        aSEndpoint.Host = Quiz_Host;
        aSEndpoint.GraphQLEndPoint = Quiz_GraphQLEndPoint;
        aSEndpoint.RealTimeEndpoint = Quiz_RealTimeEndPoint;
        aSEndpoint.ApiKey = Quiz_ApiKey;
        aSEndpoint.SubscriptionQuery = "subscription MySubscription { onCreateQuizScoreTable(RoomID:  \"" + RoomID + 
                                        "\") { UserUUID QuizID QuizIndex CorrectFlag Point RoomID Selectindex UserUUIDQuizIDIndex } }";
        return aSEndpoint;
    }
}

public class ASEndpoint{
    public string Host = "";
    public string GraphQLEndPoint = "";
    public string RealTimeEndpoint = "";
    public string ApiKey = "";
    public string SubscriptionQuery = "";
}
