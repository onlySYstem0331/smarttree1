using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadExcel : MonoBehaviour
{
    // 单例模式的静态实例
    public static LoadExcel Instance;

    public TextAsset TextAsset;
    public List<string> stringList = new List<string>();
    public List<int> levelList = new List<int>();

    private void Awake()
    {
        // 确保单例实例的唯一性
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景切换时不被销毁
            Main();
        }
        else
        {
            Destroy(gameObject); // 如果已经存在实例，销毁当前对象
        }
    }

    void Main()
    {
        if (TextAsset == null)
        {
            Debug.LogError("TextAsset 未赋值，请在 Inspector 面板中指定一个文本资源。");
            return;
        }

        string[] data = TextAsset.text.Split('\n');

        foreach (string line in data)
        {
            int level = 0;
            // 统计字符串开头连续逗号的数量
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ',')
                {
                    level++;
                }
                else
                {
                    break;
                }
            }
            stringList.Add(line.Trim(','));
            levelList.Add(level);
        }

        // 输出存储的结果
        for (int i = 0; i < stringList.Count; i++)
        {
            Debug.Log($"字符串: {stringList[i]}, 层级: {levelList[i]}");
        }
    }

    public void ChangeScreen()
    {
        SceneManager.LoadScene("test4");
    }
}