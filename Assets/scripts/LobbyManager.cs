using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class LobbyManager : MonoBehaviour
{
    private string playerName;
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
}