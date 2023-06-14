using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamListsManager : MonoBehaviour
{
    public static TeamListsManager Instance { get; private set; }

    public GameObject[] Teams;
}
