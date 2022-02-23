using System;
using System.Linq;
using UnityEngine;

namespace DollHouseView
{
    public class PoiController : MonoBehaviour
    {
        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                foreach (var obj in FindObjectsOfType<MonoBehaviour>().OfType<IPoi>())
                {
                    obj.InstantiatePoi();
                }
            }
        }
    }
}