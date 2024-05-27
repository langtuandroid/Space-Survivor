using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using GooglePlayGames.BasicApi.SavedGame;
// using GooglePlayGames.BasicApi.Events;

public class GPGSManager
{
    static GPGSManager instance = new GPGSManager();
    public static GPGSManager Instance { get { return instance; } } // 싱글톤 구현

    public bool IsAuthenticated { get { return Social.localUser.authenticated; } } // 로그인 여부를 확인합니다.

    void Init()
    {
        // var config = new PlayGamesClientConfiguration.Builder()
        //     .EnableSavedGames() // 클라우드 서비스를 이용하지 않는다면 이 줄을 지웁니다.
        //     .Build();
        // PlayGamesPlatform.InitializeInstance(config);
        // PlayGamesPlatform.DebugLogEnabled = true;
        // PlayGamesPlatform.Activate();
    }

    public void Login(bool autoLogin = false, Action<bool, ILocalUser> callback = null)
    { // 로그인을 시도합니다.(autoLogin : 앱 실행시 자동로그인 => true / 사용자가 직접 로그인 => false)
        Init();
        // PlayGamesPlatform.Instance.Authenticate(autoLogin ? SignInInteractivity.CanPromptOnce : SignInInteractivity.CanPromptAlways, (result) =>
        // {
        //     callback?.Invoke(result == SignInStatus.Success, Social.localUser);
        // });
    }
    // public void Logout() => PlayGamesPlatform.Instance.SignOut();  // 로그아웃합니다.

    public void ShowAchievementsUI() => Social.ShowAchievementsUI(); // 모든 업적 목록을 표시합니다.
    public void RevealAchievement(string gpgsID, float progress, Action<bool> callback = null) => Social.ReportProgress(gpgsID, progress, (bool success) => { callback?.Invoke(success); });  // 해당 업적을 진전시킵니다.
    public void UnlockAchievement(string gpgsID, Action<bool> callback = null) => RevealAchievement(gpgsID, 100f, callback); // 해당 업적을 달성시킵니다.

    public void ShowLeaderboardUI() => Social.ShowLeaderboardUI(); // 모든 리더보드 목록을 표시합니다.
    // public void ShowLeaderboardUI(string gpgsID) => PlayGamesPlatform.Instance.ShowLeaderboardUI(gpgsID); // 해당 리더보드를 표시합니다.
    public void ReportLeaderboard(string gpgsID, long score, Action<bool> callback = null) => Social.ReportScore(score, gpgsID, (bool success) => { callback?.Invoke(success); }); // 리더보드에 점수를 기록합니다.

    // public void FetchEvent(string gpgsID, Action<bool, IEvent> callback = null)
    // { // 해당 이벤트를 표시합니다.
    //     PlayGamesPlatform.Instance.Events.FetchEvent(
    //         DataSource.ReadCacheOrNetwork,
    //         gpgsID,
    //         (status, ievent) =>
    //         {
    //             if (status == ResponseStatus.Success)
    //             {
    //                 callback?.Invoke(true, ievent);
    //             }
    //             else
    //             {
    //                 callback?.Invoke(false, null);
    //             }
    //         }
    //     );
    // }
    // public void FetchAllEvents(Action<bool, List<IEvent>> callback = null)
    // { // 이벤트 목록을 표시합니다.
    //     PlayGamesPlatform.Instance.Events.FetchAllEvents(
    //         DataSource.ReadCacheOrNetwork,
    //         (status, ievents) =>
    //         {
    //             if (status == ResponseStatus.Success)
    //             {
    //                 callback?.Invoke(true, ievents);
    //             }
    //             else
    //             {
    //                 callback?.Invoke(false, null);
    //             }
    //         }
    //     );
    // }
    // public void IncrementEvent(string gpgsID, uint amount) => PlayGamesPlatform.Instance.Events.IncrementEvent(gpgsID, amount); // 해당 이벤트를 업데이트합니다.

    // public void SaveWithCloud(string filename, string serializedFile, Action<bool> callback = null)
    // { // 클라우드 서비스를 이용하여 저장합니다.
    //     ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

    //     if (savedGameClient == null)
    //         callback?.Invoke(false);
    //     else
    //     {
    //         savedGameClient.OpenWithAutomaticConflictResolution(
    //                     filename,
    //                     DataSource.ReadCacheOrNetwork,
    //                     ConflictResolutionStrategy.UseLongestPlaytime,
    //                     (status, game) =>
    //                     {
    //                         if (status == SavedGameRequestStatus.Success)
    //                         {
    //                             var updatedMetadata = new SavedGameMetadataUpdate.Builder().Build();
    //                             savedGameClient.CommitUpdate(
    //                                 game,
    //                                 updatedMetadata,
    //                                 System.Text.Encoding.UTF8.GetBytes(serializedFile),
    //                                 (ustatus, ugame) =>
    //                                 {
    //                                     callback?.Invoke(ustatus == SavedGameRequestStatus.Success);
    //                                 }
    //                             );
    //                         }
    //                         else
    //                         {
    //                             callback?.Invoke(false);
    //                         }
    //                     }
    //                 );
    //     }


    // }
    // public void LoadWithCloud(string filename, Action<bool, string> callback = null)
    // { // 클라우드 서비스를 이용하여 로드합니다.
    //     ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

    //     savedGameClient.OpenWithAutomaticConflictResolution(
    //         filename,
    //         DataSource.ReadCacheOrNetwork,
    //         ConflictResolutionStrategy.UseLastKnownGood,
    //         (status, game) =>
    //         {
    //             if (status == SavedGameRequestStatus.Success)
    //             {
    //                 savedGameClient.ReadBinaryData(game, (lstatus, lserializedFile) =>
    //                 {
    //                     if (lstatus == SavedGameRequestStatus.Success)
    //                     {
    //                         string data_loaded = System.Text.Encoding.UTF8.GetString(lserializedFile);
    //                         callback?.Invoke(true, data_loaded);
    //                     }
    //                     else
    //                     {
    //                         callback?.Invoke(false, null);
    //                     }
    //                 }
    //             );
    //             }
    //             else
    //             {
    //                 callback?.Invoke(false, null);
    //             }
    //         }
    //     );
    // }


    // public void DelectWithCloud(string filename, Action<bool> callback = null)
    // { // 클라우드 서비스에 저장된 데이터를 삭제합니다.
    //     ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

    //     if (savedGameClient == null)
    //         callback?.Invoke(false);
    //     else
    //     {
    //         savedGameClient.OpenWithAutomaticConflictResolution(
    //                     filename,
    //                     DataSource.ReadCacheOrNetwork,
    //                     ConflictResolutionStrategy.UseLongestPlaytime,
    //                     (status, game) =>
    //                     {
    //                         if (status == SavedGameRequestStatus.Success)
    //                         {
    //                             savedGameClient.Delete(game);
    //                             callback?.Invoke(true);
    //                         }
    //                         else
    //                         {
    //                             callback?.Invoke(false);
    //                         }
    //                     }
    //                 );
    //     }

    // }
}