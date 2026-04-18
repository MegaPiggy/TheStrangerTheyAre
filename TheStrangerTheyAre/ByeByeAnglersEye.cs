using UnityEngine;

namespace TheStrangerTheyAre
{
    public class ByeByeAnglersEye : MonoBehaviour
    {
        private GameObject anglersEye; // creates variable to store angler's eye

        public void Awake()
        {
            GetComponent<SphereCollider>().radius = 210;
            anglersEye = TheStrangerTheyAre.NewHorizonsAPI.GetPlanet("Angler's Eye").transform.Find("Sector").gameObject; // gets angler's eye
        }

        public virtual void OnTriggerEnter(Collider hitCollider)
        {
            // checks if player collides with the trigger volume
            if (hitCollider.CompareTag("PlayerDetector") && enabled)
            {
                anglersEye.SetActive(false); // deactivates object when inside the trigger
            }
        }

        public virtual void OnTriggerExit(Collider hitCollider)
        {
            // checks if player collides with the trigger volume
            if (hitCollider.CompareTag("PlayerDetector") && enabled)
            {
                anglersEye.SetActive(true); // activates object when outside the trigger
            }
        }
    }
}