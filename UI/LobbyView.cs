using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyView : View
{
    [SerializeField]
    private TMP_Text playerList;

    [SerializeField]
    private TMP_Text TeamMemberList;

    [SerializeField]
    private Button readyBtn;

    [SerializeField]
    private TMP_Text readyBtnText;

    [SerializeField]
    private Button startBtn;

    [SerializeField]
    private Button changeTeamBtn;

    [SerializeField]
    private TMP_InputField changingTeamArea;

    [SerializeField]
    private TMP_InputField changingPlayerUsername;

    public override void Initialize()
    {

        readyBtn.onClick.AddListener(() =>
        {
            Player.Instance.IsReady = !Player.Instance.IsReady;
            GameManager.Instance.checkProperStart();
        });
        //her teamlistte oyuncu varsa baþlat && checkProperStart fonksiyonu ile ekle.
        if (VariableSend.Instance.Auth == "Mod" )
        {
            startBtn.onClick.AddListener(() => {
                if (GameManager.Instance.SetStart)
                {
                    for (int i = 0; i < GameManager.Instance.Players.Count; i++)
                    {

                        if (GameManager.Instance.Players[i].authType != "Mod")
                        {
                            GameManager.Instance.Players[i].StartGame();

                            GameManager.Instance.Players[i].PlayerBegin((GameManager.Instance.Players[i].TeamNo - 1) == 0);
                        }
                        else
                        {
                            ViewManager.Instance.Show<MainView>();
                        }
                    }
                }
            });

            changeTeamBtn.onClick.AddListener(() =>
            {
                Debug.Log("INPUT AREA TEAM NO -- " + changingTeamArea.text + "--PlAYER Name--" + changingPlayerUsername.text);
                for (int i = 0; i < GameManager.Instance.Players.Count; i++)
                {
                    if (GameManager.Instance.Players[i].username == changingPlayerUsername.text)
                        GameManager.Instance.Players[i].changeTeam(changingTeamArea.text);

                    GameManager.Instance.checkProperStart();
                }
                
            });
        }
        else
        {
            startBtn.gameObject.SetActive(false);
            changeTeamBtn.gameObject.SetActive(false);
            changingTeamArea.gameObject.SetActive(false);
            changingPlayerUsername.gameObject.SetActive(false);

        }
   
        base.Initialize();

    }
    private void Update()
    {
        if (!IsInitialized) return;

        string PlayerListText = "";

        string TeamListText = "";

        int playerCount = GameManager.Instance.Players.Count;

        for(int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            Player player = GameManager.Instance.Players[i];

            PlayerListText += $"{player.username} (IsReady:{player.IsReady})(TeamNo:{player.TeamNo})(CanStart:{GameManager.Instance.SetStart})";

            if (i < playerCount - 1) PlayerListText += "\r\n";

        }
        playerList.text = PlayerListText;
        if(Player.Instance.authType != "Mod")
        {
            for (int i = 0; i < GameManager.Instance.Teams.Length; i++)
            {
                TeamScript t = GameManager.Instance.Teams[i].GetComponent<TeamScript>();
                if (t.TeamPlayers == null) continue;
                foreach (Player p in t.TeamPlayers)
                {
                    TeamListText += $"{p.username} (TeamNo:{p.TeamNo})(Team Title:{t.TitleText})";
                    if (i < GameManager.Instance.Teams.Length - 1) TeamListText += "\r\n";
                }

            }
        }
        
        TeamMemberList.text = TeamListText;

        readyBtnText.color = Player.Instance.IsReady ? Color.green : Color.red;

        startBtn.interactable = GameManager.Instance.SetStart;

    }
  
}
