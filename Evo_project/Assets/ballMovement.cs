using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballMovement : MonoBehaviour
{
    public UDPReceive udprecieve; // كائن لاستقبال البيانات
    public float scaleFactor = 0.01f; // مقياس لتحويل الإحداثيات

    void Start()
    {
        // تعيين موقع ابتدائي واضح للكرة
        // gameObject.transform.localPosition = new Vector3(0, 1, 0);
    }

    void Update()
    {
        // Receive data from Python
        string data = udprecieve.data;

        // Validate data
        if (string.IsNullOrEmpty(data) || !data.Contains(","))
        {
            Debug.Log("Invalid or missing data. Ball remains at the current position.");
            return;
        }

        try
        {
            // Clean up received data
            data = data.Trim(new char[] { '(', ')' });
            string[] info = data.Split(',');

            // Ensure data contains X, Y, and area
            if (info.Length == 3)
            {
                // Parse X, Y, and area
                float x = float.Parse(info[0]);
                float y = float.Parse(info[1]);
                float area = float.Parse(info[2]); // The area from Python

                // Calculate Z based on area
                float z = 1f / Mathf.Sqrt(area); // Estimate depth using area

                // Scale X, Y, and Z
                x *= scaleFactor;
                y *= scaleFactor;
                z *= scaleFactor;

                // Clamp X, Y, and Z values
                x = Mathf.Clamp(x, -5f, 5f);
                y = Mathf.Clamp(y, -5f, 5f);
                z = Mathf.Clamp(z, 0.1f, 5f); // Ensure Z is never zero or negative

                // Update ball position
                Vector3 targetPosition = new Vector3(x, y, z);
                gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, targetPosition, Time.deltaTime * 5f);

                Debug.Log($"Ball Position Updated: {targetPosition}");
            }
            else
            {
                Debug.LogWarning("Data does not contain the expected number of values.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing data: {e.Message}");
        }
    }

}
