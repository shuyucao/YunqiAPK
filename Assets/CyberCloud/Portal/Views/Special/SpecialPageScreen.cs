using Com.PicoVR.Gallery;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPageScreen : ScreenBase, IMsgHandle
{
    [SerializeField]
    GameObject mSpecialBack;
    [SerializeField]
    GameObject mSpecialListPage;
    [SerializeField]
    GameObject mPhotoListPage;
    [SerializeField]
    GameObject mLoading;
    [SerializeField]
    GameObject mRefreshPage;
    [SerializeField]
    GameObject mBtnRefresh;
    public static Layer layer = Layer.None;
    private bool fromLauncher = false;
    string tid = null;

    [SerializeField]
    ScrollController scrollControllerThemes;
    List<BaseData> dataListThemes;


    [SerializeField]
    ScrollController scrollControllerPhotos;
    List<BaseData> dataListPhotos;

    void Start()
    {
        mSpecialListPage.SetActive(false);
        mPhotoListPage.SetActive(false);

        UIEventListener.Get(mSpecialBack).onClick = OnClickBack;
        UIEventListener.Get(mBtnRefresh).onClick = OnclickRefresh;

        MsgManager.Instance.RegistMsg(MsgID.ThemesDataReady, this);
        MsgManager.Instance.RegistMsg(MsgID.ThemePhotoDataRefresh, this);
        MsgManager.Instance.RegistMsg(MsgID.CheckRefreshPage, this);
        MsgManager.Instance.RegistMsg(MsgID.CategoryDataReady, this);
    }

    private void OnclickRefresh(GameObject go)
    {
        Debug.Log("OnclickRefresh is clicked !!!!");
        if (MachineState.IsWifiAvailable)
        {
            mLoading.SetActive(true);
            mRefreshPage.SetActive(false);
            if(tid != null)
                DataLoader.Instance.RequestThemesPhoto(tid);
            else
                DataLoader.Instance.RequestThemes();
        }
        else
        {
            CommonAlert.Show("Home_NoNet");
        }
    }
    
    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (MsgID.ThemesDataReady == id)
        {
            mLoading.SetActive(false);
            if (mPhotoListPage.activeSelf)
            {
                mPhotoListPage.SetActive(false);
            }

            if (!mSpecialListPage.activeSelf)
            {
                mSpecialListPage.SetActive(true);
            }

            Debug.Log("Themeslist ::: " + CachePhotoData.Instance.ThemesList.ToArray().Length);
            //mSpecialListPage.InitData(CachePhotoData.Instance.ThemesList);
            SpecialPageScreen.layer = Layer.SpecialList;

            if (mRefreshPage.activeInHierarchy)
                mRefreshPage.SetActive(false);

            if (dataListThemes == null)
            {
                dataListThemes = new List<BaseData>();
            }
            else
                dataListThemes.Clear();

            for (int i = 0; i < CachePhotoData.Instance.ThemesList.ToArray().Length; i++)
            {
                ThemesModel mdata = new ThemesModel();
                mdata = CachePhotoData.Instance.ThemesList[i];
                dataListThemes.Add(mdata);
            }
            scrollControllerThemes.InitDataList(dataListThemes, true);

        }
        else if (MsgID.ThemePhotoDataRefresh == id)
        {
            mLoading.SetActive(false);

            if (mSpecialListPage.activeSelf)
            {
                mSpecialListPage.SetActive(false);
            }

            if (!mPhotoListPage.activeSelf)
            {
                mPhotoListPage.SetActive(true);
            }

            Debug.Log("tid is " + bundle.GetValue<string>("themeID"));
            tid = bundle.GetValue<string>("themeID");
            CategoryPhotoData data = CachePhotoData.Instance.GetThemesPhotosByID(bundle.GetValue<string>("themeID"));
            //mPhotoListPage.InitData(data);
            SpecialPageScreen.layer = Layer.PhotoList;

            if (mRefreshPage.activeInHierarchy)
                mRefreshPage.SetActive(false);

            if (dataListPhotos == null)
            {
                dataListPhotos = new List<BaseData>();
            }
            else
                dataListPhotos.Clear();
            Debug.Log("data.PhotoList.Count   =   "+ data.PhotoList.Count);
            for (int i = 0; i < data.PhotoList.Count; i++)
            {
                PhotoModel mdata = new PhotoModel();
                mdata = data.PhotoList[i];
                dataListPhotos.Add(mdata);
            }

            scrollControllerPhotos.InitDataList(dataListPhotos, true);
        }
        else if (id == MsgID.CheckRefreshPage)
        {
            FreshPageState();
        }
        else if (id == MsgID.BackToSL)
        {
            Back();
        }
    }
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.P))
        //{
        //    test();
        //}

    }
    public void test()
    {
        int num = 0;
        for (int i = 0; i < CachePhotoData.Instance.ThemesList.ToArray().Length; i++)
        {
            ThemesModel mdata = new ThemesModel();
            mdata = CachePhotoData.Instance.ThemesList[i];
            dataListThemes.Add(mdata);
            num += 1;
        }
        scrollControllerThemes.InitDataList(dataListThemes, true);
    }
    private void FreshPageState()
    {
        mLoading.SetActive(false);
        mRefreshPage.SetActive(true);
        Debug.Log("RefreshPage !!!!!");
        if (mPhotoListPage.activeSelf)
        {
            mPhotoListPage.SetActive(false);
        }

        if (mSpecialListPage.activeSelf)
        {
            mSpecialListPage.SetActive(false);
        }
    }

    //when this screen move to the top
    public override void OprateChangeScreen(Bundle bundle)
    {
        Debug.Log("SpecialPageScreen   OprateChangeScreen ， layer ==  " + SpecialPageScreen.layer);
        //GalleryTools.ShowLeftBar(true);
        if (bundle != null)
        {
            fromLauncher = true;
            tid = bundle.GetValue<string>("tid");
            DataLoader.Instance.RequestThemesPhoto(tid);
        }
        else if (SpecialPageScreen.layer == Layer.None || SpecialPageScreen.layer == Layer.SpecialList)
        {
            mPhotoListPage.SetActive(false);

            if (CachePhotoData.Instance.ThemesList.Count == 0)
            {
                DataLoader.Instance.RequestThemes();
            }
            else
            {
                MsgManager.Instance.SendMsg(MsgID.ThemesDataReady, null);
            }

        }
        else if (SpecialPageScreen.layer == Layer.PhotoList)
        {
            mSpecialListPage.SetActive(false);
        }
       // InputManager.OnBack += Back;
        MsgManager.Instance.RegistMsg(MsgID.BackToSL, this);
    }

    //when this screen closed
    public override void OprateCloseScreen()
    {
        Debug.Log("SpecialPageScreen   OprateCloseScreen");
        //InputManager.OnBack -= Back;
        MsgManager.Instance.RemoveMsg(MsgID.BackToSL, this);
        if (SpecialPageScreen.layer == Layer.PhotoList)
        {
            //mPhotoListPage.SetController(false);
        }
        else
        {
            //mSpecialListPage.SetController(false);
        }
    }

    private void OnClickBack(GameObject obj)
    {
        Back();
    }

    private void Back()
    {
        Debug.Log("specialpagescreen layer == "+ SpecialPageScreen.layer);
        if (SpecialPageScreen.layer == Layer.PhotoList)
        {
            mPhotoListPage.SetActive(false);
            mSpecialListPage.SetActive(true);
            SpecialPageScreen.layer = Layer.SpecialList;
            if (fromLauncher)
            {
                DataLoader.Instance.RequestThemes();
                fromLauncher = false;
            }
        }
        else
            GalleryTools.QuitApp();
    }
    public enum Layer
    {
        SpecialList,
        PhotoList,
        None
    }
}