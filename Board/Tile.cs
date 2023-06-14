using UnityEngine;

public class Tile : MonoBehaviour
{
    [field: SerializeField]
    public Transform PawnPosition { get; private set; }

    public Player owningPlayer;

    public Vector2 GridPos;

    [SerializeField]
    private MeshRenderer meshRenderer;

    private Color defaultColor;

    private void Start()
    {
        defaultColor = meshRenderer.sharedMaterial.color;   
    }
    private void LateUpdate()
    {
        if (owningPlayer == null)
        {
            meshRenderer.material.color = defaultColor;

            return;
        }

        meshRenderer.material.color = GameManager.Instance.Players.IndexOf(owningPlayer) switch
        {
            0 => Color.red,
            1 => Color.green,
            2 => Color.blue,
            3 => Color.yellow,

            _ => Color.yellow,

        };
    }
    private void OnMouseDown()
    {
        Player.Instance.controlledPawn.ServerMove(1);
    }
}
