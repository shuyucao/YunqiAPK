using System.Collections.Generic;
using UnityEngine;

public class StyleUtils
{
    private const int col4 = 4;//according to PS_2R4C
    private const int col3 = 3;//according to PS_2R3C
    private const int row = 2;
    private static Vector3 originPos = Vector3.zero;
    private const int cellwidth = 2;
    private const int cellheight = 2;
    //calculate the position 
    public static List<Vector3> GetCalculatedPos(float width, float height, PageStyle style)
    {
        List<Vector3> poslist = new List<Vector3>();
        Vector3 firstvec = new Vector3();
        Vector3 tmp = new Vector3();
        int zplus;
        float w;
        float h;
        if (style == PageStyle.PS_2R4C)
        {//偶数值计算
            w = (width - cellwidth * (col4 + 1)) / col4;
            h = (height - cellheight * (row + 1)) / row;

            firstvec.x = originPos.x - width / 2 + cellwidth + w / 2;
            firstvec.y = originPos.y + height / 2 - cellheight - h / 2;
            firstvec.z = originPos.z;

            zplus = 9;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col4; j++)
                {
                    tmp.x = firstvec.x + j * (w + cellwidth);
                    if (j == 0)
                    {
                        tmp.x += 2f;
                    }
                    else if (j == 3)
                    {
                        tmp.x -= 2f;
                    }
                    tmp.y = firstvec.y - i * (h + cellheight);
                    if (j < (col4 - 1.0) / 2)
                        tmp.z = firstvec.z - (int)((col4 - 1.0) / 2 - j) * zplus;
                    else
                        tmp.z = firstvec.z - (int)(j - (col4 - 1.0) / 2) * zplus;
                    poslist.Add(tmp);
                }
            }
        }
        else if (style == PageStyle.PS_2R3C)
        {//奇数值计算
            w = (width - cellwidth * (col3 + 1)) / col3;
            h = (height - cellheight * (row + 1)) / row;

            firstvec.x = originPos.x - width / 2 + cellwidth + w / 2;
            firstvec.y = originPos.y + height / 2 - cellheight - h / 2;
            firstvec.z = originPos.z;

            zplus = 6;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col3; j++)
                {
                    tmp.x = firstvec.x + j * (w + cellwidth);
                    tmp.y = firstvec.y - i * (h + cellheight);
                    if (j == (col3 - 1) / 2)
                        tmp.z = firstvec.z;
                    else if (j < col3 / 2)
                        tmp.z = firstvec.z - ((col3 - 1) / 2 - j) * zplus;
                    else
                        tmp.z = firstvec.z + (j - (col3 - 1) / 2) * zplus;
                    poslist.Add(tmp);
                }
            }
        }
        else if (style == PageStyle.PS_1R5C)
        {
            poslist = GetCalculatedPos(width, height, 1, 5);
        }
        //foreach (Vector3 v in poslist)
        //{
        //    Debug.LogError(v.ToString());
        //}
        return poslist;
    }

    //calculate the roatation
    public static List<Quaternion> GetCalculatedRoatation(float r, PageStyle style)
    {
        List<Quaternion> rotationlist = new List<Quaternion>();
        Quaternion quaternion = new Quaternion();
        if (style == PageStyle.PS_2R4C)
        {
            for (int i = 0; i < col4; i++)
            {
                if (i < (col4 + 1.0) / 2)
                    quaternion = Quaternion.Euler(new Vector3(0, -1 * (int)((col4 - 1.0) / 2 - i) * r, 0));
                else
                    quaternion = Quaternion.Euler(new Vector3(0, (int)(i - (col4 - 1.0) / 2) * r, 0));
                rotationlist.Add(quaternion);
            }
        }
        else if (style == PageStyle.PS_2R3C)
        {
            for (int i = 0; i < col3; i++)
            {
                if (i == (col3 - 1) / 2)
                {
                    quaternion = Quaternion.Euler(Vector3.zero);
                }
                else if (i < (col3 - 1) / 2)
                {
                    quaternion = Quaternion.Euler(new Vector3(0, -1 * (int)((col3 - 1.0) / 2 - i) * r, 0));
                }
                else
                {
                    quaternion = Quaternion.Euler(new Vector3(0, (int)(i - (col3 - 1.0) / 2) * r, 0));
                }
                rotationlist.Add(quaternion);
            }
        }
        else if (style == PageStyle.PS_1R5C)
        {
            for (int i = 0; i < 5; i++)
            {
                rotationlist.Add(Quaternion.Euler(Vector3.zero));
            }
            //rotationlist = GetCalculatedRoatation(10, 5);
        }
        //foreach (Quaternion v in rotationlist)
        //{
        //    Debug.LogError(v.eulerAngles.ToString());
        //}
        return rotationlist;
    }

    public static List<Quaternion> GetCalculatedRoatation(float r, int num)
    {
        if (num == 0)
        {
            Debug.Log("the num is 0");
            return null;
        }
        List<Quaternion> list = new List<Quaternion>();
        if (num == 1)
        {
            list.Add(Quaternion.Euler(Vector3.zero));
            return list;
        }
        int c_1, c_2;
        if (num % 2 == 1)
        {
            c_1 = c_2 = num / 2 + 1;
        }
        else
        {
            c_1 = num / 2;
            c_2 = c_1 + 1;
        }
        for (int i = 0; i < num; i++)
        {
            Quaternion tem;
            if (i + 1 < c_1)
            {
                tem = Quaternion.Euler(new Vector3(0, r * (i + 1 - c_1), 0));
            }
            else if ((i + 1) == c_1 || (i + 1) == c_2)
            {
                tem = Quaternion.Euler(Vector3.zero);
            }
            else
            {
                tem = Quaternion.Euler(new Vector3(0, r * (i + 1 - c_2), 0));
            }
            list.Add(tem);
        }
        return list;
    }

    public static List<float> GetCalculatedZ(float r, int num)
    {
        if (num == 0)
        {
            Debug.Log("the num is 0");
            return null;
        }
        List<float> list = new List<float>();
        if (num == 1)
        {
            list.Add(0);
            return list;
        }
        int c_1, c_2;
        if (num % 2 == 1)
        {
            c_1 = c_2 = num / 2 + 1;
        }
        else
        {
            c_1 = num / 2;
            c_2 = c_1 + 1;
        }
        for (int i = 0; i < num; i++)
        {
            float tem;
            if (i + 1 < c_1)
            {
                tem = r * (c_1 - i - 1);
            }
            else if ((i + 1) == c_1 || (i + 1) == c_2)
            {
                tem = 0;
            }
            else
            {
                tem = r * (i + 1 - c_2);
            }
            list.Add(tem);
        }
        return list;
    }

    public static List<Vector3> GetCalculatedPos(float width, float height, int row, int col)
    {
        if (row <= 0 || col <= 0)
        {
            Debug.LogError("the row or col is illegal!");
            return null;
        }
        List<Vector3> poslist = new List<Vector3>();
        //List<float> zlist = GetCalculatedZ(10, col);
        float row_h = height / row;
        float col_w = width / col;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                float x = ((2 * j + 1) * col_w - width) / 2;
                float y = ((2 * i + 1) * row_h - height) / 2;
                //float z = -zlist[j];
                Vector3 pos = new Vector3(x, y, 0);
                poslist.Add(pos);
            }
        }
        return poslist;
    }
}

public enum PageStyle
{
    PS_2R4C = 1,   //2 rows 4 columns
    PS_2R3C = 2,   //2 rows 3 columns
    PS_1R5C = 3,
}