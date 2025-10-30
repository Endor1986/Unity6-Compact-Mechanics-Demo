using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class PressurePlateSimpleOnce : MonoBehaviour
{
    [Header("Wer darf ausl�sen? (leer = alle)")]
    [SerializeField] private string activatorTag = "Player";
    // DE: Nur Objekte mit diesem Tag d�rfen die Platte aktivieren (leer = jeder).
    // EN: Only objects with this tag can activate the plate (empty = anyone).

    [Header("Ziel-Aktionen")]
    [SerializeField] private ElevatorSimple elevator;      // optional
    [SerializeField] private DoorMoverOnce doorToOpen;    // optional
    // DE: Optionale Ziele: Aufzug starten und/oder T�r einmalig �ffnen.
    // EN: Optional targets: start elevator and/or open door once.

    [Header("Feedback (optional)")]
    [SerializeField] private Transform plateVisual;
    [SerializeField] private float pressDepth = 0.03f;
    [SerializeField] private float pressSpeed = 12f;
    // DE: Visuelles Feedback der Druckplatte (Tiefe & Geschwindigkeit des Eindr�ckens).
    // EN: Visual feedback for the pressure plate (press depth & speed).

    private bool triggered;
    // DE: Stellt sicher, dass die Platte nur einmal ausl�st.
    // EN: Ensures the plate triggers only once.

    private Vector3 plateRestPos;
    // DE: Ausgangsposition der visuellen Platte.
    // EN: Rest/local start position of the plate visual.

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        // DE: Platte als Trigger, damit OnTriggerEnter ausl�st.
        // EN: Make the plate a trigger so OnTriggerEnter fires.

        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        // DE: Kinematischer Rigidbody ohne Schwerkraft (stabil f�r Trigger).
        // EN: Kinematic rigidbody without gravity (stable for trigger use).
    }

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        // DE: Sicherheits-Set: Collider bleibt Trigger.
        // EN: Safety: ensure collider stays a trigger.

        var rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        // DE: Stellt sicher, dass ein kinematischer RB existiert.
        // EN: Ensure a kinematic RB exists.

        if (plateVisual) plateRestPos = plateVisual.localPosition;
        // DE: Startposition f�r die R�ckstellung/Animation merken.
        // EN: Cache start local position for animation/reset.
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        // DE: Nur einmal reagieren.
        // EN: Trigger only once.

        if (!string.IsNullOrEmpty(activatorTag) && !other.CompareTag(activatorTag))
            return;
        // DE: Optionaler Tag-Filter f�r ausl�sende Objekte.
        // EN: Optional tag filter for triggering objects.

        // EINMALIG ausl�sen
        triggered = true;
        StartCoroutine(CoPressAndFire());
        // DE: Startet die Press-Animation und f�hrt Aktionen aus.
        // EN: Starts press animation and executes actions.
    }

    IEnumerator CoPressAndFire()
    {
        // 1) Optik: Platte eindr�cken / Visual press
        if (plateVisual)
        {
            Vector3 target = plateRestPos + Vector3.down * pressDepth;
            while ((plateVisual.localPosition - target).sqrMagnitude > 0.000004f)
            {
                plateVisual.localPosition = Vector3.Lerp(plateVisual.localPosition, target, Time.deltaTime * pressSpeed);
                yield return null;
            }
            plateVisual.localPosition = target;
        }

        // 2) Aktionen: Aufzug starten + T�r �ffnen (je nach Zuweisung)
        //    Actions: start elevator + open door (if assigned)
        if (elevator) elevator.StartLoop();
        if (doorToOpen) doorToOpen.Open();
    }
}
