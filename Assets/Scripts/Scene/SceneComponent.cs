using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SceneComponent : MonoBehaviour {
	/// <summary>
	/// 再生するシーン名
	/// </summary>
	public string SceneName;
	/// <summary>
	/// フェードインする際の色
	/// </summary>
	public Color FadeinColor;
	/// <summary>
	/// フェードインする時間
	/// </summary>
	public float FadeinDuration;
	/// <summary>
	/// フェードアウトする際の色
	/// </summary>
	public Color FadeoutColor;
	/// <summary>
	/// フェードアウトする時間
	/// </summary>
	public float FadeoutDuration;
	/// <summary>
	/// フェードアウトする際のSnapshot
	/// </summary>
	public AudioMixerSnapshot FadeoutSnapshot;

	[Inject] MusicController musicController;
	public string MusicName;
	/// <summary>
	/// シーン開始時の処理
	/// </summary>
	public void EntertScene () {
		Debug.Log ("START SCENE:" + SceneName);
		if (!string.IsNullOrEmpty (MusicName)) {
			musicController.Play (MusicName);
		}
	}
	/// <summary>
	/// シーン終了時の処理
	/// </summary>
	public void ExitScne () {
		Debug.Log ("EXIT SCENE:" + SceneName);
		musicController.StopAll ();
		musicController.SetMixerSnapshot (FadeoutSnapshot, FadeoutDuration);
	}
	/// <summary>
	/// シーン終了完了時の処理
	/// </summary>
	public void OnCompleteExit () {
		musicController.ResetMixerSnapshot (FadeoutSnapshot, 0.04f);
	}
}