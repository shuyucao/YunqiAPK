using UnityEngine;
using System.Collections;

public class IconButtonData : MonoBehaviour {

    private string _iconName;
    private string _textureName;
    private int _index;

    public string IconName
    {
        get {
            return _iconName;
        }

        set {
            _iconName = value;
        }
    }

    public string TextureName
    {
        get
        {
            return _textureName;
        }

        set
        {
            _textureName = value;
        }
    }

    public int Index
    {
        get
        {
            return _index;
        }

        set
        {
            _index = value;
        }
    }

}
