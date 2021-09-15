using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// portal为了支持不同的头盔此处定义头盔尝试类型和当前使用的头盔厂商
/// 除了此配置外不同头盔apk发布切换时还需要替换plugin中的文件 以及修改main.xml中的 主activity 为cyber.cloud.CyberCloudVRActivity 此activity继承头盔厂商的主activity 并且实现了手柄 键盘和鼠标事件的上传.
/// pico的pvr_controller的update中去掉手柄位置信息
/// </summary>
public class CyberCloudConfig : MonoBehaviour {
    public class DeviceTypes {
        public static string Pico = "Pico";//pico neo头盔
        public static string PicoNeo2 = "PicoNeo2";//pico neo2头盔
        public static string DaPeng = "DaPeng";//大鹏头盔
        public static string Pico2 = "Pico2";
        public static string skyworth = "skyworth";//创维
        public static string HuaWei980 = "HuaWei980";
        public static string htc = "htc";
        public static string Other = "Other";
    }
/// <summary>
/// 使用场景
/// </summary>
    public class CVRScreen
    {
        public static string LGUJa = "LGU+";//lgu+
        public static string FuJian = "FuJian";
        public static string YanShi="YanShi";
        public static string YanShi2 = "YanShi2";//jiaqi 2019 年1yue25hao发的新版中心网站接口用的演示系统
        public static string JiangXi = "JiangXi";
		public static string JiaoShi = "JiaoShi";//教室场景
	}
    public class CVRLaguage
    {
        public static string en="en";//英文
        public static string zh = "zh";
        public static string ko = "ko";//韩文
        public static string jp = "jp";//日文目前不支持
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 弹框距离相机的位置
    /// </summary>
    public  const float DialogDistance = 2.8f;
    /// <summary>
    /// 头盔厂商
    /// </summary>
    public static string currentType = DeviceTypes.PicoNeo2;
    //注意：如果是演示版本需要先修改包名为com.picovr.sbygame。***********************************************************************************************
    public static string cvrScreen = CVRScreen.JiangXi;//LGUJa时会在初始化时将语言设置成韩文
    public static string cvrLanguage = CVRLaguage.zh;
    public static string tls = "";

    /// <summary>
    /// 不同头盔厂商对应的appid不同 这样就可以下载不同的apk包
    /// </summary>
    /// <returns></returns>
    public static string PkAppID {
        get {

            if (currentType == DeviceTypes.Pico)
            {
                return "100000002";//之前发了个版本包名是com.picovr.sbygame 如今改成com.sbyvr.pico 应用id在之前的基础上加1因adp的应用id是按顺序来的
            }
            else if (currentType == DeviceTypes.DaPeng)
            {
                return "100000003";
            }
            else if (currentType == DeviceTypes.Pico2)
            {
                return "100000005";
            }
            else if (currentType == DeviceTypes.skyworth)
            {
                return "100000006";
            }
            else if (currentType == DeviceTypes.HuaWei980) {
                return "100000007";
            }
            else //每个厂家appid不同需要向视博云申请
            {
                return "100000004";
            }

            return "";
        }
    }
 
