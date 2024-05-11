using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.curStatus== Status.Game)
        {
            Vector3 playerPos = GameManager.instance.Player.transform.position;
            transform.rotation = Quaternion.Euler(65,0,0);
            transform.position = new Vector3(playerPos.x,12,playerPos.z-5);
        }
        
    }
}
