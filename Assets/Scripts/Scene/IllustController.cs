using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class IllustController : MonoBehaviour {
	[Inject]
	TransitionScreen transitionScreen;
	[Inject]
	MusicController musicController;
	SceneComponent[] scenes = null;
	SceneComponent lastScene = null;
	private void Awake () {
		scenes = GetComponentsInChildren<SceneComponent> ();
		CloseScenes ();
		RequestChange ("NONE");
	}

	void CloseScenes () {
		foreach (var s in scenes) {
			s.gameObject.SetActive (false);
		}
	}
	SceneComponent SelectScele (string sceneName) {
		SceneComponent scene = null;
		foreach (var item in scenes) {
			if (item.SceneName == sceneName) {
				scene = item;
				break;
			}
		}
		return scene;
	}
	public void RequestChange (string sceneName) {
		Debug.Log ("REQUEST: " + sceneName);
		var scene = SelectScele (sceneName);
		if (null == scene) {
			Debug.Log ("NOT FOUND");
			return;
		}
		StartCoroutine (SwitchSene (lastScene, scene));
		lastScene = scene;
	}

	IEnumerator SwitchSene (SceneComponent sceneExit, SceneComponent sceneEnter) {
		if (sceneExit != null) {
			bool closed = false;
			UnityEngine.Events.UnityAction wait = () => {
				closed = true;
			};
			transitionScreen.OnTransitionComplete.AddListener (wait);
			//シーンの終了処理を行う
			sceneExit.ExitScne ();
			//画面をフェードアウトさせる
			transitionScreen.Close (sceneExit.FadeoutColor, sceneExit.FadeoutDuration);
			//フェード完了までまつ
			while (!closed) yield return null;
			transitionScreen.OnTransitionComplete.RemoveListener (wait);
			//フェード完了後処理をする
			sceneExit.OnCompleteExit ();
			sceneExit.gameObject.SetActive (false);
		}
		if (sceneEnter != null) {
			bool closed = true;
			UnityEngine.Events.UnityAction wait = () => {
				closed = false;
			};
			sceneEnter.gameObject.SetActive (true);
			transitionScreen.OnTransitionComplete.AddListener (wait);
			//シーンの開始処理をする
			sceneEnter.EntertScene ();
			//画面をフェードインさせる
			transitionScreen.Open (sceneEnter.FadeinColor, sceneEnter.FadeinDuration);
			//フェードインをまつ
			while (closed) yield return null;
			transitionScreen.OnTransitionComplete.RemoveListener (wait);
		}
	}
}