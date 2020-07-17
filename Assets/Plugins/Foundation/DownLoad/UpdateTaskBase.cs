//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ColaFramework.Foundation.DownLoad
{
    public class UpdateTaskBase
    {
        protected bool m_StopWhenFail = false;
        protected int m_totalCnt = 1;
        protected int m_currIdx = 0;
        protected int m_maxThread = 1;
        protected int m_doneNum = 0;

        List<WorkBase> m_works = new List<WorkBase>();
        List<WorkBase> m_working = new List<WorkBase>();

        // 子类将IsFail置成true，Working就会停止处理后续的任务
        protected bool IsFail
        {
            get; set;
        }


        public virtual void Reset()
        {
            ResetWorks();
        }

        protected void ResetWorks()
        {
            for (int i = 0; i < m_working.Count; i++)
            {
                m_working[i].Dispose();
            }
            m_working.Clear();
            m_works.Clear();
            m_currIdx = 0;
            m_maxThread = 1;
            m_doneNum = 0;
        }

        // 添加新任务
        protected void AddWork(WorkBase work)
        {
            m_works.Add(work);
        }

        // 任务完成
        protected void DoneWork(WorkBase work)
        {
            int idx = m_works.IndexOf(work);
            m_works.RemoveAt(idx);
        }

        protected virtual void OnWorkProgress(float value)
        {
        }

        protected virtual void OnWorkDone()
        {
        }

        protected void StartWork()
        {
            IsFail = false;
            m_totalCnt = m_works.Count;

            // 启动Loom线程数量相等的事务
            m_maxThread = Mathf.Clamp(m_totalCnt, 1, ColaLoom.maxThreads);
            TaskManager.Instance.Create(Working());
        }

        protected IEnumerator Working()
        {
            m_currIdx = 0;
            Debug.LogFormat("Working, works count:{0}", m_works.Count);
            while (true)
            {
                for (int i = m_working.Count - 1; i >= 0; i--)
                {
                    if (m_working[i].IsFinish)
                    {
                        m_doneNum++;
                        m_working[i].Dispose();
                        m_working.RemoveAt(i);
                    }
                    else
                    {
                        m_working[i].Update();
                    }
                }

                // 有完成的事务 通知外部做表现
                OnWorkProgress(m_doneNum * 1.0f / m_totalCnt);

                if (IsFail)
                {
                    Debug.Log("----有失败任务 停止后续处理----");
                    break;
                }


                // 判断是否全部完成
                if (m_doneNum >= m_totalCnt)
                {
                    break;
                }

                // 有空闲线程 开启新的任务
                int maxNum = m_maxThread - m_working.Count;
                for (int i = 0; i < maxNum && m_currIdx < m_totalCnt; i++)
                {
                    WorkBase work = m_works[m_currIdx];
                    ++m_currIdx;
                    m_working.Add(work);
                    work.Run();
                }
                yield return null;
            }
            OnWorkDone();
        }

        //清除文件夹
        protected void ClearDirectory(string path, string key)
        {
            try
            {
                // 清空解压目录
                PlayerPrefs.DeleteKey(key);
                FileHelper.RmDir(path);
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("删除数据失败: {0}, {1}", path, e.Message);
            }
        }

    }
}