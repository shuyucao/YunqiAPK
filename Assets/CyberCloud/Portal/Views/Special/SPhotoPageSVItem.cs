using System.Collections;
using UnityEngine;

public class SPhotoPageSVItem : ScrolleItemBase
{
    public string ID { get; set; }
    private int mCurrentPage = 0;     // note:here a page is Constant.MaxLimitEachPage

    public void Init(string id)
    {
        base.InitBase();
        ID = id;
        Data.Style = PageStyle.PS_2R4C;
        Data.NumPerPage = 8;
    }

    private bool islock = false;
    public override void CreateItemList()
    {
        if (islock)
        {
            return;
        }
        StartCoroutine(CreatePageItems());
    }

    public IEnumerator CreatePageItems()
    {
        islock = true;
        if (mCurrentPage != 0)
        {
            yield return new WaitForSeconds(1f);
        }
        for (int i = 0; i < Constant.PageNumCreatOneTime; i++)
        {
            PageItemBase item = PageManager.Instance.CreateOnePageForSpecial(grid.transform, ID, Data);
            if (item != null)
            {
                grid.AddChild(item.transform);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                Debug.Log("CreateItemList   item   null !!!!" );
                break;
            }
            mCurrentPage++;
        }
        yield return new WaitForSeconds(0.6f);
        MsgManager.Instance.SendMsg(MsgID.RefreshCtrBar, null);
        islock = false;
    }
}