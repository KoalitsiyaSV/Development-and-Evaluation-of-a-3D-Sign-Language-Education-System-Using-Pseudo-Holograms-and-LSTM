using Leap.Unity;
using Leap;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BoneSnapshot
{
    public string bonePath;
    public float time;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
}

public class HandAnimationRecorder : MonoBehaviour
{
    public Transform rootBone; // 최상위 뼈 (예: Armature)
    public string animationClipName = "HandAnimation";
    public float recordingDuration = 5f; // 기록할 시간(초)
    private float elapsedTime = 0f;
    private bool isRecording = false;
    private List<BoneSnapshot> snapshots;

    void Start()
    {
        snapshots = new List<BoneSnapshot>();
    }

    void Update()
    {
        if (isRecording)
        {
            RecordBoneTransforms(rootBone);
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= recordingDuration)
            {
                StopRecording();
                CreateAnimationClip();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRecording();
        }
    }

    void StartRecording()
    {
        Debug.Log("Recording started.");
        snapshots.Clear();
        elapsedTime = 0f;
        isRecording = true;
    }

    void StopRecording()
    {
        Debug.Log("Recording stopped.");
        isRecording = false;
    }

    void RecordBoneTransforms(Transform bone)
    {
        BoneSnapshot snapshot = new BoneSnapshot
        {
            bonePath = GetBonePath(bone),
            time = elapsedTime,
            localPosition = bone.localPosition,
            localRotation = bone.localRotation,
            localScale = bone.localScale
        };
        snapshots.Add(snapshot);

        foreach (Transform child in bone)
        {
            RecordBoneTransforms(child);
        }
    }

    string GetBonePath(Transform bone)
    {
        string path = bone.name;
        while (bone.parent != null && bone.parent != rootBone.parent)
        {
            bone = bone.parent;
            path = bone.name + "/" + path;
        }
        return path;
    }

    void CreateAnimationClip()
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = false;

        var groupedSnapshots = new Dictionary<string, List<BoneSnapshot>>();
        foreach (var snapshot in snapshots)
        {
            if (!groupedSnapshots.ContainsKey(snapshot.bonePath))
            {
                groupedSnapshots[snapshot.bonePath] = new List<BoneSnapshot>();
            }
            groupedSnapshots[snapshot.bonePath].Add(snapshot);
        }

        foreach (var kvp in groupedSnapshots)
        {
            string bonePath = kvp.Key;
            List<BoneSnapshot> boneSnapshots = kvp.Value;

            AnimationCurve posXCurve = new AnimationCurve();
            AnimationCurve posYCurve = new AnimationCurve();
            AnimationCurve posZCurve = new AnimationCurve();
            AnimationCurve rotXCurve = new AnimationCurve();
            AnimationCurve rotYCurve = new AnimationCurve();
            AnimationCurve rotZCurve = new AnimationCurve();
            AnimationCurve rotWCurve = new AnimationCurve();
            AnimationCurve scaleXCurve = new AnimationCurve();
            AnimationCurve scaleYCurve = new AnimationCurve();
            AnimationCurve scaleZCurve = new AnimationCurve();

            foreach (var snapshot in boneSnapshots)
            {
                float time = snapshot.time;
                posXCurve.AddKey(time, snapshot.localPosition.x);
                posYCurve.AddKey(time, snapshot.localPosition.y);
                posZCurve.AddKey(time, snapshot.localPosition.z);
                rotXCurve.AddKey(time, snapshot.localRotation.x);
                rotYCurve.AddKey(time, snapshot.localRotation.y);
                rotZCurve.AddKey(time, snapshot.localRotation.z);
                rotWCurve.AddKey(time, snapshot.localRotation.w);
                scaleXCurve.AddKey(time, snapshot.localScale.x);
                scaleYCurve.AddKey(time, snapshot.localScale.y);
                scaleZCurve.AddKey(time, snapshot.localScale.z);
            }

            clip.SetCurve(bonePath, typeof(Transform), "localPosition.x", posXCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localPosition.y", posYCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localPosition.z", posZCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localRotation.x", rotXCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localRotation.y", rotYCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localRotation.z", rotZCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localRotation.w", rotWCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localScale.x", scaleXCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localScale.y", scaleYCurve);
            clip.SetCurve(bonePath, typeof(Transform), "localScale.z", scaleZCurve);
        }

        //AssetDatabase.CreateAsset(clip, $"Assets/Resources/{animationClipName}.anim");
        //AssetDatabase.SaveAssets();

        Debug.Log($"Animation clip '{animationClipName}' created and saved.");
    }
}
