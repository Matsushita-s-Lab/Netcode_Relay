using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;

public class LobbyBanner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button joinButton;

    public Lobby myLobby { get; private set; }
    public static EventHandler<Lobby> OnJoinLobby;

    public void Init(Lobby lobby)
    {
        myLobby = lobby;
        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();
        joinButton.onClick.AddListener(() => {
            OnJoinLobby?.Invoke(this, myLobby);
            Debug.Log("JoinLobby LobbyID=" + myLobby.Id.ToString());
        });
    }
}