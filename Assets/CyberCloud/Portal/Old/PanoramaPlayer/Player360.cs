using UnityEngine;
using System.Collections;
using System;

public class Player360 : MonoBehaviour
{
    public Texture mTexture;
    
    void Start()
    {
        //StartCoroutine(ChangeTexture());
    }


    public void SetTextureByPath( string texturePath )
    {
        Texture tempTexture = Resources.Load(texturePath) as Texture;
        if (tempTexture != null)
        {
            mTexture = tempTexture;
            gameObject.GetComponent<Renderer>().material.mainTexture = mTexture;
        }
        else
        {
            Debug.Log("Load Texture : " + texturePath + " failed");
            throw new ArgumentException("Load Texture : " + texturePath + " failed");
        }
    }


    public void SetTexture(Texture texture)
    {
        mTexture = texture;
        gameObject.GetComponent<Renderer>().material.mainTexture = mTexture;
    }
    

    IEnumerator ChangeTexture()
    {
        yield return new WaitForSeconds( 2.0f );

        string texturePath = "Textures/PanoramaPlayer/TestPic02";
        mTexture = Resources.Load(texturePath) as Texture;
        if (mTexture != null)
        {
            Debug.Log("Load Texture : " + texturePath + " success");
            gameObject.GetComponent<Renderer>().material.mainTexture = mTexture;
        }
        else
        {
            Debug.Log("Load Texture : " + texturePath + " failed");
            throw new ArgumentException( "Load Texture : " + texturePath + " failed" );
        }
        
    }


    //override public void Init()
    //{   
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        mTexture = Texture2D.CreateExternalTexture(PlayerMediaInfo.MediaWidth, PlayerMediaInfo.MediaHeight, TextureFormat.RGBA32, false, true, new System.IntPtr(textureId));
    //    }
    //}

    //override public void CreateExtraScreen()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        // For 360 player, we don't need to create extra screen.
    //        gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Player_360_Video_Texture_Shader"))
    //        {
    //            mainTexture = mTexture
    //        };
    //    }
    //}

}
