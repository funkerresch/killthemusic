using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private RenderLayer instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = RenderLayer.GetInstance();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CollectableCube")
        {
            Destroy(collision.gameObject);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CollectableCube")
        {
            Renderer rend = other.gameObject.GetComponent<Renderer>();
            ExplodingCube cube = instance.layer.parent.AddComponent<ExplodingCube>();
            cube.setMaterial(rend.material);
            cube.setPosition(other.gameObject.transform.position);
            cube.explode();
            Destroy(other.gameObject);
        }

    }
}
