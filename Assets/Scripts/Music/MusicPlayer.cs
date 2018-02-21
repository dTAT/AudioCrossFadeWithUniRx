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
	/// <summary>
	/// 楽曲再生管理クラスへの参照
	/// </summary>
	[Inject] MusicController musicController;
	IDisposable attackSubscripion;
	/// <summary>
	/// 途中で使用したスナップショット
	/// </summary>
	AudioMixerSnapshot dirtySnapshot = null;
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
		//スナップショットを汚しているならば短い時間で戻しておく
		if (null != dirtySnapshot) {
			musicController.ResetMixerSnapshot (dirtySnapshot, 0.04f);
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
		musicController.SetMixerSnapshot (musicParam.releaseSnapshot, timespan);
		dirtySnapshot = musicParam.releaseSnapshot;
	}
	/// <summary>
	/// 音量を変化する
	/// </summary>
	/// <param name="curve">変化カーブ</param>
	/// <param name="duration">変化時間</param>
	/// <param name="onComplete">変化完了時処理</param>
	/// <returns></returns>
	IDisposable FadeVolume (AnimationCurve curve, float duration, UnityAction onComplete) {
		var ret = Observable
			.FromCoroutine<float> (o => TransitionVolume (o, curve, duration))
			//TransitionVolumeのOnNextごとに呼ばれる
			.Subscribe (v => {
				audioSource.volume = v;
			}, () => onComplete.Invoke ()).AddTo (this);
		return ret;
	}
	/// <summary>
	/// 変化時間中のカーブ値を流すCoroutine
	/// </summary>
	/// <param name="observer">Coroutine処理のObserver</param>
	/// <param name="curve">変化カーブ</param>
	/// <param name="duration">変化時間</param>
	/// <returns>処理Coroutine</returns>
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