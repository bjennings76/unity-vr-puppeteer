// ---------------------------------------------------------------------
// Copyright (c) 2016 Magic Leap. All Rights Reserved.
// Magic Leap Confidential and Proprietary
// ---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Online
{
	public class OnlineManager : Singleton<OnlineManager>
	{
		public string PlayFabId;

		private void Start()
		{
			Login();
		}

		private void Login()
		{
			LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
			{
				CreateAccount = true,
				CustomId = SystemInfo.deviceUniqueIdentifier,
				InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetUserAccountInfo = true, GetUserData = true }
			};

			PlayFabClientAPI.LoginWithCustomID(request, result => {
				PlayFabId = result.PlayFabId;
				XDebug.Log(this, "Got PlayFabID: {0} {1}", PlayFabId, result.NewlyCreated ? "(new account)" : "(existing account)");
				UpdateUser(result);
			}, error => { XDebug.LogWarning("Error logging in player with custom ID: " + error.ErrorMessage); });
		}

		private void UpdateUser(LoginResult result)
		{
			UpdateUserDisplayName(result);
			UpdateUserDataStuff(result);
		}

		private void UpdateUserDisplayName(LoginResult loginResult)
		{
			if (Environment.UserName == null)
			{
				return;
			}

			if ((loginResult != null) && (loginResult.InfoResultPayload != null) &&
				(loginResult.InfoResultPayload.AccountInfo != null) && (loginResult.InfoResultPayload.AccountInfo.TitleInfo != null) &&
				(loginResult.InfoResultPayload.AccountInfo.TitleInfo.DisplayName != null) &&
				loginResult.InfoResultPayload.AccountInfo.TitleInfo.DisplayName.Equals(Environment.UserName))
			{
				XDebug.Log(this, "Display name matches. Skipping update.");
				return;
			}

			UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = Environment.UserName
			};

			PlayFabClientAPI.UpdateUserTitleDisplayName(request,
				result => { XDebug.Log(this, "User title display updated to {0}", result.DisplayName); },
				error => { XDebug.LogError(this, "Error logging player data: {0}", error.ErrorMessage); });
		}

		private void UpdateUserDataStuff(LoginResult loginResult)
		{
			Dictionary<string, string> data = new Dictionary<string, string> {
				{"In Debug", Debug.isDebugBuild.ToString()},
				{"In Editor", Application.isEditor.ToString()},
				{"Game Version", Application.version},
				{"Unity Version", Application.unityVersion},
				{"Platform", Application.platform.ToString()}
			};

			bool updateNeeded = false;

			if ((loginResult == null) || (loginResult.InfoResultPayload == null) ||
				(loginResult.InfoResultPayload.UserData == null))
			{
				updateNeeded = true;
			}
			else
			{
				foreach (string key in data.Keys)
				{
					UserDataRecord record;
					if (!loginResult.InfoResultPayload.UserData.TryGetValue(key, out record) || !record.Value.Equals(data[key]))
					{
						updateNeeded = true;
						break;
					}
				}
			}

			if (!updateNeeded)
			{
				XDebug.Log(this, "User data matches. Skipping update.");
				return;
			}

			UpdateUserDataRequest request = new UpdateUserDataRequest { Data = data };

			PlayFabClientAPI.UpdateUserData(request,
				result => {
					//XDebug.Log(this, "User data updated to v.{0}:\n{1}", result.DataVersion,
					//	request.Data.AggregateToString("\n", kvp => string.Format("- {0}: {1}", kvp.Key, kvp.Value)));
				},
				error => { XDebug.LogError(this, "Error logging player data: {0}", error.ErrorMessage); });
		}
	}
}