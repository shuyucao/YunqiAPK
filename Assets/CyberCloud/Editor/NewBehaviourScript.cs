using System;
using System.Collections;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
/// <summary>
/// 视博云用到的layer
/// </summary>
public class NewBehaviourScript : AssetPostprocessor
{
    private static string[] myTags = { "cybercloud" };
    private static string[] myLayers = { "gamelayer" ,"gameLayerLeft", "gameLayerRight"};
    //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {

       
        foreach (string tag in myTags)
        {
            AddTag(tag);
        }

        foreach (string layer in myLayers)
        {
            AddLayer(layer);
        }
             
        return;
       
    }

    static void AddTag(string tag)
    {
        if (!isHasTag(tag))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "tags")
                {
                    for (int i = 0; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                        if (string.IsNullOrEmpty(dataPoint.stringValue))
                        {
                            dataPoint.stringValue = tag;
                            tagManager.ApplyModifiedProperties();
                            return;
                        }
                    }
                }
            }
        }
    }

    static void AddLayer(string layer)
    {
        if (!isHasLayer(layer))
        {
            //加载项目设置层以及tag值管理 资源

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();//获取层或tag值所有列表信息
            MyTools.PrintDebugLog("ucvr importal layer:" + layer);
            while (it.NextVisible(true))//判断向后是否还有信息，如果没有则返回false
            {
                if (it.name == "layers")
                {
                    //层默认是32个，只能从第8个开始写入自己的层

                    for (int i = 28; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);//获取层信息
                        if (string.IsNullOrEmpty(dataPoint.stringValue))//如果制定层内为空，则可以填写自己的层名称
                        {
                            dataPoint.stringValue = layer;//设置名字

                            tagManager.ApplyModifiedProperties();//保存修改的属性

                            return;

                        }

                    }

                }
            }

        }
    }

    static bool isHasTag(string tag)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                return true;
        }
        return false;
    }

    static bool isHasLayer(string layer)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                return true;
        }
        return false;
    }
}
#endif