using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSignals {
    float flipMultiplier;
    float phaseOffset;
    float time;
    int stateNum;
    string[][] states;
    int statesLength = 4;

    public enum State { Red = 0, Green = 1 };
    /*
    public ControlSignals(Intersection intersection)
    {
        flipMultiplier = Random.Range(0,1);
        phaseOffset = 100 * Random.Range(0, 1);
        time = phaseOffset;
        stateNum = 0;

        states = new string[4][];
        states[0] = new string[] { "L", "", "L", "" };
        states[1] = new string[] { "FR", "", "FR", "" };
        states[2] = new string[] { "", "L", "", "L" };
        states[3] = new string[] { "", "FR", "", "FR" };
               
    }

    public float getFlipInterval()
    {
        return (0.1 + 0.05 * @flipMultiplier) * settings.lightsFlipInterval;
    }

    private State[] decode(string str)
    {
        State[] state = new State[3];
        state[0] = (State)((str.Contains("L"))? 1 : 0);
        state[1] = (State)((str.Contains("F")) ? 1 : 0);
        state[2] = (State)((str.Contains("R")) ? 1 : 0);
        return state;
    }

    public State getState()
    {
        State stringState = states[stateNum % statesLength];
        if (intersection.roads.length <= 2)
          stringState = ['LFR', 'LFR', 'LFR', 'LFR'];
      (@_decode x for x in stringState)
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    */
}

/*
 * 
class ControlSignals
  constructor: (@intersection) ->
    @flipMultiplier = random()
    @phaseOffset = 100 * random()
    @time = @phaseOffset
    @stateNum = 0

  @copy: (controlSignals, intersection) ->
    if !controlSignals?
      return new ControlSignals intersection
    result = Object.create ControlSignals::
    result.flipMultiplier = controlSignals.flipMultiplier
    result.time = result.phaseOffset = controlSignals.phaseOffset
    result.stateNum = 0
    result.intersection = intersection
    result

  toJSON: ->
    obj =
      flipMultiplier: @flipMultiplier
      phaseOffset: @phaseOffset

  states: [
    ['L', '', 'L', ''],
    ['FR', '', 'FR', ''],
    ['', 'L', '', 'L'],
    ['', 'FR', '', 'FR']
  ]

  @STATE = [RED: 0, GREEN: 1]

  @property 'flipInterval',
    get: -> (0.1 + 0.05 * @flipMultiplier) * settings.lightsFlipInterval

  _decode: (str) ->
    state = [0, 0, 0]
    state[0] = 1 if 'L' in str
    state[1] = 1 if 'F' in str
    state[2] = 1 if 'R' in str
    state

  @property 'state',
    get: ->
      stringState = @states[@stateNum % @states.length]
      if @intersection.roads.length <= 2
        stringState = ['LFR', 'LFR', 'LFR', 'LFR']
      (@_decode x for x in stringState)

  flip: ->
    @stateNum += 1

  onTick: (delta) =>
    @time += delta
    if @time > @flipInterval
      @flip()
      @time -= @flipInterval

    */