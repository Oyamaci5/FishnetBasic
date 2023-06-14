using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Instance { get; private set; }
    [SyncVar]
    public string username;

    [SerializeField]
    [field: SyncVar]
    public bool IsReady { get; [ServerRpc(RequireOwnership =false)] set; }

    [SyncVar]
    public Pawn controlledPawn;

    [SerializeField]
    private Pawn pawnPrefab;

    [SyncVar]
    public int TeamNo;

    [SyncVar]
    public string authType;

    [SyncVar]
    public GameObject controlledListEntry;

    public override void OnStartServer()
    {
        base.OnStartServer();

        print("Biri katýldý.");

        GameManager.Instance.Players.Add(this);

        username = VariableSend.Instance.username;

        TeamNo = VariableSend.Instance.TeamNo;

        authType = VariableSend.Instance.Auth;

    }
    public override void OnStartClient()
    {

        base.OnStartClient();

        if (!IsOwner) return;

        Instance = this;

        ViewManager.Instance.Initialize();

        Debug.Log("Start Client" + VariableSend.Instance.username + "-- " + VariableSend.Instance.TeamNo);

        SetPlayerVariable(VariableSend.Instance.username, VariableSend.Instance.TeamNo, VariableSend.Instance.Auth);

        if (VariableSend.Instance.Auth != "Mod")
            SpawnObject(this);

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerVariable(string name, int Team_No, string authType)
    {
        Debug.Log("setplayervariable");
        GameManager.Instance.Players[GameManager.Instance.Players.IndexOf(this)].username = name;
        GameManager.Instance.Players[GameManager.Instance.Players.IndexOf(this)].TeamNo = Team_No;
        GameManager.Instance.Players[GameManager.Instance.Players.IndexOf(this)].authType = authType;
    }

    [ServerRpc(RequireOwnership = false)]
    public void changeTeam(string text)
    {
        GameManager.Instance.Teams[(TeamNo - 1)].GetComponent<TeamScript>().TeamPlayers.Remove(this);

        TeamNo = int.Parse(text);

        GameManager.Instance.Teams[(TeamNo - 1)].GetComponent<TeamScript>().TeamPlayers.Add(this);

        SetPlayerVariable(username, TeamNo, authType);

        CreateNewPlayerPanel(controlledListEntry, int.Parse(text));
    }
    [ObserversRpc(BufferLast = true)]
    public void CreateNewPlayerPanel(GameObject spawned, int teamNo)
    {

        spawned.transform.SetParent(GameManager.Instance.TeamLists.GetComponent<TeamListsManager>().Teams[teamNo].GetComponent<TeamScript>().Content.transform, true);

        spawned.GetComponent<PlayerListEntry>().SetTeam(teamNo);
    }

    [ServerRpc]
    public void SpawnObject(Player script)
    {
        if (script == null ) return;

        if (controlledListEntry != null) controlledListEntry = null;

        Debug.Log("SpawnObject--" + script.username + "--" + script.TeamNo);

        GameObject playerlistEntry = Instantiate(GameManager.Instance.PlayerEntryPrefab, GameManager.Instance.TeamLists.GetComponent<TeamListsManager>().Teams[script.TeamNo].GetComponent<TeamScript>().Content.transform);
        //bakýlacak
        playerlistEntry.GetComponent<PlayerListEntry>().controllingPlayer = this;

        playerlistEntry.GetComponent<PlayerListEntry>().SetUsername(script.username);

        playerlistEntry.GetComponent<PlayerListEntry>().SetTeam(script.TeamNo);

        Spawn(playerlistEntry);

        controlledListEntry = playerlistEntry;

        GameManager.Instance.PlayerEntries.Add(playerlistEntry);
        
        if(authType != "Mod")
            GameManager.Instance.Teams[(TeamNo - 1)].GetComponent<TeamScript>().TeamPlayers.Add(this);

        SetPlayerListEntry(playerlistEntry, script.username, script.TeamNo);

        
    }
    [ObserversRpc(BufferLast = true)]
    private void SetPlayerListEntry(GameObject spawned, string name, int variable)
    {
        Debug.Log(name + "--" + variable);

        spawned.transform.SetParent(GameManager.Instance.TeamLists.GetComponent<TeamListsManager>().Teams[variable].GetComponent<TeamScript>().Content.transform, true);

        spawned.GetComponent<PlayerListEntry>().SetUsername(name);

        spawned.GetComponent<PlayerListEntry>().SetTeam(variable);

    }

    [ObserversRpc(BufferLast = true)]
    private void SetCanvasVariable(string newstring, int teamindex, int pawnindex, int score)
    {
        Pawn.Instance.username =  Pawn.Instance.usernametext.text = newstring;
        Pawn.Instance.Team = teamindex;
        Pawn.Instance.pawnIndex = pawnindex;
        Pawn.Instance.score = score;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGame()
    {
        if (checkPawn(TeamNo))
        {
            Transform spawnPoint = GameManager.Instance.SpawnPoints[TeamNo - 1].transform;

            Pawn pawnInstance = Instantiate(pawnPrefab, spawnPoint.position, Quaternion.identity);

            GameManager.Instance.Pawns.Add(pawnInstance);

            Spawn(pawnInstance.gameObject, Owner);

            controlledPawn = pawnInstance;

            controlledPawn.controllingPlayers.Add(this);

            SetCanvasVariable("Player" + TeamNo, TeamNo, GameManager.Instance.Pawns.IndexOf(controlledPawn),0);

            Pawn.Instance.username = Pawn.Instance.usernametext.text = "Player" + TeamNo;

            Pawn.Instance.Team = TeamNo;

            Pawn.Instance.pawnIndex = GameManager.Instance.Pawns.IndexOf(controlledPawn);

            Pawn.Instance.score = 0;

            CreatePawnPanel(this, Pawn.Instance.username, Pawn.Instance.score);
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.Pawns.Count; i++)
            {
                if (GameManager.Instance.Pawns[i].Team == TeamNo)
                {
                    GameManager.Instance.Pawns[i].controllingPlayers.Add(this);

                    controlledPawn = GameManager.Instance.Pawns[i];
                }
            }
        }
    }

    public void CreatePawnPanel(Player p ,string username, int score)
    {
        GameObject pawnlistentry = Instantiate(GameManager.Instance.PawnEntryPrefab, GameManager.Instance.pawnlistsTransform);

        pawnlistentry.GetComponent<PawnPanel>().controllingPawn = p.controlledPawn;

        pawnlistentry.GetComponent<PawnPanel>().SetUsername(username);

        pawnlistentry.GetComponent<PawnPanel>().ScoreTextArea.text = score.ToString();

        pawnlistentry.GetComponent<PawnPanel>().controllingPawn.controlledListPawnEntry = pawnlistentry.GetComponent<PawnPanel>();

        Spawn(pawnlistentry);

        SetPawnListEntry(pawnlistentry, username, score);
    }
    [ObserversRpc(BufferLast = true)]
    private void SetPawnListEntry(GameObject spawned, string name, int variable)
    {
        Debug.Log(name + "--" + variable);

        spawned.transform.SetParent(GameManager.Instance.pawnlistsTransform, true);

        spawned.GetComponent<PawnPanel>().SetUsername(name);

        spawned.GetComponent<PawnPanel>().ScoreTextArea.text = variable.ToString();

    }
    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.Instance.Players.Remove(this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        Debug.Log("OnstopClient" + username);

        GameManager.Instance.PlayerEntries.Remove(controlledListEntry);

        GameManager.Instance.Teams[(TeamNo - 1)].GetComponent<TeamScript>().TeamPlayers.Remove(this);

        if (controlledListEntry != null) Despawn(controlledListEntry);

    }
    [Server]
    public void StopGame()
    {
        if (controlledPawn != null) controlledPawn.Despawn();
       
    }
    [ServerRpc(RequireOwnership =false)]
    public void PlayerBegin(bool canPlaypawn)
    {
        TargetBegin(Owner, canPlaypawn);
    }

    [TargetRpc]
    public void TargetBegin(NetworkConnection connection,bool canPlay)
    {
        
        if (canPlay)
        {
            ViewManager.Instance.Show<MainView>();
        }
        else
        {
            ViewManager.Instance.Show<WaitView>();
        }
        
       
    }
    public bool checkPawn(int team)
    {
        foreach (Pawn pawn in GameManager.Instance.Pawns)
        {
            if (pawn.Team == team)
                return false;
        }
        return true;
    }
}
