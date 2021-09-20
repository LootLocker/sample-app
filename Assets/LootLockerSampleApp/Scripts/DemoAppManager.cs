using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker;
using LootLocker.Requests;
using LootLockerDemoApp;

namespace LootLockerDemoApp
{
    public class LootLockerAuthResponse : LootLockerResponse
    {
        
        public string auth_token { get; set; }
        public LootLockerUser user { get; set; }
        public string mfa_key { get; set; }
    }

    public class LootLockerInitialAuthRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class LootLockerTwoFactorAuthVerficationRequest
    {
        public string mfa_key { get; set; }
        public string secret { get; set; }
    }


    #region SubsequentRequests

    public class LootLockerSubsequentRequestsResponse : LootLockerResponse
    {
        
        public LootLockerGame[] games { get; set; }
    }

    #endregion

    public class LootLockerUser
    {
        public string name { get; set; }
        public LootLockerOrganisation[] organisations { get; set; }
    }

    public class LootLockerOrganisation
    {
        public int id { get; set; }
        public string name { get; set; }
        public LootLockerGameAndDevelopment[] games { get; set; }
    }
    public class LootLockerGameAndDevelopment
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool sandbox_mode { get; set; }
        public bool is_demo;
        public LootLockerGame development { get; set; }
    }

    public class LootLockerGame
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool sandbox_mode { get; set; }

        public bool is_demo;
    }


    public class LootLockerCreatingAGameResponse : LootLockerResponse
    {
        
        public LootLockerCAGGame game { get; set; }
    }

    public class LootLockerCAGGame
    {
        public int id { get; set; }
        public string name { get; set; }
        public string game_key { get; set; }
        public string steam_app_id { get; set; }
        public string steam_api_key { get; set; }
        public bool sandbox_mode { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public LootLockerCAGGame development { get; set; }
    }

    public class LootLockerCreatingAGameRequest
    {

        public string name, steam_app_id;
        public bool sandbox_mode;
        public int organisation_id;
        public bool demo;

    }

    public class LootLockerVerifyTwoFactorAuthenticationRequest
    {

        public int secret;

    }

    public class LootLockerSetupTwoFactorAuthenticationResponse : LootLockerResponse
    {

        
        public string mfa_token_url { get; set; }

    }
    public class LootLockerVerifyTwoFactorAuthenticationResponse : LootLockerResponse
    {
        
        public string recover_token { get; set; }
    }

    public class LootLockerRemoveTwoFactorAuthenticationResponse : LootLockerResponse
    {
        
        public string error { get; set; }
    }

    public enum LeaderboardDirection { Direction, ascending, descending }
    public enum LeaderboardType { Type, player, generic }

    public class LootLockerCreateLeaderboardRequest
    {
        public string name { get; set; }
      //  public string key { get; set; }
        public string direction_method { get; set; }
        public bool enable_game_api_writes { get; set; }
        public bool overwrite_score_on_submit { get; set; }
        public string type { get; set; }
    }

    public class LootLockerLeaderboardCreationResponse : LootLockerResponse
    {
        public int id { get; set; }
        public string key { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string direction_method { get; set; }
        public bool enable_game_api_writes { get; set; }
        public bool overwrite_score_on_submit { get; set; }
        public int game_id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class LootLockerListLeaderboardRequest : LootLockerGetRequests
    {
        public int gameId { get; set; }
        public static int? nextCursor;
        public static int? prevCursor;
        public static void Reset()
        {
            nextCursor = 0;
            prevCursor = 0;
        }
    }

    public class LootLockerListLeaderboardsResponse : LootLockerResponse
    {
        public LootLockerLeaderboardsPagination pagination { get; set; }
        public LootLockerListLeaderboardsItem[] items { get; set; }
    }

    public class LootLockerLeaderboardsPagination
    {
        public int total { get; set; }
        public int? next_cursor { get; set; }
        public int? previous_cursor { get; set; }
        public bool allowNext { get; set; }
        public bool allowPrev { get; set; }
    }

    public class LootLockerListLeaderboardsItem: ILootLockerScreenData,IListData
    {
        public int id { get; set; }
        public string key { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string direction_method { get; set; }
        public bool enable_game_api_writes { get; set; }
        public bool overwrite_score_on_submit { get; set; }
        public int game_id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int index { get ; set ; }
        public ListPopulator listParent { get ; set ; }
    }



    public class DemoSDKManager : LootLockerSDKManager
    {
        #region Authentication
        /// <summary>
        /// This is an admin call, please do not use this as a normal call. This is not intended to be used to connect to players. Lootlocker does not currently support
        /// username and password login for normal users
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="onComplete"></param>

        public static void InitialAuthRequest(string email, string password, Action<LootLockerAuthResponse> onComplete)
        {
            if (!CheckInitialized()) return;
            var data = new LootLockerInitialAuthRequest();
            data.email = email;
            data.password = password;
            DemoAppAdminRequests.InitialAuthenticationRequest(data, onComplete);
        }
        /// <summary>
        /// This is also an admin call, please do not use this for the normal game
        /// </summary>
        /// <param name="mfa_key"></param>
        /// <param name="secret"></param>
        /// <param name="onComplete"></param>
        public static void TwoFactorAuthVerification(string mfa_key, string secret, Action<LootLockerAuthResponse> onComplete)
        {

            if (!CheckInitialized()) return;
            var data = new LootLockerTwoFactorAuthVerficationRequest();
            data.mfa_key = mfa_key;
            data.secret = secret;
            DemoAppAdminRequests.TwoFactorAuthVerification(data, onComplete);

        }

        #endregion
        /// <summary>
        /// This is an admin call, for creating games
        /// </summary>
        /// <param name="name"></param>
        /// <param name="steam_app_id"></param>
        /// <param name="sandbox_mode"></param>
        /// <param name="organisation_id"></param>
        /// <param name="demo"></param>
        /// <param name="onComplete"></param>
        public static void CreatingAGame(string name, string steam_app_id, bool sandbox_mode, int organisation_id, bool demo, Action<LootLockerCreatingAGameResponse> onComplete)
        {

            // if (!CheckInitialized()) return;

            LootLockerCreatingAGameRequest data = new LootLockerCreatingAGameRequest
            {

                name = name,
                steam_app_id = steam_app_id,
                sandbox_mode = sandbox_mode,
                organisation_id = organisation_id,
                demo = demo

            };

            DemoAppAdminRequests.CreatingAGame(data, onComplete);

        }

        public static void GetDetailedInformationAboutAGame(string id, Action<LootLockerCreatingAGameResponse> onComplete)
        {
            //if (!CheckInitialized()) return;
            LootLockerGetRequest lootLockerGetRequest = new LootLockerGetRequest();
            lootLockerGetRequest.getRequests.Add(id.ToString());
            DemoAppAdminRequests.GetDetailedInformationAboutAGame(lootLockerGetRequest, onComplete);
        }


        public static void CreateLeaderboard(int gameid, LootLockerCreateLeaderboardRequest data, Action<LootLockerLeaderboardCreationResponse> onComplete)
        {
            DemoAppAdminRequests.CreateLeaderboard(gameid, data, onComplete);
        }

        public static void ListLeaderboards(LootLockerListLeaderboardRequest getValues, Action<LootLockerListLeaderboardsResponse> onComplete)
        {
            Action<LootLockerListLeaderboardsResponse> callback = (response) =>
            {
                if (response != null && response.pagination != null)
                {
                    LootLockerListLeaderboardRequest.nextCursor = response.pagination.next_cursor;
                    LootLockerListLeaderboardRequest.prevCursor = response.pagination.previous_cursor;
                    response.pagination.allowNext = response.pagination.next_cursor > 0;
                    response.pagination.allowPrev = (response.pagination.previous_cursor != null);
                }
                onComplete?.Invoke(response);
            };
            DemoAppAdminRequests.ListLeaderboards(getValues, callback);
        }

        //public static void ListTriggers(int game_id, Action<LootLockerListTriggersResponse> onComplete)
        //{
        //  //  if (!CheckInitialized()) return;
        //    LootLockerGetRequest data = new LootLockerGetRequest();
        //    data.getRequests.Add(game_id.ToString());
        //    DemoAppAdminRequests.ListTriggers(data, onComplete);
        //}

        //public static void CreateTriggers(LootLockerCreateTriggersRequest requestData, int game_id, Action<LootLockerListTriggersResponse> onComplete)
        //{
        //  //  if (!CheckInitialized()) return;
        //    LootLockerGetRequest data = new LootLockerGetRequest();
        //    data.getRequests.Add(game_id.ToString());
        //    DemoAppAdminRequests.CreateTriggers(requestData, data, onComplete);
        //}
    }

}