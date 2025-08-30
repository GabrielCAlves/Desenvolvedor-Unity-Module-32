using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ebac.StateMachine;
using System;

namespace Boss
{
    public class BossStates : StateBase
    {
        protected BossBase boss;

        public override void OnStateEnter(params object[] objs)
        {
            base.OnStateEnter(objs);

            boss = (BossBase) objs[0];
        }
    }

    public class BossStateInit : BossStates
    {
        public override void OnStateEnter(params object[] objs)
        {
            base.OnStateEnter(objs);

            boss.StartInitAnimation();
            Debug.Log("Boss: "+boss);
        }
    }

    public class BossStateWalk : BossStates
    {
        public override void OnStateEnter(params object[] objs)
        {
            base.OnStateEnter(objs);

            boss.GoToRandomPoint(OnArrive);
        }

        private void OnArrive()
        {
            boss.SwitchState(BossAction.ATTACK);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            Debug.Log("Exit Walk");
            boss.StopAllCoroutines();
        }
    }

    public class BossStateAttack : BossStates
    {
        public override void OnStateEnter(params object[] objs)
        {
            base.OnStateEnter(objs);

            boss.StartAttack(EndAttacks);
        }

        private void EndAttacks()
        {
            boss.SwitchState(BossAction.WALK);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            Debug.Log("Exit Attack");
            boss.StopAllCoroutines();
        }
    }

    public class BossStateDeath : BossStates
    {
        public override void OnStateEnter(params object[] objs)
        {
            base.OnStateEnter(objs);

            Debug.Log("Enter Death");
            boss.transform.localScale = Vector3.one * .2f;
        }
    }
}