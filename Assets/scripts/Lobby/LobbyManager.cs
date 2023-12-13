using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    private string playerName;
    private Lobby hostLobby;
    private float heartbeatTimer;
    private float heatbeatInterval = 15f;
    [SerializeField] private LobbyListView lobbyListView;

    private async void Start()
    {
        playerName = "PlayerName" + Random.Range(0, 1000).ToString();
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in:" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        LobbyListView.OnRefreshLobbyListRequest += (sender, e) => {
            RefreshLobbies();
        };

        LobbyBanner.OnJoinLobby += (sender, lobby) => {
            Debug.Log("JoinLobby LobbyID=" + lobby.Id.ToString());
            JoinLobby(lobby);
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (hostLobby == null)
            {
                CreateLobby();
            }

            RefreshLobbyHeatbeat();
        }
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "TestLobby";
            int maxPlayers = 2;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = new Player(AuthenticationService.Instance.PlayerId)
                {
                    Data = new Dictionary<string, PlayerDataObject>(){
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,playerName)}
                    }
                }
            };

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            Debug.Log("Lobby created:" + hostLobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Exception:" + e.Message);
        }
    }

    private async void RefreshLobbyHeatbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer += Time.deltaTime;
            if (heartbeatTimer > heatbeatInterval)
            {
                heartbeatTimer = 0.0f;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void RefreshLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies Count:" + queryResponse.Results.Count);
            foreach (var lobby in queryResponse.Results)
            {
                Debug.Log($"Lobby:name {lobby.Name} id {lobby.Id} code {lobby.LobbyCode} maxPlayers {lobby.MaxPlayers}");
            }
            lobbyListView.Refresh(queryResponse.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Exception:" + e.Message);
        }
    }

    private async void JoinLobby(Lobby lobby)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Player = new Player(AuthenticationService.Instance.PlayerId)
                {
                    Data = new Dictionary<string, PlayerDataObject>(){
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,playerName)}
                    }
                },
            };
            hostLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            Debug.Log("Lobby joined:" + hostLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Exception:" + e.Message);
        }
    }
}