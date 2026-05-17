using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Snapshot taken at end of OnClikStart; Replay applies it. Includes Rigidbody2D, MovePlatform,
/// BloodWallMove, MoveSpike, main camera, FinishLine, CarHeart, GameManager fields.
/// </summary>
[Serializable]
public class GameplaySnapshot
{
    public List<Rigidbody2DState> Rigidbodies = new List<Rigidbody2DState>();
    public List<MovePlatformState> MovePlatforms = new List<MovePlatformState>();
    public List<TransformOnlyState> ExtraTransforms = new List<TransformOnlyState>();

    public int FinishLineInstanceId;
    public bool FinishLineIsFinish;

    public int CarHeartInstanceId;
    public bool CarHeartIsBroken;

    public float GameManagerTimer;
    public bool GameManagerIsRestore;
    public bool GameManagerWinChildActive;
    public E_State GameManagerState;

    public bool CameraZoom2DEnabled = true;

    public static GameplaySnapshot Capture(GameManager gm)
    {
        var snap = new GameplaySnapshot();

        if (gm != null)
        {
            snap.GameManagerTimer = gm.timer;
            snap.GameManagerIsRestore = gm.isRestore;
            snap.GameManagerState = gm.currentState;
            if (gm.transform.childCount > 0)
                snap.GameManagerWinChildActive = gm.transform.GetChild(0).gameObject.activeSelf;
        }

        var rbIds = new HashSet<int>();
        var rbs = UnityEngine.Object.FindObjectsOfType<Rigidbody2D>();
        foreach (var rb in rbs)
        {
            if (rb == null)
                continue;
            rbIds.Add(rb.gameObject.GetInstanceID());
            snap.Rigidbodies.Add(Rigidbody2DState.From(rb));
        }

        foreach (var mp in UnityEngine.Object.FindObjectsOfType<MovePlatform>())
        {
            if (mp == null)
                continue;
            snap.MovePlatforms.Add(new MovePlatformState
            {
                instanceId = mp.gameObject.GetInstanceID(),
                timer = mp.timer,
                moveState = mp.moveState
            });
        }

        void AddExtraIfNotRb(Transform tr)
        {
            if (tr == null)
                return;
            int id = tr.gameObject.GetInstanceID();
            if (rbIds.Contains(id))
                return;
            snap.ExtraTransforms.Add(TransformOnlyState.From(tr));
        }

        foreach (var c in UnityEngine.Object.FindObjectsOfType<BloodWallMove>())
            AddExtraIfNotRb(c.transform);
        foreach (var c in UnityEngine.Object.FindObjectsOfType<MoveSpike>())
            AddExtraIfNotRb(c.transform);

        if (Camera.main != null)
            AddExtraIfNotRb(Camera.main.transform);

        var fls = UnityEngine.Object.FindObjectsOfType<FinishLine>();
        if (fls != null && fls.Length > 0)
        {
            var fl = fls[0];
            snap.FinishLineInstanceId = fl.gameObject.GetInstanceID();
            snap.FinishLineIsFinish = fl.isFinish;
        }

        var chs = UnityEngine.Object.FindObjectsOfType<CarHeart>();
        if (chs != null && chs.Length > 0)
        {
            var ch = chs[0];
            snap.CarHeartInstanceId = ch.gameObject.GetInstanceID();
            snap.CarHeartIsBroken = ch.isBroken;
        }

        var cz = Camera.main != null ? Camera.main.GetComponent<CameraZoom2D>() : null;
        if (cz != null)
            snap.CameraZoom2DEnabled = cz.enabled;

        return snap;
    }

    public void Apply(GameManager gm)
    {
        Rigidbodies.Sort((a, b) => a.hierarchyDepth.CompareTo(b.hierarchyDepth));
        foreach (var s in Rigidbodies)
            s.ApplyTransform();

        ExtraTransforms.Sort((a, b) => a.hierarchyDepth.CompareTo(b.hierarchyDepth));
        foreach (var s in ExtraTransforms)
            s.Apply();

        Physics2D.SyncTransforms();

        foreach (var s in Rigidbodies)
            s.ApplyPhysics();

        foreach (var s in MovePlatforms)
        {
            var go = Resolve(s.instanceId);
            if (go == null)
                continue;
            var mp = go.GetComponent<MovePlatform>();
            if (mp == null)
                continue;
            mp.timer = s.timer;
            mp.moveState = s.moveState;
        }

        if (FinishLineInstanceId != 0)
        {
            var go = Resolve(FinishLineInstanceId);
            var fl = go != null ? go.GetComponent<FinishLine>() : null;
            if (fl != null)
                fl.isFinish = FinishLineIsFinish;
        }

        if (CarHeartInstanceId != 0)
        {
            var go = Resolve(CarHeartInstanceId);
            var ch = go != null ? go.GetComponent<CarHeart>() : null;
            if (ch != null)
                ch.isBroken = CarHeartIsBroken;
        }

        if (Camera.main != null)
        {
            var cz = Camera.main.GetComponent<CameraZoom2D>();
            if (cz != null)
                cz.enabled = CameraZoom2DEnabled;
        }

        if (gm != null)
        {
            gm.timer = GameManagerTimer;
            gm.isRestore = GameManagerIsRestore;
            gm.ChangeStateTo(GameManagerState);
            if (gm.transform.childCount > 0)
                gm.transform.GetChild(0).gameObject.SetActive(GameManagerWinChildActive);
        }
    }

