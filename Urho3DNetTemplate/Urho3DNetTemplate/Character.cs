using System;
using Urho3DNet;

namespace $safeprojectname$
{
    public class Character : MoveAndOrbitComponent
    {
        public Character(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate;
        }

        public KinematicCharacterController CharacterController { get; set; }
        public AnimationController AnimationController { get; set; }
        public Node ModelPivot { get; set; }
        public Node CameraYaw { get; set; }
        public Node CameraPitch { get; set; }

        public Animation Idle { get; set; }

        public Animation Walk { get; set; }
        public Node CameraNode { get; set; }

        public float CameraDistance { get; set; } = 1.4f;

        public uint CameraCollisionMask { get; set; } = UInt32.MaxValue;

        private Animation _currentAnimation;
        private float _speed;
        private PhysicsRaycastResult _physicsRaycastResult = new PhysicsRaycastResult();

        public override void Update(float timeStep)
        {
            if (CameraPitch != null)
                CameraPitch.Rotation = new Quaternion(new Vector3(GetPitch(), 0, 0));
            if (CameraYaw != null)
                CameraYaw.Rotation = new Quaternion(new Vector3(0, GetYaw(), 0));

            if (CameraNode != null)
            {
                var parent = CameraNode.Parent;
                var physicsWorld = Scene.GetComponent<PhysicsWorld>();
                if (physicsWorld != null)
                {
                    var ray = new Ray(parent.WorldPosition,-parent.WorldDirection);
                    var target = ray.Origin + ray.Direction * CameraDistance;
                    physicsWorld.SphereCast(_physicsRaycastResult, ray, 0.1f, CameraDistance, CameraCollisionMask);
                    var distance = Math.Min(CameraDistance, _physicsRaycastResult.Distance);
                    if (CameraYaw != null && distance < CameraDistance - 1e-6f)
                    {
                        var from = CameraYaw.WorldPosition;
                        var diff = target - from;
                        var diffLength = diff.Length;
                        diff = diff / diffLength;
                        var alternativeRay = new Ray(from, diff);

                    }
                    CameraNode.WorldPosition = ray.Origin + ray.Direction * distance;
                }
            }

            var velocity = this.Velocity;
            var l = velocity.Length;

            Animation nextAnimation = null;
            if (l < 0.3f)
            {
                nextAnimation = Idle;
            }
            else
            {
                nextAnimation = Walk;
            }

            if (nextAnimation != _currentAnimation)
            {
                var animationParameters = new AnimationParameters(nextAnimation).Looped();
                AnimationController.PlayNewExclusive(animationParameters, 0.2f);
                _currentAnimation = nextAnimation;
                _speed = _currentAnimation.GetMetadata("LinearVelocity").Vector3.Length;
            }

            if (_speed > 1e-2f)
            {
                var normalizedVelocity = new Quaternion(0, GetYaw(), 0) * (velocity * (1.0f / l));
                var v = normalizedVelocity * timeStep * _speed;

                //Limit rotation to avoid instant change of model orientation.
                var targetRot = new Quaternion(Vector3.Forward, normalizedVelocity);
                var currentRot = ModelPivot.Rotation;
                var diff = Math.Abs(MathDefs.RadiansToDegrees((currentRot.Inversed * targetRot).Angle));
                diff = Math.Min(diff, 360 - diff);
                float maxAngle = 2.0f * 360.0f * timeStep;
                if (diff > maxAngle)
                {
                    float k = maxAngle / Math.Abs(diff);
                    targetRot = currentRot.Slerp(targetRot, k);
                }
                ModelPivot.Rotation = targetRot;
                CharacterController.SetLinearVelocity(v);
            }
            else
            {
                CharacterController.SetLinearVelocity(Vector3.Zero);
            }
            base.Update(timeStep);
        }

    }
}