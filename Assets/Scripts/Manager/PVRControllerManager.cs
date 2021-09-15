using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.Manager
{
    class PVRControllerManager:Singleton<PVRControllerManager>
    {
        private GameObject LeftController;
        private GameObject RightController;
        private GameObject LeftCotrollerTips;
        private GameObject RightControllerTips;
        private GameObject LeftControllerRay;
        private GameObject RightControllerRay;
        public void InitController(GameObject LeftController, GameObject RightController,GameObject LeftCotrollerTips
            ,GameObject RightControllerTips, GameObject LeftControllerRay, GameObject RightControllerRay)
        { 
            this.LeftController = LeftController;
            this.RightController = RightController;
            this.LeftCotrollerTips = LeftCotrollerTips;
            this.RightControllerTips = RightControllerTips;
            this.LeftControllerRay = LeftControllerRay;
            this.RightControllerRay = RightControllerRay;
        }


        public void HideBothController()
        {
            //LeftController.transform.GetComponent<MeshRenderer>().enabled = false ;
            //RightController.transform.GetComponent<MeshRenderer>().enabled = false;
            //LeftCotrollerTips.gameObject.SetActive(false);
            //RightControllerTips.gameObject.SetActive(false);
            //LeftControllerRay.SetActive(false);
            //RightControllerRay.SetActive(false);
        }

        public void ShowBothController()
        {
            //LeftController.transform.GetComponent<MeshRenderer>().enabled = true;
           // RightController.transform.GetComponent<MeshRenderer>().enabled = true;
            //LeftCotrollerTips.gameObject.SetActive(true);
            //RightControllerTips.gameObject.SetActive(true);
            //LeftControllerRay.SetActive(true);
            //RightControllerRay.SetActive(true);
        }
    }
}
