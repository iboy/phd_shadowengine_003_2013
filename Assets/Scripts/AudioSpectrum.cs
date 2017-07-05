using UnityEngine;
using System.Collections;

public class AudioSpectrum : MonoBehaviour {
	public float spacer = 10f;
	public float multiplier = 1f;
	void Update() {
		float[] spectrum = audio.GetSpectrumData(1024, 0, FFTWindow.BlackmanHarris);
		int i = 1;
		while (i < 1023) {
			Debug.DrawLine(new Vector3((i - 1), (spectrum[i] + spacer), 0), new Vector3(i, (spectrum[i + 1] + spacer), 0), Color.red);
			Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + spacer, 2), new Vector3(i, Mathf.Log(spectrum[i]) + spacer, 2), Color.cyan);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - spacer, 1), new Vector3(Mathf.Log(i), spectrum[i] - spacer, 1), Color.green);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.yellow);
			i++;
		}
	}
}