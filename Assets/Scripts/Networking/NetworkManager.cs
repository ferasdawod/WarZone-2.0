using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : PunBehaviour
{
    #region Singleton

    private static NetworkManager _instance;
    public static NetworkManager Instance { get { return _instance; } }

    #endregion

    public string PlayerId { get; private set; }
    public bool IsLoggedIn { get { return !string.IsNullOrEmpty(PlayerId); } }

    #region Events

    public event Action OnLoginStart;
    public event Action OnLogin;

    public event Action OnJoinRoomStart;
    public event Action OnCreateRoomStart;
    public event Action OnRoomJoined;

    #endregion

    private void Awake()
    {
        if (_instance != null) Debug.LogError("NetworkManager already exists in the scene", this);

        _instance = this;
        DontDestroyOnLoad(this);
    }

    #region Authentication

    public void Login()
    {
        if (IsLoggedIn) Debug.LogError("Attempting to login again", this);

        Debug.Log("Attempting to login", this);
        if (OnLoginStart != null) OnLoginStart.Invoke();

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = PlayFabSettings.DeviceUniqueIdentifier,
        }, OnLoginResult, OnLoginError);
    }

    private void OnLoginResult(LoginResult loginResult)
    {
        Debug.Log("Playfab login successful");

        PlayerId = loginResult.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID,
        }, OnPhotonTokenReceived, OnPhotonTokenFailed);
    }

    private void OnPhotonTokenReceived(GetPhotonAuthenticationTokenResult result)
    {
        Debug.Log("Received photon token");

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", PlayerId);    // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
        customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);

        //We finally tell Photon to use this authentication parameters throughout the entire application.
        PhotonNetwork.AuthValues = customAuth;

        //Finally we will initialize photon with any custom data
        InitializePhoton();

        if (OnLogin != null) OnLogin();
    }

    private void OnLoginError(PlayFabError playFabError)
    {
        Debug.LogError("Failed to login with playfab");
        Debug.LogError(playFabError.ErrorMessage);
        if (OnLogin != null) OnLogin();
    }

    private void OnPhotonTokenFailed(PlayFabError playFabError)
    {
        Debug.LogError("Failed to get photon token");
        Debug.LogError(playFabError.ErrorMessage);

        if (OnLogin != null) OnLogin();
    }

    #endregion

    #region Create And Join Games

    public void StartGame()
    {
        if (PhotonNetwork.connected)
        {
            Debug.Log("we are already connected");
            return;
        }

        Debug.Log("Starting a game");
        if (OnJoinRoomStart != null) OnJoinRoomStart();

        if (!PhotonNetwork.ConnectUsingSettings("0.1")) Debug.LogError("Failed to connect to multi player servers");
    }

    #endregion

    private void InitializePhoton()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRoom("feras");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonJoinRoomFailed");

        if (OnCreateRoomStart != null) OnCreateRoomStart();

        var result = PhotonNetwork.CreateRoom("feras");
        Debug.Log("CREATE ROOM RESULT = " + result);
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonRandomJoinFailed");

        if (OnCreateRoomStart != null) OnCreateRoomStart();

        var result = PhotonNetwork.CreateRoom("feras");
        Debug.Log("CREATE ROOM RESULT = " + result);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        if (OnRoomJoined != null) OnRoomJoined();
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.LogError("OnPhotonCreateRoomFailed");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        PhotonNetwork.LoadLevel(1);
    }

    #endregion
}
