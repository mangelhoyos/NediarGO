using UnityEngine;

/// <summary>
/// Behaviour of the ball throwing that is used to catch the pokemons
/// </summary>
public class PokeballBehaviour : MonoBehaviour 
{

    [Header("Pokeball settings")]
	[SerializeField] private float throwSpeed = 35f;
    [SerializeField] Camera pokeballCamera;
    [SerializeField] Rigidbody rigidbodyComp;

    [SerializeField] GameManager manager;
	private float speed;
	private float lastTouchX, lastTouchY;

	private bool thrown, holding;
	private Vector3 newPosition;

	void Start() 
    {
		Reset ();
	}

    // Check for input and set touch positions for throw calculations
	void FixedUpdate() 
    {
		if (holding)
			OnTouch ();

		if (thrown)
			return;

		if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) 
        { 
			Ray ray = pokeballCamera.ScreenPointToRay (Input.GetTouch (0).position);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100f)) 
            {
				if (hit.transform == transform) 
                {
					holding = true;
					transform.SetParent (null);
				}
			}
		}

		if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended) 
        { 
			if (lastTouchY < Input.GetTouch (0).position.y) 
            {
				ThrowBall (Input.GetTouch (0).position);
			}
		}

		if(Input.touchCount == 1) 
        {
			lastTouchX = Input.GetTouch (0).position.x;
			lastTouchY = Input.GetTouch (0).position.y;
		}
	}

    /// <summary>
    /// Resets the ball to it's initial position and enters a standby state
    /// </summary>
	void Reset()
    {
		CancelInvoke ();
		transform.position = pokeballCamera.ViewportToWorldPoint (new Vector3 (0.5f, 0.1f, pokeballCamera.nearClipPlane * 7.5f));
		newPosition = transform.position;
		thrown = holding = false;

		rigidbodyComp.useGravity = false;
		rigidbodyComp.velocity = Vector3.zero;
		rigidbodyComp.angularVelocity = Vector3.zero;
		transform.rotation = Quaternion.Euler (0f, 200f, 0f);
		transform.SetParent (pokeballCamera.transform);
	}

	void OnTouch() 
    {
		Vector3 touchPos = Input.GetTouch (0).position;
		touchPos.z = pokeballCamera.nearClipPlane * 7.5f;

		newPosition = pokeballCamera.ScreenToWorldPoint (touchPos);

		transform.localPosition = Vector3.Lerp (transform.localPosition, newPosition, 50f * Time.deltaTime);
	}

    /// <summary>
    /// Throws the ball in a curve towards the slide direction
    /// </summary>
	void ThrowBall(Vector2 touchPos) 
    {
		rigidbodyComp.useGravity = true;

		float differenceY = (touchPos.y - lastTouchY) / Screen.height * 100;
		speed = throwSpeed * differenceY;

		float x = (touchPos.x / Screen.width) - (lastTouchX / Screen.width);
		x = Mathf.Abs (Input.GetTouch (0).position.x - lastTouchX) / Screen.width * 100 * x;

		Vector3 direction = new Vector3 (x, 0f, 1f);
		direction = pokeballCamera.transform.TransformDirection (direction);

		rigidbodyComp.AddForce((direction * speed / 2f) + (Vector3.up * speed));

		holding = false;
		thrown = true;

		Invoke ("Reset", 5.0f);
	}

    /// <summary>
    /// When the ball collides with a pokemon
    /// </summary>
    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Pokemon"))
        {
            PokemonData pokeData = other.GetComponent<PokemonData>();
            manager.PokeballImpactedPokemon(pokeData);
        }
    }
    
}
