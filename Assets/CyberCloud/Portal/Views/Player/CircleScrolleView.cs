using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.PicoVR.Gallery;

public class CircleScrolleView : MonoBehaviour, IMsgHandle
{
    #region data
    public ItemClickCallback itemClick = null;
    [SerializeField]
    private UIScrollBar mScrolleBar;
    [SerializeField]
    private GameObject mLeftBtn;
    [SerializeField]
    private GameObject mRightBtn;
    [SerializeField]
    GameObject mItemTem;
    [SerializeField]
    Transform mItemRoot;


    private const int TotalItemNum = 15;
    private const int NumPerPage = 5;   //item nums per page
    private int CurPage = 1;            //current page id
    private int CurDataPage = 1;        //current data page id
    private int TotalPage;          //total page num

    public float Width = 10;            //step length
    public float Radis = 100;           //circle radis
    private float MoveSpeed = 0.6f;

    private float Sita;
    private float Degree;
    private float DegreePerPage;
    private float DegreeClipPerPage;
    private List<float> mRoaList = new List<float>();
    private List<Transform> mItemTranList = new List<Transform>();
    private CategoryPhotoData mCatData;
    private List<PhotoModel> mCurPhotoList = null;
    #endregion

    #region init
    public void Init()
    {
        InitData();
        ResetPosition();

        UIEventListener.Get(mLeftBtn).onClick = OnClickLeftBtn;
        UIEventListener.Get(mRightBtn).onClick = OnClickRightBtn;
        MsgManager.Instance.RegistMsg(MsgID.PhotoDataRefresh, this);
    }

    private bool isInit = false;
    private void InitData()
    {
        if (!isInit)
        {
            Sita = 2 * Mathf.Atan2(0.5f * Width, Radis);
            Degree = 180f * Sita / Mathf.PI;
            DegreePerPage = (NumPerPage - 1) * Degree;
            DegreeClipPerPage = NumPerPage * Degree;
            TotalPage = TotalItemNum / NumPerPage;
            mCatData = CachePhotoData.Instance.GetCatPhotoDataByID(HomePageScreen.CurrentID);
            RefreshCtrBar();

            CreateItemList();
            for (int i = 0; i < TotalPage; i++)
            {
                float delta_degree = DegreePerPage / 2 + i * DegreeClipPerPage;
                mRoaList.Add(delta_degree);
            }
            isInit = true;
        }
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if (id == MsgID.PhotoDataRefresh)
        {
            RefreshCtrBar();
        }
    }

    private void CreateItemList()
    {
        if (mItemTranList.Count > 0)
        {
            return;
        }
        mCurPhotoList = mCatData.GetPhotoData(NumPerPage, 1, TotalPage);
        if (mCurPhotoList == null)
        {
            Debug.LogError("can not get the photo data");
            return;
        }
        for (int i = 0; i < TotalItemNum; i++)
        {
            GalleryImageItem item = UnityTools.CreateComptent<GalleryImageItem>(mItemTem, mItemRoot);
            item.itemClick = itemClick;
            mItemTranList.Add(item.transform);
            if (i < mCurPhotoList.Count)
            {
                item.Init(mCurPhotoList[i]);
                item.ResetHoverPos();
            }
            else
            {
                item.gameObject.SetActive(false);
            }

            if (i >= NumPerPage)
            {
                TweenAlpha ta = TweenAlpha.Begin(item.gameObject, 0, 0);
                ta.PlayForward();
            }
        }
    }

    // refresh data
    private void RefreshData(List<PhotoModel> dlist)
    {
        if (dlist == null)
        {
            Debug.LogError("the list is null!");
            return;
        }
        mCurPhotoList = dlist;
        for (int i = 0; i < mItemTranList.Count; i++)
        {
            if (i < dlist.Count)
            {
                GalleryImageItem item = mItemTranList[i].GetComponent<GalleryImageItem>();
                item.gameObject.SetActive(true);
                item.Init(dlist[i]);
                item.ResetHoverPos();
            }
            else
            {
                mItemTranList[i].gameObject.SetActive(false);
            }
        }
    }

    private List<Transform> GetChildList()
    {
        List<Transform> list = new List<Transform>();

        for (int i = 0; i < mItemRoot.childCount; ++i)
        {
            Transform t = mItemRoot.GetChild(i);
            if (t && NGUITools.GetActive(t.gameObject))
                list.Add(t);
        }
        return list;
    }
    #endregion

