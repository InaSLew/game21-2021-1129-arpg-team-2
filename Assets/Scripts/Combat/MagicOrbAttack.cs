using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MagicOrbAttack : MonoBehaviour {
    [SerializeField] private GameObject magicOrbPrefab;
    [SerializeField] private GameObject orbSpawnPosition;
    [SerializeField] private GameObject player;
    [SerializeField] private float orbLerpTime = 0.4f;
    private GameObject createdOrb;


    private void CreateOrb()
    {
         createdOrb = Instantiate(magicOrbPrefab, orbSpawnPosition.transform.position, quaternion.identity, orbSpawnPosition.transform);
         createdOrb.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
    }

    private void LaunchOrb()
    {
        StartCoroutine(MoveOrbToTarget());
    }
    
    //Later add new script to female enemy and call following method with target.transform instead of player referenced.
    private IEnumerator MoveOrbToTarget()
    {
        float progress = 0;
        while (progress < 1 && createdOrb != null) {
            progress += Time.deltaTime / orbLerpTime;
            createdOrb.transform.position =
                Vector3.Lerp(orbSpawnPosition.transform.position, player.transform.position, progress);
            yield return 0;
        }
    }
}
