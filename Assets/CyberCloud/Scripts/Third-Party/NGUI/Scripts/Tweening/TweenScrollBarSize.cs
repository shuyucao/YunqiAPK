//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 xavier.xu
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween ScrollBar size")]
public class TweenScrollBarSize : UITweener
{
    public float from;
    public float to;

    UIScrollBar mScrollBar;

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public float value
    {
        get
        {
            if (mScrollBar != null)
            {
                return mScrollBar.barSize;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (mScrollBar != null)
            {
                mScrollBar.barSize = value;
            }
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenScrollBarSize Begin(UIScrollBar bar, float duration, float to)
    {
        TweenScrollBarSize comp = UITweener.Begin<TweenScrollBarSize>(bar.gameObject, duration);
        comp.from = comp.value;
        comp.to = to;
        comp.mScrollBar = bar;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue() { from = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue() { to = value; }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart() { value = from; }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }
}
