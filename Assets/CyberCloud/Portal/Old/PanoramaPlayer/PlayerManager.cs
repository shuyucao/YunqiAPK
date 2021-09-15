using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    private ImagePackData mImagePackData = null;
    public GameObject mBackButtonObject;
    public GameObject mShowButtonObject;
    public GameObject mScrollViewBackground;
    public GameObject mTable;
    public GameObject mIconButtonPrefab;
    public GameObject mHoverOnCollider;
    public GameObject mHoverOffCollider;
    public Player360 mPlayer360;
    public Color mDefaultIconColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
    public Color mCurrentIconColor = new Color( 0.85f, 0.75f, 0.0f, 1.0f );
    public UILabel mLoadingLabel;


    private bool mScrollViewActive = false;


    protected AndroidJavaClass mJavaClass = null;
    protected AndroidJavaObject mJavaObject = null;
    private const string JAVACLASS = "com.picovr.gallery.UnityActivity";
    private const string JAVAOBJECT = "unityActivity";

    private const string LOCALTEXTUREPATH = "/storage/emulated/0/DCIM/Camera/VID_20160223_145241.png";
    private const string JSONCATEGORYTEST = 
       "{ \"@attributes\": { \"version\": \"2.1\" },  \"status\": \"success\", \"categories\": { \"@attributes\":{ \"count\": \"6\" }, \"category\":[" + 
       "{\"cid\": \"13\", \"name\": \"风景\", \"english_name\": \"Landscape\"}" +
       "]    } }";
    private const string JSONPHOTOTEST =
   "{ \"@attributes\": { \"version\": \"2.1\" },  \"status\": \"success\",\"total\": \"2\",\"page\": \"1\", \"photos\": { \"@attributes\":{ \"count\": \"5\" }, \"photo\":[" +
   "{\"mid\": \"9\", \"title\": \"商场3\", \"cover_link\": \"http://test.photo.picovr.com/images/covers/c11111111.jpg\",\"thumbnail_link\": \"http://test.photo.picovr.com/images/thumbs/c11111111.jpg\", \"photo_link\": \"http://test.photo.picovr.com/images/photos/c22222222.jpg\"}" +
   "]    } }";
                        


	// Use this for initialization
	void Start () {

        if (mBackButtonObject != null)
        {
            Debug.Log("mBackButtonObject != null");
            UIEventListener.Get(mBackButtonObject).onClick = OnButtonClick;
        }

        if (mShowButtonObject != null)
        {
            Debug.Log("mBackButtonObject != null");
            UIEventListener.Get(mShowButtonObject).onClick = OnButtonClick;
        }

        //if (mHoverOnCollider != null)
        //{
        //    Debug.Log("mHoverOnCollider != null");
        //    UIEventListener.Get(mHoverOnCollider).onHover = (GameObject go, bool ishover) => { if (ishover) { ShowUI(true); } };
        //}

        //if (mHoverOffCollider != null)
        //{
        //    Debug.Log("mHoverOffCollider != null");
        //    UIEventListener.Get(mHoverOffCollider).onHover = (GameObject go, bool ishover) => { if (ishover) { ShowUI(false); } };
        //}


        //加载jar 获得图片文件的地址
        string result = null;
        //Debug.Log("PlayerManager mJavaClass : before RuntimePlatform.Android");
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    Debug.Log("PlayerManager mJavaClass : before AndroidJavaClass");
        //    mJavaClass = new AndroidJavaClass(JAVACLASS);

        //    Debug.Log("PlayerManager mJavaObject : before AndroidJavaObject");
        //    mJavaObject = mJavaClass.GetStatic<AndroidJavaObject>(JAVAOBJECT);
        //    Debug.Log("PlayerManager mJavaObject : after AndroidJavaObject");

        //    result = mJavaObject.Call<string>("getImgpath");
        //    Debug.Log("PlayerManager getImgpath result : " + result);
        //}


        int msg = 0;
        JsonUtils.ParseCategoryJson(JSONCATEGORYTEST, ref msg);
        //JsonUtils.ParsePhotoJson(JSONPHOTOTEST, ref msg);



        Debug.Log("PlayerManager mJar : before if");
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("PlayerManager mJar : before AndroidJavaClass");
            AndroidJavaClass jc = new AndroidJavaClass("com.picovr.gallery.UnityActivity");

            Debug.Log("PlayerManager mJar : before AndroidJavaObject");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("unityActivity");
            //AndroidJavaObject jo = jc.Get<AndroidJavaObject>("UnityActivity");
            Debug.Log("PlayerManager mJar : after AndroidJavaObject");


            result = jo.Call<string>("getCategory");
            Debug.Log("PlayerManager : getCategory : " + result);


            result = jo.Call<string>("getImgpath");
            Debug.Log("PlayerManager : getImgpath : " + result);
        }




        //mImagePackData = ImagePackData.ParseJson( null, null );
        //StartCoroutine( Load360Texture() );
        //StartCoroutine(LoadLocalTexture());   //临时用这个
        
        //StartCoroutine(GetLocalImage( LOCALTEXTUREPATH ));
        if (result != null)
        {
            StartCoroutine(GetLocalImage(result));
        }
        else
        {
            SetLoadingLabelText( "没有图片" );
        }
        

        //Show Button暂时不用 临时注掉
        //if (mTable != null)
        //{
        //    Debug.Log("mTable != null");
        //    CreateTable(mTable.transform);
        //}


        //InputManager.OnBack += OnBack;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
       // InputManager.OnBack -= OnBack;
    }



    void OnBack()
    {
 #if UNITY_EDITOR
#else
        Application.Quit();
#endif
    }


    void OnButtonClick(GameObject obj)
    {
        Debug.Log( "OnButtonClick : " + obj.name );


        if (obj == mBackButtonObject)
        {
#if UNITY_EDITOR
#else
        Application.Quit();
#endif
        }

        if (obj == mShowButtonObject)
        {
            mScrollViewActive = !mScrollViewActive;
            ShowUI(mScrollViewActive);
        }
    }


    void OnIconButtonClick(GameObject obj)
    {
        Debug.Log( "OnIconButtonClick : " + obj.name );

        if (obj.GetComponent<IconButtonData>().Index != mImagePackData.CurrentListIndex)
        {
            obj.GetComponent<UIButton>().defaultColor = mCurrentIconColor;
            obj.GetComponent<UIButton>().UpdateColor( true );

            GameObject prevObj = mTable.transform.GetChild( mImagePackData.CurrentListIndex ).gameObject;
            prevObj.GetComponent<UIButton>().defaultColor = mDefaultIconColor;
            prevObj.GetComponent<UIButton>().UpdateColor(true);
            mImagePackData.CurrentListIndex = obj.GetComponent<IconButtonData>().Index;
            
            
            Set360Texture(obj.GetComponent<IconButtonData>().TextureName);
        }
    }

    void Set360Texture( string texturePath )
    {
        if (mPlayer360 != null)
        {
            mPlayer360.SetTextureByPath(texturePath);
        }
        else
        {
            Debug.LogError("mPlayer360 == null");
        }
    }


    void CreateTable(Transform table)
    {
        Debug.Log("CreateTable : " + table.name);
        DeleteAllChildren( table );

        GameObject newItem;
        for (int i = 0; i < mImagePackData.IconList.Count; i++)
        {
            newItem = Instantiate<GameObject>(mIconButtonPrefab);
            if (newItem != null)
            {
                newItem.name = i.ToString();
                newItem.transform.parent = table;
                newItem.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                newItem.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                newItem.SetActive(true);
                UIEventListener.Get(newItem).onClick = OnIconButtonClick;

                newItem.GetComponent<IconButtonData>().TextureName = mImagePackData.IconList.ToArray()[i].TextureName;
                newItem.GetComponent<IconButtonData>().IconName = mImagePackData.IconList.ToArray()[i].IconName;
                newItem.GetComponent<IconButtonData>().Index = mImagePackData.IconList.ToArray()[i].Index;
                Texture iconTexture = Resources.Load(newItem.GetComponent<IconButtonData>().IconName) as Texture;
                if (iconTexture != null)
                {
                    newItem.GetComponent<UITexture>().mainTexture = iconTexture;
                }


                if (mImagePackData.CurrentListIndex == i)
                {
                    newItem.GetComponent<UIButton>().defaultColor = mCurrentIconColor;
                    newItem.GetComponent<UIButton>().UpdateColor(true);
                }

            }
            else
            {
                Debug.Log("Instantiate mIconButtonPrefab " + i.ToString() + " failed");
            }

        }

        table.GetComponent<UITable>().repositionNow = true;
    }


    void DeleteAllChildren(Transform container)
    {
        // safely delete all current children
        List<GameObject> oldObjects = new List<GameObject>();
        for (int i = 0; i < container.childCount; i++)
        {
            oldObjects.Add(container.GetChild(i).gameObject);
        }

        while (container.childCount > 0)
        {
            GameObject itemToDelete = oldObjects[0];
            oldObjects.RemoveAt(0);
            GameObject.DestroyImmediate(itemToDelete);
        }
    }

    void ShowUI(bool show)
    {
        if (mScrollViewBackground != null)
        {
            mScrollViewBackground.SetActive(show);
        }
    }


    void SetLoadingLabelText( string text )
    {
        if (mLoadingLabel != null)
        {
            mLoadingLabel.text = text;
        }
    }

    void SetLoadingLabelActive( bool active )
    {
        if (mLoadingLabel != null)
        {
            mLoadingLabel.transform.gameObject.SetActive(active);
        }
    }



    IEnumerator Load360Texture()
    {
        Set360Texture(mImagePackData.IconName);

        int loadingProgress = 0;
        string tempText;
        while (loadingProgress < 100)
        {
            ++loadingProgress;

            tempText = "加载中 " + loadingProgress.ToString() + "%";
            SetLoadingLabelText(tempText);
            
            yield return new WaitForEndOfFrame();
        }

        SetLoadingLabelActive( false );
        Set360Texture(mImagePackData.TextureName);
    }


    IEnumerator LoadLocalTexture()
    {
        int loadingProgress = 0;
        string tempText;
        while (loadingProgress < 100)
        {
            ++loadingProgress;

            tempText = "加载中 " + loadingProgress.ToString() + "%";
            SetLoadingLabelText(tempText);

            yield return new WaitForEndOfFrame();
        }

        SetLoadingLabelActive( false );
        Set360Texture(mImagePackData.TextureName);
    }



    public string GetFileURL(string path)
    {
        return (new System.Uri(path)).AbsoluteUri;
    }


    IEnumerator  GetLocalImage( string url )
    {
        //string filePath = "file:///" + CACHE_FILE_PATH + url.GetHashCode();
#if UNITY_EDITOR
    
        string filePath = "file:///"+url;
#elif UNITY_ANDROID
        string filePath = "file://" + url;
#endif
        Debug.Log("GetLocalImage filePath : " + filePath);
        WWW www = new WWW (filePath);
        yield return www;

        //if(www.error == null && www.progress >= 1.0f)
        if(www.error == null && www.texture != null)
        {
            Texture2D image = www.texture;
            Debug.Log("GetLocalImage texture size : width * height =  "+image.width+" * "+image.height);


            if (mPlayer360 != null)    //换贴图
            {
                mPlayer360.SetTexture(image);
            }

            SetLoadingLabelActive( false );
        }
        else
        {
            Debug.Log("GetLocalImage failed : " + www.error);
            SetLoadingLabelText( "加载失败" );
        }

        www.Dispose ();
    }

}
