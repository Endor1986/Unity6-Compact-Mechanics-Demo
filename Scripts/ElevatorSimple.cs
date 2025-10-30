using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class ElevatorSimple : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;
    // DE: Unterer/oberer Referenzpunkt für die Fahrstrecke des Aufzugs.
    // EN: Bottom/top reference transforms defining the elevator travel path.

    [Header("Fahrt")]
    [SerializeField] private float speed = 2.5f;
    [Tooltip("Wartezeit oben und unten (auch vor der ersten Fahrt, wenn so konfiguriert).")]
    [SerializeField] private float waitAtEnds = 0.3f;
    // DE: Fahrgeschwindigkeit und Pausenzeit an den Enden.
    // EN: Travel speed and pause duration at each end.

    [Header("Start-Verhalten")]
    [Tooltip("Verwende waitAtEnds auch VOR der ersten Fahrt (empfohlen).")]
    [SerializeField] private bool initialWaitUsesWaitAtEnds = true;
    [Tooltip("Eigene Start-Wartezeit (nur aktiv, wenn oben false).")]
    [SerializeField] private float initialWaitOverride = 0f;
    // DE: Startverzögerung: entweder die übliche End-Pause oder ein eigener Wert.
    // EN: Startup delay: either reuse end wait or use a custom value.

    [Header("Modi")]
    [Tooltip("Wenn true: nach Start fährt der Aufzug endlos oben/unten.")]
    [SerializeField] private bool loopMode = true;
    [Tooltip("Bei Play sofort starten (inkl. Start-Wartezeit).")]
    [SerializeField] private bool loopOnStart = false;
    // DE: loopMode = Endlosschleife; loopOnStart = automatisch beim Spielstart losfahren.
    // EN: loopMode = continuous loop; loopOnStart = auto-start on play.

    public bool IsMoving { get; private set; }
    public bool IsLooping { get; private set; }
    // DE: Laufzeit-Flags: bewegt sich gerade? läuft die Endlosschleife?
    // EN: Runtime flags: currently moving? loop running?

    private Vector3 bottomPos, topPos;
    private Coroutine loopRoutine;
    // DE: Zwischengespeicherte Positionen & Handle für die Loop-Coroutine.
    // EN: Cached positions & handle for the loop coroutine.

    void Awake()
    {
        bottomPos = bottomPoint ? bottomPoint.position : transform.position;
        topPos = topPoint ? topPoint.position : transform.position + Vector3.up * 5f;

        if ((topPos - bottomPos).sqrMagnitude < 0.0001f)
            topPos = bottomPos + Vector3.up * 3f;
        // DE: Sicherheitsfallback: wenn beide Punkte zu nah sind, definiere eine kurze Strecke nach oben.
        // EN: Safety fallback: if points are too close, create a short upward travel.

        if (speed <= 0f) speed = 2f;
        // DE: Minimale sinnvolle Geschwindigkeit erzwingen.
        // EN: Enforce a minimal reasonable speed.

        // Zu Spielbeginn sauber unten positionieren
        transform.position = bottomPos;
        // EN: Ensure the platform starts at the bottom at runtime start.
    }

    void Start()
    {
        if (loopOnStart) StartLoop();
        // DE: Optional sofort starten.
        // EN: Optionally start immediately.
    }

    // -------- Public API --------
    public void StartLoop()
    {
        if (IsLooping) return;
        if (loopRoutine != null) StopCoroutine(loopRoutine);
        loopRoutine = StartCoroutine(CoLoop());
        // DE: Startet die Endlosschleife (ggf. nach Start-Wartezeit).
        // EN: Starts the loop (with optional initial wait).
    }

    public void StopLoop()
    {
        if (loopRoutine != null) StopCoroutine(loopRoutine);
        loopRoutine = null;
        IsLooping = false;
        IsMoving = false;
        // DE: Stoppt die Schleife und markiert den Aufzug als inaktiv.
        // EN: Stops the loop and marks the elevator as inactive.
    }

    // -------- Internes --------
    IEnumerator CoLoop()
    {
        IsLooping = true;

        // Sicher unten starten
        transform.position = bottomPos;
        // EN: Ensure consistent start position.

        // ✨ Start-Wartezeit anwenden
        float firstWait = initialWaitUsesWaitAtEnds ? waitAtEnds : Mathf.Max(0f, initialWaitOverride);
        if (firstWait > 0f) yield return new WaitForSeconds(firstWait);
        // DE: Optionales Warten vor der ersten Fahrt.
        // EN: Optional pre-move wait before the first travel.

        // 1) Initial hoch
        yield return MoveTo(topPos);

        if (!loopMode)
        {
            IsLooping = false;
            yield break;
            // DE: Einmalige Fahrt beendet (kein Loop).
            // EN: Single run finished (no loop).
        }

        // 2) Endlos oben <-> unten
        while (true)
        {
            if (waitAtEnds > 0f) yield return new WaitForSeconds(waitAtEnds);
            yield return MoveTo(bottomPos);

            if (waitAtEnds > 0f) yield return new WaitForSeconds(waitAtEnds);
            yield return MoveTo(topPos);
            // DE: Pendelbewegung mit Pausen an den Enden.
            // EN: Ping-pong movement with pauses at endpoints.
        }
    }

    IEnumerator MoveTo(Vector3 target)
    {
        IsMoving = true;
        while ((transform.position - target).sqrMagnitude > 0.0004f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
            // DE: Frame-weise lineares Bewegen Richtung Zielpunkt.
            // EN: Frame-wise linear movement towards target.
        }
        transform.position = target;
        IsMoving = false;
        // DE: Ziel erreicht, Bewegung beendet.
        // EN: Target reached, movement finished.
    }
}
