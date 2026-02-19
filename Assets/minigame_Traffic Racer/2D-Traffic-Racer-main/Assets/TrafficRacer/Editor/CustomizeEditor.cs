using UnityEngine;
using UnityEditor;

namespace MadFireOn
{
    [CustomEditor(typeof(TrafficGameManager))]
    public class CustomizeEditor : Editor
    {
        private Color defaultCol;

        public override void OnInspectorGUI()
        {
            defaultCol = GUI.color;

            TrafficGameManager myTarget = (TrafficGameManager)target;

            GUILayout.Space(5);
            myTarget.totalCars = EditorGUILayout.IntField("Total Cars", myTarget.totalCars);
            myTarget.totalCoins = EditorGUILayout.IntField("Total Coins", myTarget.totalCoins);
            //myTarget.turboTime = EditorGUILayout.FloatField("Turbo Time", myTarget.turboTime);
            //myTarget.magnetTime = EditorGUILayout.FloatField("Magnet Time", myTarget.magnetTime);
            //myTarget.doubleCoinTime = EditorGUILayout.FloatField("Double Coin Time", myTarget.doubleCoinTime);

            GUILayout.Space(5);

            GUILayout.Label("Click on Reset All after making changes to below fields");

            GUI.color = Color.green;
            if (GUILayout.Button("Reset All", GUILayout.Height(50)))
            {
                // FIXED: Changed this back to ResetGameManager() to match the actual method!
                myTarget.ResetGameManager(); 
            }

            GUI.color = defaultCol;
        }
    }
}