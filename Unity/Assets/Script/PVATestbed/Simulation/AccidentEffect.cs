using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class AccidentEffect : MonoBehaviour
    {
        float alpha = 0f;
        // Use this for initialization
        void Start()
        {

        }

        public void itsAccident()
        {
            alpha = 1.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (alpha > 0)
            {
                this.transform.GetComponent<Renderer>().enabled = true;
                this.transform.GetComponent<Renderer>().material.color = new Color(this.transform.GetComponent<Renderer>().material.color.r, this.transform.GetComponent<Renderer>().material.color.g, this.transform.GetComponent<Renderer>().material.color.b, alpha);
                alpha -= 0.05f;
            }
            else
            {
                this.transform.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}