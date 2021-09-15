using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;
using static XMPPTool;

namespace Assets.CyberCloud.Scripts.OpenApi
{
    public interface ICVRMsgNotify
    {
        void initResult(int code);
        /// <summary>
        /// 应用启停状态回调
        /// </summary>
        /// <param name="statusDescription"></param>
        void appStatusCallback(StartStatus appStatus);
        /// <summary>
        /// 应用运行状态回调
        /// </summary>
        /// <param name="statusDescription"></param>
        void systemStatusCallback(string systemStatus,string errCode);
        /// <summary>
        /// 通过上层展示消息，提示框分两种类型渐隐式的和弹框式的。渐隐式的弹框为浮动窗口一定时间后自动消失，弹框式为带确认按钮的提示框需要用户操作后关闭。
        /// 如投屏开始时需要在头盔中显示一个校验码，并需要用户将该校验码输入到机顶盒中进行头盔和机顶盒的关联，这时视博云流化sdk会通过该接口通知上层应用进行校验码的展示。
        /// </summary>
        /// <param name="statusDescription"></param>
        void simpleShowDialog(int dialogType, string content, int time);
        /// <summary>
        /// 投屏结果回调
        /// </summary>
        /// <param name="statusDescription"></param>
        void castScreenCallback(CastScreen status,string checkCode);
        /// <summary>
        /// 排队回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        void queueResult(int type, String result);
    }
}