    private static GameObject Resolve(int instanceId) => InstanceIdResolver.TryGetGameObject(instanceId);
}

[Serializable]
public class Rigidbody2DState
{
    public int instanceId;
    public int parentInstanceId;
    public int hierarchyDepth;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;
    public bool activeSelf;

    public Vector2 velocity;
    public float angularVelocity;
    public bool simulated;
    public RigidbodyType2D bodyType;
    public float gravityScale;
    public float drag;
    public float angularDrag;
    public RigidbodyConstraints2D constraints;
    public CollisionDetectionMode2D collisionDetectionMode;

    public static Rigidbody2DState From(Rigidbody2D rb)
    {
        var tr = rb.transform;
        return new Rigidbody2DState
        {
            instanceId = rb.gameObject.GetInstanceID(),
            parentInstanceId = tr.parent != null ? tr.parent.gameObject.GetInstanceID() : 0,
            hierarchyDepth = GetDepth(tr),
            localPosition = tr.localPosition,
            localEulerAngles = tr.localEulerAngles,
            localScale = tr.localScale,
            activeSelf = rb.gameObject.activeSelf,
            velocity = rb.velocity,
            angularVelocity = rb.angularVelocity,
            simulated = rb.simulated,
            bodyType = rb.bodyType,
            gravityScale = rb.gravityScale,
            drag = rb.drag,
            angularDrag = rb.angularDrag,
            constraints = rb.constraints,
            collisionDetectionMode = rb.collisionDetectionMode
        };
    }

    public void ApplyTransform()
    {
        var go = Resolve(instanceId);
        if (go == null)
            return;
        var tr = go.transform;
        Transform parent = null;
        if (parentInstanceId != 0)
        {
            var p = Resolve(parentInstanceId);
            parent = p != null ? p.transform : null;
        }
        tr.SetParent(parent, false);
        tr.localPosition = localPosition;
        tr.localEulerAngles = localEulerAngles;
        tr.localScale = localScale;
        go.SetActive(activeSelf);
    }

    public void ApplyPhysics()
    {
        var go = Resolve(instanceId);
        if (go == null)
            return;
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;
        rb.bodyType = bodyType;
        rb.simulated = simulated;
        rb.gravityScale = gravityScale;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.constraints = constraints;
        rb.collisionDetectionMode = collisionDetectionMode;
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    private static GameObject Resolve(int instanceId) => InstanceIdResolver.TryGetGameObject(instanceId);

    private static int GetDepth(Transform tr)
    {
        int d = 0;
        while (tr.parent != null)
        {
            d++;
            tr = tr.parent;
        }
        return d;
    }
}

[Serializable]
public class TransformOnlyState
{
    public int instanceId;
    public int parentInstanceId;
    public int hierarchyDepth;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;
    public bool activeSelf;

    public static TransformOnlyState From(Transform tr)
    {
        return new TransformOnlyState
        {
            instanceId = tr.gameObject.GetInstanceID(),
            parentInstanceId = tr.parent != null ? tr.parent.gameObject.GetInstanceID() : 0,
            hierarchyDepth = GetDepth(tr),
            localPosition = tr.localPosition,
            localEulerAngles = tr.localEulerAngles,
            localScale = tr.localScale,
            activeSelf = tr.gameObject.activeSelf
        };
    }

    public void Apply()
    {
        var go = Resolve(instanceId);
        if (go == null)
            return;
        var tr = go.transform;
        Transform parent = null;
        if (parentInstanceId != 0)
        {
            var p = Resolve(parentInstanceId);
            parent = p != null ? p.transform : null;
        }
        tr.SetParent(parent, false);
        tr.localPosition = localPosition;
        tr.localEulerAngles = localEulerAngles;
        tr.localScale = localScale;
        go.SetActive(activeSelf);
    }

    private static GameObject Resolve(int instanceId) => InstanceIdResolver.TryGetGameObject(instanceId);

    private static int GetDepth(Transform tr)
    {
        int d = 0;
        while (tr.parent != null)
        {
            d++;
            tr = tr.parent;
        }
        return d;
    }
}

[Serializable]
public class MovePlatformState
{
    public int instanceId;
    public float timer;
    public MoveState moveState;
}
