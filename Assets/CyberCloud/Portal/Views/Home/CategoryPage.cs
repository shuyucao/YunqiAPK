using Com.PicoVR.Gallery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryPage : MonoBase
{
    public CategoryBtnClick CategoryClick = null;
    [SerializeField]
    //UISpecialGrid mGrid;
    UIGrid mGrid;
    [SerializeField]
    GameObject mCategoryOri;

    private List<CategoryModel> mCategoryList = null;
    private Dictionary<string, CategoryItem> mItemDict = new Dictionary<string, CategoryItem>();

    public void FillData()
    {
        Debug.Log("！！！！   CategoryPage FillData  mCategoryList " + CachePhotoData.Instance.CategoryList.Count);
        if (mCategoryList == null)
        {
            mCategoryList = CachePhotoData.Instance.CategoryList;
            //StartCoroutine(CeateCategoryBtn());
        }
        else
        {
            Debug.LogError("the category has inited!");
        }
    }

    private IEnumerator CeateCategoryBtn()
    {
        if (mCategoryOri == null)
        {
            Debug.LogError("the origin is null!");
            yield return null;
        }
        for (int i = 0; i < mCategoryList.Count; i++)
        {
            CategoryItem btn = UnityTools.CreateComptent<CategoryItem>(mCategoryOri, mGrid.transform);
            mGrid.AddChild(btn.transform);
            mGrid.repositionNow = true;

            if (mItemDict.ContainsKey(mCategoryList[i].CategoryID))
            {
                mItemDict.Remove(mCategoryList[i].CategoryID);
            }
            mItemDict.Add(mCategoryList[i].CategoryID, btn);
            //btn.Init(mCategoryList[i], CategoryClick);
            btn.Init(mCategoryList[i]);
            yield return new WaitForEndOfFrame();
        }
    }

    public void Switch(CategoryModel model)
    {
        CategoryItem itm;
        if (mItemDict.TryGetValue(HomePageScreen.CurrentID, out itm))
        {
            itm.Switch(false);
        }
        mItemDict.TryGetValue(model.CategoryID, out itm);
        if (itm != null)
        {
            itm.Switch(true);
        }
    }
}
