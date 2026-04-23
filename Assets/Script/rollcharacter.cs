using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class rollcharacter : MonoBehaviour
{

    [SerializeField] bool IfFinish = false;
    [SerializeField] bool IfJump = false;
    public int Dialog_index;

    public TextAsset peibiao;
    public TMP_Text textComponent;
    public float delayBetweenChars = 0.1f;
    public string[] dialog_All;
    public string[][] dialog_Tail;
    public int index = 0;
    public GameObject GameStory;

    public List<string> characterNames = new List<string>();             //角色名
    public List<Sprite> characterImages = new List<Sprite>();            //角色图片


    private Dictionary<string, Sprite> CharacterName_Sprites;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        dialog_All = peibiao.text.Split('#');


        dialog_Tail = new string[dialog_All.Length][];
        for (int i = 0; i < dialog_All.Length; i++)
        {
            // 使用 Split 方法分割每个对话字符串，并将结果赋给 dialog_Tail 的相应位置
            dialog_Tail[i] = dialog_All[i].Split('\n');
        }
        CharacterName_Sprites = new Dictionary<string, Sprite>();
        for (int i = 0; i < characterNames.Count; i++)
        {
            if (!CharacterName_Sprites.ContainsKey(characterNames[i]))
            {
                CharacterName_Sprites.Add(characterNames[i], characterImages[i]);
            }
        }
    }

    private void Update()
    {
        Talking_Begin(Dialog_index - 1);
    }

    IEnumerator ShowText(int i)
    {
        IfFinish = false;
        textComponent.text = ""; // 初始化文本组件内容
        int letter = 0;

        // 从对话中提取角色名，假设对话格式为 "角色名: 对话内容"
        string fullDialog = dialog_Tail[i][index];
        string characterName = fullDialog.Split('：')[0].Trim();
        string dialogWithoutName = fullDialog.Substring(characterName.Length + 1).Trim(); // 移除角色名和冒号


        // 逐字符显示对话内容
        while (!IfJump && letter < dialogWithoutName.Length)
        {
            textComponent.text += dialogWithoutName[letter];
            letter++;
            yield return new WaitForSeconds(delayBetweenChars);
        }

        IfJump = false;
        textComponent.text = dialogWithoutName;
        IfFinish = true;
    }



    public void Talking_Begin(int i)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (IfFinish)
            {
                index++;
                if (index >= dialog_Tail[i].Length - 1)
                {
                    index = 0;
                    GameStory.SetActive(false);
                }
                else
                {
                    textComponent.text = "";
                    StartCoroutine(ShowText(i));
                }

            }
            else if (!IfJump && !IfFinish)
            {
                IfJump = true;
            }
        }
    }
}
