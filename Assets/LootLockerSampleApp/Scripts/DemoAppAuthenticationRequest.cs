using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker;
using Newtonsoft.Json;
using System;
using LootLocker.Requests;
using LootLockerDemoApp;


namespace LootLockerDemoApp
{
    public partial class DemoAppAdminRequests
    {

        [Header("Authentication Endpoints")]
        public static EndPointClass initialAuthenticationRequest = new EndPointClass("v1/session", LootLockerHTTPMethod.POST);
        public static EndPointClass twoFactorAuthenticationCodeVerification = new EndPointClass("v1/2fa", LootLockerHTTPMethod.POST);
        public static EndPointClass subsequentRequests = new EndPointClass("v1/games", LootLockerHTTPMethod.GET);

        //Games
        [Header("Games Endpoints")]
        [Header("---------------------------")]
        public static EndPointClass getAllGamesToTheCurrentUser = new EndPointClass("v1/games", LootLockerHTTPMethod.GET);
        public static EndPointClass creatingAGame = new EndPointClass("v1/game", LootLockerHTTPMethod.POST);
        public static EndPointClass getDetailedInformationAboutAGame = new EndPointClass("v1/game/{0}", LootLockerHTTPMethod.GET);
        public static EndPointClass updatingInformationAboutAGame = new EndPointClass("v1/game/{0}", LootLockerHTTPMethod.PATCH);
        public static EndPointClass deletingGames = new EndPointClass("v1/game/{0}", LootLockerHTTPMethod.DELETE);
        //Leaderboard
        [Header("Leaderboard Endpoints")]
        [Header("---------------------------")]
        public static EndPointClass createLeaderboard = new EndPointClass("/game/{0}/leaderboards", LootLockerHTTPMethod.POST);
        public static EndPointClass getLeaderboards = new EndPointClass("/game/{0}/leaderboards?count={1}", LootLockerHTTPMethod.GET);


        public static string email;
        public static string password;

        public static string token;

        public static void InitialAuthenticationRequest(LootLockerInitialAuthRequest data, Action<LootLockerAuthResponse> onComplete)
        {

            string json = "";
            if (data == null) return;
            else json = JsonConvert.SerializeObject(data);

            EndPointClass endPoint = initialAuthenticationRequest;

            LootLockerServerRequest.CallAPI(endPoint.endPoint, endPoint.httpMethod, json, (serverResponse) =>
            {
                var response = new LootLockerAuthResponse();
                if (string.IsNullOrEmpty(serverResponse.Error))
                {
                    response = JsonConvert.DeserializeObject<LootLockerAuthResponse>(serverResponse.text);

                    if (response.mfa_key == null)
                    {
                        LootLockerConfig.current.adminToken = response.auth_token;
                    }

                    response.text = serverResponse.text;

                    email = data.email;

                    password = data.password;

                    onComplete?.Invoke(response);
                }
                else
                {
                    response.success = serverResponse.success;
                    response.Error = serverResponse.Error; 
                    response.statusCode = serverResponse.statusCode;
                    onComplete?.Invoke(response);
                }
            }, useAuthToken: false, callerRole: LootLocker.LootLockerEnums.LootLockerCallerRole.Admin);
        }

        public static void TwoFactorAuthVerification(LootLockerTwoFactorAuthVerficationRequest data, Action<LootLockerAuthResponse> onComplete)
        {
            string json = "";
            if (data == null) return;
            else json = JsonConvert.SerializeObject(data);

            EndPointClass endPoint = twoFactorAuthenticationCodeVerification;

            LootLockerServerRequest.CallAPI(endPoint.endPoint, endPoint.httpMethod, json, (serverResponse) =>
            {
                var response = new LootLockerAuthResponse();
                if (string.IsNullOrEmpty(serverResponse.Error))
                {
                    response = JsonConvert.DeserializeObject<LootLockerAuthResponse>(serverResponse.text);
                    response.text = serverResponse.text;

                    LootLockerConfig.current.adminToken = (response.auth_token);

                    onComplete?.Invoke(response);
                }
                else
                {
                    response.success = serverResponse.success;
                    response.Error = serverResponse.Error; response.statusCode = serverResponse.statusCode;
                    onComplete?.Invoke(response);
                }
            }, useAuthToken: false, callerRole: LootLocker.LootLockerEnums.LootLockerCallerRole.Admin);
        }

