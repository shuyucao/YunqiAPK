using System.Collections.Generic;
using UnityEngine;

public class HomePageVScrView : VerticalScrolleView

{
    private CategoryPhotoData mCatData;
    private List<PhotoModel> mDataList = null;

    [SerializeField]
    ScrollController scrollControllerPhotos;
    List<BaseData> dataListPhotos;

    public void InitData(CategoryPhotoData data)
    {
        if (data == null)
        {
            //TODO
            Debug.LogError("the data is null!");
            //SetActiveCtrBar(false);
            return;
        }
        mCatData = data;
     
        base.Init(mCatData.PhotoList.Count, PageManager.PageType.PhotoPage);
    }
    PhotoModel mdata = new PhotoModel();
    //base.Init創建成功后自動調用
    public override void FillData(int begin, int end)
    {
        //if (true)
        //    return;
        if (dataListPhotos == null)
            dataListPhotos = new List<BaseData>();
        else
            dataListPhotos.Clear();

        Debug.Log("mCatData PhotoList.Count    " + mCatData.PhotoList.Count);
        for (int i = 0; i < mCatData.PhotoList.Count; i++)
        {
            mdata = mCatData.PhotoList[i];
            dataListPhotos.Add(mdata);
            mdata = null;
        }
        scrollControllerPhotos.InitDataList(dataListPhotos, true);
        //显示返回按钮 这个事件leftmenu中未处理
        MsgManager.Instance.SendMsg(MsgID.BackBtn, null);
    }
}