using AccelByte.Api;
#if ENABLE_WINMD_SUPPORT
using Microsoft.Xbox.Services;
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
        Sony.NP.InitToolkit init = new Sony.NP.InitToolkit();

        try
        {
            init.SetPushNotificationsFlags(Sony.NP.PushNotificationsFlags.NewGameDataMessage | Sony.NP.PushNotificationsFlags.NewInGameMessage |
                                    Sony.NP.PushNotificationsFlags.NewInvitation | Sony.NP.PushNotificationsFlags.UpdateBlockedUsersList |
                                    Sony.NP.PushNotificationsFlags.UpdateFriendPresence | Sony.NP.PushNotificationsFlags.UpdateFriendsList);

            Sony.NP.Main.Initialize(init);

            Debug.LogAssertion("No exception was thrown even though NpToolkit should have already been initialized.");
            Logs.text += "\n NpToolkit has been initialized.";
        }
        catch (Sony.NP.NpToolkitException e)
        {
            Debug.Log("Exception thrown - test past.");
            Logs.text += "\n NpToolkit failed to initialized. Error: "+e.Message;
        }
        catch (System.Exception e)
        {
            // Unexcepted expection occured.
            Debug.LogException(e);
            Logs.text += "\n NpToolkit failed to initialized. Error: " + e.Message;
        }
#endif
#if ENABLE_WINMD_SUPPORT
        mCurrentUser = Windows.Xbox.ApplicationModel.Core.CoreApplicationContext.CurrentUser;
        mContext = new XboxLiveContext(mCurrentUser);
#endif
#if UNITY_PS4 || ENABLE_WINMD_SUPPORT
        Logs.text += "\n Start Sign-in";
        SignIn();
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }

#if ENABLE_WINMD_SUPPORT
​
    void SignIn()
    {
        try
        {
            //TODO: URL needs to be changed
            string url = "";​
            var result = mCurrentUser.GetTokenAndSignatureAsync("POST", url, "");
            Logs.text += "\n After GetTokenAndSignatureAsync...." + result;
​
            while (result.Status != Windows.Foundation.AsyncStatus.Completed)
            {
                Logs.text += "\n Waiting...." + result.Status;
                System.Threading.Thread.Sleep(1000);
            }
​
            Logs.text += "\nReturn Code: " + result.ErrorCode;
            Logs.text += "\nAuthCode: " + result.GetResults().Token;
            var user = AccelBytePlugin.GetUser();
            Logs.text += "\nLogin to AB";
            user.LoginWithOtherPlatform(AccelByte.Models.PlatformType.Live, result.GetResults().Token, loginResult =>
            {
                if (!loginResult.IsError)
                {
                    Logs.text += "\nLogin Success!";
                    user.GetData(resultData =>
                    {
                        if (!resultData.IsError)
                        {
                            Logs.text += "\nUserId: " + resultData.Value.userId;
                            Logs.text += "\nDisplayName: " + resultData.Value.displayName;
                        }
                    });
                }
            });

        }
        catch (Exception ex)
        {
            Logs.text += "\n Exception...." + ex;
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

            Sony.NP.Auth.AuthCodeResponse response = new Sony.NP.Auth.AuthCodeResponse();

            int requestId = Sony.NP.Auth.GetAuthCode(request, response);
            while (response.Locked)
            {
                new WaitForSeconds(1);
            }
            Logs.text += "\n Return Code: "+response.ReturnCode;
            Logs.text += "\nIssuerId: " + response.IssuerId;
            if (!response.IsErrorCode)
            {
                Logs.text += "\nAuthCode: " + response.AuthCode;
                var user = AccelBytePlugin.GetUser();
                Logs.text += "\nLogin to AB";
                user.LoginWithOtherPlatform(AccelByte.Models.PlatformType.PS4, response.AuthCode, result =>
                {
                    if (!result.IsError) {
                        Logs.text += "\nLogin Success!";
                        user.GetData(resultData =>
                        {
                            if (!resultData.IsError)
                            {
                                Logs.text += "\nUserId: " + resultData.Value.userId;
                                Logs.text += "\nDisplayName: " + resultData.Value.displayName;
                            }
                        });
                    }
                });
            }
        }
        catch (Sony.NP.NpToolkitException e)
        {
            Logs.text+= ("\nException : " + e.ExtendedMessage);
        }
    }

    public Sony.NP.Core.UserServiceUserId GetLocalProfiles()
    {
        Sony.NP.UserProfiles.LocalUsers users = new Sony.NP.UserProfiles.LocalUsers();

        try
        {
            Logs.text+=("\nGet Local Profiles");

            Sony.NP.UserProfiles.GetLocalUsers(users);
        }
        catch (Sony.NP.NpToolkitException e)
        {
            if (e.SceErrorCode != (int)Sony.NP.Core.ReturnCodes.SUCCESS)
            {
                Logs.text+=("\nException : " + e.ExtendedMessage);
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
                    Logs.text+=("\nUser id = " + users.LocalUsersIds[i].UserId.ToString() + " : Account id = " + users.LocalUsersIds[i].AccountId.ToString());
                }
                else
                {
                    string output = Sony.NP.Core.ConvertSceErrorToString(users.LocalUsersIds[i].SceErrorCode);

                    // Some error code has been returned for the user. This may just mean they are not singed in or have not signed up for an online account.
                    Logs.text += ("\nUser id = " + users.LocalUsersIds[i].UserId.ToString() + " : Account id = " + users.LocalUsersIds[i].AccountId.ToString() + " : Error = " + output);
                }
                    return users.LocalUsersIds[i].UserId;
            }
        }
        return Sony.NP.Core.UserServiceUserId.UserIdInvalid;
    }
#endif
}