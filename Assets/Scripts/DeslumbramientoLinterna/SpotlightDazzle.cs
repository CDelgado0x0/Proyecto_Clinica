using UnityEngine;

public class SpotlightDazzle : MonoBehaviour
{
    [SerializeField] private Light spotLight;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CanvasGroup dazzleOverlay;

    [Header("Ajustes")]
    [Range(0f, 1f)][SerializeField] private float alignmentThreshold = 0.97f;
    [SerializeField] private float maxDazzleAlpha = 0.9f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private LayerMask occlusionMask;

    private void Start()
    {
        dazzleOverlay.alpha = 0f;
    }
    private void Update()
    {
        float targetAlpha = 0f;

        Vector3 toCamera = playerCamera.transform.position - spotLight.transform.position;
        float distance = toCamera.magnitude;
        Vector3 toCameraDir = toCamera.normalized;

        // ¿La luz apunta hacia la cámara?
        float lightFacingCamera = Vector3.Dot(spotLight.transform.forward, toCameraDir);
        // ¿La cámara mira hacia la luz?
        float cameraFacingLight = Vector3.Dot(playerCamera.transform.forward, -toCameraDir);

        if (lightFacingCamera > alignmentThreshold && cameraFacingLight > alignmentThreshold)
        {
            bool blocked = Physics.Linecast(spotLight.transform.position, playerCamera.transform.position, occlusionMask);

            if (!blocked)
            {
                float alignmentFactor = Mathf.InverseLerp(alignmentThreshold, 1f, Mathf.Min(lightFacingCamera, cameraFacingLight));
                float distanceFactor = Mathf.Clamp01(spotLight.range / Mathf.Max(distance, 0.01f));
                targetAlpha = alignmentFactor * distanceFactor * maxDazzleAlpha;
            }
        }

        dazzleOverlay.alpha = Mathf.MoveTowards(dazzleOverlay.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }
}