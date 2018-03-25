using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerConnection : NetworkBehaviour
{
    public int UnitsCount = 6;
    public GameObject UnitPrefab;
    public Vector3 CameraOffset = new Vector3(0f, 25f, 15f);

    private void Start()
    {
        if (isLocalPlayer)
        {
            CmdSpawnUnits(UnitsCount, transform.position, transform.rotation);

            Camera.main.transform.position = transform.position + transform.forward * CameraOffset.z;
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, CameraOffset.y, Camera.main.transform.position.z);

            Camera.main.transform.rotation = Quaternion.Euler(45f, transform.rotation.eulerAngles.y, 0f);
        }
    }

    [Command]
    private void CmdSpawnUnits(int unitsCount, Vector3 center, Quaternion rotation)
    {
        for (int i = 0; i < unitsCount; i++)
        {
            Vector3 position = center + new Vector3(
                Random.Range(-8, 8),
                0f,
                Random.Range(-8, 8));

            GameObject go = Instantiate(UnitPrefab, position, rotation);
            NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        }
    }
}
