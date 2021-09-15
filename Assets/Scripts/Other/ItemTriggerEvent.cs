using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Windows;

namespace Assets.Scripts.Other
{
    public class ItemTriggerEvent : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag =="CeShiItem")
            {
                if (ShiYanCeShiWindowData.Instance.GetFenLeiMoKuaiNameText == null)
                {
                    return;
                }
                ShiYanCeShiWindowData.Instance.GetFenLeiMoKuaiNameText.text = other.gameObject.transform.GetChild(0).GetComponent<Text>().text;
            }
        }
    }
}
