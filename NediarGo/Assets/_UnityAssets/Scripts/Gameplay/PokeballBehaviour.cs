using UnityEngine;

/// <summary>
/// Behaviour of the ball throwing that is used to catch the pokemons
/// </summary>
public class PokeballBehaviour : MonoBehaviour 
{
	Vector2 startPos, endPos, direction;
	float touchTimeStart, touchTimeFinish, timeInterval; // to calculate swipe time to sontrol throw force in Z direction

	[Header("Pokeball preferences")]
	[SerializeField] float throwForceInXandY = 0.7f; // to control throw force in X and Y directions
	[SerializeField] float throwForceInZ = 20f; // to control throw force in Z direction
	[SerializeField] Rigidbody rb;

	[Header("Manager setup")]
	[SerializeField] GameManager manager;

	

	private Vector3 initialPosition;

	bool canBeThrown = true;
	bool pokeballUsed = false;

	void Start()
	{
		initialPosition = transform.position;
	}

	// Waits for player input to select a direction to throw the pokeball
	void Update () 
	{
		if(canBeThrown)
		{
			// if you touch the screen
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {

				// getting touch position and marking time when you touch the screen
				touchTimeStart = Time.time;
				startPos = Input.GetTouch (0).position;
			}

			// if you release your finger
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {

				// marking time when you release it
				touchTimeFinish = Time.time;

				// calculate swipe time interval 
				timeInterval = touchTimeFinish - touchTimeStart;

				// getting release finger position
				endPos = Input.GetTouch (0).position;

				// calculating swipe direction in 2D space
				direction = startPos - endPos;

				// add force to balls rigidbody in 3D space depending on swipe time, direction and throw forces
				rb.isKinematic = false;
				rb.AddForce (- direction.x * throwForceInXandY, - direction.y * throwForceInXandY, throwForceInZ / timeInterval);

				// Destroy ball in 4 seconds
				canBeThrown = false;
				Invoke("Reset",4f);
			}
		}
	}

	void Reset()
    {
		CancelInvoke();
		
		pokeballUsed = false;
		canBeThrown = true;
		transform.position = initialPosition;
		rb.isKinematic = true;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.rotation = Quaternion.Euler (0f, 200f, 0f);
	}

    /// <summary>
    /// When the ball collides with a pokemon
    /// </summary>
    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Pokemon") && !pokeballUsed)
        {
			pokeballUsed = true;
            PokemonData pokeData = other.GetComponent<PokemonData>();
            manager.PokeballImpactedPokemon(pokeData);
        }
    }
    
}
