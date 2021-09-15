using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CompilerOptionsEditorScript
{
    static CompilerOptionsEditorScript()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    static void OnEditorUpdate()
    {
        if (EditorApplication.isCompiling)
        {
            string date = System.DateTime.Now.ToString("yyyy-MM-dd");
  
            //if (date != CyberCloudConfig.publishDate)
           // {

                //EditorApplication.LockReloadAssemblies();
           // }
            //MyVersion.publishTime = date;
        }
    }
}
