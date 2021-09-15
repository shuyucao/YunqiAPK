using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IniFile
{
    private List<IniSection> m_sectionList;
    public IniFile()
    {
        m_sectionList = new List<IniSection>();
    }
    public bool LoadFromFile(string strFullPath)
    {
        try
        {
     
            //string strFullPath =fileName;
          
            if (!File.Exists(strFullPath))
            {

                MyTools.PrintDebugLogError("ucvr no config file in assets strFullPath:" + strFullPath);

                return false;
            }
            using (FileStream fs = new FileStream(strFullPath, FileMode.Open))
            {
                LoadFromStream(fs);
                return true;
            }
        }
        catch (Exception e) {

            MyTools.PrintDebugLogError("ucvr LoadFromFile:"+ e.Message);
            return false;
        }
    }
    public bool LoadFromResouceConfig()
    {
        try
        {
          
            TextAsset txt = Resources.Load("config", typeof(TextAsset)) as TextAsset;
            string configcontent = txt.text;

            //string strFullPath =fileName;
            LoadFromString(configcontent);
            return true;
        }
        catch (Exception e)
        {

            MyTools.PrintDebugLogError("ucvr LoadFromResouceConfig:" + e.Message);
            return false;
        }
    }
    void Main(string[] args)
    {
        string text = "abcd\nasdfdsf\nbsafd";
        using (StringReader sr = new StringReader(text))
        {
            string line;
            int lineIndex = 0;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine("行{0}:{1}", ++lineIndex, line);
            }
        }
    }
    private void LoadFromString(string text)
    {
        using (StringReader sr = new StringReader(text))
        {
            m_sectionList.Clear();
            string line = null;
            IniSection section = null;
            int equalSignPos = 0;//=号的标记的位置
            string key, value;
            while (true)
            {
                line = sr.ReadLine();
                if (null == line)
                {
                    break;
                }
                line = line.Trim();
                if (line == "")
                {
                    continue;
                }
                //跳过注释
                if (line.Length >= 2 && line[0] == '/' && line[1] == '/')
                {
                    continue;
                }
                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    //移除首尾的'[]'
                    line = line.Remove(0, 1);
                    line = line.Remove(line.Length - 1, 1);
                    //去SectionList缓存中找是否存在这个Section
                    section = GetSection(line);
                    //如果没有找到就直接new一个
                    if (null == section)
                    {
                        section = new IniSection(line);
                        m_sectionList.Add(section);
                    }
                }
                else
                {
                    //就是在这个头下面的数据字段，key-value格式
                    equalSignPos = line.IndexOf('=');
                    if (equalSignPos != 0)
                    {
                        key = line.Substring(0, equalSignPos);
                        value = line.Substring(equalSignPos + 1, line.Length - equalSignPos - 1);
                        section.AddKeyValue(key, value);
                    }
                    else
                    {
                        MyTools.PrintDebugLog("ucvr value is null");
                    }
                }
            }
        }
    }
    /// <summary>
    /// 取得配置文件中所有的头名称
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllSectionName()
    {
        List<string> sectionList = new List<string>();
        foreach (var sec in m_sectionList)
        {
            sectionList.Add(sec.SectionName.ToLower());
        }
        return sectionList;
    }
    /// <summary>
    /// 取得头部相关的value
    /// </summary>
    /// <param name="sectionName"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public string GetValue(string sectionName, string key, string defaultValue)
    {
        IniSection section = GetSection(sectionName);
        if (section != null)
        {
            return section.GetValue(key, defaultValue);
        }
        return defaultValue;
    }

    private void LoadFromStream(FileStream fs)
    {
        using (StreamReader sr = new StreamReader(fs))
        {
            m_sectionList.Clear();
            string line = null;
            IniSection section = null;
            int equalSignPos = 0;//=号的标记的位置
            string key, value;
            while (true)
            {
                line = sr.ReadLine();
                if (null == line)
                {
                    break;
                }
                line = line.Trim();
                if (line == "")
                {
                    continue;
                }
                //跳过注释
                if (line.Length >= 2 && line[0] == '/' && line[1] == '/')
                {
                    continue;
                }
                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    //移除首尾的'[]'
                    line = line.Remove(0, 1);
                    line = line.Remove(line.Length - 1, 1);
                    //去SectionList缓存中找是否存在这个Section
                    section = GetSection(line);
                    //如果没有找到就直接new一个
                    if (null == section)
                    {
                        section = new IniSection(line);
                        m_sectionList.Add(section);
                    }
                }
                else
                {
                    //就是在这个头下面的数据字段，key-value格式
                    equalSignPos = line.IndexOf('=');
                    if (equalSignPos != 0)
                    {
                        key = line.Substring(0, equalSignPos);
                        value = line.Substring(equalSignPos + 1, line.Length - equalSignPos - 1);
                        section.AddKeyValue(key, value);
                    }
                    else
                    {
                        MyTools.PrintDebugLog("ucvr value null");
                    }
                }
            }
        }
    }
    /// <summary>
    /// 从缓存中找Section
    /// </summary>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    private IniSection GetSection(string sectionName)
    {
        foreach (var section in m_sectionList)
        {
            if (section.SectionName.ToLower() == sectionName.ToLower())
            {
                return section;
            }
        }
        return null;
    }
}
