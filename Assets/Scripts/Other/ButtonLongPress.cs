using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;
namespace Assets.Scripts.Other
{
    class ButtonLongPress : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject parent;
        private bool isUp;
        public void OnPointerClick(PointerEventData eventData)
        {
            
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isUp = false;
            StartCoroutine(grow());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isUp = true;
        }
        private IEnumerator grow()
        {
            while (true)
            {
                if (isUp)
                {
                    break;
                }
                else
                {
                    if (gameObject.name == "LeftSlideBtn")
                    {
                        parent.transform.position = new Vector3(parent.transform.position.x + 0.1f, parent.transform.position.y, parent.transform.position.z );
                    }
                    else if(gameObject.name == "RightSlideBtn")
                    {
                        parent.transform.position = new Vector3(parent.transform.position.x - 0.1f, parent.transform.position.y, parent.transform.position.z );
                    }
                    if (gameObject.name == "LeftSlideMgrBtn")
                    {
                        parent.transform.position = new Vector3(parent.transform.position.x + 0.1f, parent.transform.position.y, parent.transform.position.z );
                    }
                    else if (gameObject.name == "RightSlideMgrBtn")
                    {
                        parent.transform.position = new Vector3(parent.transform.position.x - 0.1f, parent.transform.position.y, parent.transform.position.z);
                    }
                }
                yield
                return null;
            }
        }
    }
}
