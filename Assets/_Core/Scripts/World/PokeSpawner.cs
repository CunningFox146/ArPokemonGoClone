using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Zenject;
using Random = UnityEngine.Random;

namespace PokeGo.World
{
    public class PokeSpawner : MonoBehaviour
    {
        public event Action PokeSpawned;

        [SerializeField] private List<GameObject> _pokesPrefabs;
        private ARPlaneManager _planeManager;
        private Transform _camera;

        private async void Start()
        {
            _camera = Camera.main.transform;
            await SpawnPoke();
        }

        [Inject]
        private void Constructor(ARPlaneManager planeManager)
        {
            _planeManager = planeManager;
        }

        private async Task SpawnPoke()
        {
            var spawnPoint = await FindSpawnPoint();
            var prefab = _pokesPrefabs[Random.Range(0, _pokesPrefabs.Count)];
            var camPos = _camera.position;

            var mob = Instantiate(prefab, spawnPoint, Quaternion.identity);
            mob.transform.LookAt(new Vector3(camPos.x, spawnPoint.y, camPos.z));

            PokeSpawned?.Invoke();
        }

        private async Task<Vector3> FindSpawnPoint()
        {
            while (_planeManager.trackables.count == 0) await Task.Yield();

            Transform targetPlane = null;
            foreach (var plane in _planeManager.trackables)
            {
                if (plane.alignment != PlaneAlignment.HorizontalUp) await Task.Yield();
                targetPlane = plane.transform;
                _planeManager.requestedDetectionMode = PlaneDetectionMode.None;
            
            }

            return targetPlane?.position ?? Vector3.zero;
        }
    }
}