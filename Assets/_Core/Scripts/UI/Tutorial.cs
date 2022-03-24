using DG.Tweening;
using PokeGo.World;
using UnityEngine;
using Zenject;

namespace PokeGo.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Tutorial : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private void Awake()
        {

        }

        [Inject]
        private void Constructor(PokeSpawner spawner)
        {
            spawner.PokeSpawned += OnPokeSpawnedHander;
        }

        private void OnPokeSpawnedHander()
        {
            _canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutCirc);
        }
    }
}