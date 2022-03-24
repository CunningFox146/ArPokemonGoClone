using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace PokeGo.Ball
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private static int MobLayer;

        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _launchFactor;
        [SerializeField] private float _resetTime = 2f;

        private Vector3 _startPos;
        private Quaternion _startRot;
        private Transform _startParent;
        private float _clickStart;
        private Vector2 _clickPos;
        private Rigidbody _rb;
        private Sequence _catchSequence;


        private Coroutine _resetCoroutine;

        private void Awake()
        {
            MobLayer = LayerMask.NameToLayer("Mob");

            _startParent = transform.parent;
            _startPos = transform.localPosition;
            _startRot = transform.localRotation;
            _rb = GetComponent<Rigidbody>();
        }

        private async void OnCollisionEnter(Collision collision)
        {
            if (_catchSequence != null) return;

            if (collision.gameObject.layer == MobLayer)
            {
                await Catch(collision);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_rb.isKinematic) return;

            _clickStart = Time.time;
            _clickPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.parent = null;

            var direction = eventData.position - _clickPos;
            float timeFactor = Mathf.Max(1f - (Time.time - _clickStart), 0.1f);
            float forwardForce = timeFactor * _forwardSpeed;

            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.AddForce(
                (direction.x * _launchFactor * _startParent.right) + 
                (direction.y * _launchFactor * _startParent.transform.up) + 
                (_startParent.forward * forwardForce),
                ForceMode.Impulse);

            _rb.AddTorque(transform.right * 0.1f * timeFactor);

            StartReset(_resetTime);
        }

        private async Task Catch(Collision collision)
        {
            StopReset();
            ResetPhysics();

            _catchSequence = DOTween.Sequence()
            .Append(transform.DOMove(Vector3.up * 0.5f, 0.5f)
                .SetRelative(true)
                .OnUpdate(() => transform.LookAt(collision.transform.position))
                .OnComplete(() =>
                {
                    _rb.isKinematic = false;
                    _rb.useGravity = true;
                }))
            .Append(transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.5f))
            .Join(collision.transform.parent.DOScale(Vector3.zero, 0.5f));

            await Task.Delay(3000);

            SceneManager.LoadScene(0);
        }


        private void StopReset()
        {
            if (_resetCoroutine != null)
            {
                StopCoroutine(_resetCoroutine);
                _resetCoroutine = null;
            }
        }

        private void StartReset(float delay)
        {
            StopReset();
            _resetCoroutine = StartCoroutine(ResetCorotutine(delay));
        }

        private IEnumerator ResetCorotutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            ResetBall();
        }

        private void ResetBall()
        {
            transform.parent = _startParent;
            ResetPhysics();

            transform.localPosition = _startPos;
            transform.localRotation = _startRot;
        }

        private void ResetPhysics()
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}