using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Assets.Scripts.Constant
{
    class CommonConstant
    {
        /**
         * MEC代理服务器URL
         */
        public static String SERVER_URL_MEC_PROXY = "http://test.yqzh.tech:20010/vlab";

        /**
        * MEC代理服务器URL默认地址
        */
        public static String SERVER_URL_MEC_PROXYDEFULAT = "http://test.yqzh.tech:20010/vlab";

        /**
       * MEC代理服务器URL(UC)
       */
        public static String SERVER_URL_MEC_PROXY_UC = "http://test.yqzh.tech:20010/uc-gateway";

        /**
       * 视博云代理服务器URL
       */
        public static String SERVER_URL_SBY_PROXY = "http://183.242.55.146:12000";

        /**
        * 视博云代理服务器URL默认地址
        */
        public static String SERVER_URL_SBY_PROXYDEFULAT = "http://183.242.55.146:12000";


        //Vlab接口

        public static string SERVER_URL_VARIFY_VLAB = "/vlab";

        //uc-gateway接口

        public static string SERVER_URL_VARIFY_UC_GATEWAY = "/uc-gateway";

        /**
       * 获取学段和年级
       */
        public static string GET_GRADE_LIST = "/common/getGradeList";

        /**
      * 获取学科
      */
        public static string GET_SUBJECT_LIST = "/common/getSubjectList";
        
        /**
      * 保存用户学段年级
      */
        public static string POST_SAVE_GRADE = "/common/saveUserInfo";

        /**
 * 创建sessionURL(UC)
 */
        public static string SERVER_URL_SESSION_GET = "/v1.1/sessions";

        //获取教材下章节信息
        public static string GET_JIAOCAIZHANGJIEINFO = "/res/getTextbookDetailList";

        //获取实验资源信息
        public static string GET_SHIYANZIYUANINFO = "/test/getTestList";

        //获取用户信息
        public static string GET_USERINFO = "/common/getUserInfo";

        //获取图片验证码
        public static string SERVER_URL_IDENTIFY_CODE_GET = "/identify_code";

        /**
      * 验证图片URL(UC)
      */
        public static string SERVER_URL_VARIFY_IDENTYFY_CODE = "/identify_code/actions/valid";

        /**
        * 短信登录URL(UC)
        */
        public static String SERVER_URL_VARIFY_SMS_LOGIN = "/v1.1/sms_tokens";

        /**
      * 短信登录令牌置换URL(UC)
      */
        public static String SERVER_URL_VARIFY_SMS_LPZH = "/v1.1/person_accounts/binded/org/users";

        /**
      * 发送短信验证码URL(UC)
      */
        public static String SERVER_URL_SMS_CODE_SUBMIT = "/actions/send_sms_code";

        /**
     * 账号登录URL(UC)
     */
        public static String SERVER_URL_ACCOUNT_LOGIN = "/v1.1/tokens";

        //获取教材
        public static string GET_JIAOCAI = "/common/getSubject";

        //用户选择列表
        public static string GET_USER_CHOOSE_LIST = "/res/getTextbookList";

        //获取实验测试记录
        public static string GET_SHIYANCESHI_LIST = "/activity/activitySearchList";

        //获取所有出版社
        public static string GET_PRESS_LIST = "/common/getPress";

        //获取实验测试的地区学科
        public static string GET_SHIYANCESHI_DIQU_XUEKE = "/test/getTestTags";
        //添加用户选择教材
        public static string EDIT_USER_TEXTBOOK = "/res/saveUserTextbook";       
        //获取实验测试资源
        public static string GET_SHIYANCESHIZIYUAN_LIST = "/test/getTestSearchList";

        //获取实验测试内容
        public static string GET_SHIYANCESHINERONG_LIST = "/activity/getTestContent";

        //获取实验测试信息的接口
        public static string GET_SHIYANCESHIINFO = "/activity/getTestInfo";

        //获取实验测试详情信息接口
        public static string GET_SHIYANCESHIDETAILINFO = "/activity/getTestContentDetail";

        //获取实验详情记录弹窗的信息
        public static string GET_SHIYANXIANGQINGCESHIJILU = "/activity/getActivityRecord";

        //获取实验测试答题的记录信息
        public static string GET_SHIYANXIAINGQINGCESHIDATIJILU = "/activity/getActivityAllRecord";

        //实验测试易错项
        public static string GET_SHIYANCESHIYICUOXIANG = "/activity/getActivityError";

        //保存实验记录
        public static string GET_SAVESHIYANJILU = "/activity/saveActivity";

        //获取地理位置的百度接口
        public static string GET_LOCATIONURL = "http://api.map.baidu.com/location/ip?ak=bretF4dm6W5gqjQAXuvP0NXW6FeesRXb&coor=bd09ll";
    }
}
