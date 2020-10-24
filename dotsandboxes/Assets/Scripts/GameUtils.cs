using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public static void Convert2DToArrayIndex(int height, int x, int y, out int index) {
        index = x * height + y + 1;
    }
}
