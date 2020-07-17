//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void OnSceneIndexChanged(int sceneIndex);
public delegate void OnSceneNameChanged(string sceneName);

/// <summary>
/// 场景管理器
/// </summary>
public class SceneMgr : MonoBehaviour
{

    public Scene currentScene;

    // Use this for initialization
    void Start()
    {

    }

    /// <summary>
    /// 以异步-叠加方式加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onsceneChanged"></param>
    public void LoadSceneAdditiveAsync(string sceneName, OnSceneNameChanged onsceneChanged)
    {
        StartCoroutine(LoadTargetSceneAdditiveAsync(sceneName, onsceneChanged));
    }

    /// <summary>
    /// 以异步-叠加方式加载场景(携程调用)
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onsceneChanged"></param>
    /// <returns></returns>
    private IEnumerator LoadTargetSceneAdditiveAsync(string sceneName, OnSceneNameChanged onsceneChanged)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncOperation.isDone)
        {
            yield return asyncOperation;
        }
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        yield return null;
        currentScene = SceneManager.GetActiveScene();
        if (null != onsceneChanged)
        {
            onsceneChanged(sceneName);
        }
    }

    /// <summary>
    /// 以同步的方式加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            Debug.LogWarning(string.Format("名为{0}的场景已经加载过了！", sceneName));
        }
        SceneManager.LoadScene(sceneName);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        currentScene = SceneManager.GetActiveScene();
    }

    /// <summary>
    /// 以异步-单独的方式加载场景
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <param name="onSceneChanged"></param>
    public void LoadSceneAsync(int sceneIndex, OnSceneIndexChanged onSceneChanged)
    {
        StartCoroutine(LoadTargetSceneAsync(sceneIndex, onSceneChanged));
    }

    /// <summary>
    /// 以异步-单独的方式加载场景(携程调用)
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <param name="onSceneChanged"></param>
    /// <returns></returns>
    private IEnumerator LoadTargetSceneAsync(int sceneIndex, OnSceneIndexChanged onSceneChanged)
    {
        if (SceneManager.GetActiveScene().buildIndex == sceneIndex)
        {
            Debug.LogWarning(string.Format("索引为{0}的场景已经加载过了！", sceneIndex));
        }
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        while (!asyncOperation.isDone)
        {
            yield return asyncOperation;
        }
        Scene scene = SceneManager.GetSceneAt(sceneIndex);
        SceneManager.SetActiveScene(scene);
        yield return null;
        currentScene = SceneManager.GetActiveScene();
        if (null != onSceneChanged)
        {
            onSceneChanged(sceneIndex);
        }
    }

    /// <summary>
    /// 以异步-单独的方式加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onSceneChanged"></param>
    public void LoadSceneAsync(string sceneName, OnSceneNameChanged onSceneChanged)
    {
        StartCoroutine(LoadTargetSceneAsync(sceneName, onSceneChanged));
    }

    /// <summary>
    /// 以异步-单独的方式加载场景(携程调用)
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onSceneChanged"></param>
    /// <returns></returns>
    private IEnumerator LoadTargetSceneAsync(string sceneName, OnSceneNameChanged onSceneChanged)
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            Debug.LogWarning(string.Format("名为{0}的场景已经加载过了！", sceneName));
        }
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncOperation.isDone)
        {
            yield return asyncOperation;
        }
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        yield return null;
        currentScene = SceneManager.GetActiveScene();
        if (null != onSceneChanged)
        {
            onSceneChanged(sceneName);
        }
    }

    /// <summary>
    /// 以异步方式卸载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onSceneChanged"></param>
    public void UnloadSceneAsync(string sceneName, OnSceneNameChanged onSceneChanged)
    {
        StartCoroutine(UnloadTargeSceneAsync(sceneName, onSceneChanged));
    }

    /// <summary>
    /// 以异步方式卸载场景(携程调用)
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onSceneChanged"></param>
    /// <returns></returns>
    private IEnumerator UnloadTargeSceneAsync(string sceneName, OnSceneNameChanged onSceneChanged)
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            yield return asyncOperation;
        }
        currentScene = SceneManager.GetActiveScene();
        yield return null;
        if (null != onSceneChanged)
        {
            onSceneChanged(sceneName);
        }
    }
}