        public static void CreatingAGame(LootLockerCreatingAGameRequest data, Action<LootLockerCreatingAGameResponse> onComplete)
        {
            string json = "";
            if (data == null) return;
            else json = JsonConvert.SerializeObject(data);

            EndPointClass endPoint = creatingAGame;

            LootLockerServerRequest.CallAPI(endPoint.endPoint, endPoint.httpMethod, json, (serverResponse) =>
            {
                LootLockerCreatingAGameResponse response = new LootLockerCreatingAGameResponse();
                if (string.IsNullOrEmpty(serverResponse.Error))
                {
                    response = JsonConvert.DeserializeObject<LootLockerCreatingAGameResponse>(serverResponse.text);
                    response.text = serverResponse.text;
                    onComplete?.Invoke(response);
                }
                else
                {
                    response.success = serverResponse.success;
                    response.Error = serverResponse.Error; response.statusCode = serverResponse.statusCode;
                    onComplete?.Invoke(response);
                }
            }, useAuthToken: true, callerRole: LootLocker.LootLockerEnums.LootLockerCallerRole.Admin);
        }

        //Both this and the previous call share the same response
        public static void GetDetailedInformationAboutAGame(LootLockerGetRequest lootLockerGetRequest, Action<LootLockerCreatingAGameResponse> onComplete)
        {
            EndPointClass endPoint = getDetailedInformationAboutAGame;

            string getVariable = string.Format(endPoint.endPoint, lootLockerGetRequest.getRequests[0]);

            LootLockerServerRequest.CallAPI(getVariable, endPoint.httpMethod, "", (serverResponse) =>
            {
                LootLockerCreatingAGameResponse response = new LootLockerCreatingAGameResponse();
                if (string.IsNullOrEmpty(serverResponse.Error))
                {
                    response = JsonConvert.DeserializeObject<LootLockerCreatingAGameResponse>(serverResponse.text);
                    response.text = serverResponse.text;
                    onComplete?.Invoke(response);
                }
                else
                {
                    response.success = serverResponse.success;
                    response.Error = serverResponse.Error; response.statusCode = serverResponse.statusCode;
                    onComplete?.Invoke(response);
                }
            }, useAuthToken: true, callerRole: LootLocker.LootLockerEnums.LootLockerCallerRole.Admin);
        }

        public static void CreateLeaderboard(int gameid, LootLockerCreateLeaderboardRequest data, Action<LootLockerLeaderboardCreationResponse> onComplete)
        {
            EndPointClass endPoint = createLeaderboard;

            string getVariable = string.Format(endPoint.endPoint, gameid);

            string json = "";
            if (data == null) return;
            else json = JsonConvert.SerializeObject(data);

            LootLockerServerRequest.CallAPI(getVariable, endPoint.httpMethod, json, (serverResponse) =>
             {
                 LootLockerLeaderboardCreationResponse response = new LootLockerLeaderboardCreationResponse();
                 if (string.IsNullOrEmpty(serverResponse.Error))
                 {
                     response = JsonConvert.DeserializeObject<LootLockerLeaderboardCreationResponse>(serverResponse.text);
                     response.text = serverResponse.text;
                     response.success = true;
                     response.Error = serverResponse.Error;
                     response.statusCode = serverResponse.statusCode;
                     onComplete?.Invoke(response);
                 }
                 else
                 {
                     response.success = serverResponse.success;
                     response.Error = serverResponse.Error; response.statusCode = serverResponse.statusCode;
                     onComplete?.Invoke(response);
                 }
             }, useAuthToken: true, callerRole: LootLocker.LootLockerEnums.LootLockerCallerRole.Admin);

        }

        public static void ListLeaderboards(LootLockerListLeaderboardRequest getValues, Action<LootLockerListLeaderboardsResponse> onComplete)
        {
            EndPointClass endPoint = getLeaderboards;
            string endPointStr = endPoint.endPoint;
            string getVariable = string.Format(endPointStr, getValues.gameId, getValues.count);


            if (!string.IsNullOrEmpty(getValues.after))
            {
                getVariable = endPoint.endPoint + "&after={2}";
                endPointStr = string.Format(getVariable, getValues.gameId, getValues, int.Parse(getValues.after));
            }

            LootLockerServerRequest.CallAPI(getVariable, endPoint.httpMethod, "", (serverResponse) =>
            {
                LootLockerListLeaderboardsResponse response = new LootLockerListLeaderboardsResponse();
                if (string.IsNullOrEmpty(serverResponse.Error))
                {
                    response = JsonConvert.DeserializeObject<LootLockerListLeaderboardsResponse>(serverResponse.text);
                    response.text = serverResponse.text;
                    response.success = true;
                    response.Error = serverResponse.Error;
                    response.statusCode = serverResponse.statusCode;
                    onComplete?.Invoke(response);
                }
                else
                {
                    response.success = serverResponse.success;
                    response.Error = serverResponse.Error; 
                    response.statusCode = serverResponse.statusCode;
                    onComplete?.Invoke(response);
                }
            }, useAuthToken: true, callerRole: LootLocker.LootLockerEnums.LootLockerCallerRole.Admin);

        }

    }

}