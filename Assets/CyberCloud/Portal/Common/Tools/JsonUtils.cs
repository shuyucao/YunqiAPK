using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

public class JsonUtils
{
    public static List<CategoryModel> ParseCategoryJson(string jsonStr, ref int msg)
    {
        return new List<CategoryModel>();
    }

    public static List<PhotoModel> ParsePhotoJson(string jsonStr, ref int msg, ref int totalPage, ref int currentPage)
    {
        return new List<PhotoModel>();
    }

    public static List<PhotoModel> ParaseDimensionDoor(string jsonStr, ref int msg)
    {
        return new List<PhotoModel>();
    }

    public static List<ThemesModel> ParaseThemesJson(string jsonStr, ref int msg)
    {
        return new List<ThemesModel>();
    }

    public static List<PhotoModel> ParseThemesPhotoJson(string jsonStr, ref int msg)
    {
        return new List<PhotoModel>();
    }

    /// <summary>
    /// 0:errro  1:no update info  2:need update  3: force to update
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int ParseVersionUpdate(string str)
    {
        return 0;
    }

    public static List<LocalPhotoModel> ParseLocalPhoto(string str)
    {
        return new List<LocalPhotoModel>();
    }
}