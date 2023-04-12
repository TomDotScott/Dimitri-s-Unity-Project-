using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlippingPlatforms : MonoBehaviour
{
    public GameObject upSpikes;
    public GameObject downSpikes;

    [SerializeField] private float flipTimer;
    private bool isFlipping = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FlipPlatform()
    {
        yield return new WaitForSeconds(flipTimer);
        if (upSpikes.activeSelf == true)
        {
            upSpikes.SetActive(false);
            downSpikes.SetActive(true);
        }
        else
        {
            upSpikes.SetActive(true);
            downSpikes.SetActive(false);
        }

        if (isFlipping == true)
        {
            FlipBack();
        }
    }

    private void FlipBack()
    {
        // TODO Change "isFlipping" + call FlipPlatform
        isFlipping = false;
        StartCoroutine(FlipPlatform());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y > transform.position.y)
        {
            if (collision.gameObject.CompareTag(GameConstants.PLAYER_TAG))
            {
                if (!isFlipping)
                {
                    isFlipping = true;
                    StartCoroutine(FlipPlatform());
                }
            }
        }
    }
}
