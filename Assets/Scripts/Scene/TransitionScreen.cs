using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 遷移時のスクリーン
/// </summary>
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
	/// <summary>
	/// スクリーンを開ける＝フェードイン
	/// </summary>
	/// <param name="color">フェードイン色</param>
	/// <param name="duration">フェードイン時間</param>
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
	/// <summary>
	/// スクリーンを閉める=フェードアウト
	/// </summary>
	/// <param name="color">フェードアウト色</param>
	/// <param name="duration">フェードアウト時間</param>
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
	/// <summary>
	/// 色遷移Coroutine
	/// </summary>
	/// <param name="observer">色フェードオブザーバ</param>
	/// <param name="begin">開始色</param>
	/// <param name="end">終了色</param>
	/// <param name="curve">遷移カーブ</param>
	/// <param name="duration">遷移時間</param>
	/// <returns></returns>
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