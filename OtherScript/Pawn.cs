using FishNet.Object;
using FishNet.Object.Synchronizing;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using TMPro;
using FishNet.Connection;
using FishNet.Transporting;

public sealed class Pawn : NetworkBehaviour
{
    public static Pawn Instance { get; private set; }

    [field: SyncObject]
    public SyncList<Player> controllingPlayers { get; } = new SyncList<Player>();

    [SyncVar]
    public int currentPosition;

    private bool isMoving;

    [field: SerializeField]
    [field: SyncVar(Channel = Channel.Unreliable, OnChange = nameof(OnScoreChange))]
    public int score { get; set; }

    [SyncVar]
    public string username;

    [SyncVar]
    public int Team;

    [SyncVar]
    public int pawnIndex;

    [SerializeField]
    public TMP_Text usernametext;

    [SerializeField]
    public PawnPanel controlledListPawnEntry;

    private void Awake()
    {
        Instance = this;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ServerMove(int steps)
    {
        if (isMoving) return;
        
        isMoving = true;

        Tile[] tiles = Board.Instance.Slice(currentPosition, currentPosition + steps);

        int controllingPlayerIndex = Team - 1;

        Vector3[] path = tiles.Select(tile => tile.PawnPosition.position).ToArray();

        Tween tween = transform.DOPath(path, 2.0f);
        
        tween.OnComplete(() => 
        {
            isMoving = false;

            Debug.Log("CurrentPosition--" + currentPosition);

            currentPosition += steps;

            ObserverSetCurrentPosition(currentPosition);

            GameManager.Instance.Turn = (GameManager.Instance.Turn + 1) % GameManager.Instance.Pawns.Count;
        });
        tween.Play();

    }
    [ObserversRpc(BufferLast = true)]
    public void ObserverSetCurrentPosition(int value)
    {
        currentPosition = value;
    }
    private void OnScoreChange(int prev, int next, bool asServer)
    {
        controlledListPawnEntry._score += 1; 
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddScore(int v)
    {
        score = v;
    }
}
