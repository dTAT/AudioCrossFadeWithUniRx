using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour {
	[SerializeField]
	private const int PoolingNumber = 10;
	Queue<AudioEntity> audioQueue;

	AudioEntity.Factory factory;
	public void Awake () {
		factory = new AudioEntity.Factory ();
		audioQueue = new Queue<AudioEntity> (PoolingNumber);
		Initialize (PoolingNumber);
	}

	public AudioEntity Lend () {
		var ae = audioQueue.Dequeue ();
		ae.Activate (true);
		ae.Initialize ();
		return ae;
	}
	public void Return (AudioEntity entity) {
		entity.Initialize ();
		entity.Activate (false);
		entity.SetParent (this.transform);
		audioQueue.Enqueue (entity);
	}

	public int Count {
		get {
			return audioQueue.Count;
		}
	}
	public void Initialize (int count) {
		FillPool (count);
	}

	void EmptyPool () {
		var count = audioQueue.Count;
		for (int i = 0; i < count; ++i) {
			var a = audioQueue.Dequeue ();
			Destroy (a.gameObject);
		}
		audioQueue.Clear ();
	}

	void FillPool (int count) {
		for (var i = 0; i < count; ++i) {
			var entity = CreateEntity ();
			audioQueue.Enqueue (entity);
		}
	}
	AudioEntity CreateEntity () {
		var entity = factory.Create ();
		entity.SetParent (transform);
		entity.Initialize ();
		entity.Activate (false);
		entity.onReleaseEvent.AddListener (OnEntityRelease);
		entity.onDestroyEvent.AddListener (OnEntityRelease);
		return entity;
	}
	void OnEntityRelease (AudioEntity entity) {
		Return (entity);
	}
}