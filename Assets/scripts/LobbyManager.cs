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
            int maxPlayers = 4;
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
}