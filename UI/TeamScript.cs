using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;

public class TeamScript : NetworkBehaviour
{

    [SerializeField]
    [field: SyncObject]
    public SyncList<Player> TeamPlayers { get; } = new SyncList<Player>();

    public string TitleText;
    public TMP_Text tTitle;
    public TMP_Text tList;
    public Color32 tTitleColor = new Color32(1, 0, 0, 1);

    public GameObject Content;
    // Start is called before the first frame update
    void Awake()
    {
        tTitle.text = TitleText;
        tTitle.color = tTitleColor;
    }
}
