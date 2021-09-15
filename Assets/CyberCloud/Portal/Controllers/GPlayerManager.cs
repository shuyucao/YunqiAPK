using UnityEngine;

public class GPlayerManager : Singleton<GPlayerManager>
{
    //for online photo
    public void Play(string mid)
    {
        Bundle bundle = new Bundle();
        CachePhotoData.Instance.CurrentPhotoIndex = mid;
        PhotoModel data = CachePhotoData.Instance.GetCurrentPhotoModel();
        bundle.SetValue<PlayerType>("playertype", PlayerType.OnLineList);
        bundle.SetValue<PhotoModelBase>("data", data == null ? null : data as PhotoModelBase);
        ScreenManager.Instance.ChangeScreen(UIScreen.Player, bundle);
    }

    public void Play(PhotoModel data, PlayerType type)
    {
        Bundle bundle = new Bundle();
        CachePhotoData.Instance.CurrentPhotoIndex = data.MID;
        bundle.SetValue<PlayerType>("playertype", type);
        bundle.SetValue<PhotoModelBase>("data", data == null ? null : data as PhotoModelBase);
        ScreenManager.Instance.ChangeScreen(UIScreen.Player, bundle);
    }

    //play tuijian
    public void PlayRecommend(PhotoModel data)
    {
        if (data == null)
        {
            Debug.LogError("the data is null");
            return;
        }
        CachePhotoData.Instance.CurrentPhotoIndex = data.MID;
        Bundle bundle = new Bundle();
        bundle.SetValue<PlayerType>("playertype", PlayerType.Recommend);
        bundle.SetValue<PhotoModelBase>("data", data as PhotoModelBase);
        ScreenManager.Instance.ChangeScreen(UIScreen.Player, bundle);
    }

    //Open Dimonsion Door
    public void OpenDimonsionDoor()
    {
        PhotoModel data = CachePhotoData.Instance.GetADimensionDoorPhoto();
        if (data != null)
        {
            Bundle bundle = new Bundle();
            bundle.SetValue<PhotoModel>("data", data);
            ScreenManager.Instance.ChangeScreen(UIScreen.DimensionDoor, bundle);
        }
        else
        {
            //if current data is null then request again
            DataLoader.Instance.RequestDimensionDoor();
        }
    }

    //for local photo
    public void Play(LocalPhotoModel data)
    {
        Bundle bundle = null;
        bundle = new Bundle();
        bundle.SetValue<PlayerType>("playertype", PlayerType.Local);
        bundle.SetValue<PhotoModelBase>("data", data);
        ScreenManager.Instance.ChangeScreen(UIScreen.Player, bundle);
    }
}

public enum PlayerType
{
    OnLineList,         //online list
    Local,              //local image
    Recommend,          //Recommend one image
    OnLineOneImg        //online one image
}