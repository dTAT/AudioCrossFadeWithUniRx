using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioEntity : MonoBehaviour, IDisposable
{
    [System.Serializable]
    public class AudioEntityEvent : UnityEvent<AudioEntity> { }
    public uint id;
    bool isValid = false;
    public AudioEntityEvent onReleaseEvent;
    public AudioEntityEvent onDestroyEvent;
    public AudioSource audioSource;
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        onReleaseEvent = new AudioEntityEvent();
        onDestroyEvent = new AudioEntityEvent();
    }
    private void OnDestroy()
    {
        onDestroyEvent.Invoke(this);
    }
    void Release()
    {
        if (isValid)
        {
            onReleaseEvent.Invoke(this);
        }
        Debug.Log("release");
        isValid = false;
    }
    public void Initialize()
    {
        transform.localPosition = Vector3.zero;
        isValid = true;
        audioSource.volume = 1.0f;
        audioSource.pitch = 1.0f;
        audioSource.clip = null;
        audioSource.outputAudioMixerGroup = null;
        audioSource.playOnAwake = false;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }
    public void Activate(bool f)
    {
        gameObject.SetActive(f);
    }
    void IDisposable.Dispose()
    {
        Release();
        Debug.Log("Dispose.");
    }
    public void SetParent(Transform t)
    {
        this.transform.SetParent(t);
    }
    public class Factory
    {
        public AudioEntity Create()
        {
            var go = new GameObject();
            var c = go.AddComponent<AudioEntity>();
            c.id = (uint)UnityEngine.Random.Range(1, 99);
            c.name = c.id.ToString();
            return c;
        }
    }
}