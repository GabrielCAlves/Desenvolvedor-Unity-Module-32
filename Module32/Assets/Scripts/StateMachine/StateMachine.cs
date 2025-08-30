using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace Ebac.StateMachine
{
    public class StateMachine<T> where T : System.Enum
    {
        public Dictionary<T, StateBase> dictionaryState;

        private StateBase _currentState;

        public float timeToStartGame = 1f;

        public StateBase CurrentState
        {
            get { return _currentState; }
        }

        //public StateMachine(T state)
        //{
        //    //Init();
        //    dictionaryState = new Dictionary<T, StateBase>();
        //    SwitchState(state);
        //}

        public void Init()
        {
            dictionaryState = new Dictionary<T, StateBase>();
        }

        public void RegisterStates(T typeEnum, StateBase state)
        {
            Debug.Log("typeEnum: "+typeEnum);
            Debug.Log("Tipo do typeEnum: ");
            dictionaryState.Add(typeEnum, state);
        }

        public void SwitchState(T state, SOPlayerSetup soPlayerSetup = null, Animator animator = null, string trigger = "")
        {
            if (_currentState != null)
            {
                _currentState.OnStateExit(state, soPlayerSetup, animator);
            }

            _currentState = dictionaryState[state];

            _currentState.OnStateEnter(state, soPlayerSetup, animator, trigger);
        }

        public void SwitchState(T state, params object[] objs)
        {
            if (_currentState != null)
            {
                _currentState.OnStateExit(state);
            }

            _currentState = dictionaryState[state];

            _currentState.OnStateEnter(objs);
        }

        public void Update()
        {
            if (_currentState != null)
            {
                _currentState.OnStateStay();
            }
        }
    }
}