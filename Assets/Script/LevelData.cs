using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FlatMapSize
{
    public int x, z;
}

[System.Serializable]
public class LevelData
{
    public FlatMapSize mapSize;
    public List<string> blocks;
    public Vector3Serializable playerPosition;
}

[System.Serializable]
public struct Vector3Serializable
{
    public float x, y, z;
    public Vector3 ToVector3() => new Vector3(x, y, z);
}