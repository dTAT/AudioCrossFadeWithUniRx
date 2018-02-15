using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class ExampleRunner : MonoBehaviour {
	[Inject] MusicController musicController;
	void Start () {
		musicController.Play ("STREAM1");
		var timer1 = 20.0f;
		var timer2 = 25.0f;
		Observable.Timer (TimeSpan.FromSeconds (timer1)).Subscribe ((_) => {
			musicController.StopAll ();
			musicController.Play ("STREAM2");
		});
		Observable.Timer (TimeSpan.FromSeconds (timer1 + timer2)).Subscribe ((_) => {
			musicController.StopAll ();
			musicController.Play ("STREAM3");
		});
	}
}