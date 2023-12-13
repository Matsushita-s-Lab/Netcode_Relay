using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class RelayTest2 : MonoBehaviour
{
    //[SerializeField] TMP_InputField joinCodeInput;
    //[SerializeField] TextMeshProUGUI JoinCodeText;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // サーバーが起動したときに呼ばれるイベントを設定
        //SetRelayServerData(); // SetRelayServerData を最初に呼ぶ
        StartServerButton();
    }

    // サーバーを起動するメソッド
    public async void StartServerButton()
    {
        Debug.Log("Server has started!");

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            Debug.Log("Join Code on Server: " + (string.IsNullOrEmpty(joinCode) ? "JoinCodeText is empty" : joinCode));
            

            NetworkManager.Singleton.StartServer();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    // クライアント側
    // コードはデバッグとして目視で確認
    //public void JoinRelayButton()
    //{
    //    Debug.Log("JoinRelayButton clicked!");
    //    JoinRelay(joinCodeInput.text);
    //}

    //public async void JoinRelay(string joinCode)
    //{
    //    try
    //    {
    //        Debug.Log("JoinRelay code = " + joinCode);
    //        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
    //            joinAllocation.RelayServer.IpV4,
    //            (ushort)joinAllocation.RelayServer.Port,
    //            joinAllocation.AllocationIdBytes,
    //            joinAllocation.Key,
    //            joinAllocation.ConnectionData,
    //            joinAllocation.HostConnectionData
    //        );

    //        NetworkManager.Singleton.StartClient();
    //    }
    //    catch (RelayServiceException e)
    //    {
    //        Debug.Log(e);
    //    }
    //}
}
