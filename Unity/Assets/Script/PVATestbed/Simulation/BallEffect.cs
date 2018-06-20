using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class BallEffect : MonoBehaviour
    {
        float alpha = 0f;
        // Use this for initialization
        void Start()
        {
            alpha = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (alpha >= 0 && alpha < 1.0f)
            {
                this.transform.GetComponent<Renderer>().material.color = new Color(this.transform.GetComponent<Renderer>().material.color.r, this.transform.GetComponent<Renderer>().material.color.g, this.transform.GetComponent<Renderer>().material.color.b, alpha);
                alpha += 0.01f;
            }
            else
                alpha = 1.0f;
        }
    }
}