using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MultiPlayerMenuView : View
{
    [SerializeField]
    private Button hostBtn;

    [SerializeField]
    private Button connectBtn;

    [SerializeField]
    private Button exitBtn;

    [SerializeField]
    private TMP_InputField username;

    [SerializeField]
    private TMP_InputField TeamNoArea;

    [SerializeField]
    private TMP_InputField Auth;
    public override void Initialize()
    {
        hostBtn.onClick.AddListener(() => 
        {

            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
            VariableSend.Instance.username = username.text;
            VariableSend.Instance.TeamNo = int.Parse(TeamNoArea.text);
            VariableSend.Instance.Auth = Auth.text;
        } );

        connectBtn.onClick.AddListener(() => 
        {

            InstanceFinder.ClientManager.StartConnection();
            VariableSend.Instance.username = username.text;
            VariableSend.Instance.TeamNo = int.Parse(TeamNoArea.text);
            VariableSend.Instance.Auth = Auth.text;
        });

        exitBtn.onClick.AddListener(Application.Quit);
     
        base.Initialize();
    }
}
