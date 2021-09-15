using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Other
{
    public class RotateImage : MonoBehaviour
    {
        private void Update()
        {
            gameObject.transform.Rotate(Vector3.forward * Time.deltaTime * 360);
        }
    }
}
