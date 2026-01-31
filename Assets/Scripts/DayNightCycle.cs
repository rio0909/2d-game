using UnityEngine;

#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [Header("Time Settings")]
    [Tooltip("How long a full in-game day takes in real seconds.")]
    [SerializeField] private float realSecondsPerFullDay = 300f; // 5 minutes
    [Tooltip("Start time in hours (0-24). 8 = 8AM.")]
    [Range(0f, 24f)]
    [SerializeField] private float startHour = 8f;

    [Header("Lighting (URP 2D Global Light)")]
#if UNITY_URP
    [SerializeField] private Light2D globalLight;
#endif

    [Header("Light Intensity Over Day")]
    [Tooltip("X axis = time (0..1), Y axis = intensity (0..1).")]
    [SerializeField] private AnimationCurve intensityOverDay = DefaultIntensityCurve();

    [Header("Light Color Over Day")]
    [Tooltip("Gradient controls tint across the day (night -> morning -> noon -> evening -> night).")]
    [SerializeField] private Gradient colorOverDay = DefaultColorGradient();

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = false;

    // Internal normalized time: 0..1 (0 = midnight, 0.5 = noon)
    [SerializeField, Range(0f, 1f)] private float t;

    public float NormalizedTime => t;          // 0..1
    public float Hour => t * 24f;              // 0..24

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Convert startHour -> normalized time
        t = Mathf.Clamp01(startHour / 24f);

#if UNITY_URP
        if (globalLight == null)
            globalLight = FindAnyObjectByType<Light2D>();
#endif

        ApplyLighting();
    }

    private void Update()
    {
        if (realSecondsPerFullDay <= 0.01f) return;

        // Advance time
        float delta = Time.deltaTime / realSecondsPerFullDay;
        t += delta;

        if (t >= 1f)
        {
            t -= 1f;
            if (showDebugLog) Debug.Log("New day started!");
        }

        ApplyLighting();
    }

    public void SetTimeHour(float hour0to24)
    {
        t = Mathf.Repeat(hour0to24 / 24f, 1f);
        ApplyLighting();
    }

    private void ApplyLighting()
    {
        float intensity = Mathf.Clamp01(intensityOverDay.Evaluate(t));
        Color tint = colorOverDay.Evaluate(t);

#if UNITY_URP
        if (globalLight != null)
        {
            globalLight.intensity = intensity;
            globalLight.color = tint;
        }
#endif

        if (showDebugLog)
        {
            // Keep this light: it can spam, so only enable when debugging.
            Debug.Log($"Time: {Hour:0.00}h  t={t:0.000}  intensity={intensity:0.00}");
        }
    }

    // --------- Defaults (so it works instantly even if you don't edit curves) ---------

    private static AnimationCurve DefaultIntensityCurve()
    {
        // Night low, morning rising, noon bright, evening falling, night low
        return new AnimationCurve(
            new Keyframe(0.00f, 0.12f), // 00:00
            new Keyframe(0.20f, 0.20f), // ~04:48
            new Keyframe(0.30f, 0.55f), // ~07:12
            new Keyframe(0.50f, 1.00f), // 12:00
            new Keyframe(0.70f, 0.55f), // ~16:48
            new Keyframe(0.80f, 0.25f), // ~19:12
            new Keyframe(1.00f, 0.12f)  // 24:00
        );
    }

    private static Gradient DefaultColorGradient()
    {
        // Night (bluish) -> morning (warm) -> noon (neutral) -> evening (orange) -> night (bluish)
        Gradient g = new Gradient();

        var colors = new GradientColorKey[]
        {
            new GradientColorKey(new Color(0.55f, 0.65f, 1.00f), 0.00f), // night blue
            new GradientColorKey(new Color(1.00f, 0.80f, 0.60f), 0.28f), // sunrise warm
            new GradientColorKey(Color.white,                          0.50f), // noon
            new GradientColorKey(new Color(1.00f, 0.72f, 0.55f), 0.72f), // sunset warm
            new GradientColorKey(new Color(0.55f, 0.65f, 1.00f), 1.00f)  // night blue
        };

        var alphas = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        };

        g.SetKeys(colors, alphas);
        return g;
    }
}
