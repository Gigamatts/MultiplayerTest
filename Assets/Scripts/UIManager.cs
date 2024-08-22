using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;


public class UIManager : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clientsCounter;

    [SerializeField]
    private TextMeshProUGUI scoreCounter;
    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public static UIManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        clientsCounter.text = playersNum.Value.ToString();
        if (!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    [ServerRpc]
    public void UpdateScoreServerRpc(int score)
    {
        UIManager.instance.scoreCounter.text = score.ToString();
    }
}
