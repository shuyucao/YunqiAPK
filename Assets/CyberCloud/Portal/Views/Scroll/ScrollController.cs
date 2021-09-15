using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 列表控制模块
/// </summary>
public class ScrollController : MonoBehaviour,IScrollController
{
    public delegate bool OnGetMoreData();
    public GameObject prefab;

    public UISprite btnUp;

    public UISprite btnDown;

    public BtnGroup btnGroup;

    private bool isFirstTop = false;

    public OnGetMoreData onGetMoreData;

    [SerializeField]
    private UIScrollView mScrollView;

    [SerializeField]
    private UIWrapContent mWrapContent;

    [SerializeField]
    private float mSpeeder = 40;
   
    /// <summary>
    /// 是否回弹
    /// </summary>
    [SerializeField]
    private bool isSpringBack = true;

    /// <summary>
    /// 有多少列
    /// </summary>
    [SerializeField]
    private int columnCount = 4;
    [SerializeField]
    public List<BaseScrollRow> mScrollRows;
    [SerializeField]
    private UIScrollBar mScrollBar;

    /// <summary>
    /// 滑动速度
    /// </summary>
    [SerializeField]
    private float strength = 10;

    private float pageHeight = 0;

    //private List<BaseScrollRow> mScrollRowList;

    Vector2 mStartOffset;

    private Vector3 mStartPos;
    private Vector3 mToPos;

    private float maxY;
    private float minY;

    private List<BaseData> mDataList;

    int indexCur = 0;

    bool initView = false;


    void Awake()
    {
        if(mWrapContent != null && mScrollView.movement == UIScrollView.Movement.Vertical)
        {
            mWrapContent.maxIndex = 0;  /**竖直循环滚动中，index值为负，所以maxIndex为零*/
        }
        gameObject.SetActive(false);
        mScrollBar.gameObject.SetActive(false);
        //SliderController.OnUp += OnUp;
        //SliderController.OnDown += OnDown;

        //SliderController.OnDown_Whole += OnUp_Whole;
        //SliderController.OnUp_Whole += OnDown_Whole;
        //InputManager.OnUp += OnDown_Whole;
        //InputManager.OnDown += OnUp_Whole;

        //btnDown.GetComponent<GazeButton>().action += OnDown_Whole;
        //btnUp.GetComponent<GazeButton>().action += OnUp_Whole;
        UIEventListener.Get(btnDown.gameObject).onClick += (GameObject obj) => { OnDown_Whole(); };
        UIEventListener.Get(btnUp.gameObject).onClick += (GameObject obj) => { OnUp_Whole(); };

        if (btnGroup == null)
                    {
            GameObject go = Resources.Load("UI/BtnGroup") as GameObject;
            GameObject obj = Instantiate(go);
            obj.transform.parent = this.transform;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localPosition = new Vector3(-10f, 0f, -109f);
            obj.transform.localScale = Vector3.one;
            obj.name = "BtnGroup";
            btnGroup = this.transform.Find("BtnGroup").gameObject.GetComponent<BtnGroup>();
        }
        btnDown.gameObject.SetActive(false);
        btnUp.gameObject.SetActive(false);
        btnGroup.onClickUpNotify += OnDown_Whole;
        btnGroup.onClickDownNotify += OnUp_Whole;
        btnGroup.onClickTopNotify += OnTop_Whole;
        mToPos = mScrollView.transform.localPosition;
        pageHeight = mScrollRows[0].GetComponent<UIWidget>().height * 2;// (mScrollRows.Count - 2);
    }

    // Use this for initialization
    void Start ()
    {
        //Init();
        //gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
     

	}


