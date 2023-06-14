using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VariableSend : MonoBehaviour
{
    public static VariableSend Instance { get; private set; }
    
    [SerializeField]
    public string username;

    [SerializeField]
    public int TeamNo;

    [SerializeField]
    public string Auth;
    private void Awake()
    {
        Auth = "Player";
        Instance = this;
    }
}
