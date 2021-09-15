using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TextExtension
{
    public static void SetTextWithEllipsis(this Text textComponent, string value, int characterVisibleCount)
    {

        var updatedText = value;

        // 判断是否需要过长显示省略号
        if (value.Length > characterVisibleCount)
        {
            updatedText = value.Substring(0, characterVisibleCount - 1);
            updatedText += "…";
        }

        // update text
        textComponent.text = updatedText;
    }
}
