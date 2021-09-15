using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScrollePage : MonoBase, IMsgHandle
{
    [SerializeField]
    private GameObject mScrolleBar;
    [SerializeField]
    private GameObject mLeftBtn;
    [SerializeField]
    private GameObject mRightBtn;
    //[SerializeField]
    //private PlayerToast mToast;
    [SerializeField]
    private GameObject mIconItem;  //列表Icon的Prefab
    [SerializeField]
    private CircleScrolleView mScrolleView;

    private string mCurId;
    private CategoryPhotoData mCatgData;
    private List<ImageItemBase> mImageItemList = null;

    public void InitData()
    {
        mCatgData = CachePhotoData.Instance.GetCatPhotoDataByID(HomePageScreen.CurrentID);
        if (mCatgData != null)
        {
            mCurId = mCatgData.CategoryID;
        }

        //StartCoroutine(CreateItemList());

        UIEventListener.Get(mLeftBtn).onClick = OnClickLeftBtn;
        UIEventListener.Get(mRightBtn).onClick = OnClickRightBtn;
        MsgManager.Instance.RegistMsg(MsgID.PhotoDataRefresh, this);
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.PhotoDataRefresh)
        {
            //TODO

        }
    }

    //private IEnumerator CreateItemList()
    //{
    //    for (int i = 0; i < mCatgData.PhotoList.Count; i++)
    //    {
    //        ImageItemBase item = UnityTools.CreateComptent<ImageItemBase>(mIconItem.gameObject, mScrolleView.transform);
    //        item.Init(mCatgData.PhotoList[i]);
    //        if ((i + 1) % 10 == 0)
    //        {
    //            yield return new WaitForEndOfFrame();
    //        }

    //    }
    //    //mScrolleView.Reposition();
    //}

    private void OnClickLeftBtn(GameObject go)
    {
        mScrolleView.MoveToLeft();
    }

    private void OnClickRightBtn(GameObject go)
    {
        mScrolleView.MoveToRight();
    }
}