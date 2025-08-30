using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ebac.StateMachine
{
    public class StateBase
    {
        public virtual void OnStateEnter(params object[] objs)
        {
            Debug.Log("On state enter");
            //Player player = (Player)o;
            //player.canMove = true;
            //player.ChangeColor(Color.blue);

            //base.OnStateEnter(o);
        }

        public virtual void OnStateEnter(System.Enum state, SOPlayerSetup soPlayerSetup = null, Animator animator = null, string trigger = "")
        {
            Debug.Log("state = "+state);
            Debug.Log("state.ToString() = " + state.ToString());
            Debug.Log("Entrou em um novo estado");

            if (state.ToString().Equals("IDLE"))
            {
                Debug.Log("Entrou na condição IDLE");
                animator.SetBool(soPlayerSetup.boolRun, false);
            }
            else if (state.ToString().Equals("RUN"))
            {
                Debug.Log("Entrou na condição RUN");
                animator.SetBool(soPlayerSetup.boolRun, true);
            }
            else if (state.ToString().Equals("DEATH"))
            {
                Debug.Log("Entrou na condição DEATH");
                animator.SetTrigger(trigger);
            }
        }

        public virtual void OnStateStay()
        {
            Debug.Log("Mantendo o estado");
        }

        public virtual void OnStateExit()
        {
            Debug.Log("Saiu do estado atual");
        }

        public virtual void OnStateExit(System.Enum state, SOPlayerSetup soPlayerSetup = null, Animator animator = null)
        {
            Debug.Log("state = " + state);
            Debug.Log("Saiu do estado atual");

            if (state.ToString().Equals("IDLE"))
            {
                animator.SetBool(soPlayerSetup.boolRun, false);
            }
            else if (state.ToString().Equals("RUN"))
            {
                animator.SetBool(soPlayerSetup.boolRun, true);
            }
        }
    }
}