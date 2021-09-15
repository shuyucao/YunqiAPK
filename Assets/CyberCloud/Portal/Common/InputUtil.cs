using UnityEngine;
using System.Collections;

public class InputUtil {
	public const string Horizontal = "Horizontal";
	public const string Vertical = "Vertical";
	public const string Submit = "Submit";
	public const string Cancel = "Cancel";
	public const string Recenter = "Recenter";
	public const string LockScreen = "LockScreen";
    public const string Jump = "Jump";
#if UNITY_ANDROID && !UNITY_EDITOR

	public const string ButtonA = "Button A";
	public const string ButtonB = "Button B";
	public const string ButtonX = "Button X";
	public const string ButtonY = "Button Y";
	public const string DPadX = "DPad_X";
	public const string DPadY = "DPad_Y";
	public const string LeftXAxis = "Left_X_Axis";
	public const string LeftYAxis = "Left_Y_Axis";
	public const string RightXAxis = "Right_X_Axis";
	public const string RightYAxis = "Right_Y_Axis";
	public const string RightShoulder = "Right Shoulder";
	public const string LeftShoulder = "Left Shoulder";

#else

    public const string ButtonA = "Desktop_Button A";
	public const string ButtonB = "Desktop_Button B";
	public const string ButtonX = "Desktop_Button X";
	public const string ButtonY = "Desktop_Button Y";
	public const string DPadX = "Desktop_DPad_X";
	public const string DPadY = "Desktop_DPad_Y";
	public const string LeftXAxis = "Desktop_Left_X_Axis";
	public const string LeftYAxis = "Desktop_Left_Y_Axis";
	public const string RightXAxis = "Desktop_Right_X_Axis";
	public const string RightYAxis = "Desktop_Right_Y_Axis";
	public const string RightShoulder = "Desktop_Right Shoulder";
	public const string LeftShoulder = "Desktop_Left Shoulder";

	#endif
}
