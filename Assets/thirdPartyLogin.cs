using AccelByte.Api;
#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services;
#endif
#if UNITY_STADIA
using UnityEngine.Stadia;
using Unity.StadiaWrapper;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class thirdPartyLogin : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    Windows.Xbox.System.User mCurrentUser = null;
    XboxLiveContext mContext = null;
#endif
    public InputField Logs;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_PS4
        try
        {
            Sony.NP.Main.OnAsyncEvent += OnAsyncEvent;
            Sony.NP.InitToolkit init = new Sony.NP.InitToolkit();
            init.SetPushNotificationsFlags(Sony.NP.PushNotificationsFlags.NewInGameMessage |
                                    Sony.NP.PushNotificationsFlags.NewInvitation | Sony.NP.PushNotificationsFlags.UpdateBlockedUsersList |
                                    Sony.NP.PushNotificationsFlags.UpdateFriendPresence | Sony.NP.PushNotificationsFlags.UpdateFriendsList);

            Sony.NP.Main.Initialize(init);

            PrintLog("\n NpToolkit has been initialized.");
        }
        catch (Sony.NP.NpToolkitException e)
        {
            Debug.Log("Exception thrown - test past.");
            PrintLog("\n NpToolkit failed to initialized. Error: " +e.Message);
        }
        catch (System.Exception e)
        {
            // Unexcepted expection occured.
            Debug.LogException(e);
            PrintLog("\n NpToolkit failed to initialized. Error: " + e.Message);
        }
#endif
#if ENABLE_WINMD_SUPPORT
        mCurrentUser = Windows.Xbox.ApplicationModel.Core.CoreApplicationContext.CurrentUser;
        mContext = new XboxLiveContext(mCurrentUser);
#endif
#if UNITY_PS4 || ENABLE_WINMD_SUPPORT || UNITY_STADIA
        PrintLog("\n Start Sign-in");
        SignIn();
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }

#if ENABLE_WINMD_SUPPORT
    void SignIn()
    {
        try
        {
            //TODO: URL needs to be changed
            string url = "";
            var result = mCurrentUser.GetTokenAndSignatureAsync("POST", url, "");
            PrintLog("\n After GetTokenAndSignatureAsync...." + result);

            while (result.Status != Windows.Foundation.AsyncStatus.Completed)
            {
                PrintLog("\n Waiting...." + result.Status);
                System.Threading.Thread.Sleep(1000);
            }

            PrintLog("\nReturn Code: " + result.ErrorCode);
            PrintLog("\nAuthCode: " + result.GetResults().Token);
            var user = AccelBytePlugin.GetUser();
            PrintLog("\nLogin to AB");
            user.LoginWithOtherPlatform(AccelByte.Models.PlatformType.Live, result.GetResults().Token, loginResult =>
            {
                if (!loginResult.IsError)
                {
                    PrintLog("\nLogin Success!");
                    user.GetData(resultData =>
                    {
                        if (!resultData.IsError)
                        {
                            PrintLog("\nUserId: " + resultData.Value.userId);
                            PrintLog("\nDisplayName: " + resultData.Value.displayName);
                        }
                    });
                }
            });

        }
        catch (Exception ex)
        {
            PrintLog("\n Exception...." + ex);
            return;
        }
    }
