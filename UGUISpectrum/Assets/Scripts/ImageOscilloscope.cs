using UnityEngine;
using System.Collections;

public class ImageOscilloscope : MonoBehaviour
{
	private int resolution = 1024;
	private float[] frequeceCounter;
	private RectTransform[] waveUnits;
	[SerializeField]private GameObject unitTemplate = null;
	[Range (1, 1000)]
	[SerializeField]private int unitCount = 40;
	[SerializeField]private float step = 0.001f;
	[SerializeField]private float scale = 20.0f;
	[SerializeField]private float lerpSpeed = 10.0f;

	void Start ()
	{
		waveUnits = new RectTransform[unitCount];
		for (int i = 0; i < unitCount; i++) {
			waveUnits [i] = GameObject.Instantiate (unitTemplate).GetComponent<RectTransform> ();
			waveUnits [i].transform.SetParent (this.transform);
			waveUnits [i].gameObject.SetActive (true);
			waveUnits [i].transform.localScale = Vector3.zero;
		}
	}

	void Update ()
	{
		frequeceCounter = new float[unitCount];
		float[] fft = new float[resolution];
		AudioListener.GetSpectrumData (fft, 0, FFTWindow.BlackmanHarris);
		for (int i = 0; i < resolution; i++) {
			int index = (int)(fft [i] / step);
			if (index < unitCount && index >= 0) {
				frequeceCounter [index] += fft [i];
			}
		}
		for (int i = 0; i < unitCount; i++) {
			Vector3 newScale = new Vector3 (1, frequeceCounter [i] * scale, 1);
			waveUnits [i].localScale = Vector3.Lerp (waveUnits [i].localScale, newScale, Time.deltaTime * lerpSpeed);
		}
	}
}
