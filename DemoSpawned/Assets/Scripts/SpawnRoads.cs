using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoads : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private int _minCapacity;
    [SerializeField] private int _maxCapacity;
    [SerializeField] private bool _autoExpand;
    private GameObject currentRoad;

    private List<GameObject> _pool;
    private float spawnOffset = 9f; // Расстояние между спаунами
    private Vector3 roadPosition; // Текущая позиция дороги

    private void Start()
    {
        CreatePool();
        InvokeRepeating("SpawnRoad", 2f, 2f); // Периодический спаун дорог с интервалом в 2 секунды

        SpawnRoad();
    }

    private void OnValidate()
    {
        if (_autoExpand)
        {
            _maxCapacity = Int32.MaxValue;
        }
    }

    private void CreatePool()
    {
        _pool = new List<GameObject>(_minCapacity);

        for (int i = 0; i < _minCapacity; i++)
        {
            CreateElement();
        }
    }

    private GameObject CreateElement()
    {
        var createObject = Instantiate(roadPrefab, _container);
        createObject.SetActive(false);

        _pool.Add(createObject);
        return createObject;
    }

    public bool TryGetElement(out GameObject element)
    {
        foreach (var item in _pool)
        {
            if (!item.activeInHierarchy)
            {
                element = item;
                item.SetActive(true);
                return true;
            }
        }
        element = null;
        return false;
    }

    public GameObject GetFreeElement(Vector3 position)
    {
        var element = GetFreeElement();
        element.transform.position = position;
        return element;
    }

    public GameObject GetFreeElement(Vector3 position, Quaternion rotation)
    {
        var element = GetFreeElement(position);
        element.transform.rotation = rotation;
        return element;
    }

    public GameObject GetFreeElement()
    {
        if (TryGetElement(out var element))
        {
            return element;
        }
        if (_autoExpand && _pool.Count < _maxCapacity)
        {
            return CreateElement();
        }
        throw new Exception("Pool is over or auto-expand is disabled.");
    }

    private void SpawnRoad()
    {
        // Проверяем видимость объектов и деактивируем их, если они больше не видны
        foreach (var item in _pool)
        {
            if (item.activeInHierarchy && !IsVisible(item))
            {
                item.SetActive(false);
            }
        }

        var roadRotation = Quaternion.identity;
        var road = GetFreeElement(roadPosition, roadRotation);
        // Дополнительная настройка или модификация для созданной дороги

        // Обновляем позицию для следующего спавна
        roadPosition.x += spawnOffset;
    }

    private bool IsVisible(GameObject obj)
    {
        // Проверяем, виден ли объект на экране
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.isVisible;
        }
        else
        {
            // Если у объекта нет компонента Renderer, считаем его видимым
            return true;
        }
    }
}
