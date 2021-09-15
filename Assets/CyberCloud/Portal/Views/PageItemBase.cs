using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIDragScrollView))]
[RequireComponent(typeof(BoxCollider))]
public class PageItemBase : MonoBase
{
    [SerializeField]
    ImageItemBase itemtemp;

    private List<Vector3> mcalPos = null;
    private List<Quaternion> mcalRotate = null;

    [HideInInspector]
    public List<ImageItemBase> ItemList = new List<ImageItemBase>();
    #region property
    private string id;
    public string ID
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    private int pagenum;
    public int PageNum
    {
        get
        {
            return pagenum;
        }
        set
        {
            pagenum = value;
        }
    }

    private int itemnum;
    public int ItemNum
    {
        get
        {
            return itemnum;
        }
        set
        {
            itemnum = value;
        }
    }

    public PageData Data;
    #endregion

    public void SetData(string category_id, int page_num, PageData pagedata, List<PhotoModel> datalist)
    {
        ID = category_id;
        PageNum = page_num;
        Data = pagedata;
        BoxCollider box = gameObject.GetComponent<BoxCollider>();
        box.size = new Vector3(pagedata.Width, pagedata.Height, 1);

        CalculatePosition();
        CreateImageItems(datalist);
    }


    // for new UI frameWork
    public void SetData(float w, float h)
    {
        Data = new PageData(w, h, 8, PageStyle.PS_2R4C);
        BoxCollider box = gameObject.GetComponent<BoxCollider>();
        box.size = new Vector3(Data.Width, Data.Height, 1);

        CalculatePosition();
        CreateImageItems();
    }

    //for local page set data
    public void SetData(PageData pagedata, List<LocalPhotoModel> datalist)
    {
        Data = pagedata;
        BoxCollider box = gameObject.GetComponent<BoxCollider>();
        box.size = new Vector3(pagedata.Width, pagedata.Height, 1);

        CalculatePosition();
        CreateImageItems(datalist);
    }

    public void SetData(PageData pagedata, List<ThemesModel> datalist)
    {
        Data = pagedata;
        BoxCollider box = gameObject.GetComponent<BoxCollider>();
        box.size = new Vector3(pagedata.Width, pagedata.Height, 1);

        CalculatePosition();
        CreateImageItems(datalist);
    }

    private void CreateImageItems(List<PhotoModel> datalist)
    {
        if (datalist != null && datalist.Count != 0)
        {
            for (int i = 0; i < datalist.Count; i++)
            {
                ImageItemBase item = UnityTools.CreateComptent<ImageItemBase>(itemtemp.gameObject, transform, mcalPos[i], mcalRotate[i % mcalRotate.Count], Vector3.one, "ImageItem_" + i);
                item.Init(datalist[i]);
                ItemList.Add(item);
            }
        }
    }

    private void CreateImageItems(List<LocalPhotoModel> datalist)
    {
        if (datalist != null && datalist.Count != 0)
        {
            for (int i = 0; i < datalist.Count; i++)
            {
                ImageItemBase item = UnityTools.CreateComptent<ImageItemBase>(itemtemp.gameObject, transform, mcalPos[i], mcalRotate[i % mcalRotate.Count], Vector3.one, "ImageItem_" + i);
                item.Init(datalist[i]);
                ItemList.Add(item);
            }
        }
    }

    private void CreateImageItems(List<ThemesModel> datalist)
    {
        if (datalist != null && datalist.Count != 0)
        {
            for (int i = 0; i < datalist.Count; i++)
            {
                ImageItemBase item = UnityTools.CreateComptent<ImageItemBase>(itemtemp.gameObject, transform, mcalPos[i], mcalRotate[i % mcalRotate.Count], Vector3.one, "ImageItem_" + i);
                item.Init(datalist[i]);
                ItemList.Add(item);
            }
        }
    }

    private void CreateImageItems()
    {
        for (int i = 0; i < Data.NumPerPage; i++)
        {
            ImageItemBase item = UnityTools.CreateComptent<ImageItemBase>(itemtemp.gameObject, transform, mcalPos[i], mcalRotate[i % mcalRotate.Count], Vector3.one, "ImageItem_" + i);
            item.gameObject.SetActive(false);
            ItemList.Add(item);
        }
    }

    [ContextMenu("Execute")]
    public void RePosition()
    {
        Debug.Log("RePosition");
        CalculatePosition();
    }

    private void CalculatePosition()
    {
        //if (mcalPos == null)
        //{
        //    mcalPos = StyleUtils.GetCalculatedPos(Data.Width, Data.Height, Data.Style);
        //}
        //if (mcalRotate == null)
        //{
        //    mcalRotate = StyleUtils.GetCalculatedRoatation(Data.Radii, Data.Style);
        //}
        mcalPos = StyleUtils.GetCalculatedPos(Data.Width, Data.Height, Data.Style);
        mcalRotate = StyleUtils.GetCalculatedRoatation(Data.Radii, Data.Style);
    }
}