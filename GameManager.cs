using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    public TMP_Text TurnText;

    [field: SyncObject]
    public SyncList<Player> Players { get; } = new SyncList<Player>();

    [field: SyncObject]
    public SyncList<Pawn> Pawns { get; } = new SyncList<Pawn>();

    [field: SerializeField]
    [field: SyncVar]
    public bool SetStart { get; set; }

    [field: SerializeField]
    [field: SyncVar]
    public bool DidStart { get; private set; }

    [field: SerializeField]
    [field: SyncVar(Channel = Channel.Unreliable, OnChange = nameof(onTurnChange))]
    public int Turn { get; set; }

    [SerializeField]
    public Transform PlayerListContent;

    [SerializeField]
    public GameObject TeamlistsPrefab;

    [SerializeField]
    public Transform teamlistsTransform;

    [SerializeField]
    public Transform pawnlistsTransform;

    [SerializeField]
    public Transform BoardTransform;

    [SerializeField]
    public GameObject PlayerEntryPrefab;

    [SerializeField]
    public GameObject PawnEntryPrefab;

    public GameObject TeamLists;

    [SerializeField]
    public List<GameObject> PlayerEntries { get; } = new List<GameObject>();

    [SerializeField]
    public GameObject[] Teams;

    [SerializeField]
    public GameObject[] SpawnPoints;

    [SerializeField]
    public GameObject TilePrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        generateMap();
    }
    private void Update()
    {
        if (!IsServer) return;
        checkProperStart();
    }

    [Server]
    public void StopGame()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].StopGame();
        }
        DidStart = false;
    }
    private void onTurnChange(int prev, int next, bool asServer)
    {
        TurnText.text = next.ToString();
        foreach (Pawn pawn in Pawns)
        {
            foreach (Player player in pawn.controllingPlayers)
            {
                player.PlayerBegin((player.TeamNo - 1) == next);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void checkProperStart()
    {
        int differentTeam = 0;

        int playerCount = 0;

        if (Players == null) SetStart =  false;

        foreach (Player p in Players)
        {
            if (p.authType != "Mod") playerCount++;
        }
        if (playerCount == 0) SetStart = false;

        if (playerCount < Teams.Length)
        {
            for (int i = 0; i < Teams.Length; i++)
            {
                TeamScript t = Teams[i].GetComponent<TeamScript>();
                if (t.TeamPlayers.Count == 0) continue;
                
                differentTeam++;
            }
            if (differentTeam == playerCount && Players.All(player => player.IsReady))
            {
                SetStart = true;
            }
            else
            {
                SetStart = false;
            }
           
        }
        else
        {//detaylandýrýlanacak.
            for (int i = 0; i < Teams.Length; i++)
            {
                TeamScript t = Teams[i].GetComponent<TeamScript>();

                if (t.TeamPlayers.Count == 0) continue;

                differentTeam++;
            }
            if (differentTeam == Teams.Length && Players.All(player => player.IsReady)) SetStart = true;

            else SetStart = false;
        }
    }
    public void generateMap()
    {
        List<Vector2> coordinates = new List<Vector2>();
        int x = 0;
        int z = 0;
        for (int i = 0; i < 5; i++)
        {
            Vector2 vc;
            
            switch(i)
            {
                case 0 : x = -10; break;
                case 1:  x = -5;  break;
                case 2:  x =  0;  break;
                case 3:  x =  5;  break;
                case 4:  x =  10; break;
            }
            for (int j = 0; j < 5; j++)
            {
                switch(j)
                {
                    case 0: z = -10; break;
                    case 1: z = -5; break;
                    case 2: z = 0; break;
                    case 3: z = 5; break;
                    case 4: z = 10; break;
                }
                vc = new Vector2(x, z);
                coordinates.Add(vc);
                z = 0;
            }
            x = 0;
           
        }
        int k = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Tile tile = Instantiate(TilePrefab, new Vector3(coordinates[k].x,0, coordinates[k].y), Quaternion.Euler(new Vector3())).GetComponent<Tile>();
                tile.GridPos = new Vector2(i, j);
                tile.name = "Tile_" + i + "_" + j;
                tile.transform.SetParent(BoardTransform);
                Board.Instance.Tiles.Add(tile);
                k++;
            }
        }
    }
}
