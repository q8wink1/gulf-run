using System;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.4 — swipe / keyboard input for the 3-lane runner.
    /// Mobile: touch delta threshold. Desktop: A/D/W/S and arrow keys.
    /// Uses legacy Input API (project Active Input Handling = Both).
    /// Zero GC in the hot path beyond optional event subscribers.
    /// </summary>
    public sealed class RunnerSwipeInput : MonoBehaviour
    {
        [SerializeField] private float swipeThresholdPixels = 48f;

        public event Action LaneLeft;
        public event Action LaneRight;
        public event Action Jump;
        public event Action Slide;

        private Vector2 _touchStart;
        private bool _trackingTouch;
        private bool _gestureConsumed;

        public void SetSwipeThreshold(float pixels)
        {
            swipeThresholdPixels = Mathf.Max(8f, pixels);
        }

        private void Update()
        {
            PollKeyboard();
            PollTouch();
        }

        private void PollKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LaneLeft?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                LaneRight?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                Jump?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Slide?.Invoke();
            }
        }

        private void PollTouch()
        {
            if (Input.touchCount <= 0)
            {
                _trackingTouch = false;
                _gestureConsumed = false;
                return;
            }

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _touchStart = touch.position;
                _trackingTouch = true;
                _gestureConsumed = false;
                return;
            }

            if (!_trackingTouch || _gestureConsumed)
            {
                return;
            }

            if (touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Ended)
            {
                return;
            }

            Vector2 delta = touch.position - _touchStart;
            float absX = Mathf.Abs(delta.x);
            float absY = Mathf.Abs(delta.y);
            float threshold = swipeThresholdPixels;

            if (absX < threshold && absY < threshold)
            {
                return;
            }

            _gestureConsumed = true;
            if (absX > absY)
            {
                if (delta.x < 0f)
                {
                    LaneLeft?.Invoke();
                }
                else
                {
                    LaneRight?.Invoke();
                }
            }
            else if (delta.y > 0f)
            {
                Jump?.Invoke();
            }
            else
            {
                Slide?.Invoke();
            }
        }
    }
}
