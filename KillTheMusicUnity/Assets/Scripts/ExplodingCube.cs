using System.Collections;
using UnityEngine;

/// <summery>
/// Prototyp for exploding cube
/// </summery>

public class ExplodingCube: MonoBehaviour {

	private float force = 100f;
	private float radius = 80f;
	private float upward = 50f;
	private Vector3 position;
	private Material material;
	private Material debrisColor;
	private int numberOfPieces = 100;
	Object prefab; 

	private void SetMaterial2Transparent(Material m)
	{
		m.SetFloat("_Mode", 2);
		m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		m.SetInt("_ZWrite", 0);
		m.DisableKeyword("_ALPHATEST_ON");
		m.EnableKeyword("_ALPHABLEND_ON");
		m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		m.renderQueue = 3000;
	}

	IEnumerator turnOffColider(BoxCollider col, float waitTime)
	{

		yield return new WaitForSeconds(waitTime);
		col.isTrigger = true;



		yield  break;
	}

	IEnumerator fadeToInvisibilityAndDestroy(GameObject piece, float fadeTime)
	{
		Renderer rend = piece.transform.GetComponent<Renderer> ();
        
		SetMaterial2Transparent(rend.material);
		Color startColor = rend.material.GetColor ("_Color");
		Color endColor = new Color (startColor.r, startColor.g, startColor.b, 0);

		float t = 0.0f;

		while (t < fadeTime)
        {
			t += Time.deltaTime;
			Color lerpedColor = Color.Lerp(startColor, endColor, t/fadeTime);
			rend.material.SetColor ("_Color", lerpedColor);
			rend.material.SetColor("_EmissionColor", lerpedColor);
			
			yield return null;
		}

		Destroy (piece);
		//Destroy(this);
	}

    public void Awake()
    {
		prefab = Resources.Load<Object>("Prefabs/ExplosionCube");
		debrisColor = Resources.Load("Materials/COLORMAT0", typeof(Material)) as Material;
	}

    public ExplodingCube() {
		
		this.force = 10f;
		this.radius = 10f;
		this.upward = 10f;
	}

    public void setMaterial(Material material)
	{
		this.material = material;
	}

	public void setPosition(Vector3 position)
	{
		this.position = position;
	}

	private GameObject createPiece() {

		
		GameObject piece = Instantiate(prefab, new Vector3(position.x, 4f, position.z-1f), Quaternion.identity) as GameObject;
		piece.transform.hideFlags = HideFlags.HideInHierarchy;
		Renderer rend = piece.GetComponent<Renderer> ();
		BoxCollider col = piece.GetComponent<BoxCollider>();
		StartCoroutine(turnOffColider(col, 0.1f));
		rend.material = debrisColor;
		Rigidbody rb = piece.transform.GetComponent<Rigidbody>();
		rb.AddExplosionForce(force, piece.transform.position, radius, upward);

      
		return piece;
	}

	public void explode() {

		for (int i = 0; i< numberOfPieces; i++)
        {
			GameObject piece = createPiece();
			StartCoroutine(fadeToInvisibilityAndDestroy(piece, 2f));	
		}
	}
}
