using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizTimer : MonoBehaviour
{
        [Header("=== 倒计时UI ===")]
        public Text countdownText; // 倒计时文本（显示：60s、59s...）

        [Header("=== 题目UI ===")]
        public Text questionText; // 题目文本

        [Header("=== 选项UI（建议4个按钮） ===")]
        public Button[] optionButtons; // 选项按钮数组
        public Text[] optionTexts; // 选项文字数组（对应每个按钮）

        // 倒计时总时长（固定60秒）
        private const float TotalTime = 60f;
        // 当前剩余时间
        private float currentTime;
        // 是否已经作答（防止重复点击/超时重复触发）
        private bool isAnswered;
        // 正确答案的索引（对应选项数组的下标）
        private int correctAnswerIndex;

        /// <summary>
        /// 答题完成回调事件（外部监听，返回：是否答对）
        /// 用法：quizTimer.OnAnswerComplete += (isCorrect) => { 你的逻辑 };
        /// </summary>
        public event Action<bool> OnAnswerComplete;

        void Start()
        {
            // 初始化状态
            isAnswered = false; // 初始未出题，禁止计时
                               // 给所有选项按钮绑定点击事件
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i; // 闭包缓存索引
                optionButtons[i].onClick.AddListener(() => OnOptionClick(index));
            }
        }

        void Update()
        {
            // 仅在「未作答 + 有剩余时间」时倒计时
            if (!isAnswered && currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                currentTime = Mathf.Max(0, currentTime); // 防止时间为负
                UpdateCountdownUI();

                // 超时：自动判错
                if (currentTime <= 0)
                {
                    FinishAnswer(false);
                }
            }
        }

        /// <summary>
        /// 【核心方法】外部调用：设置题目、选项、正确答案
        /// </summary>
        /// <param name="question">题目文字</param>
        /// <param name="options">选项数组（如：["A.选项1","B.选项2","C.选项3","D.选项4"]）</param>
        /// <param name="correctIndex">正确答案的索引（0=第一个选项，1=第二个...）</param>
        public void SetQuestionData(string question, string[] options, int correctIndex)
        {
            // 重置状态
            isAnswered = false;
            currentTime = TotalTime;
            correctAnswerIndex = correctIndex;

            // 更新题目UI
            questionText.text = question;

            // 更新选项UI（自动适配选项数量，多余按钮隐藏）
            for (int i = 0; i < optionTexts.Length; i++)
            {
                if (i < options.Length)
                {
                    optionTexts[i].text = options[i];
                    optionButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }

            // 刷新倒计时显示
            UpdateCountdownUI();
        }

        /// <summary>
        /// 选项点击事件
        /// </summary>
        /// <param name="clickIndex">点击的选项索引</param>
        private void OnOptionClick(int clickIndex)
        {
            if (isAnswered) return; // 已作答，禁止重复点击

            // 判断是否答对
            bool isCorrect = clickIndex == correctAnswerIndex;
            FinishAnswer(isCorrect);
        }

        /// <summary>
        /// 结束答题（统一处理：停止计时、触发回调）
        /// </summary>
        /// <param name="isCorrect">是否答对</param>
        private void FinishAnswer(bool isCorrect)
        {
            isAnswered = true;
            OnAnswerComplete?.Invoke(isCorrect); // 触发回调
            Debug.Log($"答题结果：{(isCorrect ? "答对" : "答错/超时")}");
        }

        /// <summary>
        /// 更新倒计时UI显示
        /// </summary>
        private void UpdateCountdownUI()
        {
            countdownText.text = $"{Mathf.Ceil(currentTime)} 秒";
        }

        /// <summary>
        /// 外部调用：重置答题器（用于下一题）
        /// </summary>
        public void ResetQuiz()
        {
            isAnswered = true;
            countdownText.text = "60 秒";
            questionText.text = "题目加载中...";
            foreach (var btn in optionButtons)
            {
                btn.gameObject.SetActive(true);
            }
        }
}
