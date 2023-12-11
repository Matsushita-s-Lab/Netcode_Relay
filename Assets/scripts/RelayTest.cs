using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class RelayTest : MonoBehaviour
{
    [SerializeField] TMP_InputField joinCodeInput;
    [SerializeField] TextMeshProUGUI JoinCodeText;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    //ホスト側
    public async void CreateRelayButton()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);　//最大接続人数とリージョンIDを記載する　リージョンIDの記載がない場合は最も近いサーバーが選ばれる？(今回の場合はbest region is asia-northeast1と出力された)
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); //ここでコード取得のための変数を定義

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );　　//アロケーション情報の取得（リレーサーバーの情報）

            Debug.Log(joinCode);
            JoinCodeText.text = "Join Code: " + joinCode;
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    //クライアント側
    //コードはデバッグとして目視で確認
    public void JoinRelayButton()
    {
        JoinRelay(joinCodeInput.text);
    }

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
                );　　//入力されたJoin CodeのRelayサーバー情報、アロケーション情報を取得

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}