    /// <summary>
    /// 按“下”
    /// </summary>
    void OnDown()
    {
        if (!gameObject.activeInHierarchy) return;

        mToPos = new Vector3(mScrollView.transform.localPosition.x, mScrollView.transform
                                  .localPosition.y + mSpeeder, mScrollView.transform.localPosition.z);
        if(isSpringBack)
        {
            //maxY += mWrapContent.itemSize * 0.5f;
            if (mToPos.y > maxY + mWrapContent.itemSize * 0.5f) mToPos.y = maxY + mWrapContent.itemSize * 0.5f;
        }
        else
        {
            if (mToPos.y > maxY)
            {
                mToPos.y = maxY;
            }
        }
        RefreshCurIndex(mToPos.y);
        SpringPanel.Begin(mScrollView.gameObject, mToPos, strength).onFinished = () =>
        {
            //if (!isCanGetNewData) { return; }
            if (mScrollRows.Count >= SCDataCount())
            {
                mToPos.y = mStartPos.y + (SCDataCount() - 2) * mWrapContent.itemSize;
                if(mToPos.y < mStartPos.y)
                {
                    mToPos.y = mStartPos.y;
                }
                SpringPanel.Begin(mScrollView.gameObject, mToPos, strength);
            }
            else if(mScrollView.transform.localPosition.y >= maxY)
            {
                mToPos.y = maxY;
                SpringPanel.Begin(mScrollView.gameObject, mToPos, strength);
            }
        };
    }

    /// <summary>
    /// 按“上”
    /// </summary>
    void OnUp()
    {
        if (!gameObject.activeInHierarchy) return;

        mToPos = new Vector3(mScrollView.transform.localPosition.x, mScrollView.transform
                                  .localPosition.y - mSpeeder, mScrollView.transform.localPosition.z);
        if (isSpringBack)
        {
            if(mToPos.y < minY - mWrapContent.itemSize * 0.5f)
            {
                mToPos.y = minY - mWrapContent.itemSize * 0.5f;
            }
        }
        else
        {
            if (mToPos.y < minY)
                mToPos.y = minY;
        }
        RefreshCurIndex(mToPos.y);
        SpringPanel.Begin(mScrollView.gameObject, mToPos, strength).onFinished = () =>
        {
            if (mScrollView.transform.localPosition.y <= mStartPos.y || mWrapContent.minIndex == -1)
            {
                mToPos.y = mStartPos.y;
                SpringPanel.Begin(mScrollView.gameObject, mToPos, strength);
            }
        } ;
    }


    void OnDown_Whole()
    {
        //Debug.LogError("OnDown_Whole");
        if (!gameObject.activeInHierarchy) return;
        //mScrollBar.gameObject.SetActive(true);
        mScrollBar.value -= 2 * (mScrollBar.barSize);
        mToPos = new Vector3(mToPos.x, mToPos.y - pageHeight, mToPos.z);
        if (mToPos.y > maxY)
        {
            mToPos.y = maxY;
            mScrollBar.value = 1.0f;
        }

        if (mToPos.y < minY)
        {
            mToPos.y = minY;
            mScrollBar.value = 0.0f;
        }
        RefreshCurIndex(mToPos.y);
        SpringPanel.Begin(mScrollView.gameObject, mToPos, strength).onFinished = OnFinishMove;
  
        if (pageIndex>0)
        {
            pageIndex = pageIndex - 1;
            refreshRowItem(pageIndex);
        }
    }
    /// <summary>
    /// 向下翻页
    /// </summary>
    void OnUp_Whole()
    {
        //Debug.LogError("OnUp_Whole");
        if (!gameObject.activeInHierarchy) return;
        //mScrollBar.gameObject.SetActive(true);
        mScrollBar.value += 2 * (mScrollBar.barSize);
        mToPos = new Vector3(mToPos.x, mToPos.y + pageHeight, mToPos.z);
       // MyTools.PrintDebugLog("ucvr mToPos.y:"+ mToPos.y+ ";maxY:"+ maxY);
        if (mToPos.y > maxY)
        {
            mToPos.y = maxY;
            mScrollBar.value = 1.0f;
        }

        if (mToPos.y < minY)
        {
            mToPos.y = minY;
            mScrollBar.value = 0.0f;
        }
        RefreshCurIndex(mToPos.y);
        SpringPanel.Begin(mScrollView.gameObject, mToPos, strength).onFinished = OnFinishMove;
        int pageNums = SCDataPageCount(SCDataCount());// (int)Mathf.Ceil( SCDataCount() / 2f);
        if (pageIndex < pageNums) {
            pageIndex = pageIndex + 1;
            refreshRowItem(pageIndex);
        }
    }
    public static  int pageIndex = 0;
    /// <summary>
    /// 返回顶部
    /// </summary>
    public void OnTop_Whole()
    {
        //Debug.LogError("OnTop_Whole");
        pageIndex = 0;
        indexCur = Mathf.RoundToInt((mToPos.y - minY) / mWrapContent.itemSize);
        mToPos = new Vector3(mToPos.x, mToPos.y - pageHeight *strength, mToPos.z);
        //mScrollBar.gameObject.SetActive(true);
        mScrollBar.value -= 2*strength * (mScrollBar.barSize);
        if (mToPos.y > maxY)
        {
            mToPos.y = maxY;
            mScrollBar.value = 1.0f;
        }

        if (mToPos.y < minY)
        {
            mToPos.y = minY;
            mScrollBar.value = 0.0f;
            btnGroup.isTopMove = false;
        }
        RefreshCurIndex(mToPos.y);
        SpringPanel.Begin(mScrollView.gameObject, mToPos, strength).onFinished = OnFinishMove;
        refreshRowItem(pageIndex);
    }
    // move finish CallBack for list moveUp and moveDown
    public void OnFinishMove()
    {
        mScrollBar.gameObject.SetActive(false);
        if(btnGroup.isTopMove)
        {
            OnTop_Whole();
        }
    }

