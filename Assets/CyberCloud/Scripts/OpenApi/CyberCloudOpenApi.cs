using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.CyberCloud.Scripts.OpenApi;
using static Assets.CyberCloud.Scripts.OpenApi.OpenApiImp;
namespace Assets.CyberCloud.Scripts
{
    public class CyberCloudOpenApi
    {     
        OpenApiImp openapi;
        public CyberCloudOpenApi() {
            openapi = OpenApiImp.getOpenApi(); ;
        }
        /// <summary>
        /// 使用CyberCloudOpenApi的其他接口前请调用该接口先进行初始化
        /// </summary>
        /// <param name="gatewayUrl"></param>
        /// <param name="terminalType"></param>
        /// <param name="tenantID"></param>
        /// <param name="userID"></param>
        /// <param name="userToken"></param>
        /// <param name="logOutLevel"></param>
        /// <param name="devicenfoClassName">piconeo2已适配可以固定传"com.cybercloud.vr.player.pico.PicoVRDevice" 就可以</param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public int init (string gatewayUrl,string configServiceUrl, TerminalType terminalType, string tenantID, int logOutLevel, string deviceinfoClassName, ICVRMsgNotify notify,bool useTerminalFromRtCtrl=false, TerminalControllerType terminalControllerType = TerminalControllerType.Ctrl_Vive, bool localProjectionEnable = false)
        {
            return openapi.init(gatewayUrl, configServiceUrl, terminalType, tenantID, logOutLevel, deviceinfoClassName, notify,useTerminalFromRtCtrl, terminalControllerType, localProjectionEnable);         
        }
        /// <summary>
        /// 用于启动流化应用。
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        public int startApp(string appID, string userID, string userToken, string playToken, string authToken, Dictionary<string, string> ex) {
            return openapi.startApp(appID, userID, userToken, playToken,authToken,ex);
        }
   
        /// <summary>
        /// 用于停止流化应用。
        /// 注：应用停止后对应的流化投屏也会结束
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        public int stopApp() {           
            return openapi.stopApp();
        }
        /// <summary>
        /// 开启投屏请在应用启动成功后调用。
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        public int startCastScreen(bool encodeStandAlone) {
            return openapi.startCastScreen(encodeStandAlone);
        }
        /// <summary>
        /// 用于停止应用投屏。
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        public int stopCastScreen() {
            return openapi.stopCastScreen();
        }
        //=======================排队=======================
   
        /// <summary>
        /// 申请排队
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="userID"></param>
        /// <param name="userLevel"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public int applyQueue(string appID, string userID, int userLevel, Dictionary<string, string> ext) {
            return openapi.applyQueue(appID, userID, userLevel, ext);;
        }
        public int queryQueue(string queueCode) {
            return openapi.queryQueue(queueCode);
        }
        public int cancelQueue(string queueCode) {
            return openapi.cancelQueue(queueCode);
        }      
        //=======================排队服务本期不支持========================

        /// <summary>
        /// 该接口需要在应用启动成功后调用,应用启动过程中如果需要在终端设备上显示本地操作菜单，菜单显示过程中不希望应用响应手柄的姿态以及按键数据，此时可以通过该接口进行控制
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        public int handDeviceInfoEnable(bool enable) {
            return openapi.handDeviceInfoEnable(enable);
        }
        /// <summary>
        /// 统计日志上报开关，如果需要上报统计数据请调用该接口
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="uploadUrl"></param>
        /// <param name="appName"></param>
        /// <param name="setOperateUserID"></param>
        /// <param name="setApkVersion"></param>
        /// <returns></returns>
        public int dataStatistics(bool enable) {
            return openapi.dataStatisticsOption(enable);
        }
        public int audioEnable(bool enable)
        {
            return openapi.audioEnable(enable);
        }
        
        /// <summary>
        /// 该接口需要在应用启动成功后调用,用于设置右手手柄的索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int changeRightHandIndex(int index) {
            return openapi.changeRightHandIndex(index);
        }
    }
}
