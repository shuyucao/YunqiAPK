using System.Collections;
using UnityEngine;

public class DimensionDoorScreen : ScreenBase
{
    //when this screen move to the top
    public override void OprateChangeScreen(Bundle bundle)
    {
        UnityTools.SetCameraBlack(true);
        GalleryTools.ShowLeftBar(false);

        if (bundle != null)
        {
            PhotoModel data = bundle.GetValue<PhotoModel>("data");
            if (data != null)
            {
                StartCoroutine(StartLoadPlayer(data));
            }
            else
            {
                ScreenManager.Instance.CloseScreen(this);
            }
        }
        else
        {
            ScreenManager.Instance.CloseScreen(this);
        }
    }

    //when this screen closed
    public override void OprateCloseScreen()
    {
        UnityTools.SetCameraBlack(false);
    }

    private IEnumerator StartLoadPlayer(PhotoModel data)
    {
        yield return new WaitForSeconds(1f);
        GPlayerManager.Instance.Play(data, PlayerType.OnLineOneImg);
    }
}