using System;
using Urho3DNet;

namespace Urho3DNetTemplate
{
    public class Character : MoveAndOrbitComponent
    {
        /// Character states (on ground, falling, etc).
        private readonly BaseState[] _states;

        /// Cached structure to capture raycast results.
        private readonly PhysicsRaycastResult _physicsRaycastResult = new PhysicsRaycastResult();

        /// Character state input.
        private Inputs _inputs;
        
        /// Kinematic character controller.
        private KinematicCharacterController _characterController;

        /// Current active character state.
        private BaseState _currentState;

        public Character(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate;
            _states = new BaseState[(int)CharacterState.NumStates]
            {
                new StartState(this),
                new OnGround(this),
                new JumpState(this),
                new Fall(this)
            };
            TransitionToState(CharacterState.Start);
        }

        /// <summary>
        /// Character states.
        /// </summary>
        private enum CharacterState
        {
            /// <summary>
            /// Start state. On first update in changes into actual state based on input data.
            /// </summary>
            Start,

            /// <summary>
            /// Character is standing or running on ground.
            /// </summary>
            OnGround,

            /// <summary>
            /// Character is jumping.
            /// </summary>
            Jump,

            /// <summary>
            /// Character is in free fall.
            /// </summary>
            Fall,

            /// <summary>
            /// Total number of valid states.
            /// This should be the last state!
            /// </summary>
            NumStates
        }

        public KinematicCharacterController CharacterController
        {
            get => _characterController;
            set
            {
                if (_characterController != value)
                {
                    _characterController = value;
                    UpdateHeight();
                }
            }
        }

        public AnimationController AnimationController { get; set; }
        public Node ModelPivot { get; set; }
        public Node CameraYaw { get; set; }
        public Node CameraPitch { get; set; }

        public Animation Idle { get; set; }

        public Animation Walk { get; set; }
        public Node CameraNode { get; set; }

        public float CameraDistance { get; set; } = 1.4f;

        public bool Crouch
        {
            get => _inputs.Crouch;
            set
            {
                if (_inputs.Crouch != value)
                {
                    _inputs.Crouch = value;
                    UpdateHeight();
                }
            }
        }

        public uint CameraCollisionMask { get; set; } = uint.MaxValue;
        public Animation Run { get; set; }
        public Animation Falling { get; set; }

        public bool Jump
        {
            get => _inputs.Jump;
            set => _inputs.Jump = value;
        }

        public override void Update(float timeStep)
        {
            ApplyRotation();

            _inputs.TimeStep = timeStep;
            _inputs.InputVelocity = Velocity;
            _inputs.InputSpeed = _inputs.InputVelocity.Length;
            if (_inputs.InputSpeed < 1e-3f)
            {
                _inputs.InputSpeed = 0;
                _inputs.InputDirection = Vector3.Zero;
            }
            else
            {
                _inputs.InputDirection = _inputs.InputVelocity / _inputs.InputSpeed;
            }

            _inputs.InputDirection = _inputs.InputVelocity;
            _currentState.Update(ref _inputs);

            CharacterController.SetWalkIncrement(_inputs.CurrentVelocity * timeStep);
            RotateModel(timeStep);

            base.Update(timeStep);
        }

        private void TransitionToState(CharacterState state, ref Inputs inputs)
        {
            var targetState = _states[(int)state];
            if (_currentState != targetState)
            {
                if (_currentState != null)
                    _currentState.Exit();
                _currentState = targetState;
                if (_currentState != null)
                {
                    _currentState.Enter();
                    _currentState.Update(ref inputs);
                }
            }
        }

        private void TransitionToState(CharacterState state)
        {
            var targetState = _states[(int)state];
            if (_currentState != targetState)
            {
                if (_currentState != null)
                    _currentState.Exit();
                _currentState = targetState;
                if (_currentState != null)
                    _currentState.Enter();
            }
        }

        private void UpdateHeight()
        {
            if (CharacterController != null)
            {
                if (Crouch)
                {
                    CharacterController.Height = 0.9f;
                    CharacterController.Offset = new Vector3(0.0f, 0.45f);
                }
                else
                {
                    CharacterController.Height = 1.8f;
                    CharacterController.Offset = new Vector3(0.0f, 0.9f);
                }
            }
        }


        private void RotateModel(float timeStep)
        {
            if (_inputs.CurrentVelocity.LengthSquared > 1e-2f)
            {
                //Limit rotation to avoid instant change of model orientation.
                var targetRot = new Quaternion(Vector3.Forward, _inputs.CurrentVelocity);
                var currentRot = ModelPivot.Rotation;
                var diff = Math.Abs(MathDefs.RadiansToDegrees((currentRot.Inversed * targetRot).Angle));
                diff = Math.Min(diff, 360 - diff);
                var maxAngle = 2.0f * 360.0f * timeStep;
                if (diff > maxAngle)
                {
                    var k = maxAngle / Math.Abs(diff);
                    targetRot = currentRot.Slerp(targetRot, k);
                }

                ModelPivot.Rotation = targetRot;
            }
        }

