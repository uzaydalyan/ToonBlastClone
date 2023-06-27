using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class ItemFactory : MonoBehaviour
    {
    
        [SerializeField] Sprite[] _cubeSprites;
        [SerializeField] private GameObject _cube;
        [SerializeField] Transform _gridParent;
        [SerializeField] GameObject _rocketHorizontal;
        [SerializeField] GameObject _rocketVertical;
        
        [SerializeField] GameObject _balloon;
        [SerializeField] private GameObject _duck;

        [SerializeField] sealed class itemDictionary : SerializedDictionary<ItemType, GameObject>{};
        private int _rows;
        private int _columns;

        private int sortingOrder = 1;
        
        public void CreateItems(int width, int height, string[] input)
        {
            _rows = height;
            _columns = width;
            float extraHeight = 0.00f;
            int i = 0;
            for (int x = 0; x < GameManager.Instance.Height; x++)
            {
                for (int y = 0; y < GameManager.Instance.Width; y++)
                {
                    CreateItem(x, y, input[i], extraHeight);
                    i++;
                }
            }
        }

        public void CreateRandomCube(int x, int y, float extraHeight)
        {
            int index = Random.Range(0, _cubeSprites.Length);
            GameObject newBlock = InstantiateItem(_cube, x, y, extraHeight);
            SpriteRenderer spriteRenderer = newBlock.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _cubeSprites[index];
            newBlock.GetComponent<Item>().type = (ItemType)Enum.Parse(typeof(ItemType), GetCubeSpriteNameByIndex(index));
            newBlock.tag = GetCubeSpriteNameByIndex(index);
        }

        public void CreateItem(int x, int y, string elementName, float extraHeight)
        {
            GameObject element = GetElementByName(elementName);
            if (element == null)
            {
                CreateRandomCube(x, y, 0);
                return;
            }
            InstantiateItem(element, x, y, extraHeight);
        }

        private GameObject GetElementByName(string elementName)
        {
            GameObject element = null;

            if (elementName.Length == 1)
            {
                element = _cube;
                _cube.GetComponent<SpriteRenderer>().sprite = _cubeSprites[GetCubeSpriteIndexByName(elementName)];
                _cube.tag = elementName;
            }
            else
            {
                switch (elementName)
                {
                    case "duck":
                        element = _duck;
                        break;
                    case "balloon":
                        element = _balloon;
                        break;
                }
            }

            if (element != null)
            {
                element.GetComponent<Item>().type = (ItemType)Enum.Parse(typeof(ItemType), elementName);
            }
            return element;
        }

        private int GetCubeSpriteIndexByName(string elementName)
        {
            int index = 0;
            switch (elementName)
            {
                case "y":
                    index = 0;
                    break;
                case "r":
                    index = 1;
                    break;
                case "b":
                    index = 2;
                    break;
                case "g":
                    index = 3;
                    break;
                case "p":
                    index = 4;
                    break;
            }

            return index;
        }
        
        private string GetCubeSpriteNameByIndex(int index)
        {
            string name = "y";
            switch (index)
            {
                case 0:
                    name = "y";
                    break;
                case 1:
                    name = "r";
                    break;
                case 2:
                    name = "b";
                    break;
                case  3:
                    name = "g";
                    break;
                case 4:
                    name = "p";
                    break;
            }

            return name;
        }

        public void CreateRocketHorizontal(int x, int y)
        {
            InstantiateItem(_rocketHorizontal, x, y, 0f);
        }

        public void CreateRocketVertical(int x, int y)
        {
            InstantiateItem(_rocketVertical, x, y, 0f);
        }

        public GameObject InstantiateItem(GameObject itemPrefab, int x, int y, float extraHeight)
        {
            GameObject newItem = Instantiate(itemPrefab, new Vector2((y - (float)(_columns - 1) / 2) , x - (float)(_rows - 1) / 2 + extraHeight), Quaternion.identity, _gridParent);
            newItem.GetComponent<SpriteRenderer>().sortingOrder = x+1;
            if (sortingOrder <= _rows * _columns)
            {
                GameManager.Instance.ItemLocations[x, y] = newItem.transform.position;
            }
            sortingOrder++;
            Item newBlockScript = newItem.GetComponent<Item>();
            newBlockScript.InitItem(x, y);
            return newItem;
        }
        
    }
}