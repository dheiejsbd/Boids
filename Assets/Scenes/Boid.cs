using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Boid : MonoBehaviour
{
    Boids Boids;
    float speed;
    Vector3 RandomVec;
    public void OnInitialized(Boids _boids)
    {
        Boids = _boids;
        speed = Random.Range(Boids.speed.x, Boids.speed.y);
        StartCoroutine(RandomVecCor());
    }
    void Update()
    {
        Transform[] targets = Change(Physics.OverlapSphere(transform.position, Boids.SeparationRadius), (Collider coll) => coll.transform);
        
        
        if(targets.Length > Boids.maxNeighbourCount)
        {
            var t = new List<Transform>();
            int count = 0;

            for (int i = 0; i < targets.Length; i++)
            {
                 t.Add(targets[i]);
            }
            targets = t.ToArray();
        }

        var SeparationVec = Separation(targets).normalized * Boids.SeparationWight;
        Debug.DrawRay(transform.position, Separation(targets).normalized, Color.red);



        

        targets = Change(Physics.OverlapSphere(transform.position, Boids.NeighbourRadius), (Collider coll) => coll.transform);


        var  AlignmentVec = Alignment(targets).normalized * Boids.AlignmentWight;
        Debug.DrawRay(transform.position, Alignment(targets).normalized, Color.blue);
        
        var   CohesionVec = Cohesion(targets).normalized * Boids.CohesionWight;
        Debug.DrawRay(transform.position, Cohesion(targets).normalized, Color.green);
        
        var boundsVec = Bounds() * Boids.BoundsWight;
        var obstacle = Obstacle().normalized * Boids.obstacleWight;

        var LookVec = SeparationVec + AlignmentVec + CohesionVec + boundsVec + obstacle + RandomVec ;

        if (!Boids.Space3D) LookVec.y = 0;

        LookVec = Vector3.Lerp(transform.forward, LookVec, Time.deltaTime * Boids.RotateSpeed);
        Quaternion targetQuaternion = Quaternion.LookRotation(LookVec);
        transform.rotation = targetQuaternion;
    }

    IEnumerator RandomVecCor()
    {
        while (true)
        {
            speed = Random.Range(Boids.speed.x, Boids.speed.y);
            if(Boids.Space3D)
            {
                RandomVec = Random.insideUnitSphere;
            }
            else
            {
                var v = Random.insideUnitCircle;
                RandomVec = new Vector3(v.x, 0, v.y);
            }
            yield return new WaitForSeconds(Random.Range(0f, 3f));
        }
    }
    private void LateUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    //무리를 피하는 방향
    Vector3 Separation(Transform[] _targets)
    {
        Vector3[] targets = Change(_targets, (Transform target) => transform.position - target.position);
        return Average(targets, (Vector3 vec) => vec.normalized * vec.magnitude / Boids.SeparationRadius);
    }
    //무리의 평균 이동방향
    Vector3 Alignment(Transform[] _targets)
    {
        Vector3[] targets = Change(_targets, (Transform target) => target.forward);
        return Average(targets);
    }
    //무리의 중심 방향
    Vector3 Cohesion(Transform[] _targets)
    {
        Vector3[] targets = Change(_targets, (Transform target) => target.position);
        return Average(targets) - transform.position;
    }

    //최대 거리
    Vector3 Bounds()
    {
        var dist = Boids.transform.position - transform.position;
        return dist / Boids.MaxDist;
    }

    // 장애물
    Vector3 Obstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Boids.obstacleDist, Boids.obstacleLayer)) return hit.normal;
        return Vector3.zero;
    }

    Result[] Change<Target, Result>(Target[] target, Func<Target, Result> func)
    {
        Result[] results = new Result[target.Length];
        for (int i = 0; i < target.Length; i++)
        {
            results[i] = func(target[i]);
        }
        return results;
    }

    Vector3 Add(Vector3[] targets)
    {
        Vector3 delta = Vector3.zero;
        for (int i = 0; i < targets.Length; i++)
        {
            delta += targets[i];
        }
        return delta;
    }
    Vector3 Add(Vector3[] targets, Func<Vector3, Vector3> func)
    {
        Vector3 delta = Vector3.zero;
        for (int i = 0; i < targets.Length; i++)
        {
            delta += func(targets[i]);
        }
        return delta;
    }

    Vector3 Average(Vector3[] targets)
    {
        return Add(targets) / targets.Length;
    }
    Vector3 Average(Vector3[] targets, Func<Vector3, Vector3> func)
    {
        return Add(targets, func) / targets.Length;
    }
}
