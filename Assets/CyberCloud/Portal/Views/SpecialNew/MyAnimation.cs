using UnityEngine;
using System.Collections;

public class MyAnimation : MonoBehaviour {
    private static bool isNeed = true;
    public void set(bool b)
    {
        isNeed = b;
    }

    public bool get()
    {
        return isNeed;
    }
}
