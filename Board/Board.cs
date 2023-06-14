using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Board : NetworkBehaviour
{
    public static Board Instance { get; private set; }
   /* [field: SerializeField]
    public Tile[] Tiles { get; private set; }*/

    [field: SerializeField]
    public List<Tile> Tiles { get; } = new List<Tile>();
    private void Awake()
    {
        Instance = this;
    }
    public int Wrap(int index)
    {
        return index < 0 ? Math.Abs((Tiles.Count - Math.Abs(index)) % Tiles.Count) : index % Tiles.Count;
    }

    public Tile[] Slice(int start, int end)
    {
        if (Tiles.Count == 0) return Array.Empty<Tile>();

        List<Tile> slice = new();

        int steps = Math.Abs(end - start);

        if(end > start)
        {
            for(int i = start; i<= start + steps; i++)
            {
                slice.Add(Tiles[Wrap(i)]);
            }
        }
        else
        {
            for (int i = start; i >= start - steps; i--)
            {
                slice.Add(Tiles[Wrap(i)]);
            }
        }
        return slice.ToArray();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ServerSetTileOwner(int tileIndex, Player value)
    {
        ObserverSetTileOwner(tileIndex, value);
    }
    [ObserversRpc(BufferLast = true)]
    private void ObserverSetTileOwner(int tileIndex, Player value)
    {
        Tiles[tileIndex].owningPlayer = value; 
    }

}
