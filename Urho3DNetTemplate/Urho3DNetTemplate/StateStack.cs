using System;
using System.Collections.Generic;
using Urho3DNet;

namespace $ext_safeprojectname$
{
    public class StateStack: IDisposable
    {
        private readonly StateManager _stateManager;
        private readonly Stack<SharedPtr<ApplicationState>> _stack = new Stack<SharedPtr<ApplicationState>>();

        public StateStack(StateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public ApplicationState State => (_stack.Count > 0) ? _stack.Peek() : null;

        public void Push(ApplicationState state)
        {
            if (state != null)
            {
                _stateManager.EnqueueState(state);
                _stack.Push(state);
            }
        }

        public void Pop()
        {
            if (_stack.Count > 0)
            {
                _stack.Peek().Dispose();
                _stack.Pop();
            }
            _stateManager.EnqueueState(State);
        }

        public void Switch(ApplicationState state)
        {
            if (_stack.Count > 0)
            {
                _stack.Peek().Dispose();
                _stack.Pop();
            }
            if (state != null)
            {
                _stack.Push(state);
                _stateManager.EnqueueState(State);
            }
        }


        public void Dispose()
        {
            while (_stack.Count > 0)
            {
                _stack.Peek().Dispose();
                _stack.Pop();
            }
        }
    }
}