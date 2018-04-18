// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// NoteDataProvider takes the selected Label text of the pressed 
    /// Interactive and fills the TargetGroup with data accordingly. You might
    /// want to have some more complex logic and implement your own Data 
    /// Provider.
    /// </summary>
    public class NoteDataProvider : MonoBehaviour
    {
        public InteractiveGroup TargetGroup;

        public InteractiveSet SourceSet;

        /// <summary>
        /// some test data - imagine this coming from a web-service 
        /// or some input menu
        /// </summary>
        public Dictionary<string, List<string>> Data = new Dictionary<string, List<string>>
        {
            {
                "Normal",
                new List<string>(){
                    "Please contact me.",
                    "Please check\nthis component.",
                    "Please provide\nmore information."
                }
            },
            {
                "Warning", new List<string>(){
                    "Component needs to\nbe repaired.",
                    "Component needs to\nbe replaced.",
                    "Please provide\nmore information.",
                    "Wear a helmet",
                    "Use a mask",
                    "Wear boots",
                    "Take care:\nfire hazard.",
                    "Take care:\nchemicals."
                }
            }
        };

        /// <summary>
        /// called when clicked on one of the buttons at the SourceSet to 
        /// modify the TargetGroup data according to the selected button 
        /// (InteractiveToggle) text
        /// </summary>
        public void NoteTypeButtonSelected()
        {
            if (SourceSet.SelectedIndices.Count == 0)
            {
                // No item selected, this is only possible when your SourceSet
                // selection type is set to "Multiple".
                return;
            }

            // In this example we are only interested in the first selected
            // item. If you allow multiple selection for the SourceSet you
            // might want to have some more logic here
            int interactivePos = SourceSet.SelectedIndices[0];
            InteractiveToggle toggle = SourceSet.Interactives[interactivePos];

            LabelTheme label = toggle.gameObject.GetComponent<LabelTheme>();
            if (label == null)
            {
                // The selected InteractiveToggle does not have a LabelTheme 
                // attached, so it can not be determined what text blocks 
                // should be shown in the InteractiveGroup.
                Debug.LogWarning("Please attach a LabelTheme to the " +
                    "InteractiveToggle named \"" + toggle.gameObject.name + "\""); 
            }
            else
            { 
                TargetGroup.Titles = Data[label.Default];
                TargetGroup.UpdateData();
            }
        }

        /// <summary>
        /// The user pressed the send button. 
        /// If something is selected it will be logged.
        /// </summary>
        public void SendNote()
        {
            InteractiveSet interactiveSet = TargetGroup.GetInteractiveSet();
            foreach (int index in interactiveSet.SelectedIndices)
            {
                Debug.Log("Send new note: " + TargetGroup.Titles[index].Replace("\n", " "));
            }
            if (interactiveSet.SelectedIndices.Count == 0)
            {
                Debug.Log("Please select a note.");
            }
        }
    }
}
