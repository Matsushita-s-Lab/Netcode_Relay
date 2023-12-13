using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyListView : MonoBehaviour
{
    [SerializeField] private GameObject lobbyBannerPrefab;
    [SerializeField] private Transform lobbyListContent;
    [SerializeField] private Button refreshButton;

    public static EventHandler OnRefreshLobbyListRequest;

    private void Start()
    {
        refreshButton.onClick.AddListener(() => {
            OnRefreshLobbyListRequest?.Invoke(this, EventArgs.Empty);
        });
    }
    public void Refresh(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var lobby in lobbyList)
        {
            GameObject lobbyBanner = Instantiate(lobbyBannerPrefab, lobbyListContent);
            lobbyBanner.GetComponent<LobbyBanner>().Init(lobby);
        }
    }
}