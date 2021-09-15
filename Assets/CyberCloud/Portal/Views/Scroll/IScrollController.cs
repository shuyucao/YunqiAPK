using UnityEngine;
using System.Collections;


/// <summary>
/// 滑动控制器 
/// </summary>
public interface IScrollController
{
    /// <summary>
    /// 滚动视图中的数据个数
    /// </summary>
    /// <returns></returns>
    int SCDataCount();

    /// <summary>
    /// 获取滚动视图中的指定序号上的数据
    /// </summary>
    /// <param name="dataIndex"></param>
    /// <returns></returns>
    object SCGetData(int dataIndex);


    /// <summary>
    /// 滚动视图滚动时数据刷新
    /// </summary>
    /// <param name="go"></param>
    /// <param name="wrapIndex"></param>
    /// <param name="realIndex"></param>
    void SCRowItemDataUpdate(GameObject go, int wrapIndex, int realIndex);


    /// <summary>
    /// 获取更多的数据 (实现接口类时，如果没有获取更多数据的需求，方法体应为空)
    /// </summary>
    void SCGetMoreDatas();


    /// <summary>
    /// 获取更多数据成功后 (请求数据响应成功并解析添加完成后调用)
    /// </summary>
    void SCOnGetMoreDatasSuccess();



}
