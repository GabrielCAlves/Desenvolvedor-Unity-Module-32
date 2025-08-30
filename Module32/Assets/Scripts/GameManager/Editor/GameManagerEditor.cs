using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public bool showFoldout;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager gameManager = (GameManager) target;

        EditorGUILayout.Space(30);
        EditorGUILayout.LabelField("State Machine");

        if (gameManager.stateMachine == null)
        {
            return;
        }

        if (gameManager.stateMachine.CurrentState != null)
        {
            EditorGUILayout.LabelField("Current State: ", gameManager.stateMachine.CurrentState.ToString());
        }

        showFoldout = EditorGUILayout.Foldout(showFoldout, "Available States");

        if (showFoldout)
        {
            if (gameManager.stateMachine.dictionaryState != null)
            {
                var keys = gameManager.stateMachine.dictionaryState.Keys.ToArray();
                var vals = gameManager.stateMachine.dictionaryState.Values.ToArray();

                for (int i = 0; i < keys.Length; ++i)
                {
                    EditorGUILayout.LabelField($"{keys[i]} :: {vals[i]}");
                }
            }
        }
    }
}
