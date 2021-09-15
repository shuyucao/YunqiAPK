using UnityEngine;
using System.Collections;

public class Lanuch : MonoBehaviour
{
    void Start()
    {
        Debug.Log("unity launch !!!!!!!!!!!!!!!!!!!!!");
        Ftimer.AddEvent("load home page", 0.5f, () =>
        {
            Application.LoadLevel(1);
        });
    }

    public void SetStartType(string type)
    {
        Debug.Log("unity launch   SetStartType!!!!!!!!!!!!!!!!!!!!! " + type);
        Main.StartType = type;
    }

    public void SetStartValue(string value)
    {
        Debug.Log("unity launch   SetStartValue!!!!!!!!!!!!!!!!!!!!! "+ value);
        Main.StartValue = value;
    }
}
