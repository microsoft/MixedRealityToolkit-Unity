// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Simple data source that generates a large list of randomly generated entries.
    /// This can be used to test paging and virtualization.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Data Binding/Data Source Dynamic List")]
    public class DataSourceDynamicList : DataSourceGOBase
    {
        [Tooltip("The number of random image entries to generate for image collection.")]
        [SerializeField]
        protected int collectionSize = 1000;

        [Tooltip("URL template for fetching images.")]
        [SerializeField]
        protected string imageUrlTemplate;

        protected int maxImageId = 101;     // # of images actually available from "001" to "101"
        protected int _counter = 0;

        public override IDataSource AllocateDataSource()
        {
            return new DataSourceObjects();
        }

        private void Update()
        {
            _counter++;
        }

        protected override void InitializeDataSource()
        {
            // Enable either the dynamic or static versions of
            // the same data set structure.

            InitializeDataSourceDynamic();

            // Alternative method of populating the data source
            // InitializeDataSourceStatic();
        }

        protected void InitializeDataSourceDynamic()
        {
            string[] dates = { "February 26, 2020", "January 1, 2000", "March 1, 2018", "April 29, 2017", "May 5, 1999", "June 21, 1980" };
            string[] words = { "mixed reality", "MRTK", "HoloLens", "Unity", "Visual Studio", "Middleware", "Microsoft", "XR", "VR", "AR", "MR", "C#", "Azure" };

            DataChangeSetBegin();

            for (int i = 0; i < collectionSize; i++)
            {
                int imageId = 1 + (i % maxImageId);
                string pathBase = string.Format("images[{0:d}]", i);
                string id = string.Format("{0:D6}", i);
                string title = string.Format("Image #{0:d}", i);
                string url = string.Format(imageUrlTemplate, imageId);

                Randomize(words);
                string description = "";
                for (int w = 0; w < 5; w++)
                {
                    description = description + words[w] + " ";
                }

                SetValue(pathBase + ".id", id);
                SetValue(pathBase + ".title", title);
                SetValue(pathBase + ".description", description);
                SetValue(pathBase + ".imageUrl", url);
            }

            DataChangeSetEnd();
            NotifyAllChanged();
        }

        protected void Randomize(string[] items)
        {
            System.Random rand = new System.Random();

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                (items[j], items[i]) = (items[i], items[j]);
            }
        }

        /// <summary>
        /// Alternative static list of entries of the same data structure as the
        /// randomly generated entries of InitializeDataSourceDynamic().
        /// </summary>
        protected void InitializeDataSourceStatic()
        {
            /****
             *  Equivalent JSON to the programmatically generated data set.
             *
             * { "images" : [
                    { "id" : "0001", "imageUrl" : string.Format(imageUrlTemplate, 1),
                      "title" : "Stone Mountain and lone tree",
                        "description" : "Taken while rock climbing on Stone Mountain in NC."
                    },
                    { "id" : "0002", "imageUrl" : string.Format(imageUrlTemplate, 2),
                      "title" : "Brain Celosia flower",
                        "description" : "Found this amazing flower in NC."
                    }
               ]}
            ****/
            DataChangeSetBegin();

            SetValue("images[0].id", "0001");
            SetValue("images[0].title", "Stone Mountain and lone tree");
            SetValue("images[0].description", "Taken while rock climbing on Stone Mountain in NC.");
            SetValue("images[0].imageUrl", string.Format(imageUrlTemplate, 1));

            SetValue("images[1].id", "0002");
            SetValue("images[1].title", "Brain Celosia flower");
            SetValue("images[1].description", "Found this amazing flower in NC.");
            SetValue("images[1].imageUrl", string.Format(imageUrlTemplate, 2));

            DataChangeSetEnd();
        }
    }
}
