using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorMoverOnce : MonoBehaviour
{
    [Header("Bewegung / Movement")]
    [Tooltip("Bewegungsrichtung (z.B. nach oben). / Movement direction (e.g., upwards).")]
    [SerializeField] private Vector3 moveDirection = Vector3.up;

    [Tooltip("Distanz in Einheiten (Meter). / Travel distance in units (meters).")]
    [SerializeField] private float moveDistance = 2f;

    [Tooltip("Geschwindigkeit in Einheiten/Sekunde. / Speed in units per second.")]
    [SerializeField] private float speed = 2f;

    [Header("Optionen / Options")]
    [Tooltip("Sanftes Auslaufen am Ende (Lerp). / Smooth ease-out near the end (Lerp).")]
    [SerializeField] private bool easeOut = true;

    [Tooltip("Kollisionskomponenten nach Öffnen deaktivieren. / Disable colliders after opening.")]
    [SerializeField] private bool disableCollidersAfterOpen = true;

    [Tooltip("Objekt in lokalen Koordinaten bewegen (statt Welt). / Move in local space instead of world space.")]
    [SerializeField] private bool useLocalSpace = false;

    // DE: Öffnungsstatus (nur einmal öffnen).
    // EN: Open state (open only once).
    private bool opened;

    // DE: Start- und Zielposition für die Bewegung.
    // EN: Start and target positions for the motion.
    private Vector3 startPos;
    private Vector3 targetPos;

    void Awake()
    {
        // DE: Startposition basierend auf lokalem oder Welt-Raum festlegen.
        // EN: Determine start position in local or world space.
        startPos = useLocalSpace ? transform.localPosition : transform.position;

        // DE: Sichere, normalisierte Richtung (Fallback: Up), Zielposition berechnen.
        // EN: Safe, normalized direction (fallback: Up), compute target position.
        Vector3 dir = moveDirection.sqrMagnitude < 0.000001f ? Vector3.up : moveDirection.normalized;
        targetPos = startPos + dir * Mathf.Max(0f, moveDistance);
    }

    /// <summary>
    /// DE: Einmalig öffnen (fährt auf targetPos). 
    /// EN: Open once (move towards targetPos).
    /// </summary>
    public void Open()
    {
        if (opened) return;     // DE: Bereits geöffnet? Dann nichts tun. / EN: Already opened? Do nothing.
        opened = true;
        StopAllCoroutines();    // DE: Sicherheit: konkurrierende Bewegungen stoppen. / EN: Safety: stop competing motions.
        StartCoroutine(CoMoveToTarget());
    }

    IEnumerator CoMoveToTarget()
    {
        // DE: Kleine Abschluss-Toleranz für die Zielprüfung.
        // EN: Small epsilon tolerance when checking arrival at target.
        const float eps = 0.0004f;

        while (true)
        {
            // DE: Aktuelle Position lesen (lokal oder welt).
            // EN: Read current position (local or world).
            Vector3 current = useLocalSpace ? transform.localPosition : transform.position;

            Vector3 next;
            if (easeOut)
            {
                // DE: Sanftes Auslaufen gegen Ende mittels Lerp (faktisch zeitbasierte Dämpfung).
                // EN: Smooth ease-out towards the end via Lerp (time-based damping).
                next = Vector3.Lerp(current, targetPos, Time.deltaTime * Mathf.Max(6f, speed * 2f));
            }
            else
            {
                // DE: Konstante Schrittweite Richtung Ziel.
                // EN: Constant step towards target.
                next = Vector3.MoveTowards(current, targetPos, speed * Time.deltaTime);
            }

            // DE: Neue Position zurückschreiben (lokal oder welt).
            // EN: Write back new position (local or world).
            if (useLocalSpace) transform.localPosition = next;
            else transform.position = next;

            // DE: Ziel erreicht? Schleife beenden und exakt snappen.
            // EN: Target reached? Snap exactly and exit loop.
            if ((next - targetPos).sqrMagnitude <= eps)
            {
                if (useLocalSpace) transform.localPosition = targetPos;
                else transform.position = targetPos;
                break;
            }

            yield return null; // DE: Nächsten Frame abwarten. / EN: Wait until next frame.
        }

        // DE: Optional alle Collider deaktivieren, damit die Tür offen „nicht mehr kollidiert“.
        // EN: Optionally disable all colliders so the door no longer collides when open.
        if (disableCollidersAfterOpen)
        {
            foreach (var col in GetComponentsInChildren<Collider>())
                col.enabled = false;
        }
    }
}