    bool istest = false;
    /// <summary>
    /// 初始化数据（第一次传入数据时调用， isReset为True; 刷新数据时如果需要回到第一行调用，isReset为True，否则调用 RefreshScrollView）
    /// </summary>
    /// <param name="list">数据</param>
    /// <param name="isReset"> 是否重置（回到顶部） </param>
    public void InitDataList(List<BaseData> list, bool isReset = false)
    {
        //return;
        mDataList = list;
      
        //
        //显示内容区域
        gameObject.SetActive(true);
        //Debug.LogError("this is mdatalist:::" + mDataList.Count);
        //return;
        Init();
     
        //根据行数初始化显示翻页按钮
        btnGroup.Init((SCDataCount() <= 2 ? true : false), BtnGroup.GroupType.GT_GAMELIST);
        mScrollBar.barSize = 1.0f / SCDataCount();
        mScrollBar.value = 0.0f;
        if (mDataList == null || mDataList.Count <= 0)
        {
            MyTools.PrintDebugLogError("ucvr app list is null");
            mScrollView.gameObject.SetActive(false);
            return;
        }
        else
            mScrollView.gameObject.SetActive(true);
   
       mWrapContent.minIndex = 0 - SCDataCount() + 1;
        SetMaxYAndMinY();
        if (mWrapContent.minIndex >= 0)
        {
            mWrapContent.minIndex = -1;
        }
        mWrapContent.onInitializeItem = SCRowItemDataUpdate;
        mWrapContent.onScrollEndItem = SCGetMoreDatas;
        if (isReset)
        {
            //return;
            //mScrollView.ResetPosition();
            mScrollView.transform.localPosition = mStartPos;
            mScrollView.GetComponent<UIPanel>().clipOffset = mStartOffset;
            mWrapContent.SortBasedOnScrollMovement();
            mWrapContent.UpdateAllItem();
            indexCur = 0;
            //Debug.LogError("this mStartPos"+ mStartPos);
            mToPos = mStartPos;
            isFirstTop = true;
            RefreshCurIndex(mToPos.y);
        }
        else
        {
            //Debug.LogError("this is222");
            startLoadDatas();
            //StartCoroutine(startLoadDatas());
        }
    }
    public int SCDataPageCount(int rownums)
    {
        int count = 0;
        if (rownums > 0)
        {
            count = rownums / 2;

            if (rownums % 2 > 0)
                count += 1;

        }
        MyTools.PrintDebugLog("ucvr pageNums:"+count);
        return count;
    }
    //渲染没一行数据
    void startLoadDatas() {

        for (int i = 0, length = mScrollRows.Count; i < length; i++)
        {
            mScrollRows[i].UpdateData(mScrollRows[i].mRowIndex);
        }
    }
    public void refreshRowItem(int pageindex)
    {
        for (int i = 0, length = mScrollRows.Count; i < length; i++)
        {
            mScrollRows[i].refreshRowItem(pageindex);
      
        }
    }
        /// <summary>
        ///  记录scrollView等的初始化信息
        /// </summary>
        void Init()
    {
        //return;
        if(!initView)
        {
            initView = true;
            mScrollView.GetComponent<UIPanel>().onClipMove += OnMove;
            mStartPos = mScrollView.transform.localPosition;

            mStartOffset = mScrollView.GetComponent<UIPanel>().clipOffset;
            if (mScrollRows == null )
            {
                mScrollRows = new List<BaseScrollRow>();
            }
          
        }
        int minindex = mScrollRows.Count;
       
        if (minindex < SCDataCount())
        {
            for (int i = minindex; i < SCDataCount() ; i++)
            {//创建预设
                GameObject obj = Instantiate(prefab);

                obj.transform.parent = this.mWrapContent.transform;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localPosition = new Vector3(0, i, 0);
                obj.transform.localScale = Vector3.one;
                mScrollRows.Add(obj.GetComponent<BaseScrollRow>());
            }
        }
        for (int i = 0, length = mScrollRows.Count; i < length; i++)
        {
            mScrollRows[i].Init(this);
            mScrollRows[i].gameObject.SetActive(false);
        }
 
    }
    /// <summary>
    /// 计算最大Y值和最小Y值
    /// </summary>
    private void SetMaxYAndMinY()
    {        
        maxY = mScrollRows.Count*mWrapContent.itemSize + mStartPos.y;// (Mathf.Abs(mWrapContent.minIndex) + 1 - (mScrollRows.Count -2 )) * mWrapContent.itemSize + mStartPos.y;
        minY = mStartPos.y;

        if (mScrollBar != null)
        {

        }
    }


