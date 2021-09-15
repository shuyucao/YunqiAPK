using UnityEngine;
using System.Collections;

public class Launcher : MonoBehaviour
{

    public const int ACTION_LAUNCHER = 0;
    public const int ACTION_VEDIO_PLAYER = 1;
    public const int ACTION_VEDIO_LIST = 2;
    public const int ACTION_VEDIO_360PLAYER = 3;
    public const int ACTION_VEDIO_3603DPLAYER = 12;
    public const int ACTION_GAMES = 4;
    public const int ACTION_MUSIC_LIST = 5;
    public const int ACTION_MUSIC_PLAYER = 6;
    public const int ACTION_PERSONAL_CENTER = 7;
    public const int ACTION_PIC_LIST = 8;
    public const int ACTION_PIC_PLAYER = 9;
    public const int ACTION_PIC_360PLAYER = 10;
    public const int ACTION_APPS = 11;
    public const int ACTION_SETTINGS = 90;


    const string SCENE_LAUNCHER_NAME = "Launcher";
    const string SCENE_VEDIO_PLAYER_NAME = "VideoPlayer";
    const string SCENE_VIDEO_LIST_NAME = "Movies";
    const string SCENE_VIDEO_360PLAYER_NAME = "Video360Player";
    const string SCENE_VIDEO_3603DPLAYER_NAME = "Video3603DPlayer";
    const string SCENE_GAMES_NAME = "Games";
    const string SCENE_MUSIC_LIST_NAME = "MusicList";
    const string SCENE_MUSIC_PLAYER_NAME = "MusicPlayer";
    const string SCENE_PERSONAL_CENTER_NAME = "PersonalCenter";
    const string SCENE_PIC_LIST_NAME = "PictureList";
    const string SCENE_PIC_PLAYER_NAME = "PicturePlayer";
    const string SCENE_PIC_360PLAYER_NAME = "Picture360Player";
    const string SCENE_APPS_NAME = "AppList";
    const string SCENE_SETTINGS = "Settings";
    const string SCENE_HOME = "Home";


    public UISprite mSprite;
    public UILabel mLable;

    // Use this for initialization
    void Start()
    {
        int action = ACTION_LAUNCHER;
        string sceneName = GetSceneName(action);

        if (sceneName == SCENE_HOME)
        {
            //WingScreenManager.FirstScreen = GetScreenName(action);
        }
        StartCoroutine("StartLoading", sceneName);
    }

    string GetSceneName(int action)
    {
        string levelName = SCENE_LAUNCHER_NAME;
        switch (action)
        {
            case ACTION_LAUNCHER:
                levelName = SCENE_HOME;
                break;
            case ACTION_VEDIO_PLAYER:
                levelName = SCENE_VEDIO_PLAYER_NAME;
                break;
            case ACTION_VEDIO_LIST:
                levelName = SCENE_HOME;
                break;
            case ACTION_VEDIO_360PLAYER:
                levelName = SCENE_VIDEO_360PLAYER_NAME;
                break;
            case ACTION_VEDIO_3603DPLAYER:
                levelName = SCENE_VIDEO_3603DPLAYER_NAME;
                break;
            case ACTION_GAMES:
                levelName = SCENE_GAMES_NAME;
                break;
            case ACTION_MUSIC_LIST:
                levelName = SCENE_MUSIC_LIST_NAME;
                break;
            case ACTION_MUSIC_PLAYER:
                levelName = SCENE_MUSIC_PLAYER_NAME;
                break;
            case ACTION_PERSONAL_CENTER:
                levelName = SCENE_PERSONAL_CENTER_NAME;
                break;
            case ACTION_PIC_LIST:
                levelName = SCENE_PIC_LIST_NAME;
                break;
            case ACTION_PIC_PLAYER:
                levelName = SCENE_PIC_PLAYER_NAME;
                break;
            case ACTION_PIC_360PLAYER:
                levelName = SCENE_PIC_360PLAYER_NAME;
                break;
            case ACTION_APPS:
                levelName = SCENE_APPS_NAME;
                break;
            case ACTION_SETTINGS:
                levelName = SCENE_HOME;
                break;
            default:
                break;
        }

        Debug.Log("Action is :" + action.ToString() + " levelName is :" + levelName);
        return levelName;
    }

    //EWingScreen GetScreenName(int action)
    //{
    //    EWingScreen screen = EWingScreen.launcher;
    //    switch (action)
    //    {
    //        case ACTION_LAUNCHER:
    //            screen = EWingScreen.launcher;
    //            break;
    //        case ACTION_VEDIO_LIST:
    //            screen = EWingScreen.movie;
    //            break;
    //        case ACTION_GAMES:
    //            screen = EWingScreen.game;
    //            break;
    //        case ACTION_SETTINGS:
    //            screen = EWingScreen.user;
    //            break;
    //    }
    //    return screen;
    //}


    IEnumerator StartLoading(string sceneName)
    {
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = Application.LoadLevelAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                //uiSprite.fillAmount=displayProgress/100.0f;
                mLable.text = "1234";
                Debug.Log(mLable.text);
                //uiSlider.value = displayProgress / 100.0f;

                yield return new WaitForEndOfFrame();
            }
        }


        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            //uiSlider.value=displayProgress/100.0f;
            mSprite.fillAmount = displayProgress / 100.0f;
            mLable.text = displayProgress.ToString();
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }
}
