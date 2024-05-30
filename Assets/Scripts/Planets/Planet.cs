using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public TileManager currentMine;

    public List<Transform> enemySpawnPoints = new List<Transform>();

    public Gravity gravity;
    public float radius;

    private void OnEnable()
    {
        PlanetManager.RegisterPlanet(this);
        EventManager.AddListener(EventManager.GameEvent.TICDocked, OnTICDocked);
        EventManager.AddListener(EventManager.GameEvent.TICUndocked, OnTICUndocked);
        //Debug.Log("Planet Size: " + GetAtmosphereSize());
    }

    private void OnDisable()
    {
        PlanetManager.UnRegisterPlanet(this);
        //EventManager.RemoveListener(EventManager.GameEvent.TICDocked, OnTICDocked);
        //EventManager.RemoveListener(EventManager.GameEvent.TICUndocked, OnTICUndocked);
        EventManager.RemoveMyListeners(this);
    }

    private void OnTICDocked(EventData data)
    {
        currentMine = data.GetMineEntrance("MineEntrance").mine;
    }

    private void OnTICUndocked(EventData data)
    {
        currentMine = null;
    }

    public float GetAtmosphereSize()
    {
        float size = transform.localScale.x * ((CircleCollider2D)GetComponentInChildren<Gravity>().gravityCollider).radius;
        //Debug.Log(gameObject.name + " 'Global Size' is at: " + size);
        this.radius = size;
        return size;
    }
}