    void OnMove(UIPanel panel)
    {
        if(indexCur >= SCDataCount() - 4)
        {
            SCGetMoreDatas();
        }

    }


    /// <summary>
    /// 刷新列表  （数据刷新时调用，多为有删除数据时调用）
    /// </summary>
    public void RefreshScrollView()
    {
        mWrapContent.minIndex = 0 - SCDataCount() + 1;
        if (mWrapContent.minIndex >= 0)
        {
            mWrapContent.minIndex = -1;
        }
        SetMaxYAndMinY();
        if (indexCur > SCDataCount() - 2)
        {
            indexCur = SCDataCount() - 2;
        }
        if (indexCur < 0)
        {
            indexCur = 0;
        }
        Debug.Log("indexCur : " + indexCur);
        mToPos = new Vector3(mScrollView.transform.localPosition.x, mStartPos.y + indexCur * mWrapContent
                              .itemSize, mScrollView.transform.localPosition.z);
        SpringPanel.Begin(mScrollView.gameObject, mToPos, 20).onFinished = () =>
        {
             mWrapContent.UpdateAllItem();
        };
    }
    /// <summary>
    /// 计算当前滑到哪一行
    /// </summary>
    /// <param name="y"></param>
    private void RefreshCurIndex(float y)
    {
        indexCur = Mathf.RoundToInt((y - minY) / mWrapContent.itemSize);

        if(SCDataCount() <= 2)
        {
            btnUp.GetComponent<BoxCollider>().enabled = false;
            btnDown.GetComponent<BoxCollider>().enabled = false;
            btnDown.spriteName = "bt_up";
            btnUp.spriteName = "bt_down";
            btnDown.color = Color.gray;
            btnUp.color = Color.gray;
            return;
        }
        if(indexCur >= SCDataCount() - 2 )
        {
            if (isCanGetNewData)
            {
                indexCur = SCDataCount() - 2;
                //TipsPanel.GetInstance().ShowInfo(Localization.Get("Bar_Last"), Color.white);
                CommonAlert.Show("Bar_Last", false, null, false);
                btnUp.GetComponent<BoxCollider>().enabled = false;
                btnDown.GetComponent<BoxCollider>().enabled = true;
                btnUp.spriteName = "bt_down_disable";
                btnDown.spriteName = "bt_up";
                btnDown.color = Color.white;
                btnUp.color = Color.gray;
                btnGroup.RefreshActive(false, true);
            }
            else
            {
            }
           
        }
        else if(indexCur <= 0)
        {
            indexCur = 0;
            if(!isFirstTop)
            {
                //TipsPanel.GetInstance().ShowInfo(Localization.Get("Bar_First"), Color.white);
                CommonAlert.Show("Bar_First", false, null, false);
            }
            btnUp.GetComponent<BoxCollider>().enabled = true;
            btnDown.GetComponent<BoxCollider>().enabled = false;
            btnDown.spriteName = "bt_up_disable";
            btnUp.spriteName = "bt_down";
            btnDown.color = Color.gray;
            btnUp.color = Color.white;
            btnGroup.RefreshActive(true, false);
            isFirstTop = false;
        }
        else
        {
            btnUp.GetComponent<BoxCollider>().enabled = true;
            btnDown.GetComponent<BoxCollider>().enabled = true;
            btnDown.spriteName = "bt_up";
            btnUp.spriteName = "bt_down";
            btnDown.color = Color.white;
            btnUp.color = Color.white;
            btnGroup.RefreshActive(false, false);
        }

    }



