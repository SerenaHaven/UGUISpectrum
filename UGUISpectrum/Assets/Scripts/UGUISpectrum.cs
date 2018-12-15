using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUISpectrum : MonoBehaviour {
	private UGUIOscilloscope oscilloscope = null;
	[SerializeField]private int resolution = 1024;
	[SerializeField]private int segment = 50;
	[SerializeField]private float step = 0.001f;
	[SerializeField]private float lerpSpeed = 10.0f;
	[SerializeField]private float scale = 20.0f;
	private float[] samplesRemain;
	void Start () {
		oscilloscope = this.GetComponent<UGUIOscilloscope> ();	
	}
	
	void Update () {
		if (segment <= 0 || segment > resolution) {
			return;
		}

		float[] spectrum = new float[resolution];
		AudioListener.GetSpectrumData (spectrum, 0, FFTWindow.Hanning);

		float[] samples = new float[segment];
		for (int i = 0; i < resolution; i++) {
			int index = (int)(spectrum [i] / step);
			if (index < segment && index >= 0) {
				samples [index] += spectrum [i];
			}
		}
		for (int i = 0; i < segment; i++) {
			samples [i] *= scale;
		}
		if (samplesRemain == null || samplesRemain.Length != segment) {
			samplesRemain = new float[segment];
		}
		for (int i = 0; i < samplesRemain.Length; i++) {
			samplesRemain[i] = Mathf.Lerp(samplesRemain[i], samples[i], Time.deltaTime * lerpSpeed);
		}
		oscilloscope.SetSamples (samplesRemain);
	}
}