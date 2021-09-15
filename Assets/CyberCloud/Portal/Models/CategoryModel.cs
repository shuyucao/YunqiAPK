public class CategoryModel
{

    private string _categoryID;
    private string _name;
    private string _englishName;

    public string CategoryID
    {
        get
        {
            return _categoryID;
        }

        set
        {
            _categoryID = value;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public string EnglishName
    {
        get
        {
            return _englishName;
        }

        set
        {
            _englishName = value;
        }
    }
}
