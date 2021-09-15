using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ImageScrollView : MonoBehaviour
{
    [Tooltip("Number of Columns for one page")]
    public int ColumnsPerPage;
    [Tooltip("Number of Rows for one page")]
    public int RowsPerPage;
    [Tooltip("Horizontal Space between items")]
    public int HorizontalSpacing;
    [Tooltip("Vertical Space between items")]
    public int VerticalSpacing;
    [Tooltip("Spacing between two page")]
    public float PageSpacing;
    [Tooltip("Item template object,each item is generated from this object")]
    public GameObject ItemTemplate;
    [Tooltip("Left Scroll Bar")]
    public UISprite ScrollBarLeft;
    [Tooltip("Right Scroll Bar")]
    public UISprite ScrollBarRight;
    public HomeScreen mHomeScreen;
    [Tooltip("Top left item's position")]
    private Vector3 BasePosition;
    public UISprite mScrollLeftBg;
    public UISprite mScrollRightBg;
    private float PageWidth = 0.0f;
    private float PageHeight = 0.0f;
    private float PageMoveDuration = 1.0f;
    private float FadeAnimationDuration = 1.0f;
    private int CurrentPageIndex = 0;
    private bool AcceptKeyEvent = false;
    private int ItemCount;
    private int PageCount;
    private int ActualPageCount;
    private List<GameObject> ItemList = null;
    private Vector3 mOriginBarPos = new Vector3(0.0f, 25.0f, 0.0f);
    private int mOriginBarHeight = 7;
    private bool mPageTransiting = false;  //目前只用在OnAllIconDownloaded_Home

    public enum PageDirection
    {
        Previous,
        Next,
    }

    // Use this for initialization
    void Start()
    {

        UIPanel panel = gameObject.GetComponent<UIPanel>();
        if (panel != null)
        {
            //BasePosition = new Vector3(panel.GetViewSize().x * -0.5f, panel.GetViewSize().y * 0.5f, 0.0f);
            BasePosition = new Vector3(-60.0f, 20.0f, 50.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || (Input.GetAxis("Vertical") == 1))
        {
            TransitPage(PageDirection.Previous);
        }

        if (Input.GetKeyDown(KeyCode.S) || (Input.GetAxis("Vertical") == -1))
        {
            TransitPage(PageDirection.Next);
        }
    }

    void OnDestroy()
    {
        ClearData();
    }

    public void InitInput()
    {
       // InputManager.OnUp += OnUp;
      //  InputManager.OnDown += OnDown;
    }

    public void DestroyInput()
    {
       // InputManager.OnUp -= OnUp;
     //   InputManager.OnDown -= OnDown;
    }

    void OnUp()
    {
        TransitPage(PageDirection.Previous);
    }

    void OnDown()
    {
        TransitPage(PageDirection.Next);
    }

    #region 几个给HomeScreen用的public函数

    public List<GameObject> GetItemList()
    {
        return ItemList;
    }

    public void InitData(int totalPage, int itemCount)
    {
        ItemList = new List<GameObject>();

        ItemCount = itemCount;
        if (ItemCount % ColumnsPerPage == 0)
        {
            ActualPageCount = ItemCount / ColumnsPerPage;
        }
        else
        {
            ActualPageCount = ItemCount / ColumnsPerPage + 1;
        }

        PageCount = totalPage;


        InitItems(ItemCount);
        UpdateScrollBarState();

        //等图全下完了才能点
        MakeAllItemsDisabled();

        if (PageCount == 1)
        {
            HomeScreen.OnAllIconDownloaded += OnAllIconDownloaded_Home;
        }

    }


    public void ClearData()
    {
        PageWidth = 0.0f;
        PageHeight = 0.0f;

        CurrentPageIndex = 0;
        AcceptKeyEvent = false;
        ItemCount = 0;
        PageCount = 0;
        ActualPageCount = 0;


        StopAllCoroutines();

        if (ItemList != null)
        {
            while (ItemList.Count > 0)
            {
                GameObject itemToDelete = ItemList[0];
                GameObject.DestroyImmediate(itemToDelete);
                itemToDelete = null;
                ItemList.RemoveAt(0);
            }
            ItemList = null;
        }

        ResetScrollBarState();
        Debug.Log("ClearData Finished");
    }


    public void PrepareForGetFirst()
    {
        //等图全下完了才能点
        MakeAllItemsDisabled();
        HomeScreen.OnAllIconDownloaded += OnAllIconDownloaded_Home;
    }



    public void AddItem(int addNum)
    {
        int oldItemCount = ItemCount;

        ItemCount = ItemCount + addNum;
        if (ItemCount % ColumnsPerPage == 0)
        {
            ActualPageCount = ItemCount / ColumnsPerPage;
        }
        else
        {
            ActualPageCount = ItemCount / ColumnsPerPage + 1;
        }

        for (int i = 0; i < addNum; i++)
        {
            ItemList.Add(InstantiateItemByIndex(oldItemCount + i));
            UIEventListener.Get(ItemList[i]).onClick += OnClick;
            UIEventListener.Get(ItemList[i]).onHover += OnHover;
        }

    }



    public void SetItemTitle(int index, string title)
    {
        if (index + 1 > ItemList.Count)
        {
            Debug.LogError("SetItemTitle : index + 1 > ItemList.Count !");
        }
        else
        {
            GameObject label = ItemList[index].transform.Find("Label").gameObject;
            if (label != null)
            {
                label.GetComponent<UILabel>().text = title;
            }
        }
    }



    public void RecoverList(int totalPage, int itemCount)
    {
        ItemList = new List<GameObject>();

        ItemCount = itemCount;   //ItemCount只在InitData AddItem RecoverList几个函数里用到
        if (ItemCount % ColumnsPerPage == 0)
        {
            ActualPageCount = ItemCount / ColumnsPerPage;
        }
        else
        {
            ActualPageCount = ItemCount / ColumnsPerPage + 1;
        }

        PageCount = totalPage;


        //有可能需要下载图片
        mHomeScreen.ResetDownloadCounting();
        HomeScreen.OnAllIconDownloaded += OnAllIconDownloaded_Home;
        //创建图片列表
        string title;
        List<PhotoModel> photoList = GlobalPhotoData.Instance.GetPhotoList();
        List<IconTexture> iconTextureList = GlobalPhotoData.Instance.GetIconTextureList();
        for (int i = 0; i < itemCount; i++)
        {
            ItemList.Add(InstantiateItemByIndex(i));
            UIEventListener.Get(ItemList[i]).onClick += OnClick;
            UIEventListener.Get(ItemList[i]).onHover += OnHover;

            GameObject label = ItemList[i].transform.Find("Label").gameObject;
            if (label != null)
            {
                title = photoList[i].Title;
                label.GetComponent<UILabel>().text = title;
            }

            GameObject textureObject = ItemList[i].transform.Find("texture").gameObject;
            if (textureObject != null)
            {

                IconTexture icon = iconTextureList.Find(
                            delegate (IconTexture it)
                            {
                                return it.index == i;
                            });

                if (icon == null)
                {
                    //找不到图片 需要下载
                    Debug.Log("RecoverList : " + i + "  iconTextureList does not have the texture !");
                    mHomeScreen.DownloadIcon_Index(i, ItemList[i]);
                }
                else
                {
                    textureObject.GetComponent<UITexture>().mainTexture = icon.texture;
                }

            }


            //底色
            //if (i == GlobalPhotoData.Instance.mCurrentPhotoIndex)
            //{
            //    GameObject bg = ItemList[i].transform.FindChild( "BG" ).gameObject;
            //    if (bg != null)
            //    {
            //        bg.SetActive( true );
            //    }
            //}
        }
        UpdateScrollBarState();
        //调整图标和ScrollBar的位置
        int globalIndex = GlobalPhotoData.Instance.mCurrentPhotoIndex;
        int indexAtPage = (globalIndex / ColumnsPerPage) + 1;
        if (indexAtPage > 2)
        {
            int movePageNum = indexAtPage - 2;
            CurrentPageIndex = movePageNum;
            Vector3 tempPos = new Vector3();
            float alpha = 1.0f;
            bool active = true;
            int tempPage = 0;
            for (int i = 0; i < itemCount; i++)
            {
                tempPos.x = ItemList[i].transform.localPosition.x;
                tempPos.y = ItemList[i].transform.localPosition.y + PageHeight * movePageNum;
                tempPos.z = 50.0f;

                tempPage = i / ColumnsPerPage;
                if ((tempPage == (CurrentPageIndex + 1)) || (tempPage == CurrentPageIndex))
                {
                    tempPos.z -= 50.0f;
                    alpha = 1.0f;
                    active = true;
                }
                else
                {
                    alpha = 0.0f;
                    active = false;
                }
                int itemColumnIndexInPage = i % ColumnsPerPage;
                if (itemColumnIndexInPage == 0 || itemColumnIndexInPage == 3)
                {
                    tempPos.z -= 9.0f;
                }
                ItemList[i].transform.localPosition = tempPos;
                ItemList[i].GetComponent<PosterButton>().OriginPos = tempPos;
                ItemList[i].GetComponent<UIWidget>().alpha = alpha;
                ItemList[i].SetActive(active);
            }
            
            if (ScrollBarLeft != null && ScrollBarRight != null)
            {
                Vector3 barPos = new Vector3();
                barPos = ScrollBarLeft.transform.localPosition;
                barPos.y = barPos.y - ScrollBarLeft.height * movePageNum;

                ScrollBarLeft.transform.localPosition = barPos;
                ScrollBarRight.transform.localPosition = barPos;
            }
        }
        //等图全下完了才能点
        MakeAllItemsDisabled();
        if (0 == mHomeScreen.GetDownloadingIconNum())
        {
            if (HomeScreen.OnAllIconDownloaded != null)
            {
                HomeScreen.OnAllIconDownloaded(true);
            }
        }
    }

    #endregion

    private void UpdateScrollBarState()
    {
        if (ScrollBarLeft == null || ScrollBarRight == null)
        {
            return;
        }

        if (PageCount == 1 || PageCount == 2)
        {
            //ScrollBarLeft.alpha = ScrollBarRight.alpha = 0.0f;
            ScrollBarLeft.transform.parent.gameObject.SetActive(false);
            ScrollBarRight.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            //ScrollBarLeft.alpha = ScrollBarRight.alpha = 1.0f;
            ScrollBarLeft.height = ScrollBarRight.height = System.Convert.ToInt32(mScrollLeftBg.height / (PageCount - 1));
            mScrollLeftBg.height = ScrollBarLeft.height * (PageCount - 1);
            mScrollRightBg.height = ScrollBarRight.height * (PageCount - 1);
            ScrollBarLeft.transform.localPosition = new Vector3(0, mScrollLeftBg.height / 2.0f, 0);
            ScrollBarRight.transform.localPosition = new Vector3(0, mScrollRightBg.height / 2.0f, 0);

            ScrollBarLeft.transform.parent.gameObject.SetActive(true);
            ScrollBarRight.transform.parent.gameObject.SetActive(true);
        }
    }

    private void AdjustScrollBarPosition(PageDirection direction)
    {
        if (ScrollBarLeft != null && ScrollBarRight != null)
        {
            if (direction == PageDirection.Next)
            {
                ScrollBarLeft.transform.DOLocalMoveY(ScrollBarLeft.transform.localPosition.y - ScrollBarLeft.height, PageMoveDuration, false);
                ScrollBarRight.transform.DOLocalMoveY(ScrollBarRight.transform.localPosition.y - ScrollBarRight.height, PageMoveDuration, false);
            }
            else
            {
                ScrollBarLeft.transform.DOLocalMoveY(ScrollBarLeft.transform.localPosition.y + ScrollBarLeft.height, PageMoveDuration, false);
                ScrollBarRight.transform.DOLocalMoveY(ScrollBarRight.transform.localPosition.y + ScrollBarRight.height, PageMoveDuration, false);
            }
        }
    }

    private void ResetScrollBarState()
    {
        if (ScrollBarLeft != null && ScrollBarRight != null)
        {
            ScrollBarLeft.transform.localPosition = mOriginBarPos;
            ScrollBarRight.transform.localPosition = mOriginBarPos;

            ScrollBarLeft.height = ScrollBarRight.height = mOriginBarHeight;
        }
    }

    private void InitItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            ItemList.Add(InstantiateItemByIndex(i));
            UIEventListener.Get(ItemList[i]).onClick += OnClick;
            UIEventListener.Get(ItemList[i]).onHover += OnHover;
        }
    }

    void OnClick(GameObject obj)
    {
        Debug.Log("OnClick : " + obj.name);
    }

    void OnHover(GameObject obj, bool isHover)
    { }

    private GameObject InstantiateItemByIndex(int index)
    {
        GameObject go = GameObject.Instantiate((Object)ItemTemplate, this.gameObject.transform.position, this.gameObject.transform.rotation) as GameObject;
        PosterButton btn = go.GetComponent<PosterButton>();
        go.name = "item_" + index;
        go.transform.parent = this.gameObject.transform;
        go.transform.localScale = this.gameObject.transform.localScale;
        int itemWidth = ItemTemplate.GetComponent<UIWidget>().width;
        int itemHeight = ItemTemplate.GetComponent<UIWidget>().height;
        int itemColumnIndexInPage = index % ColumnsPerPage;
        go.transform.localPosition = CalculateItemPositionByIndex(index);
        go.transform.localRotation = CalculateItemRotationByIndex(index);
        if (index / ColumnsPerPage != 0 && index / ColumnsPerPage != 1)
        {
            go.transform.GetComponent<UIWidget>().alpha = 0;
            go.SetActive(false);
        }
        else
        {
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z - 50.0f);
            //go.GetComponentInChildren<UITexture>().depth = go.GetComponentInChildren<UITexture>().depth + 1;
        }

        if (index == 0)
        {
            PageWidth = ColumnsPerPage * itemWidth + (ColumnsPerPage - 1) * HorizontalSpacing;
            PageHeight = itemHeight * 1 + VerticalSpacing + PageSpacing;
        }
        btn.OriginPos = go.transform.localPosition;
        //UILabel label = go.transform.FindChild("Label").GetComponent<UILabel>();
        //labelColor = label.color;
        return go;
    }

    private Vector3 CalculateItemPositionByIndex(int index)
    {
        Vector3 position = Vector3.zero;
        int itemWidth = ItemTemplate.GetComponent<UIWidget>().width;
        int itemHeight = ItemTemplate.GetComponent<UIWidget>().height;

        int pageIndex = index / (ColumnsPerPage * RowsPerPage);
        int itemColumnIndexInPage = index % ColumnsPerPage;


        float y = 0.0f;
        if (index / ColumnsPerPage == 0)
        {
            y = BasePosition.y - (itemHeight + VerticalSpacing) * (index / ColumnsPerPage);
        }
        else
        {
            float up = ItemList[index - ColumnsPerPage].transform.localPosition.y;
            y = up - (itemHeight + VerticalSpacing);
        }


        float offsetZ = 0.0f;
        if (itemColumnIndexInPage == 0 || itemColumnIndexInPage == 3)
        {
            offsetZ = -9.0f;
        }


        position = new Vector3(BasePosition.x + (index % ColumnsPerPage) * (itemWidth + HorizontalSpacing),
                       y,
                       BasePosition.z + offsetZ);

        return position;
    }

    private Quaternion CalculateItemRotationByIndex(int index)
    {
        int itemColumnIndexInPage = index % ColumnsPerPage;
        Quaternion quaternion = new Quaternion();
        //int itemInclinedAngel = -AngelDelta * (ColumnsPerPage / 2) + itemColumnIndexInPage * AngelDelta;

        float angleY = 0.0f;
        if (itemColumnIndexInPage == 0)
        {
            angleY = -20.0f;
        }
        else if (itemColumnIndexInPage == 3)
        {
            angleY = 20.0f;
        }
        quaternion.eulerAngles = new Vector3(0, angleY, 0);
        return quaternion;
    }

    void RequestNextPage()
    {
        HomeScreen.OnPageRequestFinished += OnPageRequestFinished_Home;
        mPageTransiting = true;
        HomeScreen.OnAllIconDownloaded += OnAllIconDownloaded_Home;
        if (mHomeScreen != null)
        {
            int page = ActualPageCount + 1;
            mHomeScreen.SendNextPageRequest(page.ToString());
        }
    }

    void OnPageRequestFinished_Home(bool success = false)
    {
        HomeScreen.OnPageRequestFinished -= OnPageRequestFinished_Home;

        if (success)
        {
            ResetAcceptKeyEvent();
            TransitPage(PageDirection.Next, true);
        }
        else
        {
            WingToastManager.Instance.Show(Localization.Get("Home_NoNet"));

            HomeScreen.OnAllIconDownloaded -= OnAllIconDownloaded_Home;
            Invoke("ResetAcceptKeyEvent", 0.1f);
            Invoke("UpdateEventResponseForAllItems", 0.1f);
        }
    }

    void OnAllIconDownloaded_Home(bool success = false)
    {
        HomeScreen.OnAllIconDownloaded -= OnAllIconDownloaded_Home;
        if (mPageTransiting = true)
        {
            mPageTransiting = false;
            Invoke("ResetAcceptKeyEvent", Mathf.Max(PageMoveDuration, FadeAnimationDuration) + 0.1f);
            Invoke("UpdateEventResponseForAllItems", Mathf.Max(PageMoveDuration, FadeAnimationDuration));
        }
        else
        {
            Invoke("ResetAcceptKeyEvent", 0.1f);
            Invoke("UpdateEventResponseForAllItems", 0.1f);
        }
    }

    private void TransitPage(PageDirection direction, bool isCallback = false)
    {
        if (AcceptKeyEvent)
        {
            if (direction == PageDirection.Previous)
            {
                if (CurrentPageIndex == 0)
                {
                    Debug.Log("Reached the first page!");
                    WingToastManager.Instance.Show(Localization.Get("Bar_First"));
                    return;
                }
            }
            else
            {
                if (CurrentPageIndex >= (PageCount - 2))
                {
                    Debug.Log("Reached last page!");
                    WingToastManager.Instance.Show(Localization.Get("Bar_Last"));
                    return;
                }
            }

            AcceptKeyEvent = false;
            if (direction == PageDirection.Previous)
            {
                CurrentPageIndex--;
            }
            else
            {
                CurrentPageIndex++;
                if (CurrentPageIndex >= ActualPageCount - 1)
                {
                    if (CurrentPageIndex > ActualPageCount - 1)
                    {
                        Debug.LogError("TransitPage something error");
                    }
                    //请求新数据
                    CurrentPageIndex--;
                    //只有当请求完成才能继续操作
                    //AcceptKeyEvent = true;
                    MakeAllItemsDisabled();
                    RequestNextPage();
                    return;
                }
            }
            MakeAllItemsDisabled();
            foreach (GameObject obj in ItemList)
            {
                ItemMove(obj, direction);
            }
            //检查贴图
            if (isCallback == false)
            {
                mPageTransiting = true;
                HomeScreen.OnAllIconDownloaded += OnAllIconDownloaded_Home;
                mHomeScreen.ResetDownloadCounting();
                int retIndex = 0;
                foreach (GameObject obj in ItemList)
                {
                    retIndex = ItemTextureCheck(obj, direction);
                    if (retIndex >= 0)
                    {
                        mHomeScreen.DownloadIcon_Index(retIndex, obj);
                    }
                }
            }
            AdjustScrollBarPosition(direction);
            if (0 == mHomeScreen.GetDownloadingIconNum())
            {
                if (HomeScreen.OnAllIconDownloaded != null)
                {
                    HomeScreen.OnAllIconDownloaded(true);
                }
            }
        }
    }

    /// <summary>
    /// Make all items disabled.This method is called before items move.
    /// If we don't call this method before items move,when items move away
    /// out of visible area,it can also receive hover event and play sound.
    /// </summary>
    private void MakeAllItemsDisabled()
    {
        foreach (GameObject obj in ItemList)
        {
            obj.GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// Even items are invisible in the panel,they can receive hover event and play sound.
    /// So we need to disable their collider when items are invisible.
    /// </summary>
    private void UpdateEventResponseForAllItems()
    {
        foreach (GameObject obj in ItemList)
        {
            obj.GetComponent<BoxCollider>().enabled = true;

            //bool isVisible = MainPanel.IsVisible(obj.GetComponent<UIWidget>());
            //if (isVisible)
            //{
            //    //                log("Visible item:" + obj.name);
            //    obj.GetComponent<BoxCollider>().enabled = true;
            //}
            //else
            //{
            //    obj.GetComponent<BoxCollider>().enabled = false;
            //}
        }
    }

    int ItemTextureCheck(GameObject obj, PageDirection direction)
    {
        int retIndex = -1;
        int index = int.Parse(obj.name.Split('_')[1]);

        bool needCheck = false;
        if (index / ColumnsPerPage == CurrentPageIndex)
        {
            needCheck = true;
        }
        else if (index / ColumnsPerPage == CurrentPageIndex + 1)
        {
            needCheck = true;
        }

        if (needCheck)
        {
            PosterButton poster = obj.GetComponent<PosterButton>();
            GameObject textureObject = obj.transform.Find("texture").gameObject;
            if (textureObject != null)
            {
                UITexture icon = textureObject.GetComponent<UITexture>();

                if (icon.mainTexture == poster.mDefaultTexture)
                {
                    //需要下载新图
                    retIndex = index;
                }
            }
        }


        return retIndex;
    }

    private void ItemMove(GameObject obj, PageDirection direction)
    {
        if (obj != null)
        {
            int index = int.Parse(obj.name.Split('_')[1]);
            PosterButton btn = obj.GetComponent<PosterButton>();
            if (direction == PageDirection.Previous)
            {
                if (index / ColumnsPerPage == CurrentPageIndex)
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y - PageHeight, obj.transform.localPosition.z - 50.0f);
                    obj.transform.DOLocalMoveZ(obj.transform.localPosition.z - 50.0f, PageMoveDuration, false);
                    //obj.transform.DOLocalMoveY(obj.transform.localPosition.y - PageHeight - 5, PageMoveDuration, false);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y - PageHeight, PageMoveDuration, false);
                    TweenAlpha.Begin(obj, PageMoveDuration, 1f);
                    obj.SetActive(true);
                }
                else if (index / ColumnsPerPage == CurrentPageIndex + 2)
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y - PageHeight, obj.transform.localPosition.z + 50.0f);
                    obj.transform.DOLocalMoveZ(obj.transform.localPosition.z + 50.0f, PageMoveDuration, false);
                    //obj.transform.DOLocalMoveY(obj.transform.localPosition.y - PageHeight - 5, PageMoveDuration, false);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y - PageHeight, PageMoveDuration, false);
                    TweenAlpha.Begin(obj, PageMoveDuration, 0f);
                    StartCoroutine(DelayToSetActionFalse(obj));
                }
                else if (index / ColumnsPerPage == CurrentPageIndex + 1)
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y - PageHeight, obj.transform.localPosition.z);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y - PageHeight, PageMoveDuration, false);
                    obj.SetActive(true);
                }
                else
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y - PageHeight, obj.transform.localPosition.z);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y - PageHeight, PageMoveDuration, false);
                    StartCoroutine(DelayToSetActionFalse(obj));
                }
            }
            else
            {
                if (index / ColumnsPerPage == (CurrentPageIndex - 1))
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y + PageHeight, obj.transform.localPosition.z + 50.0f);
                    obj.transform.DOLocalMoveZ(obj.transform.localPosition.z + 50.0f, PageMoveDuration, false);
                    //obj.transform.DOLocalMoveY(obj.transform.localPosition.y + PageHeight + 5, PageMoveDuration, false);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y + PageHeight, PageMoveDuration, false);
                    TweenAlpha.Begin(obj, PageMoveDuration, 0f);
                    StartCoroutine(DelayToSetActionFalse(obj));
                }
                else if (index / ColumnsPerPage == (CurrentPageIndex + 1))
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y + PageHeight, obj.transform.localPosition.z - 50.0f);
                    obj.transform.DOLocalMoveZ(obj.transform.localPosition.z - 50.0f, PageMoveDuration, false);
                    //obj.transform.DOLocalMoveY(obj.transform.localPosition.y + PageHeight + 5, PageMoveDuration, false);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y + PageHeight, PageMoveDuration, false);
                    TweenAlpha.Begin(obj, PageMoveDuration, 1f);
                    obj.SetActive(true);
                }
                else if (index / ColumnsPerPage == CurrentPageIndex)
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y + PageHeight, obj.transform.localPosition.z);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y + PageHeight, PageMoveDuration, false);
                    obj.SetActive(true);
                }
                else
                {
                    btn.OriginPos = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y + PageHeight, obj.transform.localPosition.z);
                    obj.transform.DOLocalMoveY(obj.transform.localPosition.y + PageHeight, PageMoveDuration, false);
                    StartCoroutine(DelayToSetActionFalse(obj));
                }
            }

        }
    }

    IEnumerator DelayToSetActionFalse(GameObject obj)
    {
        yield return new WaitForSeconds(PageMoveDuration);
        obj.SetActive(false);
    }

    private void ResetAcceptKeyEvent()
    {
        AcceptKeyEvent = true;
    }

    public void UpdateItemPick(GameObject go)
    {
        Debug.Log("UpdateItemPick : " + go.name);
        GameObject bg = go.transform.Find("BG").gameObject;
        if (bg != null)
        {
            bg.SetActive(true);
        }
    }
}