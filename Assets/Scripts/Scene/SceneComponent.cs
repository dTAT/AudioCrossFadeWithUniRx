using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneComponent : MonoBehaviour {
	public string SceneName;
	public Color FadeinColor;
	public float FadeinDuration;
	public Color FadeoutColor;
	public float FadeoutDuration;

	[Inject] MusicController musicController;
	public string MusicName;
	public void EntertScene () {
		Debug.Log ("START SCENE:" + SceneName);
		if (!string.IsNullOrEmpty (MusicName)) {
			musicController.Play (MusicName);
		}
	}
	public void ExitScne () {
		Debug.Log ("EXIT SCENE:" + SceneName);
		musicController.StopAll ();
	}
}