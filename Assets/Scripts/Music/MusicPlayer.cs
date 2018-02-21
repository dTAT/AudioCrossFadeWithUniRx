using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Operators;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using Zenject;

/// <summary>
/// 楽曲プレイヤー
/// </summary>
public class MusicPlayer : MonoBehaviour {
	/// <summary>
	/// 楽曲再生に必要な情報オブジェクト
	/// </summary>
	MusicParamObject musicParam;
	/// <summary>
	/// 音構造
	/// </summary>
	AudioEntity audioEntity;
	AudioSource audioSource;
	/// <summary>
	/// 音情報キャッシュ
	/// </summary>
	[Inject] AudioPool audioPool;
	IDisposable attackSubscripion;
	/// <summary>
	/// 初期化処理
	/// </summary>
	/// <param name="music">再生楽曲情報</param>
	void Setup (MusicParamObject music) {
		musicParam = music;
		audioEntity = audioPool.Rent ();
		audioSource = audioEntity.audioSource;
		if (audioSource == null) {
			Debug.LogError ("null source");
		}
	}
	/// <summary>
	/// 再生開始
	/// </summary>
	/// <param name="music">再生楽曲情報</param>
	/// <param name="Immediately">即時再生開始するか</param>
	public void Play (MusicParamObject music, bool Immediately = false) {
		Setup (music);
		audioSource.clip = music.musicClip;
		audioSource.outputAudioMixerGroup = music.outputMixerGroup;
		audioSource.Play ();
		if (Immediately) {
			return;
		}
		var curve = music.attackCCurve;
		var timespan = music.attackTime;
		//fadein
		attackSubscripion = FadeVolume (curve, timespan, () => { });
	}
	/// <summary>
	/// 終了前処理
	/// </summary>
	void Cleanup () {
		audioSource.Stop ();
		if (audioEntity != null) {
			audioPool.Return (audioEntity);
		}
		Destroy (this.gameObject);
	}
	/// <summary>
	/// 停止処理
	/// </summary>
	/// <param name="Immediateely"></param>
	public void Stop (bool Immediateely = false) {
		attackSubscripion.Dispose ();
		var timespan = musicParam.releaseTime;
		var curve = musicParam.releaseCurve;
		if (Immediateely) {
			Cleanup ();
			return;
		}
		FadeVolume (curve, timespan, Cleanup);
	}

	IDisposable FadeVolume (AnimationCurve curve, float duration, UnityAction onComplete) {
		var ret = Observable
			.FromCoroutine<float> (o => TransitionVolume (o, curve, duration))
			//TransitionVolumeのOnNextごとに呼ばれる
			.Subscribe (v => {
				audioSource.volume = v;
			}, () => onComplete.Invoke ()).AddTo (this);
		return ret;
	}

	IEnumerator TransitionVolume (IObserver<float> observer, AnimationCurve curve, float duration) {
		var timer = duration;
		while (timer > 0) {
			timer -= Time.deltaTime;
			var v = curve.Evaluate (1 - Mathf.Clamp01 (timer / duration));
			observer.OnNext (v);
			yield return null;
		}
		observer.OnCompleted ();
	}
}