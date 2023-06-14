using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class PawnPanel : NetworkBehaviour
{
    public static PawnPanel Instance { get; private set; }

    [SerializeField]
    public TMP_Text PawnUsernameTextArea;

    [SerializeField]
    public TMP_Text ScoreTextArea;

    [SerializeField]
    public Pawn controllingPawn;

    [SyncVar]
    public Color _color;

    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnUsernameChange))]
    private string _username;

    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnScoreChange))]
    public int _score;

    private void OnScoreChange(int prev, int next, bool asServer)
    {
        ScoreTextArea.text = next.ToString();
    }
    private void OnUsernameChange(string prev, string next, bool asServer)
    {
        PawnUsernameTextArea.text = next;
    }

    /*[ServerRpc(RequireOwnership = false)]
    public void SetScore(int score)
    {
        _score = score;
    }*/

    [ServerRpc(RequireOwnership = false)]
    public void SetUsername(string username)
    {
        _username = username;
    }
}