#endif
#if UNITY_PS4
    void SignIn()
    {
        try
        {
            Sony.NP.Auth.GetAuthCodeRequest request = new Sony.NP.Auth.GetAuthCodeRequest();

            // test values from SDK nptoolkit sample ... replace with your own project values
            Sony.NP.Auth.NpClientId clientId = new Sony.NP.Auth.NpClientId();
            clientId.Id = "";

            request.ClientId = clientId;
            request.Scope = "psn:s2s";
            request.UserId = GetLocalProfiles();
            PrintLog("\nGet UserId Success! UserId: " + request.UserId);

            Sony.NP.Auth.AuthCodeResponse response = new Sony.NP.Auth.AuthCodeResponse();
            int requestId = Sony.NP.Auth.GetAuthCode(request, response);
            while (response.Locked)
            {
                new WaitForSeconds(1);
            }
            PrintLog("\n Return Code: " + response.ReturnCode);
            PrintLog("\nIssuerId: " + response.IssuerId);
            if (!response.IsErrorCode)
            {
                PrintLog("\nAuthCode: " + response.AuthCode);
                var user = AccelBytePlugin.GetUser();
                PrintLog("\nLogin to AB");
                user.LoginWithOtherPlatform(AccelByte.Models.PlatformType.PS4, response.AuthCode, result =>
                {
                    if (!result.IsError)
                    {
                        PrintLog("\nLogin Success!");
                        user.GetData(resultData =>
                        {
                            if (!resultData.IsError)
                            {
                                PrintLog("\nUserId: " + resultData.Value.userId);
                                PrintLog("\nDisplayName: " + resultData.Value.displayName);
                            }
                        });
                    }
                });
            }
        }
        catch (Sony.NP.NpToolkitException e)
        {
            PrintLog("\nException : " + e.ExtendedMessage);
        }
    }

    public Sony.NP.Core.UserServiceUserId GetLocalProfiles()
    {
        Sony.NP.UserProfiles.LocalUsers users = new Sony.NP.UserProfiles.LocalUsers();

        try
        {
            PrintLog("\nGet Local Profiles");

            Sony.NP.UserProfiles.GetLocalUsers(users);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            if (e.SceErrorCode != (int)Sony.NP.Core.ReturnCodes.SUCCESS)
            {
                PrintLog("\nException : " + e.ExtendedMessage);
                return Sony.NP.Core.UserServiceUserId.UserIdInvalid;
            }
        }

        // Even if an exception has occured check the results for each user. 
        for (int i = 0; i < users.LocalUsersIds.Length; i++)
        {
            if (users.LocalUsersIds[i].UserId.Id != Sony.NP.Core.UserServiceUserId.UserIdInvalid)
            {
                if (users.LocalUsersIds[i].SceErrorCode == (int)Sony.NP.Core.ReturnCodes.SUCCESS)
                {
                    PrintLog("\nUser id = " + users.LocalUsersIds[i].UserId.ToString() + " : Account id = " + users.LocalUsersIds[i].AccountId.ToString());
                }
                else
                {
                    string output = Sony.NP.Core.ConvertSceErrorToString(users.LocalUsersIds[i].SceErrorCode);

                    // Some error code has been returned for the user. This may just mean they are not singed in or have not signed up for an online account.
                    PrintLog("\nUser id = " + users.LocalUsersIds[i].UserId.ToString() + " : Account id = " + users.LocalUsersIds[i].AccountId.ToString() + " : Error = " + output);
                }
                    return users.LocalUsersIds[i].UserId;
            }
        }
        return Sony.NP.Core.UserServiceUserId.UserIdInvalid;
    }

    public void OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
    {
        if (callbackEvent.Service == Sony.NP.ServiceTypes.Auth)
        {
            switch (callbackEvent.ApiCalled)
            {
                case Sony.NP.FunctionTypes.AuthGetAuthCode:
                    OutputAuthCode(callbackEvent.Response as Sony.NP.Auth.AuthCodeResponse);
                    break;
                default:
                    break;
            }
        }
    }

    private void OutputAuthCode(Sony.NP.Auth.AuthCodeResponse response)
    {
        if (response == null) return;

        PrintLog("Auth Code Response");

        if (response.Locked == false)
        {
            PrintLog("AuthCode : " + response.AuthCode);
            PrintLog("IssuerId : " + response.IssuerId);
            var user = AccelBytePlugin.GetUser();
            PrintLog("\nLogin to AB");
            user.LoginWithOtherPlatform(AccelByte.Models.PlatformType.PS4, response.AuthCode, result =>
            {
                if (!result.IsError)
                {
                    PrintLog("\nLogin Success!");
                    user.GetData(resultData =>
                    {
                        if (!resultData.IsError)
                        {
                            PrintLog("\nUserId: " + resultData.Value.userId);
                            PrintLog("\nDisplayName: " + resultData.Value.displayName);
                        }
                    });
                }
            });
        }
    }
#endif
#if UNITY_STADIA
    void SignIn()
    {
        GgpPlayerId playerId = StadiaNativeApis.GgpGetPrimaryPlayerId();

        float startTime = Time.realtimeSinceStartup;
        while (playerId.Value == (int)GgpIdConstants.kGgpInvalidId && Time.realtimeSinceStartup - startTime < 10f)
        {
            new WaitForSeconds(0.5f);
            playerId = StadiaNativeApis.GgpGetPrimaryPlayerId();
        }
        if (playerId.Value == (int)GgpIdConstants.kGgpInvalidId)
        {
            PrintLog("\n[STADIA] Can't retrieve playerId!");
        }
        PrintLog("\n[STADIA] PlayerId: " + playerId.Value);

        GgpStatus reqStatus;
        GgpPlayerJwt playerJwt = StadiaNativeApis.GgpGetJwtForPlayer(playerId, 1000, new GgpJwtFields((ulong)GgpJwtFieldValues.kGgpJwtField_None)).GetResultBlocking<GgpPlayerJwt>(out reqStatus);

        PrintLog("\nPlayerJwt: " + playerJwt.jwt);

        var user = AccelBytePlugin.GetUser();
        PrintLog("\nLogin to AB");
        user.LoginWithOtherPlatform(AccelByte.Models.PlatformType.Stadia, playerJwt.jwt, loginResult =>
        {
            if (!loginResult.IsError)
            {
                PrintLog("\nLogin Success!");
                user.GetData(resultData =>
                {
                    if (!resultData.IsError)
                    {
                        PrintLog("\nUserId: " + resultData.Value.userId);
                        PrintLog("\nDisplayName: " + resultData.Value.displayName);
                    }
                });
            }
            else
            {
                PrintLog("\nCannot logged in.");
                PrintLog("\nError: " + loginResult.Error.Code + " | Message: " + loginResult.Error.Message);
            }
        });
    }
#endif
    public void PrintLog(string Message)
    {
        Logs.text += (Message);
        Debug.Log(Message);
    }
}