using UnityEngine;
using System.Collections;

public class LabelWrap : MonoBehaviour {

    void Start()
    {
        UILabel uiLabel = gameObject.GetComponent<UILabel>();
        string strContent = uiLabel.text; // UILabel中显示的内容
        string strOut = string.Empty;
        // 当前配置下的UILabel是否能够包围Text内容
        // Wrap是NGUI中自带的方法，其中strContent表示要在UILabel中显示的内容，strOur表示处理好后返回的字符串，uiLabel.height是字符串的高度 。
        bool bWarp = uiLabel.Wrap(strContent, out strOut, uiLabel.height);
        // 如果不能，就是说Text内容不能全部显示，这个时候，我们把最后一个字符去掉，换成省略号"..."
        if (!bWarp)
        {
            strOut = strOut.Substring(0, strOut.Length - 1);
            strOut += "...";
        }
        // 如果可以包围，就是说Text内容可以完全显示，这个时候，我们不做处理，直接显示内容。
        uiLabel.text = strOut;
    }
}
