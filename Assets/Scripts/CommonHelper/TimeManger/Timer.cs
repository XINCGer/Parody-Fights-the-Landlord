//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System;

namespace ColaFramework
{
    /// <summary>
    /// Allows you to run events on a delay without the use of <see cref="Coroutine"/>s
    /// or <see cref="MonoBehaviour"/>s.
    /// Timer provides count, countdown, Tick and other functions
    /// To create and start a Timer, use the <see cref="Register"/> method.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// How long the timer takes to complete from start to finish.
        /// </summary>
        private float _duration;
        private Action _onComplete;
        private Action<float> _onPerFrame;
        private Action<float> _onPerSecond;
        private Action<float> _onCountDown;

        /// <summary>
        /// Whether the timer will run again after completion.
        /// </summary>
        private bool _isLoop;

        /// <summary>
        /// Whether the timer uses real-time or game-time. Real time is unaffected by changes to the timescale
        /// of the game(e.g. pausing, slow-mo), while game time is affected.
        /// </summary>
        private bool _usesRealTime;

        /// <summary>
        /// the Gameobject bind to this timer.The timer will be destoryed On Update when the bind gameobject is null.
        /// </summary>
        private GameObject _autoDestroyOwner;

        /// <summary>
        /// ID bind to this timer
        /// </summary>
        public long UseId { get; set; }

        /// <summary>
        /// Whether or not the timer was cancelled.
        /// </summary>
        public bool IsCancelled { get; set; }
        private bool _hasAutoDestroyOwner;

        /// <summary>
        /// Initial value of timer ID alloc
        /// </summary>
        private static long _allocUseId = 1;

        /// <summary>
        /// Alloc the timer ID by grow up 1 each time.
        /// </summary>
        public static long AllocUseId
        {
            get { return ++_allocUseId; }
        }

        private float _startTime = 0f;
        private float _elapsedTimeTemp = 0f;
        private float _elapsedSecond = 0f;

        /// <summary>
        /// CountDown function, onComplete will be fired in duration,onCountDown will be fired per second.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <param name="onCountDown"></param>
        /// <returns></returns>
        public static long CountDown(float duration,
            Action onComplete,
            Action<float> onCountDown)
        {
            return Register(duration, onComplete, null, null, onCountDown);
        }

