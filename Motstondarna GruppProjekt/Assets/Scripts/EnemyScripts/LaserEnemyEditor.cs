using UnityEngine;
using UnityEditor;

//Max Script
#if (UNITY_EDITOR)
[CustomEditor(typeof(LaserEnemy))]
public class LaserEnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LaserEnemy enemy = (LaserEnemy)target;

        //En knapp f�r att testa om lasern funkar - Max
        if (GUILayout.Button("Toggle Lasers"))
        {
            if (enemy.lasersOn)
            {
                enemy.TurnOffLasers();
            }
            else
            {
                enemy.TurnOnLasers();
            }
        }
    }
}
#endif