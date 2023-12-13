using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class JoinClient : MonoBehaviour
{

    [SerializeField] TMP_InputField joinCodeInput;

    //private async void Start()
    //{
    //    await UnityServices.InitializeAsync();

    //    AuthenticationService.Instance.SignedIn += () =>
    //    {
    //        Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
    //    };
    //    await AuthenticationService.Instance.SignInAnonymouslyAsync();

    //}
    //public void JoinRelayButton()
    //{
    //    Debug.Log("JoinRelayButton clicked!");
    //    JoinRelay(joinCodeInput.text);
    //}

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("JoinRelay code = " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
