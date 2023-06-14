using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntry : NetworkBehaviour
{
    public static PlayerListEntry Instance { get; private set; }

    [SerializeField]
    public TMP_Text OyuncuAdi;

    [SerializeField]
    public TMP_Text TeamText;

    [SerializeField]
    public Player controllingPlayer;

    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnColorChange))]
    private Color _color;

    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnScoreChange))]
    private int _score;

    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnUsernameChange))]
    private string _username;

    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnTeamChange))]
    public int TeamNo;

    private void Awake()
    {
        _color = OyuncuAdi.color;
    }
    private void OnColorChange(Color prev, Color next, bool asServer)
    {
        OyuncuAdi.color = next;
        TeamText.color = next;
    }

    private void OnScoreChange(int prev, int next, bool asServer)
    {
        TeamText.text = next.ToString();
    }
    private void OnUsernameChange(string prev, string next, bool asServer)
    {
        OyuncuAdi.text = next;
    }

    private void OnTeamChange(int prev, int next, bool asServer)
    {
        TeamText.text = next.ToString();
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetColor(Color color)
    {
        _color = color;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetScore(int score)
    {
        _score = score;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetUsername(string username)
    {
        _username = username;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTeam(int teamNo)
    {
        TeamNo = teamNo;
    }
}
