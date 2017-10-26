
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class VoiceController : MonoBehaviour
{ 
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();



    void Start()
    {
        keywords.Add("New Event", () =>
        {
            // Call the OnNewEvent method to visualize next event
            Debug.Log("You just said New Event, rendering...");
            BroadcastMessage("OnNewEvent");
        });

        keywords.Add("Old Event", () =>
        {
            // Call the OnOldEvent method to visualize previous event
            Debug.Log("You just said Old Event, rendering...");
            BroadcastMessage("OnOldEvent");
        });

        keywords.Add("No Event", () =>
        {
            // Call the OnNoEvent method to visualize full detector
            Debug.Log("You just said No Event, unrendering...");
            BroadcastMessage("OnNoEvent");
        });

        keywords.Add("Control Room", () =>
        {
            // Call the OnControlRoom method to teleport
            Debug.Log("You just said Control Room, teleporting...");
            BroadcastMessage("OnControlRoom");
        });

        keywords.Add("Cavern", () =>
        {
            // Call the OnCavern method to teleport
            Debug.Log("You just said Cavern, teleporting...");
            BroadcastMessage("OnCavern");
        });

        keywords.Add("Surface", () =>
        {
            // Call the OnSurface method to teleport
            Debug.Log("You just said Surface, teleporting...");
            BroadcastMessage("OnSurface");
        });

        keywords.Add("Far Side", () =>
        {
            // Call the OnFarSide method to teleport
            Debug.Log("You just said Far Side, teleporting...");
            BroadcastMessage("OnFarSide");
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();

    } // end Start

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    } // end OnPhraseRecognized

} // end VoiceController
