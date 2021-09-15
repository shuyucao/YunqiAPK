using UnityEngine;

public class CircleScrolleBar : MonoBehaviour
{
    [SerializeField]
    UISprite mCircle;
    [SerializeField]
    UILabel mLable;

    public void SetData(float val)
    {
        mCircle.fillAmount = val / 100;
        mLable.text = val.ToString() + "%";
    }
}