    #region move
    private bool islock = false;   // 上下翻页锁定
    private IEnumerator Move(bool isnext)
    {
        islock = true;
        int flag = -1;
        if (isnext && CurPage == TotalPage)
        {
            flag = RePlaceItemList(true);
        }
        else if (!isnext && CurPage == 1)
        {
            flag = RePlaceItemList(false);
        }
        yield return new WaitForEndOfFrame();
        if (flag != -1 && flag != 5)
        {
            Debug.Log("the flag is:" + flag);
            if (flag == 1)
            {
                PlayAnimation(false);
            }
            else if (flag == 2)
            {
                PlayAnimation(true);
            }
        }
        else
        {
            // 如果下一页没有数据就不move，并且给出提示
            if (isnext && CheckDataReachend(CurPage + 1))
            {
                PlayAnimation(false);
            }
            else
            {
                Quaternion tem_qua;
                float delta_degree = mRoaList[isnext ? CurPage : (CurPage - 2)];
                if (isnext)
                {
                    CurPage++;
                    CurDataPage++;
                }
                else
                {
                    CurDataPage--;
                    CurPage--;
                }
                RefreshCtrBar();
                FadeInPage(CurPage, true);
                tem_qua = Quaternion.Euler(0, -delta_degree, 0);
                TweenRotation tr = TweenRotation.Begin(mItemRoot.gameObject, 0.6f, tem_qua);
                tr.AddOnFinished(() =>
                {
                    islock = false;
                    FadeInPage(CurPage - 1, false);
                    FadeInPage(CurPage + 1, false);
                });
                tr.PlayForward();
            }
        }
    }

    /// <summary>
    /// 第一页和最后一页时播放一个模拟动画
    /// </summary>
    private bool animLock = false;   // 动画锁定
    private void PlayAnimation(bool isfirst)
    {
        if (!animLock)
        {
            StartCoroutine(PlayFirstEndAnim(isfirst));
        }
    }

    /// <summary>
    /// 此处没有用tweenroatation是因为他在localRotation的插值上存在bug，某些特定角度的旋转动画有问题
    /// 因此在此利用协程模拟一个简单的回弹动画
    /// 这种方式不是特别好，但目前先这么整吧
    /// </summary>
    private IEnumerator PlayFirstEndAnim(bool isfirst)
    {
        animLock = true;
        int frame_total = Mathf.CeilToInt(0.3f / Time.deltaTime);
        float perdegree = DegreePerPage * 0.5f / frame_total;
        Quaternion ori = mItemRoot.localRotation;
        Vector3 pos = ori.eulerAngles;

        int i = 1;

        int flag = isfirst ? 1 : -1;
        while (i <= frame_total)
        {
            mItemRoot.localRotation = Quaternion.Euler(new Vector3(0, pos.y + flag * i * perdegree, 0));
            i++;
            yield return 0;
        }

        while (i > 1)
        {
            mItemRoot.localRotation = Quaternion.Euler(new Vector3(0, pos.y + flag * i * perdegree, 0));
            i--;
            yield return 0;
        }

        // 解除锁定
        islock = false;
        animLock = false;
    }

    private void MoveToNext(bool isnext)
    {
        if (islock)
        {
            return;
        }
        else
        {
            StartCoroutine(Move(isnext));
        }
    }

    public void MoveToRight()
    {
        MoveToNext(true);
    }

    public void MoveToLeft()
    {
        MoveToNext(false);
    }

    private void FadeInPage(int page, bool isactive)
    {
        if (page > TotalPage || page < 1)
        {
            return;
        }
        for (int i = (page - 1) * NumPerPage; i < mItemTranList.Count && i < page * NumPerPage; i++)
        {
            TweenAlpha ta = TweenAlpha.Begin(mItemTranList[i].gameObject, 0.3f, isactive ? 1f : 0);
            ta.PlayForward();
        }
    }
    #endregion

