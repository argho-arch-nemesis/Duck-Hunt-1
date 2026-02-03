using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    private float inputHorizontal;
    private float inputVertical;

    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string shootButton = "Shoot";

    public Button reset;
    public Button shoot;

    private Vector3 zero = new Vector3(0, 0, 0);
    private Vector3 one = new Vector3(1, 1, 1);

    private float visualizationDistance = 10f;
    private Camera cam;

    public Shooter shooter;
    
    [SerializeField]
    private ParticleSystem bulletImpactVFX;

    public SpriteRenderer crosshairRenderer;

    //Camera bound area
    private float minX, maxX, minY, maxY;

    void Awake()
    {
        reset.onClick.AddListener(ResetCrosshair);
        shoot.onClick.AddListener(ShootBullet);
        crosshairRenderer = gameObject.GetComponent<SpriteRenderer>();
        cam = Camera.main;

        // Calculate the four corners of the viewport in world space
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0.4f, visualizationDistance));
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, visualizationDistance));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, visualizationDistance));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0.4f, visualizationDistance));

        // Add padding if your object has a renderer/collider size
        minX = bottomLeft.x;
        maxX = topRight.x;
        minY = bottomLeft.y;
        maxY = topRight.y;
        
    }

    void Update()
    {
        inputHorizontal = SimpleInput.GetAxis(horizontalAxis);
        inputVertical = SimpleInput.GetAxis(verticalAxis);

        this.transform.position += new Vector3(inputHorizontal, inputVertical) / 3;
        CheckScreenBounds();
        
        // for debug
        if (Input.GetKeyDown(KeyCode.Space))ShootBullet();
    }

    void CheckScreenBounds()
    {
        Vector3 pos = transform.position;
        // Clamp position within calculated bounds [2]

        if (pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY)
        {
            crosshairRenderer.DOColor(Color.red, 0.2f).OnComplete(()=>crosshairRenderer.color = Color.white);
            this.transform.DOPunchScale(one*.1f, 0.2f).OnComplete(()=> this.transform.localScale=one);
        }
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void ResetCrosshair()
    {
        this.transform.position = zero;
    }

    void ShootBullet()
    {
        var pos = new Vector3(this.transform.position.x,this.transform.position.y,cam.transform.position.z);
        shooter.shoot(pos);
        bulletImpactVFX.transform.position = this.transform.position;
        bulletImpactVFX.Play();
    }
}