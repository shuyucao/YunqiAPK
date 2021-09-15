using UnityEngine;

public class Constant
{
    public const int maxPage = 100;                 //最大的pageitem缓存数量
    public const int MaxCountPerPage = 10;          //每个pageitem所能包含最大的图片数量
    public const int MaxLimitEachPage = 40;         //每次发送申请的数量
    public static int ImgCountPerPage = 8;          //每个pageitem所包含的图片数量
    public const int PageNumCreatOneTime = 5;       //一次加载的pageitem 数量
    public const int PageNumCreatOneTime_ForPlayerScreen = 8;       //一次加载的pageitem 数量
    public const int MaxNumOfOnceRequest = 1000;

    public const int pageMaxThemes = 1000;    // 每页请求最大的专题数
    public const int pageMaxThemesPhoto = 1000; // 每次请求的最大的某专题下图片列表数

    public const string CategoryOfAllCid = "0";     //“全部”类别的CID


    public static Vector3 ScreenOriginPos = new Vector3(140, 3, 10);

    public const string Tag_720Yun = "720yun";
    public const string Tag_CNTraveler = "CNTraveler";
    public const string Tag_AP = "AP";
    public const string Tag_Pico = "pico";

    public const string Tag_Huodong = "1";
    public const string Tag_Fufei = "2";
    public const string Tag_New = "3";
    public const string Tag_Cehua = "4";
    public const string Tag_Hot = "5";
    public const string Tag_Zhuanti = "6";
    public const string Tag_Tuiguang = "7";
    public const string Tag_Huiyuan = "8";

    public const float PlayerLoadTimeOut = 20;

    public const int MaxPersistUnloadNum = 10;    // 最大累计清除大图的数量，超过次数了进行一次Resources.UnloadUnusedAssets()操作
}