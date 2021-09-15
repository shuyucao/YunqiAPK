using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Config
{
    public class PrefabType
    {
        public enum Window
        {
            BaseInfoWindow,
            TextbookSelectWindow,
            LoginWindow,
            LoadingWindow,
            MainWindow,
            ShiYanXuanZeWindow,
            UniversalLoadingWindow,
            TextbookMgrWindow,
            ShiYanCeShiWindow,
            ShiYanCeShiXiangQingMainWindow,
            ShiYanCeShiXiangQingJiLuWindow,
            ShiYanCeShiXiangQingJiLuDaTiWindow,
            ShiYanCeShiYiCuoXiangWindow,
            ExperimentHistoryWindow,
            YinSiZhengCeAndXieYiWindow,
            VerifyCodeTip,
        }
        public enum WindowGrid
        {
            NianJiGrid,
            XueDuanGrid,
            TextbookGrid,
            SubjectGrid,
            TextbookMgrGrid,
        }

    }
}
