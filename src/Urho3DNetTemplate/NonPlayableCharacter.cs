using System;
using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class NonPlayableCharacter: LogicComponent
    {
        private Vector3 _targret;
        private float _distance = 0.0f;
        float _restTime = 0.0f;
        public NonPlayableCharacter(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UsePostupdate;
        }

        private static Random _rnd = new Random();

        public override void PostUpdate(float timeStep)
        {
            base.Update(timeStep);
            var currentPosition = Node.WorldPosition;
            if (_restTime > 0)
            {
                _restTime -= timeStep;
                if (_restTime > 0.0f)
                {
                    return;
                }

                var dir = new Vector3(_rnd.Next(3) - 1, 0, _rnd.Next(3) - 1) * 5.0f;
                if (dir.ApproximatelyEquivalent(Vector3.Zero))
                {
                    _restTime += 1.0f;
                    return;
                }

                _targret = currentPosition + dir;
                _distance = float.MaxValue;
                var moveAndOrbitComponent = (MoveAndOrbitComponent)Node.GetComponentImplementation(nameof(MoveAndOrbitComponent));
                moveAndOrbitComponent.SetVelocity(dir.Normalized);
            }

            var diff = _targret - currentPosition;
            diff.Y = 0;
            var distance = diff.Length;
            if (distance >= _distance)
            {
                _restTime = 2.0f;
                var moveAndOrbitComponent = (MoveAndOrbitComponent)Node.GetComponentImplementation(nameof(MoveAndOrbitComponent));
                moveAndOrbitComponent.SetVelocity(Vector3.Zero);
                return;
            }

            _distance = distance;
        }
    }
}