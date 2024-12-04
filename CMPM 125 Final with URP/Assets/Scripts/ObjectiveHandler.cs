using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectiveOrder;
    [SerializeField] private TextMeshProUGUI objectiveUIText;
    //[SerializeField] private GameObject endGate;
    // Start is called before the first frame update
    void Start()
    {
        objectiveUIText.SetText("New Objective: " + objectiveOrder[0].name);// + objectiveOrder[0].name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void completeObjective(GameObject obj)
    {
        if (objectiveOrder[0] == obj)
        {
            // play player/object animation
            obj.SetActive(false);
            objectiveOrder.RemoveAt(0);
            objectiveUIText.SetText("Objective Complete!");
            StartCoroutine(WaitBeforeNext());
        }

        else
        {
            objectiveUIText.SetText("This is not the current objective");
            StartCoroutine(WaitBeforeNext());
        }
    }

    public IEnumerator WaitBeforeNext()
    {
        Debug.Log("reaching coroutine");
        yield return new WaitForSeconds(1.5f);
        if (objectiveOrder.Count > 0)
        {
            objectiveUIText.SetText("New Objective: " + objectiveOrder[0].name);
        }

        else
        {
            //open end gate
            objectiveUIText.SetText("All Objectives Completed, Escape");
        }
    }
}
