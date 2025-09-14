using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public Dictionary<int, SceneSaver> savedScenes;
    public Dictionary<string, PropertyData> generalData;
    public int savedCurrency = 0;

    public static Action<string, PropertyData> Save;
    public static Action<int> SaveCurrency;

    //GeneralData
    public delegate PropertyData getBool(string objectName);
    public static getBool LoadGeneralData;
    public static Action<PropertyData> SaveGeneralData;

    public delegate int getCurrency();
    public static getCurrency GetCurrency;

    public delegate PropertyData getProperty(string objectName, string variableName);
    public static getProperty GetData;

    public static PersistentData instance;

    private void Awake()
    {
        if (instance == null)
        {
            Save = OnSave;
            GetData = OnGetData;

            GetCurrency = OnLoadCurrency;
            SaveCurrency = OnSaveCurrency;

            SaveGeneralData = OnSaveShop;
            LoadGeneralData = OnLoadShop;

            savedScenes = new();
            instance = this;
        }

    }

    #region ShopData

    void OnSaveShop(PropertyData t)
    {
        if (generalData == null)
            generalData = new();

        if (!generalData.ContainsKey(t.keyName))
            generalData.Add(t.keyName, t);
        else
            generalData[t.keyName] = t;
    }

    PropertyData OnLoadShop(string slot)
    {
        if (generalData == null)
            generalData = new();

        if (!generalData.ContainsKey(slot))
            return null;
        else
            return generalData[slot];
    }

    #endregion

    public void OnSaveCurrency(int money)
    {
        savedCurrency = money;
    }

    public int OnLoadCurrency()
    {
        return savedCurrency;
    }


    public void OnSave(string objectName, PropertyData data)
    {
        int scene = SceneManager.GetActiveScene().buildIndex;

        if (!savedScenes.ContainsKey(scene))
        {
            var saver = new SceneSaver
            {
                savedObjects = new()
            };

            savedScenes.Add(scene, saver);
            //Debug.Log("New scene saved!");
        }

        if (!savedScenes[scene].savedObjects.ContainsKey(objectName))
        {
            savedScenes[scene].savedObjects.Add(objectName, new ObjectData(objectName));
            //Debug.Log("New object saved!");
        }

        if (savedScenes[scene].savedObjects.TryGetValue(objectName, out ObjectData obj))
        {
            obj.Save(data);
            //Debug.Log("New property!");
        }
    }

    public PropertyData OnGetData(string objectName, string variableName)
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        if (!savedScenes.ContainsKey(scene))
            return null;
        //Debug.Log("Scene data recovered!");

        if (savedScenes[scene].savedObjects.TryGetValue(objectName, out ObjectData obj))
        {
            //Debug.Log("Obj data recovered!");

            if (obj.savedData.TryGetValue(variableName, out PropertyData v))
            {
                //Debug.Log("Property data recovered!");
                return v;
            }
        }

        return null;
    }

    public struct SceneSaver
    {
        public Dictionary<string, ObjectData> savedObjects;

        public void UpdateObj(ObjectData obj)
        {
            if (savedObjects == null)
                savedObjects = new Dictionary<string, ObjectData>();

            if (!savedObjects.ContainsKey(obj.objectName))
                savedObjects.Add(obj.objectName, obj);
            else
                savedObjects[obj.objectName] = obj;
        }
    }

    //Object
    public struct ObjectData
    {
        public string objectName;
        public Dictionary<string, PropertyData> savedData;

        public ObjectData(string _objectName)
        {
            objectName = _objectName;
            savedData = new();
        }

        public void Save(string _objectName, PropertyData data)
        {
            objectName = _objectName;
            if (savedData == null)
                savedData = new Dictionary<string, PropertyData>();

            if (!savedData.ContainsKey(data.keyName))
                savedData.Add(data.keyName, data);
            else
                savedData[data.keyName] = data;
        }

        public void Save(PropertyData data)
        {
            if (savedData == null)
                savedData = new Dictionary<string, PropertyData>();

            if (!savedData.ContainsKey(data.keyName))
                savedData.Add(data.keyName, data);
            else
                savedData[data.keyName] = data;
        }
    }

    //Propierties
    public class PropertyData
    {
        public string keyName;

        public PropertyData(string variableName)
        {
            keyName = variableName;
        }

        public virtual void setData(object obj) { }
        public virtual object getData() { return null; }
    }

    public class IntData : PropertyData
    {
        public int value;

        public IntData(string variableName, int v) : base(variableName) { value = v; }

        public override object getData() { return value; }
        public override void setData(object obj) { value = (int)obj; }
    }

    public class ObjectArrayData : PropertyData
    {
        public Player.Inventory.Object[] value;

        public ObjectArrayData(string variableName, Player.Inventory.Object[] v) : base(variableName) { value = v; }

        public override object getData() { return value; }
        public override void setData(object obj) { value = (Player.Inventory.Object[])obj; }
    }

    public class BoolData : PropertyData
    {
        public bool value;

        public BoolData(string variableName, bool v) : base(variableName) { value = v; }

        public override object getData() { return value; }
        public override void setData(object obj) { value = (bool)obj; }
    }

    public class StringData : PropertyData
    {
        public string value;

        public StringData(string variableName, string v) : base(variableName) { value = v; }

        public override object getData() { return value; }
        public override void setData(object obj) { value = (string)obj; }
    }
}
