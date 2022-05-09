using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boids : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] int Count;
    public bool Space3D;
    [SerializeField] float SpawnRange;
    public float MaxDist = 100;

    public Vector2 speed;
    public float RotateSpeed = 1f;
    public float fov;
    public float NeighbourRadius = 10;
    public float SeparationRadius = 5;
    public int maxNeighbourCount = 50;
    public float obstacleDist;
    public LayerMask obstacleLayer;

    [Header("피하는 방향")]
    [Range(0,10)] public float SeparationWight = 1;
    [Header("평균 방향")]
    [Range(0,10)] public float AlignmentWight = 1;
    [Header("중심 방향")]
    [Range(0,10)] public float CohesionWight = 1;
    [Range(0,10)]public float BoundsWight = 1;
    [Range(0,10)] public float obstacleWight = 10;

    private void Start()
    {

        for (int i = 0; i < Count; i++)
        {
            GameObject instance = Instantiate(obj, transform);
            instance.GetComponent<Boid>().OnInitialized(this);

            if(Space3D)
            {
                instance.transform.position += Random.insideUnitSphere * SpawnRange;
                instance.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            }
            else
            {
                var v = Random.insideUnitCircle * SpawnRange;
                instance.transform.position += new Vector3(v.x, 0, v.y);
                instance.transform.rotation = Quaternion.Euler(new Vector3(0,Random.Range(0, 360),0));
            }
        
        }
    }

}
