using UnityEngine;

public class BossModelReplacer : MonoBehaviour
{
    public GameObject bossPrefab;
    public string newModelPath = "Assets/NewBossModel.fbx";

    void Start()
    {
        ReplaceModel();
    }

    void ReplaceModel()
    {
        Object newModelObject = Resources.Load(newModelPath);
        Mesh newMesh = ((GameObject)newModelObject).GetComponentInChildren<MeshFilter>().sharedMesh;

        MeshFilter meshFilter = bossPrefab.GetComponentInChildren<MeshFilter>();

        meshFilter.sharedMesh = newMesh;
    }
}