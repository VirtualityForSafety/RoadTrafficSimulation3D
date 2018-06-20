using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public enum TrafficState { CarGoPedStop = 0, CarWarnPedStop = 1, CarStopPedStop = 2, CarStopPedGo = 3, CarStopPedWarn = 4 }
    public class TrafficLight : MonoBehaviour
    {
        public TrafficState currentState;
        GameObject carRed;
        GameObject carYellow;
        GameObject carGreen;
        GameObject carGreenLeft;
        GameObject pedRed;
        GameObject pedGreen;
        AbsDirection direction;
        int blinkInterval= SimParameter.crossingBlinkInterval;
        int blinkCount;

        // Use this for initialization
        void Start()
        {
            blinkCount = 0;
            currentState = TrafficState.CarStopPedWarn;
            carRed = this.transform.Find("CarRed").gameObject;
            carYellow = this.transform.Find("CarYellow").gameObject;
            carGreen = this.transform.Find("CarGreen").gameObject;
            carGreenLeft = this.transform.Find("CarGreenLeft").gameObject;
            pedRed = this.transform.Find("PedRed").gameObject;
            pedGreen = this.transform.Find("PedGreen").gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if(currentState == TrafficState.CarStopPedGo)
            {
                carRed.GetComponent<Renderer>().enabled = true;
                carYellow.GetComponent<Renderer>().enabled = false;
                carGreen.GetComponent<Renderer>().enabled = false;
                carGreenLeft.GetComponent<Renderer>().enabled = false;
                pedRed.GetComponent<Renderer>().enabled = false;
                pedGreen.GetComponent<Renderer>().enabled = true;
            }
            else if(currentState == TrafficState.CarWarnPedStop)
            {
                carRed.GetComponent<Renderer>().enabled = false;
                carYellow.GetComponent<Renderer>().enabled = true;
                carGreen.GetComponent<Renderer>().enabled = false;
                carGreenLeft.GetComponent<Renderer>().enabled = false;
                pedRed.GetComponent<Renderer>().enabled = true;
                pedGreen.GetComponent<Renderer>().enabled = false;
            }
            else if (currentState == TrafficState.CarStopPedStop)
            {
                carRed.GetComponent<Renderer>().enabled = true;
                carYellow.GetComponent<Renderer>().enabled = false;
                carGreen.GetComponent<Renderer>().enabled = false;
                carGreenLeft.GetComponent<Renderer>().enabled = false;
                pedRed.GetComponent<Renderer>().enabled = true;
                pedGreen.GetComponent<Renderer>().enabled = false;
            }
            else if(currentState == TrafficState.CarGoPedStop)
            {
                carRed.GetComponent<Renderer>().enabled = false;
                carYellow.GetComponent<Renderer>().enabled = false;
                carGreen.GetComponent<Renderer>().enabled = true;
                carGreenLeft.GetComponent<Renderer>().enabled = true;
                pedRed.GetComponent<Renderer>().enabled = true;
                pedGreen.GetComponent<Renderer>().enabled = false;
            }
            else if (currentState == TrafficState.CarStopPedWarn)
            {
                carRed.GetComponent<Renderer>().enabled = true;
                carYellow.GetComponent<Renderer>().enabled = false;
                carGreen.GetComponent<Renderer>().enabled = false;
                carGreenLeft.GetComponent<Renderer>().enabled = false;
                pedRed.GetComponent<Renderer>().enabled = false;
                if (blinkCount > blinkInterval*2)
                {
                    pedGreen.GetComponent<Renderer>().enabled = true;
                    blinkCount = 0;
                }
                else if (blinkCount > blinkInterval)
                {
                    pedGreen.GetComponent<Renderer>().enabled = false;
                }
            }
            blinkCount++;
        }

        public void setDirection(AbsDirection _direction)
        {
            direction = _direction;
        }

        public AbsDirection getDirection()
        {
            return direction;
        }
    }

}
