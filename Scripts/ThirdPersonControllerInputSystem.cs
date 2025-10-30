using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class ThirdPersonControllerInputSystem : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform cameraTransform;
    // DE: Referenz auf die Spielerkamera (für kamera-relative Bewegung). Fällt auf Camera.main zurück, wenn leer.
    // EN: Reference to the player camera (for camera-relative movement). Falls back to Camera.main when empty.

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    // DE: Grundlaufgeschwindigkeit.
    // EN: Base walking speed.

    [SerializeField] private float sprintSpeed = 7.5f;
    // DE: Geschwindigkeit beim Sprinten.
    // EN: Speed while sprinting.

    [SerializeField] private float acceleration = 20f;
    // DE: Beschleunigungsrate, mit der die Zielgeschwindigkeit erreicht wird.
    // EN: Acceleration rate towards target speed.

    [SerializeField, Range(0f, 1f)] private float airControl = 0.35f;
    // DE: Anteil der Beschleunigung in der Luft (0 = keine Kontrolle, 1 = volle Kontrolle).
    // EN: Portion of acceleration applied while airborne (0 = none, 1 = full).

    [SerializeField] private float groundDrag = 4f;
    // DE: Dämpfung/Drag am Boden (hier im Code später als linearDamping gesetzt).
    // EN: Ground drag (later assigned as linearDamping in code).

    [SerializeField] private float airDrag = 0.5f;
    // DE: Dämpfung/Drag in der Luft.
    // EN: Air drag.

    [Header("Sprint / Schnellrennen")]
    [Tooltip("Shift gedrückt halten = Sprint. Bei false: Antippen toggelt Sprint.")]
    [SerializeField] private bool holdToSprint = true;
    // DE: Sprint-Halten oder Sprint-Umschalten per Tastendruck.
    // EN: Hold-to-sprint or toggle sprint on key press.

    [Tooltip("Zusätzliche Sprint-Keys (frei belegbar).")]
    [SerializeField] private KeyCode[] sprintKeys = new KeyCode[] { KeyCode.LeftShift, KeyCode.RightShift };
    // DE: Liste an Tasten, die Sprint auslösen können.
    // EN: List of keys that can trigger sprint.

    [Tooltip("Optional: Button/Axis-Name (z. B. 'Fire3'). Leer lassen, wenn ungenutzt.")]
    [SerializeField] private string sprintAxisOrButton = "";
    // DE: Unterstützt altes Input-System für Buttons/Achsen.
    // EN: Supports old Input Manager button/axis.

    [Tooltip("Kurzer Turbo zusätzlich zum Sprint (1.0 = aus).")]
    [SerializeField] private float sprintBoostMultiplier = 1.35f;
    // DE: Multiplikator für kurzzeitigen Sprint-Boost.
    // EN: Multiplier for short sprint boost.

    [SerializeField] private float sprintBoostDuration = 0.35f;
    // DE: Dauer des Boosts.
    // EN: Duration of the boost.

    [SerializeField] private float sprintBoostCooldown = 1.25f;
    // DE: Abklingzeit bis zum nächsten Boost.
    // EN: Cooldown until next boost.

    [SerializeField] private KeyCode boostKey = KeyCode.LeftAlt;
    // DE: Taste für den Boost.
    // EN: Key to trigger the boost.

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5.8f;
    // DE: Stärke des Sprungimpulses.
    // EN: Strength of the jump impulse.

    [SerializeField] private float gravityMultiplier = 2.0f;
    // DE: Zusätzliche Gravitation (für schnellere Fallspeed).
    // EN: Additional gravity (for snappier fall speed).

    [SerializeField] private float coyoteTime = 0.08f;
    // DE: Toleranzzeit kurz nach Verlassen des Bodens, in der Sprünge noch zählen.
    // EN: Grace period after leaving ground where jump still registers.

    [SerializeField] private float jumpBuffer = 0.08f;
    // DE: Zeitfenster, in dem vor der Landung gedrückte Sprünge gepuffert werden.
    // EN: Window buffering jump input pressed slightly before landing.

    [Tooltip("Zu Beginn Sprünge für X Sekunden ignorieren (gegen Start-Hüpfer).")]
    [SerializeField] private float startJumpLock = 0.2f;
    // DE: Verhindert ungewollte Sprünge direkt beim Szenenstart.
    // EN: Prevents unintended jumps right at scene start.

    [Header("Grounding")]
    [SerializeField] private float groundCheckOffset = 0.05f;
    // DE: Zusätzliche Prüftiefe unter dem Collider für die Bodenabfrage.
    // EN: Extra check depth below the collider for ground detection.

    [SerializeField] private LayerMask groundMask = ~0;
    // DE: Layer für begehbaren Boden.
    // EN: Layers considered as ground.

    [SerializeField, Range(0f, 80f)] private float maxSlopeAngle = 50f;
    // DE: Maximale Steigung, die noch als Boden gilt.
    // EN: Maximum slope still considered ground.

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private bool isGrounded;
    // DE: Aktuell auf dem Boden?
    // EN: Currently grounded?

    private bool hasTouchedGround;
    // DE: Hat den Boden mindestens einmal berührt (für Coyote/Start-Logik)?
    // EN: Has touched ground at least once (for coyote/start logic)?

    private float lastGroundedTime;
    // DE: Zeitpunkt der letzten Bodenberührung (für Coyote Time).
    // EN: Time of last grounding (for coyote time).

    private float lastJumpPressedTime;
    // DE: Zeitpunkt der letzten Sprung-Eingabe (für Jump Buffer).
    // EN: Time when jump was last pressed (for jump buffer).

    private float ignoreJumpUntil;
    // DE: Sperrzeitpunkt bis zu dem Sprünge ignoriert werden (Start-Hüpfer-Schutz).
    // EN: Time until jump input is ignored (start-hop protection).

    private Vector3 groundNormal = Vector3.up;
    // DE: Normale des Bodens (für Projektion/Bewegung auf der Ebene).
    // EN: Ground normal (for projecting movement onto the plane).

    // Sprint state
    private bool sprintActive;
    private float boostEndTime = -999f;
    private float nextBoostReadyTime = 0f;
    // DE: Sprint-/Boost-Zustände und -Zeiten.
    // EN: Sprint/boost state and timing.

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        // PhysX stabilisieren
        rb.useGravity = true;                         // DE: Gravitation aktivieren. | EN: Enable gravity.
        rb.constraints = RigidbodyConstraints.FreezeRotation; // DE: Rotation sperren (kein Umkippen). | EN: Freeze rotation (prevent tipping).
        rb.interpolation = RigidbodyInterpolation.Interpolate; // DE: Glättung. | EN: Smoothing.
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // DE: Stabile Kollisionen bei schneller Bewegung. | EN: Stable collisions at higher speeds.

        // Start-Zustände (verhindert Start-Hüpfer)
        lastJumpPressedTime = -999f; // DE: „Kein Sprung“ initial. | EN: Initialize as "no jump".
        lastGroundedTime = -999f;    // DE: Noch nicht geerdet. | EN: Not grounded yet.
        hasTouchedGround = false;    // DE: Bis erste Bodenberührung. | EN: Until first ground touch.
        ignoreJumpUntil = Time.time + startJumpLock; // DE: Anfangssperre. | EN: Startup lock.

        // ggf. Kamera fallback
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;

        // evtl. Restimpulse aus der Szene neutralisieren
        rb.linearVelocity = Vector3.zero;
        // DE: HINWEIS: Im klassischen Rigidbody heißt das Property 'velocity'.
        // EN: NOTE: In classic Rigidbody the property is 'velocity'.
    }

    void Update()
    {
        // Jump-Puffer erfassen
        if (Input.GetButtonDown("Jump"))
            lastJumpPressedTime = Time.time;
        // DE: Speichert den Zeitpunkt des Sprunginputs für Jump-Buffer.
        // EN: Stores jump input time for jump buffer.

        // Ground prüfen
        GroundCheck();
        // DE: Aktualisiert Bodenstatus, Normale und Zeitstempel.
        // EN: Updates grounded state, normal and timestamps.

        // --- Sprint lesen (Keys oder optional Button/Axis) ---
        bool sprintHeld = AnySprintKeyHeld() || AxisOrButtonHeld();
        bool sprintPressed = AnySprintKeyDown() || AxisOrButtonDown();
        // DE: Unterstützt Halten oder Toggle je nach Einstellung.
        // EN: Supports hold or toggle based on setting.

        if (holdToSprint)
        {
            sprintActive = sprintHeld; // DE: Halten = aktiv. | EN: Hold = active.
        }
        else
        {
            if (sprintPressed) sprintActive = !sprintActive; // DE: Toggle. | EN: Toggle.
        }

        // --- Boost (nur wenn bewegt + (am Boden oder kurz nach Boden)) ---
        if (Input.GetKeyDown(boostKey) && sprintActive && Time.time >= nextBoostReadyTime)
        {
            Vector2 mv = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (mv.sqrMagnitude > 0.01f && (isGrounded || (Time.time - lastGroundedTime) <= 0.1f))
            {
                boostEndTime = Time.time + sprintBoostDuration;
                nextBoostReadyTime = Time.time + sprintBoostCooldown;
                // DE: Startet einen kurzzeitigen Speed-Boost.
                // EN: Starts a short speed boost.
            }
        }
    }

    void FixedUpdate()
    {
        // Eingaben (altes Input System)
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = Vector2.ClampMagnitude(input, 1f);
        // DE: Normiert Eingaben auf Länge ≤ 1 (diagonal nicht schneller).
        // EN: Clamp inputs so diagonal speed isn’t higher.

        // kamera-gerichtete Achsen
        Vector3 fwd = Vector3.forward, right = Vector3.right;
        if (cameraTransform)
        {
            Vector3 camFwd = cameraTransform.forward; camFwd.y = 0f; camFwd.Normalize();
            Vector3 camRight = cameraTransform.right; camRight.y = 0f; camRight.Normalize();
            fwd = camFwd; right = camRight;
            // DE: Bewegen relativ zur Blickrichtung.
            // EN: Move relative to camera heading.
        }

        // Wunschrichtung -> auf Bewegungs-Ebene projizieren
        Vector3 wishDir = (fwd * input.y + right * input.x);
        Vector3 movePlaneNormal = isGrounded ? groundNormal : Vector3.up;
        wishDir = Vector3.ProjectOnPlane(wishDir, movePlaneNormal).normalized;
        // DE: Verhindert „Klettern“ an schrägen Flächen, hält Bewegung auf der Ebene.
        // EN: Prevents climbing/sliding issues by projecting onto movement plane.

        // Zielgeschwindigkeit (inkl. Sprint/Boost)
        float baseSpeed = sprintActive ? sprintSpeed : walkSpeed;
        if (Time.time < boostEndTime && sprintBoostMultiplier > 1f) baseSpeed *= sprintBoostMultiplier;
        Vector3 targetVelHorizontal = wishDir * baseSpeed;
        // DE: Horizontale Zielgeschwindigkeit (ohne y).
        // EN: Horizontal target velocity (no y).

        // aktuelle Geschwindigkeiten
        Vector3 vel = rb.linearVelocity;
        Vector3 velHorizontal = Vector3.ProjectOnPlane(vel, Vector3.up);
        // DE: Trenne horizontale von vertikaler Geschwindigkeit.
        // EN: Separate horizontal from vertical velocity.

        // Beschleunigung (am Boden direkter, in Luft reduziert)
        float usedAccel = isGrounded ? acceleration : acceleration * Mathf.Max(0f, airControl);
        velHorizontal = Vector3.MoveTowards(velHorizontal, targetVelHorizontal, usedAccel * Time.fixedDeltaTime);
        // DE: Weiches Angleichen an Zielgeschwindigkeit.
        // EN: Smoothly approach target velocity.

        // „Nicht an die Wand kleben“: am Boden kleine Restgeschwindigkeit dämpfen
        if (isGrounded && input.sqrMagnitude < 0.0001f && velHorizontal.sqrMagnitude < 0.04f)
            velHorizontal = Vector3.zero;
        // DE: Stoppt Kriechen bei sehr kleinen Restwerten.
        // EN: Stops crawling at tiny residual speeds.

        // Velocity zurückschreiben
        rb.linearVelocity = velHorizontal + Vector3.up * vel.y;
        // DE: HINWEIS: Klassisch wäre hier 'rb.velocity'.
        // EN: NOTE: With classic Rigidbody this would be 'rb.velocity'.

        // Drag passend setzen
        rb.linearDamping = isGrounded ? groundDrag : airDrag;
        // DE: HINWEIS: Klassisch wäre hier 'rb.drag'.
        // EN: NOTE: With classic Rigidbody this would be 'rb.drag'.

        // Extra-Gravity (macht Fallspeed knackiger)
        if (gravityMultiplier > 1f)
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        // DE: Zusätzliche Gravitation additiv zur Engine-Gravitation.
        // EN: Extra gravity additive to engine gravity.

        // Jump (Coyote + Buffer) — Start-Sperre beachten
        bool canCoyote = hasTouchedGround && (Time.time - lastGroundedTime) <= coyoteTime;
        bool wantsJump = (Time.time >= ignoreJumpUntil) && ((Time.time - lastJumpPressedTime) <= jumpBuffer);

        if (wantsJump && (isGrounded || canCoyote))
        {
            lastJumpPressedTime = -999f;

            // Fallgeschwindigkeit neutralisieren, damit Sprung knackig ist
            Vector3 v = rb.linearVelocity;
            if (v.y < 0f) v.y = 0f;
            rb.linearVelocity = v;
            // DE: HINWEIS: Klassisch wäre hier 'rb.velocity'.
            // EN: NOTE: Classic Rigidbody uses 'rb.velocity'.

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            // DE: Direkt nach Impuls als „in der Luft“ markieren.
            // EN: Mark as airborne right after the impulse.
        }
    }

    private void GroundCheck()
    {
        // einfache Bodenprüfung via SphereCast unterhalb des Spielers
        float radius = Mathf.Max(0.05f, (capsule ? capsule.radius : 0.3f) - 0.01f);
        Vector3 origin = transform.position + Vector3.up * (radius + 0.01f);
        float checkDist = (capsule ? (capsule.height * 0.5f) : 1f) + groundCheckOffset;

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, checkDist, groundMask, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlopeAngle)
            {
                isGrounded = true;
                groundNormal = hit.normal;
                if (!hasTouchedGround) hasTouchedGround = true;
                lastGroundedTime = Time.time;
                return;
            }
        }

        isGrounded = false;
        groundNormal = Vector3.up;
        // DE: Kein gültiger Boden getroffen -> in der Luft.
        // EN: No valid ground hit -> airborne.
    }

    // ---------- Helpers: Sprint Input ----------
    private bool AnySprintKeyHeld()
    {
        if (sprintKeys == null) return false;
        for (int i = 0; i < sprintKeys.Length; i++)
            if (Input.GetKey(sprintKeys[i])) return true;
        return false;
        // DE: Prüft, ob eine der Sprinttasten gehalten wird.
        // EN: Checks if any sprint key is held.
    }

    private bool AnySprintKeyDown()
    {
        if (sprintKeys == null) return false;
        for (int i = 0; i < sprintKeys.Length; i++)
            if (Input.GetKeyDown(sprintKeys[i])) return true;
        return false;
        // DE: Prüft auf „KeyDown“ für Toggle-Modus.
        // EN: Checks for key down (toggle mode).
    }

    private bool AxisOrButtonHeld()
    {
        if (string.IsNullOrWhiteSpace(sprintAxisOrButton)) return false;
        return Input.GetButton(sprintAxisOrButton) || Input.GetAxisRaw(sprintAxisOrButton) > 0.5f;
        // DE: Unterstützt Button (1/0) und Achsenwerte (>0.5) als „gehalten“.
        // EN: Supports button (1/0) and axis (>0.5) as "held".
    }

    private bool AxisOrButtonDown()
    {
        if (string.IsNullOrWhiteSpace(sprintAxisOrButton)) return false;
        return Input.GetButtonDown(sprintAxisOrButton);
        // DE: „ButtonDown“ für Toggle-Modus via Input-Manager.
        // EN: Button down for toggle mode via Input Manager.
    }

    void OnDrawGizmosSelected()
    {
        if (!capsule) capsule = GetComponent<CapsuleCollider>();
        float radius = capsule ? Mathf.Max(0.05f, capsule.radius - 0.01f) : 0.3f;
        Vector3 origin = transform.position + Vector3.up * (radius + 0.01f);
        float checkDist = (capsule ? (capsule.height * 0.5f) : 1f) + groundCheckOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin + Vector3.down * checkDist, radius);
        // DE: Visualisiert die Bodenprüfung im Editor.
        // EN: Visualizes the ground check in the editor.
    }
}
