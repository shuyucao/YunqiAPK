using Com.PicoVR.Gallery;
using UnityEngine;
using DG.Tweening;

public class CategoryItem : MonoBase, IMsgHandle
{
    //public CategoryBtnClick Click = null;
    [SerializeField]
    UILabel mLabel;
    //[SerializeField]
    //GameObject mBG;
    private string mCategoryID = null;
    Vector3 target = new Vector3(0, 0, -50);
    private bool isInit = false;
    //private UIButton mUIbtn;
    public void Init(CategoryModel model)
    {
        if (isInit)
        {
            return;
        }
        else
        {
            mCategoryID = model.CategoryID;
            mLabel.text = model.Name;      //TODO 多语言
            UIEventListener.Get(cachegameobject).onClick = (GameObject go) =>
            {
                if (mCategoryID == HomePageScreen.CurrentID)
                {
                    return;
                }
                    Bundle b = new Bundle();
                b.SetValue<CategoryModel>("ModelInfo", model);
                MsgManager.Instance.SendMsg(MsgID.LeftSecondMenuClick, b);
            };
            UIEventListener.Get(cachegameobject).onHover = OnHoverItem;
            MsgManager.Instance.RegistMsg(MsgID.CategorySelected, this);
            MsgManager.Instance.RegistMsg(MsgID.LeftSecondMenuClick, this);
            isInit = true;
        }
    }

    public void HandleMessage(MsgID id, Bundle bundle)
    {
        if(id == MsgID.CategorySelected || id == MsgID.LeftSecondMenuClick)
        {
            if (mCategoryID != HomePageScreen.CurrentID)
                Switch(false);
            else
                Switch(true);
        }
    }
    public void Switch(bool on)
    {
        if (on)
        {
            mLabel.color = new Color(77 / 255f, 206 / 255f, 218 / 255f, 255);
            mLabel.transform.DOLocalMove(target, 0.3f);
            mLabel.spacingX = 8;
        }
        else
        {
            mLabel.color = new Color(191/255f, 191/255f, 191/255f, 255);
            this.gameObject.GetComponentInChildren<UILabel>().transform.DOLocalMove(Vector3.zero, 0.3f);
            mLabel.spacingX = 0;
        }
    }

    private void OnHoverItem(GameObject go, bool ishover)
    {
        if (mCategoryID != HomePageScreen.CurrentID)
        {
            if (ishover) {
                mLabel.transform.DOLocalMove(target, 0.3f);
                mLabel.fontStyle = FontStyle.Bold;
                mLabel.color = new Color(1, 1, 1);
                mLabel.spacingX = 8;
            }
            else {
                mLabel.transform.DOLocalMove(Vector3.zero, 0.3f);
                mLabel.fontStyle = FontStyle.Normal;
                mLabel.color = new Color(191 / 255f, 191 / 255f, 191 / 255f, 255);
                mLabel.spacingX = 0;
            }
        }
    }
}