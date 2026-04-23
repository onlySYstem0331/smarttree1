using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Animation_Control
{

    public static GameObject InstantiatePrefabAsChild(string PrefabPath, Vector3 Position, string Parent_Transfrom)
    {
        GameObject prefab = Resources.Load<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"未能加载位于 {PrefabPath} 的预制体，请检查资源路径。");
            return null;
        }

        GameObject father = GameObject.Find(Parent_Transfrom);
        if (father == null)
        {
            Debug.LogError($"未找到名为 {Parent_Transfrom} 的父物体。");
            return null;
        }
        Quaternion rotation = Quaternion.identity;
        GameObject temp = Object.Instantiate(prefab, Position, rotation);

        // 设置父物体
        temp.transform.SetParent(father.transform);

        // 设置局部位置
        temp.transform.localPosition = Position;

        return temp;
    }

}
