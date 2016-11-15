using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG {

    public class PlayerController : MonoBehaviour {
        

        private void Update()
        {
            UpdateLookAt();
            UpdateMovement();
            UpdateAttack();
        }

        private void UpdateLookAt()
        {
            // resolve look direction
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // create a ground plane on the character's height
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            float hitDistance;

            if(groundPlane.Raycast(ray, out hitDistance)) {
                transform.LookAt(ray.GetPoint(hitDistance));
            }
        }

        private void UpdateMovement()
        {
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");


        }

        private void UpdateAttack()
        {
        }

    }

}