using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Assets.Scripts.Other
{
    class AutoRollEditionName : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private float speed = 0.01f;
        public ScrollRect scrollRect;
        private bool beginMove = false;
        private bool Reset = false;
        public Image image;
        public Transform content;
        void Awake()
        {
        }
        void Update()
        {
            if (beginMove)
            {
                transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z );
                if (/*Mathf.Abs( */scrollRect.horizontalNormalizedPosition/*)*/>= 1f)
                {
                    beginMove = false;
                }
            }
            else
            {
                if (scrollRect == null)
                {
                    return;
                }
                transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
                if (/*Mathf.Abs( */scrollRect.horizontalNormalizedPosition/*)*/>= 1f)
                {
                    beginMove = false;
                }
            }
            if (Pvr_UnitySDKAPI.Controller.UPvr_GetAxis2D(1).y<0)
            {
                content.position = new Vector3(content.position.x, content.position.y+0.1f*speed, content.position.z);
            }
            else if (Pvr_UnitySDKAPI.Controller.UPvr_GetAxis2D(1).y > 0)
            {
                content.position = new Vector3(content.position.x, content.position.y - 0.1f*speed, content.position.z);
            }
            if (Pvr_UnitySDKAPI.Controller.UPvr_GetAxis2D(0).y < 0)
            {
                content.position = new Vector3(content.position.x, content.position.y + 0.1f * speed, content.position.z);
            }
            else if (Pvr_UnitySDKAPI.Controller.UPvr_GetAxis2D(0).y > 0)
            {
                content.position = new Vector3(content.position.x, content.position.y - 0.1f * speed, content.position.z);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (scrollRect == null)
            {
                return;
            }
            beginMove = true;
            if (image == null)
            {
                return;
            }
            image.sprite = Resources.Load<Sprite>("WindowUI/下拉-hov_sel");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (scrollRect == null)
            {
                return;
            }
            beginMove = false;
            Reset = true;
            if (image == null)
            {
                return;
            }
            image.sprite = Resources.Load<Sprite>("WindowUI/透明底");
        }
    }
}
