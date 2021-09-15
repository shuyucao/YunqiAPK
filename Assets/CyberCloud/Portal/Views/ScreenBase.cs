using UnityEngine;

public class ScreenBase : MonoBase
{
    public delegate void ScreenBaseCallback();
    //public ScreenBaseCallback OnChangeScreenCallBack = null;
    //public ScreenBaseCallback OnCloseScreenCallBack = null;

    public UIScreen ScreenType = UIScreen.None;

    //when this screen move to the top
    public virtual void OprateChangeScreen(Bundle bundle) { }

    //when this screen closed
    public virtual void OprateCloseScreen() { }
}