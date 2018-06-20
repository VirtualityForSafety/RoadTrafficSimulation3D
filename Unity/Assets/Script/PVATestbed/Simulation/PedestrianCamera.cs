using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCPAR.SIM.DataLogging;

namespace SCPAR.SIM.PVATestbed
{
    public class PedestrianCamera : MonoBehaviour {
        public World world;
        public bool startRecording = false;
        public PedestrianPositioning positioning;
        public AccidentEffect accidentEffect;

        // Use this for initialization
        void Start() {
            world = GameObject.Find("World").GetComponent<World>();
            accidentEffect = GameObject.Find("AccidentEffect").GetComponent<AccidentEffect>();
            
            positioning = GetComponent<PedestrianPositioning>();
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("End of the road");
            if (other.GetComponent<SummonArea>() != null)
            {
                Debug.Log("The end!");
                if(positioning!=null)
                    positioning.initialize();
            }
            else if (other.GetComponent<Car>() != null)
            {
                Debug.Log("Accident!");
                if (accidentEffect != null)
                    accidentEffect.itsAccident();
            }
        }

        // Update is called once per frame
        void Update() {
            if (world != null && world.isReady)
            {
                if (startRecording)
                    GameObject.Find("FrameCapturer").GetComponent<FrameCapturer>().isCapturing = true;
            }
        }
    }
}
