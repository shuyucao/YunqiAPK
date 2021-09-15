using UnityEngine;
using System.Collections;

public class Home : MonoBehaviour {

    static public int levelIndex = 1;
	// Use this for initialization
	void Start () {
        StartCoroutine(StartLoading());
	}

    IEnumerator StartLoading()
    {
        AsyncOperation op = Application.LoadLevelAsync(levelIndex);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }
}
