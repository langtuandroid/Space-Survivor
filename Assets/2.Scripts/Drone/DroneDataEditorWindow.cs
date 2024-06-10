using UnityEditor;
using UnityEngine;

public class DroneDataEditorWindow : EditorWindow
{
    [MenuItem("Window/Drone Data")]
    public static void ShowWindow()
    {
        GetWindow<DroneDataEditorWindow>("Drone Data");
    }

    private Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("Drone Data", EditorStyles.boldLabel);
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        
        foreach (var droneType in System.Enum.GetValues(typeof(DroneDataDefine.AttackDroneType)))
        {
            DrawDroneData((DroneDataDefine.AttackDroneType)droneType);
        }

        GUILayout.EndScrollView();
    }

    private void DrawDroneData(DroneDataDefine.AttackDroneType droneType)
    {
        var droneData = DroneDataDefine.AttackDroneDic[droneType];

        GUILayout.BeginVertical("box");
        GUILayout.Label(droneType.ToString(), EditorStyles.boldLabel);

        GUILayout.Label("Damage:");
        GUILayout.BeginHorizontal();
        foreach (var dmg in droneData.damage)
        {
            GUILayout.Label(dmg.ToString(), GUILayout.Width(30));
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Attack Speed:");
        GUILayout.BeginHorizontal();
        foreach (var speed in droneData.attackSpeed)
        {
            GUILayout.Label(speed.ToString("0.00"), GUILayout.Width(50));
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.Space(10);
    }
}