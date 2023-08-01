using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    public GameManager gm;
    [SerializeField] private GameObject restartUI;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Point")
        {
            Destroy(other.gameObject);
            gm.points += 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "BoundingSpace")
        {
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        restartUI.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
