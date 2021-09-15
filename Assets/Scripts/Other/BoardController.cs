using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace Assets.Scripts.Other
{
    class BoardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject Board;
        public void OnPointerEnter(PointerEventData eventData)
        {
            Board.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Board.SetActive(false);
        }
    }
}
