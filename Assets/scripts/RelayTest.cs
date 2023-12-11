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

    //�z�X�g��
    public async void CreateRelayButton()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);�@//�ő�ڑ��l���ƃ��[�W����ID���L�ڂ���@���[�W����ID�̋L�ڂ��Ȃ��ꍇ�͍ł��߂��T�[�o�[���I�΂��H(����̏ꍇ��best region is asia-northeast1�Əo�͂��ꂽ)
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); //�����ŃR�[�h�擾�̂��߂̕ϐ����`

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );�@�@//�A���P�[�V�������̎擾�i�����[�T�[�o�[�̏��j

            Debug.Log(joinCode);
            JoinCodeText.text = "Join Code: " + joinCode;
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    //�N���C�A���g��
    //�R�[�h�̓f�o�b�O�Ƃ��Ėڎ��Ŋm�F
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
                );�@�@//���͂��ꂽJoin Code��Relay�T�[�o�[���A�A���P�[�V���������擾

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}