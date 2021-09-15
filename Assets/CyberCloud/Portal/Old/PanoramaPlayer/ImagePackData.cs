using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImagePackData {

    public static int MaxListNum = 5;

    private string _iconName;
    private string _textureName;
    private int _currentListIndex;
    public List<IconButtonData> IconList = new List<IconButtonData>();


    public string IconName
    {
        get
        {
            return _iconName;
        }

        set
        {
            _iconName = value;
        }
    }

    public string TextureName
    {
        get {
            return _textureName;
        }

        set {
            _textureName = value;
        }
    }

    public int CurrentListIndex
    {
        get {
            return _currentListIndex;
        }

        set {
            _currentListIndex = value;
        }
    }


    public static ImagePackData ParseJson(string jsonStr, string key)
    {
        ImagePackData packData = new ImagePackData();


        packData.CurrentListIndex = Random.Range(0, 5);
        packData.IconList.Clear();
        IconButtonData buttonData;
        for (int i = 0; i < MaxListNum; i++)
        {
            string texturePath = "Textures/PanoramaPlayer/TestPic0" + Random.Range(1, 3).ToString();
            if (packData.CurrentListIndex == i)
            {
                packData.TextureName = texturePath;
                packData.IconName = texturePath + "_Icon";
            }


            buttonData = new IconButtonData();  //这么用会有警告
            buttonData.TextureName = texturePath;
            buttonData.IconName = texturePath + "_Icon";
            buttonData.Index = i;
            packData.IconList.Add(buttonData);
        }

        return packData;
    }
}
