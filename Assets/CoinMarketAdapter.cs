using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinMarketAdapter : MonoBehaviour
{
    public Camera myCam;
    public RectTransform prefarb;
    public Text countText;
    public RectTransform content;
    public Sprite[] ID;
    int i;
    bool firstLoad =true;

    private void Start()
    {
        InvokeRepeating("UpdateItems", 1f,30f);
    }
    private void Update()
    {
        if (firstLoad)
        {
            var color = myCam.backgroundColor;
            color.r += 1f * Time.deltaTime;
            color.r = Mathf.Clamp(color.r, 0, 255);
            color.g += 1f * Time.deltaTime;
            color.g = Mathf.Clamp(color.r, 0, 255);
            color.b += 1f * Time.deltaTime;
            color.b = Mathf.Clamp(color.r, 0, 255);
            myCam.backgroundColor = color;
        }
    }

    public void UpdateItems()
    {
        i = 0;
        int modelsCount = 1;
        string url = "http://194.67.214.125:6006?count=" + modelsCount;
        WWW www = new WWW(url);
        StartCoroutine(GetItems(www, results => OnReceivedModels(results)));
    }

    void OnReceivedModels(TestItemModel[] models)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var model in models)
        {
            var instance = GameObject.Instantiate(prefarb.gameObject) as GameObject;
            instance.transform.SetParent(content, false);
            InitializeItemView(instance, model);
        }
        if (firstLoad)
        {
            content.anchoredPosition = new Vector2(0f, -11886.5f);
            firstLoad = false;
        }

    }

    void InitializeItemView(GameObject viewGameObject, TestItemModel model)
    {
        TestItemView view = new TestItemView(viewGameObject.transform);
        view.name.text = model.name;
        view.symbol.text = model.symbol;
        view.price_usd.text = "$ " +  model.price_usd;
        view.market_cap_usd.text = "$ " + model.market_cap_usd;

        view.percent_change_1h.text = model.percent_change_1h + "% ";
        if (CheckRiseFall(model.percent_change_1h))
            view.percent_change_1h.color = Color.green;
        else
            view.percent_change_1h.color = Color.red;

        view.percent_change_24h.text = model.percent_change_24h + "% ";
        if (CheckRiseFall(model.percent_change_24h))
            view.percent_change_24h.color = Color.green;
        else
            view.percent_change_24h.color = Color.red;

        view.percent_change_7d.text = model.percent_change_7d + "% ";
        if (CheckRiseFall(model.percent_change_7d))
            view.percent_change_7d.color = Color.green;
        else
            view.percent_change_7d.color = Color.red;

                if (i < 10)
        {
            view.ID.sprite = ID[i];
            i++;
        }
        else
            view.ID.sprite = null; 
    }

    bool CheckRiseFall(string value)
    {
       float CheckValue;
        if (float.TryParse(value, out CheckValue) && CheckValue > 0)
            return true;       
        else
            return false;
    }

    IEnumerator GetItems(WWW www, System.Action<TestItemModel[]> callback)
    {
        yield return www;

        if (www.error == null)
        {
            TestItemModel[] mList = JsonHelper.getJsonArray<TestItemModel>(www.text);
            Debug.Log("WWW Success: " + www.text);
            callback(mList);
        }
        else
        {
            TestItemModel[] errList = new TestItemModel[1];
            errList[0] = new TestItemModel();
            errList[0].name = www.error;
            Debug.Log("WWW Error: " + www.error);
            callback(errList);
        }
    }

    public class TestItemView
    {
        public Text name;
        public Text symbol;
        public Text price_usd;
        public Text market_cap_usd;
        public Image ID;
        public Text percent_change_1h;
        public Text percent_change_24h;
        public Text percent_change_7d;

        public TestItemView(Transform rootView)
        {
            name = rootView.Find("name").GetComponent<Text>();
            price_usd =  rootView.Find("price_usd").GetComponent<Text>();
            symbol = rootView.Find("symbol").GetComponent<Text>();
            market_cap_usd = rootView.Find("market_cap_usd").GetComponent<Text>();
            percent_change_1h =rootView.Find("percent_change_1h").GetComponent<Text>();
            percent_change_24h= rootView.Find("percent_change_24h").GetComponent<Text>();
            percent_change_7d = rootView.Find("percent_change_7d").GetComponent<Text>();
            ID = rootView.Find("ID").GetComponent<Image>();
        }
    }

    [System.Serializable]
    public class TestItemModel
    {
        public string id;
        public string name;
        public string symbol;
        public string price_usd;
        public string market_cap_usd;
        public string percent_change_1h;
        public string percent_change_24h;
        public string percent_change_7d;
    }

    public class JsonHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        public static string arrayToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = array;
            return JsonUtility.ToJson(wrapper);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}