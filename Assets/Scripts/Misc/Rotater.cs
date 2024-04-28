using UnityEngine;

class Rotater: MonoBehaviour
{
    public void Update()
    {
        _angle = transform.rotation.eulerAngles.x;
        if (_angle <= rotationAngle)
            _multiplier *= -1;

        transform.Rotate(transform.right, rotationSpeed * _multiplier * Time.deltaTime);
    }

    public float rotationSpeed;
    public float rotationAngle;
    public float _angle;
    public float _multiplier = 1;
}
