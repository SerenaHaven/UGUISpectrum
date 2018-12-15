using UnityEngine;
using UnityEngine.UI;

public class UGUIOscilloscope : RawImage {
	private enum Style{OneSide, TwoSides, FourSides};
	[SerializeField]private Style style = Style.OneSide;
	[SerializeField]private bool fitRectTransform = true;
	[SerializeField]private float centerWidth = 1.0f;
	private float[] samples;
	private float max;

	private UGUIOscilloscope()
	{
		base.useLegacyMeshGeneration = false;
	}

	public void SetSamples(float[] samples)
	{
		if (samples == null) {
			return;
		}
		this.samples = samples;
		max = 0.0f;
		for (int i = 0; i < samples.Length; i++) {
			max = samples [i] > max ? samples [i] : max;
		}
		if (max == 0.0f) {
			return;
		}
		if (fitRectTransform == false) {
			switch (style) {
			case Style.OneSide:
				rectTransform.pivot = new Vector2 (rectTransform.pivot.x, 0.0f);
				break;
			case Style.TwoSides:
				rectTransform.pivot = new Vector2 (rectTransform.pivot.x, 0.5f);
				break;
			case Style.FourSides:
				rectTransform.pivot = new Vector2 (0.5f, 0.5f);
				break;
			}
			rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, max);
		}
		OnValidate();
	}

	protected override void OnPopulateMesh (VertexHelper vh)
	{
		vh.Clear ();
		if (samples == null || samples.Length <= 1 || rectTransform.rect.width == 0 || rectTransform.rect.height == 0) {
			return;
		}

		int samplesLength = samples.Length;
		float width = rectTransform.rect.width;
		float height = rectTransform.rect.height;
		float multiplier = fitRectTransform == true ? (height / max) : 1.0f;
		float segmentWidth = width / (samplesLength - 1);
		Vector3[] vertices = null;
		Vector3 offset = default(Vector3);

		switch (style) {
		case Style.OneSide:
			vertices = new Vector3[samplesLength * 2];
			offset = new Vector3 (width * rectTransform.pivot.x, height * rectTransform.pivot.y, 0.0f);
			for (int i = 0; i < samplesLength; i++) {
				float y = samples [i] * multiplier;
				y = y == 0 ? centerWidth : y;
				vertices [i] = new Vector3 (segmentWidth * i, y, 0.0f) - offset;
				vertices [i + samplesLength] = new Vector3 (segmentWidth * i, 0.0f, 0.0f) - offset;
			}
			break;
		case Style.TwoSides:
			vertices = new Vector3[samplesLength * 2];
			offset = new Vector3 (width * rectTransform.pivot.x, height * (rectTransform.pivot.y - 0.5f), 0.0f);
			multiplier *= 0.5f;
			for (int i = 0; i < samplesLength; i++) {
				float y = samples [i] * multiplier;
				y = y == 0 ? centerWidth : y;
				vertices [i] = new Vector3 (segmentWidth * i, y, 0.0f) - offset;
				vertices [i + samplesLength] = new Vector3 (segmentWidth * i, -y, 0.0f) - offset;
			}
			break;
		case Style.FourSides:
			vertices = new Vector3[samplesLength * 4 - 2];
			offset = new Vector3 (width * (rectTransform.pivot.x - 0.5f), height * (rectTransform.pivot.y - 0.5f), 0.0f);
			multiplier *= 0.5f;
			segmentWidth *= 0.5f;
			vertices [samplesLength - 1] = new Vector3 (0.0f, samples [0] * multiplier, 0.0f) - offset;
			vertices [samplesLength * 3 - 2] = new Vector3 (0.0f, -samples [0] * multiplier, 0.0f) - offset;
			for (int i = 0; i < samplesLength - 1; i++) {
				float x = segmentWidth * (i - samplesLength + 1);
				float y = samples [Mathf.Abs(i - samplesLength + 1)] * multiplier;
				y = y == 0 ? centerWidth : y;
				vertices [i] = new Vector3 (x, y, 0.0f) - offset;
				vertices [samplesLength * 2 - 2 - i] = new Vector3 (-x, y, 0.0f) - offset;
				vertices [samplesLength * 2 - 1 + i] = new Vector3 (x, -y, 0.0f) - offset;
				vertices [samplesLength * 4 - 3 - i] = new Vector3 (-x, -y, 0.0f) - offset;
			}
			break;
		}

		if (vertices != null && vertices.Length > 0) {
			for (int i = 0; i < vertices.Length; i++) {
				vh.AddVert (vertices [i], color, new Vector2 (vertices [i].x / width, vertices [i].y / height));
			}
			int halfVerticesLength = (int)(vertices.Length * 0.5f);
			for (int i = 0; i < halfVerticesLength - 1; i++) {
				vh.AddTriangle (i, i + halfVerticesLength + 1, i + halfVerticesLength);
				vh.AddTriangle (i, i + 1, i + halfVerticesLength + 1);
			}
		}
	}
}