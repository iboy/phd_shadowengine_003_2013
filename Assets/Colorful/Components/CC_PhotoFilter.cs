using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Photo Filter")]
public class CC_PhotoFilter : CC_Base
{
	public Color color = new Color(1.0f, 0.5f, 0.2f, 1.0f);
	public float density = 0.35f;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (density == 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}

		material.SetColor("_rgb", color);
		material.SetFloat("_density", density);
		Graphics.Blit(source, destination, material);
	}
}
