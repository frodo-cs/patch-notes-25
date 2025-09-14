using Player.UI;
using System;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public int money = 0;
    public static Action<int> AddMoney;

    public delegate int getCurrency();
    public static getCurrency GetMoney;

    private void Awake() {
        AddMoney = AddMoneyEvent;
        GetMoney = GetMoneyEvent;
    }

    private void Start() {
        OnLoad();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddMoneyEvent(10);
        } else if(Input.GetKeyDown(KeyCode.S)) {
            AddMoneyEvent(-10);
        }
    }

    public void AddMoneyEvent(int delta = 0) {
        money += delta;
        Save();

        UserInterface.OnWalletUpdated?.Invoke();
    }

    public int GetMoneyEvent() { return money; }

    public void Save() {
        PersistentData.SaveCurrency?.Invoke(money);
    }

    public void OnLoad() {
        var r = PersistentData.GetCurrency?.Invoke();
        money = r == null ? 0 : r.Value;

        UserInterface.OnWalletUpdated?.Invoke();
    }
}
