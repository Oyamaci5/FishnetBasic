using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainView : View
{
    public static MainView Instance { get; private set; }

    [SerializeField]
    private Button purchaseTileBtn;

    [SerializeField]
    private Button moveForwardBtn;

    [SerializeField]
    private Button moveBackwardBtn;

    [SerializeField]
    private TMP_Text PlayerScoreList;
    public override void Initialize()
    {
        purchaseTileBtn.onClick.AddListener(() =>
        {
            int pawnPosition = Player.Instance.controlledPawn.currentPosition;
            
            if(Board.Instance.Tiles[pawnPosition].owningPlayer == null)
            {
                Board.Instance.ServerSetTileOwner(pawnPosition, Player.Instance);

                foreach(Pawn p in GameManager.Instance.Pawns)
                {
                    if ((p.Team - 1) == GameManager.Instance.Turn)
                        p.AddScore(p.score + 1);
                }
            }
        });

        /*moveForwardBtn.onClick.AddListener(() => Player.Instance.controlledPawn.ServerMove(1));

        moveBackwardBtn.onClick.AddListener(() => Player.Instance.controlledPawn.ServerMove(-1));*/

        base.Initialize();
    }


    private void Awake()
    {      
        Instance = this;
    }
    private void Update()
    {
        if (!IsInitialized) return;

        string PlayerListText = "";

        int pawnsCount = GameManager.Instance.Pawns.Count;

        for (int i = 0; i < GameManager.Instance.Pawns.Count; i++)
        {
            Pawn pawn = GameManager.Instance.Pawns[i];
  
            PlayerListText += $"{pawn.username} (Score:{pawn.score})(Team:{pawn.Team})";

            if (i < pawnsCount - 1) PlayerListText += "\r\n";

        }
        PlayerScoreList.text = PlayerListText;

    }
   
}
