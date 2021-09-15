using UnityEngine;
using System.Collections;
using System;


public enum WingScene
{
    Home = 0,
    Player = 1,
}

public class WingSceneManager : Singleton<WingSceneManager> {

    public WingScene CurrentScene = WingScene.Home;
    private string[] transitionNames = new string[] { "Fade" };
    private string[] transitionPrefabs = new string[] { "Transitions/WingFadeTransition" };
    //private string defaultTransitionName = "Fade";

    //load transition prefab
    private void DoTransition(string transitionName)
    {
        if (string.IsNullOrEmpty(transitionName))
        {
            Debug.LogWarning("given transition name is null or empty.");
            transitionName = "Fade";
            return;
        }
        int index = Array.IndexOf(transitionNames, transitionName);
        string transitionPrefab = null;
        if (index != -1) transitionPrefab = transitionPrefabs[index];
        GameObject prefab = (GameObject)Resources.Load(transitionPrefab);
        if (prefab == null)
        {
            throw new ArgumentException("no transition prefab found at path " + transitionPrefab);
        }

        GameObject instance = (GameObject)GameObject.Instantiate(prefab);
        WingTransition transition = instance.GetComponent<WingTransition>();
        if (transition == null)
        {
            throw new ArgumentException("no transition found at prefab " + transitionPrefab);
        }
    }
}


