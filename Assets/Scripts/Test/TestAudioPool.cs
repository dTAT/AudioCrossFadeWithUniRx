using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestAudioPool {

	[Test]
	public void TestAudioPoolSimplePasses () {
		// Use the Assert class to test conditions.
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator TestAudioPoolWithEnumeratorPasses () {
		var pool = new GameObject ().AddComponent<AudioPool> ();
		//生成確認
		yield return null;
		Assert.IsNotNull (pool);
		//数量確認
		int count = pool.Count;
		//初期化確認
		Assert.AreEqual (count, pool.Count);
		yield return new WaitForSeconds (1.0f);
		var a = pool.Lend ();
		Assert.IsNotNull (a);
		//生成されたことで一つ減っているのを確認
		Assert.AreEqual (count - 1, pool.Count);
		yield return new WaitForSeconds (1.0f);
		pool.Return (a);
		//返却することで元の数に戻っているのを確認
		Assert.AreEqual (count, pool.Count);
		yield return new WaitForSeconds (1.0f);

		const int lentalCount = 5;
		var sourceList = new List<AudioEntity> ();
		for (var i = 0; i < lentalCount; ++i) {
			var b = pool.Lend ();
			sourceList.Add (b);
		}
		//借りた分減っていることを確認
		Assert.AreEqual ((count - lentalCount), pool.Count);
		var rand = new System.Random ();
		var randList = new List<AudioEntity> ();
		while (sourceList.Count > 0) {
			var index = rand.Next (0, sourceList.Count);
			randList.Add (sourceList[index]);
			sourceList.RemoveAt (index);
		}
		yield return new WaitForSeconds (1.0f);
		//ランダムな順番で返却する
		while (randList.Count > 0) {
			var c = randList[0];
			randList.Remove (c);
			pool.Return (c);
			yield return new WaitForSeconds (1.0f);
		}
		yield return new WaitForSeconds (1.0f);
		//返却数を確認する
		Assert.AreEqual (count, pool.Count);
		var mayDestroyObject = new GameObject ();
		var mayDestroyEntity = pool.Lend ();
		mayDestroyEntity.transform.SetParent (mayDestroyObject.transform);
		yield return new WaitForSeconds (1.0f);
		//貸出先のオブジェクトを破棄する
		GameObject.Destroy (mayDestroyEntity.gameObject);
		yield return new WaitForSeconds (1.0f);
		Assert.AreEqual (count, pool.Count);
		yield return new WaitForSeconds (1.0f);
	}
}