    #region position
    private void ResetPosition()
    {
        List<PositionData> poslist = CalculatePosition(TotalItemNum);

        for (int i = 0; i < TotalItemNum; i++)
        {
            mItemTranList[i].localPosition = poslist[i].position;
            mItemTranList[i].localRotation = poslist[i].quaternion;
        }

        if (CurPage == 1)
        {
            mItemRoot.localRotation = Quaternion.Euler(0, -mRoaList[0], 0);
        }
        else if (CurPage == TotalPage)
        {
            mItemRoot.localRotation = Quaternion.Euler(0, -mRoaList[TotalPage - 1], 0);
        }
    }

    private List<PositionData> CalculatePosition(int num)
    {
        this.InitData();
        List<PositionData> data = new List<PositionData>();
        for (int i = 0; i < num; i++)
        {
            float x = Radis * Mathf.Sin(i * Sita);
            float z = Radis * Mathf.Cos(i * Sita);
            Vector3 tem_pos = new Vector3(x, 0, z);
            Quaternion tem_qua = Quaternion.AngleAxis(i * Degree, Vector3.up);
            PositionData tem_data = new PositionData(tem_pos, tem_qua);
            data.Add(tem_data);
        }
        return data;
    }

    [ContextMenu("Execute")]
    private void Reposition()
    {
        isInit = false;
        ResetPosition();
    }

    /// <summary>
    /// set the page to first
    /// 5:OK  1:last page  2:first page  3:no cache data  4:unknow error
    /// </summary>
    /// <param name="page"></param>
    private int RePlaceItemList(bool islast)
    {
        List<PhotoModel> phList = null;
        List<Transform> temp = new List<Transform>();
        if (islast)
        {
            if (mCatData.IsLastPage(NumPerPage, CurDataPage))
            {
                //CommonAlert.Show("Bar_Last");
                return 1;
            }

            phList = mCatData.GetPhotoData(NumPerPage, CurDataPage, CurDataPage + TotalPage - 1);
            if (phList == null || phList.Count == NumPerPage)
            {
                DataLoader.Instance.RequestNextPage();
                return 3;
            }

            for (int i = (TotalPage - 1) * NumPerPage; i < TotalPage * NumPerPage && i < mItemTranList.Count; i++)
            {
                temp.Add(mItemTranList[i]);
            }
            for (int i = 0; i < (TotalPage - 1) * NumPerPage; i++)
            {
                temp.Add(mItemTranList[i]);
            }
            CurPage = 1;
        }
        else
        {
            if (CurDataPage == 1)
            {
                //CommonAlert.Show("Bar_First");
                return 2;
            }

            phList = mCatData.GetPhotoData(NumPerPage, CurDataPage - TotalPage + 1, CurDataPage);
            //Debug.LogError("page:" + (CurDataPage - TotalPage + 1) + "--" + CurDataPage);
            if (phList == null || phList.Count != TotalItemNum)
            {
                Debug.LogError("phlist is null ?:" + (phList == null) + " curdatapage:" + CurDataPage);
                return 4;
            }

            for (int i = NumPerPage; i < mItemTranList.Count; i++)
            {
                temp.Add(mItemTranList[i]);
            }
            for (int i = 0; i < NumPerPage; i++)
            {
                temp.Add(mItemTranList[i]);
            }
            CurPage = TotalPage;
        }
        mItemTranList = temp;
        ResetPosition();
        RefreshData(phList);
        return 5;
    }

    private bool CheckDataReachend(int page)
    {
        if (mCurPhotoList.Count <= (page - 1) * NumPerPage)
        {
            //CommonAlert.Show("Bar_Last");
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    private void OnClickLeftBtn(GameObject go)
    {
        MoveToLeft();
    }

    private void OnClickRightBtn(GameObject go)
    {
        MoveToRight();
    }

    private void RefreshCtrBar()
    {
        int total = Mathf.CeilToInt((float)mCatData.PhotoList.Count / NumPerPage);
        float size = 1.0f / total;
        if (CurDataPage == 1)
        {
            SetScrolleBar(0, size);
        }
        else if (CurDataPage == total)
        {
            SetScrolleBar(1, size);
        }
        else
        {
            float val = (float)CurDataPage / (total + 1);
            SetScrolleBar(val, size);
        }
    }

    private void SetScrolleBar(float val, float size)
    {
        mScrolleBar.barSize = size;
        TweenScrollBar ts = TweenScrollBar.Begin(mScrolleBar.gameObject, MoveSpeed, val);
        ts.PlayForward();

    }

    void OnDestroy()
    {
    }
}