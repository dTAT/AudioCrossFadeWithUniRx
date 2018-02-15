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
	/// 初期化処理
	/// </summary>
	/// <param name="music">再生楽曲情報</param>
	void Setup (MusicParamObject music) {
		musicParam = music;
		audioEntity = audioPool.Lend ();
		audioSource = audioEntity.audioSource;
		if (audioSource == null) {
			Debug.LogError ("null source");
		}
	}
	/// <summary>
	/// 再生開始
	/// </summary>
	/// <param name="music">再生楽曲情報</param>
	/// <param name="Immidiate">即時再生開始するか</param>
	public void Play (MusicParamObject music, bool Immidiate = false) {
		Setup (music);
		audioSource.clip = music.musicClip;
		audioSource.outputAudioMixerGroup = music.outputMixerGroup;
		audioSource.Play ();
		if (Immidiate) {
			return;
		}
		var curve = music.attackCCurve;
		var timespan = music.attackTime;
		//fadein
		ControlVolumeWithCurve (curve, timespan, () => { });
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
	/// <param name="Immidiate"></param>
	public void Stop (bool Immidiate = false) {
		var timespan = musicParam.releaseTime;
		var curve = musicParam.releaseCurve;
		if (Immidiate) {
			Cleanup ();
			return;
		}
		ControlVolumeWithCurve (curve, timespan, Cleanup);
	}
	/// <summary>
	///　指定カーブと時間に合わせて音量を調整する
	/// </summary>
	/// <param name="curve">音量調整カーブ</param>
	/// <param name="timespan">音量調整時間</param>
	/// <param name="onComplete">完了後処理</param>//
	private void ControlVolumeWithCurve (AnimationCurve curve, float timespan, UnityAction onComplete) {
		this.UpdateAsObservable ()
			.Select (_ => Time.deltaTime) //deltaを流して
			.Scan ((sum, delta) => sum += delta) //deltaの合計を得て
			.TakeWhile (x => x < timespan) //終了時刻前なら
			.Select (x => x / timespan) //開始から終了までの比率を計算して流して
			.Subscribe ((x) => {
				var ratio = curve.Evaluate (x);
				audioSource.volume = ratio;
			}, () => { //終わったらcomplete
				onComplete.Invoke ();
			}).AddTo (this);
	}
}