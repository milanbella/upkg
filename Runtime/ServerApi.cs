using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerApi : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void CallApi() {
        Debug.Log("called ServerApi.CallApi()");
    }

    public void CallApiUser() {
        Debug.Log("called ServerApi.CallApi()");
        StartCoroutine(this.ApiUser());
    }

    public IEnumerator ApiUser() {
        Debug.Log("called ServerApi.CallApi()");
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/api/user");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
        }
    }
}
