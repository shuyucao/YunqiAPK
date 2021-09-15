using UnityEngine;

public class ImageItemBase : MonoBase
{
    public UITexture mTexture;
    private PhotoModelBase mData = null;
    public PhotoModelBase Data
    {
        get
        {
            return mData;
        }
        set
        {
            mData = value;
        }
    }

    private bool misTexReady = false;
    public bool IsTexReady
    {
        get
        {
            return misTexReady;
        }
    }

    public virtual void Init(PhotoModelBase data) { }

    public void SetTexture(Texture2D tex)
    {
        if (mTexture != null && tex != null)
        {
            mTexture.mainTexture = tex;
            misTexReady = true;
        }
    }

    public virtual void LoadTexture() { }

    public void ResetReadyState()
    {
        if (misTexReady)
        {
            //SetTexture(CachePhotoData.Instance.GetIconTexture("default"));
            misTexReady = false;
        }
    }
}