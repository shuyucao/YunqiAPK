using UnityEngine;
using System.Collections;

/// <summary>
/// 列表项（单个列项）
/// </summary>
public class BaseScrollItem : MonoBehaviour
{

    private IScrollController mController;

    private int itemDataIndex;

    protected object itemData;

    /// <summary>
    /// 初始化 滑动控制器
    /// </summary>
    /// <param name="controller"></param>
    public void Init(IScrollController controller)
    {
        mController = controller;
    }


    /// <summary>
    /// 数据刷新
    /// </summary>
    /// <param name="index"></param>
    public virtual void UpdateData(int index)
    {
        itemDataIndex = index;
        itemData = mController.SCGetData(itemDataIndex);
        gameObject.SetActive(itemData != null);
    }

    public virtual void clear()
    {
      
    }
    public virtual void refreshItem(int pageindex)
    {

    }

}
