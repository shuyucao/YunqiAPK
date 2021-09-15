using UnityEngine;
using System.Collections;

public class CategoryButton : MonoBehaviour {

    private string mCategoryID = null;
    public Vector3 OriginPosition;
    public string CategoryID
    {
        get {
            return mCategoryID;
        }
        set {
            mCategoryID = value;
        }
    }

    public UILabel mLabel;

    public string GetName()
    {
        string name = null;
        if (mLabel != null)
        {
            name = mLabel.text;
        }
        return name;
    }

    public void SetName( string name )
    {
        if (mLabel != null)
        {
            mLabel.text = name;
        }
    }

}
