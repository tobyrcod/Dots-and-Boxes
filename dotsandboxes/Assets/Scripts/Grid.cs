using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private float cellSize;
    private Vector3 origin;

    public Grid(float cellSize, Vector2 origin) {
        this.cellSize = cellSize;
        this.origin = origin;
    }

    public Vector2 GetWorldPosition(int x, int y) {
        return origin + new Vector3(x, y) * cellSize;
    }
}
