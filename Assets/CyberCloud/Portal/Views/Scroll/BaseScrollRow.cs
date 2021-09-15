using UnityEngine;
using System.Collections;

/// <summary>
/// 列表项（行项，含多个列项）
/// </summary>
public class BaseScrollRow : MonoBehaviour
{
    private IScrollController mController;

    [SerializeField]
    private BaseScrollItem[] mScrollItems;

    public int mRowIndex;

    private int mRowCount;


    public void Init(IScrollController controller)
    {
        if (mScrollItems == null || mScrollItems.Length <= 0)
        {
            Debug.LogError("mScrollItems is null !!!");
            return;
        }
        mController = controller;
        foreach (var item in mScrollItems)
        {
            item.Init(mController);
        }
        mRowCount = mScrollItems.Length;
    }


    //渲染数据增加延迟
    void startLoadDatas(int rowIndex)
    {

        for (int i = 0; i < mRowCount; i++)
        {
            
            mScrollItems[i].clear();
            mScrollItems[i].UpdateData(rowIndex * mRowCount + i);
      
        }
    }
    public void refreshRowItem(int pageindex) {
        for (int i = 0; i < mRowCount; i++)
        {
            mScrollItems[i].refreshItem(pageindex);     
        }
    }
    public virtual void UpdateData(int index)
    {
        //
        mRowIndex = index;
        if (mScrollItems == null || mScrollItems.Length <= 0)
        {
            Debug.LogError("mScrollItems is null !!!");
            return;
        }

        //int indexfrom0 = index>0?index - 1:0;

        if (index < mController.SCDataCount())
        {
            gameObject.SetActive(true);
       
            startLoadDatas(index);
        }
        else
        {
            //Debug.LogError("mScrollItems 555");
            gameObject.SetActive(false);
   
        }

    }


}
