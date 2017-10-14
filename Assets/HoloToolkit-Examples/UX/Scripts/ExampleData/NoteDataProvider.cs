using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class NoteDataProvider : MonoBehaviour
    {
        public InteractiveGroup TargetGroup;

        public InteractiveSet SourceSet;

        // some test data - imagine this comming from a web-service or some input menu
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
        /// modify the TargetGroup data
        /// </summary>
        public void NoteTypeButtonSelected()
        {
            if (SourceSet.SelectedIndices.Count == 0)
            {
                return;
            }
            int interactivePos = SourceSet.SelectedIndices[0];
            string title = SourceSet.Interactives[interactivePos].gameObject.GetComponent<LabelTheme>().Default;
            TargetGroup.Titles = Data[title];
            TargetGroup.UpdateData();
        }
    }
}
