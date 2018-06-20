using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class SummonArea : MonoBehaviour
    {
        public bool hasCarInside = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Car>() != null)
            {
                //Debug.Log("SummonArea has a car!");
                hasCarInside = true;
            }
            else
            {
                hasCarInside = false;
            }
                
        }
        private void OnTriggerExit(Collider other)
        {
            //Debug.Log("Car exits summon area!");
            hasCarInside = false;
        }
    }
}