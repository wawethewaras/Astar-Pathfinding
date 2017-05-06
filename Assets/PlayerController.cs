using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rigidbody;
    public float moveSpeed;


	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void Update () {
        Vector2 movement_Vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        movement_Vector = movement_Vector.normalized * moveSpeed;
        rigidbody.velocity = movement_Vector;
    }
}
