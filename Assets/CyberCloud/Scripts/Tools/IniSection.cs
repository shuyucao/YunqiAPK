using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IniSection
{
    private string sectionName;
    private Dictionary<string, string> m_dicKeyValue;

    public string SectionName
    {
        get { return this.sectionName; }
        set { this.sectionName = value; }
    }

    public IniSection(string name)
    {
        this.sectionName = name;
        this.m_dicKeyValue = new Dictionary<string, string>();
    }
    /// <summary>
    /// 添加key-value的值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddKeyValue(string _key, string _value)
    {
        string value = null;
        if (m_dicKeyValue.TryGetValue(_key, out value))
        {
            if (value != null)
            {
                m_dicKeyValue[_key] = _value;
            }
        }
        else
        {
            m_dicKeyValue.Add(_key, _value);
        }
    }
    /// <summary>
    /// 根据key取得value，如果没有取到就返回默认的值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public string GetValue(string key, string defaultValue)
    {
        string value = null;
        m_dicKeyValue.TryGetValue(key, out value);
        if (m_dicKeyValue.TryGetValue(key, out value))
        {
            return value;
        }
        return defaultValue;
    }
}