        /// <summary>
        /// Wait or TimeOut function. onComplete will be fired in duration.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static long Wait(float duration,
            Action onComplete)
        {
            return Register(duration, onComplete);
        }

        /// <summary>
        /// RunPerFrame function. onPerFrame will be fired in per frame.
        /// </summary>
        /// <param name="onPerFrame"></param>
        /// <returns></returns>
        public static long RunPerFrame(Action<float> onPerFrame)
        {
            return Register(-1, null, onPerFrame);
        }

        /// <summary>
        /// RunPerFrame function. onPerFrame will be fired in per frame.
        /// The timer will be destoryed On Update when the bind gameobject is null.
        /// </summary>
        /// <param name="onPerFrame"></param>
        /// <param name="autoDestroyOwner"></param>
        /// <returns></returns>
        public static long RunPerFrame(Action<float> onPerFrame, GameObject autoDestroyOwner)
        {
            return Register(-1,
                null,
                onPerFrame,
                null,
                null,
                false,
                false,
                autoDestroyOwner);
        }

        /// <summary>
        /// RunPerSecond function. onPerSecond will be fired in per second.
        /// The timer will be destoryed On Update when the bind gameobject is null.
        /// </summary>
        /// <param name="onPerSecond"></param>
        /// <param name="autoDestroyOwner"></param>
        /// <returns></returns>
        public static long RunPerSecond(Action<float> onPerSecond, GameObject autoDestroyOwner)
        {
            return Register(-1,
                null,
                null,
                onPerSecond,
                null,
                false,
                false,
                autoDestroyOwner);
        }

        /// <summary>
        /// RunBySecond function. onComplete will be fired in per duration
        /// The timer will be destoryed On Update when the bind gameobject is null.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <param name="autoDestroyOwner"></param>
        /// <returns></returns>
        public static long RunBySeconds(float duration, Action onComplete, GameObject autoDestroyOwner)
        {
            return Register(duration, onComplete, null, null, null, true, false, autoDestroyOwner);
        }

        /// <summary>
        /// Register a new timer that should fire an event after a certain amount of time
        /// has elapsed
        /// Registered timers are destroyed when the scene changes.
        /// <returns>A timer object that allows you to examine stats and stop/resume progress.</returns>
        /// <param name="duration">The time to wait before the timer should fire, in seconds.</param>
        /// <param name="onComplete">An action to fire when the timer completes.</param>
        /// <param name="onPerFrame">An action that should fire each frame the timer is updated.</param>
        /// <param name="onPerSecond">An action that should fire each time the timer is updated. Takes the amount
        /// of time passed in seconds since the start of the timer's current loop</param>
        /// <param name="onCountDown">An action that should fire each time the timer is updated. Takes the amount
        /// of time passed in seconds since the start of the timer's current loop</param>
        /// <param name="isLoop">Whether the timer should repeat after executing.</param>
        /// <param name="useRealTime">Whether the timer uses real-time(i.e. not affected by pauses,
        /// slow/fast motion) or game-time(will be affected by pauses and slow/fast-motion).</param>
        /// <param name="autoDestroyOwner">An object to attach this timer to. After the object is destroyed,
        /// the timer will expire and not execute. This allows you to avoid annoying <see cref="NullReferenceException"/>s
        /// by preventing the timer from running and accessessing its parents' components
        /// after the parent has been destroyed.</param>
        /// <returns></returns>
        public static long Register(float duration = 0,
            Action onComplete = null,
            Action<float> onPerFrame = null,
            Action<float> onPerSecond = null,
            Action<float> onCountDown = null,
            bool isLoop = false,
            bool useRealTime = false,
            GameObject autoDestroyOwner = null
        )
        {
            var timer = TimerManager.Instance.Get();
            timer.Reset();

            timer.UseId = AllocUseId;
            timer._duration = duration;
            timer._onComplete = onComplete;
            timer._onPerFrame = onPerFrame;
            timer._onPerSecond = onPerSecond;
            timer._onCountDown = onCountDown;
            timer._isLoop = isLoop;
            timer._usesRealTime = useRealTime;
            timer._hasAutoDestroyOwner = autoDestroyOwner != null;
            timer._autoDestroyOwner = autoDestroyOwner;
            timer._startTime = timer.GetWorldTime();
            timer._elapsedTimeTemp = 0f;
            timer._elapsedSecond = 0f;

            return timer.UseId;
        }


        private float GetWorldTime()
        {
            return _usesRealTime ? Time.realtimeSinceStartup : Time.time;
        }


        private float GetFireTime()
        {
            return _startTime + _duration;
        }

        /// Cancels a timer. The main benefit of this over the method on the instance is that you will not get
        /// a <see cref="NullReferenceException"/> if the timer is null.
        /// </summary>
        /// <param name="useId">the ID which timer you want to cancel.</param>
        public static void Cancel(long useId)
        {
            if (useId <= 0)
            {
                return;
            }

            Timer timer = null;
            if (TimerManager.Instance.TryGet(useId, ref timer))
            {
                timer.IsCancelled = true;
            }
        }


        public Timer()
        {
            Reset();
        }


        public void Reset()
        {
            UseId = 0;
            _duration = 0f;
            _onComplete = null;
            _onPerFrame = null;
            _onPerSecond = null;
            _onCountDown = null;
            _isLoop = false;
            _usesRealTime = false;
            _elapsedTimeTemp = 0f;
            _elapsedSecond = 0f;
            IsCancelled = false;
        }


        public void OnUpdate(float deltaTime)
        {
            if (IsCancelled) return;
            if (_hasAutoDestroyOwner && _autoDestroyOwner == null)
            {
                Cancel(UseId);
                return;
            }

            var worldTime = GetWorldTime();
            var fireTime = GetFireTime();

            if (_duration >= 0 && worldTime >= fireTime)
            {
                if (_onComplete != null) _onComplete();

                if (_isLoop)
                {
                    _startTime = worldTime;
                }
                else
                {
                    Cancel(UseId);
                    return;
                }
            }

            if (_onPerFrame != null) _onPerFrame(deltaTime);

            _elapsedTimeTemp += deltaTime;
            if (_elapsedTimeTemp >= 1f)
            {
                _elapsedTimeTemp -= 1;
                _elapsedSecond++;
                if (_onPerSecond != null) _onPerSecond(_elapsedSecond);

                if (_duration >= 0)
                {
                    var countDown = _duration - _elapsedSecond;
                    if (_onCountDown != null) _onCountDown(countDown >= 1f ? countDown : 0f);
                }
            }
        }
    }
}