using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemy
{
    public class EnemyWalk : EnemyBase
    {
        public GameObject enemyFigure;

        public GameObject[] waypoints;
        public float minDistance = 20f;
        public float speed = 10f;

        public bool _canWalk = true;

        private int _index = 0;
        private int _randomIndex = 0;

        public override void Update()
        {
            base.Update();

            Debug.Log("lookatPlayer = "+lookAtPlayer);

            Walk();
        }

        private void Walk()
        {
            if (waypoints.Length > 0 && _canWalk)
            {
                if (gameObject.transform.name == "Enemy_Boss")
                {
                    GoToRandomPoint();
                }
                else
                {
                    if (Vector3.Distance(enemyFigure.transform.position, waypoints[_index].transform.position) < minDistance)
                    {
                        ++_index;

                        if (_index >= waypoints.Length)
                        {
                            _index = 0;
                        }
                    }

                    if (_currentLife > 0)
                    {
                        transform.position = Vector3.MoveTowards(enemyFigure.transform.position, waypoints[_index].transform.position, Time.deltaTime * speed);
                        transform.LookAt(waypoints[_index].transform.position);
                    }
                }
            }
        }

        public void GoToRandomPoint()
        {
            _randomIndex = UnityEngine.Random.Range(0, waypoints.Length);
            StartCoroutine(GoToPointCoroutine(waypoints[_randomIndex].transform));
        }

        IEnumerator GoToPointCoroutine(Transform t)
        {
            while (Vector3.Distance(transform.position, t.position) > 1f)
            {
                transform.LookAt(waypoints[_randomIndex].transform.position);

                transform.position = Vector3.MoveTowards(transform.position, t.position, Time.deltaTime * speed);
                
                yield return new WaitForEndOfFrame();
            }
        }
    }
}