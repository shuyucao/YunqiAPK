//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UICircleTexture), true)]
public class UICircleTextureInspector : UIWidgetInspector
{
    UICircleTexture mTex;

    protected override void OnEnable()
    {
        base.OnEnable();
        mTex = target as UICircleTexture;
    }

    protected override bool ShouldDrawProperties()
    {
        if (target == null) return false;
        SerializedProperty sp = NGUIEditorTools.DrawProperty("Texture", serializedObject, "mTexture");
        NGUIEditorTools.DrawProperty("Material", serializedObject, "mMat");

        if (sp != null) NGUISettings.texture = sp.objectReferenceValue as Texture;

        if (mTex != null && (mTex.material == null || serializedObject.isEditingMultipleObjects))
        {
            NGUIEditorTools.DrawProperty("Shader", serializedObject, "mShader");
        }

        EditorGUI.BeginDisabledGroup(mTex == null || mTex.mainTexture == null || serializedObject.isEditingMultipleObjects);

        //NGUIEditorTools.DrawProperty("Percent", serializedObject, "mPercent");
        mTex.Percent = EditorGUILayout.Slider("Value", mTex.Percent, 0f, 1f);
        mTex.Height = EditorGUILayout.Slider("Height", mTex.Height, 0f, 1f);
        //DrawLegacyFields();

        EditorGUI.EndDisabledGroup();
        return true;
    }

    /// <summary>
    /// Allow the texture to be previewed.
    /// </summary>

    public override bool HasPreviewGUI()
    {
        return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1) &&
            (mTex != null) && (mTex.mainTexture as Texture2D != null);
    }

    /// <summary>
    /// Draw the sprite preview.
    /// </summary>

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        Texture2D tex = mTex.mainTexture as Texture2D;

        if (tex != null)
        {
            Rect tc = mTex.uvRect;
            tc.xMin *= tex.width;
            tc.xMax *= tex.width;
            tc.yMin *= tex.height;
            tc.yMax *= tex.height;
            NGUIEditorTools.DrawSprite(tex, rect, mTex.color, tc, mTex.border);
        }
    }
}
