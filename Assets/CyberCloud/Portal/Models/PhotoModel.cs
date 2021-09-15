using UnityEngine;
using System.Collections;

public class PhotoModelBase : BaseData
{

}

public class PhotoModel : PhotoModelBase
{
    private string _mid;
    private string _title;
    private string _coverLink;
    private string _thumbnailLink;
    private string _photoLink;
    private string _provider;
    private string _logoimg;

    public string MID
    {
        get
        {
            return _mid;
        }

        set
        {
            _mid = value;
        }
    }

    public string Title
    {
        get
        {
            return _title;
        }

        set
        {
            _title = value;
        }
    }

    public string CoverLink
    {
        get
        {
            return _coverLink;
        }

        set
        {
            _coverLink = value;
        }
    }

    public string ThumbnailLink
    {
        get
        {
            return _thumbnailLink;
        }

        set
        {
            _thumbnailLink = value;
        }
    }

    public string PhotoLink
    {
        get
        {
            return _photoLink;
        }

        set
        {
            _photoLink = value;
        }
    }

    public string Provider
    {
        get
        {
            return _provider;
        }
        set
        {
            _provider = value;
        }
    }

    public string LogoImg
    {
        get
        {
            return _logoimg;
        }
        set
        {
            _logoimg = value;
        }
    }

}

public class LocalPhotoModel : PhotoModelBase
{
    private string _title;
    private string _thumbnailLink;
    private string _photoLink;

    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
        }
    }

    public string ThumbnailLink
    {
        get
        {
            return _thumbnailLink;
        }
        set
        {
            _thumbnailLink = value;
        }
    }

    public string PhotoLink
    {
        get
        {
            return _photoLink;
        }
        set
        {
            _photoLink = value;
        }
    }
}