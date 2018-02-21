using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour {
	Image screenImage;
	Color currentColor = Color.black;
	public UnityEvent OnTransitionComplete = new UnityEvent ();
	[SerializeField]
	AnimationCurve openCurve;
	[SerializeField]
	AnimationCurve closeCuve;

	private void Awake () {
		screenImage = GetComponent<Image> ();
		screenImage.color = currentColor;
	}
	public void Open (Color color, float duration) {
		screenImage.raycastTarget = true;
		color.a = 0.0f;
		Observable
			.FromCoroutine<Color> (
				o => TransitionColor (o, currentColor, color, openCurve, duration))
			.Subscribe (c => screenImage.color = c, () => {
				screenImage.raycastTarget = false;
				OnTransitionComplete.Invoke ();
				currentColor = color;
			});
	}

	public void Close (Color color, float duration) {
		screenImage.raycastTarget = true;
		Observable
			.FromCoroutine<Color> (
				o => TransitionColor (o, currentColor, color, openCurve, duration))
			.Subscribe (c => screenImage.color = c, () => {
				OnTransitionComplete.Invoke ();
				currentColor = color;
			});
	}
	IEnumerator TransitionColor (IObserver<Color> observer, Color begin, Color end, AnimationCurve curve, float duration) {
		var timer = duration;
		while (timer > 0) {
			timer -= Time.deltaTime;
			var r = curve.Evaluate (timer / duration);
			var c = Color.Lerp (end, begin, r);
			observer.OnNext (c);
			yield return null;
		}
		observer.OnCompleted ();
	}
}