using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCreate : MonoBehaviour
{

    // 原有参数保留，只加这2个（碰撞修正用）
    [Header("碰撞防重叠")]
    public float overlapCorrectionDistance = 0.2f; // 每次挪开的距离
    public int maxCorrectionSteps = 5; // 最大修正步数（防止无限挪）
    //  面板可显示：同时存 节点层级(int) + 文本(string)
    [System.Serializable]
    public struct NodeData
    {
        public int level;   // 原0/1/2（由前置逗号数决定）
        public string text; // 表格里的文本（逗号间的内容，无逗号）
    }

    //  替换旧数组：Inspector面板直接编辑、显示
    public List<NodeData> nodeDataList;

    public GameObject nodePrefab;
    public float radiusLevel1 = 3f; // 0→1 半径
    public float radiusLevel2 = 6f; // 1→2 半径
    public Material lineMaterial;
    public TextAsset csvFile;

    public List<GameObject> allCreatedNodes = new List<GameObject>();
    private List<LineRenderer> allLines = new List<LineRenderer>();
    private GameObject linesParent;

    IEnumerator Start()
    {
        ParseCSVToNodeSequence();

        if (nodePrefab == null)
        {
            Debug.LogError("Node prefab is not assigned.");
            yield break;
        }

        linesParent = new GameObject("LinesParent");
        linesParent.transform.SetParent(transform);

        GenerateTree(); // 第一步：先生成所有节点
        ModifyTestNumbers(); // 先赋值编号

        yield return StartCoroutine(FixNodeOverlaps());

        DrawLinesBetweenAllParentAndChildren();

        Debug.Log("✅ 流程完成：节点生成→小球修正→线条绘制");
    }

    void GenerateTree()
    {
        List<Transform> level1Parents = new List<Transform>();
        Dictionary<Transform, int> parentIndexMap = new Dictionary<Transform, int>();
        Dictionary<Transform, List<Transform>> level2Children = new Dictionary<Transform, List<Transform>>();

        Transform root = null;
        Transform currentParent = null;

        for (int i = 0; i < nodeDataList.Count; i++)
        {
            int nodeLevel = nodeDataList[i].level;
            GameObject newNode = Instantiate(nodePrefab);
            allCreatedNodes.Add(newNode);
            newNode.transform.SetParent(transform);

            color_change_2 nodeScript = newNode.GetComponentInChildren<color_change_2>();
            if (nodeScript != null)
                nodeScript.nodeText = nodeDataList[i].text;

            if (nodeLevel == 0)
            {
                root = newNode.transform;
                root.position = Vector3.zero;
                newNode.name = $"Node_0_{nodeDataList[i].text}";
                currentParent = root;
            }
            else if (nodeLevel == 1)
            {
                float currentRadius;
                if (level1Parents.Count % 2 == 0)
                {
                    currentRadius = radiusLevel1; // 偶数个：还原原半径
                }
                else
                {
                    currentRadius = radiusLevel1 * 0.5f; // 奇数个：切半
                }

                // 原有逻辑完全不变，只用新的currentRadius计算位置
                float totalLevel1 = GetNodeCount(1);
                float angle = 2 * Mathf.PI * level1Parents.Count / totalLevel1;
                // 🔥 替换radiusLevel1为currentRadius
                Vector3 position = new Vector3(Mathf.Cos(angle) * currentRadius, Mathf.Sin(angle) * currentRadius, 0);

                newNode.transform.position = position;
                newNode.transform.SetParent(root);
                newNode.name = $"Node_1_{level1Parents.Count}_{nodeDataList[i].text}";

                level1Parents.Add(newNode.transform);
                parentIndexMap[newNode.transform] = i;
                level2Children[newNode.transform] = new List<Transform>();
                currentParent = newNode.transform;
            }
            else if (nodeLevel == 2)
            {
                if (currentParent != null && level2Children.ContainsKey(currentParent))
                {
                    List<Transform> children = level2Children[currentParent];
                    int totalChild = GetChildCountByIndex(parentIndexMap[currentParent]);
                    if (totalChild == 0) totalChild = 1;

                    float parentAngle = Mathf.Atan2(currentParent.position.y, currentParent.position.x);
                    float maxUpAngle = parentAngle + Mathf.PI / 2;
                    float maxDownAngle = parentAngle - Mathf.PI / 2;

                    float t = 0;
                    if (totalChild > 1)
                    {
                        t = (float)children.Count / (totalChild - 1);
                    }
                    float finalAngle = Mathf.Lerp(maxUpAngle, maxDownAngle, t);

                    Vector3 position = currentParent.position + new Vector3(
                        Mathf.Cos(finalAngle) * radiusLevel2,
                        Mathf.Sin(finalAngle) * radiusLevel2,
                        0
                    );

                    newNode.transform.position = position;
                    newNode.transform.SetParent(currentParent);
                    newNode.name = $"Node_2_{children.Count}_{nodeDataList[i].text}";
                    children.Add(newNode.transform);
                }
            }
        }
    }

    int GetChildCountByIndex(int parentIndex)
    {
        int count = 0;
        for (int i = parentIndex + 1; i < nodeDataList.Count; i++)
        {
            if (nodeDataList[i].level == 2) count++;
            else if (nodeDataList[i].level == 1) break;
        }
        return count;
    }

    void DrawLinesBetweenAllParentAndChildren()
    {
        foreach (GameObject node in allCreatedNodes)
        {
            Transform parent = node.transform.parent;
            if (parent != null && parent != transform)
            {
                CreateLine(parent, node.transform);
            }
        }
    }

    LineRenderer CreateLine(Transform start, Transform end)
    {
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(linesParent.transform);
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);
        allLines.Add(lineRenderer);
        return lineRenderer;
    }

    int GetNodeCount(int level)
    {
        int count = 0;
        foreach (var data in nodeDataList)
        {
            if (data.level == level) count++;
        }
        return count;
    }
    void ParseCSVToNodeSequence()    //文件读取操作(写入到nodedata之中)
    {
        // 初始化新数据列表
        nodeDataList = new List<NodeData>();

        if (csvFile == null)
        {
            Debug.LogError("未拖入CSV文件！");
            return;
        }

        // 按回车分割所有行
        string[] lines = csvFile.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 逐行解析
        foreach (string line in lines)
        {
            NodeData data = new NodeData();
            int commaCount = 0;
            int textStartIndex = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ',')
                {
                    commaCount++;
                    textStartIndex = i + 1;
                }
                else
                {
                    break; // 遇到非逗号，停止统计前置逗号
                }
            }

            // 赋值层级：0个逗号=0，1个=1，2个=2
            data.level = Mathf.Min(commaCount, 2);

            string textContent = "";
            if (textStartIndex < line.Length)
            {
                // 找下一个逗号的位置（截断用）
                int nextCommaIndex = line.IndexOf(',', textStartIndex);
                if (nextCommaIndex == -1)
                {
                    // 无后续逗号 → 取到行尾
                    textContent = line.Substring(textStartIndex);
                }
                else
                {
                    // 有后续逗号 → 取中间文本
                    textContent = line.Substring(textStartIndex, nextCommaIndex - textStartIndex);
                }
            }
            data.text = textContent.Trim(); // 去空格，更整洁

            // 加入数据列表
            nodeDataList.Add(data);
        }

        Debug.Log($"CSV解析完成，共{nodeDataList.Count}条数据：\n" + string.Join("\n", nodeDataList.ConvertAll(d => $"Level:{d.level}, Text:{d.text}")));
    }

    void ModifyTestNumbers()
    {
        int temp = 0;
        foreach (GameObject node in allCreatedNodes)
        {
            Transform circleChild = node.transform.Find("Circle");
            if (circleChild != null)
            {
                color_change_2 colorChangeScript = circleChild.GetComponent<color_change_2>();
                if (colorChangeScript != null)
                {
                    colorChangeScript.TestNumber = temp;
                    temp++;
                }
            }
        }
    }

    //  核心：碰撞检测+自动挪开节点（彻底防重叠）
    // 🔥 修复API错误 + 兼容「父空物体+子Circle」结构 彻底解决重叠
    IEnumerator FixNodeOverlaps()
    {
        Debug.Log("开始检测节点重叠并修正...");

        // 存储：节点父物体 + 小球的碰撞体
        List<(GameObject nodeParent, Collider2D ballCollider)> nodeColliders = new List<(GameObject, Collider2D)>();

        foreach (GameObject nodeParent in allCreatedNodes)
        {
            // 找到你的小球子物体(名字Circle，和你原有代码完全一致)
            Transform circleTrans = nodeParent.transform.Find("Circle");
            if (circleTrans == null) continue;

            Collider2D col = circleTrans.GetComponent<Collider2D>();
            if (col != null)
            {
                nodeColliders.Add((nodeParent, col));
            }
        }

        // 初始化碰撞过滤（不设置会报错）
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.NoFilter(); // 不过滤，检测所有碰撞

        // 多步修正，彻底分开重叠节点
        for (int step = 0; step < maxCorrectionSteps; step++)
        {
            bool hasOverlap = false;

            foreach (var item in nodeColliders)
            {
                GameObject parent = item.nodeParent;
                Collider2D ballCol = item.ballCollider;
                Collider2D[] overlaps = new Collider2D[5]; // 存储重叠结果

                //  修复API：返回int重叠数量，不再报类型转换错
                int overlapCount = Physics2D.OverlapCollider(ballCol, contactFilter, overlaps);

                // 遍历所有真正重叠的物体
                for (int i = 0; i < overlapCount; i++)
                {
                    Collider2D overlapCol = overlaps[i];
                    // 跳过自己 & 只处理小球
                    if (overlapCol == ballCol || overlapCol.transform.name != "Circle")
                        continue;

                    // 计算推开方向，移动【父物体】（保证结构正确）
                    Vector2 dir = (parent.transform.position - overlapCol.transform.position).normalized;
                    parent.transform.Translate(dir * overlapCorrectionDistance);
                    hasOverlap = true;
                }
            }

            if (!hasOverlap) break; // 无重叠直接退出
            yield return null;
        }

        Debug.Log("节点重叠修正完成！");
    }
}