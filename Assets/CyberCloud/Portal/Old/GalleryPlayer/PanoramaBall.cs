using UnityEngine;

public enum PhotoTextureType
{
    None,
    Cover,
    Photo
}

public class PanoramaBall : MonoBehaviour
{
    public Texture mTexture;  //只是测试函数用
    private PhotoTextureType mCurrentTextureType = PhotoTextureType.None;
    public PhotoTextureType CurrentTextureType
    {
        get
        {
            return mCurrentTextureType;
        }
        set
        {
            if (mCurrentTextureType == PhotoTextureType.None)
            {
                if (value == PhotoTextureType.Photo)
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        UnityTools.SetMirrorUV(gameObject);
                    }
                }
            }
            else if (mCurrentTextureType != value)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    UnityTools.SetMirrorUV(gameObject);
                }
            }

            mCurrentTextureType = value;
        }
    }

    void OnDestroy()
    {
        SetTextureAndDestroyOld(null, PhotoTextureType.Cover);
    }

    public void SetTextureAndDestroyOld(Texture texture, PhotoTextureType type)
    {
        Texture oldTexture = gameObject.GetComponent<Renderer>().material.mainTexture;
        PhotoTextureType oldType = CurrentTextureType;

        if (texture != null)
        {
            float c = 221.0f / 255.0f;
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(c, c, c, 1.0f));
            CurrentTextureType = type;
            gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 0.0f, 0.0f, 1.0f));
        }

        // 释放老贴图
        if (oldType == PhotoTextureType.Photo)
        {
            UnLoadOldTexture(oldTexture);
        }
    }

    private int persistUnloadTex = 0;
    /// <summary>
    /// 累计清除MaxPersistUnloadNum次之后调用一次UnloadUnusedAssets
    /// 防止每一次都调用，耗费大量的cpu性能
    /// </summary>
    /// <param name="tex"></param>
    private void UnLoadOldTexture(Texture tex)
    {
        GameObject.DestroyImmediate(tex);
        tex = null;
        persistUnloadTex++;
        if (persistUnloadTex >= Constant.MaxPersistUnloadNum)
        {
            Resources.UnloadUnusedAssets();
            persistUnloadTex = 0;
        }
    }
}