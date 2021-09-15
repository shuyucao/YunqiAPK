using Assets.Scripts.Result;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data 
{
   public class LoginWindowData : Singleton<LoginWindowData>
    {
        //是否登录
        private bool isLogin = false;
        public bool IsLogin
        {
            get { return isLogin; }
            set { isLogin = value; }
        }

        /// <summary>
        /// 保存Seesion数据
        /// </summary>
        private CreateSessionResult createSessionResult = new CreateSessionResult();

        public void SaveSessionResult(CreateSessionResult createSessionResult)
        {
            this.createSessionResult = createSessionResult;
        }

        public CreateSessionResult ReadSessionResult()
        {
            return createSessionResult;
        }

        /// <summary>
        /// 保存账号密码登录数据
        /// </summary>
        private AccountLoginResult accountLoginResult = null;

        public void SaveAccountLoginResult(AccountLoginResult accountLoginResult)
        {
            this.accountLoginResult = accountLoginResult;
        }

        public AccountLoginResult ReadAccountLoginResult()
        {
            return accountLoginResult;
        }

        //短信登录
        private SMSLoginResult sMSLoginResult = null;
        public void SaveSMSLoginResult(SMSLoginResult sMSLoginResult)
        {
            this.sMSLoginResult = sMSLoginResult;
        }

        public SMSLoginResult ReadSMSLoginResult(SMSLoginResult sMSLoginResult)
        {
            return sMSLoginResult;
        }
        public SMSLoginResult SetSMSLoginResult()
        {
            return sMSLoginResult = null;
        }

        //存储短信令牌切换账号登录令牌
        private ToKenSwapResult toKenSwapResult = null;

        public void SaveToKenSwapResult(ToKenSwapResult toKenSwapResult)
        {
            this.toKenSwapResult = toKenSwapResult;
        }
        public ToKenSwapResult ReadToKenSwapResult()
        {
            return toKenSwapResult;
        }

        private UserInfoResult sUserInfoResult = null;

        //获取用户信息
        public void SaveUserInfoResult(UserInfoResult userInfoResult)
        {
            this.sUserInfoResult = userInfoResult;
        }

        public UserInfoResult GetUserInfoResult()
        {
            return sUserInfoResult;
        }
    }
}

