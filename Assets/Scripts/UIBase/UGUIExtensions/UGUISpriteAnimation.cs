//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 2DSprite序列帧动画组件
/// </summary>

public class UGUISpriteAnimation : MonoBehaviour
{
	/// <summary>
	/// 帧率：每秒多少帧(支持正播和倒播，帧率为正正播，帧率为负倒播)
	/// </summary>

	[SerializeField] protected int framerate = 20;
	
	/// <summary>
	/// 动画是否收到TimeScale影响
	/// </summary>

	public bool ignoreTimeScale = true;

	/// <summary>
	/// 动画是否循环播放
	/// </summary>

	public bool loop = true;

	/// <summary>
	/// 用于播放的Sprite数据
	/// </summary>

	public Sprite[] frames;

    /// <summary>
    /// 承载Sprite的Image组件
    /// </summary>
    public Image image;

	int mIndex = 0;
	float mUpdate = 0f;

	/// <summary>
	/// 动画是否在播放中
	/// </summary>

	public bool isPlaying { get { return enabled; } }

	/// <summary>
	/// 动画每秒的帧率
	/// </summary>

	public int framesPerSecond { get { return framerate; } set { framerate = value; } }

	/// <summary>
	/// 播放动画/如果动画播放到结尾了会从头开始重新播放
	/// </summary>

	public void Play ()
	{
		if (frames != null && frames.Length > 0)
		{
			if (!enabled && !loop)
			{
				int newIndex = framerate > 0 ? mIndex + 1 : mIndex - 1;
				if (newIndex < 0 || newIndex >= frames.Length)
					mIndex = framerate < 0 ? frames.Length - 1 : 0;
			}
			
			enabled = true;
			UpdateSprite();
		}
	}

	/// <summary>
	/// 暂停动画
	/// </summary>

	public void Pause () { enabled = false; }

	/// <summary>
	/// 将动画重置为开始第一帧
	/// </summary>

	public void ResetToBeginning ()
	{
		mIndex = framerate < 0 ? frames.Length - 1 : 0;
		UpdateSprite();
	}


	void Start () { Play(); }


	void Update ()
	{
		if (frames == null || frames.Length == 0)
		{
			enabled = false;
		}
		else if (framerate != 0)
		{
            float time = ignoreTimeScale ? Time.unscaledTime : Time.time;

			if (mUpdate < time)
			{
				mUpdate = time;
				int newIndex = framerate > 0 ? mIndex + 1 : mIndex - 1;

				if (!loop && (newIndex < 0 || newIndex >= frames.Length))
				{
					enabled = false;
					return;
				}

				mIndex = RepeatIndex(newIndex, frames.Length);
				UpdateSprite();
			}
		}
	}

    /// <summary>
    /// 循环重置index
    /// </summary>
    /// <param name="val"></param>
    /// <param name="max"></param>
    /// <returns></returns>
     public int RepeatIndex(int val, int max)
    {
        if (max < 1) return 0;
        while (val < 0) val += max;
        while (val >= max) val -= max;
        return val;
    }

	/// <summary>
	/// 刷新展示Sprite
	/// </summary>

     void UpdateSprite()
     {
         if (image == null)
         {
             image = GetComponent<Image>();
             if (image == null)
             {
                 enabled = false;
                 return;
             }
         }

         float time = ignoreTimeScale ? Time.unscaledTime : Time.time;
         if (framerate != 0) mUpdate = time + Mathf.Abs(1f / framerate);

         if (image != null)
         {
             image.sprite = frames[mIndex];
             image.SetNativeSize();
         }
     }
}
