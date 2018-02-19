using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;
/// <summary>
/// 楽曲再生管理クラス
/// </summary>
public class MusicController : MonoBehaviour {
	public MusicParamObject[] musicParamObjects;
	public GameObject musicPlayerPrefab;
	[Inject]
	AudioPool audioPool;
	[Inject]
	DiContainer diContainer;
	List<MusicPlayer> musicPlayers = new List<MusicPlayer> ();
	/// <summary>
	/// 再生開始
	/// </summary>
	/// <param name="MusicName">再生する楽曲のプログラム内の名前</param>
	public void Play (string MusicName, bool Immediately = false) {
		var music = SelectMusic (MusicName);
		if (null == music) {
			return;
		}
		var go = diContainer.InstantiatePrefab (musicPlayerPrefab);
		go.transform.SetParent (transform);
		var player = go.GetComponent<MusicPlayer> ();
		player.Play (music, Immediately);
		musicPlayers.Add (player);
	}
	/// <summary>
	/// 全部止める
	/// </summary>
	public void StopAll (bool Immediately = false) {
		musicPlayers.ForEach (x => x.Stop (Immediately));
		musicPlayers.Clear ();
	}
	/// <summary>
	/// 楽曲情報オブジェクトの取得
	/// </summary>
	/// <param name="name">再生する楽曲情報オブジェクト</param>
	/// <returns></returns>
	MusicParamObject SelectMusic (string name) {
		if (musicParamObjects == null)
			return null;
		var music = musicParamObjects.First (x => x.Name == name);
		return music;
	}
}