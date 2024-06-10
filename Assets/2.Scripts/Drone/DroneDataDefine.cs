using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DroneDataDefine
{
    public static Dictionary<AttackDroneType, AttackDroneData> AttackDroneDic = new Dictionary<AttackDroneType, AttackDroneData>()
    {
        { AttackDroneType.bullet, new BulletDroneData()},
{ AttackDroneType.missile, new MissileDroneData()},
{ AttackDroneType.laser, new LaserDroneData()}
    };

    [MenuItem("Test/AttackDrone")]
    public static void Print()
    {
        MonoBehaviour.print(AttackDroneDic[AttackDroneType.bullet].damage[10]);
    }


    #region AttackDrone

    public enum AttackDroneType
    {
        bullet,
        missile,
        laser
    }

    public abstract class AttackDroneData
    {
        public readonly int[] damage;

        public readonly float[] attackSpeed;

        protected AttackDroneData(int[] damage, float[] attackSpeed)
        {
            this.damage = damage;
            this.attackSpeed = attackSpeed;
        }
    }

    public class BulletDroneData : AttackDroneData
    {
        public BulletDroneData() : base(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
                new float[] { 3, 2.9f, 2.8f, 2.7f, 2.6f, 2.5f, 2.4f, 2.3f, 2.2f, 2.1f, 2.0f, 1.9f, 1.8f, 1.7f, 1.6f, 1.5f, 1.4f, 1.3f, 1.2f, 1.1f, 1, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, }
            )
        { }
    }

    public class MissileDroneData : AttackDroneData
    {
        public MissileDroneData() : base(
                new int[] { 5, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60 },
                new float[] { 5.0f, 4.86f, 4.72f, 4.58f, 4.44f, 4.3f, 4.16f, 4.02f, 3.88f, 3.74f, 3.6f, 3.46f, 3.32f, 3.18f, 3.04f, 2.9f, 2.76f, 2.62f, 2.48f, 2.34f, 2.2f, 2.06f, 1.92f, 1.78f, 1.64f, 1.5f, 1.36f, 1.22f, 1.08f, 0.94f }
            )
        { }
    }

    public class LaserDroneData : AttackDroneData
    {

        public LaserDroneData() : base(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
                new float[] { 1.5f, 1.45f, 1.4f, 1.35f, 1.3f, 1.25f, 1.2f, 1.15f, 1.1f, 1.05f, 1.0f, 0.95f, 0.9f, 0.85f, 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.55f, 0.5f, 0.45f, 0.4f, 0.35f, 0.3f, 0.25f, 0.2f, 0.15f, 0.1f, 0.1f }
            )
        { }
    }

    #endregion

    #region DefenceDrone

    #endregion

    #region AssistDrone

    #endregion
}
