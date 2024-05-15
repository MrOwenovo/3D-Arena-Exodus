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
        // 加载新模型资源
        Object newModelObject = Resources.Load(newModelPath);
        Mesh newMesh = ((GameObject)newModelObject).GetComponentInChildren<MeshFilter>().sharedMesh;

        // 获取 bossPrefab 中需要替换的 MeshFilter 组件
        MeshFilter meshFilter = bossPrefab.GetComponentInChildren<MeshFilter>();

        // 替换模型网格
        meshFilter.sharedMesh = newMesh;
    }
}