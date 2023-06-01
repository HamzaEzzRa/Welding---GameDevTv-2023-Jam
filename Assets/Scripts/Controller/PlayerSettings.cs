using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public enum EAutoParentMode
    {
        NONE,
        ANY,
        TARGET_PARENT,
    }

    [Header("General")]
    public float PlayerHeight = 1.8f;
    public float PlayerRadius = 0.5f;

    [Header("First Person Camera")]
    public bool InvertedControls;
    public Vector2 Sensitivity = new Vector2(75f, 50f);
    [FloatRangeSlider(-180f, 180f)] public FloatRange PitchRange = new FloatRange(-75f, 75f);
    public float WalkingFOV = 60f;
    public float RunningFOV = 70f;
    public float FOVSlewRate = 15f;

    [Header("Head Bobbing")]
    public bool HeadBobEnabled = true;
    public float HeadBobMinVelocity = 0.1f;
    public float HeadBobBlendSpeed = 1f;
    public AnimationCurve HeadBobVerticalCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0.2f);
    public AnimationCurve HeadBobHorizontalCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0.05f);
    public AnimationCurve HeadBobPeriodCurve = AnimationCurve.EaseInOut(0f, 0.5f, 1f, 0.25f);

    [Header("Movement")]
    public float WalkSpeed = 5.5f;
    public float RunSpeed = 12f;
    public float SpeedBlend = 1f;

    public float SlopeLimit = 50f;
    public float GroundOffset = 0.05f;
    public LayerMask GroundMask = ~0;

    public EAutoParentMode AutoParentMode = EAutoParentMode.NONE;

    [Header("Jumping")]
    public float JumpHeight = 2.25f;
    public bool AllowAirMovement = false;
    [Range(0f, 1f)] public float AirSpeedFactor = 0.65f;

    [Header("Falling")]
    public Vector3 PlayerGravity = new Vector3(0f, -9.8f, 0f);
    public float CoyoteTime = 0.1f;

    [Header("Crouching")]
    [Range(0.1f, 1f)] public float CrouchHeightFactor = 0.7f;
    [Range(0f, 1f)] public float CrouchSpeedFactor = 0.6f;
    public float CrouchTime = 0.5f;

    [Header("Stairs")]
    public float StepLookAhead = 0.1f;
    [FloatRangeSlider(0f, 1f)] public FloatRange StepHeightRange = new FloatRange(0.1f, 0.45f);
    public float StepInstant = 0.05f;

    [Header("Interactions")]
    public float MaxInteractionDistance = 2f;
    public LayerMask InteractionMask = ~0;

    [Header("Physics Material")]
    public PhysicMaterial PlayerDefaultMaterial;
    public PhysicMaterial PlayerSlopeMaterial;

    [Header("Audio")]
    public float UnscaledWalkSoundInterval = 4.5f;
    public float UnscaledRunSoundInterval = 7.5f;
    public float GroundHitMinAirTime = 0.2f;
}
