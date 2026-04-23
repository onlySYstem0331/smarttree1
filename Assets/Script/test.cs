using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject knowable;
    private Button targetButton;

    private void Awake()
    {
        // 获取挂载该脚本的 GameObject 上的 Button 组件
        targetButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // 当按钮所在的对象被激活时，开始协程
        StartCoroutine(DelayButtonActivation());
    }

    private IEnumerator DelayButtonActivation()
    {
        // 禁用按钮
        targetButton.interactable = false;

        // 等待15秒
        yield return new WaitForSeconds(3f);

        // 启用按钮
        targetButton.interactable = true;
    }

    public void Close()
    {
        knowable.SetActive(false);
        UIManager.Instance.Init = true;
        
    }


}