    /// <summary>
    /// 不同头盔厂商对应的Pk不同 这样就可以下载不同的apk包
    /// </summary>
    /// <returns></returns>
    public static string ApkPackage
    {
        get
        {
            if (currentType == DeviceTypes.Pico)
                return "com.sbyvr.pico";
            else if (currentType == DeviceTypes.DaPeng)
                return "com.sbyvr.deepoon";
            return "";
        }
    }
    /// <summary>
    /// true仅仅导出player apk，false导出带portal的apk
    /// </summary>
    public static bool ExportOnlyPlayer = false;//
    /// <summary>
    /// 启动模式
    /// 0是云
    /// 1直连
    /// </summary>
    public static int startMode = 0;//
    /// <summary>
    /// 默认的启动长串
    /// </summary>
    public static string loginInfo = "";//
    /// <summary>
    /// 投屏灰盒测试配置文件地址
    /// </summary>
    public static string castScreenTestConfigFileUrl = "";
    /// <summary>
    /// 配置此id后 apk启动后应用会自动启动并统计应用启动次数和成功次数
    /// </summary>
    public static string TestAutoStartAppID = "";
    /// <summary>
    /// 是否打印每一帧的时延 1是 0是否 ，配置成0时打印的是每秒的平均时延*
    /// </summary>
    public static int printFrameTimeDelay = 0;
    /// <summary>
    /// 是否自动启动投屏测试服务自动自动后无需手动点击openTcp按钮
    /// </summary>
    public static int autoStartCastScreenTestService;
        /// <summary>
      /// 是否显示投屏菜单
    /// </summary>
    public static int showCastScreen=0;
    /// <summary>
    /// 是否开启自动投屏只有autoStartCastScreenTestService有效时才生效
    /// </summary>
    public static int autoStartCastScreen;
    /// <summary>
    /// 大鹏头盔在演示环境中开启投屏功能
    /// </summary>
    public static string DeepoonCastScreenDemonstration = "0";
      /// <summary>
    /// 终端是否上传统计信息
    /// 接口logTransmissionEnable 参数0是不上传，1上传到sby_test_data,110上传到sby_internal_test_data
    /// </summary>
    public static int statisticsUpLoad = 0;
    /// <summary>
    /// 终端是否上传统计信息
    /// </summary>
    public static string statisticsUpLoadUrl = "";

     /**此处的行业分类必须在cms配置的一级分类中有**/
    public static string classificationOfProfessions = "app";
      /**预测测试，建议有些头盔不知是否增加了预测，可以通过此处配置一个预测值查看日志CVR:DeltaTest可以看是否增加了预测，此功能轻易不要要打开否则会增加接口调用此时，默认配置0即可**/
    public static int deltaTimeTest = 0;
    /**应用启动映射，每组映射之间以‘;’分号分隔,分组内以‘:’冒号分隔，组内的配置顺序是：screenNo:index:appID（场景码+位置好+应用id）**/
    public static string mapapps = "";
    /**
     *unity渲染完成后是否 交由视博云native层显示 。
     * */
    public static bool NativeShowScreen = false;
    /// <summary>
    /// 区域码code
    /// </summary>
    public static string CyberZoneCode = "";
    /// <summary>
    /// 区域码描述
    /// </summary>
    public static string CyberZoneDesc = "";
    /// <summary>
    /// 启动参数此参数会拼接到应用启动串后面用于特定测试场景如指定服务器启动
    /// </summary>
    public static string startParam = "";
    /// <summary>
    /// 终端类型，用于判断与前端通信使用udp或tcp通信的依据，使用前需要在中心管理网站配置终端类型对应的通信协议，再将对应的终端类型配置到此处
    /// </summary>
    public static int terminalType = 0;
    /// <summary>
    /// 是否使用应用分辨率，0是不使用，1是使用。不使用应用分辨率时使用屏幕分辨率，当分辨率大时会应用终端刷新时延變大
    /// </summary>
    public static int useAppResolution = 0;
    /**日誌級別  VERBOSE:2，  DEBUG:3， INFO:4， WARN:5， ERROR:6**/
    public static int logOutLevel = 0;
    /// <summary>
    /// 教室场景下模拟终端的个数。头盔个数有限为了模拟多个学生直播画面通过该参数模拟，如果不需要请配置成0.非0时模拟的设备id为“设备id+序号”
    /// </summary>
    public static int deviceTestNumJscj=0;
    /// <summary>
    /// 应用适配平台后台地址，当地址不为空时通过该地址获取启动参数，用于应用适配开发者启动测试应用
    /// </summary>
    public static string adapt_platform = "";
    /// <summary>
    /// 是否启用新的投屏屏支持协议 1、支持新的srt投屏协议 0、是不支持
    /// </summary>
    public static int newScreenProtocol = 0;
    /// <summary>
    /// tls配置是否支持ping指令，如果ping被禁用此处需要配置成0
    /// </summary>
    public static int usePingCmd = 1;
    /**终端手柄类型Ctrl_None = 0    Ctrl_OculusCV1 = 1    Ctrl_Vive = 2    Ctrl_Nolo = 3    Ctrl_ViveIndex = 4    Ctrl_OculusRift = 5**/
    public static int controllerType = 2;
    /** 是否由终端控制帧率 **/
    public static int useTerminalFrmRtCtrl = 0;
    public static string tenantID = "cybercloud";
}
