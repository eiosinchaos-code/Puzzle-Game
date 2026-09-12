using Unity.VisualScripting;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    Vector3 originalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    { 
        originalPosition = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Drag()
    {
        print("Dragging"); 
        //GameObject.Find(“Image”).transform.position = Input.mousePosition;
        gameObject.transform.position = Input.mousePosition;
    }
    public void Drop() 
    { 
        CheckMatch(); 
    }
    public void CheckMatch()
    {
        //GameObject ph1 = GameObject.Find (“PH1”); 
        //GameObject img = GameObject.Find (“image”);
        GameObject img = this.gameObject; string tag = this.gameObject.tag; 
        GameObject ph1 = GameObject.Find("PH" + tag); 
        float distance = Vector3.Distance(ph1.transform.position, img.transform.position); 
        print("Distance" + distance);
        if (distance <= 50)
            Snap(img, ph1);
        else 
            MoveBack();

    }
    public void MoveBack() 
    { 
        transform.position = originalPosition; 
    }
    public void Snap(GameObject img, GameObject ph) 
    { 
        img.transform.position = ph.transform.position; 
    }
    public void InitCardPosition() 
    { 
        originalPosition = transform.position;
    }
}