        private void ApplyRotation()
        {
            if (CameraPitch != null)
                CameraPitch.Rotation = new Quaternion(new Vector3(GetPitch()));
            if (CameraYaw != null)
                CameraYaw.Rotation = new Quaternion(new Vector3(0, GetYaw()));

            if (CameraNode != null)
            {
                var parent = CameraNode.Parent;
                var physicsWorld = Scene.GetComponent<PhysicsWorld>();
                if (physicsWorld != null)
                {
                    var ray = new Ray(parent.WorldPosition, -parent.WorldDirection);
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
        }

        /// <summary>
        ///     Input data for state.
        /// </summary>
        public struct Inputs
        {
            /// Current frame time step.
            public float TimeStep;

            /// Current frame input velocity defined by AI or controls.
            public Vector3 InputVelocity;

            /// InputVelocity vector length.
            public float InputSpeed;

            /// Normalized input direction.
            public Vector3 InputDirection;

            /// Is jump button pressed.
            public bool Jump;

            /// Is character crouching.
            public bool Crouch;

            /// Last known velocity.
            public Vector3 CurrentVelocity;
        }

        /// <summary>
        ///     Base class for character state.
        /// </summary>
        public class BaseState
        {
            /// <summary>
            ///     Construct BaseState.
            /// </summary>
            /// <param name="character">Character component.</param>
            protected BaseState(Character character)
            {
                Character = character;
            }

            /// <summary>
            ///     Character component.
            /// </summary>
            public Character Character { get; }

            /// <summary>
            ///     Executes when the state becomes active.
            /// </summary>
            public virtual void Enter()
            {
            }

            /// <summary>
            ///     Executes when the state becomes inactive.
            /// </summary>
            public virtual void Exit()
            {
            }

            /// <summary>
            ///     Update state.
            /// </summary>
            public virtual void Update(ref Inputs inputs)
            {
            }
        }

        public class OnGround : BaseState
        {
            /// How much time character could be in air before it considers to be falling down.
            private const float InairThresholdTime = 0.1f;
            /// In air timer. Due to possible physics inaccuracy, character can be off ground for max. 1/10 second and still be allowed to move.
            private float _inAirTimer;
            /// Movement speed from animation.
            private float _speed;
            /// Current animation.
            private Animation _currentAnimation;

            public OnGround(Character character) : base(character)
            {
            }

            /// <inheritdoc />
            public override void Enter()
            {
                _inAirTimer = 0.0f;
                _currentAnimation = null;
            }

            /// <inheritdoc />
            public override void Update(ref Inputs inputs)
            {
                if (!Character.CharacterController.OnGround())
                    _inAirTimer += inputs.TimeStep;
                else
                    _inAirTimer = 0;

                var softGrounded = _inAirTimer < InairThresholdTime;
                if (!softGrounded)
                {
                    Character.TransitionToState(CharacterState.Fall, ref inputs);
                    return;
                }

                Animation nextAnimation = null;
                if (inputs.InputSpeed < 0.3f)
                {
                    nextAnimation = Character.Idle;
                    _speed = 0.0f;
                }
                else if (inputs.InputSpeed > 0.7f)
                {
                    nextAnimation = Character.Run;
                }
                else
                {
                    nextAnimation = Character.Walk;
                }

                if (nextAnimation != _currentAnimation)
                {
                    var animationParameters = new AnimationParameters(nextAnimation).Looped();
                    Character.AnimationController.PlayNewExclusive(animationParameters, 0.2f);
                    _currentAnimation = nextAnimation;
                    _speed = _currentAnimation.GetMetadata("LinearVelocity").Vector3.Length;
                }

                inputs.CurrentVelocity = new Quaternion(0, Character.GetYaw(), 0) * (inputs.InputDirection * _speed);

                if (inputs.Jump)
                {
                    Character.TransitionToState(CharacterState.Jump);
                }
            }
        }

        public class Fall : BaseState
        {
            public Fall(Character character) : base(character)
            {
            }

            public override void Enter()
            {
                var animationParameters = new AnimationParameters(Character.Falling).Looped();
                Character.AnimationController.PlayNewExclusive(animationParameters, 0.2f);
            }

            public override void Update(ref Inputs inputs)
            {
                if (Character.CharacterController.OnGround())
                {
                    Character.TransitionToState(CharacterState.OnGround, ref inputs);
                    return;
                }

                var velocity = new Quaternion(0, Character.GetYaw(), 0) *
                               (inputs.InputDirection * inputs.CurrentVelocity.Length);
                inputs.CurrentVelocity = inputs.CurrentVelocity.Lerp(velocity, 2*inputs.TimeStep);
            }
        }

        public class JumpState : Fall
        {
            public JumpState(Character character) : base(character)
            {
            }

            public override void Enter()
            {
                base.Enter();
                Character.CharacterController.Jump(new Vector3(0,7.5f,0));
            }
        }

        public class StartState : BaseState
        {
            public StartState(Character character) : base(character)
            {
            }

            public override void Update(ref Inputs inputs)
            {
                if (Character.CharacterController.OnGround())
                    Character.TransitionToState(CharacterState.OnGround, ref inputs);
                else
                    Character.TransitionToState(CharacterState.Fall, ref inputs);
            }
        }
    }
}