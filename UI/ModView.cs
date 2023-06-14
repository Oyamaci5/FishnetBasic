using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModView : View
{
    [SerializeField]
    private Button connectBtn;

    [SerializeField]
    private TMP_InputField username;

    [SerializeField]
    private TMP_InputField password;

    public override void Initialize()
    {

        connectBtn.onClick.AddListener(() =>
        {
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
            VariableSend.Instance.username = username.text;
            VariableSend.Instance.TeamNo = 1;
            VariableSend.Instance.Auth = "Mod";
        });

        base.Initialize();
    }
}
