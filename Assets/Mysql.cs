using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mysql 
{
    private string server;
    private string database;
    private string uid;
    private string password;
    private string connectionString;


    private static Mysql instance;

    // 单例访问属性
    public static Mysql Instance
    {
        get
        {
            if (instance == null)
            {
                // 初始化单例实例
                instance = new Mysql();
            }
            return instance;
        }
    }


    public void test()
    {
        server = "localhost";
        database = "bao";
        uid = "root";
        password = "123456";

        // 构建连接字符串
        connectionString = "SERVER=" + server + ";" +
                           "DATABASE=" + database + ";" +
                           "UID=" + uid + ";" +
                           "PASSWORD=" + password + ";";

        // 打开数据库连接
        using (MySqlConnection dbConnection = new MySqlConnection(connectionString))
        {
            try
            {
                dbConnection.Open();

                // 创建一个SQL命令
                using (MySqlCommand dbCommand = new MySqlCommand())
                {
                    dbCommand.Connection = dbConnection;
                    // 示例SQL查询
                    string sqlQuery = "SELECT * FROM bao";
                    dbCommand.CommandText = sqlQuery;

                    // 执行查询并获取结果
                    using (MySqlDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            // 读取数据
                            int id = dataReader.GetInt32(0);
                            string name = dataReader.GetString(1);
                            Debug.Log("ID: " + id + ", Name: " + name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("数据库连接出错: " + e.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }
    }
   
}