using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// 楽曲再生に必要な情報オブジェクト
/// </summary>
public class MusicParamObject : ScriptableObject {
	/// <summary>
	/// プログラム内での名前
	/// </summary>
	public string Name;
	/// <summary>
	/// 再生開始時のフェードインカーブ
	/// </summary>
	public AnimationCurve attackCCurve;
	/// <summary>
	/// フェードイン時間 
	/// </summary>
	public float attackTime;
	/// <summary>
	/// フェードアウトカーブ
	/// </summary>
	public AnimationCurve releaseCurve;
	/// <summary>
	///フェードアウト時間 
	/// </summary>
	public float releaseTime;
	/// <summary>
	/// 再生時につなぐミキサーグループ
	/// </summary>
	public AudioMixerGroup outputMixerGroup;
	/// <summary>
	/// 再生音源
	/// </summary>
	public AudioClip musicClip;
}