    /// <summary>
    /// 滚动视图中的数据个数
    /// 获取行数
    /// </summary>
    /// <returns></returns>
    public int SCDataCount()
    {
        int count = 0;
        if(mDataList != null)
        {
            count = mDataList.Count / columnCount;
            //Debug.LogError("mDataList.Count:" + mDataList.Count + ";columnCount:" + columnCount);
            if (mDataList.Count % columnCount > 0)
                count += 1;
        }

        return count;
    }

    /// <summary>
    /// 获取滚动视图中的指定序号上的数据
    /// </summary>
    /// <param name="dataIndex"></param>
    /// <returns></returns>
    public object SCGetData(int dataIndex)
    {
        object data = null;
        if (mDataList != null)
        {
            if(dataIndex >= 0 && dataIndex < mDataList.Count)
            {
                data = mDataList[dataIndex];
            }
        }
        return data;
    }

    /// <summary>
    /// 是否可以调用获取数据  （滑到底部时只调用一次，在获取数据成功之前不可再次调用，亦可以加入其他的条件）
    /// </summary>
    bool isCanGetNewData = true;

    /// <summary>
    /// 获取更多数据成功后 (请求数据响应成功并解析添加完成后调用 可用委托事件的方式调用；例如wing里面的请求网络数据响应成功并解析后 )
    /// </summary>
    public void SCOnGetMoreDatasSuccess()
    {
        mWrapContent.minIndex = 0 - SCDataCount() + 1;
        if (mWrapContent.minIndex >= 0)
        {
            mWrapContent.minIndex = -1;
        }
        SetMaxYAndMinY();
        indexCur += 1;
        //RefreshScrollView();
        isCanGetNewData = true;

    }

    //IEnumerator GetNewDatas()
    //{
    //    yield return new WaitForSeconds(1f);
    //    controllerTest.DatasStructure();
    //}


    /// <summary>
    /// 获取更多的数据   （如：wing里面的发起网络请求数据）
    /// </summary>
    public void SCGetMoreDatas()
    {
        if (isCanGetNewData)
        {
            bool isGetMore = false;
           if(onGetMoreData != null)
            {
                isGetMore = onGetMoreData();
            }
            isCanGetNewData = !isGetMore;
            //isCanGetNewData = false;
        }
        
    }


    /// <summary>
    /// 滚动视图滚动时数据刷新
    /// </summary>
    /// <param name="go"></param>
    /// <param name="wrapIndex"></param>
    /// <param name="realIndex"></param>
    public void SCRowItemDataUpdate(GameObject go, int wrapIndex, int realIndex)
    {
        
        int rowDataIndex = Mathf.Abs(realIndex); /**竖直滑动时realIndex为负，所以取绝对值**/
        go.GetComponent<BaseScrollRow>().UpdateData(rowDataIndex);
        go.name = realIndex.ToString();
    }
}
