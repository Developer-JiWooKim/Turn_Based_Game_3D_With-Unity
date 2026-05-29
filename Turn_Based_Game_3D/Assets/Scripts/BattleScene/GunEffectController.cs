using UnityEngine;

public class GunEffectController : MonoBehaviour
{
    [SerializeField] private float _bulletTrailDuration = 0.05f;

    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private LineRenderer   _bulletTrail;
    [SerializeField] private Transform      _muzzlePoint;

    public void Initialize()
    {
        if (_bulletTrail != null)
        {
            _bulletTrail.enabled = false;
        }
            
    }

    public void PlayFireEffect(Vector3 targetPosition)
    {
        _muzzleFlash?.Play();

        if (_bulletTrail != null && _muzzlePoint != null)
        {
            _ = PlayBulletTrailAsync(targetPosition);
        }
    }

    private async Awaitable PlayBulletTrailAsync(Vector3 targetPosition)
    {
        _bulletTrail.SetPosition(0, _muzzlePoint.position);
        _bulletTrail.SetPosition(1, targetPosition);
        _bulletTrail.enabled = true;

        await Awaitable.WaitForSecondsAsync(_bulletTrailDuration, destroyCancellationToken);

        _bulletTrail.enabled = false;
    }
}