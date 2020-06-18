using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Inscription))]
public class InscriptionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as Inscription;

        myScript.isQuestion = GUILayout.Toggle(myScript.isQuestion, "IsQuestion");
        myScript.inscriptionPoint = (Transform)EditorGUILayout.ObjectField("InscriptionPoint", myScript.inscriptionPoint, typeof(Transform), true);

        if (!myScript.isQuestion)
        {
            myScript.isReply = GUILayout.Toggle(myScript.isReply, "IsReply");
            myScript.text = EditorGUILayout.TextField("Text:", myScript.text);
            myScript.color = EditorGUILayout.ColorField("Color:", myScript.color);
        }
        else
        {
            myScript.isReply = false;
        }
    }
}
