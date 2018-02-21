using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class IllustController : MonoBehaviour {
	[Inject]
	TransitionScreen transitionScreen;
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
			sceneExit.ExitScne ();
			transitionScreen.Close (sceneExit.FadeoutColor, sceneExit.FadeoutDuration);
			while (!closed) yield return null;
			transitionScreen.OnTransitionComplete.RemoveListener (wait);
			sceneExit.gameObject.SetActive (false);
		}
		if (sceneEnter != null) {
			bool closed = true;
			UnityEngine.Events.UnityAction wait = () => {
				closed = false;
			};
			sceneEnter.gameObject.SetActive (true);
			transitionScreen.OnTransitionComplete.AddListener (wait);
			sceneEnter.EntertScene ();
			transitionScreen.Open (sceneEnter.FadeinColor, sceneEnter.FadeinDuration);
			while (closed) yield return null;
			transitionScreen.OnTransitionComplete.RemoveListener (wait);
		}
	}
}