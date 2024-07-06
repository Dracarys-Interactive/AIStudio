using System;
using System.Collections.Generic;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public class DialogueActionManager : Singleton<DialogueActionManager>
    {
        private float _currentTime;

        [SerializeReference] private DialogueActionBase _runningAction;

        // This is a List in lieu of a Queue so it can be observed in the Inspector
        [SerializeReference] private List<DialogueActionBase> _queue = new List<DialogueActionBase>();

        private bool _actionRunning = false;
        private object _actionRunningLock = new object();

        public float currentTime
        {
            get { return _currentTime; }
            private set { _currentTime = value; }
        }

        public bool ActionsPending
        {
            get { return _queue.Count > 0; }
        }

        private bool actionRunning
        {
            get
            {
                lock (_actionRunningLock)
                {
                    return _actionRunning;
                }
            }
            set
            {
                lock (_actionRunningLock)
                {
                    _actionRunning = value;
                }
            }
        }

        void FixedUpdate()
        {
            currentTime = Time.time;

            if (!actionRunning && ActionsPending)
            {
                actionRunning = true;
                DialogueActionBase action = _runningAction = Dequeue();
                action.onCompletion += ActionCompleted;
                action.startTime = currentTime;
                action.Invoke();
            }
        }

        private DialogueActionBase Dequeue()
        {
            DialogueActionBase front = _queue[0];
            _queue.RemoveAt(0);
            return front;
        }

        private void ActionCompleted()
        {
            _runningAction.onCompletion -= ActionCompleted;
            _runningAction.completionTime = currentTime;
            Log($"action: {_runningAction.GetType().Name} wait time: {_runningAction.startTime - _runningAction.creationTime} " +
                $"run time: {_runningAction.completionTime - _runningAction.startTime}");
            _runningAction = null;
            actionRunning = false;
        }

        public void EnqueueAction(DialogueActionBase action, bool force = false)
        {
            if (!DialogueManager.Instance.dialogueClosed || force)
                Enqueue(action);
        }

        private void Enqueue(DialogueActionBase action)
        {
            _queue.Add(action);
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}
