using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPAR.SIM.PVATestbed
{
    public class TrafficLightManager : MonoBehaviour
    {
        List<TrafficLight> trafficLights;
        int changeCount = 0;
        int changeInterval = SimParameter.trafficLightInterval;
        bool isOperating = false;
        public bool turnAllGreenLights = false;
        public bool turnAllRedLights = false;
        TrafficState[] currentStates;

        private void Start()
        {
            isOperating = false;
            currentStates = new TrafficState[4];
        }

        public void addTrafficLight(TrafficLight light)
        {
            if (trafficLights == null)
                trafficLights = new List<TrafficLight>();
            trafficLights.Add(light);
        }

        public void initialize()
        {
            isOperating = true;
        }

        void setAllTrafficLights(AbsDirection direction)
        {
            setTrafficState(direction);
            for (int i = 0; i < trafficLights.Count; i++)
            {
                if (trafficLights[i].getDirection() == direction)
                {
                    if (changeCount % changeInterval > changeInterval - SimParameter.warningInterval)
                        trafficLights[i].currentState = TrafficState.CarWarnPedStop;
                    else
                        trafficLights[i].currentState = TrafficState.CarGoPedStop;
                }
                else if (trafficLights[i].getDirection() == (AbsDirection)(((int)direction + 1) % 4))
                {
                    if (changeCount % changeInterval > changeInterval - SimParameter.warningInterval)
                        trafficLights[i].currentState = TrafficState.CarStopPedWarn;
                    else
                        trafficLights[i].currentState = TrafficState.CarStopPedGo;
                }
                else
                    trafficLights[i].currentState = TrafficState.CarStopPedStop;
            }
        }

        void setTrafficState(AbsDirection direction)
        {
            int index = (int)direction;
            if (changeCount % changeInterval > changeInterval - SimParameter.warningInterval)
                currentStates[index] = TrafficState.CarWarnPedStop;
            else
                currentStates[index] = TrafficState.CarGoPedStop;
            if (changeCount % changeInterval > changeInterval - SimParameter.warningInterval)
                currentStates[(index+1)%4] = TrafficState.CarGoPedStop;
            else
                currentStates[(index + 1) % 4] = TrafficState.CarStopPedGo;
            currentStates[(index + 2) % 4] = TrafficState.CarStopPedStop;
            currentStates[(index + 3) % 4] = TrafficState.CarStopPedStop;
        }

        public TrafficState getCurrentState(AbsDirection direction)
        {
            return currentStates[(int)direction];
        }

        void setAllTurnLightsOn(TrafficState givenState)
        {
            for (int i = 0; i < trafficLights.Count; i++)
            {
                trafficLights[i].currentState = givenState;
            }
            for(int i=0; i<4; i++)
            {
                currentStates[i] = givenState;
            }
        }

        private void FixedUpdate()
        {
            if (isOperating)
            {
                if (turnAllGreenLights)
                    setAllTurnLightsOn(TrafficState.CarGoPedStop);
                else if(turnAllRedLights)
                    setAllTurnLightsOn(TrafficState.CarStopPedStop);
                else
                {
                    if (changeCount >= changeInterval * 4)
                        changeCount = 0;
                    else if (changeCount >= changeInterval * 3)
                    {
                        setAllTrafficLights(AbsDirection.S);
                    }
                    else if (changeCount >= changeInterval * 2)
                    {
                        setAllTrafficLights(AbsDirection.W);
                    }
                    else if (changeCount >= changeInterval)
                    {
                        setAllTrafficLights(AbsDirection.N);
                    }
                    else
                    {
                        setAllTrafficLights(AbsDirection.E);
                    }

                    changeCount++;
                }                
            }
        }
    }
}