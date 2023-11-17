using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int Tile { get; set; }
    public int Next { get; set; }

    public Move() { }
    public Move(int tile, int next)
    {
        this.Tile = tile;
        this.Next = next;
    }
}
